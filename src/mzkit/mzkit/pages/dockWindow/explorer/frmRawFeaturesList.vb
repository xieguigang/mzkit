#Region "Microsoft.VisualBasic::85534e62a2794ba293e87f31c0464e0d, mzkit\src\mzkit\mzkit\pages\dockWindow\explorer\frmRawFeaturesList.vb"

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

    '   Total Lines: 735
    '    Code Lines: 565
    ' Comment Lines: 31
    '   Blank Lines: 139
    '     File Size: 31.62 KB


    ' Class frmRawFeaturesList
    ' 
    '     Properties: CurrentRawFile
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: checkIon, GetSelectedNodes, GetXICCollection
    ' 
    '     Sub: Button1_Click, Clear, ClearSelectionsToolStripMenuItem_Click, CollapseToolStripMenuItem_Click, CopyIonsToolStripMenuItem_Click
    '          CustomToolStripMenuItem_Click, DefaultToolStripMenuItem_Click, DeleteFileToolStripMenuItem_Click, exportMgf, ExportMzPackToolStripMenuItem_Click
    '          frmFileExplorer_Activated, frmFileExplorer_Closing, frmFileExplorer_Load, GeneralFlavoneToolStripMenuItem_Click, IonScansToolStripMenuItem_Click
    '          IonSearchToolStripMenuItem_Click, IonTableToolStripMenuItem_Click, loadInternal, LoadRaw, LoadXICIons
    '          MetaDNASearchToolStripMenuItem_Click, MolecularNetworkingToolStripMenuItem_Click, NatureProductToolStripMenuItem_Click, OpenViewerToolStripMenuItem_Click, SearchFormulaToolStripMenuItem_Click
    '          SelectAllToolStripMenuItem_Click, ShowBPCToolStripMenuItem_Click, ShowPropertiesToolStripMenuItem_Click, ShowTICToolStripMenuItem_Click, ShowXICToolStripMenuItem_Click
    '          SmallMoleculeToolStripMenuItem_Click, SpectrumSearchToolStripMenuItem_Click, TextBox2_Click, ToolStripButton2_Click, ToolStripButton3_Click
    '          ToolStripButton4_Click, ToolStripButton5_Click, TreeView1_AfterCheck, treeView1_AfterSelect, treeView1_DragDrop
    '          treeView1_DragEnter, XICToolStripMenuItem_Click, XICViewToolStripMenuItem_Click
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports BioNovoGene.mzkit_win32.My
Imports RibbonLib.Interop
Imports Task
Imports WeifenLuo.WinFormsUI.Docking
Imports stdNum = System.Math

