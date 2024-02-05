Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1

Public Module Data

    <Extension>
    Public Function ExtractXIC(mz As Double, mzdiff As Tolerance) As Func(Of ScanMS1, D2Chromatogram)
        Return Function(d)
                   Return New D2Chromatogram With {
                       .scan_time = d.rt,
                       .intensity = d.GetIntensity(mz, mzdiff),
                       .chromatogram = d.products _
                            .Select(Function(t)
                                        Return New ChromatogramTick With {
                                            .Time = t.rt,
                                            .Intensity = t.GetIntensity(mz, mzdiff)
                                        }
                                    End Function) _
                            .ToArray
                   }
               End Function
    End Function

    <Extension>
    Public Function ExtractTIC(d As ScanMS1) As D2Chromatogram
        Return New D2Chromatogram With {
            .intensity = d.TIC,
            .scan_time = d.rt,
            .chromatogram = d.products _
                .Select(Function(t)
                            Return New ChromatogramTick With {
                                .Intensity = t.into.Sum,
                                .Time = t.rt
                            }
                        End Function) _
                .ToArray
        }
    End Function
End Module
