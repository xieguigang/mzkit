Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports stdNum = System.Math

Public Class ColorScaleMap

    Public Property range As Double()
        Get
            Return _range
        End Get
        Set(value As Double())
            _range = value
            Call refreshSlideBar()
        End Set
    End Property

    Public Property mapLevels As Integer
        Get
            Return _maplevels
        End Get
        Set(value As Integer)
            _maplevels = value
            Call refreshSlideBar()
        End Set
    End Property

    Public Property colorMap As ScalerPalette
        Get
            Return _mapName
        End Get
        Set
            _mapName = Value
            _colorMap = Designer _
                .GetColors(Value.Description, _maplevels) _
                .Select(Function(c) New SolidBrush(c)) _
                .ToArray

            Call refreshSlideBar()
        End Set
    End Property

    Dim WithEvents refreshTrigger As New Timer()
    Dim drag As Boolean = False
    Dim min As Integer
    Dim max As Integer
    Dim isMin As Boolean
    Dim isMax As Boolean

    Dim _maplevels As Integer
    Dim _range As Double() = {0, 1}
    Dim _colorMap As SolidBrush()
    Dim _mapName As ScalerPalette

    Private Sub SlideBar_MouseMove(sender As Object, e As MouseEventArgs) Handles SlideBar.MouseMove
        If drag Then
            If isMin Then
                min = e.X
            ElseIf isMax Then
                max = e.X
            Else
                ' do nothing 
            End If
        End If
    End Sub

    Private Sub SlideBar_MouseDown(sender As Object, e As MouseEventArgs) Handles SlideBar.MouseDown
        drag = True

        ' check item
        ' is min or max
        If stdNum.Abs(e.X - min) < 10 Then
            isMin = True
            isMax = False
        ElseIf stdNum.Abs(e.X - max) < 10 Then
            isMax = True
            isMin = False
        Else
            isMin = False
            isMax = False
        End If
    End Sub

    Private Sub SlideBar_MouseUp(sender As Object, e As MouseEventArgs) Handles SlideBar.MouseUp
        drag = False
    End Sub

    Private Sub refreshSlideBar()
        Dim width As Integer = Me.Width
        Dim dw As Integer = width / _maplevels
        Dim h As Integer = Height - SlideBar.Height
        Dim g As Graphics = colorMapDisplay.CreateGraphics
        Dim x As Integer

        For Each col As SolidBrush In _colorMap
            g.FillRectangle(col, New Rectangle(x, 0, dw, h))
        Next

        Call g.Flush()
    End Sub

    Private Sub ColorScaleMap_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub

    Private Sub refreshTrigger_Tick(sender As Object, e As EventArgs) Handles refreshTrigger.Tick
        Call SlideBar.Invalidate()
    End Sub
End Class
