Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

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
            Dim access As New WorkspaceAccess(zip, msg)
            Dim contents As New List(Of NamedValue(Of Raw))

            For Each block In access.EnumerateBlocks
                Call contents.AddRange(access.ReleaseCache(block))
            Next

            For Each metaGroup In contents.GroupBy(Function(meta) meta.Name)
                Yield New NamedCollection(Of Raw) With {
                    .name = metaGroup.Key,
                    .value = metaGroup _
                        .Select(Function(i) i.Value) _
                        .ToArray
                }
            Next
        End Function

        Friend Function getTempref(meta As Raw) As String
            Return $"{App.AppSystemTemp}/.cache/{meta.cache.Substring(0, 2)}/{meta.cache}.mzPack"
        End Function
    End Module
End Namespace