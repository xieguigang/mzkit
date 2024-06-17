#Region "Microsoft.VisualBasic::73edbee682fe658ce50d2adb5db65538, visualize\plot\PeakAssign.vb"

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

    '   Total Lines: 364
    '    Code Lines: 297 (81.59%)
    ' Comment Lines: 14 (3.85%)
    '    - Xml Docs: 71.43%
    ' 
    '   Blank Lines: 53 (14.56%)
    '     File Size: 15.97 KB


    ' Class PeakAssign
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: DrawSpectrumPeaks, ResizeImages, ResizeThisWidth
    ' 
    '     Sub: PlotInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.d3js.Layout
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render

''' <summary>
''' 通过KCF图模型为ms2二级质谱碎片鉴定分子碎片的具体的结构式
''' </summary>
Public Class PeakAssign : Inherits Plot

    ReadOnly matrix As ms2()
    ReadOnly title As String
    ReadOnly barHighlight As String
    ReadOnly images As Dictionary(Of String, Image)
    ReadOnly labelIntensity As Double = 0.3

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="title$"></param>
    ''' <param name="matrix"></param>
    ''' <param name="barHighlight"></param>
    ''' <param name="theme"></param>
    ''' <param name="images">the annotated molecular parts image</param>
    Public Sub New(title$,
                   matrix As IEnumerable(Of ms2),
                   barHighlight As String,
                   labelIntensity As Double,
                   theme As Theme,
                   Optional images As Dictionary(Of String, Image) = Nothing)

        MyBase.New(theme)

        Me.labelIntensity = labelIntensity
        Me.title = title
        Me.matrix = matrix.ToArray
        Me.barHighlight = barHighlight

        If images Is Nothing Then
            Me.images = New Dictionary(Of String, Image)
        Else
            Me.images = images
        End If

        xlabel = "M/z ratio"
        ylabel = "Relative Intensity (%)"
    End Sub

    Private Function ResizeThisWidth(original As Image, maxwidth As Integer) As Size
        Dim intWidth As Integer = original.Width
        Dim intHeight As Integer = original.Height
        Dim newWidth, newHeight As Integer

        If intWidth > maxwidth Then
            newWidth = maxwidth
            newHeight = maxwidth * (intHeight / intWidth)

            Return New Size(newWidth, newHeight)
        Else
            Return original.Size
        End If
    End Function

    Private Function ResizeImages(canvas As GraphicsRegion, ratio As Double) As Dictionary(Of String, (img As Image, size As Size))
        Dim output As New Dictionary(Of String, (Image, Size))
        Dim img As Image
        Dim maxWidth As Integer = canvas.PlotRegion.Width * ratio
        Dim maxHeight As Integer = canvas.PlotRegion.Height * ratio

        For Each item In images
            img = item.Value

            If img.Width > img.Height Then
                ' via height
                output(item.Key) = (img, ResizeThisWidth(img, maxwidth:=maxHeight / (img.Height / img.Width)))
            Else
                ' via width
                output(item.Key) = (img, ResizeThisWidth(img, maxWidth))
            End If
        Next

        Return output
    End Function

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim images = ResizeImages(canvas, ratio:=0.2)

        If matrix.Length = 0 Then
            Call "MS matrix is empty in peak assign plot!".Warning
            Return
        End If

        Dim maxinto As Double = matrix.Select(Function(p) p.intensity).Max
        Dim rect As RectangleF = canvas.PlotRegion.ToFloat
        Dim xticks As Double() = (matrix.Select(Function(p) p.mz).Range * 1.125).CreateAxisTicks

        If xticks.All(Function(ti) ti = xticks(0)) Then
            Dim xtmp As Double = matrix.Select(Function(p) p.mz).Average

            xticks = {xtmp, xtmp * 0.85, xtmp * 1.125}
            xticks = xticks.CreateAxisTicks
        End If

        Dim xscale = d3js.scale.linear().domain(values:=xticks).range(values:={rect.Left, rect.Right})
        Dim yscale = d3js.scale.linear().domain(values:=New Double() {0, 110}).range(values:={rect.Top, rect.Bottom})
        Dim scaler As New DataScaler() With {
            .AxisTicks = (xticks.Take(xticks.Length - 1).AsVector, New Vector(New Double() {0, 20, 40, 60, 80, 100})),
            .region = canvas.PlotRegion,
            .X = xscale,
            .Y = yscale
        }
        Dim css As CSSEnvirnment = g.LoadEnvironment
        Dim bottomY As Double = rect.Bottom
        Dim labelFont As Font = css.GetFont(CSSFont.TryParse(theme.tagCSS))
        Dim titleFont As Font = css.GetFont(CSSFont.TryParse(theme.mainCSS))

        Call Axis.DrawAxis(
            g, canvas, scaler,
            showGrid:=True,
            xlabel:=xlabel,
            ylabel:=ylabel,
            XtickFormat:="F0",
            YtickFormat:="F0",
            gridFill:=theme.gridFill,
            xlayout:=XAxisLayoutStyles.Bottom,
            tickFontStyle:=theme.axisTickCSS,
            gridX:=Nothing,
            axisStroke:=theme.axisStroke,
            htmlLabel:=False,
            labelFont:=theme.axisLabelCSS
        )

        Dim ZERO As New PointF(rect.Left, rect.Bottom)
        Dim RIGHT As New PointF(rect.Right, rect.Bottom)

        Call g.DrawLine(css.GetPen(Stroke.TryParse(theme.axisStroke)), ZERO, RIGHT)

        Dim labelSize As SizeF
        Dim barStyle As Stroke = Stroke.TryParse(theme.lineStroke)
        Dim barColor As Brush = barStyle.fill.GetBrush
        Dim barWidth As Single = css.GetLineWidth(barStyle)
        Dim barHighlight As Brush = Me.barHighlight.GetBrush
        Dim label As String

        label = xlabel
        labelSize = g.MeasureString(label, css.GetFont(theme.axisLabelCSS))
        RIGHT = New PointF(rect.Right - labelSize.Width, rect.Bottom + 5)

        g.DrawString(label, css.GetFont(theme.axisLabelCSS), Brushes.Black, RIGHT)

        Dim labels As New List(Of Label)
        Dim anchors As New List(Of Anchor)

        Static empty As Index(Of String) = {"-", "_", "NULL", "NA", "null"}

        For Each product As ms2 In matrix
            Dim pt As PointF = scaler.Translate(product.mz, product.intensity / maxinto * 100)
            Dim bar As New RectangleF With {
                .X = pt.X - barWidth / 2,
                .Y = pt.Y,
                .Height = bottomY - pt.Y,
                .Width = barWidth
            }
            Dim drawMzLabel As Boolean = product.intensity / maxinto >= labelIntensity

            label = product.Annotation

            If (product.intensity / maxinto > 0.05) AndAlso (Not label.StringEmpty) AndAlso (Not label Like empty) Then
                If images.ContainsKey(label) Then
                    labelSize = images(label).size.SizeF
                Else
                    Dim block = label.Match("\[.+\]")

                    If Not block.StringEmpty Then
                        Dim name = label.Replace(block, "").Trim

                        If name.Length > 8 OrElse block.Length > 8 Then
                            If Not (block.StringEmpty OrElse name.StringEmpty) Then
                                If block.Length > name.Length Then
                                    label = block & vbCrLf & New String(" "c, (block.Length - name.Length) / 2) & name
                                Else
                                    label = New String(" "c, (name.Length - block.Length) / 2) & block & vbCrLf & name
                                End If
                            End If
                        End If
                    End If

                    labelSize = g.MeasureString(label, labelFont)
                End If

                If drawMzLabel Then
                    pt = New PointF(bar.X + bar.Width / 2, bar.Y - g.MeasureString(label, labelFont).Height)
                Else
                    pt = bar.Location.OffSet2D(bar.Width / 2, 0)
                End If

                Call g.FillRectangle(barHighlight, bar)

                If (Not images.ContainsKey(label)) AndAlso label.Length < 18 Then
                    pt = New PointF With {
                        .X = pt.X - labelSize.Width / 2,
                        .Y = pt.Y - labelSize.Height
                    }
                    g.DrawString(label, labelFont, Brushes.Black, pt)
                Else
                    Call anchors.Add(pt)

                    If images.ContainsKey(label) Then
                        Call New Label With {
                            .text = label,
                            .X = pt.X - labelSize.Width / 2, .Y = 0,
                            .width = labelSize.Width,
                            .height = labelSize.Height,
                            .pinned = False
                        }.DoCall(AddressOf labels.Add)
                    Else
                        Call New Label With {
                            .text = label,
                            .X = pt.X - labelSize.Width / 2, .Y = pt.Y + labelSize.Height,
                            .width = labelSize.Width,
                            .height = labelSize.Height,
                            .pinned = False
                        }.DoCall(AddressOf labels.Add)
                    End If
                End If
            Else
                Call g.FillRectangle(barColor, bar)
            End If

            labels.Add(New Label With {.height = bar.Height, .pinned = True, .width = bar.Width, .X = bar.X, .Y = bar.Y})
            anchors.Add(New Anchor)

            If drawMzLabel Then
                label = product.mz.ToString("F2")
                labelSize = g.MeasureString(label, labelFont)
                pt = bar.Location
                pt = New PointF With {
                    .X = pt.X - labelSize.Width / 2,
                    .Y = pt.Y - labelSize.Height
                }

                Call g.DrawString(label, labelFont, Brushes.Black, pt)
                Call labels.Add(New Label With {.height = labelSize.Height, .pinned = True, .width = labelSize.Width, .X = pt.X, .Y = pt.Y})
                Call anchors.Add(New Anchor)
            End If
        Next

        labelSize = g.MeasureString(title, titleFont)

        Dim location As New PointF With {
            .X = (canvas.Width - labelSize.Width) / 2,
            .Y = (rect.Top - labelSize.Height) / 2
        }

        Call g.DrawString(title, titleFont, Brushes.Black, location)

        If theme.drawLabels Then
            Dim labelBrush As Brush = theme.tagColor.GetBrush
            Dim labelConnector As Pen = css.GetPen(Stroke.TryParse(theme.tagLinkStroke))
            Dim connectorLead As Pen = css.GetPen(Stroke.TryParse(theme.tagLinkStroke))

            Call d3js.forcedirectedLabeler(
                    ejectFactor:=2,
                    dist:=$"50,{canvas.Width / 2}",
                    condenseFactor:=100,
                    avoidRegions:={}' {New RectangleF(rect.Left, rect.Bottom - rect.Height * 0.2, rect.Width, rect.Height * 0.2)}
                 ) _
               .Labels(labels) _
               .Anchors(anchors) _
               .Width(rect.Width) _
               .Height(rect.Height) _
               .WithOffset(rect.Location) _
               .Start(showProgress:=False, nsweeps:=2500)

            labelConnector.EndCap = LineCap.ArrowAnchor

            ' show the annotation labels
            For Each i As SeqValue(Of Label) In labels.SeqIterator
                If i.value.pinned Then
                    Continue For
                End If

                Dim a As PointF = i.value.GetTextAnchor(anchors(i))
                Dim b As PointF = anchors(i)
                Dim c As New PointF With {.X = a.X - (a.X - b.X) * 0.65, .Y = a.Y}

                If a.Y >= i.value.rectangle.Bottom Then
                    Call g.DrawLine(labelConnector, a, b)
                Else
                    Call g.DrawLine(connectorLead, a, c)
                    Call g.DrawLine(labelConnector, c, b)
                End If

                If images.ContainsKey(i.value.text) Then
                    Call g.DrawImage(images(i.value.text).img, i.value.rectangle)
                Else
                    Call g.DrawString(i.value.text, labelFont, labelBrush, i.value.location)
                End If
            Next
        End If
    End Sub

    Public Shared Function DrawSpectrumPeaks(matrix As LibraryMatrix,
                                             Optional size$ = "1280,900",
                                             Optional padding$ = "padding:100px 50px 150px 150px;",
                                             Optional bg$ = "white",
                                             Optional gridFill$ = "white",
                                             Optional title As String = Nothing,
                                             Optional barHighlight$ = NameOf(Color.DarkRed),
                                             Optional barStroke$ = "stroke: steelblue; stroke-width: 5px; stroke-dash: solid;",
                                             Optional titleCSS$ = "font-style: normal; font-size: 16; font-family: " & FontFace.MicrosoftYaHei & ";",
                                             Optional labelCSS$ = "font-style: normal; font-size: 8; font-family: " & FontFace.MicrosoftYaHei & ";",
                                             Optional axisLabelCSS$ = "font-style: normal; font-size: 10; font-family: " & FontFace.MicrosoftYaHei & ";",
                                             Optional axisTicksCSS$ = "font-style: normal; font-size: 10; font-family: " & FontFace.SegoeUI & ";",
                                             Optional axisStroke$ = Stroke.AxisStroke,
                                             Optional connectorStroke$ = "stroke: gray; stroke-width: 3.5px; stroke-dash: dash;",
                                             Optional labelIntensity As Double = 0.3,
                                             Optional images As Dictionary(Of String, Image) = Nothing,
                                             Optional xlabel$ = "M/z ratio",
                                             Optional ylabel$ = "Relative Intensity (%)",
                                             Optional showAnnotationText As Boolean = True,
                                             Optional driver As Drivers = Drivers.Default,
                                             Optional dpi As Integer = 300) As GraphicsData
        Dim theme As New Theme With {
            .padding = padding,
            .background = bg,
            .gridFill = gridFill,
            .lineStroke = barStroke,
            .mainCSS = titleCSS,
            .tagCSS = labelCSS,
            .axisTickCSS = axisTicksCSS,
            .axisStroke = axisStroke,
            .axisLabelCSS = axisLabelCSS,
            .tagLinkStroke = connectorStroke,
            .drawLabels = showAnnotationText
        }
        Dim app As New PeakAssign(
            title:=title Or matrix.name.AsDefault,
            matrix:=matrix.ms2,
            barHighlight:=barHighlight,
            labelIntensity:=labelIntensity, ' If(showAnnotationText, labelIntensity, Single.MaxValue),
            theme:=theme,
            images:=images
        ) With {
            .xlabel = xlabel,
            .ylabel = ylabel
        }

        Return app.Plot(size, ppi:=dpi, driver:=driver)
    End Function
End Class
