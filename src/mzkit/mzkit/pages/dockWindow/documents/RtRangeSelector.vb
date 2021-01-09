Public Class RtRangeSelector

    Public Event RangeSelect(min As Double, max As Double)

    Public Property fillColor As Color = Color.Green

    Dim start As Integer
    Dim endPox As Integer
    Dim onSelect As Boolean

    Private Sub RtRangeSelector_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        start = e.X
        onSelect = True
    End Sub

    Private Sub RtRangeSelector_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        endPox = e.X
        onSelect = False

        Dim length As Double = Width
        Dim min As Double = {start, endPox}.Min / length
        Dim max As Double = {start, endPox}.Max / length

        RaiseEvent RangeSelect(min, max)
    End Sub

    Private Sub RtRangeSelector_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If onSelect Then
            endPox = e.X
        End If
    End Sub

    Private Sub RtRangeSelector_Load(sender As Object, e As EventArgs) Handles Me.Load
        Timer1.Enabled = True
        Timer1.Start()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If onSelect Then
            Dim left = {start, endPox}.Min
            Dim right = {start, endPox}.Max

            Using g = Me.CreateGraphics
                Call g.FillRectangle(New SolidBrush(BackColor), New RectangleF(0, 0, Width, Height))
                Call g.FillRectangle(New SolidBrush(fillColor), New RectangleF(left, 0, right - left, Height))
            End Using
        End If
    End Sub
End Class
