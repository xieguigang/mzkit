Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Public Module DrawScatter

    <Extension>
    Public Function Draw3DPeaks(raw As Raw) As Image
        Dim ms1 As ms1_scan() = raw _
            .GetMs1Scans _
            .Select(Function(m1)
                        Return m1.mz.Select(Function(mzi, i) New ms1_scan With {.mz = mzi, .intensity = m1.into(i), .scan_time = m1.rt})
                    End Function) _
            .IteratesALL _
            .ToArray
        Dim XIC = ms1 _
            .GroupBy(Function(m) m.mz, Tolerance.DeltaMass(0.1)) _
            .Select(Function(mz)
                        Return New NamedCollection(Of ChromatogramTick) With {
                            .name = mz.name,
                            .value = mz _
                                .OrderBy(Function(t) t.scan_time) _
                                .Select(Function(t)
                                            Return New ChromatogramTick With {.Time = t.scan_time, .Intensity = t.intensity}
                                        End Function) _
                                .ToArray
                        }
                    End Function) _
            .ToArray

        Return XIC.TICplot(parallel:=True, showLabels:=False, showLegends:=False).AsGDIImage
    End Function

    <Extension>
    Public Function DrawScatter(raw As Raw) As Image
        Dim ms1 As ms1_scan() = raw _
            .GetMs1Scans _
            .Select(Function(m1)
                        Return m1.mz.Select(Function(mzi, i) New ms1_scan With {.mz = mzi, .intensity = m1.into(i), .scan_time = m1.rt})
                    End Function) _
            .IteratesALL _
            .ToArray

        Return RawScatterPlot.Plot(samples:=ms1, rawfile:=raw.source.FileName).AsGDIImage
    End Function

    <Extension>
    Public Function DrawScatter(raw As mzPack) As Image
        Dim ms1 As ms1_scan() = raw.MS _
            .Select(Function(m1)
                        Return m1.mz.Select(Function(mzi, i) New ms1_scan With {.mz = mzi, .intensity = m1.into(i), .scan_time = m1.rt})
                    End Function) _
            .IteratesALL _
            .ToArray

        Return RawScatterPlot.Plot(samples:=ms1).AsGDIImage
    End Function
End Module
