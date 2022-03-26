Imports System.Threading
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Net.Tcp

Module HeartBeat

    Dim port As Integer
    Dim service As IDisposable
    Dim thread As New ThreadStart(AddressOf HeartBeat.Listen)

    Public Sub Register(Of T As {ITaskDriver, IDisposable})(service As T)
        HeartBeat.service = service
    End Sub

    Public Sub Start(master As Integer)
        Dim thread As New Thread(HeartBeat.thread)

        thread.Start()
        HeartBeat.port = master
    End Sub

    Private Sub Listen()
        Call Threading.Thread.Sleep(5 * 1000)

        Do While True
            Dim req = New TcpRequest(port).SendMessage("check")

            If Not req = "OK!" Then
                Exit Do
            Else
                Call Threading.Thread.Sleep(1000)
            End If
        Loop

        Call service.Dispose()
    End Sub
End Module
