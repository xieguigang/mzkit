Public Class frmSpectrumSearch

    Private Sub frmSpectrumSearch_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim page As New PageSpectrumSearch With {.Text = "Spectrum Similarity Search"}

        Controls.Add(page)

        page.Dock = DockStyle.Fill
        TabText = "Spectrum Similarity Search"
        Text = TabText
    End Sub
End Class