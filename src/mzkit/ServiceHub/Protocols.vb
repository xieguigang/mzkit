Imports System.Threading
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Task

Public Module Protocols

    Public Function StartServer(Rscript As String) As RunSlavePipeline
        Dim cli As String = Rscript
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Rscript.Path, $"""{cli}""")

        Call pipeline.CommandLine.__DEBUG_ECHO

        ' AddHandler pipeline.SetProgress, AddressOf Progress.SetProgress
        ' AddHandler pipeline.Finish, Sub() Progress.Invoke(Sub() Progress.Close())

        Call New Thread(AddressOf pipeline.Run).Start()

        Return pipeline
    End Function
End Module
