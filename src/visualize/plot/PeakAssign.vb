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
Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.d3js
Imports Microsoft.VisualBasic.Imaging.d3js.Layout
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Text
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS

''' <summary>
''' 通过KCF图模型为ms2二级质谱碎片鉴定分子碎片的具体的结构式
''' </summary>
Public Class PeakAssign : Inherits Plot

    ReadOnly matrix As ms2()
    ReadOnly title As String
    ReadOnly barHighlight As String
    ReadOnly images As Dictionary(Of String, Image)

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="title$"></param>
    ''' <param name="matrix"></param>
    ''' <param name="barHighlight"></param>
    ''' <param name="theme"></param>
    ''' <param name="images">the annotated molecular parts image</param>
    Public Sub New(title$, matrix As IEnumerable(Of ms2), barHighlight As String, theme As Theme, images As Dictionary(Of String, Image))
        MyBase.New(theme)

        Me.title = title
        Me.matrix = matrix.ToArray
        Me.barHighlight = barHighlight

        If images Is Nothing Then
            Me.images = New Dictionary(Of String, Image)
        Else
            Me.images = images
        End If
    End Sub

    Private Function ResizeThisWidth(original As Image, maxwidth As Integer) As Image
        Dim intWidth As Integer = original.Width
        Dim intHeight As Integer = original.Height
        Dim newWidth, newHeight As Integer

        If intWidth > maxwidth Then
            newWidth = maxwidth
            newHeight = maxwidth * (intHeight / intWidth)

            Return New Bitmap(original, newWidth, newHeight)
        Else
            Return original
        End If
    End Function

    Private Function ResizeImages(canvas As GraphicsRegion, ratio As Double) As Dictionary(Of String, Image)
        Dim output As New Dictionary(Of String, Image)
        Dim img As Image
        Dim maxWidth As Integer = canvas.PlotRegion.Width * ratio
        Dim maxHeight As Integer = canvas.PlotRegion.Height * ratio

        For Each item In images
            img = item.Value

            If img.Width > img.Height Then
                ' via height
                output(item.Key) = ResizeThisWidth(img, maxwidth:=maxHeight / (img.Height / img.Width))
            Else
                ' via width
                output(item.Key) = ResizeThisWidth(img, maxWidth)
            End If
        Next

        Return output
    End Function

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim images = ResizeImages(canvas, ratio:=0.2)
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

        Dim ZERO As New PointF(rect.Left, rect.Bottom)
        Dim RIGHT As New PointF(rect.Right, rect.Bottom)

        Call g.DrawLine(Stroke.TryParse(theme.axisStroke), ZERO, RIGHT)

        Dim labelSize As SizeF
        Dim barStyle As Stroke = Stroke.TryParse(theme.lineStroke)
        Dim barColor As Brush = barStyle.fill.GetBrush
        Dim barHighlight As Brush = Me.barHighlight.GetBrush
        Dim label As String

        label = "M/z ratio"
        labelSize = g.MeasureString(label, CSSFont.TryParse(theme.axisLabelCSS))
        RIGHT = New PointF(rect.Right - labelSize.Width, rect.Bottom + 5)

        g.DrawString(label, CSSFont.TryParse(theme.axisLabelCSS), Brushes.Black, RIGHT)

        Dim labels As New List(Of Label)

        For Each product As ms2 In matrix
            Dim pt As PointF = scaler.Translate(product.mz, product.intensity / maxinto * 100)
            Dim bar As New RectangleF With {
                .X = pt.X - barStyle.width / 2,
                .Y = pt.Y,
                .Height = bottomY - pt.Y,
                .Width = barStyle.width
            }

            label = product.Annotation

            If Not label.StringEmpty Then
                If images.ContainsKey(label) Then
                    labelSize = images(label).Size.SizeF
                Else
                    label = label.DoWordWrap(20, "")
                    labelSize = g.MeasureString(label, labelFont)
                End If

                pt = New PointF(pt.X - labelSize.Height / 2, pt.Y - 10)

                Call g.FillRectangle(barHighlight, bar)
                Call New Label With {
                    .text = label,
                    .X = pt.X, .Y = pt.Y,
                    .width = labelSize.Width,
                    .height = labelSize.Height,
                    .pinned = False
                }.DoCall(AddressOf labels.Add)
            Else
                Call g.FillRectangle(barColor, bar)
            End If

            labels.Add(New Label With {.height = bar.Height, .pinned = True, .width = bar.Width, .X = bar.X, .Y = bar.Y})

            If product.intensity / maxinto >= 0.2 Then
                label = product.mz.ToString("F2")
                labelSize = g.MeasureString(label, labelFont)
                pt = New PointF With {
                    .X = pt.X - labelSize.Width / 2,
                    .Y = pt.Y - labelSize.Height
                }

                Call g.DrawString(label, labelFont, Brushes.Black, pt)
                Call labels.Add(New Label With {.height = labelSize.Height, .pinned = True, .width = labelSize.Width, .X = pt.X, .Y = pt.Y})
            End If
        Next

        labelSize = g.MeasureString(title, titleFont)

        Dim location As New PointF With {
            .X = (canvas.Width - labelSize.Width) / 2,
            .Y = (rect.Top - labelSize.Height) / 2
        }

        Call g.DrawString(title, titleFont, Brushes.Black, location)

        Dim anchors As Anchor() = labels.GetLabelAnchors(r:=1)

        Call d3js.forcedirectedLabeler _
           .Labels(labels) _
           .Anchors(anchors) _
           .Width(rect.Width) _
           .Height(rect.Height) _
           .WithOffset(rect.Location) _
           .Start(showProgress:=False, nsweeps:=1500)

        Dim labelBrush As Brush = theme.tagColor.GetBrush
        Dim labelConnector As Pen = Stroke.TryParse(theme.tagLinkStroke)

        labelConnector.EndCap = LineCap.ArrowAnchor

        For Each i As SeqValue(Of Label) In labels.SeqIterator
            If i.value.pinned Then
                Continue For
            End If

            Call g.DrawLine(labelConnector, i.value.GetTextAnchor(anchors(i)), anchors(i))

            If images.ContainsKey(i.value.text) Then
                Call g.DrawImageUnscaled(images(i.value.text), i.value.rectangle.Location.ToPoint)
            Else
                Call g.DrawString(i.value.text, labelFont, labelBrush, i.value)
            End If
        Next
    End Sub

    Public Shared Function DrawSpectrumPeaks(matrix As LibraryMatrix,
                                             Optional size$ = "1600,1080",
                                             Optional padding$ = "padding:150px 100px 85px 125px;",
                                             Optional bg$ = "white",
                                             Optional gridFill$ = "white",
                                             Optional barHighlight$ = NameOf(Color.DarkRed),
                                             Optional barStroke$ = "stroke: skyblue; stroke-width: 5px; stroke-dash: solid;",
                                             Optional titleCSS$ = "font-style: normal; font-size: 16; font-family: " & FontFace.MicrosoftYaHei & ";",
                                             Optional labelCSS$ = "font-style: normal; font-size: 8; font-family: " & FontFace.MicrosoftYaHei & ";",
                                             Optional axisLabelCSS$ = "font-style: normal; font-size: 10; font-family: " & FontFace.MicrosoftYaHei & ";",
                                             Optional axisTicksCSS$ = "font-style: normal; font-size: 10; font-family: " & FontFace.SegoeUI & ";",
                                             Optional axisStroke$ = Stroke.AxisStroke,
                                             Optional images As Dictionary(Of String, Image) = Nothing) As GraphicsData

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
        Dim app As New PeakAssign(matrix.name, matrix.ms2, barHighlight, theme, images)

        Return app.Plot(size, ppi:=200)
    End Function
End Class
