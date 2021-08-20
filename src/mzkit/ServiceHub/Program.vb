Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("app")>
Module Program



    <ExportAPI("run")>
    Public Sub Main(port As Integer)

    End Sub

    Private Function callback(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Using bytes As New MemoryStream(request.ChunkBuffer)
            Dim data As IPCBuffer = IPCBuffer.ParseBuffer(bytes)

            Call $"accept callback data: {data.ToString}".__DEBUG_ECHO
            Call processor.SaveResponse(data.requestId, data.buffer)

            Return New DataPipe(NetResponse.RFC_OK)
        End Using
    End Function
End Module
