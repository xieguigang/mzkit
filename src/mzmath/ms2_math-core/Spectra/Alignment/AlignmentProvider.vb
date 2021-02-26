#Region "Microsoft.VisualBasic::c9a844eff93634351c0d5c3755e04bee, ms2_math-core\Spectra\SpectrumTree\AlignmentProvider.vb"

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

'     Class AlignmentProvider
' 
'         Constructor: (+1 Overloads) Sub New
' 
'     Enum ScoreAggregates
' 
'         max, min, sum
' 
'  
' 
' 
' 
'     Class CosAlignment
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: CreateAlignment, GetScore
' 
'     Class JaccardAlignment
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: CreateAlignment, GetScore
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports stdNum = System.Math
Imports vec = Microsoft.VisualBasic.Math

Namespace Spectra

    Public MustInherit Class AlignmentProvider

        Protected intocutoff As LowAbundanceTrimming
        Protected mzwidth As Tolerance

        Sub New(mzwidth As Tolerance, intocutoff As LowAbundanceTrimming)
            Me.mzwidth = mzwidth
            Me.intocutoff = intocutoff
        End Sub

        Public MustOverride Function GetScore(a As ms2(), b As ms2()) As Double
        Public MustOverride Function GetScore(alignment As SSM2MatrixFragment()) As (forward#, reverse#)

        Public Function CreateAlignment(a As PeakMs2, b As PeakMs2) As AlignmentOutput
            Dim align As AlignmentOutput = CreateAlignment(a.mzInto, b.mzInto)

            align.query = GetMeta(a)
            align.reference = GetMeta(b)

            Return align
        End Function

        Private Shared Function GetMeta(peak As PeakMs2) As Meta
            Return New Meta With {
                .id = peak.lib_guid,
                .mz = peak.mz,
                .rt = peak.rt
            }
        End Function

        Public Function CreateAlignment(a As ms2(), b As ms2()) As AlignmentOutput
            Dim align As SSM2MatrixFragment() = GlobalAlignment _
                .CreateAlignment(a, b, mzwidth) _
                .ToArray
            Dim scores As (forward#, reverse#) = GetScore(align)

            Return New AlignmentOutput With {
                .alignments = align,
                .forward = scores.forward,
                .reverse = scores.reverse
            }
        End Function

    End Class

    Public Enum ScoreAggregates
        min
        max
        sum
    End Enum

    Public Class CosAlignment : Inherits AlignmentProvider

        ''' <summary>
        ''' 1. min
        ''' 2. max
        ''' 3. sum
        ''' </summary>
        ReadOnly scoreAggregate As Func(Of Double, Double, Double)

        Public Sub New(mzwidth As Tolerance, intocutoff As LowAbundanceTrimming, Optional aggregate As ScoreAggregates = ScoreAggregates.min)
            MyBase.New(mzwidth, intocutoff)

            Select Case aggregate
                Case ScoreAggregates.min : scoreAggregate = AddressOf stdNum.Min
                Case ScoreAggregates.max : scoreAggregate = AddressOf stdNum.Max
                Case ScoreAggregates.sum : scoreAggregate = Function(a, b) a + b
                Case Else
                    Throw New InvalidProgramException(aggregate)
            End Select
        End Sub

        Public Overrides Function GetScore(a As ms2(), b As ms2()) As Double
            Dim scores = GlobalAlignment.TwoDirectionSSM(a, b, mzwidth)
            Dim min As Double = scoreAggregate(scores.forward, scores.reverse)

            Return min
        End Function

        Public Overrides Function GetScore(alignment() As SSM2MatrixFragment) As (forward As Double, reverse As Double)
            Dim a, b As Vector
            Dim align As SSM2MatrixFragment()
            Dim forward, reverse As Double

            ' forward
            align = alignment _
                .Where(Function(x)
                           Return Not x.query.IsNaNImaginary AndAlso x.query > 0
                       End Function) _
                .ToArray

            a = align.Select(Function(x) x.query).AsVector
            b = align.Select(Function(x) If(x.ref.IsNaNImaginary, 0, x.ref)).AsVector

            forward = vec.SSM(a, b)

            ' reverse
            align = alignment _
                .Where(Function(x)
                           Return Not x.ref.IsNaNImaginary AndAlso x.ref > 0
                       End Function) _
                .ToArray

            a = align.Select(Function(x) If(x.query.IsNaNImaginary, 0, x.query)).AsVector
            b = align.Select(Function(x) x.ref).AsVector

            reverse = vec.SSM(a, b)

            Return (forward, reverse)
        End Function
    End Class

    Public Class JaccardAlignment : Inherits AlignmentProvider

        ReadOnly topSet As Integer

        Public Sub New(mzwidth As Tolerance, intocutoff As LowAbundanceTrimming, Optional topSet As Integer = 5)
            MyBase.New(mzwidth, intocutoff)

            Me.topSet = topSet
        End Sub

        Public Overrides Function GetScore(a As ms2(), b As ms2()) As Double
            Return GlobalAlignment.JaccardIndex(a, b, mzwidth, topSet)
        End Function

        Public Overrides Function GetScore(alignment() As SSM2MatrixFragment) As (forward As Double, reverse As Double)
            Static NA As Index(Of String) = {"NA", "n/a", "NaN"}

            Dim intersect As Integer = Aggregate a As SSM2MatrixFragment
                                       In alignment
                                       Where Not a.da Like NA
                                       Into Sum(1)
            Dim union As Integer = alignment.Length
            Dim J As Double = intersect / union

            Return (J, J)
        End Function
    End Class
End Namespace
