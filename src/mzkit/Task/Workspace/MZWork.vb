#Region "Microsoft.VisualBasic::76bf227d5ce278b0dae0979f0f41bbad, mzkit\src\mzkit\Task\Workspace\MZWork.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 127
    '    Code Lines: 90
    ' Comment Lines: 12
    '   Blank Lines: 25
    '     File Size: 5.27 KB


    ' Module MZWork
    ' 
    '     Function: ExportWorkspace, ImportWorkspace
    ' 
    ' /********************************************************************************/

#End Region

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
    Public Function ExportWorkspace(workspace As WorkspaceFile, save As String, msg As Action(Of String)) As Boolean
        Using file As Stream = save.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False), zip As New ZipArchive(file, ZipArchiveMode.Create)
            ' /
            ' +--- mzpack (raw data files)
            ' +--- meta (meta data of the raw data file)
            ' +--- src (R# automation script files)
            For Each rawfile In workspace.cacheFiles.SafeQuery
                ' save message pack
                Dim nameKey As String = rawfile.Key

                If nameKey.Contains(vbCr) OrElse nameKey.Contains(vbLf) Then
                    nameKey = nameKey.LineTokens.Last

                    If nameKey.Length > 24 Then
                        nameKey = "..." & nameKey.Substring(nameKey.Length - 21)
                    End If
                End If

                Call msg($"pack raw [{nameKey}]...")

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

                Dim metapack = zip.CreateEntry($"meta/{nameKey}.pack").Open

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
    Public Function ImportWorkspace(mzwork As String, msg As Action(Of String)) As WorkspaceFile
        Dim workspace As New WorkspaceFile With {
            .cacheFiles = New Dictionary(Of String, Raw())
        }

        Using buffer As Stream = mzwork.Open(FileMode.Open, doClear:=False, [readOnly]:=True), zip As New ZipArchive(buffer, ZipArchiveMode.Read)
            Dim filelist = zip.Entries.Where(Function(f) f.FullName.StartsWith("meta/")).ToArray
            Dim scripts As New List(Of String)

            For Each metafile In filelist
                Dim key As String = metafile.FullName.Replace("meta/", "").BaseName
                Dim content As Raw() = MsgPackSerializer.Deserialize(Of Raw())(metafile.Open)

                ' save mzpack to temp and then modify cache path
                For Each meta As Raw In content
                    Dim tempfile As String = $"{App.AppSystemTemp}/.cache/{meta.cache.Substring(0, 2)}/{meta.cache}.mzPack"
                    Dim zipfile = zip.Entries.Where(Function(f) f.FullName = $"mzpack/{meta.cache}").First

                    msg($"unpack raw data [{key}/{tempfile.FileName}]...")

                    zipfile.Open.FlushStream(tempfile)
                    meta.cache = tempfile.Replace("\", "/")
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
