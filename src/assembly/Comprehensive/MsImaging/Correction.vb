Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache

Namespace MsImaging

    Public MustInherit Class Correction

        Public MustOverride Function GetPixelRowX(scanMs1 As ScanMS1) As Integer

    End Class
End Namespace