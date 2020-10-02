Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.BSON
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Text

''' <summary>
''' the mzkit workspace file
''' </summary>
Public Class ViewerProject : Implements ISaveHandle, IFileReference

    ''' <summary>
    ''' 原始数据文件的缓存对象列表
    ''' </summary>
    Dim cacheFiles As Dictionary(Of String, Raw())

    ''' <summary>
    ''' 自动化脚本的文件路径列表
    ''' </summary>
    Dim scriptFiles As String()

    Public Property FilePath As String Implements IFileReference.FilePath

    Public ReadOnly Property Count As Integer
        Get
            Return cacheFiles.Values.Select(Function(a) a.Length).Sum
        End Get
    End Property

    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
        Get
            Return {New ContentType With {.Details = "mzkit workspace", .FileExt = "mzwork", .MIMEType = "application/mzwork", .Name = "Mzkit workspace"}}
        End Get
    End Property

    Public Function GetRawDataFiles() As IEnumerable(Of Raw)
        Return cacheFiles.Values.IteratesALL
    End Function

    Public Function GetAutomationScripts() As IEnumerable(Of String)
        Return scriptFiles.AsEnumerable
    End Function

    Public Function SaveAs(cache As IEnumerable(Of Raw), scripts As IEnumerable(Of String)) As ViewerProject
        Return New ViewerProject With {
            .cacheFiles = cache _
                .GroupBy(Function(a) a.source.FileName) _
                .ToDictionary(Function(a) a.Key,
                              Function(a) a.ToArray),
            .FilePath = FilePath,
            .scriptFiles = scripts.ToArray
        }
    End Function

    Public Shared Function LoadWorkspace(cacheList As String, progress As Action(Of String)) As ViewerProject
        Dim viewer As New ViewerProject With {
            .FilePath = cacheList
        }
        Dim rawBuffer As Byte() = cacheList.ReadBinary
        Dim workspace As WorkspaceFile

        If rawBuffer.IsNullOrEmpty Then
            workspace = New WorkspaceFile With {
                .cacheFiles = New Dictionary(Of String, Raw()),
                .scriptFiles = {}
            }
        Else
            Call progress("Load raw file list...")

            workspace = rawBuffer _
               .DoCall(AddressOf BSONFormat.Load) _
               .CreateObject(GetType(WorkspaceFile))
        End If

        Call progress("File reading success!")

        Return viewer
    End Function

    Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        Dim works As New WorkspaceFile With {
            .cacheFiles = cacheFiles,
            .scriptFiles = scriptFiles
        }
        Dim model As JsonElement = GetType(WorkspaceFile).GetJsonElement(works, New JSONSerializerOptions)

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
