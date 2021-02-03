Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Namespace GCMS.QuantifyAnalysis

    Public MustInherit Class QuantifyIonExtract : Inherits FeatureExtract(Of Raw)

        Protected ReadOnly ions As QuantifyIon()
        Protected ReadOnly ms1ppm As Tolerance
        Protected ReadOnly dadot3 As Tolerance = Tolerance.DeltaMass(0.3)

        Protected Sub New(ions As IEnumerable(Of QuantifyIon), peakwidth As DoubleRange, centroid As Tolerance)
            Call MyBase.New(peakwidth)

            Me.ms1ppm = centroid
            Me.ions = ions.ToArray
        End Sub

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
    End Class
End Namespace