Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language

Public Class PageSpectrumSearch

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub DataGridView1_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellEndEdit
        ' do previews
        Dim ms2 As New List(Of ms2)

        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            ms2 += New ms2 With {
                .mz = Val(DataGridView1.Rows(i).Cells(0).Value),
                .intensity = Val(DataGridView1.Rows(i).Cells(1).Value),
                .quantity = .intensity
            }
        Next

        PictureBox1.BackgroundImage = New LibraryMatrix With {
            .centroid = True,
            .ms2 = ms2.ToArray,
            .name = "custom spectrum"
        }.MirrorPlot _
         .AsGDIImage
    End Sub

    Private Sub PageSpectrumSearch_Load(sender As Object, e As EventArgs) Handles Me.Load
        PictureBox1.BackgroundImageLayout = ImageLayout.Zoom
    End Sub
End Class
