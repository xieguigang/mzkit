Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Public Class frmSMILESViewer

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim smilesStr As String = Strings.Trim(TextBox1.Text)
        Dim graph = ParseChain.ParseGraph(smilesStr)
        Dim network As New NetworkGraph

        For Each v In graph.vertex
            network.CreateNode($"{v.ID}-{v.elementName}", New NodeData With {.label = v.elementName})
        Next
        For Each l In graph.graphEdges
            network.CreateEdge(
                u:=$"{l.U.ID}-{l.U.elementName}",
                v:=$"{l.V.ID}-{l.V.elementName}",
                weight:=l.bond
            )
        Next

        Canvas1.Graph = network
    End Sub
End Class