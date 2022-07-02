Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem

Public Module mzStreamWriter

    <Extension>
    Public Function WriteStream(mzpack As mzPack, file As Stream, Optional meta_size As Long = 4 * 1024 * 1024) As Boolean
        Using pack As New StreamPack(file)
            Call pack.Clear(meta_size)
            Call mzpack.WriteStream(pack)
        End Using

        Return True
    End Function

    <Extension>
    Private Sub WriteStream(mzpack As mzPack, pack As StreamPack)
        Call pack.WriteText(mzpack.Application.ToString, ".etc/app.cls")
    End Sub
End Module
