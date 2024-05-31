#Region "Microsoft.VisualBasic::5511a83f7494b772e00409b9ddb50c23, visualize\plot\NMR\fidDataPlot.vb"

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

    '   Total Lines: 51
    '    Code Lines: 45 (88.24%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (11.76%)
    '     File Size: 1.83 KB


    ' Class fidDataPlot
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: PlotInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.NMRFidTool
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Data.ChartPlots.Plots
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Math.Interpolation
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render

Public Class fidDataPlot : Inherits Plot

    ReadOnly fid As Fid

    Public Sub New(fidData As Fid, theme As Theme)
        MyBase.New(theme)

        Me.fid = fidData
        Me.xlabel = "Time"
        Me.ylabel = "Amplitude"
        Me.main = "NMR fidData Plot"
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim points As PointData() = fid.Real _
            .Select(Function(t, i) New PointData(t, fid.Imaginary(i))) _
            .ToArray
        Dim css As CSSEnvirnment = g.LoadEnvironment
        Dim lineStyle As Pen = css.GetPen(Stroke.TryParse(theme.lineStroke))
        Dim line As New SerialData With {
            .color = lineStyle.Color,
            .lineType = DashStyle.Solid,
            .pointSize = theme.pointSize,
            .pts = points,
            .shape = LegendStyles.SolidLine,
            .title = "NMR fidData",
            .width = lineStyle.Width
        }
        Dim app As New LinePlot2D({line}, theme, fill:=False, fillPie:=False, interplot:=Splines.None) With {
            .main = main,
            .ylabel = ylabel,
            .xlabel = xlabel,
            .legendTitle = legendTitle,
            .zlabel = zlabel
        }

        Call app.Plot(g, canvas)
    End Sub
End Class
