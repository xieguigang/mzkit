#Region "Microsoft.VisualBasic::09771544be37abc74220f87908b64c78, visualize\MsImaging\Blender\Ruler.vb"

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

    '   Total Lines: 94
    '    Code Lines: 72
    ' Comment Lines: 2
    '   Blank Lines: 20
    '     File Size: 3.58 KB


    '     Class Ruler
    ' 
    '         Properties: width
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: eval
    ' 
    '         Sub: DrawOnCanvas, DrawOnImage, layout
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.MIME.Html.CSS

Namespace Blender

    Public Class Ruler

        ReadOnly theme As Theme

        Public Property width As Double?

        <DebuggerStepThrough>
        Sub New(theme As Theme)
            Me.theme = theme
        End Sub

        Public Sub DrawOnImage(MSI As Image, dimension As Size, color As Color, resolution As Double)
            Using g As Graphics = Graphics.FromImage(MSI)
                Dim canvas As New Graphics2D(g, MSI.Size)

                g.InterpolationMode = InterpolationMode.HighQualityBicubic
                g.PixelOffsetMode = PixelOffsetMode.HighQuality
                g.CompositingQuality = CompositingQuality.HighQuality
                g.SmoothingMode = SmoothingMode.HighQuality
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit

                Call DrawOnCanvas(
                    g:=canvas,
                    dimsize:=dimension,
                    rect:=New Rectangle(New Point, MSI.Size),
                    color:=color,
                    resolution:=resolution
                )
            End Using
        End Sub

        Private Function eval(rect As Rectangle, dimsize As Size, resolution As Double, ByRef rulerWidth As Double) As Double
            Dim ratio As Double = rect.Width / dimsize.Width

            If width Is Nothing OrElse width <= 0 Then
                rulerWidth = rect.Width * 0.2
                Return (rulerWidth / ratio * resolution)
            Else
                ratio = dimsize.Width / rect.Width
                rulerWidth = ratio * width
                Return CDbl(width)
            End If
        End Function

        Private Sub layout(rect As Rectangle, rulerWidth As Double, bottom As Double,
                           <Out> ByRef left As PointF,
                           <Out> ByRef right As PointF)

            Dim offsetX As Double = rect.Width * 0.05
            Dim padding As Double = 12.5

            If offsetX < padding Then
                offsetX = padding
            End If

            left = New PointF(rect.Left + offsetX, bottom)
            right = New PointF(left.X + rulerWidth, bottom)
        End Sub

        Public Sub DrawOnCanvas(g As IGraphics, dimsize As Size, rect As Rectangle, color As Color, resolution As Double)
            ' width drawing on the canvas
            Dim rulerWidth As Double = 0
            Dim pen As Pen = Stroke.TryParse(theme.lineStroke).GDIObject
            Dim font As Font = CSSFont.TryParse(theme.tagCSS).GDIObject(g.Dpi)
            ' width of the ruler standards for
            Dim physical As String = eval(rect, dimsize, resolution, rulerWidth).ToString("F2") & " um"
            Dim fontsize As SizeF = g.MeasureString(physical, font)
            Dim left, right As PointF
            Dim bottom As Double = rect.Bottom - fontsize.Height * 2

            Call layout(rect, rulerWidth, bottom, left, right)

            Dim center As New PointF(left.X + (rulerWidth - fontsize.Width) / 2, bottom + 10)

#Disable Warning
            pen.Color = color

            Call g.DrawLine(pen, left, right)
            Call g.DrawString(physical, font, New SolidBrush(color), center)
#Enable Warning
        End Sub

    End Class
End Namespace
