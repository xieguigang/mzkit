Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

''' <summary>
''' Make sample spectrum aligns to the peaktable based on the pearson correlation method
''' </summary>
Public Class SpectrumGrid

    Private Iterator Function Clustering(rawdata As IEnumerable(Of NamedCollection(Of PeakMs2))) As IEnumerable(Of NamedCollection(Of PeakMs2))

    End Function

    Public Iterator Function AssignPeaks(peaks As IEnumerable(Of xcms2)) As IEnumerable(Of (peak As xcms2, ms2 As PeakMs2()))

    End Function

End Class
