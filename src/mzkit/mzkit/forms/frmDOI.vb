Public Class frmDOI
    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process.Start("https://bio.tools/mzkit")
    End Sub

    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        Process.Start("https://mzkit.org")
    End Sub

    Private Sub frmDOI_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim file As String = $"{App.HOME}/Reference.rtf"

        If Not file.FileExists Then
            file = $"{App.HOME}/../../src/mzkit/Reference.rtf"
        End If

        RichTextBox1.Clear()
        RichTextBox1.LoadFile(file, RichTextBoxStreamType.RichText)
    End Sub

    Private Sub RichTextBox1_TextChanged(sender As Object, e As EventArgs) Handles RichTextBox1.TextChanged

    End Sub
End Class