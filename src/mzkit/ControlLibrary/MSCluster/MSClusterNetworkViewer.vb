Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Public Class MSClusterNetworkViewer

    Public Sub LoadModel(graph As NetworkGraph)
        Canvas1.Graph = graph
    End Sub

    Private Sub PhysicalEngineToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PhysicalEngineToolStripMenuItem.Click
        If PhysicalEngineToolStripMenuItem.Checked Then
            ' turn on engine
            PhysicalEngineToolStripMenuItem.Text = "Physical Engine (On)"
        Else
            ' turn off engine
            PhysicalEngineToolStripMenuItem.Text = "Physical Engine (Off)"
        End If

        Canvas1.SetPhysical(PhysicalEngineToolStripMenuItem.Checked)
    End Sub
End Class
