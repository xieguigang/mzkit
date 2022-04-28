Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.IO.MessagePack

Namespace MZWork

    Public Module SaveArchive

        <Extension>
        Public Sub ExportWorkspace(caches As Dictionary(Of String, Raw()), pack As ZipArchive, msg As Action(Of String))
            ' /
            ' +--- mzpack (raw data files)
            ' +--- meta (meta data of the raw data file)
            ' +--- src (R# automation script files)
            For Each rawfile As KeyValuePair(Of String, Raw()) In caches
                Call rawfile.packRawGroup(pack, msg)
            Next
        End Sub

        <Extension>
        Private Sub writeRawBuffer(rawfiles As IEnumerable(Of Raw), pack As ZipArchive)
            For Each cache As Raw In rawfiles
                ' write cache data file
                Using buffer As Stream = cache.cache.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                    Dim container = pack.CreateEntry($"mzpack/{cache.cache.BaseName}").Open

                    Call buffer.CopyTo(container)
                    Call container.Flush()
                    Call container.Close()
                    Call container.Dispose()
                End Using
            Next
        End Sub

        Private Function niceKey(nameKey As String) As String
            If nameKey.Contains(vbCr) OrElse nameKey.Contains(vbLf) Then
                nameKey = nameKey.LineTokens.Last

                If nameKey.Length > 24 Then
                    nameKey = "..." & nameKey.Substring(nameKey.Length - 21)
                End If
            End If

            Return nameKey
        End Function

        <Extension>
        Private Sub packRawGroup(rawfile As KeyValuePair(Of String, Raw()), pack As ZipArchive, msg As Action(Of String))
            ' save message pack
            Dim nameKey As String = niceKey(rawfile.Key)

            Call msg($"pack raw [{nameKey}]...")
            Call rawfile.Value.writeRawBuffer(pack)

            ' save raw meta
            Dim meta As Raw() = rawfile.Value _
                .Select(Function(f)
                            Return New Raw With {
                                .cache = f.cache.BaseName,
                                .numOfScan1 = f.numOfScan1,
                                .numOfScan2 = f.numOfScan2,
                                .rtmax = f.rtmax,
                                .rtmin = f.rtmin,
                                .source = f.source.FileName
                            }
                        End Function) _
                .ToArray

            Dim metapack = pack.CreateEntry($"meta/{nameKey}.pack").Open

            Call MsgPackSerializer.SerializeObject(meta, metapack)
            Call metapack.Flush()
            Call metapack.Close()
            Call metapack.Dispose()
        End Sub
    End Module
End Namespace