Imports System.Drawing
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS

Public Class PlotProperty

    Public Property width As Integer
    Public Property height As Integer
    Public Property background As Color
    Public Property padding As New PaddingHelper
    Public Property showLegend As Boolean = True
    Public Property lineWidth As Single = 5

End Class

Public Structure PaddingHelper

    Public Property top As Integer
    Public Property right As Integer
    Public Property bottom As Integer
    Public Property left As Integer

    Public Function GetPadding() As Padding
        Return New Padding(left, top, right, bottom)
    End Function

    Public Overrides Function ToString() As String
        Return GetPadding.ToString
    End Function

End Structure