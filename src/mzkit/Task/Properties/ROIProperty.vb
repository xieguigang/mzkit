Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram

Public Class ROIProperty

    <Description("The min value of the rt of current region of interested on the chromatography.")>
    Public Property rtmin As Double
    <Description("The max value of the rt of current region of interested on the chromatography.")>
    Public Property rtmax As Double
    <Description("The time point of the max intensity point of current region of interested on the chromatography.")>
    Public Property rt As Double
    <Description("The peak area of current region of interested on the chromatography.")>
    Public Property peakArea As Double
    <Description("The noise baseline value of current region of interested on the chromatography.")>
    Public Property baseline As Double

    Sub New(ROI As ROI)
        rtmin = ROI.time.Min
        rtmax = ROI.time.Max
        rt = ROI.rt
        peakArea = ROI.integration
        baseline = ROI.baseline
    End Sub
End Class
