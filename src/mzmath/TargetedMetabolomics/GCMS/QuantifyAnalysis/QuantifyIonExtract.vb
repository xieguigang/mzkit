Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports stdNum = System.Math

Namespace GCMS.QuantifyAnalysis

    Public MustInherit Class QuantifyIonExtract : Inherits FeatureExtract(Of Raw)

        Protected Friend ReadOnly ions As QuantifyIon()
        ''' <summary>
        ''' apply for convert to centroid data
        ''' </summary>
        Protected Friend ReadOnly ms1ppm As Tolerance
        Protected Friend ReadOnly dadot3 As Tolerance = Tolerance.DeltaMass(0.3)

        Protected Sub New(ions As IEnumerable(Of QuantifyIon), peakwidth As DoubleRange, centroid As Tolerance)
            Call MyBase.New(peakwidth)

            Me.ms1ppm = centroid
            Me.ions = ions.ToArray
        End Sub

        Public Overrides Iterator Function GetSamplePeaks(sample As Raw) As IEnumerable(Of TargetPeakPoint)
            Dim ROI As ROI() = GetTICPeaks(sample.GetTIC).ToArray
            Dim rtmin As Vector = ROI.Select(Function(r) r.time.Min).ToArray
            Dim rtmax As Vector = ROI.Select(Function(r) r.time.Max).ToArray
            Dim ms As ms2()() = ROI.Select(Function(r) GetMsScan(sample, r.time)).ToArray
            Dim feature As ROI

            For Each ion As QuantifyIon In ions
                Dim rtminScore As Vector = (ion.rt.Min - rtmin).Abs
                Dim rtmaxScore As Vector = (ion.rt.Max - rtmax).Abs
                Dim cos As Vector = ms _
                    .Select(Function(spectra)
                                With GlobalAlignment.TwoDirectionSSM(ion.ms, spectra, dadot3)
                                    Return stdNum.Min(.forward, .reverse)
                                End With
                            End Function) _
                    .ToArray

                rtminScore = rtminScore.Max / rtminScore - 1
                rtmaxScore = rtmaxScore.Max / rtmaxScore - 1

                Dim scores As Vector = (rtminScore + rtmaxScore) * cos

                If scores.All(Function(xi) xi = 0.0) Then
                    Continue For
                Else
                    feature = ROI(Which.Max(scores))
                End If

                Yield GetPeak(ion.id, feature.time, sample)
            Next
        End Function

        Protected Function GetMsScan(sample As Raw, rt As DoubleRange) As ms2()
            Dim ms_scan As ms1_scan() = sample.GetMsScan(rt)
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

            Return spectra
        End Function

        Protected MustOverride Function GetPeak(ion_id As String, rt As DoubleRange, sample As Raw) As TargetPeakPoint

    End Class
End Namespace