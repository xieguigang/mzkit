Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Public Class frmSMILESViewer

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim smilesStr As String = Strings.Trim(TextBox1.Text)
        Dim graph = ParseChain.ParseGraph(smilesStr)
        Dim network As New NetworkGraph

        For Each v In graph.vertex
            network.CreateNode(v.label, New NodeData With {.label = v.elementName, .color = Brushes.Black})
        Next
        For Each l In graph.graphEdges
            Dim url = network.CreateEdge(
                  u:=l.U.label,
                  v:=l.V.label,
                  weight:=l.bond
              )

            url.data.style = New Pen(Color.Red, 2)
            network.AddEdge(url)
        Next

        Canvas1.Graph = network
    End Sub
End Class