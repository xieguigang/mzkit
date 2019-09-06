Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.MassSpectrum.Math.Chromatogram
Imports SMRUCC.MassSpectrum.Math.Ms1
Imports SMRUCC.MassSpectrum.Visualization

Module CLI

    <ExportAPI("/TIC")>
    <Usage("/TIC /in <data.csv> [/out <plot.png>]")>
    Public Function TICplot(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.TIC.png"
        Dim ppm20 = Tolerance.PPM(20)
        Dim data = [in].LoadCsv(Of TICPoint) _
            .GroupBy(Function(p) p.mz, Function(a, b) True = ppm20(a, b)) _
            .Select(Function(ion)
                        Return New NamedCollection(Of ChromatogramTick) With {
                            .name = $"m/z {ion.First.mz.ToString("F4")}",
                            .value = ion _
                                .Select(Function(t)
                                            Return New ChromatogramTick With {
                                                .Time = t.time,
                                                .Intensity = t.intensity
                                            }
                                        End Function) _
                                .ToArray
                        }
                    End Function) _
            .ToArray

        Return data.TICplot().Save(out).CLICode
    End Function
End Module
