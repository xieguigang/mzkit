#Region "Microsoft.VisualBasic::397e710ede2306e84fd49640c1373296, mz\Program.vb"

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
'     Function: Main, MGF, MzWaves
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.MassSpectrum.Assembly.MarkupData
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzXML
Imports SMRUCC.MassSpectrum.Math.Ms1.PrecursorType
Imports SMRUCC.MassSpectrum.Math.Spectra

Module Program

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
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
        Dim table As MzReport() = MzCalculator.Calculate(mass, mode).ToArray

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
                            .ToDictionary(Function(p) Math.Round(p.mz, rounds).ToString,
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

    <ExportAPI("/export")>
    <Usage("/export /in <data.mzXML> /scan <ms2_scan> [/out <out.txt>]")>
    <Description("Export a single ms2 scan data.")>
    <Argument("/scan", False, CLITypes.Integer,
              AcceptTypes:={GetType(Integer)},
              Description:="The scan index number.")>
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

    ''' <summary>
    ''' 将二级碎片数据按照m/z分组导出
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Dump.ms2")>
    <Usage("/Dump.ms2 /in <lib.mzXML> [/out <out.directory>]")>
    Public Function DumpMs2(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.dump/"
        Dim allMs2Scans = mzXML.XML.LoadScans([in]) _
            .Where(Function(s) s.msLevel = "2") _
            .ToArray

        ' 首先按照m/z分组

    End Function
End Module

