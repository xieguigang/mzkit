Imports System.Drawing

Public Class PhenographSpot

    Public Property id As String
    Public Property phenograph_cluster As String

    Public Overrides Function ToString() As String
        Return $"[{phenograph_cluster}] {id}"
    End Function

    Public Function GetPixel() As Point
        Dim t As String() = id.Split(","c)
        Dim x As Integer = Integer.Parse(t(0))
        Dim y As Integer = Integer.Parse(t(1))

        Return New Point(x, y)
    End Function

End Class