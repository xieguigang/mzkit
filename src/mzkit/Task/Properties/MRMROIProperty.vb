Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Public Class MRMROIProperty

    Public Property precursor As Double
    Public Property product As Double
    Public Property rtmin As Double
    Public Property rtmax As Double
    Public Property rt As Double
    Public Property peakArea As Double
    Public Property baseline As Double

    Sub New(chr As chromatogram)
        Dim TIC = chr.Ticks

        If chr.precursor IsNot Nothing AndAlso chr.product IsNot Nothing Then
            precursor = chr.precursor.MRMTargetMz
            product = chr.product.MRMTargetMz
        End If

        Dim ROI = TIC.Shadows _
            .PopulateROI(
                baselineQuantile:=0.65,
                angleThreshold:=5,
                peakwidth:=New Double() {8, 30},
                snThreshold:=3
            ) _
          .OrderByDescending(Function(r) r.integration).FirstOrDefault

        If Not ROI Is Nothing Then
            rtmin = ROI.time.Min
            rtmax = ROI.time.Max
            rt = ROI.rt
            peakArea = ROI.integration
            baseline = ROI.baseline
        End If
    End Sub
End Class
