#Region "Microsoft.VisualBasic::61d7e5c9279ec680837911dabd56c2b4, mzmath\ms2_math-core\Spectra\SpectraEncoder.vb"

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

    '   Total Lines: 69
    '    Code Lines: 46 (66.67%)
    ' Comment Lines: 15 (21.74%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 8 (11.59%)
    '     File Size: 2.66 KB


    '     Module SpectraEncoder
    ' 
    ' 
    '         Delegate Function
    ' 
    '             Function: Decode, GetEncoder, LibraryMatrix
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.BinaryDumping
Imports Microsoft.VisualBasic.Text

Namespace Spectra

    ''' <summary>
    ''' Spectra matrix encoder helper for mysql/csv
    ''' </summary>
    Public Module SpectraEncoder

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="spectrum"></param>
        ''' <param name="centroid"></param>
        ''' <param name="average">make average spectrum instead of make sum of the spectrum peaks</param>
        ''' <returns>
        ''' this function returns a sum spectrum of the given spectrum collection. may be returns nothing if the given spectrum collection is empty.
        ''' </returns>
        <Extension>
        Public Function SpectrumSum(spectrum As IEnumerable(Of LibraryMatrix),
                                    Optional centroid As Double = 0.1,
                                    Optional average As Boolean = False) As LibraryMatrix

            Dim pool As New List(Of LibraryMatrix)
            Dim peaks As New List(Of Double)

            For Each spec As LibraryMatrix In spectrum
                Call pool.Add(spec)
                Call peaks.AddRange(spec.Select(Function(a) a.mz))
            Next

            If pool.Count = 1 Then
                Return New LibraryMatrix(pool.First.AsEnumerable)
            ElseIf pool.Count = 0 Then
                Return Nothing
            End If

            Dim mzIndex As MzPool = peaks.CreateCentroidFragmentSet(centroid)
            Dim v As Double() = New Double(mzIndex.size - 1) {}
            Dim size As Integer = mzIndex.size

            For Each spec As LibraryMatrix In pool
                v = SIMD.Add.f64_op_add_f64(v, spec.DeconvoluteMS(size, mzIndex))
            Next

            If average Then
                v = SIMD.Divide.f64_op_divide_f64_scalar(v, pool.Count)
            End If

            Return New LibraryMatrix(mzIndex.ionSet, v)
        End Function

        <Extension>
        Public Function CreateCentroidFragmentSet(fragments As IEnumerable(Of Double),
                                                  Optional centroid As Double = 0.1,
                                                  Optional window_size As Double = 1) As MzPool
            Dim mzgroups = fragments _
                .GroupBy(offset:=centroid) _
                .Select(Function(a) Val(a.name)) _
                .OrderBy(Function(mzi) mzi) _
                .ToArray
            Dim pool As New MzPool(mzgroups, win_size:=window_size)

            Return pool
        End Function

        <Extension>
        Public Function DeconvoluteMS(sp As LibraryMatrix, len As Integer, mzIndex As MzPool) As Double()
            Return DeconvoluteScan(sp.Select(Function(a) a.mz).ToArray, sp.Select(Function(a) a.intensity).ToArray, len, mzIndex)
        End Function

        ''' <summary>
        ''' make alignment of the scan data to a given set of the mz index data
        ''' </summary>
        ''' <param name="mz"></param>
        ''' <param name="into"></param>
        ''' <param name="len">
        ''' should be the length of the <paramref name="mzIndex"/>
        ''' </param>
        ''' <param name="mzIndex"></param>
        ''' <returns>
        ''' a vector of the intensity data which is aligned with the mz vector
        ''' </returns>
        Public Function DeconvoluteScan(mz As Double(),
                                        into As Double(),
                                        len As Integer,
                                        mzIndex As MzPool) As Double()

            Dim v As Double() = New Double(len - 1) {}
            Dim mzi As Double
            Dim hit As MzIndex
            Dim scan_size As Integer = mz.Length

            For i As Integer = 0 To scan_size - 1
                mzi = mz(i)
                hit = mzIndex.SearchBest(mzi)

                If hit Is Nothing Then
                    ' 20221102
                    '
                    ' missing data
                    ' could be caused by the selective ion data export
                    ' just ignores of this problem
                Else
                    v(hit.index) += into(i)
                End If
            Next

            Return v
        End Function

        Public Delegate Function Encoder(Of T)(mzData As T()) As String

        ''' <summary>
        ''' 将质谱图编码为base64字符串
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="getX"></param>
        ''' <param name="getY"></param>
        ''' <returns></returns>
        Public Function GetEncoder(Of T)(getX As Func(Of T, Double), getY As Func(Of T, Double)) As Encoder(Of T)
            Return Function(matrix As T()) As String
                       Dim table$ = matrix _
                           .Select(Function(m)
                                       Return {getX(m), getY(m)}.JoinBy(ASCII.TAB)
                                   End Function) _
                           .JoinBy(vbCrLf)
                       Dim bytes As Byte() = TextEncodings.UTF8WithoutBOM.GetBytes(table)
                       Dim base64$ = bytes.ToBase64String

                       Return base64
                   End Function
        End Function

        ReadOnly network As New NetworkByteOrderBuffer

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="mz64"></param>
        ''' <param name="into64"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' the numeric data vector should be encoded in network byte order
        ''' </remarks>
        Public Iterator Function Decode(mz64 As String, into64 As String) As IEnumerable(Of ms2)
            Dim mz As Double() = network.ParseDouble(mz64)
            Dim into As Double() = network.ParseDouble(into64)

            For i As Integer = 0 To mz.Length - 1
                Yield New ms2(mz(i), into(i))
            Next
        End Function

        ''' <summary>
        ''' 对base64字符串做解码，重新生成质谱图
        ''' </summary>
        ''' <param name="base64"></param>
        ''' <returns></returns>
        Public Function Decode(base64 As String) As (x#, y#)()
            Dim bytes As Byte() = Convert.FromBase64String(base64)
            Dim table$ = TextEncodings.UTF8WithoutBOM.GetString(bytes)
            Dim fragments$() = table.LineTokens
            Dim matrix = fragments _
                .Select(Function(r)
                            With r.Split(ASCII.TAB)
                                Return (x:=Val(.ByRef(0)), y:=Val(.ByRef(1)))
                            End With
                        End Function) _
                .ToArray

            Return matrix
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function LibraryMatrix(decode As (x#, y#)()) As LibraryMatrix
            Return decode _
                .Select(Function(d)
                            Return New ms2 With {
                                .mz = d.x,
                                .intensity = d.y
                            }
                        End Function) _
                .ToArray
        End Function
    End Module
End Namespace
