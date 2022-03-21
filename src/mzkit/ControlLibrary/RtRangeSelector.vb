#Region "Microsoft.VisualBasic::80ba40ead49b4071c893d23c3c31fce3, mzkit\src\mzkit\ControlLibrary\RtRangeSelector.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 239
    '    Code Lines: 154
    ' Comment Lines: 27
    '   Blank Lines: 58
    '     File Size: 7.22 KB


    ' Class RtRangeSelector
    ' 
    '     Properties: AllowMoveRange, FillColor, rtmax, rtmin, SelectedColor
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: DrawTIC, OnPaint, RefreshRtRangeSelector, RefreshSelector, RtRangeSelector_Load
    '          RtRangeSelector_MouseDown, RtRangeSelector_MouseMove, RtRangeSelector_MouseUp, RtRangeSelector_Paint, SetRange
    '          SetTIC, Timer1_Tick, updatelabel
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language

Public Class RtRangeSelector

    ''' <summary>
    ''' 返回实际的保留时间的秒数范围
    ''' </summary>
    ''' <param name="min"></param>
    ''' <param name="max"></param>
    Public Event RangeSelect(min As Double, max As Double)

    Public Property SelectedColor As Color = Color.Green
    Public Property FillColor As Color = Color.Blue

    Public Property rtmin As Double
    Public Property rtmax As Double

    Dim start As Integer
    Dim endPox As Integer
    Dim lastX As Integer
    Dim rtX As Integer

    Dim onSelect As Boolean
    Dim onMoveRange As Boolean
    Dim TIC As ChromatogramTick()
    Dim RtRange As Double

    Public Property AllowMoveRange As Boolean = True

    Dim TIC_time As DoubleRange
    Dim TIC_max As Double

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        ' DoubleBuffered = True

        ' SetStyle(ControlStyles.UserPaint, True)
        ' SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        ' SetStyle(ControlStyles.DoubleBuffer, True)
    End Sub

    Public Sub SetTIC(data As ChromatogramTick())
        TIC = data
        RtRange = data.Last.Time - data.First.Time
        TIC_time = TIC.Select(Function(x) x.Time).Range
        TIC_max = TIC.Select(Function(x) x.Intensity).Max

        Call Me.Invalidate()
        Call RefreshRtRangeSelector()
    End Sub

    Private Sub RtRangeSelector_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        If e.Button <> MouseButtons.Left Then
            Return
        End If

        If e.X > start AndAlso e.X < endPox Then
            onMoveRange = AllowMoveRange
            lastX = e.X
        Else
            start = e.X
            onSelect = True
        End If
    End Sub

    Private Sub RtRangeSelector_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        If e.Button <> MouseButtons.Left Then
            Return
        End If

        endPox = e.X
        onSelect = False
        onMoveRange = False

        Dim length As Double = Width

        With {start, endPox}
            rtmin = .Min / length
            rtmax = .Max / length

            rtmin = TIC(Scan0).Time + rtmin * RtRange
            rtmax = TIC(Scan0).Time + rtmax * RtRange

            RaiseEvent RangeSelect(rtmin, rtmax)

            start = .Min
            endPox = .Max
        End With
    End Sub

    Private Sub RtRangeSelector_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        rtX = e.X

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
        Timer1.Interval = 100
        Timer1.Enabled = True
        Timer1.Start()

        ResizeRedraw = True
        DoubleBuffered = False
    End Sub

    Private Sub DrawTIC(g As Graphics)
        If TIC.IsNullOrEmpty Then
            Return
        End If

        Using TICcurve As New GraphicsPath
            Dim height As Double = Me.Height
            Dim width As Double = Me.Width
            Dim scaleX = d3js.scale.linear.domain(values:={TIC_time.Min, TIC_time.Max}).range(New Double() {0, width})
            Dim scaleY = d3js.scale.linear.domain(values:={0.0, TIC_max}).range(New Double() {0, height})
            Dim i As i32 = Scan0
            Dim x1, x2 As Single
            Dim y1, y2 As Single

            Call TICcurve.AddLine(0, CSng(height), CSng(scaleX(TIC(i).Time)), CSng(height - scaleY(TIC(i).Intensity)))

            For j As Integer = 1 To TIC.Length - 1
                x1 = CSng(scaleX(TIC(i).Time))
                y1 = CSng(height - scaleY(TIC(++i).Intensity))
                x2 = CSng(scaleX(TIC(j).Time))
                y2 = CSng(height - scaleY(TIC(j).Intensity))

                Call TICcurve.AddLine(x1, y1, x2, y2)
            Next

            Call TICcurve.AddLine(x2, y2, x2, CSng(height))

            Call TICcurve.CloseAllFigures()

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

            If onSelect OrElse AllowMoveRange Then
                Call g.FillRectangle(New SolidBrush(SelectedColor.Alpha(125)), New RectangleF(left, 0, right - left, Height))
            End If
        End Using

        Call updatelabel(left, right)
    End Sub

    Sub updatelabel(left As Integer, right As Integer)
        If TIC.IsNullOrEmpty Then
            Return
        End If

        ' update label
        Dim length As Double = Width

        rtmin = left / length
        rtmax = right / length

        rtmin = TIC(Scan0).Time + rtmin * RtRange
        rtmax = TIC(Scan0).Time + rtmax * RtRange

        Dim curTime = rtX / length
        curTime = TIC(Scan0).Time + curTime * RtRange

        Label1.Text = $"Time: {curTime.ToString("F2")} sec; time range selected [{rtmin.ToString("F2")}, {rtmax.ToString("F2")}]"
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If onSelect OrElse onMoveRange Then
            Call RefreshSelector()
        End If

        With {start, endPox}
            Call updatelabel(.Min, .Max)
        End With
    End Sub

    ''' <summary>
    ''' redraw
    ''' </summary>
    Public Sub RefreshRtRangeSelector() Handles Me.Resize
        Me.Invalidate()
    End Sub

    Private Sub RtRangeSelector_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint

    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        Call RefreshSelector()
        'Dim width = Me.Width
        'Dim height = Me.Height

        'Using g As Graphics = e.Graphics
        '    If width > 0 AndAlso height > 0 Then
        '        Call g.FillRectangle(New SolidBrush(BackColor), New RectangleF(0, 0, width, height))
        '        Call DrawTIC(g)
        '    End If
        'End Using
    End Sub

    'Protected Overrides Sub OnValidated(e As EventArgs)
    '    MyBase.OnValidated(e)

    '    Call RefreshSelector()
    'End Sub
End Class
