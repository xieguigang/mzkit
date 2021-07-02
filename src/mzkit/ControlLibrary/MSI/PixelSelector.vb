#Region "Microsoft.VisualBasic::6a3dd245529ef0e1201a382ee4c68ac9, src\mzkit\ControlLibrary\MSI\PixelSelector.vb"

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
    '     Properties: MSImage
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: canvasMouseDown, DrawSelectionBox, getPoint, picCanvas_MouseClick, picCanvas_MouseMove
    '          picCanvas_MouseUp, ShowMessage
    ' 
    ' /********************************************************************************/

#End Region

Public Class PixelSelector

    Public Event SelectPixel(x As Integer, y As Integer)
    Public Event SelectPixelRegion(x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer)

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
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

    Sub canvasMouseDown(sender As Object, e As MouseEventArgs) Handles picCanvas.MouseDown
        If e.Button <> MouseButtons.Left Then
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

    ' Draw the area selected.
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

            RaiseEvent SelectPixelRegion(rangeStart.X, rangeStart.Y, rangeEnd.X, rangeEnd.Y)
        End If
    End Sub

    Private Sub picCanvas_MouseClick(sender As Object, e As MouseEventArgs) Handles picCanvas.MouseClick
        If e.Button <> MouseButtons.Left Then
            Return
        Else
            drawing = False
        End If

        Dim xpoint = 0
        Dim ypoint = 0

        Call getPoint(e, xpoint, ypoint)

        RaiseEvent SelectPixel(xpoint, ypoint)
    End Sub
End Class

