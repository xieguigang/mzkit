Imports stdNum = System.Math

Public Class ColorScaleMap

    Public Property range As Double() = {0, 1}

    Dim WithEvents refreshTrigger As New Timer()
    Dim drag As Boolean = False
    Dim min As Integer
    Dim max As Integer
    Dim isMin As Boolean
    Dim isMax As Boolean

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

    End Sub

    Private Sub ColorScaleMap_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub

    Private Sub refreshTrigger_Tick(sender As Object, e As EventArgs) Handles refreshTrigger.Tick
        Call SlideBar.Invalidate()
    End Sub
End Class
