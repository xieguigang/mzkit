#Region "Microsoft.VisualBasic::4009e49a3c62782623343c15287ea2f2, src\visualize\plot\ChromatogramPlot.vb"

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

' Module ChromatogramPlot
' 
'     Function: MRMChromatogramPlot, (+2 Overloads) TICplot
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.d3js
Imports Microsoft.VisualBasic.Imaging.d3js.Layout
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime

Public Module ChromatogramPlot

    ''' <summary>
    ''' 将所有离子对所捕获的色谱曲线数据都绘制在同一张图上面
    ''' </summary>
    ''' <param name="ions"></param>
    ''' <param name="mzML$"></param>
    ''' <param name="size$"></param>
    ''' <param name="margin$"></param>
    ''' <param name="bg$"></param>
    ''' <param name="colorsSchema$"></param>
    ''' <param name="penStyle$"></param>
    ''' <param name="labelFontStyle$"></param>
    ''' <param name="labelConnectorStroke$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function MRMChromatogramPlot(ions As IonPair(),
                                        mzML$,
                                        Optional size$ = "1600,1000",
                                        Optional margin$ = g.DefaultLargerPadding,
                                        Optional bg$ = "white",
                                        Optional colorsSchema$ = "scibasic.category31()",
                                        Optional penStyle$ = Stroke.ScatterLineStroke,
                                        Optional labelFontStyle$ = CSSFont.Win7Normal,
                                        Optional labelConnectorStroke$ = Stroke.StrongHighlightStroke,
                                        Optional labelLayoutTicks% = 1000,
                                        Optional tolerance$ = "ppm:20") As GraphicsData

        Dim mzTolerance As Tolerance = Ms1.Tolerance.ParseScript(tolerance)
        Dim MRM As IonChromatogram() = IonPair _
            .GetIsomerism(ions, mzTolerance) _
            .ExtractIonData(
                mzML:=mzML,
                assignName:=Function(ion) ion.name,
                tolerance:=mzTolerance
            )

        Return MRM.Select(Function(c)
                              Return New NamedCollection(Of ChromatogramTick) With {
                                 .name = c.name,
                                 .value = c.chromatogram,
                                 .description = c.description
                              }
                          End Function) _
                  .ToArray _
                  .TICplot(
                      size:=size,
                      bg:=bg,
                      colorsSchema:=colorsSchema,
                      labelConnectorStroke:=labelConnectorStroke,
                      labelFontStyle:=labelFontStyle,
                      margin:=margin,
                      penStyle:=penStyle,
                      fillCurve:=False,
                      labelLayoutTicks:=labelLayoutTicks
                  )
    End Function

    ''' <summary>
    ''' 从mzML文件之中解析出色谱数据之后，将所有的色谱峰都绘制在一张图之中进行可视化
    ''' </summary>
    ''' <param name="size$"></param>
    ''' <param name="margin$"></param>
    ''' <param name="bg$"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function TICplot(ionData As NamedCollection(Of ChromatogramTick),
                            Optional size$ = "1600,1000",
                            Optional margin$ = g.DefaultLargerPadding,
                            Optional bg$ = "white",
                            Optional colorsSchema$ = "scibasic.category31()",
                            Optional penStyle$ = Stroke.ScatterLineStroke,
                            Optional labelFontStyle$ = CSSFont.Win7Normal,
                            Optional labelConnectorStroke$ = Stroke.StrongHighlightStroke,
                            Optional labelLayoutTicks% = 500,
                            Optional showLabels As Boolean = True,
                            Optional fillCurve As Boolean = True,
                            Optional axisLabelFont$ = CSSFont.Win7Large,
                            Optional axisTickFont$ = CSSFont.Win10NormalLarger,
                            Optional showLegends As Boolean = True,
                            Optional legendFontCSS$ = CSSFont.Win10Normal,
                            Optional intensityMax As Double = 0) As GraphicsData

        Return {ionData}.TICplot(
            size:=size,
            axisLabelFont:=axisLabelFont,
            axisTickFont:=axisTickFont,
            bg:=bg,
            penStyle:=penStyle,
            labelFontStyle:=labelFontStyle,
            labelConnectorStroke:=labelConnectorStroke,
            colorsSchema:=colorsSchema,
            margin:=margin,
            labelLayoutTicks:=labelLayoutTicks,
            showLabels:=showLabels,
            showLegends:=showLegends,
            fillCurve:=fillCurve,
            legendFontCSS:=legendFontCSS,
            intensityMax:=intensityMax
        )
    End Function

    ''' <summary>
    ''' 从mzML文件之中解析出色谱数据之后，将所有的色谱峰都绘制在一张图之中进行可视化
    ''' </summary>
    ''' <param name="size"></param>
    ''' <param name="margin"></param>
    ''' <param name="bg"></param>
    ''' <param name="deln">legend每一列有多少个进行显示</param>
    ''' <param name="labelColor"></param>
    ''' <param name="penStyle">
    ''' CSS value for controls of the line drawing style
    ''' </param>
    <Extension>
    Public Function TICplot(ionData As NamedCollection(Of ChromatogramTick)(),
                            Optional size$ = "1600,1000",
                            Optional margin$ = g.DefaultLargerPadding,
                            Optional bg$ = "white",
                            Optional colorsSchema$ = "scibasic.category31()",
                            Optional penStyle$ = Stroke.ScatterLineStroke,
                            Optional labelFontStyle$ = CSSFont.Win7Small,
                            Optional labelConnectorStroke$ = Stroke.StrongHighlightStroke,
                            Optional labelLayoutTicks% = 1000,
                            Optional labelColor$ = "black",
                            Optional showLabels As Boolean = True,
                            Optional showGrid As Boolean = False,
                            Optional fillCurve As Boolean = True,
                            Optional axisLabelFont$ = CSSFont.Win7Large,
                            Optional axisTickFont$ = CSSFont.Win10NormalLarger,
                            Optional showLegends As Boolean = True,
                            Optional legendFontCSS$ = CSSFont.Win10Normal,
                            Optional deln% = 10,
                            Optional isXIC As Boolean = False,
                            Optional fillAlpha As Integer = 180,
                            Optional gridFill As String = "rgb(245,245,245)",
                            Optional timeRange As Double() = Nothing,
                            Optional parallel As Boolean = False,
                            Optional intensityMax As Double = 0) As GraphicsData

        Dim theme As New Theme
        Dim TIC As New TICplot(theme)

        Return TIC.Plot(size)
    End Function
End Module
