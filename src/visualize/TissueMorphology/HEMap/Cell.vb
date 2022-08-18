Imports System.Drawing
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Class Cell

    ''' <summary>
    ''' the location X of the grid
    ''' </summary>
    ''' <returns></returns>
    Public Property X As Integer
    ''' <summary>
    ''' the location Y of the grid
    ''' </summary>
    ''' <returns></returns>
    Public Property Y As Integer
    ''' <summary>
    ''' average value of Red channel
    ''' </summary>
    ''' <returns></returns>
    Public Property R As Double
    ''' <summary>
    ''' average value of Green channel
    ''' </summary>
    ''' <returns></returns>
    Public Property G As Double
    ''' <summary>
    ''' average value of Blue channel
    ''' </summary>
    ''' <returns></returns>
    Public Property B As Double
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property Black As [Object]

End Class

Public Class [Object]

    Public Property Pixels As Integer
    Public Property Density As Double

    Public Shared Function Eval(rect As Grid(Of Color),
                                target As Color,
                                gridSize As Integer,
                                Optional tolerance As Integer = 5,
                                Optional densityGrid As Integer = 5) As [Object]

        Dim hits As New List(Of Integer)

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
            .Density = (New Vector(hits) / (densityGrid ^ 2)).Average
        }
    End Function

End Class