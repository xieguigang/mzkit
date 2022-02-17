Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' *.mzwork handler
''' </summary>
Public Module MZWork

    <Extension>
    Public Function ExportWorkspace(workspace As WorkspaceFile, save As String) As Boolean
        Using file As Stream = save.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False), zip As New ZipArchive(file, ZipArchiveMode.Create)
            ' /
            ' +--- mzpack (raw data files)
            ' +--- meta (meta data of the raw data file)
            ' +--- src (R# automation script files)
            For Each rawfile In workspace.cacheFiles.SafeQuery
                For Each cache As Raw In rawfile.Value
                    ' write cache data file
                    Using buffer As Stream = cache.cache.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                        Dim container = zip.CreateEntry($"mzpack/{cache.cache.BaseName}").Open

                        Call buffer.CopyTo(container)
                        Call container.Flush()
                        Call container.Close()
                        Call container.Dispose()
                    End Using
                Next

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

                ' save message pack
                Dim metapack = zip.CreateEntry($"meta/{rawfile.Key}.pack").Open

                Call MsgPackSerializer.SerializeObject(meta, metapack)
                Call metapack.Flush()
                Call metapack.Close()
                Call metapack.Dispose()
            Next

            ' save rscript files
            For Each script In workspace.scriptFiles.SafeQuery
                Using buffer As Stream = script.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                    Dim container = zip.CreateEntry($"src/{script.FileName}").Open

                    Call buffer.CopyTo(container)
                    Call container.Flush()
                    Call container.Close()
                    Call container.Dispose()
                End Using
            Next
        End Using

        Return True
    End Function

    <Extension>
    Public Function ImportWorkspace(mzwork As String) As WorkspaceFile
        Dim workspace As New WorkspaceFile With {
            .cacheFiles = New Dictionary(Of String, Raw())
        }

        Using buffer As Stream = mzwork.Open(FileMode.Open, doClear:=False, [readOnly]:=True), zip As New ZipArchive(buffer, ZipArchiveMode.Read)
            Dim filelist = zip.Entries.Where(Function(f) f.FullName.StartsWith("meta/")).ToArray
            Dim scripts As New List(Of String)

            For Each metafile In filelist
                Dim key As String = metafile.FullName.Replace("meta/", "").FileName
                Dim content As Raw() = MsgPackSerializer.Deserialize(Of Raw())(metafile.Open)

                ' save mzpack to temp and then modify cache path
                For Each meta As Raw In content
                    Dim tempfile As String = $"{App.AppSystemTemp}/.cache/{meta.cache.Substring(0, 2)}/{meta.cache}.mzPack"
                    Dim zipfile = zip.Entries.Where(Function(f) f.FullName = $"mzpack/{meta.cache}").First

                    zipfile.Open.FlushStream(tempfile)
                    meta.cache = tempfile
                Next

                workspace.cacheFiles.Add(key, content)
            Next

            filelist = zip.Entries.Where(Function(f) f.FullName.StartsWith("src/")).ToArray

            For Each script In filelist
                Dim scriptName As String = script.FullName.Replace("src/", "")
                Dim text As String = New StreamReader(script.Open).ReadToEnd
                Dim path As String = $"{App.ProductProgramData}/.script/{mzwork.MD5}/{scriptName}"

                Call text.SaveTo(path)
                Call scripts.Add(path)
            Next

            workspace.scriptFiles = scripts.ToArray
        End Using

        Return workspace
    End Function
End Module
