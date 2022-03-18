#Region "Microsoft.VisualBasic::a3b25ed620aa69f50c53c4c5ec69a626, mzkit\src\mzkit\Task\Workspace\ViewerProject.vb"

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

    '   Total Lines: 162
    '    Code Lines: 125
    ' Comment Lines: 9
    '   Blank Lines: 28
    '     File Size: 5.62 KB


    ' Class ViewerProject
    ' 
    '     Properties: Count, FilePath, MimeType, work
    ' 
    '     Function: FindRawFile, GetAutomationScripts, GetRawDataFiles, LoadWorkspace, (+2 Overloads) Save
    '               SaveAs
    ' 
    '     Sub: Add
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports System.Threading
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.BSON
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml.Models

''' <summary>
''' the mzkit workspace file
''' </summary>
Public Class ViewerProject : Implements ISaveHandle, IFileReference

    Public Property work As New WorkspaceFile

    Public Property FilePath As String Implements IFileReference.FilePath

    Public ReadOnly Property Count As Integer
        Get
            If work.cacheFiles Is Nothing Then
                Return 0
            Else
                Return work.cacheFiles.Values.Select(Function(a) a.Length).Sum
            End If
        End Get
    End Property

    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
        Get
            Return {New ContentType With {.Details = "mzkit workspace", .FileExt = "mzwork", .MIMEType = "application/mzwork", .Name = "Mzkit workspace"}}
        End Get
    End Property

    Public Function GetRawDataFiles() As IEnumerable(Of Raw)
        If work.cacheFiles Is Nothing Then
            Return {}
        Else
            Return work.cacheFiles.Values.IteratesALL
        End If
    End Function

    Public Sub Add(raw As Raw)
        If Not work.cacheFiles.ContainsKey(raw.source.FileName) Then
            work.cacheFiles.Add(raw.source.FileName, {})
        End If

        work.cacheFiles(raw.source.FileName) = work.cacheFiles(raw.source.FileName) _
            .JoinIterates(raw) _
            .ToArray
    End Sub

    Public Function FindRawFile(path As String) As Raw
        Dim fullName As String = path

        If Not path.Any(AddressOf ASCII.IsNonPrinting) Then
            fullName = path.GetFullPath
        End If

        Return GetRawDataFiles _
            .Where(Function(a)
                       Dim name As String = a.source

                       If Not name.Any(AddressOf ASCII.IsNonPrinting) Then
                           name = name.GetFullPath
                       End If

                       Return name = fullName
                   End Function) _
            .FirstOrDefault
    End Function

    Public Function GetAutomationScripts() As IEnumerable(Of String)
        Return work.scriptFiles.AsEnumerable
    End Function

    Public Function SaveAs(cache As IEnumerable(Of Raw), scripts As IEnumerable(Of String), opened As IEnumerable(Of String), unsaved As IEnumerable(Of NamedValue)) As ViewerProject
        Dim workspace As New WorkspaceFile With {
            .cacheFiles = cache _
                .GroupBy(Function(a) a.source.FileName) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.ToArray
                              End Function),
            .scriptFiles = scripts.ToArray,
            .openedScripts = opened.ToArray,
            .unsavedScripts = unsaved.ToArray
        }

        Return New ViewerProject With {
            .work = workspace,
            .FilePath = FilePath
        }
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="cacheList">the filepath of the <see cref="WorkspaceFile"/></param>
    ''' <param name="progress"></param>
    ''' <returns></returns>
    Public Shared Function LoadWorkspace(cacheList As String, progress As Action(Of String)) As ViewerProject
        Dim rawBuffer As Byte() = cacheList.ReadBinary
        Dim workspace As WorkspaceFile

        If rawBuffer.IsNullOrEmpty Then
            workspace = New WorkspaceFile With {
                .cacheFiles = New Dictionary(Of String, Raw()),
                .scriptFiles = {},
                .openedScripts = {},
                .unsavedScripts = {}
            }
        Else
            Call progress("Load raw file list...")

            Try
                workspace = rawBuffer _
                   .DoCall(AddressOf BSONFormat.Load) _
                   .CreateObject(GetType(WorkspaceFile))
            Catch ex As Exception
                Call App.LogException(ex)
                Call progress($"The workspace file is damaged, skip loading {cacheList}...")
                Call Thread.Sleep(1000)

                workspace = New WorkspaceFile With {
                    .cacheFiles = New Dictionary(Of String, Raw()),
                    .scriptFiles = {},
                    .unsavedScripts = {},
                    .openedScripts = {}
                }
            End Try
        End If

        Call progress("File reading success!")

        Dim viewer As New ViewerProject With {
            .FilePath = cacheList,
            .work = workspace
        }

        Return viewer
    End Function

    Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        Dim model As JsonElement = GetType(WorkspaceFile).GetJsonElement(work, New JSONSerializerOptions)

        Using buffer As FileStream = path.Open(doClear:=True)
            Call DirectCast(model, JsonObject).WriteBuffer(buffer)
        End Using

        FilePath = path

        Return True
    End Function

    Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
        Return Save(path, encoding.CodePage)
    End Function
End Class
