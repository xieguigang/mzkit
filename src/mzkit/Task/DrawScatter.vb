Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq

Public Module DrawScatter

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
End Module
