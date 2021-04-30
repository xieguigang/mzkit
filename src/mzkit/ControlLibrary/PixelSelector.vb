Public Class PixelSelector

    Dim ScanSize As Size
    Dim renderRect As Rectangle

    Public Sub SetImage(img As Image, scanSize As Size)
        Me.PictureBox1.BackgroundImage = img
        Me.ScanSize = scanSize

        Call PixelSelectorResize()
    End Sub

    Public Function GetSelectedPixel() As Point
        Dim mouse = PointToClient(MousePosition)

    End Function

    Private Sub PixelSelectorResize() Handles Me.Resize
        If ScanSize.IsEmpty Then
            Return
        End If

        Dim sizeCtl As Size = PictureBox1.Size
        Dim ratio1 As Double = sizeCtl.Width / sizeCtl.Height
        Dim ratio2 As Double = ScanSize.Width / ScanSize.Height


    End Sub
End Class
