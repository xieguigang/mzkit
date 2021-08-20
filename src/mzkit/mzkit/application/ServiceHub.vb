Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Parallel
Imports mzkit.Tcp
Imports ServiceHub
Imports Task
Imports mzkit.My

Module ServiceHub

    Dim MSI_pipe As RunSlavePipeline
    Dim MSI_service As Integer = -1

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
    Public Sub LoadMSI(raw As String)
        Dim request As New RequestStream(MSI.Protocol, ServiceProtocol.LoadMSI, Encoding.UTF8.GetBytes(raw))

        If MSI_service <= 0 Then
            Call MyApplication.host.showStatusMessage("MS-imaging services is not started yet!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Call New TcpRequest("localhost", MSI_service).SendMessage(request)
        End If
    End Sub

    Public Sub CloseMSIEngine()
        If MSI_service > 0 Then
            Dim request As New RequestStream(MSI.Protocol, ServiceProtocol.ExitApp, Encoding.UTF8.GetBytes("shut down!"))
            MSI_pipe = Nothing
            MSI_service = -1
        End If
    End Sub

End Module
