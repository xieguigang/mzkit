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

    Public Property ScaleX As Integer
    Public Property ScaleY As Integer

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

    Public Property layers As New Dictionary(Of String, [Object])

    Public ReadOnly Property isBlack As Boolean
        Get
            Return Black.Ratio > 0.975
        End Get
    End Property

End Class

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