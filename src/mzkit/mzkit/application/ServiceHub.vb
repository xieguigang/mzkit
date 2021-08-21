Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports mzkit.My
Imports mzkit.Tcp
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

    Public MessageCallback As Action(Of String)

    Public Sub StartMSIService()
        Call CloseMSIEngine()

        MSI_pipe = Global.ServiceHub.Protocols.StartServer(RscriptPipelineTask.GetRScript("ServiceHub/MSI-host.R"), MSI_service)

        If MSI_service <= 0 Then
            Call MyApplication.host.showStatusMessage("MS-Imaging service can not start!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Call $"MS-Imaging service started!({MSI_service})".__DEBUG_ECHO
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="raw">
    ''' filepath full name of the mzpack raw data file.
    ''' </param>
    Public Function LoadMSI(raw As String, dimSize As Size) As MsImageProperty
        Dim config As String = $"{dimSize.Width},{dimSize.Height}={raw}"
        Dim data As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.LoadThermoRawMSI, Encoding.UTF8.GetBytes(config)))
        Dim output As MsImageProperty = data _
            .GetString(Encoding.UTF8) _
            .LoadJSON(Of Dictionary(Of String, String)) _
            .DoCall(Function(info)
                        Return New MsImageProperty(info)
                    End Function)

        Return output
    End Function

    Public Function LoadPixels(mz As IEnumerable(Of Double), mzErr As Tolerance) As PixelData()
        Dim config As New LayerLoader With {.mz = mz.ToArray, .method = If(TypeOf mzErr Is PPMmethod, "ppm", "da"), .mzErr = mzErr.DeltaTolerance}
        Dim configBytes As Byte() = BSON.GetBuffer(config.GetType.GetJsonElement(config, New JSONSerializerOptions)).ToArray
        Dim data As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.LoadMSILayers, configBytes))

        If data Is Nothing Then
            Return {}
        Else
            Return PixelData.Parse(data.ChunkBuffer)
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="raw">
    ''' filepath full name of the mzpack raw data file.
    ''' </param>
    Public Function LoadMSI(raw As String) As MsImageProperty
        Dim data As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.LoadMSI, Encoding.UTF8.GetBytes(raw)))
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

    Public Sub ExportMzpack(fileName As String)
        Call handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.ExportMzpack, Encoding.UTF8.GetBytes(fileName)))
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
            MSI_pipe = Nothing
            MSI_service = -1
        End If
    End Sub

    Private Sub MSI_pipe_SetMessage(message As String) Handles MSI_pipe.SetMessage
        If MessageCallback Is Nothing Then
            Call message.__DEBUG_ECHO
        Else
            Call MessageCallback(message)
        End If
    End Sub
End Module
