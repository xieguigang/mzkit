Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports stdNum = System.Math

Namespace GCMS.QuantifyAnalysis

    Public Class SIMIonExtract : Inherits FeatureExtract(Of Raw)

        ReadOnly ions As QuantifyIon()
        ReadOnly ms1ppm As Tolerance
        ReadOnly dadot3 As Tolerance = Tolerance.DeltaMass(0.3)

        Public Sub New(ions As IEnumerable(Of QuantifyIon), peakwidth As DoubleRange, centroid As Tolerance)
            Call MyBase.New(peakwidth)

            Me.ms1ppm = centroid
            Me.ions = ions.ToArray
        End Sub

        Public Overrides Iterator Function GetSamplePeaks(sample As Raw) As IEnumerable(Of TargetPeakPoint)
            Dim sampleName As String = sample.fileName.BaseName
            Dim ROI As ROI() = GetTICPeaks(sample.GetTIC).ToArray

            For Each ion As QuantifyIon In ions
                Dim ms_scan As ms1_scan() = sample.GetMsScan(ion.rt)
                Dim spectra As ms2() = ms_scan _
                    .Select(Function(scan)
                                Return New ms2 With {
                                    .mz = scan.mz,
                                    .quantity = scan.intensity,
                                    .intensity = .quantity
                                }
                            End Function) _
                    .ToArray _
                    .Centroid(ms1ppm, LowAbundanceTrimming.Default) _
                    .ToArray
                Dim scores = GlobalAlignment.TwoDirectionSSM(ion.ms, spectra, dadot3)

                If stdNum.Min(scores.forward, scores.reverse) < 0.65 Then
                    Continue For
                End If

                ' SIM模式下只使用响应度最高的碎片做定量计算
                Dim maxInto As Double = spectra _
                    .Select(Function(mz) mz.intensity) _
                    .Max
                Dim tick As New ChromatogramTick With {
                    .Intensity = maxInto,
                    .Time = ion.rt.Average
                }
                Dim q As New Quantile With {
                    .Quantile = 1,
                    .Percentage = 1
                }

                Yield New TargetPeakPoint With {
                    .Name = ion.id,
                    .SampleName = sampleName,
                    .Peak = New ROIPeak With {
                        .base = 0,
                        .peakHeight = maxInto,
                        .ticks = {tick},
                        .window = New DoubleRange(ion.rt)
                    },
                    .ChromatogramSummary = {q}
                }
            Next
        End Function
    End Class
End Namespace