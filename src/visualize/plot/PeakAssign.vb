#Region "Microsoft.VisualBasic::efca05a74e4e30b75109d52d9b78af3d, src\visualize\plot\PeakAssign.vb"

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

' Module PeakAssign
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Text
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS

''' <summary>
''' 通过KCF图模型为ms2二级质谱碎片鉴定分子碎片的具体的结构式
''' </summary>
Public Class PeakAssign : Inherits Plot

    ReadOnly matrix As ms2()
    ReadOnly title As String

    Public Sub New(title$, matrix As IEnumerable(Of ms2), theme As Theme)
        MyBase.New(theme)

        Me.title = title
        Me.matrix = matrix.ToArray
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim maxinto As Double = matrix.Select(Function(p) p.intensity).Max
        Dim rect As RectangleF = canvas.PlotRegion.ToFloat
        Dim xticks As Double() = (matrix.Select(Function(p) p.mz).Range * 1.125).CreateAxisTicks
        Dim xscale = d3js.scale.linear().domain(xticks).range(values:={rect.Left, rect.Right})
        Dim yscale = d3js.scale.linear().domain(New Double() {0, 100}).range(values:={rect.Top, rect.Bottom})
        Dim scaler As New DataScaler() With {
            .AxisTicks = (xticks.AsVector, New Vector(New Double() {0, 20, 40, 60, 80, 100})),
            .region = canvas.PlotRegion,
            .X = xscale,
            .Y = yscale
        }
        Dim bottomY As Double = rect.Bottom
        Dim text As New GraphicsText(DirectCast(g, Graphics2D).Graphics)
        Dim labelFont As Font = CSSFont.TryParse(theme.tagCSS)
        Dim titleFont As Font = CSSFont.TryParse(theme.mainCSS)

        Call Axis.DrawAxis(
            g, canvas, scaler,
            showGrid:=True,
            xlabel:="M/z ratio",
            ylabel:="Relative Intensity (%)",
            XtickFormat:="F4",
            YtickFormat:="F0",
            gridFill:=theme.gridFill,
            xlayout:=XAxisLayoutStyles.None,
            tickFontStyle:=theme.axisTickCSS,
            gridX:=Nothing,
            axisStroke:=theme.axisStroke,
            htmlLabel:=False,
            labelFont:=theme.axisLabelCSS
        )

        Call g.DrawLine(Stroke.TryParse(theme.axisStroke), New PointF(rect.Left, rect.Bottom), New PointF(rect.Right, rect.Bottom))

        Dim labelSize As SizeF
        Dim barStyle As Stroke = Stroke.TryParse(theme.lineStroke)
        Dim barColor As Brush = barStyle.fill.GetBrush
        Dim label As String

        For Each product As ms2 In matrix
            Dim pt As PointF = scaler.Translate(product.mz, product.intensity / maxinto * 100)
            Dim bar As New RectangleF With {
                .X = pt.X - barStyle.width / 2,
                .Y = pt.Y,
                .Height = bottomY - pt.Y,
                .Width = barStyle.width
            }

            Call g.FillRectangle(barColor, bar)

            label = product.Annotation

            If Not label.StringEmpty Then
                labelSize = g.MeasureString(label, labelFont)
                pt = New PointF(pt.X - labelSize.Height / 2, pt.Y)

                Call text.DrawString(label, labelFont, Brushes.Black, pt, 270)
            ElseIf product.intensity / maxinto >= 0.2 Then
                label = product.mz.ToString("F2")
                labelSize = g.MeasureString(label, labelFont)
                pt = New PointF(pt.X - labelSize.Width / 2, pt.Y - labelSize.Height)

                Call g.DrawString(label, labelFont, Brushes.Black, pt)
            End If
        Next

        labelSize = g.MeasureString(title, titleFont)

        Dim location As New PointF With {
            .X = (canvas.Width - labelSize.Width) / 2,
            .Y = (rect.Top - labelSize.Height) / 2
        }

        Call g.DrawString(title, titleFont, Brushes.Black, location)
    End Sub

    Public Shared Function DrawSpectrumPeaks(matrix As LibraryMatrix,
                                             Optional size$ = "1600,1080",
                                             Optional padding$ = "padding:150px 100px 85px 125px;",
                                             Optional bg$ = "white",
                                             Optional gridFill$ = "white",
                                             Optional barStroke$ = "stroke: steelblue; stroke-width: 8px; stroke-dash: solid;",
                                             Optional titleCSS$ = "font-style: normal; font-size: 16; font-family: " & FontFace.MicrosoftYaHei & ";",
                                             Optional labelCSS$ = "font-style: normal; font-size: 8; font-family: " & FontFace.MicrosoftYaHei & ";",
                                             Optional axisLabelCSS$ = "font-style: normal; font-size: 12; font-family: " & FontFace.MicrosoftYaHei & ";",
                                             Optional axisTicksCSS$ = "font-style: normal; font-size: 10; font-family: " & FontFace.SegoeUI & ";",
                                             Optional axisStroke$ = Stroke.AxisStroke) As GraphicsData

        Dim theme As New Theme With {
            .padding = padding,
            .background = bg,
            .gridFill = gridFill,
            .lineStroke = barStroke,
            .mainCSS = titleCSS,
            .tagCSS = labelCSS,
            .axisTickCSS = axisTicksCSS,
            .axisStroke = axisStroke,
            .axisLabelCSS = axisLabelCSS
        }
        Dim app As New PeakAssign(matrix.name, matrix.ms2, theme)

        Return app.Plot(size, ppi:=200)
    End Function
End Class
