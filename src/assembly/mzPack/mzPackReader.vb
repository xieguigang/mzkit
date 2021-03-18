Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache

Public Class mzPackReader : Inherits BinaryStreamReader

    Public Sub New(file As String)
        MyBase.New(file)
    End Sub

    Protected Overrides Sub loadIndex()
        MyBase.loadIndex()
    End Sub

    Public Function GetThumbnail() As Bitmap
        Dim offset As Long
        Dim bytes As Byte()

        file.Seek(file.Length - 8, SeekOrigin.Begin)
        offset = file.ReadInt64
        file.Seek(offset, SeekOrigin.Begin)
        bytes = file.ReadBytes(file.Length - 8 - offset)

        Using buffer As New MemoryStream(bytes)
            Return Image.FromStream(buffer)
        End Using
    End Function
End Class
