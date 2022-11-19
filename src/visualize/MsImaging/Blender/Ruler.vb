Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.MIME.Html.CSS

Namespace Blender

    Public Class Ruler

        ReadOnly theme As Theme

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

        Public Sub DrawOnCanvas(g As IGraphics, dimsize As Size, rect As Rectangle, color As Color, resolution As Double)
            Dim ratio As Double = rect.Width / dimsize.Width
            Dim rulerWidth As Double = rect.Width * 0.2
            Dim pen As Pen = Stroke.TryParse(theme.lineStroke).GDIObject
            Dim font As Font = CSSFont.TryParse(theme.tagCSS).GDIObject(g.Dpi)
            Dim physical As String = (rulerWidth / ratio * resolution).ToString("F2") & " um"
            Dim fontsize As SizeF = g.MeasureString(physical, font)
            Dim bottom As Double = rect.Bottom - fontsize.Height * 2
            Dim left As New PointF(rect.Left + rect.Width * 0.05, bottom)
            Dim right As New PointF(left.X + rulerWidth, bottom)
            Dim center As New PointF(left.X + (rulerWidth - fontsize.Width) / 2, bottom + 10)

#Disable Warning
            pen.Color = color

            Call g.DrawLine(pen, left, right)
            Call g.DrawString(physical, font, New SolidBrush(color), center)
#Enable Warning
        End Sub

    End Class
End Namespace