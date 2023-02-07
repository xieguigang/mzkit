Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml

Namespace Spectra

    Public Class EntropyAlignment : Inherits AlignmentProvider

        Public Sub New(mzwidth As Tolerance, intocutoff As LowAbundanceTrimming)
            MyBase.New(mzwidth, intocutoff)
        End Sub

        Public Overrides Function GetScore(a() As ms2, b() As ms2) As Double
            Return SpectralEntropy.calculate_entropy_similarity(a, b, mzwidth)
        End Function

        Public Overrides Function GetScore(alignment() As SSM2MatrixFragment) As (forward As Double, reverse As Double)
            Dim score As Double = SpectralEntropy.calculate_entropy_similarity(alignment)
            Return (score, score)
        End Function
    End Class
End Namespace