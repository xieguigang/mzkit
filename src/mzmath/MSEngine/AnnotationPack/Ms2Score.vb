
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

''' <summary>
''' the ms2 alignment result
''' </summary>
Public Class Ms2Score

    ''' <summary>
    ''' the precursor m/z of the ms2 spectrum
    ''' </summary>
    ''' <returns></returns>
    Public Property precursor As Double
    ''' <summary>
    ''' rt of the ms2 spectrum precursor ion
    ''' </summary>
    ''' <returns></returns>
    Public Property rt As Double
    ''' <summary>
    ''' intensity of the ms2 spectrum its precursor ion
    ''' </summary>
    ''' <returns></returns>
    Public Property intensity As Double
    ''' <summary>
    ''' the library hit name
    ''' </summary>
    ''' <returns></returns>
    Public Property libname As String
    ''' <summary>
    ''' ms2 alignment score
    ''' </summary>
    ''' <returns></returns>
    Public Property score As Double
    ''' <summary>
    ''' the ms2 spectrum of current alignment hit result
    ''' </summary>
    ''' <returns></returns>
    Public Property ms2 As ms2()
    ''' <summary>
    ''' the source file name
    ''' </summary>
    ''' <returns></returns>
    Public Property source As String

End Class