#Region "Microsoft.VisualBasic::2f4ab02400f56a33bf9994134c39168b, mzkit\src\mzkit\ServiceHub\ServiceProtocols\MSI.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 221
    '    Code Lines: 164
    ' Comment Lines: 18
    '   Blank Lines: 39
    '     File Size: 9.27 KB


    ' Class MSI
    ' 
    '     Properties: Protocol, TcpPort
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ExportMzPack, GetBPCIons, GetIonStatList, GetMSILayers, GetPixel
    '               GetPixelRectangle, Load, LoadSummaryLayer, Quit, Run
    '               Unload
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Microsoft.VisualBasic.Net.Protocols.Reflection
Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Task

<Protocol(GetType(ServiceProtocol))>
Public Class MSI : Implements ITaskDriver, IDisposable

    Public Shared ReadOnly Property Protocol As Long = New ProtocolAttribute(GetType(ServiceProtocol)).EntryPoint

    Dim socket As TcpServicesSocket
    Dim MSI As Drawer

    Private disposedValue As Boolean

    Public ReadOnly Property TcpPort As Integer
        Get
            Return socket.LocalPort
        End Get
    End Property

    Sub New(Optional debugPort As Integer? = Nothing)
        Dim port As Integer = If(debugPort Is Nothing, GetFirstAvailablePort(), debugPort)

        Me.socket = New TcpServicesSocket(port, debug:=Not debugPort Is Nothing)
        Me.socket.ResponseHandler = AddressOf New ProtocolHandler(Me, debug:=Not debugPort Is Nothing).HandleRequest

        Call RunSlavePipeline.SendMessage($"socket={TcpPort}")
        Call HeartBeat.Register(Me)
    End Sub

    Public Function Run() As Integer Implements ITaskDriver.Run
        Return socket.Run
    End Function

    <Protocol(ServiceProtocol.LoadMSI)>
    Public Function Load(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim filepath As String = request.GetString(Encoding.UTF8)
        Dim info As Dictionary(Of String, String)

        If filepath.ExtensionSuffix("cdf") Then
            Using cdf As New netCDFReader(filepath)
                MSI = New Drawer(cdf.CreatePixelReader)
            End Using
        Else
            Dim mzpack As mzPack

            Using file As Stream = filepath.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                mzpack = mzPack.ReadAll(file, ignoreThumbnail:=True).ScanMeltdown(gridSize:=10)
                MSI = New Drawer(mzpack)
            End Using
        End If

        info = MsImageProperty.GetMSIInfo(MSI)

        Return New DataPipe(info.GetJson(indent:=False, simpleDict:=True))
    End Function

    <Protocol(ServiceProtocol.CutBackground)>
    Public Function CutBackground(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim allPixels As PixelScan() = MSI.pixelReader.AllPixels.ToArray
        Dim intensity As Double() = allPixels.Select(Function(d) d.GetMzIonIntensity).IteratesALL.ToArray
        Dim q As Double = TrIQThreshold.TrIQThreshold(intensity, 0.7)
        Dim cut As Double = intensity.Max * q
        Dim info As Dictionary(Of String, String)

        allPixels = allPixels _
            .Where(Function(i)
                       Return i.GetMzIonIntensity.Max <= cut
                   End Function) _
            .ToArray

        MSI = New Drawer(allPixels.CreatePixelReader)
        info = MsImageProperty.GetMSIInfo(MSI)

        Return New DataPipe(info.GetJson(indent:=False, simpleDict:=True))
    End Function

    <Protocol(ServiceProtocol.GetIonStatList)>
    Public Function GetIonStatList(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim allPixels As PixelScan() = MSI.pixelReader.AllPixels.ToArray
        Dim ions As IonStat() = IonStat.DoStat(allPixels).ToArray
        Dim json As JsonElement = ions.GetType.GetJsonElement(ions, New JSONSerializerOptions With {.indent = False})

        Return New DataPipe(BSON.BSONFormat.SafeGetBuffer(json))
    End Function

    ''' <summary>
    ''' get ms data of a given pixel point
    ''' </summary>
    ''' <param name="request"></param>
    ''' <param name="remoteAddress"></param>
    ''' <returns></returns>
    <Protocol(ServiceProtocol.GetPixel)>
    Public Function GetPixel(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim xy As Integer() = request.GetIntegers
        Dim pixel As PixelScan = MSI.pixelReader.GetPixel(xy(0), xy(1))

        Call RunSlavePipeline.SendMessage($"read [{xy(0)},{xy(1)}]")

        If pixel Is Nothing Then
            Call RunSlavePipeline.SendMessage($"but missing pixel data at [{xy(0)},{xy(1)}]!")
            Return New DataPipe(New Byte() {})
        Else
            Return New DataPipe(New InMemoryVectorPixel(pixel).GetBuffer)
        End If
    End Function

    ''' <summary>
    ''' get ms data of a given rectangle region
    ''' </summary>
    ''' <param name="request"></param>
    ''' <param name="remoteAddress"></param>
    ''' <returns></returns>
    <Protocol(ServiceProtocol.GetPixelRectangle)>
    Public Function GetPixelRectangle(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim rect As Integer() = request.GetIntegers
        Dim pixels As InMemoryVectorPixel() = MSI.pixelReader _
            .GetPixel(rect(0), rect(1), rect(2), rect(3)) _
            .Select(Function(p)
                        Return New InMemoryVectorPixel(p)
                    End Function) _
            .ToArray

        Return New DataPipe(InMemoryVectorPixel.GetBuffer(pixels))
    End Function

    <Protocol(ServiceProtocol.ExportMzpack)>
    Public Function ExportMzPack(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim filename As String = request.GetString(Encoding.UTF8)
        Dim reader = MSI.pixelReader

        Using buffer As Stream = filename.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Call New mzPack With {
                .MS = DirectCast(reader, ReadRawPack) _
                    .GetScans _
                    .ToArray
            }.Write(buffer, progress:=AddressOf RunSlavePipeline.SendMessage)
        End Using

        Return New DataPipe(Encoding.UTF8.GetBytes("OK!"))
    End Function

    <Protocol(ServiceProtocol.UnloadMSI)>
    Public Function Unload(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        MSI.Dispose()
        MSI.Free

        Return New DataPipe(Encoding.UTF8.GetBytes("OK!"))
    End Function

    <Protocol(ServiceProtocol.ExitApp)>
    Public Function Quit(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Call socket.Dispose()
        Return New DataPipe(Encoding.UTF8.GetBytes("OK!"))
    End Function

    ''' <summary>
    ''' get multiple layers data of a given mz list
    ''' </summary>
    ''' <param name="request"></param>
    ''' <param name="remoteAddress"></param>
    ''' <returns></returns>
    <Protocol(ServiceProtocol.LoadMSILayers)>
    Public Function GetMSILayers(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim config As LayerLoader = BSON.Load(request.ChunkBuffer).CreateObject(Of LayerLoader)
        Dim layers As PixelData() = MSI.LoadPixels(config.mz, config.GetTolerance).ToArray

        layers = KnnInterpolation.KnnFill(layers, MSI.dimension, dx:=3, dy:=3)

        Return New DataPipe(PixelData.GetBuffer(layers))
    End Function

    <Protocol(ServiceProtocol.LoadSummaryLayer)>
    Public Function LoadSummaryLayer(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim summaryType As IntensitySummary = BitConverter.ToInt32(request.ChunkBuffer, Scan0)
        Dim summary As PixelScanIntensity() = MSI.pixelReader _
            .GetSummary _
            .GetLayer(summaryType) _
            .ToArray
        Dim byts As Byte() = PixelScanIntensity.GetBuffer(summary)

        Return New DataPipe(byts)
    End Function

    <Protocol(ServiceProtocol.GetBasePeakMzList)>
    Public Function GetBPCIons(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim data As New List(Of ms2)
        Dim pointTagged As New List(Of (X!, Y!, mz As ms2))

        For Each px As PixelScan In MSI.pixelReader.AllPixels
            Dim mz As ms2 = px.GetMs.OrderByDescending(Function(a) a.intensity).FirstOrDefault

            pointTagged.Add((px.X, px.Y, mz))

            If Not mz Is Nothing Then
                data.Add(mz)
            End If
        Next

        data = data.ToArray _
             .Centroid(Tolerance.PPM(20), New RelativeIntensityCutoff(0.01)) _
             .AsList

        Dim da As Tolerance = Tolerance.DeltaMass(0.1)
        Dim mzGroup = pointTagged _
            .GroupBy(Function(p) p.mz.mz, da) _
            .Select(Function(a)
                        Return (Val(a.name), a.ToArray)
                    End Function) _
            .ToArray

        Dim mzList As Double() = data _
            .OrderByDescending(Function(a)
                                   Return mzGroup _
                                       .Where(Function(i) da(i.Item1, a.mz)) _
                                       .Select(Function(p) p.ToArray) _
                                       .IteratesALL _
                                       .Count
                               End Function) _
            .Select(Function(m) m.mz) _
            .ToArray

        Return New DataPipe(mzList)
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call socket.Dispose()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
