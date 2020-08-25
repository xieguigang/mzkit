Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic

Public Class PageMoleculeNetworking

    Public Sub loadNetwork(MN As SpectrumTreeCluster)
        DataGridView1.Rows.Clear()
        DataGridView2.Rows.Clear()
        PictureBox1.BackgroundImage = Nothing

        Dim g = TreeGraph(Of PeakMs2, PeakMs2).CreateGraph(MN.getRoot, Function(a) a.lib_guid)

        For Each node In g.vertex
            DataGridView2.Rows.Add(node.label, node.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE))
        Next
        For Each edge In g.graphEdges
            DataGridView1.Rows.Add(edge.U.label, edge.V.label, edge.weight)
        Next
    End Sub

    Private Sub PageMoleculeNetworking_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub

    Private Sub TabControl1_TabIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.TabIndexChanged
        If TabControl1.SelectedTab Is TabPage3 AndAlso PictureBox1.BackgroundImage Is Nothing Then
            ' render network image

        End If
    End Sub
End Class
