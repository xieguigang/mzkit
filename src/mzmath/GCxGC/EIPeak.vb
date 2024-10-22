Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Public Class EIPeak(Of T)

    Public Property peak As T
    Public Property spectrum As LibraryMatrix()

    Public Function GetRepresentativeSpectrum(Optional centroid As Double = 0.1) As LibraryMatrix
        Return spectrum.SpectrumSum(centroid, average:=True)
    End Function

    Public Overrides Function ToString() As String
        Return peak.ToString
    End Function

End Class
