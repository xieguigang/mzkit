Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace Configuration

    Public Class RawFileViewerSettings

        Public Property XIC_ppm As Double = 20
        Public Property colorSet As String()

        Public Property method As TrimmingMethods = TrimmingMethods.RelativeIntensity
        Public Property intoCutoff As Double = 0.05
        Public Property quantile As Double = 0.65
        Public Property fill As Boolean = True

        Public Function GetMethod() As LowAbundanceTrimming
            If method = TrimmingMethods.RelativeIntensity Then
                Return New RelativeIntensityCutoff(intoCutoff)
            Else
                Return New QuantileIntensityCutoff(quantile)
            End If
        End Function

    End Class

    Public Enum TrimmingMethods
        RelativeIntensity
        Quantile
    End Enum
End Namespace