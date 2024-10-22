Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Public Class DimensionalSpectrum

    ''' <summary>
    ''' the rt of dimension 1
    ''' </summary>
    ''' <returns></returns>
    Public Property rt1 As Double
    ''' <summary>
    ''' the dimension 2 spectrum data
    ''' </summary>
    ''' <returns></returns>
    Public Property ms2 As PeakMs2()

    Public Property totalIon As Double
    Public Property baseIntensity As Double

End Class
