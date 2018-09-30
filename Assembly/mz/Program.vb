Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.MassSpectrum.Assembly.MarkupData
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzXML
Imports SMRUCC.MassSpectrum.Math.MSMS

Module Program

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    ''' <summary>
    ''' rt, mz1, mz2, mz3, ...
    ''' t1, into1,into2,into3,...
    ''' t2, into4,into5,into6,...
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/waves")>
    <Usage("/waves /in <data.mzXML> [/mz.range <[min, max], default is all> /mz.round <default=5> /out <data.tsv>]")>
    Public Function MzWaves(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim mzRange As DoubleRange = args <= "/mz.range"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}-[{mzRange.Min},{mzRange.Max}].xls"
        Dim rounds As Integer = args("/mz.round") Or 5
        Dim allMs1Scans = mzXML.XML _
            .LoadScans([in]) _
            .Where(Function(s) s.msLevel = "1") _
            .ToArray
        Dim timeScans As DataSet() = allMs1Scans _
            .Select(Function(s)
                        Dim rt As String = s.retentionTime
                        Dim mzInto = s.ExtractMzI
                        Dim scanData As Dictionary(Of String, Double) = mzInto _
                            .peaks _
                            .ToDictionary(Function(p) Math.Round(p.mz, rounds).ToString,
                                          Function(p) p.intensity)

                        Return New DataSet With {
                            .ID = rt,
                            .Properties = scanData
                        }
                    End Function) _
            .ToArray

        Return timeScans.SaveTo(out).CLICode
    End Function

    <ExportAPI("/export")>
    <Usage("/export /in <data.mzXML> /scan <ms2_scan> [/out <out.txt>]")>
    Public Function MGF(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim scan& = args <= "/scan"
        Dim out$ = args("/out") Or $"{[in]}#{scan}.txt"
        Dim allScans = mzXML.XML.LoadScans([in]).ToArray
        Dim allMs2FullScan = allScans _
            .Where(Function(s) s.msLevel = "2") _
            .ToArray
        Dim ms2Scan = allMs2FullScan(scan)
        Dim ms2Peaks = ms2Scan.ExtractMzI

        Using file As StreamWriter = out.OpenWriter
            Call file.WriteLine(ms2Peaks.name)
            Call file.WriteLine($"mz range: [{ms2Scan.lowMz}, {ms2Scan.highMz}]")
            Call file.WriteLine($"peaks: {ms2Scan.peaksCount}")
            Call file.WriteLine($"activation: {ms2Scan.precursorMz.activationMethod} @ {ms2Scan.collisionEnergy}V")
            Call file.WriteLine(ms2Peaks.peaks.Print(addBorder:=False))
            Call file.WriteLine()

            Dim mzinto As LibraryMatrix = ms2Peaks _
                .peaks _
                .Select(Function(x)
                            Return New ms2 With {
                                .mz = x.mz,
                                .intensity = x.intensity,
                                .quantity = x.intensity
                            }
                        End Function) _
                .ToArray
            mzinto = mzinto / mzinto.Max
            mzinto = mzinto(mzinto!intensity >= (5 / 100))
            mzinto = mzinto _
                .Select(Function(m)
                            Return New ms2 With {
                                .mz = m.mz,
                                .quantity = m.quantity,
                                .intensity = Math.Round(m.intensity * 100)
                            }
                        End Function) _
                .ToArray

            Call file.WriteLine(mzinto.Print(addBorder:=False))
        End Using

        Return 0
    End Function
End Module
