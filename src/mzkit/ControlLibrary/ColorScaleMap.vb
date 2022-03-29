#Region "Microsoft.VisualBasic::e15a15680a9489f611cd698b705fc683, mzkit\src\mzkit\ControlLibrary\MSI\ColorScaleMap.vb"

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

'   Total Lines: 110
'    Code Lines: 90
' Comment Lines: 3
'   Blank Lines: 17
'     File Size: 3.09 KB


' Class ColorScaleMap
' 
'     Properties: colorMap, mapLevels, range
' 
'     Sub: colorMapDisplay_Paint, ColorScaleMap_Load, refreshSlideBar, refreshTrigger_Tick, SlideBar_MouseDown
'          SlideBar_MouseMove, SlideBar_MouseUp
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports stdNum = System.Math

Public Class ColorScaleMap

    Public Property range As Double()
        Get
            Return _range
        End Get
        Set(value As Double())
            If value.IsNullOrEmpty OrElse value.Length <> 2 Then
                Return
            End If

            _range = value
            Call refreshSlideBar()

            Label1.Text = value(0).ToString("G3")
            Label2.Text = value(1).ToString("G3")
        End Set
    End Property

    Public Property mapLevels As Integer
        Get
            Return _maplevels
        End Get
        Set(value As Integer)
            If value <= 0 Then
                Return
            End If

            _maplevels = value
            Call refreshSlideBar()
        End Set
    End Property

    Public Property colorMap As ScalerPalette
        Get
            Return _mapName
        End Get
        Set
            If mapLevels <= 0 Then
                Return
            End If

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

    Dim _maplevels As Integer = 30
    Dim _range As Double() = {0, 1}
    Dim _colorMap As SolidBrush()
    Dim _mapName As ScalerPalette

    Private Sub SlideBar_MouseMove(sender As Object, e As MouseEventArgs)
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

    Private Sub SlideBar_MouseDown(sender As Object, e As MouseEventArgs)
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

    Private Sub SlideBar_MouseUp(sender As Object, e As MouseEventArgs)
        drag = False
    End Sub

    Private Sub refreshSlideBar()
        Call Me.Refresh()
        Call colorMapDisplay.Refresh()
    End Sub

    Private Sub ColorScaleMap_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub

    'Private Sub refreshTrigger_Tick(sender As Object, e As EventArgs) Handles refreshTrigger.Tick
    '    Call SlideBar.Invalidate()
    'End Sub

    Private Sub colorMapDisplay_Paint(sender As Object, e As PaintEventArgs) Handles colorMapDisplay.Paint
        'If SlideBar Is Nothing OrElse _colorMap.IsNullOrEmpty Then
        '    Return
        'End If

        If _colorMap.IsNullOrEmpty Then
            Return
        End If

        Dim width As Integer = Me.Width
        Dim dw As Integer = width / _maplevels
        Dim h As Integer = colorMapDisplay.Height  ' Height - SlideBar.Height
        Dim g As Graphics = e.Graphics
        Dim x As Integer

        For Each col As SolidBrush In _colorMap
            g.FillRectangle(col, New Rectangle(x, 0, dw, h))
            x += dw
        Next
    End Sub
End Class
