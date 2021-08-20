Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Task

Module ServiceHub

    Dim services As RunSlavePipeline

    Public Sub Start()
        services = Global.ServiceHub.Protocols.StartServer(RscriptPipelineTask.GetRScript("ServiceHub/MSI-host.R"))
    End Sub

End Module
