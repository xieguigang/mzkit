Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.IO.MessagePack

Namespace MZWork

    Public Module ParseArchive

        ''' <summary>
        ''' export and save mzpack cache files to system tempdir
        ''' </summary>
        ''' <param name="zip"></param>
        ''' <param name="msg"></param>
        ''' <returns>
        ''' returns the metadata collection
        ''' </returns>
        <Extension>
        Public Iterator Function LoadRawGroups(zip As ZipArchive, msg As Action(Of String)) As IEnumerable(Of NamedCollection(Of Raw))
            Dim filelist = zip.Entries _
                .Where(Function(f) f.FullName.StartsWith("meta/")) _
                .ToArray

            For Each metafile As ZipArchiveEntry In filelist
                Dim key As String = metafile.FullName.Replace("meta/", "").BaseName
                Dim content As Raw() = MsgPackSerializer.Deserialize(Of Raw())(metafile.Open)

                ' save mzpack to temp and then modify cache path
                For Each meta As Raw In content
                    Dim tempfile As String = getTempref(meta)
                    Dim zipfile As ZipArchiveEntry = zip.Entries _
                        .Where(Function(f)
                                   Return f.FullName = $"mzpack/{meta.cache}"
                               End Function) _
                        .First

                    Call msg($"unpack raw data [{key}/{tempfile.FileName}]...")

                    zipfile.Open.FlushStream(tempfile)
                    meta.cache = tempfile.Replace("\", "/")
                Next

                Yield New NamedCollection(Of Raw) With {
                    .name = key,
                    .value = content
                }
            Next
        End Function

        Private Function getTempref(meta As Raw) As String
            Return $"{App.AppSystemTemp}/.cache/{meta.cache.Substring(0, 2)}/{meta.cache}.mzPack"
        End Function
    End Module
End Namespace