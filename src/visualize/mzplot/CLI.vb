#Region "Microsoft.VisualBasic::6b008ee6eccf796347f9043a0b0d86e9, visual\mzplot\CLI.vb"

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

    ' Module CLI
    ' 
    '     Function: LinearFittings, mzIntoXIC, TICplot
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Chromatogram
Imports SMRUCC.MassSpectrum.Math.Ms1
Imports SMRUCC.MassSpectrum.Visualization

<CLI> Module CLI

    <ExportAPI("/XIC")>
    <Usage("/XIC /data <points.csv> [/out <XIC.png>]")>
    Public Function mzIntoXIC(args As CommandLine) As Integer
        Dim in$ = args <= "/data"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.XIC.png"
        Dim data = [in].LoadCsv(Of TICPoint).ToArray
        Dim XIC = data _
            .Select(Function(p)
                        Return New ChromatogramTick With {
                            .Time = p.mz,
                            .Intensity = p.intensity
                        }
                    End Function) _
            .OrderBy(Function(p) p.Time) _
            .ToArray
        Dim XICdata As New NamedCollection(Of ChromatogramTick) With {.name = "XIC", .value = XIC}

        Call "Do TIC plot...".__INFO_ECHO

        Return {XICdata}.TICplot(
            showLabels:=False,
            showLegends:=False,
            fillCurve:=False,
            isXIC:=True
        ).Save(out) _
         .CLICode
    End Function

    <ExportAPI("/TIC")>
    <Usage("/TIC /in <data.csv> [/XIC /rt <rt_fieldName, default=rt> /into <intensity_fieldName, default=intensity> /out <plot.png>]")>
    <Description("Do TIC plot based on the given chromatogram table data.")>
    <Argument("/in", False, CLITypes.File,
              AcceptTypes:={GetType(TICPoint), GetType(ms1_scan)},
              Extensions:="*.csv",
              Description:="The mzXML dump data.")>
    <Argument("/XIC", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="If this option enabled, then will do mz group at first.")>
    Public Function TICplot(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim isXIC As Boolean = args("/XIC")
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.{"TIC" Or "XIC".When(isXIC)}.png"
        Dim data As NamedCollection(Of ChromatogramTick)()

        If isXIC Then
            Dim da03 = Tolerance.DeltaMass(0.3)

            data = [in].LoadCsv(Of TICPoint) _
                .GroupBy(Function(p) p.mz, Function(a, b) True = da03(a, b)) _
                .AsParallel _
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
                                    .OrderBy(Function(p) p.Time) _
                                    .ToArray
                            }
                        End Function) _
                .ToArray
        Else
            Dim rtField = args("/rt") Or "rt"
            Dim intoField = args("/into") Or "intensity"
            Dim tic = EntityObject _
                .LoadDataSet([in]) _
                .Select(Function(d As EntityObject)
                            Dim rt As Double = Val(d(rtField))
                            Dim into As Double = Val(d(intoField))

                            Return New ChromatogramTick With {
                                .Intensity = into,
                                .Time = rt
                            }
                        End Function) _
                .OrderBy(Function(p) p.Time) _
                .ToArray

            data = {
                New NamedCollection(Of ChromatogramTick) With {
                    .name = [in].BaseName,
                    .value = tic
                }
            }
        End If

        Call "Do TIC plot...".__INFO_ECHO

        Return data.TICplot(
            showLabels:=False,
            showLegends:=False,
            fillCurve:=False
        ).Save(out) _
         .CLICode
    End Function

    ''' <summary>
    ''' 主要是用于靶向定量程序的测试操作
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("/linear")>
    <Usage("/linear /ref <concentration.list> /tpa <tpa.list> [/tpa.is <tpa.list> /title <plot_title> /weighted /out <result.directory>]")>
    <Description("Test of the targetted metabolism quantify program.")>
    <Argument("/ref", False, CLITypes.String,
              AcceptTypes:={GetType(Double())},
              Description:="A list of reference concentration value points with comma symbol as delimiter.")>
    <Argument("/tpa", False, CLITypes.String,
              AcceptTypes:={GetType(Double())},
              Description:="A list of total peak area integration value of the reference samples target ion.")>
    <Argument("/tpa.is", True, CLITypes.String,
              AcceptTypes:={GetType(Double())},
              Description:="A list of total peak area integration value of the IS compound.")>
    <Argument("/weighted", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="This flag argument controls the linear fitting algorithm that will be used.")>
    <Argument("/out", True, CLITypes.File,
              Extensions:="*.png, *.csv",
              Description:="A directory path for save the result table and plot images, by defualt is current directory.")>
    Public Function LinearFittings(args As CommandLine) As Integer
        Dim ref#() = args("/ref").Split(",").AsDouble
        Dim tpa#() = args("/tpa").Split(",").AsDouble
        Dim tpaIS#() = args("/tpa.is").Split(",").AsDouble
        Dim title$ = args("/title") Or "LinearFittings"
        Dim isWeighted As Boolean = args <= "/weighted"
        Dim out$ = args("/out") Or "./"
        Dim points = StandardCurve.CreateModelPoints(ref, tpa, tpaIS, 5).ToArray
        Dim fit As IFitted = FitModel.CreateLinearRegression(points, isWeighted)
        Dim model As New FitModel With {
            .LinearRegression = fit,
            .Name = title,
            .[IS] = New MRM.Models.IS With {
                .CIS = 5,
                .ID = "IS",
                .name = "IS"
            }
        }

        Call model.LinearRegression.ErrorTest.SaveTo($"{out}/input.csv")
        Call model.StandardCurves().Save($"{out}/standard_curve.png")

        Return 0
    End Function
End Module

