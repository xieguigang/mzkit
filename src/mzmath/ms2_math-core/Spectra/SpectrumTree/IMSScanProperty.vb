Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType

Namespace Spectra

    Public Interface IMSScanProperty
        Inherits IMSProperty
        Property ScanID As Integer
        Property Spectrum As List(Of SpectrumPeak)
        Sub AddPeak(ByVal mass As Double, ByVal intensity As Double, ByVal Optional comment As String = Nothing)
    End Interface

    Public Interface IMSProperty
        Property ChromXs As ChromXs
        Property IonMode As IonModes
        Property PrecursorMz As Double
    End Interface
End Namespace