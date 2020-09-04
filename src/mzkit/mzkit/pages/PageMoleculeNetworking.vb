#Region "Microsoft.VisualBasic::66ceca9709033a9152046cf3f7bf9059, src\mzkit\mzkit\pages\PageMoleculeNetworking.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
' 
' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' MIT License
' 
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.



' /********************************************************************************/

' Summaries:

' Class PageMoleculeNetworking
' 
'     Sub: DataGridView1_CellContentClick, loadNetwork, PageMoleculeNetworking_Load, PageMoleculeNetworking_VisibleChanged, SaveImageToolStripMenuItem_Click
'          saveNetwork
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports mzkit.My
Imports RibbonLib.Interop
Imports Task

Public Class PageMoleculeNetworking

    Dim g As NetworkGraph
    Dim rawMatrix As EntityClusterModel()
    Dim nodeInfo As Protocols

    Public Sub loadNetwork(MN As IEnumerable(Of EntityClusterModel), nodes As Protocols, cutoff As Double)
        DataGridView1.Rows.Clear()
        DataGridView2.Rows.Clear()

        ' g = TreeGraph(Of PeakMs2, PeakMs2).CreateGraph(MN.getRoot, Function(a) a.lib_guid, Function(a) $"M{CInt(a.mz)}T{CInt(a.rt)}")
        '.doRandomLayout _
        '.doForceLayout(iterations:=100)
        g = New NetworkGraph
        rawMatrix = MN.ToArray
        nodeInfo = nodes

        For Each row In rawMatrix
            Dim info As NetworkingNode = nodeInfo.Cluster(row.ID)
            Dim rt As Double() = info.members.Select(Function(a) a.rt).ToArray

            g.CreateNode(row.ID, New NodeData With {
                .Properties = New Dictionary(Of String, String) From {
                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, row.Cluster},
                    {"member_size", info.members.Length},
                    {"m/z", info.mz},
                    {"rt", $"[{rt.Min.ToString("F3")}, {rt.Max.ToString("F3")}]"}
                }})
        Next

        Dim duplicatedEdges As New Index(Of String)
        Dim uniqueKey As String

        For Each row In rawMatrix
            For Each link In row.Properties.Where(Function(l) l.Value >= cutoff AndAlso l.Key <> row.ID)
                uniqueKey = {row.ID, link.Key}.OrderBy(Function(str) str).JoinBy(" vs ")

                If Not uniqueKey Like duplicatedEdges Then
                    Call duplicatedEdges.Add(uniqueKey)
                    Call g.CreateEdge(row.ID, link.Key, link.Value)
                End If
            Next
        Next

        For Each node In g.vertex
            Dim info = nodeInfo.Cluster(node.label)

            DataGridView2.Rows.Add(node.label, node.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE), info.members.Length, info.mz, node.data("rt"))
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
            MyApplication.host.ribbonItems.TabGroupNetworkTools.ContextAvailable = ContextAvailability.Active
        Else
            MyApplication.host.ribbonItems.TabGroupNetworkTools.ContextAvailable = ContextAvailability.NotAvailable
        End If
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        If e.ColumnIndex = 3 AndAlso e.RowIndex > -1 Then
            Dim row = DataGridView1.Rows(e.RowIndex)
            Dim a = CStr(row.Cells(0).Value)
            Dim b = CStr(row.Cells(1).Value)
            Dim host = MyApplication.host

            If a Is Nothing OrElse b Is Nothing Then
                Return
            End If

            Dim nodeA = nodeInfo.Cluster(a)
            Dim nodeB = nodeInfo.Cluster(b)
            Dim matrix As SSM2MatrixFragment() = GlobalAlignment.CreateAlignment(nodeA.representation.ms2, nodeB.representation.ms2, Tolerance.DeltaMass(0.3)).ToArray

            host.mzkitTool.showMatrix(matrix, $"{row.Cells(0).Value}_vs_{row.Cells(1).Value}")

            host.mzkitTool.PictureBox1.BackgroundImage = MassSpectra.AlignMirrorPlot(nodeA.representation, nodeB.representation).AsGDIImage
            host.mzkitTool.CustomTabControl1.SelectedTab = host.mzkitTool.TabPage5

            host.ShowPage(host.mzkitTool)
        End If
    End Sub
End Class

