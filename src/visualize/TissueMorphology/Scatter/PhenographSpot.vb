Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors

Public Class PhenographSpot

    ''' <summary>
    ''' the spot pixel id, in string format of ``[x,y]``.
    ''' </summary>
    ''' <returns></returns>
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

    Public Shared Function GetSpotColorIndex(phenograph As IEnumerable(Of PhenographSpot), Optional colorSet As String = "paper") As Dictionary(Of String, Color)
        Dim classes As String() = phenograph.Select(Function(p) p.phenograph_cluster).Distinct.ToArray
        Dim colors As Color() = Designer.GetColors(colorSet, n:=classes.Length)
        Dim index As New Dictionary(Of String, Color)

        For i As Integer = 0 To classes.Length - 1
            Call index.Add(classes(i), colors(i))
        Next

        Return index
    End Function
End Class