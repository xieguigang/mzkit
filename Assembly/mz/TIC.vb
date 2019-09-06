Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.MassSpectrum.Assembly.MarkupData
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzXML
Imports SMRUCC.MassSpectrum.Math.Chromatogram
Imports SMRUCC.MassSpectrum.Math.Ms1
Imports SMRUCC.MassSpectrum.Math.Spectra

Partial Module Program

    <ExportAPI("/selective.TIC")>
    <Usage("/selective.TIC /mz <mz.list> /raw <raw.mzXML> [/tolerance <default=ppm:20> /out <TIC.png>]")>
    <Description("Do TIC plot on a given list of selective parent ions.")>
    <Argument("/mz", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(String)},
              Extensions:="*.txt, *.csv",
              Description:="A list file for specific the m/z values.")>
    <Argument("/out", True, CLITypes.File,
              Extensions:="*.png, *.svg",
              Description:="The output TIC plot image file path.")>
    Public Function SelectiveTIC(args As CommandLine) As Integer
        Dim mz$ = args <= "/mz"
        Dim raw$ = args <= "/raw"
        Dim out$ = args("/out") Or $"{mz.TrimSuffix}-{raw.FileName}_TIC.png"
        Dim mzlist As Double() = mz.ReadAllLines.AsDouble
        Dim ms1Scans As mzXML.scan() = mzXML.XML.LoadScans(raw) _
            .Where(Function(scan) scan.msLevel = "1") _
            .ToArray
        Dim tolerance As Tolerance = Tolerance.ParseScript(args("/tolerance") Or "ppm:20")
        Dim chromatogram = ms1Scans _
            .Select(Function(scan)
                        Dim peaks = scan.ExtractMzI.peaks
                        Dim rt# = PeakMs2.RtInSecond(scan.retentionTime)

                        Return peaks _
                            .Where(Function(ion)
                                       Return mzlist.Any(Function(p) True = tolerance(p, ion.mz))
                                   End Function) _
                            .Select(Function(parent)
                                        Dim tick As New ChromatogramTick With {
                                            .Time = rt,
                                            .Intensity = parent.intensity
                                        }
                                        Dim parentIon = parent.mz

                                        Return (mz:=parentIon, tick:=tick)
                                    End Function)
                    End Function) _
            .IteratesALL _
            .Select(Function(parent)
                        Return New TICPoint With {
                            .mz = parent.mz,
                            .time = parent.tick.Time,
                            .intensity = parent.tick.Intensity
                        }
                    End Function) _
            .ToArray

        Dim datafile = out.TrimSuffix & ".points.csv"

        Call chromatogram.SaveTo(datafile)

        Return CLI.mzplot.FromEnvironment(App.HOME).TICplot(datafile, out:=out)
    End Function
End Module
