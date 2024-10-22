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

    ''' <summary>
    ''' total ion of current dimension 1 ms1 scan data
    ''' </summary>
    ''' <returns></returns>
    Public Property totalIon As Double
    ''' <summary>
    ''' the base peak intensity value of current dimension 1 ms1 scan data 
    ''' </summary>
    ''' <returns></returns>
    Public Property baseIntensity As Double

End Class
