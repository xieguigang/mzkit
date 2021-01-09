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
    Dim onSelect As Boolean
    Dim TIC As ChromatogramTick()
    Dim RtRange As Double

    Public Sub SetTIC(data As ChromatogramTick())
        TIC = data
        RtRange = data.Last.Time - data.First.Time

        Using g = Me.CreateGraphics
            Call DrawTIC(g)
        End Using
    End Sub

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

        min = TIC(Scan0).Time + min * RtRange
        max = TIC(Scan0).Time + max * RtRange

        RaiseEvent RangeSelect(min, max)
    End Sub

    Private Sub RtRangeSelector_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If onSelect Then
            endPox = e.X
        End If
    End Sub

    Private Sub RtRangeSelector_Load(sender As Object, e As EventArgs) Handles Me.Load
        Timer1.Interval = 1
        Timer1.Enabled = True
        Timer1.Start()

        DoubleBuffered = True
    End Sub

    Private Sub DrawTIC(g As Graphics)
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

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If onSelect Then
            Dim left = {start, endPox}.Min
            Dim right = {start, endPox}.Max

            Using g = Me.CreateGraphics
                Call g.FillRectangle(New SolidBrush(BackColor), New RectangleF(0, 0, Width, Height))
                Call DrawTIC(g)
                Call g.FillRectangle(New SolidBrush(SelectedColor.Alpha(125)), New RectangleF(left, 0, right - left, Height))
            End Using
        End If
    End Sub
End Class
