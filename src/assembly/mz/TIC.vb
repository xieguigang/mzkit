#Region "Microsoft.VisualBasic::84e0353f59931421888a125b92167a50, src\assembly\mz\TIC.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
' 
' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' MIT License
' 
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.



' /********************************************************************************/

' Summaries:

' Module Program
' 
'     Function: TIC, XIC, XICMgf
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime

Partial Module Program

    <ExportAPI("/mgf.XIC")>
    <Usage("/mgf.XIC /raw <data.mgf> [/out <XIC.png>]")>
    <Argument("/raw", False, CLITypes.File,
              Extensions:="*.mgf",
              Description:="This parameter could be file name list use comma symbol as delimiter.")>
    Public Function XICMgf(args As CommandLine) As Integer
        Dim raw$ = args <= "/raw"
        Dim files = CLITools.GetFileList(raw).ToArray
        Dim out$

        If files.Length = 1 Then
            out = args("/out") Or $"{raw.TrimSuffix}.plot.png"
        Else
            out = args("/out") Or $"{files.First.ParentPath}/{files.Select(Function(file) file.BaseName).JoinBy(",")}.plot.png"
        End If

        Dim ions = MgfReader.ReadIons(files) _
            .Select(Function(ion)
                        If ion.Meta.TryGetValue("activation", [default]:="") = "ms1" Then
                            Return ion.Peaks _
                                .Select(Function(peak)
                                            Return New TICPoint With {
                                                .intensity = peak.intensity,
                                                .mz = peak.mz,
                                                .time = ion.RtInSeconds
                                            }
                                        End Function)
                        Else
                            Return {
                                New TICPoint With {
                                    .intensity = ion.PepMass.text,
                                    .mz = ion.PepMass.name,
                                    .time = ion.RtInSeconds
                                }
                            }
                        End If
                    End Function) _
            .IteratesALL _
            .OrderBy(Function(p) p.time) _
            .ToArray

        Dim datafile = out.TrimSuffix & ".points.csv"
        Dim plot = CLI.mzplot.FromEnvironment(App.HOME)

        Call ions.SaveTo(datafile)

        Return plot.mzIntoXIC(datafile)
    End Function

    <ExportAPI("/TIC")>
    <Usage("/TIC /raw <data.mgf> [/mz /out <TIC.png>]")>
    <Argument("/raw", False, CLITypes.File,
              Extensions:="*.mgf",
              Description:="This parameter could be file name list use comma symbol as delimiter.")>
    <Argument("/mz", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="The m/z value as x axis.")>
    Public Function TIC(args As CommandLine) As Integer
        Dim raw$ = args <= "/raw"
        Dim files = CLITools.GetFileList(raw).ToArray
        Dim out$
        Dim ismzX As Boolean = args("/mz")

        If files.Length = 1 Then
            out = args("/out") Or $"{raw.TrimSuffix}.plot.png"
        Else
            out = args("/out") Or $"{files.First.ParentPath}/{files.Select(Function(file) file.BaseName).JoinBy(",")}.plot.png"
        End If

        Dim ions = MgfReader.ReadIons(files) _
            .Select(Function(ion)
                        Return New TICPoint With {
                            .intensity = ion.PepMass.text,
                            .mz = ion.PepMass.name,
                            .time = ion.RtInSeconds
                        }
                    End Function) _
            .OrderBy(Function(p) p.time) _
            .ToArray

        Dim datafile = out.TrimSuffix & ".points.csv"
        Dim plot = CLI.mzplot.FromEnvironment(App.HOME)

        Call ions.SaveTo(datafile)

        If ismzX Then
            Return plot.mzIntoXIC(datafile)
        Else
            Return plot.TICplot(datafile, rt:=NameOf(TICPoint.time), into:=NameOf(TICPoint.intensity), out:=out)
        End If
    End Function

    <ExportAPI("/XIC")>
    <Usage("/XIC /mz <mz.list> /raw <raw.mzXML> [/tolerance <default=ppm:20> /out <XIC.png>]")>
    <Description("Do TIC plot on a given list of selective parent ions.")>
    <Argument("/mz", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(String)},
              Extensions:="*.txt, *.csv",
              Description:="A list file for specific the m/z values.")>
    <Argument("/out", True, CLITypes.File,
              Extensions:="*.png, *.svg",
              Description:="The output TIC plot image file path.")>
    Public Function XIC(args As CommandLine) As Integer
        Dim mz$ = args <= "/mz"
        Dim raw$ = args <= "/raw"
        Dim out$ = args("/out") Or $"{mz.TrimSuffix}-{raw.FileName}_XIC.png"
        Dim mzlist As Double() = mz.ReadAllLines.AsDouble

        Call "Load all ms1 scans".__DEBUG_ECHO
        Dim tolerance As Tolerance = Tolerance.ParseScript(args("/tolerance") Or "ppm:20")
        Dim chromatogram = mzXML.XML.LoadScans(raw) _
            .Where(Function(scan) scan.msLevel = "1") _
            .AsParallel _
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
