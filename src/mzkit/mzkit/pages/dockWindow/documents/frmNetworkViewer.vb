Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.SpringForce

Public Class frmNetworkViewer

    Private Sub frmNetworkViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Text = "Network Canvas"
        TabText = "Network Canvas"

        Call ApplyVsTheme(ContextMenuStrip1)
    End Sub

    Public Sub SetGraph(g As NetworkGraph, layout As ForceDirectedArgs)
        If layout Is Nothing Then
            layout = ForceDirectedArgs.DefaultNew
        End If

        Canvas1.Graph() = g
        Canvas1.SetFDGParams(layout)
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

    Private Sub ConfigLayoutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConfigLayoutToolStripMenuItem.Click
        Call InputDialog.Input(Of InputNetworkLayout)(
            Sub(config)

            End Sub)
    End Sub

    Private Sub ShowLabelsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowLabelsToolStripMenuItem.Click
        Canvas1.ShowLabel = ShowLabelsToolStripMenuItem.Checked
    End Sub
End Class