Public Class frmSpectrumSearch

    Friend page As New PageSpectrumSearch With {.Text = "Spectrum Similarity Search"}

    Private Sub frmSpectrumSearch_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(page)

        page.Dock = DockStyle.Fill
        TabText = "Spectrum Similarity Search"
        Text = TabText
    End Sub
End Class