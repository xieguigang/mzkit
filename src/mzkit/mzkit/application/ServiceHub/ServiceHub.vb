#Region "Microsoft.VisualBasic::2126d619ebad7dc3f71b7e8a472efe16, mzkit\src\mzkit\mzkit\application\ServiceHub.vb"

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

    '   Total Lines: 224
    '    Code Lines: 163
    ' Comment Lines: 19
    '   Blank Lines: 42
    '     File Size: 8.88 KB


    ' Module ServiceHub
    ' 
    '     Properties: MSIEngineRunning
    ' 
    '     Function: DoIonStats, (+2 Overloads) GetPixel, handleServiceRequest, LoadBasePeakMzList, (+2 Overloads) LoadMSI
    '               LoadPixels, LoadSummaryLayer
    ' 
    '     Sub: CloseMSIEngine, ExportMzpack, MSI_pipe_SetMessage, MSI_pipe_SetProgress, StartMSIService
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.mzkit_win32.My
Imports BioNovoGene.mzkit_win32.Tcp
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.Net.HTTP
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports ServiceHub
Imports Task

Module ServiceHub

    Dim WithEvents MSI_pipe As RunSlavePipeline
    Dim MSI_service As Integer = -1

    Public ReadOnly Property MSIEngineRunning As Boolean
        Get
            Return MSI_service > 0
        End Get
    End Property

    Public ReadOnly Property appPort As Integer
        Get
            Return MSI_service
        End Get
    End Property

    Public MessageCallback As Action(Of String)

    ''' <summary>
    ''' --debug --port=33361
    ''' </summary>
    Public debugPort As Integer?

    ''' <summary>
    ''' this method will close the engine at first
    ''' </summary>
    Public Sub StartMSIService()
        Call CloseMSIEngine()

        MSI_pipe = Global.ServiceHub.Protocols.StartServer(RscriptPipelineTask.GetRScript("ServiceHub/MSI-host.R"), MSI_service, debugPort) ', HeartBeat.Start)

        ' hook message event handler
        AddHandler MSI_pipe.SetMessage, AddressOf MSI_pipe_SetMessage

        If MSI_service <= 0 Then
            Call MyApplication.host.showStatusMessage("MS-Imaging service can not start!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Call $"MS-Imaging service started!({MSI_service})".__DEBUG_ECHO
        End If

        MessageCallback = Nothing
    End Sub

    Public Function DoIonStats() As IonStat()
        Dim data As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.GetIonStatList, "read"))

        If data Is Nothing Then
            Return {}
        Else
            Dim ions = BSON.Load(data).CreateObject(Of IonStat())
            Return ions
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="raw">
    ''' filepath full name of the mzpack raw data file.
    ''' </param>
    Public Function LoadMSI(raw As String, dimSize As Size, message As Action(Of String)) As MsImageProperty
        MessageCallback = message

        Dim config As String = $"{dimSize.Width},{dimSize.Height}={raw}"
        Dim data As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.LoadThermoRawMSI, Encoding.UTF8.GetBytes(config)))
        Dim output As MsImageProperty = data _
            .GetString(Encoding.UTF8) _
            .LoadJSON(Of Dictionary(Of String, String)) _
            .DoCall(Function(info)
                        Return New MsImageProperty(info)
                    End Function)

        MessageCallback = Nothing

        Return output
    End Function

    Public Function LoadPixels(mz As IEnumerable(Of Double), mzErr As Tolerance) As PixelData()
        Return MSIProtocols.LoadPixels(mz, mzErr, AddressOf handleServiceRequest)
    End Function

    Public Function CutBackground() As MsImageProperty
        Dim data As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.CutBackground, Encoding.UTF8.GetBytes("1")))
        Dim output As MsImageProperty = data _
            .GetString(Encoding.UTF8) _
            .LoadJSON(Of Dictionary(Of String, String)) _
            .DoCall(Function(info)
                        Return New MsImageProperty(info)
                    End Function)

        Return output
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="raw">
    ''' filepath full name of the mzpack raw data file.
    ''' </param>
    Public Function LoadMSI(raw As String, message As Action(Of String)) As MsImageProperty
        MessageCallback = message

        Dim data As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.LoadMSI, Encoding.UTF8.GetBytes(raw)))

        MessageCallback = Nothing

        If data Is Nothing Then
            Call MyApplication.host.warning($"Failure to load MS-imaging raw data file: {raw}...")
            Return Nothing
        End If

        Dim output As MsImageProperty = data _
            .GetString(Encoding.UTF8) _
            .LoadJSON(Of Dictionary(Of String, String)) _
            .DoCall(Function(info)
                        Return New MsImageProperty(info)
                    End Function)

        Return output
    End Function

    Private Function handleServiceRequest(request As RequestStream) As RequestStream
        If MSI_service <= 0 Then
            Call MyApplication.host.showStatusMessage("MS-imaging services is not started yet!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return Nothing
        Else
            Return New TcpRequest("localhost", MSI_service).SendMessage(request)
        End If
    End Function

    Public Sub ExportMzpack(savefile As String)
        Call handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.ExportMzpack, Encoding.UTF8.GetBytes(savefile)))
    End Sub

    Public Function GetPixel(x As Integer, y As Integer, w As Integer, h As Integer) As InMemoryVectorPixel()
        Dim xy As Byte() = {x, y, w, h}.Select(AddressOf BitConverter.GetBytes).IteratesALL.ToArray
        Dim output As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.GetPixelRectangle, xy))

        If output Is Nothing Then
            Return Nothing
        Else
            Return InMemoryVectorPixel.ParseVector(output.ChunkBuffer).ToArray
        End If
    End Function

    Public Function GetPixel(x As Integer, y As Integer) As PixelScan
        Dim xy As Byte() = BitConverter.GetBytes(x).JoinIterates(BitConverter.GetBytes(y)).ToArray
        Dim output As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.GetPixel, xy))

        If output Is Nothing Then
            Return Nothing
        ElseIf HTTP_RFC.RFC_OK <> output.Protocol AndAlso output.Protocol <> 0 Then
            Call MyApplication.host.showStatusMessage("MSI service backend panic.", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return Nothing
        Else
            Return InMemoryVectorPixel.Parse(output.ChunkBuffer)
        End If
    End Function

    Public Function LoadBasePeakMzList() As Double()
        Dim data As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.GetBasePeakMzList, {}))

        If data Is Nothing Then
            Return {}
        Else
            Return data.GetDoubles
        End If
    End Function

    Public Function LoadSummaryLayer(summary As IntensitySummary) As PixelScanIntensity()
        Dim data As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.LoadSummaryLayer, BitConverter.GetBytes(CInt(summary))))

        If data Is Nothing Then
            Return {}
        Else
            Return PixelScanIntensity.Parse(data.ChunkBuffer)
        End If
    End Function

    Public Sub CloseMSIEngine()
        If MSI_service > 0 Then
            Dim request As New RequestStream(MSI.Protocol, ServiceProtocol.ExitApp, Encoding.UTF8.GetBytes("shut down!"))

            Call handleServiceRequest(request)

            ' detach message event handler
            RemoveHandler MSI_pipe.SetMessage, AddressOf MSI_pipe_SetMessage

            MSI_pipe = Nothing
            MSI_service = -1
        End If
    End Sub

    Private Sub MSI_pipe_SetMessage(message As String) Handles MSI_pipe.SetMessage
        Call Application.DoEvents()

        If MessageCallback Is Nothing Then
            Call MyApplication.LogText(message)
        Else
            Call MessageCallback(message)
        End If
    End Sub

    Private Sub MSI_pipe_SetProgress(percentage As Integer, details As String) Handles MSI_pipe.SetProgress
        Call Application.DoEvents()

        If MessageCallback Is Nothing Then
            Call MyApplication.LogText(details)
        Else
            Call MessageCallback(details)
        End If
    End Sub
End Module
