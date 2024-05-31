#Region "Microsoft.VisualBasic::62452009c45301cf387ce9171db48a2a, visualize\plot\PeakTablePlot.vb"

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

    '   Total Lines: 87
    '    Code Lines: 74 (85.06%)
    ' Comment Lines: 1 (1.15%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 12 (13.79%)
    '     File Size: 3.88 KB


    ' Class PeakTablePlot
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: PlotInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Scaler
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render

Public Class PeakTablePlot : Inherits Plot

    ReadOnly peakSet As PeakSet
    ReadOnly mapLevels As Integer = 64
    ReadOnly cut As Double = 0.65

    Public Sub New(peakSet As PeakSet, theme As Theme)
        MyBase.New(theme)

        Me.peakSet = peakSet.Norm
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim xTicks As Double() = peakSet.peaks.Select(Function(pk) pk.rt).Range.CreateAxisTicks
        Dim rect As Rectangle = canvas.PlotRegion
        Dim scaleX = d3js.scale.linear.domain(values:=xTicks).range(integers:={rect.Left, rect.Right})
        Dim sampleNames As String() = peakSet.sampleNames
        Dim dy As Double = rect.Height / sampleNames.Length
        Dim css As CSSEnvirnment = g.LoadEnvironment
        Dim idFont As Font = css.GetFont(CSSFont.TryParse(theme.axisLabelCSS))
        Dim lbSize As SizeF
        Dim y As Double = rect.Top
        Dim x As Double
        Dim dot As RectangleF
        Dim colors As SolidBrush() = Designer.GetColors(theme.colorSet, mapLevels).Select(Function(c) New SolidBrush(c)).ToArray
        Dim indexRange As New DoubleRange(0, mapLevels)
        Dim allIntensity As Double() = peakSet.peaks.Select(Function(pk) pk.Properties.Values).IteratesALL.ToArray
        Dim qcut As Double = TrIQ.FindThreshold(allIntensity, Me.cut, N:=mapLevels)
        Dim valueRange As DoubleRange = New Double() {allIntensity.Min, qcut}
        Dim color As Integer
        Dim strokePen As Pen = css.GetPen(Stroke.TryParse(theme.axisStroke))
        Dim scaler As New DataScaler With {
            .region = rect,
            .X = scaleX,
            .Y = d3js.scale.linear.domain(values:={0.0, 1.0}).range(integers:={rect.Top, rect.Bottom}),
            .AxisTicks = (xTicks.AsVector, Nothing)
        }
        Dim tickFont As Font = css.GetFont(CSSFont.TryParse(theme.axisTickCSS))
        Dim pos As PointF

        Call Axis.DrawX(g, strokePen, "Retention Time(s)", scaler, XAxisLayoutStyles.Bottom, 0, Nothing, theme.axisLabelCSS, Brushes.Black, tickFont, Brushes.Black, htmlLabel:=False)

        ' for each sample as matrix row
        For Each sampleId As String In sampleNames
            lbSize = g.MeasureString(sampleId, idFont)
            pos = New PointF(rect.Left - lbSize.Width / 2, y + (dy - lbSize.Height) / 2)
            g.DrawString(sampleId, idFont, Brushes.Black, pos)

            For Each peak In peakSet.peaks
                x = scaleX(peak.rt)
                dot = New RectangleF With {
                    .X = x,
                    .Y = y,
                    .Width = 5,
                    .Height = dy
                }
                color = CInt(valueRange.ScaleMapping(peak(sampleId), indexRange))

                If color < 0 Then color = 0
                If color >= colors.Length Then color = colors.Length - 1

                If color = 0 Then
                    Call g.FillRectangle(Brushes.White, rect:=dot)
                Else
                    Call g.FillRectangle(colors(color), rect:=dot)
                End If
            Next

            y += dy

            Call Console.WriteLine(sampleId)
        Next
    End Sub
End Class
