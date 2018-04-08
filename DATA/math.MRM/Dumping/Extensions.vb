Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Scripting
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzML
Imports SMRUCC.MassSpectrum.Math.Chromatogram

Namespace Dumping

    Public Module Extensions

        <Extension>
        Public Function PopulatePeaks(ionPairs As IonPair(), raw$, Optional baselineQuantile# = 0.65) As (ion As IonPair, peak As MRMPeak)()
            Dim ionData = LoadChromatogramList(path:=raw) _
                .MRMSelector(ionPairs) _
                .Where(Function(ion) Not ion.chromatogram Is Nothing) _
                .Select(Function(ion)
                            Dim vector As IVector(Of ChromatogramTick) = ion.chromatogram.Ticks.Shadows
                            Dim peak = vector.MRMPeak(baselineQuantile:=baselineQuantile)
                            Dim peakTicks = vector.PickArea(range:=peak)
                            Dim mrm As New MRMPeak With {
                                .Window = peak,
                                .Base = vector.Baseline(baselineQuantile),
                                .Ticks = peakTicks,
                                .PeakHeight = Aggregate t In .Ticks Into Max(t.Intensity)
                            }

                            Return (ion.ion, mrm)
                        End Function) _
                .ToArray

            Return ionData
        End Function
    End Module
End Namespace


