Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.BSON
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Text

Public Class ViewerProject : Implements ISaveHandle, IFileReference

    Dim cacheFiles As Dictionary(Of String, Raw())

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

    Public Shared Function LoadWorkspace(cacheList As String, progress As Action(Of String)) As ViewerProject
        Dim viewer As New ViewerProject With {.FilePath = cacheList}
        Dim rawBuffer As Byte() = cacheList.ReadBinary

        If rawBuffer.IsNullOrEmpty Then
            viewer.cacheFiles = New Dictionary(Of String, Raw())
        Else
            Call progress("Load raw file list...")

            viewer.cacheFiles = rawBuffer _
               .DoCall(AddressOf BSONFormat.Load) _
               .CreateObject(GetType(Dictionary(Of String, Raw())))
        End If

        Call progress("File reading success!")

        Return viewer
    End Function

    Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        Dim schema As Type = cacheFiles.GetType
        Dim model As JsonElement = schema.GetJsonElement(cacheFiles, New JSONSerializerOptions)

        ' Progress("write workspace file...")

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
