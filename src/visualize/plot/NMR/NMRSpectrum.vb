#Region "Microsoft.VisualBasic::28f6bff2977537fa1420c083ca43183c, visualize\plot\NMR\NMRSpectrum.vb"

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


    ' Code Statistics:

    '   Total Lines: 76
    '    Code Lines: 61 (80.26%)
    ' Comment Lines: 9 (11.84%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (7.89%)
    '     File Size: 2.59 KB


    ' Class NMRSpectrum
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: PlotInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Data.ChartPlots.Plots
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Math.Interpolation
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render

Public Class NMRSpectrum : Inherits Plot

    ''' <summary>
    ''' the x axis
    ''' </summary>
    ReadOnly ppm As Double()
    ''' <summary>
    ''' the y axis
    ''' </summary>
    ReadOnly intensity As Double()
    ''' <summary>
    ''' the title of the spectrum line
    ''' </summary>
    ReadOnly title As String

    Public Sub New(nmr As LibraryMatrix, theme As Theme)
        MyBase.New(theme)

        ppm = nmr.Select(Function(m) m.mz).ToArray
        intensity = nmr.Select(Function(m) m.intensity).ToArray
        title = nmr.name
        theme.yAxisLayout = YAxisLayoutStyles.None
        theme.xAxisReverse = True
        theme.drawLegend = False
        xlabel = "ppm"
        ylabel = "absorb"
        main = "NMR spectrum"
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim points As PointData() = ppm _
            .Select(Function(ppmi, i) New PointData(ppmi, intensity(i))) _
            .ToArray
        Dim css As CSSEnvirnment = g.LoadEnvironment
        Dim lineStyle As Pen = css.GetPen(Stroke.TryParse(theme.lineStroke))
        Dim line As New SerialData With {
            .color = lineStyle.Color,
            .lineType = DashStyle.Solid,
            .pointSize = theme.pointSize,
            .pts = points,
            .shape = LegendStyles.SolidLine,
            .title = title,
            .width = lineStyle.Width
        }
        Dim app As New LinePlot2D(
            data:={line},
            theme:=theme,
            fill:=False,
            fillPie:=False,
            interplot:=Splines.None
        ) With {
            .legendTitle = legendTitle,
            .main = main,
            .xlabel = xlabel,
            .ylabel = ylabel,
            .zlabel = zlabel
        }

        Call app.Plot(g, canvas)
    End Sub
End Class
