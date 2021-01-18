Imports System.ComponentModel
Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS

Public Class PlotProperty

    <Category("Plot")> Public Property width As Integer = 2048
    <Category("Plot")> Public Property height As Integer = 1600
    <Category("Plot")> Public Property background As Color = Color.White
    <Category("Plot")> Public Property title As String
    <Category("Plot")> Public Property xlabel As String = "X"
    <Category("Plot")> Public Property ylabel As String = "Y"

    <Category("Padding")> <DisplayName("top")> Public Property padding_top As Integer = 100
    <Category("Padding")> <DisplayName("left")> Public Property padding_left As Integer = 100
    <Category("Padding")> <DisplayName("right")> Public Property padding_right As Integer = 100
    <Category("Padding")> <DisplayName("bottom")> Public Property padding_bottom As Integer = 100

    <Category("Styles")> Public Property show_legend As Boolean = True
    <Category("Styles")> Public Property show_grid As Boolean = True
    <Category("Styles")> Public Property line_width As Single = 5
    <Category("Styles")> Public Property point_size As Single = 10
    <Category("Styles")> Public Property gridFill As Color = "rgb(245,245,245)".TranslateColor

    Public Function GetPadding() As Padding
        Return New Padding(padding_left, padding_top, padding_right, padding_bottom)
    End Function

End Class
