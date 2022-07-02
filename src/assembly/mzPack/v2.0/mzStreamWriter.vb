Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem

Public Module mzStreamWriter

    <Extension>
    Public Function WriteStream(mzpack As mzPack, file As Stream, Optional meta_size As Long = 4 * 1024 * 1024) As Boolean
        Using pack As New StreamPack(file)
            Call pack.Clear(meta_size)
            Call mzpack.WriteStream(pack)
            Call pack.WriteText(mzpack.readme, "readme.txt")
        End Using

        Return True
    End Function

    <Extension>
    Private Function readme(mzpack As mzPack) As String
        Dim sb As New StringBuilder
        Dim app = mzpack.Application

        Call sb.AppendLine($"mzPack data v2.0")
        Call sb.AppendLine($"for MZKit application {app.ToString}({app.Description}) data analysis.")

        Return sb.ToString
    End Function

    <Extension>
    Private Sub WriteStream(mzpack As mzPack, pack As StreamPack)
        Call pack.WriteText(mzpack.Application.ToString, ".etc/app.cls")

        For Each ms1 In mzpack.MS
            Using scan1 As New BinaryDataWriter(pack.OpenBlock($"/MS/{ms1.scan_id}/Scan1.dat"))
                Call ms1.WriteScan1(scan1)
            End Using

        Next
    End Sub
End Module
