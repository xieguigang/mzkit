#Region "Microsoft.VisualBasic::810bba4568992b83d5574209401da66b, Rscript\Library\mzkit.quantify\Visual.vb"

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

    ' Module Visual
    ' 
    '     Function: asChromatogram, chromatogramPlot, DrawStandardCurve, MRMchromatogramPeakPlot
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime

<Package("mzkit.quantify.visual")>
Module Visual

    <ExportAPI("as.chromatogram")>
    <RApiReturn(GetType(ChromatogramTick()))>
    Public Function asChromatogram(data As Object,
                                   Optional time$ = "Time",
                                   Optional into$ = "Intensity",
                                   Optional env As Environment = Nothing) As Object
        Dim timeVec As Double()
        Dim intoVec As Double()

        If data Is Nothing Then
            Return Nothing
        ElseIf TypeOf data Is Rdataframe Then
            timeVec = REnv.asVector(Of Double)(DirectCast(data, Rdataframe).GetColumnVector(time))
            intoVec = REnv.asVector(Of Double)(DirectCast(data, Rdataframe).GetColumnVector(into))
        ElseIf TypeOf data Is DataSet() Then
            timeVec = DirectCast(data, DataSet()).Vector(time)
            intoVec = DirectCast(data, DataSet()).Vector(into)
        Else
            Return Internal.debug.stop($"invalid data sequence: {data.GetType.FullName}", env)
        End If

        Return timeVec _
            .Select(Function(t, i)
                        Return New ChromatogramTick With {
                            .Time = t,
                            .Intensity = intoVec(i)
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' Draw standard curve
    ''' </summary>
    ''' <param name="model">The linear model of the targeted metabolism model data.</param>
    ''' <param name="title">The plot title</param>
    ''' <param name="samples">The point data of samples</param>
    ''' <returns></returns>
    <ExportAPI("standard_curve")>
    Public Function DrawStandardCurve(model As StandardCurve,
                                      Optional title$ = "",
                                      Optional samples As NamedValue(Of Double)() = Nothing,
                                      Optional size$ = "1600,1200",
                                      Optional margin$ = "padding: 200px 100px 150px 150px",
                                      Optional factorFormat$ = "G4",
                                      Optional sampleLabelFont$ = CSSFont.Win10NormalLarger,
                                      Optional labelerIterations% = 1000) As GraphicsData

        Return StandardCurvesPlot.StandardCurves(
            model:=model,
            samples:=samples,
            name:=title,
            size:=size,
            margin:=margin,
            factorFormat:=factorFormat,
            sampleLabelFont:=sampleLabelFont,
            labelerIterations:=labelerIterations
        )
    End Function

    <ExportAPI("chromatogram.plot")>
    Public Function chromatogramPlot(mzML$, ions As IonPair(), Optional labelLayoutTicks% = 2000) As GraphicsData
        Return ions.MRMChromatogramPlot(mzML, labelLayoutTicks:=labelLayoutTicks)
    End Function

    ''' <summary>
    ''' Visualization plot of the MRM chromatogram peaks data.
    ''' </summary>
    ''' <param name="chromatogram">the extract MRM chromatogram peaks data.</param>
    ''' <param name="title">the plot title</param>
    ''' <param name="size">the size of the output image</param>
    ''' <param name="fill">fill polygon of the TIC plot?</param>
    ''' <param name="gridFill">color value for fill the grid background</param>
    ''' <param name="lineStyle">
    ''' Css value for adjust the plot style of the curve line of the chromatogram peaks data.
    ''' </param>
    ''' <param name="relativeTimeScale"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("MRM.chromatogramPeaks.plot")>
    <RApiReturn(GetType(GraphicsData))>
    Public Function MRMchromatogramPeakPlot(<RRawVectorArgument>
                                            chromatogram As Object,
                                            Optional title$ = "MRM Chromatogram Peak Plot",
                                            Optional size As Object = "2200,1600",
                                            Optional fill As Boolean = True,
                                            Optional gridFill$ = "rgb(250,250,250)",
                                            Optional lineStyle$ = "stroke: black; stroke-width: 2px; stroke-dash: solid;",
                                            <RRawVectorArgument>
                                            Optional relativeTimeScale As Object = Nothing,
                                            Optional parallel As Boolean = False,
                                            Optional env As Environment = Nothing) As Object

        If chromatogram Is Nothing Then
            Return REnv.Internal.debug.stop("No chromatogram provided!", env)
        End If

        If relativeTimeScale Is Nothing Then
            relativeTimeScale = New Double() {}
        Else
            relativeTimeScale = DirectCast(REnv.asVector(Of Double)(relativeTimeScale), Double())
        End If

        If TypeOf chromatogram Is ChromatogramTick() Then
            Return DirectCast(chromatogram, ChromatogramTick()).Plot(
                title:=title,
                showMRMRegion:=True,
                showAccumulateLine:=True,
                size:=InteropArgumentHelper.getSize(size, "2100,1650"),
                curveStyle:=lineStyle
            )
        ElseIf TypeOf chromatogram Is list AndAlso DirectCast(chromatogram, list).slots.All(Function(c) REnv.isVector(Of ChromatogramTick)(c.Value)) Then
            Return DirectCast(chromatogram, list).slots _
                .Select(Function(sample)
                            Return New NamedCollection(Of ChromatogramTick) With {
                                .name = sample.Key,
                                .value = REnv.asVector(Of ChromatogramTick)(sample.Value)
                            }
                        End Function) _
                .ToArray _
                .TICplot(
                    size:=InteropArgumentHelper.getSize(size, "2200,1440"),
                    fillCurve:=fill,
                    gridFill:=gridFill,
                    penStyle:=lineStyle,
                    timeRange:=relativeTimeScale,
                    parallel:=parallel
                )
        Else
            Return REnv.Internal.debug.stop($"Invalid input data: {chromatogram.GetType.FullName}", env)
        End If
    End Function
End Module
