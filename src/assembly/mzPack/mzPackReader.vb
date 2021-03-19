Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Net.Http

Public Class mzPackReader : Inherits BinaryStreamReader

    ReadOnly otherScanners As New Dictionary(Of String, Long)

    Public Sub New(file As String)
        MyBase.New(file)
    End Sub

    Protected Overrides Sub loadIndex()
        MyBase.loadIndex()

        ' load other scanner
        Call loadScannerIndex()
    End Sub

    Private Sub loadScannerIndex()
        Dim nsize As Integer
        Dim scanPos As Long
        Dim scanId As String

        Using file.TemporarySeek
            file.Seek(MSscannerIndex.nextBlock, SeekOrigin.Begin)
            nsize = file.ReadInt32

            For i As Integer = 0 To nsize - 1
                scanPos = file.ReadInt64
                scanId = file.ReadString(BinaryStringFormat.ZeroTerminated)
                otherScanners(scanId) = scanPos
            Next
        End Using
    End Sub

    Public Function GetThumbnail() As Bitmap
        Dim offset As Long
        Dim bytes As Byte()

        file.Seek(file.Length - 8, SeekOrigin.Begin)
        offset = file.ReadInt64
        file.Seek(offset, SeekOrigin.Begin)
        bytes = file.ReadBytes(file.Length - 8 - offset)

        Using buffer As New MemoryStream(bytes), img As Stream = buffer.UnGzipStream
            Return Image.FromStream(img)
        End Using
    End Function
End Class
