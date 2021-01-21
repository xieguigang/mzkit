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

    Public Property work As WorkspaceFile

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

        work.cacheFiles(raw.source.FileName) = work.cacheFiles(raw.source.FileName).JoinIterates(raw).ToArray
    End Sub

    Public Function FindRawFile(path As String) As Raw
        Return GetRawDataFiles.Where(Function(a) a.source.GetFullPath = path.GetFullPath).FirstOrDefault
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
