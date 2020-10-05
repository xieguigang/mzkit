Imports Microsoft.VisualBasic.Math.Quantile

Namespace Spectra

    Public Class QuantileIntensityCutoff : Inherits LowAbundanceTrimming

        Public Sub New(cutoff As Double)
            MyBase.New(cutoff)
        End Sub

        Protected Overrides Function lowAbundanceTrimming(spectrum() As ms2) As ms2()
            Dim quantile = spectrum.Select(Function(a) a.intensity).GKQuantile
            Dim threshold As Double = quantile.Query(Me.m_threshold)

            Return spectrum.Where(Function(a) a.intensity >= threshold).ToArray
        End Function

        Public Overrides Function ToString() As String
            Return $"intensity_quantile >= {m_threshold}"
        End Function

    End Class
End Namespace