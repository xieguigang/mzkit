Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Namespace GCMS.QuantifyAnalysis

    Public Class ScanIonExtract : Inherits QuantifyIonExtract

        Public Sub New(ions As IEnumerable(Of QuantifyIon), peakwidth As DoubleRange, centroid As Tolerance)
            MyBase.New(ions, peakwidth, centroid)
        End Sub

        Protected Overrides Function GetPeak(ion_id As String, rt As DoubleRange, sample As Raw) As TargetPeakPoint
            Dim sampleName As String = sample.fileName.BaseName
            Dim spectra As ms1_scan() = sample.GetMsScan(rt)
            Dim tick As ChromatogramTick() = spectra _
                .Select(Function(mzi)
                            Return New ChromatogramTick With {
                                .Time = mzi.scan_time,
                                .Intensity = mzi.intensity
                            }
                        End Function) _
                .ToArray
            Dim q As Quantile() = tick.Summary.ToArray
            Dim maxInto As Double = spectra.Select(Function(mz) mz.intensity).Max

            Return New TargetPeakPoint With {
                .Name = ion_id,
                .SampleName = sampleName,
                .Peak = New ROIPeak With {
                    .base = tick.Baseline,
                    .peakHeight = maxInto,
                    .ticks = tick,
                    .window = New DoubleRange(rt)
                },
                .ChromatogramSummary = q
            }
        End Function
    End Class
End Namespace