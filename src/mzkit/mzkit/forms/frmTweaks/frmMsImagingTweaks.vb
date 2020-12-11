Public Class frmMsImagingTweaks

    Private Sub frmMsImagingTweaks_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.TabText = "MsImage Parameters"
    End Sub

    Private Sub ClearSelectionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearSelectionToolStripMenuItem.Click
        Call CheckedListBox1.SelectedItems.Clear()
        Call CheckedListBox1.SelectedIndices.Clear()
    End Sub
End Class