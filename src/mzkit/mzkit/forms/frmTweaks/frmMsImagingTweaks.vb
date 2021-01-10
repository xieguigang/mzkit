Public Class frmMsImagingTweaks

    Private Sub frmMsImagingTweaks_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.TabText = "MsImage Parameters"

        Call ApplyVsTheme(ContextMenuStrip1, ToolStrip1)
    End Sub

    Private Sub ClearSelectionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearSelectionToolStripMenuItem.Click

    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click

    End Sub
End Class