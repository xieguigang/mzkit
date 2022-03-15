Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Public Class frmSMILESViewer

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim smilesStr As String = Strings.Trim(TextBox1.Text)
        Dim graph = ParseChain.ParseGraph(smilesStr)
        Dim network As NetworkGraph
    End Sub
End Class