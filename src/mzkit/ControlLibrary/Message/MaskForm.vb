Public Class MaskForm

    Sub New(point As Point, size As Size)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Opacity = 0.5
        BackColor = Color.LightGray
        FormBorderStyle = FormBorderStyle.None
        StartPosition = FormStartPosition.Manual

        Dim dx As Integer = 5
        Dim dy As Integer = 5

        Me.Location = New Point(point.X + dx, point.Y)
        Me.Size = New Size(size.Width - dx * 2, size.Height - dy)
    End Sub

    Public Function ShowDialogForm(dialog As Form) As DialogResult
        Me.Show()
        Dim result = dialog.ShowDialog
        Me.Close()
        Return result
    End Function

End Class