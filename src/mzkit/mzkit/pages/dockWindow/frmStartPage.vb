Public Class frmStartPage

    Dim startPage As New PageStart

    Private Sub frmStartPage_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(startPage)
        startPage.Dock = DockStyle.Fill
    End Sub
End Class