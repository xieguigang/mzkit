Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Math

Public MustInherit Class FeatureExtract(Of Sample)

    ReadOnly peakwidth As DoubleRange

    Sub New(peakwidth As DoubleRange)
        Me.peakwidth = peakwidth
    End Sub

    Public MustOverride Function GetSamplePeaks(sample As Sample) As IEnumerable(Of TargetPeakPoint)

    Protected Function GetTICPeaks(TIC As IEnumerable(Of ChromatogramTick)) As IEnumerable(Of ROI)
        Return TIC.Shadows.PopulateROI(peakwidth)
    End Function

End Class
