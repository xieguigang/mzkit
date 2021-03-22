#Region "Microsoft.VisualBasic::e863c3f63f4a3bb248dff26d1a855230, src\mzmath\ms2_math-core\Spectra\Alignment\CosAlignment.vb"

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

    '     Class CosAlignment
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) GetScore
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

End Namespace
