Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' tools for decode GC-MS raw data
''' </summary>
Public Module GCMSDeconv

    ''' <summary>
    ''' this function deconvolution of the gc-ms TIC data, extract the corresponding peaks,
    ''' and then extract the ms1 spectrum fro each detected peaks data.
    ''' </summary>
    ''' <param name="gcms">should be a collection of the ms1 scan data which is extract from the rawdata files.</param>
    ''' <returns></returns>
    Public Iterator Function DeconvGCMSRawdata(gcms As IEnumerable(Of PeakMs2), peakwidth As DoubleRange,
                                               Optional quantile# = 0.65,
                                               Optional sn_threshold As Double = 3,
                                               Optional joint As Boolean = True) As IEnumerable(Of GCMSPeak)

        Dim pool As PeakMs2() = gcms.SafeQuery.ToArray
        Dim TIC As ChromatogramTick() = pool.ToChromatogram.OrderBy(Function(t) t.Time).ToArray
        Dim peaks = TIC.DeconvPeakGroups(peakwidth, quantile, sn_threshold, joint).ToArray
        Dim da As Tolerance = Tolerance.DeltaMass(0.3)
        Dim cutoff As New RelativeIntensityCutoff(0.05)

        For Each peak As PeakFeature In peaks
            Dim spectrumMs1 = (From scan As PeakMs2
                               In pool.AsParallel
                               Where scan.rt >= peak.rtmin AndAlso scan.rt <= peak.rtmax).ToArray
            Dim union As ms2() = spectrumMs1 _
                .Select(Function(p) p.mzInto) _
                .IteratesALL _
                .ToArray _
                .CentroidSum(da, cutoff) _
                .ToArray

            Yield New GCMSPeak(peak) With {.Spectrum = union}
        Next
    End Function
End Module

Public Class GCMSPeak : Inherits PeakFeature

    Public Property Spectrum As ms2()

    Sub New()
    End Sub

    Sub New(peakdata As PeakFeature)
        Call MyBase.New(peakdata)
    End Sub

End Class