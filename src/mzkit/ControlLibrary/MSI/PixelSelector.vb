Imports stdNum = System.Math

Public Class PixelSelector

    Dim ScanSize As Size

    Private m_OriginalBitmap As Bitmap
    Private m_CurrentBitmap As Bitmap
    Private m_TempBitmap As Bitmap
    Private m_Gr As Graphics
    Private m_Pen As Pen

    Private m_SelectingArea As Boolean
    Private m_X1 As Integer
    Private m_Y1 As Integer



    Public Sub SetImage(img As Image, scanSize As Size)
        Me.picCanvas.BackgroundImage = img
        Me.ScanSize = scanSize

        Call PixelSelectorResize()
    End Sub

    Public Function GetSelectedPixel() As Point


    End Function

    Private Sub PixelSelectorResize() Handles Me.Resize


    End Sub

    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles picCanvas.MouseDown
        ' Make sure we have a picture loaded.
        If m_OriginalBitmap Is Nothing Then Exit Sub

        m_SelectingArea = True
        m_X1 = e.X
        m_Y1 = e.Y

        ' Make a copy of the current bitmap 
        'and prepare to draw.
        m_TempBitmap = New Bitmap(m_CurrentBitmap)
        m_Gr = Graphics.FromImage(m_TempBitmap)
        m_Pen = New Pen(Color.Yellow)
        m_Pen.DashStyle = Drawing2D.DashStyle.Dash
    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles picCanvas.MouseMove
        If Not m_SelectingArea Then Return

        ' Start with the current image.
        m_Gr.DrawImage(m_CurrentBitmap, 0, 0)

        ' Draw the new selection box.
        m_Gr.DrawRectangle(m_Pen,
           stdNum.Min(m_X1, e.X),
           stdNum.Min(m_Y1, e.Y),
           stdNum.Abs(e.X - m_X1),
           stdNum.Abs(e.Y - m_Y1))

        ' Display the result.
        picCanvas.Image = m_TempBitmap
    End Sub

    Private Sub picCanvas_MouseUp(sender As Object, e As MouseEventArgs) Handles picCanvas.MouseUp
        If Not m_SelectingArea Then Exit Sub
        m_SelectingArea = False

        ' Make sure this point is on the picture.
        Dim x As Integer = e.X
        If x < 0 Then x = 0
        If x > m_OriginalBitmap.Width - 1 Then x =
            m_OriginalBitmap.Width - 1

        Dim y As Integer = e.Y
        If y < 0 Then y = 0
        If y > m_OriginalBitmap.Height - 1 Then y =
            m_OriginalBitmap.Height - 1

        ' Pixelate the selected area.
        PixelateArea(
              stdNum.Min(m_X1, x),
             stdNum.Min(m_Y1, y),
              stdNum.Abs(x - m_X1),
              stdNum.Abs(y - m_Y1))

        ' We're done drawing for now.
        m_Pen.Dispose()
        m_Gr.Dispose()
        m_TempBitmap.Dispose()

        m_Pen = Nothing
        m_Gr = Nothing
        m_TempBitmap = Nothing
    End Sub

    ''' <summary>
    ''' Pixelate the area.
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <param name="wid"></param>
    ''' <param name="hgt"></param>
    ''' <remarks>
    ''' http://www.vb-helper.com/howto_net_image_pixelator.html
    ''' </remarks>
    Private Sub PixelateArea(ByVal x As Integer, ByVal y As _
        Integer, ByVal wid As Integer, ByVal hgt As Integer)
        Const cell_wid As Integer = 20
        Const cell_hgt As Integer = 10

        ' Start with the current image.
        m_Gr.DrawImage(m_CurrentBitmap, 0, 0)

        ' Make x and y multiples of cell_wid/cell_hgt
        ' from the origin.
        Dim new_x As Integer = cell_wid * Int(x \ cell_wid)
        Dim new_y As Integer = cell_hgt * Int(y \ cell_hgt)

        ' Pixelate the selected area.
        For x1 As Integer = new_x To x + wid Step cell_wid
            For y1 As Integer = new_y To y + hgt Step cell_hgt
                AverageRectangle(x1, y1, cell_wid, cell_hgt)
            Next y1
        Next x1

        ' Set the current bitmap to the result.
        m_CurrentBitmap = New Bitmap(m_TempBitmap)

        ' Display the result.
        picCanvas.Image = m_CurrentBitmap
    End Sub

    ' Fill this rectangle with the average of its pixel values.
    Private Sub AverageRectangle(ByVal x As Integer, ByVal y As _
        Integer, ByVal wid As Integer, ByVal hgt As Integer)
        ' Make sure we don't exceed the image's bounds.
        If x < 0 Then x = 0
        If x + wid >= m_OriginalBitmap.Width Then
            wid = m_OriginalBitmap.Width - x - 1
        End If
        If wid <= 0 Then Exit Sub

        If y < 0 Then y = 0
        If y + hgt >= m_OriginalBitmap.Height Then
            hgt = m_OriginalBitmap.Height - y - 1
        End If
        If hgt <= 0 Then Exit Sub

        ' Get the total red, green, and blue values.
        Dim clr As Color
        Dim r As Integer
        Dim g As Integer
        Dim b As Integer
        For i As Integer = 0 To hgt - 1
            For j As Integer = 0 To wid - 1
                clr = m_CurrentBitmap.GetPixel(x + j, y + i)
                r += clr.R
                g += clr.G
                b += clr.B
            Next j
        Next i

        ' Calculate the averages.
        r \= wid * hgt
        g \= wid * hgt
        b \= wid * hgt

        ' Set the pixel values.
        Dim ave_brush As New SolidBrush(Color.FromArgb(255, r,
            g, b))
        m_Gr.FillRectangle(ave_brush, x, y, wid, hgt)
        ave_brush.Dispose()
    End Sub
End Class
