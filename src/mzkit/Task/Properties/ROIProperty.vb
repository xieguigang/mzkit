Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram

Public Class ROIProperty

    Public Property rtmin As Double
    Public Property rtmax As Double
    Public Property rt As Double
    Public Property peakArea As Double
    Public Property baseline As Double

    Sub New(ROI As ROI)
        rtmin = ROI.time.Min
        rtmax = ROI.time.Max
        rt = ROI.rt
        peakArea = ROI.integration
        baseline = ROI.baseline
    End Sub
End Class
