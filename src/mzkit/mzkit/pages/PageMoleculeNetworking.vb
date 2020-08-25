Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Imaging

Public Class PageMoleculeNetworking

    Dim g As NetworkGraph

    Public Sub loadNetwork(MN As SpectrumTreeCluster)
        DataGridView1.Rows.Clear()
        DataGridView2.Rows.Clear()
        PictureBox1.BackgroundImage = Nothing

        g = TreeGraph(Of PeakMs2, PeakMs2) _
            .CreateGraph(MN.getRoot, Function(a) a.lib_guid) _
            .doRandomLayout _
            .doForceLayout

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
            If Not g Is Nothing Then
                PictureBox1.BackgroundImage = g.DrawImage.AsGDIImage
            End If
        End If
    End Sub
End Class
