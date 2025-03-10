﻿#Region "Microsoft.VisualBasic::af728ed5f5d69d039c629da1a8f50648, mzmath\ms2_math-core\Spectra\SpectraEncoder.vb"

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

    '   Total Lines: 218
    '    Code Lines: 130 (59.63%)
    ' Comment Lines: 60 (27.52%)
    '    - Xml Docs: 85.00%
    ' 
    '   Blank Lines: 28 (12.84%)
    '     File Size: 9.10 KB


    '     Module SpectraEncoder
    ' 
    '         Function: CreateCentroidFragmentSet, DeconvoluteMS, DeconvoluteScan, SpectrumSum
    '         Delegate Function
    ' 
    '             Function: (+2 Overloads) Decode, GetEncoder, LibraryMatrix
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.SplashID
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
        ''' Create a representative spectrum via sum or mean aggregate method
        ''' </summary>
        ''' <param name="spectrum"></param>
        ''' <param name="centroid"></param>
        ''' <param name="average">make average spectrum instead of make sum of the spectrum peaks</param>
        ''' <returns>
        ''' this function returns a sum spectrum of the given spectrum collection. may be returns nothing if the given spectrum collection is empty.
        ''' </returns>
        <Extension>
        Public Function SpectrumSum(Of T As ISpectrum)(spectrum As IEnumerable(Of T),
                                                       Optional centroid As Double = 0.1,
                                                       Optional average As Boolean = False) As LibraryMatrix
            Dim pool As New List(Of T)
            Dim peaks As New List(Of Double)

            For Each spec As T In spectrum
                Call pool.Add(spec)
                Call peaks.AddRange(spec.GetIons.Select(Function(a) a.mz))
            Next

            If pool.Count = 1 Then
                Return New LibraryMatrix(pool.First.GetIons)
            ElseIf pool.Count = 0 Then
                Return Nothing
            End If

            Dim mzIndex As MzPool = peaks.CreateCentroidFragmentSet(centroid, verbose:=False)
            Dim v As Double() = New Double(mzIndex.size - 1) {}
            Dim size As Integer = mzIndex.size
            Dim fragments As ms2()
            Dim mz As Double()
            Dim intensity As Double()

            For Each spec As T In pool
                fragments = spec.GetIons.ToArray
                mz = fragments.Select(Function(a) a.mz).ToArray
                intensity = fragments.Select(Function(a) a.intensity).ToArray
                v = SIMD.Add.f64_op_add_f64(v, DeconvoluteScan(mz, intensity, size, mzIndex))
            Next

            If average Then
                v = SIMD.Divide.f64_op_divide_f64_scalar(v, pool.Count)
            End If

            v = SIMD.Divide.f64_op_divide_f64_scalar(v, v.Max)

            Return New LibraryMatrix(mzIndex.ionSet, v)
        End Function

        <Extension>
        Public Function CreateCentroidFragmentSet(fragments As IEnumerable(Of Double),
                                                  Optional centroid As Double = 0.1,
                                                  Optional window_size As Double = 1,
                                                  Optional verbose As Boolean = True) As MzPool
            Dim mzgroups = fragments _
                .GroupBy(offset:=centroid) _
                .Select(Function(a) Val(a.name)) _
                .OrderBy(Function(mzi) mzi) _
                .ToArray
            Dim pool As New MzPool(mzgroups,
                                   win_size:=window_size,
                                   verbose:=verbose)
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
        Public Iterator Function Decode(mz64 As String, into64 As String,
                                        Optional gzip As NetworkByteOrderBuffer.Compression = NetworkByteOrderBuffer.Compression.none,
                                        Optional no_magic As Boolean = False) As IEnumerable(Of ms2)

            Dim mz As Double() = network.ParseDouble(mz64, gzip, no_magic)
            Dim into As Double() = network.ParseDouble(into64, gzip, no_magic)

            For i As Integer = 0 To mz.Length - 1
                Yield New ms2(mz(i), into(i))
            Next
        End Function

        ''' <summary>
        ''' 对base64字符串做解码，重新生成质谱图
        ''' </summary>
        ''' <param name="base64"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' the text format description about the input data:
        ''' 
        ''' input data should be a text encoded matrix data:
        ''' 
        ''' each line is a ion fragment data, in layout of [mz][tab][intensity]
        ''' then multiple ion fragments data consist the matrix text
        ''' 
        ''' the matrix text then encoded as base64 string in utf-8 text encoded format.
        ''' </remarks>
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
        Public Function LibraryMatrix(decode As (x#, y#)(), Optional name As String = Nothing) As LibraryMatrix
            Dim msms = decode _
                .Select(Function(d)
                            Return New ms2 With {
                                .mz = d.x,
                                .intensity = d.y
                            }
                        End Function) _
                .ToArray

            Return New LibraryMatrix(name, msms)
        End Function
    End Module
End Namespace
