Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq

<HideModuleName>
Public Module Extensions

    <Extension>
    Public Function Geometry2D(polygons As IEnumerable(Of Polygon2D),
                               dimension As Size,
                               label As String,
                               color As Color) As TissueRegion

        Dim x As New List(Of Integer)
        Dim y As New List(Of Integer)
        Dim regions As Polygon2D() = polygons.SafeQuery.ToArray

        For i As Integer = 1 To dimension.Width
            For j As Integer = 1 To dimension.Height
#Disable Warning
                If regions.Any(Function(r) r.inside(i, j)) Then
                    x.Add(i)
                    y.Add(j)
                End If
#Enable Warning
            Next
        Next

        Return New TissueRegion With {
            .color = color,
            .label = label,
            .points = x _
                .Select(Function(xi, i)
                            Return New Point(xi, y(i))
                        End Function) _
                .ToArray
        }
    End Function
End Module
