Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Parallel
Imports mzkit.Tcp
Imports ServiceHub
Imports Task
Imports mzkit.My
Imports Microsoft.VisualBasic.Linq
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging

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

    End Function

    Public Function LoadPixels(mz As IEnumerable(Of Double), mzErr As Tolerance) As PixelData()

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

    Public Function GetPixel(x As Integer, y As Integer, w As Integer, h As Integer) As PixelScan()

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

    Public Sub CloseMSIEngine()
        If MSI_service > 0 Then
            Dim request As New RequestStream(MSI.Protocol, ServiceProtocol.ExitApp, Encoding.UTF8.GetBytes("shut down!"))
            MSI_pipe = Nothing
            MSI_service = -1
        End If
    End Sub

    Private Sub MSI_pipe_SetMessage(message As String) Handles MSI_pipe.SetMessage
        Call MessageCallback(message)
    End Sub
End Module
