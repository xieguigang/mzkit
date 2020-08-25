Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Imaging
Imports RibbonLib.Interop

Public Class PageMoleculeNetworking

    Dim g As NetworkGraph
    Dim host As frmMain

    Public Sub loadNetwork(MN As SpectrumTreeCluster)
        DataGridView1.Rows.Clear()
        DataGridView2.Rows.Clear()

        g = TreeGraph(Of PeakMs2, PeakMs2).CreateGraph(MN.getRoot, Function(a) a.lib_guid, Function(a) $"M{CInt(a.mz)}T{CInt(a.rt)}")
        '.doRandomLayout _
        '.doForceLayout(iterations:=100)

        For Each node In g.vertex
            DataGridView2.Rows.Add(node.label, node.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE))
        Next
        For Each edge In g.graphEdges
            DataGridView1.Rows.Add(edge.U.label, edge.V.label, edge.weight)
        Next

        ' PictureBox1.BackgroundImage = g.DrawImage(labelerIterations:=-1).AsGDIImage
    End Sub

    Public Sub saveNetwork()
        If Not g Is Nothing Then
            Using file As New FolderBrowserDialog With {.ShowNewFolderButton = True}
                If file.ShowDialog = DialogResult.OK Then
                    Call g.Tabular.Save(output:=file.SelectedPath)
                End If
            End Using
        End If
    End Sub

    Private Sub SaveImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveImageToolStripMenuItem.Click
        'If Not PictureBox1.BackgroundImage Is Nothing Then
        '    Using file As New SaveFileDialog With {.Filter = "image(*.png)|*.png"}
        '        If file.ShowDialog = DialogResult.OK Then
        '            Call PictureBox1.BackgroundImage.SaveAs(file.FileName)
        '        End If
        '    End Using
        'End If
    End Sub

    Private Sub PageMoleculeNetworking_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        If Visible Then
            host.ribbonItems.TabGroupNetworkTools.ContextAvailable = ContextAvailability.Active
        Else
            host.ribbonItems.TabGroupNetworkTools.ContextAvailable = ContextAvailability.NotAvailable
        End If
    End Sub

    Private Sub PageMoleculeNetworking_Load(sender As Object, e As EventArgs) Handles Me.Load
        host = DirectCast(ParentForm, frmMain)
    End Sub
End Class
