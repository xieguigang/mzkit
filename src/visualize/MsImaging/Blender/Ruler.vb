#Region "Microsoft.VisualBasic::bf6e2f5bd469d952f0177d3d235ccd00, visualize\MsImaging\Blender\Ruler.vb"

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
    '    Code Lines: 67 (77.01%)
    ' Comment Lines: 2 (2.30%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 18 (20.69%)
    '     File Size: 3.26 KB


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
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render
Imports std = System.Math

Namespace Blender

    Public Class Ruler

        ReadOnly theme As Theme

        Public Property width As Double?

        <DebuggerStepThrough>
        Sub New(theme As Theme)
            Me.theme = theme
        End Sub

        Public Sub DrawOnImage(MSI As Image, dimension As Size, color As Color, resolution As Double)
            Using g As IGraphics = DriverLoad.CreateGraphicsDevice(MSI)
                Call DrawOnCanvas(
                    g,
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
            Dim css As CSSEnvirnment = g.LoadEnvironment
            Dim pen As Pen = css.GetPen(Stroke.TryParse(theme.lineStroke))
            Dim font As Font = CSS.GetFont(CSSFont.TryParse(theme.tagCSS))
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

        ''' <summary>
        ''' 对微米单位的距离值进行自动格式化优化显示
        ''' </summary>
        ''' <param name="micrometers">以微米为单位的距离值</param>
        ''' <returns>格式化后的距离字符串（单位：km/m/cm/mm/μm）</returns>
        Public Function AutoLengthFormat(micrometers As Double) As String
            ' 处理无效值
            If micrometers <= 0 Then Return "0 μm"
            If micrometers.IsNaNImaginary Then Return "n/a μm"

            ' 定义单位及换算系数（基于1米=10^6微米）
            Dim units As String() = {"km", "m", "cm", "mm", "μm"}
            Dim factors As Double() = {
                0.000000001,  ' 1 km = 10^9 μm
                0.000001,  ' 1 m  = 10^6 μm
                0.0001,  ' 1 cm = 10^4 μm
                0.001,  ' 1 mm = 10^3 μm
                1      ' 1 μm = 1 μm
            }

            ' 计算最适配的单位索引
            Dim exp As Integer

            If micrometers >= factors(0) * 1000000000.0 Then  ' 处理超大值（>1000km）
                exp = 0
            Else
                exp = CInt(std.Floor(std.Log(micrometers) / std.Log(1000)))  ' 对数确定量级
                exp = std.Max(0, std.Min(exp, factors.Length - 1))  ' 限制索引范围
            End If

            ' 换算到目标单位
            Dim value As Double = micrometers * factors(exp)
            ' 动态选择格式：大单位保留小数，小单位取整
            Dim formatStr As String = If(exp <= 1, "N2", "N0")  ' km/m保留2位小数，其余取整

            Return $"{value.ToString(formatStr)} {units(exp)}"    ' 组合结果
        End Function
    End Class
End Namespace
