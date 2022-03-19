#Region "Microsoft.VisualBasic::55db16f24ac667303220bdd68e881a8a, mzkit\src\visualize\MsImaging\IndexedCache\XICIndex.vb"

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

    '   Total Lines: 169
    '    Code Lines: 129
    ' Comment Lines: 9
    '   Blank Lines: 31
    '     File Size: 6.56 KB


    '     Class XICIndex
    ' 
    '         Properties: height, mz, offset, source, time
    '                     tolerance, width
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetOffsets
    ' 
    '         Sub: WriteIndexFile, writePixel
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO

Namespace IndexedCache

    Public Class XICIndex

        Public ReadOnly Property mz As Double()
        Public ReadOnly Property offset As Long()
        Public ReadOnly Property width As Integer
        Public ReadOnly Property height As Integer
        ''' <summary>
        ''' the file name of the upstream source file
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property source As String
        Public ReadOnly Property tolerance As String
        Public ReadOnly Property time As Date

        Public Const MagicHeader As String = "BioNovoGene/MSI"

        Sub New(mz As Double(),
                offset As Long(),
                width As Integer,
                height As Integer,
                source As String,
                tolerance As String,
                time As Date)

            Me.time = time
            Me.mz = mz
            Me.offset = offset
            Me.width = width
            Me.height = height
            Me.source = source
            Me.tolerance = tolerance
        End Sub

        Public Iterator Function GetOffsets(mz As Double, tolerance As Tolerance) As IEnumerable(Of Long)
            For i As Integer = 0 To Me.mz.Length - 1
                If tolerance(_mz(i), mz) Then
                    Yield _offset(i)
                End If
            Next
        End Function

        Public Shared Sub WriteIndexFile(cache As XICWriter, file As Stream)
            Dim mz As Double() = cache.offsets.Keys.ToArray
            Dim nfeatures As Integer = mz.Length

            Using out As New BinaryDataWriter(file) With {.ByteOrder = ByteOrder.BigEndian}
                Call out.Write(MagicHeader, BinaryStringFormat.NoPrefixOrTermination)
                ' write meta data
                Call out.Write(nfeatures)
                Call out.Write(cache.width)
                Call out.Write(cache.height)
                Call out.Write(cache.src, BinaryStringFormat.ZeroTerminated)
                Call out.Write(cache.tolerance.GetScript, BinaryStringFormat.ZeroTerminated)
                Call out.Write(Now.ToString, BinaryStringFormat.ZeroTerminated)
                Call out.Write(CByte(0))
                Call out.Write(mz)
                Call out.Flush()

                Dim offsetPos As Long = out.Position

                ' write placeholder
                Call out.Write(mz.Select(Function(any) 0&).ToArray)
                ' placeholder of offset to pixel cache
                Call out.Write(0&)
                Call out.Write(CByte(0))
                Call out.Flush()

                Dim offsets As New Dictionary(Of Double, Long)

                Call cache.Dispose()

                Using cachefile As New BinaryDataReader(cache.cache.Open(FileMode.Open))
                    For Each mzi As Double In mz
                        Dim offset As Long = cache.offsets(mzi).position
                        Dim nlen As Integer = cache.length(mzi)
                        Dim size As Integer = XICWriter.delta * nlen
                        ' [x,y] intensity
                        Dim bytes = cachefile.ReadBytes(size)
                        Dim x As Integer() = New Integer(nlen - 1) {}
                        Dim y As Integer() = New Integer(nlen - 1) {}
                        Dim intensity As Double() = New Double(nlen - 1) {}

                        Using ms As New MemoryStream(bytes), temp As New BinaryDataReader(ms)
                            For i As Integer = 0 To intensity.Length - 1
                                Dim xy = temp.ReadInt32s(2)

                                x(i) = xy(0)
                                y(i) = xy(1)
                                intensity(i) = temp.ReadDouble
                            Next
                        End Using

                        Erase bytes

                        offsets(mzi) = out.Position

                        Call out.Write(mzi)
                        Call out.Write(nlen)
                        Call out.Write(intensity)
                        Call out.Write(x)
                        Call out.Write(y)
                        Call out.Write(CByte(0))
                        Call out.Flush()
                    Next
                End Using

                Dim pixelsOffset As Long = out.Position
                Dim centroid = cache.centroidPixels _
                    .GroupBy(Function(p) p.X) _
                    .OrderBy(Function(x) x.Key) _
                    .ToArray
                Dim pixelsMatrix As Long()() = MAT(Of Long)(cache.height, cache.width)

                ' write placeholder
                For Each row In pixelsMatrix
                    Call out.Write(row)
                Next

                Call out.Flush()

                For Each x In centroid
                    For Each yPixels As IGrouping(Of Integer, ibdPixel) In x _
                        .GroupBy(Function(p) p.Y) _
                        .OrderBy(Function(y) y.Key)

                        For Each y As ibdPixel In yPixels
                            pixelsMatrix(yPixels.Key - 1)(x.Key - 1) = out.Position
                            writePixel(out, y)
                        Next
                    Next
                Next

                Call out.Flush()
                Call out.Seek(pixelsOffset, SeekOrigin.Begin)

                For Each row In pixelsMatrix
                    Call out.Write(row)
                Next

                Call out.Flush()

                out.Seek(offsetPos, SeekOrigin.Begin)
                out.Write(mz.Select(Function(mzi) offsets(mzi)).ToArray)
                out.Write(pixelsOffset)
                out.Flush()
            End Using
        End Sub

        Private Shared Sub writePixel(out As BinaryDataWriter, pixel As ibdPixel)
            Dim matrix = pixel.GetMs

            Call out.Write(pixel.X)
            Call out.Write(pixel.Y)
            Call out.Write(matrix.Length)
            Call out.Write(matrix.Select(Function(m) m.mz).ToArray)
            Call out.Write(matrix.Select(Function(m) m.intensity).ToArray)
            Call out.Flush()
        End Sub

    End Class
End Namespace
