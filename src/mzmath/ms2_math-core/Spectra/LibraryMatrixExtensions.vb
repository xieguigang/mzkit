#Region "Microsoft.VisualBasic::2f147acb55797cf37e4c754a952c47ce, mzkit\src\mzmath\ms2_math-core\Spectra\LibraryMatrixExtensions.vb"

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

'   Total Lines: 124
'    Code Lines: 71
' Comment Lines: 39
'   Blank Lines: 14
'     File Size: 4.71 KB


'     Module LibraryMatrixExtensions
' 
'         Function: AbSciexBaselineHandling, AsMatrix, Centroid, CentroidMode, Max
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.BinaryDumping

Namespace Spectra

    ''' <summary>
    ''' Library matrix math
    ''' </summary>
    ''' 
    <HideModuleName>
    Public Module LibraryMatrixExtensions

        ''' <summary>
        ''' MAX(<see cref="ms2.intensity"/>)
        ''' </summary>
        ''' <param name="matrix"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Max(matrix As LibraryMatrix) As Double
            If matrix.Length = 0 Then
                Return 0
            Else
                Return matrix.ms2 _
                    .Select(Function(r) r.intensity) _
                    .Max
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="lib_ms2"></param>
        ''' <param name="title"></param>
        ''' <param name="normalize">matrix will be normalized to [0, 1]</param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function AsMatrix(lib_ms2 As IEnumerable(Of Library),
                                 Optional title As String = Nothing,
                                 Optional normalize As Boolean = True) As LibraryMatrix

            Dim raw As ms2() = lib_ms2 _
                .Select(Function(l)
                            Return New ms2 With {
                                .mz = l.ProductMz,
                                .intensity = l.LibraryIntensity
                            }
                        End Function) _
                .ToArray
            Dim libM As New LibraryMatrix With {.ms2 = raw, .name = title}

            If normalize Then
                libM = libM / libM.Max
            End If

            Return libM
        End Function

        ''' <summary>
        ''' Convert profile matrix to centroid matrix
        ''' </summary>
        ''' <param name="[lib]"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function CentroidMode([lib] As LibraryMatrix,
                                     tolerance As Tolerance,
                                     Optional cutoff As LowAbundanceTrimming = Nothing) As LibraryMatrix

            [lib].ms2 = [lib].ms2.Centroid(tolerance, cutoff Or LowAbundanceTrimming.Default).ToArray
            [lib].centroid = True

            Return [lib]
        End Function

        ''' <summary>
        ''' Convert profile matrix to centroid matrix
        ''' </summary>
        ''' <param name="peaks"></param>
        ''' <param name="cutoff"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' order of data processing:
        ''' 
        ''' ``intensity_cutoff -> centroid``
        ''' </remarks>
        <Extension>
        Public Function Centroid(peaks As ms2(), tolerance As Tolerance, cutoff As LowAbundanceTrimming) As IEnumerable(Of ms2)
            Dim maxInto = If(peaks.IsNullOrEmpty, 0, peaks.Select(Function(p) p.intensity).Max)

            ' removes low intensity fragment peaks
            ' for save calculation time
            peaks = cutoff.Trim(peaks)

            If peaks.Length = 0 Then
                Return {}
            Else
                ' 20200702 due to the reason of we not calculate the peakarea
                ' so that there is no needs for populate ROI
                ' find the highest fragment directly
                Return peaks _
                    .GroupBy(Function(ms2) ms2.mz, AddressOf tolerance.Equals) _
                    .Select(Function(g)
                                ' 合并在一起的二级碎片的相应强度取最高的为结果
                                Dim fragments As ms2() = g.ToArray
                                Dim maxi As Integer = which.Max(fragments.Select(Function(m) m.intensity))
                                Dim max As ms2 = fragments(maxi)

                                Return max
                            End Function) _
                    .ToArray
            End If
        End Function

        ''' <summary>
        ''' removes noise peak of the ab5600 raw data file
        ''' </summary>
        ''' <param name="msData"></param>
        ''' <param name="cut"></param>
        ''' <returns></returns>
        <Extension>
        Public Function AbSciexBaselineHandling(msData As IEnumerable(Of ms2), Optional cut As Integer = 2) As IEnumerable(Of ms2)
            Return msData _
                .GroupBy(Function(i) i.intensity, offsets:=1) _
                .Where(Function(i) i.Length < cut) _
                .Select(Function(i) i.value) _
                .IteratesALL
        End Function

        Public Function ParseStream(data As Byte()) As LibraryMatrix
            Using buf As New MemoryStream(data)
                Return ParseStream(buf)
            End Using
        End Function

        Public Function ParseStream(data As Stream) As LibraryMatrix
            Dim file As New BinaryReader(data)
            Dim size As Integer
            Dim buf As Byte()
            Dim name As String
            Dim mz, into As Double()
            Dim decoder As New NetworkByteOrderBuffer

            size = file.ReadInt32
            buf = file.ReadBytes(size)
            name = Encoding.UTF8.GetString(buf)

            size = file.ReadInt32 * RawStream.DblFloat
            buf = file.ReadBytes(size)
            mz = decoder.decode(buf)
            buf = file.ReadBytes(size)
            into = decoder.decode(buf)

            Return New LibraryMatrix(name, mz, into)
        End Function

        <Extension>
        Public Function GetStream(mat As LibraryMatrix) As Byte()
            Using buf As New MemoryStream
                Dim bytes As Byte()

                bytes = Encoding.UTF8.GetBytes(mat.name)
                buf.Write(BitConverter.GetBytes(bytes.Length), Scan0, 4)
                buf.Write(bytes, Scan0, bytes.Length)

                Dim encoder As New NetworkByteOrderBuffer
                Dim mz As Byte() = encoder.encode(mat.mz)
                Dim into As Byte() = encoder.encode(mat.intensity)
                Dim sizeOf As Integer = mz.Length

                buf.Write(BitConverter.GetBytes(mat.Array.Length), Scan0, 4)
                buf.Write(mz, Scan0, sizeOf)
                buf.Write(into, Scan0, sizeOf)

                Return buf.ToArray
            End Using
        End Function
    End Module
End Namespace
