#Region "Microsoft.VisualBasic::1bb3077a7d443cebb9bcdb36474560ef, E:/mzkit/src/assembly/mzPack//v1.0/mzPackReader.vb"

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

    '   Total Lines: 160
    '    Code Lines: 119
    ' Comment Lines: 9
    '   Blank Lines: 32
    '     File Size: 4.78 KB


    ' Class mzPackReader
    ' 
    '     Properties: chromatogram, ChromatogramScanners
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    '     Function: GetThumbnail, OpenScannerData, ReadThumbnailInternal
    ' 
    '     Sub: loadChromatogram, loadIndex, loadScannerIndex
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Net.Http

''' <summary>
''' v1 mzpack stream reader
''' </summary>
Public Class mzPackReader : Inherits BinaryStreamReader

    Dim otherScanners As Dictionary(Of String, Long)
    Dim hasThumbnail As Boolean
    Dim otherIndex As BufferRegion

    Public ReadOnly Property ChromatogramScanners As IEnumerable(Of String)
        Get
            If otherScanners.IsNullOrEmpty Then
                Return {}
            Else
                Return otherScanners.Keys
            End If
        End Get
    End Property

    Public ReadOnly Property chromatogram As Chromatogram

    Public Sub New(file As String)
        Call MyBase.New(file)
    End Sub

    ''' <summary>
    ''' create a new version 1 mzpack file reader
    ''' </summary>
    ''' <param name="file"></param>
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
            nsize = file.ReadInt32 ' read int32 count

            For i As Integer = 0 To nsize - 1
                scanPos = file.ReadInt64
                scanId = file.ReadString(BinaryStringFormat.ZeroTerminated)
                otherScanners(scanId) = scanPos
            Next

            otherIndex = New BufferRegion With {
                .position = indexOffset,
                .size = file.Position - .position
            }

            Call loadChromatogram(file)
        End Using
    End Sub

    Private Sub loadChromatogram(file As BinaryDataReader)
        If file.EndOfStream OrElse file.ReadInt64 <> 0 Then
            Return
        End If

        Dim byteSize As Long = ChromatogramBuffer.MeasureSize(file.ReadInt32)

        ' 数据校验不通过
        If byteSize <> file.ReadInt64 Then
            Return
        End If

        Using buffer As New MemoryStream(file.ReadBytes(byteSize))
            buffer.Seek(Scan0, SeekOrigin.Begin)
            _chromatogram = ChromatogramBuffer.FromBuffer(buffer)
        End Using
    End Sub

    Public Function GetThumbnail() As Bitmap
        If Not hasThumbnail Then
            Return Nothing
        Else
            Try
                Return ReadThumbnailInternal()
            Catch ex As Exception
                Call App.LogException(New Exception("error while read the ver1 mzpack thumbnail image.", ex))
                Return Nothing
            End Try
        End If
    End Function

    Private Function ReadThumbnailInternal() As Bitmap
        Dim offset As Long
        Dim bytes As Byte()
        Dim nsize As Long

        file.Seek(file.Length - 8, SeekOrigin.Begin)
        offset = file.ReadInt64

        If offset <= 16 Then
            Return Nothing
        End If

        file.Seek(offset, SeekOrigin.Begin)
        nsize = file.Length - 8 - offset

        If nsize <= 0 Then
            Call "mzpack stream offset error while read v1 mzpack thumbnail image: negative bytes count?".Warning
            Return Nothing
        End If

        bytes = file.ReadBytes(nsize)

        If bytes.IsNullOrEmpty Then
            Return Nothing
        End If

        Using buffer As New MemoryStream(bytes), img As Stream = buffer.UnGzipStream
            Return Image.FromStream(img)
        End Using
    End Function
End Class
