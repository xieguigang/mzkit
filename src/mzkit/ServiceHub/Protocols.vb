Imports System.Threading
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Linq
Imports Task

Public Module Protocols

    Public Function StartServer(Rscript As String, ByRef service As Integer) As RunSlavePipeline
        Dim cli As String = Rscript
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Rscript.Path, $"""{cli}""")
        Dim tcpPort As Integer = -1

        Call pipeline.CommandLine.__DEBUG_ECHO

        AddHandler pipeline.SetMessage,
            Sub(msg)
                If msg.StartsWith("socket=") Then
                    tcpPort = msg.Match("\d+").DoCall(AddressOf Integer.Parse)
                Else
                    Call msg.__DEBUG_ECHO
                End If
            End Sub

        Call New Thread(AddressOf pipeline.Run).Start()

        For i As Integer = 0 To 1000
            service = tcpPort

            If service > 0 Then
                Exit For
            Else
                Thread.Sleep(500)
            End If
        Next

        Return pipeline
    End Function
End Module
