Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Net.Http

Public Class mzPackReader : Inherits BinaryStreamReader

    Dim otherScanners As Dictionary(Of String, Long)
    Dim hasThumbnail As Boolean
    Dim otherIndex As BufferRegion

    Public ReadOnly Property ChromatogramScanners As IEnumerable(Of String)
        Get
            Return otherScanners.Keys
        End Get
    End Property

    Public Sub New(file As String)
        Call MyBase.New(file)
    End Sub

    Sub New(file As Stream)
        Call MyBase.New(file)
    End Sub

    Public Function OpenScannerData(key As String) As Stream
        Dim start As Long = otherScanners(key)
        Dim size As Integer

        file.Seek(start, SeekOrigin.Begin)
        size = file.ReadInt32

        Return New MemoryStream(file.ReadBytes(size))
    End Function

    Protected Overrides Sub loadIndex()
        MyBase.loadIndex()

        If MSscannerIndex.nextBlock >= file.Length Then
            hasThumbnail = False
        Else
            ' load other scanner
            Call loadScannerIndex()

            If otherIndex.nextBlock >= file.Length Then
                hasThumbnail = False
            Else
                hasThumbnail = True
            End If
        End If
    End Sub

    Private Sub loadScannerIndex()
        Dim nsize As Integer
        Dim scanPos As Long
        Dim scanId As String
        Dim indexOffset As Long

        otherScanners = New Dictionary(Of String, Long)

        Using file.TemporarySeek
            file.Seek(MSscannerIndex.nextBlock, SeekOrigin.Begin)
            file.Seek(file.ReadInt64, SeekOrigin.Begin)

            indexOffset = file.Position
            nsize = file.ReadInt32

            For i As Integer = 0 To nsize - 1
                scanPos = file.ReadInt64
                scanId = file.ReadString(BinaryStringFormat.ZeroTerminated)
                otherScanners(scanId) = scanPos
            Next

            otherIndex = New BufferRegion With {
                .position = indexOffset,
                .size = file.Position - .position
            }
        End Using
    End Sub

    Public Function GetThumbnail() As Bitmap
        If Not hasThumbnail Then
            Return Nothing
        Else
            Dim offset As Long
            Dim bytes As Byte()

            file.Seek(file.Length - 8, SeekOrigin.Begin)
            offset = file.ReadInt64
            file.Seek(offset, SeekOrigin.Begin)
            bytes = file.ReadBytes(file.Length - 8 - offset)

            Using buffer As New MemoryStream(bytes), img As Stream = buffer.UnGzipStream
                Return Image.FromStream(img)
            End Using
        End If
    End Function
End Class
