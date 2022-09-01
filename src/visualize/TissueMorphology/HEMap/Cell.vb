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
