Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports RibbonLib.Interop
Imports Task

Public Class PageMoleculeNetworking

    Dim g As NetworkGraph
    Dim host As frmMain
    Dim rawMatrix As EntityClusterModel()
    Dim nodeInfo As Dictionary(Of String, ScanEntry)

    Public Sub loadNetwork(MN As IEnumerable(Of EntityClusterModel), nodes As Dictionary(Of String, ScanEntry))
        DataGridView1.Rows.Clear()
        DataGridView2.Rows.Clear()

        ' g = TreeGraph(Of PeakMs2, PeakMs2).CreateGraph(MN.getRoot, Function(a) a.lib_guid, Function(a) $"M{CInt(a.mz)}T{CInt(a.rt)}")
        '.doRandomLayout _
        '.doForceLayout(iterations:=100)
        g = New NetworkGraph
        rawMatrix = MN.ToArray
        nodeInfo = nodes

        For Each row In rawMatrix
            Dim info = nodeInfo(row.ID)
            g.CreateNode(row.ID, New NodeData With {
                .Properties = New Dictionary(Of String, String) From {
                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, row.Cluster},
                    {"scan", info.id},
                    {"m/z", info.mz},
                    {"rt", info.rt},
                    {"intensity", info.intensity},
                    {"polarity", info.polarity},
                    {"charge", info.charge}
                }})
        Next

        For Each row In rawMatrix
            For Each link In row.Properties
                If g.GetElementByID(link.Key) Is Nothing Then
                    g.CreateNode(link.Key)
                End If

                g.CreateEdge(row.ID, link.Key, link.Value)
            Next
        Next

        For Each node In g.vertex
            Dim info = nodeInfo(node.label)

            DataGridView2.Rows.Add(node.label, node.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE), info.id, info.mz, info.rt, info.intensity, info.polarity, info.charge)
        Next
        For Each edge In g.graphEdges
            DataGridView1.Rows.Add(edge.U.label, edge.V.label, edge.weight, "View")
        Next

        ' PictureBox1.BackgroundImage = g.DrawImage(labelerIterations:=-1).AsGDIImage
    End Sub

    Public Sub saveNetwork()
        If Not g Is Nothing Then
            Using file As New FolderBrowserDialog With {.ShowNewFolderButton = True}
                If file.ShowDialog = DialogResult.OK Then
                    Call g.Tabular({"scan", "m/z", "rt", "intensity", "polarity", "charge"}).Save(output:=file.SelectedPath)
                    Call Process.Start(file.SelectedPath)
                End If
            End Using
        Else
            MessageBox.Show("No network graph object is found! Please goto raw file viewer page and select a raw file to run [Molecule Networking] analysis!", "No network graph object!", MessageBoxButtons.OK, MessageBoxIcon.Warning)
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

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        If e.ColumnIndex = 3 AndAlso e.RowIndex > -1 Then
            Dim row = DataGridView1.Rows(e.RowIndex)
            Dim a = CStr(row.Cells(0).Value)
            Dim b = CStr(row.Cells(1).Value)

            a = nodeInfo(a).id
            b = nodeInfo(b).id


        End If
    End Sub
End Class
