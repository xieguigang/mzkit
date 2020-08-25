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
            .doForceLayout(iterations:=100)

        For Each node In g.vertex
            DataGridView2.Rows.Add(node.label, node.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE))
        Next
        For Each edge In g.graphEdges
            DataGridView1.Rows.Add(edge.U.label, edge.V.label, edge.weight)
        Next

        PictureBox1.BackgroundImage = g.DrawImage(labelerIterations:=-1).AsGDIImage
    End Sub

    Private Sub SaveImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveImageToolStripMenuItem.Click
        If Not PictureBox1.BackgroundImage Is Nothing Then
            Using file As New SaveFileDialog With {.Filter = "image(*.png)|*.png"}
                If file.ShowDialog = DialogResult.OK Then
                    Call PictureBox1.BackgroundImage.SaveAs(file.FileName)
                End If
            End Using
        End If
    End Sub
End Class
