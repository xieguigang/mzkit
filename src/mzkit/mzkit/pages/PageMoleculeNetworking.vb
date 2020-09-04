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

Imports System.Threading
Imports System.Windows.Forms.ListViewItem
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports mzkit.cooldatagridview
Imports mzkit.My
Imports RibbonLib.Interop
Imports WeifenLuo.WinFormsUI.Docking

Public Class PageMoleculeNetworking

    Dim g As NetworkGraph
    Dim rawMatrix As EntityClusterModel()
    Dim nodeInfo As Protocols
    Dim rawLinks As Dictionary(Of String, Dictionary(Of String, (id$, forward#, reverse#)))
    Dim tooltip As New PlotTooltip

    Public Sub RenderNetwork()
        If g Is Nothing Then
            MyApplication.host.showStatusMessage("You should run molecular networking at first!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        ElseIf g.vertex.Count > 500 OrElse g.graphEdges.Count > 700 Then
            MyApplication.host.showStatusMessage("The network size is huge for create layout, entire progress will be very slow...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        End If

        Dim progress As New frmTaskProgress
        Dim viewer As New frmPlotViewer
        Dim showSingle As Boolean = False
        Dim graph As NetworkGraph = g.Copy

        If Not showSingle Then
            Dim links = g.connectedNodes.ToList

            For Each node In g.vertex.ToArray
                If links.IndexOf(node) = -1 Then
                    g.RemoveNode(node)
                End If
            Next
        End If

        viewer.Show(MyApplication.host.dockPanel)
        viewer.DockState = DockState.Hidden

        Dim task As New Thread(
            Sub()
                Thread.Sleep(500)
                progress.Invoke(Sub() progress.Label1.Text = "Run network layouts...")

                g = g.doRandomLayout.doForceLayout(iterations:=1)
                progress.Invoke(Sub() progress.Label1.Text = "do network render plot...")

                Dim plot As Image = g.DrawImage(
                    canvasSize:="1920,1080",
                    labelerIterations:=-1
                ).AsGDIImage

                viewer.Invoke(Sub()
                                  viewer.PictureBox1.BackgroundImage = plot
                                  viewer.DockState = DockState.Document
                              End Sub)

                progress.Invoke(Sub() progress.Close())
            End Sub)

        task.Start()
        progress.ShowDialog()
    End Sub

    Public Sub loadNetwork(MN As IEnumerable(Of EntityClusterModel),
                           nodes As Protocols,
                           rawLinks As Dictionary(Of String, Dictionary(Of String, (id$, forward#, reverse#))),
                           cutoff As Double)

        DataGridView1.Rows.Clear()
        ' DataGridView2.Rows.Clear()
        TreeListView1.Items.Clear()

        ' g = TreeGraph(Of PeakMs2, PeakMs2).CreateGraph(MN.getRoot, Function(a) a.lib_guid, Function(a) $"M{CInt(a.mz)}T{CInt(a.rt)}")
        '.doRandomLayout _
        '.doForceLayout(iterations:=100)
        g = New NetworkGraph
        rawMatrix = MN.ToArray
        nodeInfo = nodes
        tooltip.LoadInfo(nodeInfo)

        Me.rawLinks = rawLinks

        For Each row In rawMatrix
            Dim info As NetworkingNode = nodeInfo.Cluster(row.ID)
            Dim rt As Double() = info.members.Select(Function(a) a.rt).ToArray
            Dim maxrt As Double = info.members.OrderByDescending(Function(a) a.Ms2Intensity).First.rt

            g.CreateNode(row.ID, New NodeData With {
                .Properties = New Dictionary(Of String, String) From {
                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, row.Cluster},
                    {"member_size", info.members.Length},
                    {"m/z", info.mz},
                    {"rt", maxrt},
                    {"rtmin", rt.Min},
                    {"rtmax", rt.Max},
                    {"area", info.members.Sum(Function(a) a.Ms2Intensity)}
                }})
        Next

        Dim duplicatedEdges As New Index(Of String)
        Dim uniqueKey As String

        For Each row In rawMatrix
            Dim rawLink = rawLinks(row.ID)

            For Each link In row.Properties.Where(Function(l) l.Value >= cutoff AndAlso l.Key <> row.ID)
                uniqueKey = {row.ID, link.Key}.OrderBy(Function(str) str).JoinBy(" vs ")

                If Not uniqueKey Like duplicatedEdges Then
                    Call duplicatedEdges.Add(uniqueKey)
                    Call g.CreateEdge(row.ID, link.Key, link.Value, New EdgeData With {.Properties = New Dictionary(Of String, String) From {
                                      {"forward", rawLink.TryGetValue(link.Key).forward},
                                      {"reverse", rawLink.TryGetValue(link.Key).reverse}
                                 }})
                End If
            Next
        Next

        For Each node In g.vertex
            Dim info = nodeInfo.Cluster(node.label)

            ' DataGridView2.Rows.Add(node.label, node.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE), info.members.Length, info.mz, node.data("rt"), node.data("rtmin"), node.data("rtmax"), node.data("area"))

            Dim row As New TreeListViewItem With {.Text = node.label, .ImageIndex = 0, .ToolTipText = node.label}

            For Each member In info.members
                Dim ion As New TreeListViewItem(member.lib_guid) With {.ImageIndex = 1, .ToolTipText = member.lib_guid}

                ion.SubItems.Add(New ListViewSubItem With {.Text = member.file})
                ion.SubItems.Add(New ListViewSubItem With {.Text = member.mzInto.Length})
                ion.SubItems.Add(New ListViewSubItem With {.Text = member.mz})
                ion.SubItems.Add(New ListViewSubItem With {.Text = member.rt})
                ion.SubItems.Add(New ListViewSubItem With {.Text = "n/a"})
                ion.SubItems.Add(New ListViewSubItem With {.Text = "n/a"})
                ion.SubItems.Add(New ListViewSubItem With {.Text = member.Ms2Intensity})

                row.Items.Add(ion)
            Next

            row.SubItems.Add(New ListViewSubItem With {.Text = node.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)})
            row.SubItems.Add(New ListViewSubItem With {.Text = info.members.Length})
            row.SubItems.Add(New ListViewSubItem With {.Text = info.mz})
            row.SubItems.Add(New ListViewSubItem With {.Text = node.data("rt")})
            row.SubItems.Add(New ListViewSubItem With {.Text = node.data("rtmin")})
            row.SubItems.Add(New ListViewSubItem With {.Text = node.data("rtmax")})
            row.SubItems.Add(New ListViewSubItem With {.Text = node.data("area")})

            TreeListView1.Items.Add(row)
        Next
        For Each edge In g.graphEdges
            DataGridView1.Rows.Add(edge.U.label, edge.V.label, edge.data!forward, edge.data!reverse, "View")
        Next

        ' PictureBox1.BackgroundImage = g.DrawImage(labelerIterations:=-1).AsGDIImage
    End Sub

    Public Sub saveNetwork()
        If Not g Is Nothing Then
            Using file As New FolderBrowserDialog With {.ShowNewFolderButton = True}
                If file.ShowDialog = DialogResult.OK Then
                    Call g.Tabular({"member_size", "m/z", "rt", "rtmin", "rtmax", "area"}).Save(output:=file.SelectedPath)
                    Call Process.Start(file.SelectedPath)
                End If
            End Using
        Else
            MessageBox.Show("No network graph object is found! Please goto raw file viewer page and select a raw file to run [Molecule Networking] analysis!", "No network graph object!", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub SaveImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveImageToolStripMenuItem.Click
        Dim cluster As TreeListViewItem
        Dim host = MyApplication.host

        If TreeListView1.SelectedItems.Count = 0 Then
            Return
        Else
            cluster = TreeListView1.SelectedItems(0)
        End If

        If cluster.ChildrenCount > 0 Then
            ' 是一个cluster
            Dim clusterId As String = cluster.Text
            Dim clusterSpectrum = nodeInfo.Cluster(clusterId).representation

            host.mzkitTool.showMatrix(clusterSpectrum.ms2, clusterId)
            host.mzkitTool.PictureBox1.BackgroundImage = MassSpectra.MirrorPlot(clusterSpectrum).AsGDIImage
        Else
            ' 是一个spectrum
            Dim spectrumName As String = cluster.Text
            Dim spectrum = nodeInfo.GetSpectrum(spectrumName)

            host.mzkitTool.showMatrix(spectrum.mzInto, spectrumName)
            host.mzkitTool.PictureBox1.BackgroundImage = MassSpectra.MirrorPlot(New LibraryMatrix With {.ms2 = spectrum.mzInto, .name = spectrumName}).AsGDIImage
        End If

        host.mzkitTool.CustomTabControl1.SelectedTab = host.mzkitTool.TabPage5
        host.ShowPage(host.mzkitTool)
    End Sub

    Private Sub PageMoleculeNetworking_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        If Visible Then
            MyApplication.host.ribbonItems.TabGroupNetworkTools.ContextAvailable = ContextAvailability.Active
        Else
            MyApplication.host.ribbonItems.TabGroupNetworkTools.ContextAvailable = ContextAvailability.NotAvailable
        End If
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        If e.ColumnIndex = 4 AndAlso e.RowIndex > -1 Then
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

    Private Sub PageMoleculeNetworking_Load(sender As Object, e As EventArgs) Handles Me.Load
        DataGridView1.CoolGrid
        ' DataGridView2.CoolGrid
        tooltip.OwnerDraw = True
    End Sub

    Private Sub TreeListView1_Click(sender As Object, e As EventArgs) Handles TreeListView1.Click

    End Sub

    Private Sub TreeListView1_DoubleClick(sender As Object, e As EventArgs) Handles TreeListView1.DoubleClick

    End Sub

    Private Sub TreeListView1_MouseMove(sender As Object, e As MouseEventArgs) Handles TreeListView1.MouseMove

    End Sub
End Class

