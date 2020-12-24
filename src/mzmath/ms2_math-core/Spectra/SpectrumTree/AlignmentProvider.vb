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

    Public Class CosAlignment : Inherits AlignmentProvider

        Public Sub New(mzwidth As Tolerance, intocutoff As LowAbundanceTrimming)
            MyBase.New(mzwidth, intocutoff)
        End Sub

        Public Overrides Function GetScore(a As ms2(), b As ms2()) As Double
            Dim scores = GlobalAlignment.TwoDirectionSSM(a, b, mzwidth)
            Dim min As Double = stdNum.Min(scores.forward, scores.reverse)

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