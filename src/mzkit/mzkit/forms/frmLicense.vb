Public Class frmLicense

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Private Sub frmLicense_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox1.Text = My.Resources.LICENSE
    End Sub
End Class