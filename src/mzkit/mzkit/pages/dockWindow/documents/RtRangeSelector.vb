Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language

Public Class RtRangeSelector

    Public Event RangeSelect(min As Double, max As Double)

    Public Property SelectedColor As Color = Color.Green
    Public Property FillColor As Color = Color.Blue

    Dim start As Integer
    Dim endPox As Integer
    Dim lastX As Integer

    Dim onSelect As Boolean
    Dim onMoveRange As Boolean
    Dim TIC As ChromatogramTick()
    Dim RtRange As Double

    Public Sub SetTIC(data As ChromatogramTick())
        TIC = data
        RtRange = data.Last.Time - data.First.Time

        Using g = Me.CreateGraphics
            Call g.FillRectangle(New SolidBrush(BackColor), New RectangleF(0, 0, Width, Height))
            Call DrawTIC(g)
        End Using
    End Sub

    Private Sub RtRangeSelector_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        If e.X > start AndAlso e.X < endPox Then
            onMoveRange = True
            lastX = e.X
        Else
            start = e.X
            onSelect = True
        End If
    End Sub

    Private Sub RtRangeSelector_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        endPox = e.X
        onSelect = False
        onMoveRange = False

        Dim length As Double = Width

        With {start, endPox}
            Dim min As Double = .Min / length
            Dim max As Double = .Max / length

            min = TIC(Scan0).Time + min * RtRange
            max = TIC(Scan0).Time + max * RtRange

            RaiseEvent RangeSelect(min, max)

            start = .Min
            endPox = .Max
        End With
    End Sub

    Private Sub RtRangeSelector_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If onSelect Then
            endPox = e.X
        ElseIf onMoveRange Then
            Dim dx = e.X - lastX

            lastX = e.X
            start += dx
            endPox += dx
        End If
    End Sub

    Private Sub RtRangeSelector_Load(sender As Object, e As EventArgs) Handles Me.Load
        Timer1.Interval = 30
        Timer1.Enabled = True
        Timer1.Start()

        DoubleBuffered = True
    End Sub

    Private Sub DrawTIC(g As Graphics)
        If TIC.IsNullOrEmpty Then
            Return
        End If

        Using TICcurve As New GraphicsPath
            Dim height As Double = Me.Height
            Dim width As Double = Me.Width
            Dim scaleX = d3js.scale.linear.domain(TIC.Select(Function(x) x.Time)).range(New Double() {0, width})
            Dim scaleY = d3js.scale.linear.domain(TIC.Select(Function(x) x.Intensity)).range(New Double() {0, height})
            Dim i As i32 = Scan0

            For j As Integer = 1 To TIC.Length - 1
                TICcurve.AddLine(CSng(scaleX(TIC(i).Time)), CSng(height - scaleY(TIC(++i).Intensity)), CSng(scaleX(TIC(j).Time)), CSng(height - scaleY(TIC(j).Intensity)))
            Next

            TICcurve.CloseAllFigures()
            g.FillPath(New SolidBrush(FillColor), TICcurve)
        End Using
    End Sub

    Public Sub SetRange(left As Double, right As Double)
        start = (left - TIC(Scan0).Time) / RtRange
        endPox = (right - TIC.Last.Time) / RtRange

        Call RefreshSelector()
    End Sub

    Public Sub RefreshSelector()
        Dim left = {start, endPox}.Min
        Dim right = {start, endPox}.Max

        Using g = Me.CreateGraphics
            Call g.FillRectangle(New SolidBrush(BackColor), New RectangleF(0, 0, Width, Height))
            Call DrawTIC(g)
            Call g.FillRectangle(New SolidBrush(SelectedColor.Alpha(125)), New RectangleF(left, 0, right - left, Height))
        End Using
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If onSelect OrElse onMoveRange Then
            Call RefreshSelector()
        End If
    End Sub

    Private Sub RtRangeSelector_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Using g = Me.CreateGraphics
            Call g.FillRectangle(New SolidBrush(BackColor), New RectangleF(0, 0, Width, Height))
            Call DrawTIC(g)
        End Using
    End Sub
End Class
