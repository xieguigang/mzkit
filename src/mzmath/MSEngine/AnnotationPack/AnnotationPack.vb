
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

''' <summary>
''' Result data pack for save the annotation result data
''' </summary>
Public Class AnnotationPack

    ''' <summary>
    ''' the ms2 spectrum alignment search hits
    ''' </summary>
    ''' <returns></returns>
    Public Property libraries As Dictionary(Of String, AlignmentHit)

    ''' <summary>
    ''' the ms1 peaktable
    ''' </summary>
    ''' <returns></returns>
    Public Property peaks As xcms2()

End Class

Public Class AlignmentHit

    Public Property xcms_id As String
    Public Property libname As String
    Public Property mz As Double
    Public Property rt As Double
    Public Property RI As Double
    Public Property theoretical_mz As Double
    Public Property exact_mass As Double
    Public Property adducts As String
    Public Property ppm As Double
    Public Property occurrences As Integer
    Public Property biodeep_id As String
    Public Property name As String
    Public Property formula As String
    Public Property npeaks As Integer

    ''' <summary>
    ''' sample hits of current library reference
    ''' </summary>
    ''' <returns></returns>
    Public Property samplefiles As Dictionary(Of String, Ms2Score)

End Class

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