Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports stdNum = System.Math

Namespace Spectra

    Public MustInherit Class AlignmentProvider

        Protected intocutoff As LowAbundanceTrimming
        Protected mzwidth As Tolerance

        Sub New(mzwidth As Tolerance, intocutoff As LowAbundanceTrimming)
            Me.mzwidth = mzwidth
            Me.intocutoff = intocutoff
        End Sub

        Public MustOverride Function GetScore(a As ms2(), b As ms2()) As Double

        Public MustOverride Function CreateAlignment(a As PeakMs2, b As PeakMs2) As AlignmentOutput

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

        Public Overrides Function CreateAlignment(a As PeakMs2, b As PeakMs2) As AlignmentOutput
            Throw New NotImplementedException()
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

        Public Overrides Function CreateAlignment(a As PeakMs2, b As PeakMs2) As AlignmentOutput
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace