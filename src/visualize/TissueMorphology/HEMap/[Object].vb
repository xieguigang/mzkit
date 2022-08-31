Imports System.Drawing
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra

''' <summary>
''' layer object data
''' </summary>
Public Class [Object]

    Public Property Pixels As Integer
    Public Property Density As Double
    Public Property Ratio As Double

    Public Shared Function Eval(rect As Grid(Of Color),
                                target As Color,
                                gridSize As Integer,
                                Optional tolerance As Integer = 5,
                                Optional densityGrid As Integer = 5) As [Object]

        Dim hits As New List(Of Integer)
        Dim A As Double = densityGrid ^ 2

        rect = rect _
            .Cells _
            .Where(Function(c)
                       Return c.data.Equals(target, tolerance:=tolerance)
                   End Function) _
            .DoCall(AddressOf Grid(Of Color).CreateReadOnly)

        For x As Integer = 1 To gridSize Step densityGrid
            For y As Integer = 1 To gridSize Step densityGrid
                hits.Add(rect.Query(x, y, densityGrid).Count)
            Next
        Next

        Return New [Object] With {
            .Pixels = rect.Cells.Count,
            .Density = (New Vector(integers:=hits) / A).Average,
            .Ratio = .Pixels / (gridSize ^ 2)
        }
    End Function

End Class