Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Public Class EIPeak(Of T)

    ''' <summary>
    ''' the ms1 peak
    ''' </summary>
    ''' <returns></returns>
    Public Property peak As T
    ''' <summary>
    ''' all spectrum that pick from current peak rt range.
    ''' </summary>
    ''' <returns></returns>
    Public Property spectrum As LibraryMatrix()

    ''' <summary>
    ''' make average spectrum of current <see cref="spectrum"/> collection.
    ''' </summary>
    ''' <param name="centroid"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetRepresentativeSpectrum(Optional centroid As Double = 0.1) As LibraryMatrix
        Return spectrum.SpectrumSum(centroid, average:=True)
    End Function

    Public Overrides Function ToString() As String
        Return peak.ToString
    End Function

End Class