Public Class frmRawFeaturesList

    Public ReadOnly Property CurrentRawFile As Raw

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        DoubleBuffered = True
    End Sub

    Private Sub frmFileExplorer_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        MyApplication.host.ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active
    End Sub

    Private Sub frmFileExplorer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub frmFileExplorer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(treeView1)

        ContextMenuStrip1.RenderMode = ToolStripRenderMode.System

        treeView1.HotTracking = True
        treeView1.CheckBoxes = True
        treeView1.BringToFront()
        treeView1.ContextMenuStrip = ContextMenuStrip1
        treeView1.ShowLines = True
        treeView1.ShowRootLines = True
        treeView1.Dock = DockStyle.Fill
        treeView1.AllowDrop = True

        Me.TabText = "Features Explorer"

        Call ApplyVsTheme(ContextMenuStrip1, ToolStrip1)
    End Sub

    Dim rtmin, rtmax As Double

    ''' <summary>
    ''' reload
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        If CurrentRawFile Is Nothing Then
            Call MyApplication.host.showStatusMessage("No raw data file was loaded!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Call loadInternal(CurrentRawFile, Double.MinValue, Double.MaxValue)
        End If
    End Sub

    Private Sub loadInternal(raw As Raw, rtmin As Double, rtmax As Double)
        Dim hasUVscans As Boolean = False

        If (Not CurrentRawFile Is Nothing) AndAlso (Not raw Is CurrentRawFile) Then
            CurrentRawFile.UnloadMzpack()
        End If

        Me.rtmin = rtmin
        Me.rtmax = rtmax

        XICViewToolStripMenuItem.Checked = False
        _CurrentRawFile = raw
        treeView1.loadRawFile(raw, hasUVscans, rtmin, rtmax)
        checked.Clear()

        If Not hasUVscans Then
            WindowModules.UVScansList.DockState = DockState.Hidden
        End If
    End Sub

    Public Sub LoadRaw(raw As Raw, Optional rtmin As Double = Double.MinValue, Optional rtmax As Double = Double.MaxValue)
        ' skip of reload identical files
        If raw Is CurrentRawFile AndAlso rtmin = Me.rtmin AndAlso rtmax = Me.rtmax Then
            Return
        Else
            loadInternal(raw, rtmin, rtmax)
        End If
    End Sub

    Public Iterator Function GetXICCollection(ppm As Double) As IEnumerable(Of NamedCollection(Of ChromatogramTick))
        Dim scans = GetSelectedNodes _
            .Where(Function(t) TypeOf t.Tag Is ScanMS2) _
            .Select(Function(a) DirectCast(a.Tag, ScanMS2)) _
            .GroupBy(Function(a) a.parentMz, Tolerance.DeltaMass(0.3)) _
            .ToArray
        Dim raw As Raw = CurrentRawFile

        For Each scanId In scans.Select(Function(a) a.First.scan_id)
            Yield MyApplication.mzkitRawViewer.getXICMatrix(raw, scanId, ppm, relativeInto:=False)
        Next
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    Dim checked As New List(Of TreeNode)

    ''' <summary>
    ''' 不包含 root node
    ''' </summary>
    ''' <returns></returns>
    Public Function GetSelectedNodes() As IEnumerable(Of TreeNode)
        Return checked.AsEnumerable
    End Function

    Dim lockCheckList As Boolean

    Public Sub Clear()
        lockCheckList = True

        For i As Integer = 0 To checked.Count - 1
            checked(i).Checked = False
        Next

        checked.Clear()
        lockCheckList = False
    End Sub

    Private Sub TreeView1_AfterCheck(sender As Object, e As TreeViewEventArgs) Handles treeView1.AfterCheck
        If lockCheckList Then
            Return
        End If

        If TypeOf e.Node.Tag Is ScanMS2 Then
            If e.Node.Checked Then
                checked.Add(e.Node)
            Else
                checked.Remove(e.Node)
            End If
        Else
            Dim checked As Boolean = e.Node.Checked
            Dim node As TreeNode

            ' ms1 批量选择ms2
            ' 或者uv批量选择uvscans

            For i As Integer = 0 To e.Node.Nodes.Count - 1
                node = e.Node.Nodes(i)
                node.Checked = checked

                If checked Then
                    If TypeOf node.Tag Is ScanMS2 Then
                        Me.checked.Add(node)
                    ElseIf TypeOf node.Tag Is UVScan Then

                    End If
                Else
                    If TypeOf node.Tag Is ScanMS2 Then
                        Me.checked.Remove(node)
                    ElseIf TypeOf node.Tag Is UVScan Then

                    End If
                End If
            Next
        End If

        Me.checked = Me.checked.Distinct.AsList

        ClearToolStripMenuItem.Text = $"Clear [{checked.Count} XIC Ions]"
    End Sub

    Private Sub ClearSelectionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearToolStripMenuItem.Click
        lockCheckList = True
        checked.Clear()

        For i As Integer = 0 To treeView1.Nodes.Count - 1
            treeView1.Nodes(i).Checked = False

            If Not treeView1.Nodes(i).Nodes Is Nothing Then
                For j As Integer = 0 To treeView1.Nodes(i).Nodes.Count - 1
                    treeView1.Nodes(i).Nodes(j).Checked = False
                Next
            End If
        Next

        lockCheckList = False
    End Sub

    Private Sub DeleteFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteFileToolStripMenuItem.Click
        Dim fileExplorer = WindowModules.fileExplorer

        If fileExplorer.deleteFileNode(fileExplorer.findRawFileNode(CurrentRawFile), confirmDialog:=True) = DialogResult.Yes Then
            _CurrentRawFile = Nothing
            treeView1.Nodes.Clear()
            checked.Clear()
        End If
    End Sub

    Private Sub SelectAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectAllToolStripMenuItem.Click
        lockCheckList = True
        checked.Clear()

        For i As Integer = 0 To treeView1.Nodes.Count - 1
            treeView1.Nodes(i).Checked = True

            If Not treeView1.Nodes(i).Nodes Is Nothing Then
                For j As Integer = 0 To treeView1.Nodes(i).Nodes.Count - 1
                    treeView1.Nodes(i).Nodes(j).Checked = True
                    checked.Add(treeView1.Nodes(i).Nodes(j))
                Next
            End If
        Next

        lockCheckList = False
    End Sub

    Private Sub treeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles treeView1.AfterSelect
        MyApplication.host.ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active

        If TypeOf e.Node.Tag Is UVScan Then

        ElseIf Not e.Node.Tag Is Nothing Then
            ' scan节点
            Dim raw As Task.Raw = CurrentRawFile
            Dim scanId As String = e.Node.Text

            Call MyApplication.host.mzkitTool.showSpectrum(scanId, raw)
        End If

        Call MyApplication.host.mzkitTool.ShowPage()
        Call WindowModules.fileExplorer.UpdateMainTitle(CurrentRawFile.source)
    End Sub

    Private Sub CollapseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CollapseToolStripMenuItem.Click
        Dim currentNode = treeView1.SelectedNode

        If currentNode Is Nothing Then
            Return
        End If

        If TypeOf currentNode.Tag Is ScanMS2 Then
            currentNode.Parent.Collapse()
        Else
            currentNode.Collapse()
        End If
    End Sub

    Private Sub TextBox2_Click(sender As Object, e As EventArgs) Handles ToolStripSpringTextBox1.Click
        MyApplication.host.showStatusMessage("Input a number for m/z search, or input formula text for precursor ion match!")
    End Sub

    Private Sub ShowTICToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowTICToolStripMenuItem.Click
        If Not CurrentRawFile Is Nothing Then
            Call MyApplication.host.mzkitTool.TIC({CurrentRawFile}, isBPC:=False)
        End If
    End Sub

    Private Sub ShowBPCToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowBPCToolStripMenuItem.Click
        If Not CurrentRawFile Is Nothing Then
            Call MyApplication.host.mzkitTool.TIC({CurrentRawFile}, isBPC:=True)
        End If
    End Sub

    Friend Sub ShowXICToolStripMenuItem_Click() Handles ShowXICToolStripMenuItem.Click
        Dim ions = GetSelectedNodes.ToArray

        If ions.Length >= 500 Then
            If MessageBox.Show("There are too many ions for create XIC plot, probably you should uncheck some ions for reduce data, continute to procedure?", "Too much data!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = DialogResult.Cancel Then
                MyApplication.host.showStatusMessage("Show XIC plot for too many ions has been cancel!")
                Return
            End If
        ElseIf CurrentRawFile Is Nothing Then
            MyApplication.host.showStatusMessage("No raw data file is selected!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        ElseIf treeView1.SelectedNode Is Nothing OrElse treeView1.SelectedNode.Text Is Nothing Then
            MyApplication.host.showStatusMessage("No ion data selected for create XIC plot!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        ' scan节点
        Dim raw As Task.Raw = CurrentRawFile
        Dim ppm As Double = MyApplication.host.GetPPMError()
        Dim plotTIC As NamedCollection(Of ChromatogramTick) = MyApplication.mzkitRawViewer.getXICMatrix(raw, treeView1.SelectedNode.Text, ppm, relativeInto:=False)

        If plotTIC.value.IsNullOrEmpty Then
            ' 当前没有选中MS2，但是可以显示选中的XIC
            If ions.Length > 0 Then
            Else
                MyApplication.host.showStatusMessage("No ion was selected for XIC plot...", My.Resources.StatusAnnotations_Warning_32xLG_color)
                Return
            End If
        Else
            Call MyApplication.mzkitRawViewer.showMatrix(plotTIC.value, Name)
        End If

        Call MyApplication.mzkitRawViewer.ShowXIC(ppm, plotTIC, AddressOf GetXICCollection, raw.GetXICMaxYAxis)
    End Sub

    Private Sub MolecularNetworkingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MolecularNetworkingToolStripMenuItem.Click
        Dim similarityCutoff As Double = MyApplication.host.ribbonItems.SpinnerSimilarity.DecimalValue
        Dim progress As New frmTaskProgress

        progress.ShowProgressTitle("Run molecular networking", directAccess:=True)
        progress.ShowProgressDetails("Initialized...", directAccess:=True)

        Dim runTask As New Thread(Sub() Call MyApplication.mzkitRawViewer.MolecularNetworkingTool(progress, similarityCutoff))

        runTask.Start()
        progress.ShowDialog()
    End Sub

    Private Sub CustomToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CustomToolStripMenuItem.Click
        MyApplication.host.mzkitSearch.ComboBox1.SelectedIndex = 0
        SearchFormulaToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub DefaultToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DefaultToolStripMenuItem.Click
        MyApplication.host.mzkitSearch.ComboBox1.SelectedIndex = 1
        SearchFormulaToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub SmallMoleculeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SmallMoleculeToolStripMenuItem.Click
        MyApplication.host.mzkitSearch.ComboBox1.SelectedIndex = 2
        SearchFormulaToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub NatureProductToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NaturalProductToolStripMenuItem.Click
        MyApplication.host.mzkitSearch.ComboBox1.SelectedIndex = 3
        SearchFormulaToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub GeneralFlavoneToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GeneralFlavoneToolStripMenuItem.Click
        MyApplication.host.mzkitSearch.ComboBox1.SelectedIndex = 4
        SearchFormulaToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub SearchFormulaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SearchFormulaToolStripMenuItem.Click
        Dim node = treeView1.SelectedNode

        If Not node Is Nothing AndAlso CurrentRawFile.cacheFileExists Then
            Dim mz = CurrentRawFile.FindMs2Scan(node.Text)

            If Not mz Is Nothing AndAlso mz.parentMz > 0 Then
                Dim charge As Double = mz.charge
                Dim ionMode As Integer = mz.polarity

                If charge = 0 Then
                    charge = 1
                End If

                MyApplication.host.mzkitSearch.doMzSearch(mz.parentMz, charge, ionMode)
                MyApplication.host.ShowPage(MyApplication.host.mzkitSearch)
            End If
        End If
    End Sub

    Private Sub XICToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles XICToolStripMenuItem.Click
        If GetSelectedNodes.Count = 0 Then
            MessageBox.Show("No chromatogram data for XIC plot, please use XIC -> Add for add data!", "No data save", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            Dim ppm As Double = MyApplication.host.GetPPMError()

            Call exportMgf(
                Iterator Function() As IEnumerable(Of MGF.Ions)

                    For Each xic As NamedCollection(Of ChromatogramTick) In GetXICCollection(ppm)
                        Dim parent As New NamedValue With {.name = xic.description.Split.First, .text = xic.value.Select(Function(a) a.Intensity).Max}
                        Dim ion As New MGF.Ions With {
                            .Title = xic.name,
                            .Peaks = xic.value _
                                .Select(Function(a)
                                            Return New ms2 With {
                                                .mz = a.Time,
                                                .intensity = a.Intensity
                                            }
                                        End Function) _
                                .ToArray,
                            .PepMass = parent,
                            .Rawfile = xic.description.GetTagValue(" ").Value
                        }

                        Yield ion
                    Next

                End Function, "XIC.mgf")
        End If
    End Sub

    Private Sub exportMgf(getIons As Func(Of IEnumerable(Of MGF.Ions)), defaultFileName$)
        Using file As New SaveFileDialog With {.Filter = "Mgf ASCII spectrum data(*.mgf)|*.mgf", .FileName = defaultFileName}
            If file.ShowDialog = DialogResult.OK Then
                Using OutFile As StreamWriter = file.FileName.OpenWriter()
                    For Each ion In getIons()
                        ion.WriteAsciiMgf(OutFile)
                    Next
                End Using
            End If
        End Using
    End Sub

    Private Sub IonScansToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IonScansToolStripMenuItem.Click
        If GetSelectedNodes.Count = 0 Then
            MessageBox.Show("No chromatogram data for XIC plot, please use XIC -> Add for add data!", "No data save", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            Call exportMgf(Iterator Function() As IEnumerable(Of Ions)
                               For Each peak In MyApplication.mzkitRawViewer.getSelectedIonSpectrums(Nothing)
                                   Yield peak.MgfIon
                               Next
                           End Function, "Ions_scan.mgf")
        End If
    End Sub

    Private Sub IonTableToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IonTableToolStripMenuItem.Click
        If GetSelectedNodes.Count = 0 Then
            MessageBox.Show("No chromatogram data for XIC plot, please use XIC -> Add for add data!", "No data save", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            Using file As New SaveFileDialog With {.Filter = "Ion Table(*.csv)|*.csv", .FileName = "Ions_scan.csv"}
                If file.ShowDialog = DialogResult.OK Then
                    Using outFile As StreamWriter = file.FileName.OpenWriter()
                        Dim da03 As Tolerance = DAmethod.DeltaMass(0.3)
                        Dim intocutoff As LowAbundanceTrimming = LowAbundanceTrimming.intoCutff

                        Call outFile.WriteLine($"ID,mz,rt,rt(min),intensity,totalIons,Ion1:into,Ion2:into,Ion3:into,Ion4:into,Ion5:into")

                        For Each peak As PeakMs2 In MyApplication.mzkitRawViewer.getSelectedIonSpectrums(Nothing)
                            Dim id As String = If(CInt(peak.rt) = 0, $"M{CInt(peak.mz)}", $"M{CInt(peak.mz)}T{CInt(peak.rt)}")
                            Dim mz As String = peak.mz.ToString("F4")
                            Dim rt As String = peak.rt.ToString("F2")
                            Dim rtmin As String = (peak.rt / 60).ToString("F1")
                            Dim into As String = peak.intensity
                            Dim TIC As Double = peak.mzInto.Sum(Function(i) i.intensity)
                            Dim maxInto As Double = peak.mzInto.OrderByDescending(Function(i) i.intensity).FirstOrDefault?.intensity
                            Dim ions As String() = peak.mzInto _
                                .Centroid(da03, intocutoff) _
                                .OrderByDescending(Function(m) m.intensity) _
                                .Take(5) _
                                .Select(Function(m) $"{m.mz.ToString("F4")}:{stdNum.Round(100 * m.intensity / maxInto)}") _
                                .ToArray

                            Call outFile.WriteLine({id, mz, rt, rtmin, into, TIC}.JoinIterates(ions).JoinBy(","))
                        Next
                    End Using
                End If
            End Using
        End If
    End Sub

    Private Sub IonSearchToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IonSearchToolStripMenuItem.Click
        Dim currentScan = treeView1.SelectedNode?.Tag

        If currentScan Is Nothing OrElse Not TypeOf currentScan Is ScanMS2 Then
            Return
        End If

        Call FeatureSearchHandler.SearchByMz(DirectCast(currentScan, ScanMS2).parentMz, {CurrentRawFile}, True)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        If CurrentRawFile Is Nothing Then
            Call MyApplication.host.showStatusMessage("please load a raw data file at first!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Call FeatureSearchHandler.SearchByMz(Strings.Trim(ToolStripSpringTextBox1.Text), {CurrentRawFile}, True)
        End If
    End Sub

    Private Sub SpectrumSearchToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SpectrumSearchToolStripMenuItem.Click
        Dim currentScan = treeView1.SelectedNode?.Tag

        If currentScan Is Nothing OrElse Not TypeOf currentScan Is ScanMS2 Then
            Return
        End If

        Dim ms2 As ScanMS2 = currentScan
        Dim searchPage As New frmSpectrumSearch

        searchPage.Show(MyApplication.host.dockPanel)
        searchPage.page.loadMs2(ms2.GetMs)
        searchPage.page.runSearch()
    End Sub

    Private Sub ShowPropertiesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowPropertiesToolStripMenuItem.Click
        MyApplication.host.ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active

        If treeView1.SelectedNode Is Nothing Then
            Call MyApplication.host.showStatusMessage("No ion feature was selected...", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        Call MyApplication.host.ShowPropertyWindow()
        Call MyApplication.host.mzkitTool.ShowPage()

        WindowModules.fileExplorer.UpdateMainTitle(CurrentRawFile.source)
    End Sub

    Private Sub OpenViewerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenViewerToolStripMenuItem.Click
        If Not CurrentRawFile Is Nothing Then
            Call VisualStudio.ShowDocument(Of frmUntargettedViewer)().loadRaw(CurrentRawFile)
        End If
    End Sub

    Private Sub MetaDNASearchToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MetaDNASearchToolStripMenuItem.Click
        If Not CurrentRawFile Is Nothing Then
            Call ConnectToBioDeep.RunMetaDNA(CurrentRawFile)
        End If
    End Sub

    Private Sub ExportMzPackToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportMzPackToolStripMenuItem.Click
        If CurrentRawFile Is Nothing Then
            Return
        End If

        Using file As New SaveFileDialog With {
            .Title = "Export mzPack file!",
            .Filter = "BioNovoGene mzPack(*.mzPack)|*.mzPack"
        }
            If file.ShowDialog = DialogResult.OK Then
                CurrentRawFile.SaveAs(file.FileName)
            End If
        End Using
    End Sub

    ''' <summary>
    ''' 切换为XIC视图
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub XICViewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles XICViewToolStripMenuItem.Click
        If CurrentRawFile Is Nothing Then
            Return
        ElseIf XICViewToolStripMenuItem.Checked Then
            ' switch back to MS scan groups
            XICViewToolStripMenuItem.Checked = False

            treeView1.Nodes.Clear()
            treeView1.loadRawFile(CurrentRawFile, False, rtmin, rtmax)
            checked.Clear()
        Else
            Call CurrentRawFile _
                .GetMs2Scans _
                .Where(Function(t) t.rt >= rtmin AndAlso t.rt <= rtmax) _
                .DoCall(AddressOf LoadXICIons)
        End If
    End Sub

    Private Sub LoadXICIons(data As IEnumerable(Of ScanMS2))
        Dim allMs2 = data _
            .GroupBy(Function(t) t.parentMz, Tolerance.DeltaMass(0.1)) _
            .OrderBy(Function(t) Val(t.name)) _
            .ToArray

        treeView1.Nodes.Clear()
        checked.Clear()

        XICViewToolStripMenuItem.Checked = True

        For Each mz1 As NamedCollection(Of ScanMS2) In allMs2
            Dim mzNode As TreeNode = treeView1.Nodes.Add(Val(mz1.name).ToString("F4") & $", {mz1.Length} MS/MS scans")

            For Each ms2 As ScanMS2 In mz1
                Call mzNode.Nodes.Add(New TreeNode(ms2.scan_id) With {
                    .Tag = ms2,
                    .ImageIndex = 1,
                    .SelectedImageIndex = 1
                })
            Next
        Next
    End Sub

    Private Function checkIon(ByRef mz As Double) As Boolean
        mz = Val(Strings.Trim(ToolStripSpringTextBox1.Text))

        If mz = 0.0 Then
            Call MyApplication.host.showStatusMessage($"M/z value expression '{ToolStripSpringTextBox1.Text}' is not a valid number expression, please input a valid m/z value...", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return False
        ElseIf CurrentRawFile Is Nothing Then
            Call MyApplication.host.showStatusMessage("Please load a raw data file at first!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return False
        End If

        Return True
    End Function

    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        Dim mz As Double
        Dim ppm As Double = MyApplication.host.GetPPMError

        If Not checkIon(mz) Then
            Return
        End If

        Dim Ms2 = CurrentRawFile.LoadMzpack(Sub(src, cache) frmFileExplorer.getRawCache(src,, cache)) _
            .GetMs2Scans _
            .Where(Function(m)
                       Return PPMmethod.PPM(m.parentMz, mz) <= ppm
                   End Function) _
            .OrderBy(Function(m) m.rt) _
            .ToArray

        Call treeView1.Nodes.Clear()

        Dim mzNode As TreeNode = treeView1.Nodes.Add($"M/z {mz.ToString("F4")}, {Ms2.Length} hits")

        For Each item In Ms2
            Call mzNode.Nodes.Add(New TreeNode(item.ToString) With {.Tag = item, .ImageIndex = 1, .SelectedImageIndex = 1})
        Next
    End Sub

    Private Sub treeView1_DragDrop(sender As Object, e As DragEventArgs) Handles treeView1.DragDrop
        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)
        Dim firstFile As String = files.ElementAtOrDefault(Scan0)

        If Not firstFile Is Nothing Then
            Call MyApplication.host.OpenFile(firstFile, showDocument:=True)
        End If
    End Sub

    ''' <summary>
    ''' 从原始数据文件中挑出目标一级离子作图
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click
        Dim mz As Double
        Dim ppm As Tolerance = Tolerance.DeltaMass(0.05)

        If Not checkIon(mz) Then
            Return
        End If

        Dim XIC = CurrentRawFile.LoadMzpack(Sub(src, cache) frmFileExplorer.getRawCache(src,, cache)).loaded.MS _
            .Select(Function(scan) (scan.rt, scan.GetIntensity(mz, ppm))) _
            .Where(Function(p) p.Item2 > 0) _
            .OrderBy(Function(p) p.rt) _
            .Select(Function(p) New ChromatogramTick With {.Time = p.rt, .Intensity = p.Item2}) _
            .ToArray

        Call MyApplication.mzkitRawViewer.TIC({New NamedCollection(Of ChromatogramTick)($"XIC m/z: {mz.ToString("F4")}", XIC)})
    End Sub

    Private Sub CopyIonsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyIonsToolStripMenuItem.Click
        If GetSelectedNodes.Count = 0 Then
            MyApplication.host.showStatusMessage("No chromatogram data for XIC plot, please use XIC -> Add for add data!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        Dim outfile As New StringWriter()
        Dim da03 As Tolerance = DAmethod.DeltaMass(0.3)
        Dim intocutoff As LowAbundanceTrimming = LowAbundanceTrimming.intoCutff

        Call outfile.WriteLine({"ID", "mz", "rt", "rt(min)", "intensity", "file", "Ion1", "Ion2", "Ion3", "Ion4", "Ion5"}.JoinBy(vbTab))

        For Each peak As PeakMs2 In MyApplication.mzkitRawViewer.getSelectedIonSpectrums(Nothing)
            Dim id As String = If(CInt(peak.rt) = 0, $"M{CInt(peak.mz)}", $"M{CInt(peak.mz)}T{CInt(peak.rt)}")
            Dim mz As String = peak.mz.ToString("F4")
            Dim rt As String = peak.rt.ToString("F2")
            Dim rtmin As String = (peak.rt / 60).ToString("F1")
            Dim into As String = peak.intensity
            Dim ions As String() = peak.mzInto _
                .Centroid(da03, intocutoff) _
                .OrderByDescending(Function(m) m.intensity) _
                .Take(5) _
                .Select(Function(m) m.mz.ToString("F4")) _
                .ToArray

            Call outfile.WriteLine({id, mz, rt, rtmin, into, peak.file}.JoinIterates(ions).JoinBy(vbTab))
        Next

        Call Clipboard.Clear()
        Call Clipboard.SetText(outfile.ToString)
        Call MyApplication.host.showStatusMessage("Ions data is copy to clipboard!")
    End Sub

    Private Sub ToolStripButton5_Click(sender As Object, e As EventArgs) Handles ToolStripButton5.Click
        Dim ms2 As Double = Val(ToolStripSpringTextBox1.Text)
        Dim test As Tolerance = Tolerance.DeltaMass(0.05)

        If CurrentRawFile Is Nothing Then
            Call MyApplication.host.warning("Open a raw data file at first!")
            Return
        End If

        Dim matched = CurrentRawFile _
            .LoadMzpack(Sub(src, cache) frmFileExplorer.getRawCache(src,, cache)).loaded _
            .MS _
            .Select(Function(i) i.products.SafeQuery) _
            .IteratesALL _
            .Where(Function(i)
                       If i.mz.IsNullOrEmpty Then
                           Return False
                       End If

                       Dim mz2 As ms2 = i.GetMs.Select(Function(mzi)
                                                           If test(mzi.mz, ms2) Then
                                                               Return mzi
                                                           Else
                                                               Return Nothing
                                                           End If
                                                       End Function).Where(Function(mzi) Not mzi Is Nothing).OrderByDescending(Function(mzi) mzi.intensity).FirstOrDefault

                       If mz2 Is Nothing OrElse mz2.intensity / i.into.Max < 0.5 Then
                           Return False
                       Else
                           Return True
                       End If
                   End Function) _
            .ToArray

        Call LoadXICIons(matched)
    End Sub

    Private Sub treeView1_DragEnter(sender As Object, e As DragEventArgs) Handles treeView1.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub
End Class
