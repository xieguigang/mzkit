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