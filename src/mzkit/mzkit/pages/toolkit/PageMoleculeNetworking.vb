#Region "Microsoft.VisualBasic::0d5250942a956247ab7c1c459936e338, mzkit\src\mzkit\mzkit\pages\toolkit\PageMoleculeNetworking.vb"

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


    ' Code Statistics:

    '   Total Lines: 326
    '    Code Lines: 262
    ' Comment Lines: 7
    '   Blank Lines: 57
    '     File Size: 16.10 KB


    ' Class PageMoleculeNetworking
    ' 
    '     Sub: DataGridView1_CellContentClick, loadNetwork, PageMoleculeNetworking_Load, PageMoleculeNetworking_VisibleChanged, RefreshNetwork
    '          RenderNetwork, SaveImageToolStripMenuItem_Click, saveNetwork
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Threading
Imports System.Windows.Forms.ListViewItem
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.MoleculeNetworking
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports BioNovoGene.mzkit_win32.cooldatagridview
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.ForceDirected
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.SpringForce
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports RibbonLib.Interop
Imports WeifenLuo.WinFormsUI.Docking
Imports stdNum = System.Math

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

        Dim viewer As frmNetworkViewer = VisualStudio.ShowDocument(Of frmNetworkViewer)(title:="Molecular Networking Viewer")
        Dim showSingle As Boolean = False
        Dim graph As NetworkGraph = g.Copy

        If Not showSingle Then
            Dim links = graph.connectedNodes.ToList

            For Each node In graph.vertex.ToArray
                If links.IndexOf(node) = -1 Then
                    graph.RemoveNode(node)
                End If
            Next
        End If

        Call graph.ComputeNodeDegrees

        Dim minRadius As Single = Globals.Settings.network.nodeRadius.min
        Dim degreeRange As New DoubleRange(graph.vertex.Select(Function(a) CDbl(a.degree.In + a.degree.Out)).ToArray)
        Dim similarityRange As New DoubleRange(graph.graphEdges.Select(Function(a) a.weight).ToArray)
        Dim nodeRadiusRange As DoubleRange = Globals.Settings.network.nodeRadius.AsDoubleRange
        Dim linkWidthRange As DoubleRange = Globals.Settings.network.linkWidth.AsDoubleRange
        Dim nodeRadius As Func(Of Graph.Node, Single) = Function(v) degreeRange.ScaleMapping(v.degree.In + v.degree.Out, nodeRadiusRange)
        Dim linkWidth As Func(Of Graph.Edge, Single) = Function(l) similarityRange.ScaleMapping(l.weight, linkWidthRange)
        Dim nodeClusters = graph.vertex.Select(Function(a) a.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)).Distinct.Indexing
        Dim colorSet As SolidBrush() = Designer.GetColors("Paper", nodeClusters.Count, alpha:=120).Select(Function(a) New SolidBrush(a)).ToArray
        Dim cancel As Value(Of Boolean) = False

        For Each v In graph.vertex
            v.data.color = colorSet(nodeClusters.IndexOf(v.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)))
            v.data.size = New Double() {nodeRadius(v)}
        Next

        Dim linkColor As Color = Color.FromArgb(161, 168, 172)

        For Each l In graph.graphEdges
            l.data.style = New Pen(linkColor, linkWidth(l))
        Next

        Call viewer.SetGraph(graph, layout:=Globals.Settings.network.layout)
        Call viewer.Show(MyApplication.host.dockPanel)
    End Sub

    Public Sub RefreshNetwork()
        If rawMatrix.IsNullOrEmpty Then
            MyApplication.host.showStatusMessage("no network graph data!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        Dim similarityCutoff As Double = MyApplication.host.ribbonItems.SpinnerSimilarity.DecimalValue
        Dim buzy As New frmProgressSpinner

        Call New Thread(Sub()
                            Call Thread.Sleep(500)
                            Call Me.Invoke(Sub() loadNetwork(rawMatrix, nodeInfo, rawLinks, similarityCutoff))
                            Call buzy.Invoke(Sub() buzy.Close())

                            MyApplication.host.showStatusMessage($"Refresh network with new similarity filter {similarityCutoff} success!")
                        End Sub).Start()
        Call buzy.ShowDialog()
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

        Dim colors As LoopArray(Of String) = Designer.GetColors("Set1:c9", 10).Select(AddressOf ToHtmlColor).AsLoop
        Dim colorIndex As New Dictionary(Of String, String)

        Me.rawLinks = rawLinks

        For Each row In rawMatrix
            Dim info As NetworkingNode = nodeInfo.Cluster(row.ID)
            Dim rt As Double() = info.members.Select(Function(a) a.rt).ToArray
            Dim maxrt As Double = info.members.OrderByDescending(Function(a) a.Ms2Intensity).First.rt
            Dim color As String = colorIndex.ComputeIfAbsent(row.Cluster, Function(cl) colors.Next)

            g.CreateNode(row.ID, New NodeData With {
                .Properties = New Dictionary(Of String, String) From {
                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, row.Cluster},
                    {"member_size", info.members.Length},
                    {"m/z", info.mz},
                    {"rt", maxrt},
                    {"rtmin", rt.Min},
                    {"rtmax", rt.Max},
                    {"area", info.members.Sum(Function(a) a.Ms2Intensity)},
                    {"color", color}
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

        If g.graphEdges.Count >= 8000 AndAlso MessageBox.Show("There are two many edges in your network, do you wan to increase the similarity threshold for reduce network size?", "To many edges", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            MyApplication.host.ribbonItems.SpinnerSimilarity.DecimalValue = 0.98
            Call RefreshNetwork()
            Return
        End If

        Call g.ComputeNodeDegrees
        Call g.ComputeBetweennessCentrality

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
            DataGridView1.Rows.Add(
                edge.U.label,
                edge.V.label,
                stdNum.Min(Val(edge.data!forward), Val(edge.data!reverse)).ToString("F4"),
                Val(edge.data!forward).ToString("F4"),
                Val(edge.data!reverse).ToString("F4"),
                "View Alignment"
            )
            Application.DoEvents()
        Next

        DataGridView2.Rows.Clear()
        DataGridView2.Rows.Add("nodes", g.vertex.Count)
        DataGridView2.Rows.Add("edges", g.graphEdges.Count)
        DataGridView2.Rows.Add("single nodes", g.vertex.Count - g.connectedNodes.Length)
        DataGridView2.Rows.Add("similarity_threshold", cutoff)

        For Each cluster In rawMatrix.GroupBy(Function(a) a.Cluster).OrderBy(Function(a) Val(a.Key))
            DataGridView2.Rows.Add($"#Cluster_{cluster.Key}", cluster.Count)
        Next
    End Sub

    Public Sub saveNetwork()
        If Not g Is Nothing Then
            Using file As New FolderBrowserDialog With {.ShowNewFolderButton = True}
                If file.ShowDialog = DialogResult.OK Then
                    Dim meta As New Dictionary(Of String, String)

                    For i As Integer = 0 To DataGridView2.Rows.Count - 1
                        Dim key As String = Scripting.ToString(DataGridView2.Rows(i).Cells(0).Value)
                        Dim val As String = Scripting.ToString(DataGridView2.Rows(i).Cells(1).Value)

                        meta(key) = val
                    Next

                    Call g.Tabular(
                        propertyNames:={"member_size", "m/z", "rt", "rtmin", "rtmax", "area", "forward", "reverse", "color"},
                        creators:={My.User.Name},
                        title:="Molecular Networking",
                        description:="Molecular Networking Model",
                        keywords:={"spectrum"},
                        links:={"http://mzkit.org"},
                        meta:=meta
                    ).Save(output:=file.SelectedPath)
                    Call Process.Start(file.SelectedPath)
                End If
            End Using
        Else
            MessageBox.Show("No network graph Object Is found! Please GoTo raw file viewer page And Select a raw file To run [Molecule Networking] analysis!", "No network graph object!", MessageBoxButtons.OK, MessageBoxIcon.Warning)
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
        If e.ColumnIndex = 5 AndAlso e.RowIndex > -1 Then
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
            Dim alignment As New AlignmentOutput With {.alignments = matrix}

            host.mzkitTool.showMatrix(matrix, $"{row.Cells(0).Value}_vs_{row.Cells(1).Value}")

            host.mzkitTool.PictureBox1.BackgroundImage = MassSpectra.AlignMirrorPlot(nodeA.representation, nodeB.representation).AsGDIImage
            host.mzkitTool.CustomTabControl1.SelectedTab = host.mzkitTool.TabPage5

            host.ShowPage(host.mzkitTool)
        End If
    End Sub

    Private Sub PageMoleculeNetworking_Load(sender As Object, e As EventArgs) Handles Me.Load
        DataGridView1.CoolGrid
        DataGridView2.CoolGrid
        tooltip.OwnerDraw = True
    End Sub
End Class
