#Region "Microsoft.VisualBasic::b32004ae44eff56dc158f1c187bc6bce, mzkit\services\MZWork\MZWork.vb"

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

    '   Total Lines: 71
    '    Code Lines: 49 (69.01%)
    ' Comment Lines: 8 (11.27%)
    '    - Xml Docs: 37.50%
    ' 
    '   Blank Lines: 14 (19.72%)
    '     File Size: 2.65 KB


    ' Module MZWork
    ' 
    '     Function: ExportWorkspace, ImportWorkspace
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' *.mzwork handler
''' </summary>
Public Module MZWork

    <Extension>
    Public Function ExportWorkspace(workspace As WorkspaceFile, save As String, msg As Action(Of String)) As Boolean
        Using file As Stream = save.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False),
            zip As New ZipArchive(file, ZipArchiveMode.Create)

            ' /
            ' +--- mzpack (raw data files)
            ' +--- meta (meta data of the raw data file)
            ' +--- src (R# automation script files)
            Call workspace.cacheFiles.ExportWorkspace(zip, msg)

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

        Using buffer As Stream = mzwork.Open(FileMode.Open, doClear:=False, [readOnly]:=True),
            zip As New ZipArchive(buffer, ZipArchiveMode.Read)

            Dim scripts As New List(Of String)
            Dim filelist = zip.Entries _
                .Where(Function(f) f.FullName.StartsWith("src/")) _
                .ToArray

            For Each group In zip.LoadRawGroups(msg)
                Call workspace.cacheFiles.Add(group.name, group.ToArray)
            Next

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
