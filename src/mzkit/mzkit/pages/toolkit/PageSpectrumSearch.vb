Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports mzkit.My

Public Class PageSpectrumSearch

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub DataGridView1_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellEndEdit
        Call refreshPreviews()
    End Sub

    Private Sub refreshPreviews()
        ' do previews
        Dim ms2 As New List(Of ms2)

        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            ms2 += New ms2 With {
                .mz = Val(DataGridView1.Rows(i).Cells(0).Value),
                .intensity = Val(DataGridView1.Rows(i).Cells(1).Value),
                .quantity = .intensity
            }
        Next

        If ms2.All(Function(mz) mz.intensity = 0) OrElse ms2.All(Function(mz) mz.mz = 0) Then
            MyApplication.host.showStatusMessage("all of the mass spectrum fragment their intensity or product m/z is ZERO!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        PictureBox1.BackgroundImage = New LibraryMatrix With {
            .centroid = True,
            .ms2 = ms2.Where(Function(a) a.mz > 0).ToArray,
            .name = "custom spectrum"
        }.MirrorPlot _
         .AsGDIImage
    End Sub

    Private Sub PageSpectrumSearch_Load(sender As Object, e As EventArgs) Handles Me.Load
        PictureBox1.BackgroundImageLayout = ImageLayout.Zoom
    End Sub

    Private Sub PageSpectrumSearch_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp, DataGridView1.KeyUp, PictureBox1.KeyUp, TabPage1.KeyUp

    End Sub

    Private Sub TabPage1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown, DataGridView1.KeyDown, PictureBox1.KeyDown, TabPage1.KeyDown
        If e.KeyCode = Keys.V AndAlso e.Control AndAlso Clipboard.ContainsText Then
            Call loadFromMgfIon()
        End If
    End Sub

    Private Sub loadFromMgfIon()
        Dim textLines As String() = Clipboard.GetText.LineTokens

        If textLines.IsNullOrEmpty Then
            Return
        End If

        Dim ion As MGF.Ions = MGF.MgfReader.StreamParser(textLines).FirstOrDefault

        If ion Is Nothing OrElse ion.Peaks.IsNullOrEmpty Then
            Call MyApplication.host.showStatusMessage("invalid mgf text format!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            DataGridView1.Rows.Clear()

            For Each ms2 As ms2 In ion.Peaks
                DataGridView1.Rows.Add(ms2.mz, ms2.intensity)
            Next

            Call refreshPreviews()
        End If
    End Sub

    Private Sub PasteMgfTextToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PasteMgfTextToolStripMenuItem.Click
        If Clipboard.ContainsText OrElse Clipboard.GetText.StringEmpty Then
            Call loadFromMgfIon()
        Else
            Call MyApplication.host.showStatusMessage("no content data in your clipboard...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        End If
    End Sub

    Private Sub SavePreviewPlotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SavePreviewPlotToolStripMenuItem.Click
        If PictureBox1.BackgroundImage Is Nothing Then
            Call MyApplication.host.showStatusMessage("no plot image for save...", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        Using file As New SaveFileDialog With {.Filter = "plot image(*.png)|*.png"}
            If file.ShowDialog = DialogResult.OK Then
                Call PictureBox1.BackgroundImage.SaveAs(file.FileName)
            End If
        End Using
    End Sub
End Class
