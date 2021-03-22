#Region "Microsoft.VisualBasic::76052d0b1d3c080b939ab80d1586e43a, src\assembly\mz\Program.vb"

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
    '     Function: Calculator, CentroidPeaksData, DumpAsMgf, DumpMs2Batch, GetPeaktable
    '               Main, MzPack, MzWaves, printMatrix
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.Text
Imports stdNum = System.Math

<CLI> Module Program

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    <ExportAPI("/pack")>
    <Description("convert the mzXML/mzMl raw data file into the mzpack binary format.")>
    <Usage("/pack /xml <*.mzXML/mzML> [/mzpack <outfile.mzpack>]")>
    Public Function MzPack(args As CommandLine) As Integer

    End Function

    <ExportAPI("/mz.calculate")>
    <Usage("/mz.calculate /mass <mass> [/mode <+/-, default=+> /out <out.csv/html/txt>]")>
    <Argument("/out", True, CLITypes.File, PipelineTypes.std_out,
              AcceptTypes:={GetType(String)},
              Description:="If this argument is not config in cli input, then result will be print on console.")>
    <Argument("/mass", False, CLITypes.Double,
              AcceptTypes:={GetType(Double)},
              Description:="The exact mass value.")>
    <Argument("/mode", True, CLITypes.String,
              AcceptTypes:={GetType(String)},
              Description:="The polarity mode, except of value +/-, and the value of pos/neg/p/n is also accepts.")>
    Public Function Calculator(args As CommandLine) As Integer
        Dim mass# = args("/mass")
        Dim mode$ = args("/mode") Or "+"
        Dim out$ = args("/out")
        Dim table As PrecursorInfo() = MzCalculator.EvaluateAll(mass, mode).ToArray

        If out.StringEmpty Then
            ' print on console
            Using file As New StreamWriter(Console.OpenStandardOutput)
                Call table.Print(file)
            End Using
        ElseIf out.ExtensionSuffix.TextEquals("csv") Then
            Return table.SaveTo(out).CLICode
        ElseIf out.ExtensionSuffix.TextEquals("html") Then
            Using file As StreamWriter = out.OpenWriter
                Call table.PrintTable(file)
            End Using
        Else
            Using file As StreamWriter = out.OpenWriter
                Call table.Print(file, False)
            End Using
        End If

        Return 0
    End Function

    ''' <summary>
    ''' rt, mz1, mz2, mz3, ...
    ''' t1, into1,into2,into3,...
    ''' t2, into4,into5,into6,...
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/waves")>
    <Usage("/waves /in <data.mzXML> [/mz.range <[min, max], default is all> /mz.round <default=5> /out <data.xls>]")>
    <Description("Export the ms1 intensity matrix.")>
    Public Function MzWaves(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim mzRange As DoubleRange = DoubleRange.TryParse(args <= "/mz.range")
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}-[{mzRange?.Min},{mzRange?.Max}].xls"
        Dim rounds As Integer = args("/mz.round") Or 5
        Dim allMs1Scans = mzXML.XML _
            .LoadScans([in]) _
            .Where(Function(s) s.msLevel = "1") _
            .ToArray
        Dim mzFilter As Func(Of Double, Boolean)

        If mzRange Is Nothing Then
            mzFilter = Function() True
        Else
            mzFilter = Function(mz) mzRange.IsInside(mz)
        End If

        Dim timeScans As DataSet() = allMs1Scans _
            .Select(Function(s)
                        Dim rt As String = s.retentionTime
                        Dim mzInto = s.ExtractMzI
                        Dim scanData As Dictionary(Of String, Double) = mzInto _
                            .peaks _
                            .Where(Function(p) mzFilter(p.mz)) _
                            .ToDictionary(Function(p) stdNum.Round(p.mz, rounds).ToString,
                                          Function(p) p.intensity)

                        Return New DataSet With {
                            .ID = rt,
                            .Properties = scanData
                        }
                    End Function) _
            .ToArray

        Return timeScans _
            .SaveTo(out, metaBlank:=0, tsv:=True) _
            .CLICode
    End Function

    ''' <summary>
    ''' 进行数据的解卷积
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/peaktable")>
    <Usage("/peaktable /in <raw.mzXML> [/tolerance <default=da:0.3> /out <peaktable.xls>]")>
    <Argument("/ms2", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="Use ms2 data for the calculation of the peaktable.")>
    Public Function GetPeaktable(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.peaktable.xls"
        Dim allScans = mzXML.XML.ReadSingleFile([in], 1)
        Dim tolerance As Tolerance = Tolerance.ParseScript(args("/tolerance") Or "da:0.3")
        Dim basename$ = [in].BaseName
        Dim peaktable As PeakFeature() = allScans _
            .Select(Function(scan)
                        ' ms1的数据总是使用raw intensity值
                        Dim peakScans = scan.ScanData(basename, raw:=True)
                        Dim ms1 = peakScans.mzInto _
                            .Select(Function(frag)
                                        Return New ms1_scan With {
                                            .intensity = frag.intensity,
                                            .mz = frag.mz,
                                            .scan_time = peakScans.rt
                                        }
                                    End Function)
                        Return ms1
                    End Function) _
            .IteratesALL _
            .GetMzGroups(tolerance) _
            .DecoMzGroups _
            .ToArray

        Return peaktable.SaveTo(out).CLICode
    End Function

    <ExportAPI("/export")>
    <Usage("/export /in <data.mzXML> /scan <ms2_scan> [/out <out.txt>]")>
    <Description("Export a single ms2 scan data.")>
    <Argument("/scan", False, CLITypes.Integer,
              AcceptTypes:={GetType(Integer)},
              Description:="The scan index number.")>
    Public Function printMatrix(args As CommandLine) As Integer
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
                                .intensity = x.intensity
                            }
                        End Function) _
                .ToArray
            mzinto = mzinto / mzinto.Max
            mzinto = mzinto(mzinto!intensity >= (5 / 100))
            mzinto = mzinto _
                .Select(Function(m)
                            Return New ms2 With {
                                .mz = m.mz,
                                .intensity = stdNum.Round(m.intensity * 100)
                            }
                        End Function) _
                .ToArray

            Call file.WriteLine(mzinto.Print(addBorder:=False))
        End Using

        Return 0
    End Function

    ''' <summary>
    ''' 在所给定的误差范围内将原始数据进行简化
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("/centroid")>
    <Usage("/centroid /mgf <raw.mgf> [/ms2.tolerance <default=da:0.1> /into.cutoff <default=0.05> /out <simple.mgf>]")>
    <Description("Removes low abundance fragment details from the ms2 peaks from the profile mode raw data.")>
    <Argument("/into.cutoff", True, CLITypes.Double,
              AcceptTypes:={GetType(Double)},
              Description:="A relative intensity cutoff value for removes low abundance ms2 fragments. 
              This cutoff value should be in range of ``[0, 1)``.")>
    Public Function CentroidPeaksData(args As CommandLine) As Integer
        Dim in$ = args <= "/mgf"
        Dim ms2Tolerance As Tolerance = Tolerance.ParseScript(args("/ms2.tolerance") Or "da:0.1")
        Dim intoCutoff As New RelativeIntensityCutoff(args("/into.cutoff") Or 0.05)
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.centroid.mgf"
        Dim centroidIons = MgfReader.StreamParser([in]) _
            .Select(Function(p)
                        Dim peaks As New LibraryMatrix With {.ms2 = p.Peaks}
                        Dim simplify = peaks.CentroidMode(ms2Tolerance, intoCutoff)

                        Return p
                    End Function) _
            .ToArray

        Using mgfWriter As StreamWriter = out.OpenWriter(Encodings.ASCII, append:=False)
            For Each ion In centroidIons
                Call ion.WriteAsciiMgf(mgfWriter, False)
            Next
        End Using
    End Function

    ''' <summary>
    ''' 将二级碎片数据按照m/z分组导出
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/mgf")>
    <Description("Export all of the ms2 ions in target mzXML file and save as mgf file format. Load data from mgf file is more faster than mzXML raw data file.")>
    <Usage("/mgf /in <rawdata.mzXML> [/relative /ms1 /out <ions.mgf>]")>
    <Argument("/relative", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="Dumping the relative intensity value instead of the raw intensity value.")>
    <Argument("/in", False, CLITypes.File, PipelineTypes.std_in,
              Extensions:="*.mzXML",
              Description:="File path of the mzXML raw data file.")>
    <Argument("/out", True, CLITypes.File, PipelineTypes.std_out,
              AcceptTypes:={GetType(Ions)},
              Extensions:="*.txt,*.mgf",
              Description:="The file path for mgf text output.")>
    <Argument("/ms1", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="The mgf file will also includes ms1 data.")>
    Public Function DumpAsMgf(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.mgf"
        Dim peak As PeakMs2
        Dim basename$ = [in].FileName
        Dim relativeInto As Boolean = args("/relative")
        Dim includesMs1 As Boolean = args("/ms1")

        If [in].GetFullPath = out.GetFullPath Then
            Throw New InvalidDataException("Input and output can not be the same file!")
        End If

        Using mgfWriter As StreamWriter = out.OpenWriter(Encodings.ASCII, append:=False)
            For Each ms2Scan As scan In mzXML.XML _
                .LoadScans([in]) _
                .Where(Function(s)
                           If includesMs1 Then
                               Return True
                           Else
                               Return s.msLevel = 2
                           End If
                       End Function)

                If ms2Scan.msLevel = 1 Then
                    ' ms1的数据总是使用raw intensity值
                    peak = ms2Scan.ScanData(basename, raw:=True)
                Else
                    peak = ms2Scan.ScanData(basename, raw:=Not relativeInto)
                End If

                peak _
                    .MgfIon _
                    .WriteAsciiMgf(mgfWriter, relativeInto)
            Next
        End Using

        Return 0
    End Function

    <ExportAPI("/mgf.batch")>
    <Usage("/mgf.batch /in <data.directory> [/index_only /out <data.directory>]")>
    Public Function DumpMs2Batch(args As CommandLine) As Integer
        Dim in$ = (args <= "/in").GetDirectoryFullPath
        Dim out$ = args("/out") Or [in]
        Dim outMgf$
        Dim index As New TermTree(Of String) With {
            .ID = 0,
            .Childs = New Dictionary(Of String, Tree(Of String, String)),
            .Data = "#",
            .label = "/",
            .Parent = Nothing
        }
        Dim this = CLI.mz.FromEnvironment(App.HOME)
        Dim indexOnly As Boolean = args("/index_only")

        For Each rawfile As String In ls - l - r - "*.mzXML" <= [in]
            outMgf = rawfile.Replace("\", "/").Replace([in], "")
            outMgf = outMgf.ChangeSuffix("mgf")

            Call index.Add(outMgf, outMgf.FileName)

            If Not indexOnly Then
                Call this.DumpAsMgf(rawfile, $"{out}/{outMgf}")
            Else
                Call rawfile.__DEBUG_ECHO
            End If
        Next

        Return index _
            .GetJson(maskReadonly:=True) _
            .SaveTo($"{out}/index.json") _
            .CLICode
    End Function
End Module
