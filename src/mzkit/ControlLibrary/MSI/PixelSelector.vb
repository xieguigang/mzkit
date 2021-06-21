Public Class PixelSelector

    Public Property MSImage As Image
        Get
            Return picCanvas.BackgroundImage
        End Get
        Set(value As Image)
            picCanvas.BackgroundImage = value
        End Set
    End Property

    Public Sub ShowMessage(text As String)
        ToolStripStatusLabel1.Text = text
    End Sub
End Class
