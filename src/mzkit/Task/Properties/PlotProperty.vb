Imports System.ComponentModel
Imports System.Drawing
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS

Public Class PlotProperty

    <Category("Plot")> Public Property width As Integer
    <Category("Plot")> Public Property height As Integer
    <Category("Plot")> Public Property background As Color

    <Category("Padding")> <DisplayName("top")> Public Property padding_top As Integer = 100
    <Category("Padding")> <DisplayName("left")> Public Property padding_left As Integer = 100
    <Category("Padding")> <DisplayName("right")> Public Property padding_right As Integer = 100
    <Category("Padding")> <DisplayName("bottom")> Public Property padding_bottom As Integer = 100

    <Category("Styles")> Public Property showLegend As Boolean = True
    <Category("Styles")> Public Property lineWidth As Single = 5
    <Category("Styles")> Public Property PointSize As Single = 10

    Public Function GetPadding() As Padding
        Return New Padding(padding_left, padding_top, padding_right, padding_bottom)
    End Function

End Class
