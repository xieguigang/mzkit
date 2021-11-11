Imports System.Drawing
Imports Microsoft.VisualBasic.Data.GraphTheory

Namespace Imaging

    Public Class KnnInterpolation

        Public Shared Function KnnFill(layer As SingleIonLayer, Optional resolution As Integer = 10) As SingleIonLayer
            Dim graph As Grid(Of PixelData) = Grid(Of PixelData).Create(layer.MSILayer)
            Dim size As Size = layer.DimensionSize
            Dim dx As Integer = size.Width / resolution
            Dim dy As Integer = size.Height / resolution
            Dim pixels As New List(Of PixelData)
            Dim point As PixelData

            For i As Integer = 1 To size.Width
                For j As Integer = 1 To size.Height
                    point = graph.GetData(i, j)

                    If point Is Nothing Then

                    End If
                Next
            Next

        End Function

    End Class
End Namespace