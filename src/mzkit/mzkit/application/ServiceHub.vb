Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Parallel
Imports mzkit.Tcp
Imports ServiceHub
Imports Task

Module ServiceHub

    Dim MSI_pipe As RunSlavePipeline
    Dim MSI_service As Integer

    Public Sub StartMSIService()
        MSI_pipe = Global.ServiceHub.Protocols.StartServer(RscriptPipelineTask.GetRScript("ServiceHub/MSI-host.R"), MSI_service)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="raw">
    ''' filepath full name of the mzpack raw data file.
    ''' </param>
    Public Sub LoadMSI(raw As String)
        Dim request As New RequestStream(MSI.Protocol, ServiceProtocol.LoadMSI, Encoding.UTF8.GetBytes(raw))
        Dim resp = New TcpRequest("localhost", MSI_service).SendMessage(request)
    End Sub

End Module
