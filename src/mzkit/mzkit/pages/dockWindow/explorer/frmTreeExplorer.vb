Imports BioNovoGene.Analytical.MassSpectrometry.Assembly

Public Class frmTreeExplorer

    Dim tree As SpectrumTree

    Private Sub frmTreeExplorer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.TabText = "Spectrum Tree Explorer"
    End Sub

    Public Sub LoadTree(tree As SpectrumTree)
        Me.tree = tree
    End Sub

    Private Sub TreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TreeView1.AfterSelect

    End Sub
End Class