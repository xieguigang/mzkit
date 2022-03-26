Imports System.Threading
Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Parallel

Public Module HeartBeat

    Dim socket As TcpServicesSocket

    Public Function Start() As Integer
        If socket Is Nothing OrElse Not socket.Running Then
            socket = New TcpServicesSocket(localPort:=TCPExtensions.GetFirstAvailablePort(5000))
            socket.ResponseHandler = AddressOf HeartBeatRequestHandler

            Call New Thread(AddressOf socket.Run).Start()
        End If

        Return socket.LocalPort
    End Function

    Private Function HeartBeatRequestHandler(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Return New DataPipe("OK!")
    End Function
End Module
