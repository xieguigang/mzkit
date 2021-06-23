Public Class PixelSelector
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Dim orginal_image As Size
    Dim dimension As Size

    Public Property MSImage(Optional dimsize As Size = Nothing) As Image
        Get
            Return picCanvas.BackgroundImage
        End Get
        Set(value As Image)
            picCanvas.BackgroundImage = value
            dimension = dimsize

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

    Public Sub ShowMessage(text As String)
        ToolStripStatusLabel1.Text = text
    End Sub

    Dim drawing As Boolean = False
    Dim startPoint, endPoint As Point

    Sub canvasMouseDown(sender As Object, e As MouseEventArgs) Handles picCanvas.MouseDown
        drawing = True
        startPoint = e.Location
        DrawSelectionBox(startPoint)
    End Sub

    Private Sub picCanvas_MouseMove(sender As Object, e As MouseEventArgs) Handles picCanvas.MouseMove
        If Not picCanvas.BackgroundImage Is Nothing Then
            Dim Pic_width = orginal_image.Width / picCanvas.Width
            Dim Pic_height = orginal_image.Height / picCanvas.Height
            ' 得到图片上的坐标点
            Dim xpoint As Integer = e.X * Pic_width
            Dim ypoint As Integer = e.Y * Pic_height

            ToolStripStatusLabel2.Text = $"[{xpoint}, {ypoint}]"
        End If

        If Not drawing Then
            Return
        End If

        DrawSelectionBox(e.Location)
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
            drawing = False
        End If
    End Sub
End Class
