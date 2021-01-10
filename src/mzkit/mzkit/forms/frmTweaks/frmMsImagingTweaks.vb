Public Class frmMsImagingTweaks

    Dim checkedMz As New List(Of TreeNode)

    Public Iterator Function GetSelectedIons() As IEnumerable(Of Double)
        If checkedMz.Count > 0 Then
            For Each node In checkedMz
                Yield DirectCast(node.Tag, Double)
            Next
        Else
            If Not Win7StyleTreeView1.SelectedNode Is Nothing Then
                If Win7StyleTreeView1.SelectedNode.Tag Is Nothing Then
                    For Each node As TreeNode In Win7StyleTreeView1.SelectedNode.Nodes
                        Yield DirectCast(node.Tag, Double)
                    Next
                Else
                    Yield DirectCast(Win7StyleTreeView1.SelectedNode.Tag, Double)
                End If
            End If
        End If
    End Function

    Private Sub frmMsImagingTweaks_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.TabText = "MsImage Parameters"

        Call ApplyVsTheme(ContextMenuStrip1, ToolStrip1)
    End Sub

    Private Sub ClearSelectionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearSelectionToolStripMenuItem.Click
        For Each item In checkedMz
            item.Checked = False
        Next

        checkedMz.Clear()
    End Sub

    Private Sub Win7StyleTreeView1_AfterCheck(sender As Object, e As TreeViewEventArgs) Handles Win7StyleTreeView1.AfterCheck
        If e.Node.Checked Then
            If e.Node.Tag Is Nothing Then
                For Each mz As TreeNode In e.Node.Nodes
                    mz.Checked = True
                    checkedMz.Add(mz)
                Next
            Else
                checkedMz.Add(e.Node)
            End If
        Else
            If e.Node.Tag Is Nothing Then
                For Each mz As TreeNode In e.Node.Nodes
                    mz.Checked = False
                    checkedMz.Remove(mz)
                Next
            Else
                checkedMz.Remove(e.Node)
            End If
        End If
    End Sub
End Class