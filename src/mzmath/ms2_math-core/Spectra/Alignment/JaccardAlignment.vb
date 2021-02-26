Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace Spectra

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