Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports mzkit.My

Public Class frmSRMIonsExplorer

    Public Sub LoadMRM(file As String)
        Call CheckedListBox1.Items.Clear()

        For Each chr As chromatogram In file.LoadChromatogramList
            CheckedListBox1.Items.Add(chr)
        Next
    End Sub

    Private Sub CheckedListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CheckedListBox1.SelectedIndexChanged
        If Not CheckedListBox1.SelectedItem Is Nothing Then
            Dim chr As chromatogram = CheckedListBox1.SelectedItem
            Dim ticks As ChromatogramTick() = chr.Ticks

            Call MyApplication.host.mzkitTool.ShowMRMTIC(chr.id, ticks)
        End If
    End Sub

    Private Sub ShowTICOverlapToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowTICOverlapToolStripMenuItem.Click

    End Sub
End Class