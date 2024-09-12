Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml

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

    Public Property forward As Double
    Public Property reverse As Double
    Public Property jaccard As Double
    Public Property entropy As Double

    ''' <summary>
    ''' the ms2 spectrum of current alignment hit result
    ''' </summary>
    ''' <returns></returns>
    Public Property ms2 As SSM2MatrixFragment()
    ''' <summary>
    ''' the source file name
    ''' </summary>
    ''' <returns></returns>
    Public Property source As String

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetSampleSpectrum() As IEnumerable(Of ms2)
        Return ms2 _
            .Where(Function(a) a.query > 0) _
            .Select(Function(a)
                        Return New ms2(a.mz, a.query)
                    End Function)
    End Function

    Public Overrides Function ToString() As String
        Return $"{libname}@{source}"
    End Function

End Class