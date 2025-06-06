﻿#Region "Microsoft.VisualBasic::dbdafea5a31cf656a30422f537a472da, mzmath\ms2_math-core\Spectra\GlobalAlignment.vb"

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

    '   Total Lines: 259
    '    Code Lines: 136 (52.51%)
    ' Comment Lines: 90 (34.75%)
    '    - Xml Docs: 56.67%
    ' 
    '   Blank Lines: 33 (12.74%)
    '     File Size: 11.03 KB


    '     Module GlobalAlignment
    ' 
    '         Properties: ppm20
    ' 
    '         Function: Align, AlignMatrix, CreateAlignment, CreateAnnotation, findMatch
    '                   (+3 Overloads) JaccardIndex, MzIntersect, MzUnion, SharedPeakCount, TopPeaks
    '                   TwoDirectionSSM
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Extensions
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Scripting
Imports std = System.Math

Namespace Spectra

    ''' <summary>
    ''' Global alignment of two MS/MS matrix.
    ''' </summary>
    Public Module GlobalAlignment

        Public ReadOnly Property ppm20 As [Default](Of Tolerance) = Tolerance.PPM(20)

        '''' <summary>
        '''' ### shared peak count
        '''' 
        '''' In the matrix ``M``, ``i`` and ``j`` are positions, where ``i`` is the
        '''' horizontal coordinate And ``j`` Is the vertical coordinate. For Each
        '''' cell in the matrix, a score Is calculated (Si,j). If the two compared
        '''' masses are a match, Then ``C`` the cost Is equal To 0. However, If
        '''' the two masses are outside the designated ppm Error window,
        '''' then the cost Is equal to 1.
        '''' </summary>
        '''' <param name="query"></param>
        '''' <param name="subject"></param>
        '''' <returns></returns>
        'Public Function NWGlobalAlign(query As LibraryMatrix, subject As LibraryMatrix, Optional tolerance As Tolerance = Nothing) As GlobalAlign(Of ms2)()
        '    Dim massEquals As IEquals(Of ms2)
        '    Dim empty As New ms2 With {
        '        .mz = -1,
        '        .intensity = -1,
        '        .quantity = -1
        '    }

        '    With tolerance Or da3
        '        massEquals = Function(q, s)
        '                         Return .Assert(q.mz, s.mz)
        '                     End Function
        '    End With

        '    With New NeedlemanWunsch(Of ms2)(query, subject, massEquals, empty, Function(ms2) ms2.ToString.First).Compute()
        '        Return .PopulateAlignments.ToArray
        '    End With
        'End Function

        ''' <summary>
        ''' 取出响应度前<paramref name="top"/>个数量的质谱图碎片
        ''' </summary>
        ''' <param name="spectra"></param>
        ''' <param name="top%"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function TopPeaks(spectra As LibraryMatrix, top%) As IEnumerable(Of ms2)
            Return spectra _
                .OrderByDescending(Function(mz) mz.intensity) _
                .Take(top)
        End Function

        ''' <summary>
        ''' 只计算响应度最高的前<paramref name="top"/>个二级碎片之中的相同mz的碎片数量
        ''' </summary>
        ''' <param name="query"></param>
        ''' <param name="subject"></param>
        ''' <param name="tolerance"></param>
        ''' <param name="top%"></param>
        ''' <returns></returns>
        Public Function SharedPeakCount(query As LibraryMatrix, subject As LibraryMatrix,
                                        Optional tolerance As Tolerance = Nothing,
                                        Optional top% = 10) As Integer

            Dim q As ms2() = query.TopPeaks(top).ToArray
            Dim s As ms2() = subject.TopPeaks(top).ToArray

            With tolerance Or Tolerance.DefaultTolerance
                Dim share As Integer = s _
                    .Where(Function(mz As ms2)
                               Dim find As ms2 = q _
                                   .Where(Function(frag)
                                              Return .Equals(frag.mz, mz.mz)
                                          End Function) _
                                   .FirstOrDefault
                               Return Not find Is Nothing
                           End Function) _
                    .Count

                Return share
            End With
        End Function

        ''' <summary>
        ''' 计算两个质谱图之间的杰卡德相似性系数
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <param name="tolerance"></param>
        ''' <param name="topSet"></param>
        ''' <returns></returns>
        Public Function JaccardIndex(x As ms2(), y As ms2(), tolerance As Tolerance, Optional topSet As Integer = 5) As Double
            Dim mzx As Double() = x.OrderByDescending(Function(a) a.intensity).Take(topSet).Select(Function(a) a.mz).ToArray
            Dim mzy As Double() = y.OrderByDescending(Function(a) a.intensity).Take(topSet).Select(Function(a) a.mz).ToArray

            Return JaccardIndex(mzx, mzy, tolerance)
        End Function

        Public Function MzIntersect(mzx As Double(), mzy As Double(), tolerance As Tolerance) As Double()
            Dim intersects As New List(Of Double)

            For Each xi In mzx
                For Each yi In mzy
                    If tolerance(xi, yi) Then
                        intersects.Add(xi)
                        Exit For
                    End If
                Next
            Next

            Return intersects.ToArray
        End Function

        Public Function JaccardIndex(mzx As Double(), mzy As Double(), tolerance As Tolerance) As Double
            Dim union As Double() = MzUnion(mzx, mzy, tolerance)
            Dim intersects As Double() = MzIntersect(mzx, mzy, tolerance)

            Return intersects.Length / union.Length
        End Function

        <Extension>
        Public Function JaccardIndex(align As SSM2MatrixFragment()) As Double
            Dim union As Integer = align.Length
            Dim intersects As Integer = align _
                .Where(Function(a) a.query > 0 AndAlso a.ref > 0) _
                .Count

            Return intersects / union
        End Function

        Public Function MzUnion(mzx As Double(), mzy As Double(), tolerance As Tolerance) As Double()
            Return mzx _
                .JoinIterates(mzy) _
                .GroupBy(Function(mz) mz, tolerance.Equals) _
                .Select(Function(a) Val(a.name)) _
                .ToArray
        End Function

        ''' <summary>
        ''' xy分别为预测或者标准品的结果数据，无顺序之分，并且这两个质谱图数据应该是经过centroid化之后的
        ''' </summary>
        ''' <param name="x">
        ''' 
        ''' </param>
        ''' <param name="y"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function TwoDirectionSSM(x As ms2(), y As ms2(), tolerance As Tolerance) As (forward#, reverse#)
            Return (GlobalAlignment.Align(x, y, tolerance), GlobalAlignment.Align(y, x, tolerance))
        End Function

        ''' <summary>
        ''' 以<paramref name="ref"/>为基准，从<paramref name="query"/>之中选择出对应的<see cref="ms2.mz"/>信号响应信息，完成对齐操作
        ''' </summary>
        ''' <param name="query"></param>
        ''' <param name="ref"></param>
        ''' <returns></returns>
        Public Function Align(query As ms2(), ref As ms2(), Optional tolerance As Tolerance = Nothing) As Double
            If query.IsNullOrEmpty OrElse ref.IsNullOrEmpty Then
                Return 0
            Else
                Dim q As Vector = query.AlignMatrix(ref, tolerance Or ppm20).Shadows!intensity
                Dim s As Vector = ref.Shadows!intensity

                Return SSM(q / q.Max, s / s.Max)
            End If
        End Function

        ''' <summary>
        ''' 在ref之中找不到对应的mz，则into为零
        ''' </summary>
        ''' <param name="query"></param>
        ''' <param name="ref"></param>
        ''' <param name="tolerance"></param>
        ''' <returns></returns>
        <Extension>
        Public Function AlignMatrix(query As ms2(), ref As ms2(), tolerance As Tolerance) As ms2()
            Return ref _
                .Select(Function(mz)
                            Return findMatch(mz, query, tolerance)
                        End Function) _
                .ToArray
        End Function

        Private Function findMatch(mz As ms2, query As ms2(), tolerance As Tolerance) As ms2
            ' 2017-10-29
            '
            ' 当找不到的时候，会返回一个空的structure对象，这个时候intensity为零
            ' 所以在这个Linq表达式中，后面不需要使用Where来删除对象了
            Dim match = query _
                .Where(Function(q) tolerance(q.mz, mz.mz)) _
                .ToArray

            If match.Length = 0 Then
                ' With single intensity ZERO
                Return New ms2 With {
                    .mz = mz.mz,
                    .intensity = 0.0
                }
            Else
                Dim subject As IVector(Of ms2) = match.Shadows
                ' 返回响应值最大的
                Return subject(which.Max(subject!intensity))
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="query">the sample query data</param>
        ''' <param name="refrs">the reference data</param>
        ''' <param name="tolerance"></param>
        ''' <returns></returns>
        Public Iterator Function CreateAlignment(query As ms2(), refrs As ms2(), tolerance As Tolerance) As IEnumerable(Of SSM2MatrixFragment)
            Dim union = MzUnion(query.Select(Function(m) m.mz).ToArray, refrs.Select(Function(m) m.mz).ToArray, tolerance)

            For Each mz As Double In union
                Dim qmz = query.Where(Function(a) tolerance(a.mz, mz)).FirstOrDefault
                Dim rmz = refrs.Where(Function(a) tolerance(a.mz, mz)).FirstOrDefault

                Yield New SSM2MatrixFragment With {
                    .mz = mz,
                    .query = If(qmz Is Nothing, 0, qmz.intensity),
                    .ref = If(rmz Is Nothing, 0, rmz.intensity),
                    .da = If(qmz Is Nothing OrElse rmz Is Nothing, Double.NaN, std.Abs(qmz.mz - rmz.mz)),
                    .annotation = CreateAnnotation(qmz, rmz)
                }
            Next
        End Function

        Private Function CreateAnnotation(q As ms2, r As ms2) As String
            If q Is Nothing OrElse q.Annotation.StringEmpty(, True) Then
                Return r?.Annotation
            End If

            If r Is Nothing OrElse r.Annotation.StringEmpty(, True) Then
                Return q?.Annotation
            End If

            Return {q.Annotation, r.Annotation}.Distinct.JoinBy("_")
        End Function
    End Module
End Namespace
