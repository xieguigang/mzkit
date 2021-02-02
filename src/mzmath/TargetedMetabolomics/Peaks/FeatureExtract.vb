Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear

Public MustInherit Class FeatureExtract(Of Sample)

    Protected MustOverride Function GetSamplePeaks(sample As Sample) As IEnumerable(Of TargetPeakPoint)

End Class
