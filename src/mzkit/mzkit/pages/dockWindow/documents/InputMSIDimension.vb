Public Class InputMSIDimension

    Public ReadOnly Property Dims As String

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text.StringEmpty OrElse TextBox2.Text.StringEmpty Then
            MessageBox.Show("Invalid size!", "Mzkit Win32", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        _Dims = $"{TextBox1.Text},{TextBox2.Text}"

        Me.DialogResult = DialogResult.OK
    End Sub
End Class