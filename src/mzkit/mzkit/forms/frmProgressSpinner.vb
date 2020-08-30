Public Class frmProgressSpinner

    Dim theImage As Image = My.Resources.spinner

    Sub RunAnimation()
        ' the image Is set up for animation using the
        ' ImageAnimator class And an event handler
        ImageAnimator.Animate(theImage, New EventHandler(AddressOf OnFrameChanged))
    End Sub

    Private Sub OnFrameChanged(o As Object, e As EventArgs)
        Me.Invalidate()
    End Sub

    Private Sub frmProgressSpinner_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        'Get the next frame ready for rendering for the image in this thread.
        ImageAnimator.UpdateFrames()
        Dim g = e.Graphics
        'Draw the current frame in the animation for all of the images

        g.DrawImage(theImage, 0, 0, theImage.Width, theImage.Height)
    End Sub

    Private Sub frmProgressSpinner_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' SetStyle(ControlStyles.SupportsTransparentBackColor, True)
        Me.TransparencyKey = Color.Green
        Me.BackColor = Color.Green

        RunAnimation()
    End Sub
End Class