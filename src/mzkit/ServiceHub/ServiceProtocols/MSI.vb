Imports System.IO
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
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
Imports Microsoft.VisualBasic.Net.Protocols.Reflection
Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Task

<Protocol(GetType(ServiceProtocol))>
Public Class MSI : Implements ITaskDriver

    Public Shared ReadOnly Property Protocol As Long = New ProtocolAttribute(GetType(ServiceProtocol)).EntryPoint

    Dim socket As TcpServicesSocket
    Dim MSI As Drawer

    Public ReadOnly Property TcpPort As Integer
        Get
            Return socket.LocalPort
        End Get
    End Property

    Sub New()
        Me.socket = New TcpServicesSocket(GetFirstAvailablePort())
        Me.socket.ResponseHandler = AddressOf New ProtocolHandler(Me).HandleRequest

        Call RunSlavePipeline.SendMessage($"socket={TcpPort}")
    End Sub

    Public Function Run() As Integer Implements ITaskDriver.Run
        Return socket.Run
    End Function

    <Protocol(ServiceProtocol.LoadMSI)>
    Public Function Load(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim filepath As String = request.GetString(Encoding.UTF8)

        If filepath.ExtensionSuffix("cdf") Then
            Using cdf As New netCDFReader(filepath)
                MSI = New Drawer(cdf.CreatePixelReader)
            End Using
        Else
            Dim mzpack As mzPack

            Using file As FileStream = filepath.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                mzpack = mzPack.ReadAll(file, ignoreThumbnail:=True)
                MSI = New Drawer(mzpack)
            End Using
        End If

        Dim info As Dictionary(Of String, String) = MsImageProperty.GetMSIInfo(MSI)

        Return New DataPipe(info.GetJson(indent:=False, simpleDict:=True))
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
        Dim cache As New InMemoryVectorPixel(pixel)

        Return New DataPipe(cache.GetBuffer)
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

        Using buffer As Stream = filename.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Call New mzPack With {
                .MS = DirectCast(MSI.pixelReader, ReadRawPack) _
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
End Class

