#Region "Microsoft.VisualBasic::0aac3e1a21f7a789e7ccea9bf2c88cd4, src\mzkit\ControlLibrary\MSI\PixelSelector.vb"

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

' Class PixelSelector
' 
'     Properties: MSImage, Pixel
' 
'     Constructor: (+1 Overloads) Sub New
'     Sub: canvasMouseDown, DrawSelectionBox, getPoint, picCanvas_MouseClick, picCanvas_MouseMove
'          picCanvas_MouseUp, ShowMessage
' 
' /********************************************************************************/

#End Region

Imports stdNum = System.Math

Public Class PixelSelector

    Public Event SelectPixel(x As Integer, y As Integer)
    Public Event SelectPixelRegion(region As Rectangle)

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        picCanvas.BackgroundImageLayout = ImageLayout.Stretch
    End Sub

    Dim orginal_image As Size
    Dim dimension As Size

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pixel_size"></param>
    ''' <returns></returns>
    Public Property MSImage(Optional pixel_size As Size = Nothing) As Image
        Get
            Return picCanvas.BackgroundImage
        End Get
        Set(value As Image)
            picCanvas.BackgroundImage = value
            dimension = pixel_size

            If value IsNot Nothing AndAlso (dimension.Width = 0 OrElse dimension.Height = 0) Then
                Throw New InvalidExpressionException("dimension size can not be ZERO!")
            End If

            If value Is Nothing Then
                orginal_image = Nothing
            Else
                orginal_image = value.Size
                orginal_image = New Size With {
                    .Width = orginal_image.Width / dimension.Width,
                    .Height = orginal_image.Height / dimension.Height
                }
            End If
        End Set
    End Property

    Dim oldMessage As String = "MSI Viewer"

    Public Sub ShowMessage(text As String)
        ToolStripStatusLabel1.Text = text
    End Sub

    Dim drawing As Boolean = False
    Dim startPoint, endPoint As Point
    Dim rangeStart As Point
    Dim rangeEnd As Point

    Public ReadOnly Property HasRegionSelection As Boolean
        Get
            Return startPoint.IsEmpty OrElse endPoint.IsEmpty
        End Get
    End Property

    Public ReadOnly Property RegionSelectin As Rectangle
        Get
            Dim left As Integer = stdNum.Min(startPoint.X, endPoint.X)
            Dim top As Integer = stdNum.Min(startPoint.Y, endPoint.Y)
            Dim right As Integer = stdNum.Max(startPoint.X, endPoint.X)
            Dim bottom As Integer = stdNum.Max(startPoint.Y, endPoint.Y)

            Return New Rectangle(left, top, right - left, bottom - top)
        End Get
    End Property

    Public Sub ClearSelection()
        startPoint = Nothing
        endPoint = Nothing
    End Sub

    Sub canvasMouseDown(sender As Object, e As MouseEventArgs) Handles picCanvas.MouseDown
        If e.Button <> MouseButtons.Left Then
            If e.Button = MouseButtons.Right Then
                drawing = False
            End If

            Return
        End If

        Dim xpoint = 0
        Dim ypoint = 0

        drawing = True
        startPoint = e.Location

        getPoint(e, xpoint, ypoint)
        DrawSelectionBox(startPoint)

        rangeStart = New Point(xpoint, ypoint)

        oldMessage = ToolStripStatusLabel1.Text
        ShowMessage("Select Pixels By Range.")
    End Sub

    Private Sub picCanvas_MouseMove(sender As Object, e As MouseEventArgs) Handles picCanvas.MouseMove
        If Not picCanvas.BackgroundImage Is Nothing Then
            Dim xpoint = 0
            Dim ypoint = 0

            getPoint(e, xpoint, ypoint)
            ToolStripStatusLabel2.Text = $"[{xpoint}, {ypoint}]"
        End If

        If Not drawing Then
            Return
        End If

        DrawSelectionBox(e.Location)
    End Sub

    Private Sub getPoint(e As MouseEventArgs, ByRef xpoint As Integer, ByRef ypoint As Integer)
        Dim Pic_width = orginal_image.Width / picCanvas.Width
        Dim Pic_height = orginal_image.Height / picCanvas.Height

        ' 得到图片上的坐标点
        xpoint = e.X * Pic_width
        ypoint = e.Y * Pic_height
    End Sub

    ''' <summary>
    ''' Draw the area selected.
    ''' </summary>
    ''' <param name="end_point"></param>
    Private Sub DrawSelectionBox(end_point As Point)
        ' Save the end point.
        endPoint = end_point

        If (endPoint.X < 0) Then endPoint.X = 0
        If (endPoint.X >= picCanvas.Width) Then endPoint.X = picCanvas.Width - 1
        If (endPoint.Y < 0) Then endPoint.Y = 0
        If (endPoint.Y >= picCanvas.Height) Then endPoint.Y = picCanvas.Height - 1

        ' Draw the selection area.
        Dim x = Math.Min(startPoint.X, endPoint.X)
        Dim y = Math.Min(startPoint.Y, endPoint.Y)
        Dim width = Math.Abs(startPoint.X - endPoint.X)
        Dim height = Math.Abs(startPoint.Y - endPoint.Y)

        picCanvas.CreateGraphics.DrawRectangle(Pens.Red, x, y, width, height)
        picCanvas.Refresh()
    End Sub

    Private Sub picCanvas_MouseUp(sender As Object, e As MouseEventArgs) Handles picCanvas.MouseUp
        If Not drawing Then
            Return
        Else
            Dim xpoint = 0
            Dim ypoint = 0

            getPoint(e, xpoint, ypoint)
            ShowMessage(oldMessage)

            rangeEnd = New Point(xpoint, ypoint)
            drawing = False

            RaiseEvent SelectPixelRegion(RegionSelectin)
        End If
    End Sub

    Public ReadOnly Property Pixel As Point

    Private Sub picCanvas_MouseClick(sender As Object, e As MouseEventArgs) Handles picCanvas.MouseClick
        Dim xpoint = 0
        Dim ypoint = 0

        Call getPoint(e, xpoint, ypoint)

        _Pixel = New Point(xpoint, ypoint)

        If e.Button <> MouseButtons.Left Then
            Return
        Else
            drawing = False
            RaiseEvent SelectPixel(xpoint, ypoint)
        End If
    End Sub

    Private Sub PixelSelector_Load(sender As Object, e As EventArgs) Handles Me.Load
        Timer1.Enabled = True
        Timer1.Start()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If HasRegionSelection Then
            DrawSelectionBox(endPoint)
        End If
    End Sub
End Class
