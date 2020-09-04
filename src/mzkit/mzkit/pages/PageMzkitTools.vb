#Region "Microsoft.VisualBasic::b7f8ba228d88b57a3609f576041ef180, src\mzkit\mzkit\pages\PageMzkitTools.vb"

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

' Class PageMzkitTools
' 
'     Function: getXICMatrix
' 
'     Sub: AddToolStripMenuItem_Click, applyLevelFilter, Button1_Click, ClearToolStripMenuItem_Click, CustomToolStripMenuItem_Click
'          DataGridView1_CellContentClick, DefaultToolStripMenuItem_Click, DeleteFileToolStripMenuItem_Click, ExportExactMassSearchTable, ExportToolStripMenuItem_Click
'          GeneralFlavoneToolStripMenuItem_Click, ImportsRaw, InitializeFileTree, ListBox1_SelectedIndexChanged, missingCacheFile
'          MolecularNetworkingToolStripMenuItem_Click, MS1ToolStripMenuItem_Click, MS2ToolStripMenuItem_Click, NatureProductToolStripMenuItem_Click, PageMzkitTools_Load
'          PictureBox1_Click, PictureBox1_DoubleClick, runMzSearch, SaveFileCache, SaveImageToolStripMenuItem_Click
'          SaveMatrixToolStripMenuItem_Click, SearchFormulaToolStripMenuItem_Click, searchInFileByMz, SearchInFileToolStripMenuItem_Click, setCurrentFile
'          (+3 Overloads) showMatrix, showSpectrum, showStatusMessage, ShowTICToolStripMenuItem_Click, ShowXICToolStripMenuItem_Click
'          SmallMoleculeToolStripMenuItem_Click, TreeView1_AfterSelect
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports mzkit.DockSample
Imports mzkit.My
Imports RibbonLib
Imports RibbonLib.Controls.Events
Imports RibbonLib.Interop
Imports Task
Imports WeifenLuo.WinFormsUI.Docking
Imports stdNum = System.Math

Public Class PageMzkitTools

    Dim RibbonItems As RibbonItems
    Dim matrix As [Variant](Of ms2(), ChromatogramTick(), SSM2MatrixFragment())
    Dim matrixName As String
    Dim TreeView1 As TreeView
    Dim ListBox1 As ListBox

    Friend _ribbonExportDataContextMenuStrip As ExportData

    Public Sub Ribbon_Load(ribbon As Ribbon)
        _ribbonExportDataContextMenuStrip = New ExportData(ribbon, RibbonItems.cmdContextMap)
    End Sub

    Sub InitializeFileTree()
        If TreeView1.LoadRawFileCache = 0 Then
            MyApplication.host.showStatusMessage($"It seems that you don't have any raw file opended. You could open raw file through [File] -> [Open Raw File].", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            TreeView1.SelectedNode = TreeView1.Nodes.Item(Scan0)
            setCurrentFile()
        End If

        MyApplication.host.ToolStripStatusLabel2.Text = TreeView1.GetTotalCacheSize
    End Sub

    Private Sub missingCacheFile(raw As Raw)
        If MessageBox.Show($"The specific raw data cache is missing, run imports again?{vbCrLf}{raw.cache.GetFullPath}", "Cache Not Found!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = DialogResult.OK Then
            Dim newRaw = getRawCache(raw.source)

            For i As Integer = 0 To TreeView1.Nodes.Count - 1
                If TreeView1.Nodes(i).Tag Is raw Then
                    TreeView1.Nodes(i).Tag = newRaw
                End If
            Next

            MyApplication.host.showStatusMessage("Ready!")
            MyApplication.host.ToolStripStatusLabel2.Text = TreeView1.GetTotalCacheSize
        End If
    End Sub

    Public Sub SaveFileCache()
        Call TreeView1.SaveRawFileCache
    End Sub

    Public Sub ImportsRaw(fileName As String)
        Call TreeView1.addRawFile(getRawCache(fileName))

        MyApplication.host.showStatusMessage("Ready!")
        MyApplication.host.ToolStripStatusLabel2.Text = TreeView1.GetTotalCacheSize
    End Sub

    Public Function getRawCache(fileName As String) As Raw
        Dim progress As New frmTaskProgress() With {.Text = $"Imports raw data [{fileName}]"}
        Dim showProgress As Action(Of String) = Sub(text) progress.Invoke(Sub() progress.Label1.Text = text)
        Dim task As New Task.ImportsRawData(fileName, showProgress, Sub() Call progress.Invoke(Sub() progress.Close()))
        Dim runTask As New Thread(AddressOf task.RunImports)

        MyApplication.host.showStatusMessage("Run Raw Data Imports")
        progress.Label2.Text = progress.Text

        Call runTask.Start()
        Call progress.ShowDialog()

        'Call New frmRawViewer() With {
        '    .MdiParent = Me,
        '    .Text = file.FileName,
        '    .rawFile = task.raw
        '}.Show()
        Return task.raw
    End Function

    Private Sub TreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs)
        If TypeOf e.Node.Tag Is Task.Raw Then

            Dim TIC = rawTIC(e.Node.Tag, False)

            ' 原始文件节点
            ' 只显示当前文件的TIC图
            showMatrix(TIC.value, TIC.name)

            PictureBox1.BackgroundImage = ChromatogramPlot.TICplot(TIC, colorsSchema:=Globals.GetColors).AsGDIImage

            MyApplication.host.ShowPage(Me)

            MyApplication.host.Invoke(Sub() RibbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.NotAvailable)

        ElseIf TypeOf e.Node.Tag Is ms2() Then
            ' TIC 图绘制
            Dim raw = DirectCast(e.Node.Tag, ms2())
            Dim selects = TreeView1.CurrentRawFile.raw
            Dim TIC As New NamedCollection(Of ChromatogramTick) With {
                .name = $"m/z {raw.Select(Function(m) m.mz).Min.ToString("F3")} - {raw.Select(Function(m) m.mz).Max.ToString("F3")}",
                .value = raw _
                    .Select(Function(a)
                                Return New ChromatogramTick With {
                                    .Time = Val(a.Annotation),
                                    .Intensity = a.intensity
                                }
                            End Function) _
                    .ToArray
            }
            Dim maxY As Double = selects.scans.Select(Function(a) a.TIC).Max

            TIC.value = {
                New ChromatogramTick With {.Time = selects.rtmin},
                New ChromatogramTick With {.Time = selects.rtmax}
            }.JoinIterates(TIC.value) _
             .OrderBy(Function(c) c.Time) _
             .ToArray
            showMatrix(TIC.value, TIC.name)
            PictureBox1.BackgroundImage = TIC.TICplot(intensityMax:=maxY, colorsSchema:=Globals.GetColors).AsGDIImage

            MyApplication.host.ShowPage(Me)
        Else
            ' scan节点
            Dim raw As Task.Raw = e.Node.Parent.Tag
            Dim scanId As String = e.Node.Text

            Call showSpectrum(scanId, raw)
            Call MyApplication.host.ShowPage(Me)
        End If

        Call setCurrentFile()
    End Sub

    Private Sub PageMzkitTools_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim host = MyApplication.host
        RibbonItems = host.ribbonItems
        TreeView1 = host.TreeView1
        ListBox1 = host.searchList.ListBox1

        Call InitializeFileTree()

        AddHandler ListBox1.SelectedIndexChanged, AddressOf ListBox1_SelectedIndexChanged
        AddHandler TreeView1.AfterSelect, AddressOf TreeView1_AfterSelect
        AddHandler host.fileExplorer.Button1.Click, Sub(obj, evt) Call SearchByMz(host.fileExplorer.TextBox2.Text)

        AddHandler host.fileExplorer.ShowTICToolStripMenuItem.Click, AddressOf ShowTICToolStripMenuItem_Click
        AddHandler host.fileExplorer.ShowXICToolStripMenuItem.Click, AddressOf ShowXICToolStripMenuItem_Click

        AddHandler host.fileExplorer.ClearToolStripMenuItem.Click, AddressOf ClearToolStripMenuItem_Click
        AddHandler host.fileExplorer.ExportToolStripMenuItem.Click, AddressOf ExportToolStripMenuItem_Click

        AddHandler host.fileExplorer.MS1ToolStripMenuItem.Click, AddressOf MS1ToolStripMenuItem_Click
        AddHandler host.fileExplorer.MS2ToolStripMenuItem.Click, AddressOf MS2ToolStripMenuItem_Click

        AddHandler host.fileExplorer.MolecularNetworkingToolStripMenuItem.Click, AddressOf MolecularNetworkingToolStripMenuItem_Click

        AddHandler host.fileExplorer.SearchInFileToolStripMenuItem.Click, AddressOf SearchInFileToolStripMenuItem_Click
        AddHandler host.fileExplorer.CustomToolStripMenuItem.Click, AddressOf CustomToolStripMenuItem_Click
        AddHandler host.fileExplorer.DefaultToolStripMenuItem.Click, AddressOf DefaultToolStripMenuItem_Click
        AddHandler host.fileExplorer.SmallMoleculeToolStripMenuItem.Click, AddressOf SmallMoleculeToolStripMenuItem_Click
        AddHandler host.fileExplorer.NatureProductToolStripMenuItem.Click, AddressOf NatureProductToolStripMenuItem_Click
        AddHandler host.fileExplorer.GeneralFlavoneToolStripMenuItem.Click, AddressOf GeneralFlavoneToolStripMenuItem_Click
    End Sub

    Dim currentMatrix As [Variant](Of ms2(), ChromatogramTick())

    Public Sub ShowPropertyWindow()
        Dim propertyWin = MyApplication.host.propertyWin
        Dim dockRight = propertyWin.DockState = DockState.Hidden OrElse propertyWin.DockState = DockState.Unknown

        If dockRight Then
            propertyWin.DockState = DockState.DockRight
        End If
    End Sub

    Private Sub showSpectrum(scanId As String, raw As Raw)
        If raw.cache.FileExists Then
            Dim prop As SpectrumProperty = Nothing
            Dim scanData As LibraryMatrix = raw.GetSpectrum(scanId, prop)

            showMatrix(scanData.ms2, scanId)

            Dim title1$
            Dim title2$

            If prop.msLevel = 1 Then
                title1 = $"MS1 Scan@{prop.retentionTime}sec"
                title2 = scanData.name
            Else
                title1 = $"M/Z {prop.precursorMz}, RT {prop.rtmin}min"
                title2 = scanData.name
            End If

            Dim draw As Image = scanData.MirrorPlot(titles:={title1, title2}).AsGDIImage
            Dim propertyWin = MyApplication.host.propertyWin

            propertyWin.Invoke(
                Sub()
                    ' PropertyGrid1.SelectedObject = prop
                    'PropertyGrid1.Refresh()
                    propertyWin.propertyGrid.SelectedObject = prop
                    propertyWin.propertyGrid.Refresh()

                    ShowPropertyWindow()
                End Sub)

            PictureBox1.BackgroundImage = draw
            ShowTabPage(TabPage5)
        Else
            Call missingCacheFile(raw)
        End If
    End Sub

    Public Sub setCurrentFile()
        If TreeView1.Nodes.Count = 0 Then
            MyApplication.host.showStatusMessage("No raw file opened.")
            Return
        End If

        With TreeView1.CurrentRawFile.raw
            Static selectedFile As String

            If selectedFile <> MyApplication.host.ToolStripStatusLabel1.Text Then
                selectedFile = $"{ .source.FileName} [{ .numOfScans} scans]"
                MyApplication.host.showStatusMessage(selectedFile)
                ListBox1.Items.Clear()
            End If

            MyApplication.host.Text = $"BioNovoGene Mzkit [{ .source.GetFullPath}]"
        End With

        If Not TreeView1.CurrentRawFile.raw.cache.FileExists Then
            TreeView1.SelectedNode.ImageIndex = 1
            TreeView1.SelectedNode.SelectedImageIndex = 1
        End If
    End Sub

    Private Function rawTIC(raw As Raw, isBPC As Boolean) As NamedCollection(Of ChromatogramTick)
        Dim TIC As New NamedCollection(Of ChromatogramTick) With {
            .name = $"{If(isBPC, "BPC", "TIC")} [{raw.source.FileName}]",
            .value = raw.scans _
                .Where(Function(a) a.mz = 0R) _
                .Select(Function(m)
                            Return New ChromatogramTick With {.Time = m.rt, .Intensity = If(isBPC, m.BPC, m.TIC)}
                        End Function) _
                .ToArray
        }

        TIC.value = {
                New ChromatogramTick With {.Time = raw.rtmin},
                New ChromatogramTick With {.Time = raw.rtmax}
            }.JoinIterates(TIC.value) _
             .OrderBy(Function(c) c.Time) _
             .ToArray

        Return TIC
    End Function

    Public Sub ShowTICToolStripMenuItem_Click(sender As Object, e As EventArgs)
        TIC(isBPC:=False)
    End Sub

    Public Sub TIC(isBPC As Boolean)
        Dim rawList As New List(Of Raw)

        For i As Integer = 0 To TreeView1.Nodes.Count - 1
            If Not TreeView1.Nodes(i).Checked Then
                If Not TreeView1.Nodes(i) Is TreeView1.SelectedNode Then
                    Continue For
                End If
            End If

            Dim raw As Raw = TreeView1.Nodes(i).Tag

            If Not raw.cache.FileExists Then
                Call missingCacheFile(raw)
            End If

            rawList.Add(raw)
        Next

        If rawList.Count = 0 Then
            Dim current = TreeView1.CurrentRawFile.raw

            If current Is Nothing Then
                MyApplication.host.showStatusMessage("No file data selected for TIC plot...")
                Return
            Else
                rawList.Add(current)
            End If
        End If

        Dim TICList As New List(Of NamedCollection(Of ChromatogramTick))

        For Each raw As Raw In rawList
            TICList.Add(rawTIC(raw, isBPC))
        Next

        showMatrix(TICList(Scan0).value, TICList(Scan0).name)

        PictureBox1.BackgroundImage = ChromatogramPlot.TICplot(TICList.ToArray, colorsSchema:=Globals.GetColors).AsGDIImage

        MyApplication.host.ShowPage(Me)

        'Using cache As New netCDFReader(raw.raw.cache)
        '    Dim progress As New frmTaskProgress() With {.Text = $"Reading TIC raw data [{raw.raw.source}]"}
        '    Dim showProgress As Action(Of String) = Sub(text) progress.Invoke(Sub() progress.Label1.Text = text)
        '    Dim mzgroups As NamedCollection(Of ms2)() = {}
        '    Dim runTask As New Thread(
        '            Sub()
        '                Dim ms1n = raw.raw.scans.Where(Function(a) a.mz = 0R).Count
        '                Dim i As i32 = 1
        '                Dim allMz As New List(Of ms2)
        '                Dim mztemp As ms2()

        '                For Each scan In raw.raw.scans
        '                    If scan.mz = 0 Then
        '                        Dim entry = cache.getDataVariableEntry(scan.id)
        '                        Dim rt As String = entry.attributes.Where(Function(a) a.name = "retentionTime").FirstOrDefault?.value

        '                        mztemp = cache.getDataVariable(entry).numerics.AsMs2.ToArray

        '                        For i2 As Integer = 0 To mztemp.Length - 1
        '                            mztemp(i2).Annotation = rt
        '                        Next

        '                        allMz.AddRange(mztemp)
        '                        showProgress($"[{++i}/{ms1n}] {scan.id}")
        '                    End If
        '                Next

        '                showProgress("Run m/z group....")
        '                mzgroups = allMz _
        '                    .GroupBy(Function(mz) mz.mz, Tolerance.DeltaMass(5)) _
        '                    .Select(Function(a)
        '                                Dim max = a.Select(Function(m) m.intensity).Max

        '                                Return New NamedCollection(Of ms2) With {.value = a.value.Where(Function(m) m.intensity / max >= 0.05).OrderBy(Function(m) Val(m.Annotation)).ToArray}
        '                            End Function) _
        '                    .ToArray
        '                progress.Invoke(Sub() progress.Close())
        '            End Sub)

        '    showStatusMessage("Run Raw Data Imports")
        '    progress.Label2.Text = progress.Text

        '    Call runTask.Start()
        '    Call progress.ShowDialog()

        '    For Each mzblock In mzgroups
        '        Dim range As New DoubleRange(mzblock.Select(Function(m) m.mz))

        '        raw.tree.Nodes.Add(New TreeNode($"m/z {range.Min.ToString("F3")} - {range.Max.ToString("F3")}") With {.Tag = mzblock.ToArray})
        '    Next

        '    showStatusMessage("Ready!")
        'End Using

        ' MyApplication.host.Invoke(Sub() RibbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.NotAvailable)
    End Sub

    Public Sub SaveImageToolStripMenuItem_Click()
        If Not PictureBox1.BackgroundImage Is Nothing Then
            Dim preFileName As String = matrixName.NormalizePathString(alphabetOnly:=False)

            Using file As New SaveFileDialog With {.Filter = "image(*.png)|*.png", .FileName = preFileName & ".png"}
                If file.ShowDialog = DialogResult.OK Then
                    Call PictureBox1.BackgroundImage.SaveAs(file.FileName)
                    Call Process.Start(file.FileName)
                End If
            End Using
        Else
            MyApplication.host.showStatusMessage("No plot image for save, please select one spectrum to start!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        End If
    End Sub

    Private Sub applyLevelFilter()
        Dim raw = TreeView1.CurrentRawFile

        If Not raw.raw Is Nothing Then
            raw.tree.Nodes.Clear()
            raw.tree.addRawFile(raw.raw, MyApplication.host.fileExplorer.MS1ToolStripMenuItem.Checked, MyApplication.host.fileExplorer.MS2ToolStripMenuItem.Checked)
        End If
    End Sub

    Private Sub MS1ToolStripMenuItem_Click(sender As Object, e As EventArgs)
        MyApplication.host.fileExplorer.MS1ToolStripMenuItem.Checked = Not MyApplication.host.fileExplorer.MS1ToolStripMenuItem.Checked
        applyLevelFilter()
    End Sub

    Private Sub MS2ToolStripMenuItem_Click(sender As Object, e As EventArgs)
        MyApplication.host.fileExplorer.MS2ToolStripMenuItem.Checked = Not MyApplication.host.fileExplorer.MS2ToolStripMenuItem.Checked
        applyLevelFilter()
    End Sub

    Public Sub SearchByMz(text As String)
        If text.StringEmpty Then
            Return
        ElseIf text.IsNumeric Then
            Call searchInFileByMz(mz:=Val(text))
        Else
            ' formula
            Dim exact_mass As Double = Math.EvaluateFormula(text)
            Dim ppm As Double = Val(RibbonItems.PPMSpinner.DecimalValue)
            Dim raw As Raw = TreeView1.CurrentRawFile.raw

            DataGridView1.Rows.Clear()
            DataGridView1.Columns.Clear()

            MyApplication.host.showStatusMessage($"Search MS ions for [{text}] exact_mass={exact_mass} with tolerance error {ppm} ppm")

            DataGridView1.Columns.Add(New DataGridViewLinkColumn With {
                  .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                  .ValueType = GetType(String),
                  .HeaderText = "scan Id"})

            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn() With {
                  .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                  .ValueType = GetType(String),
                  .HeaderText = "m/z"})

            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn() With {
                  .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                  .ValueType = GetType(String),
                  .HeaderText = "rt"})

            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn() With {
                  .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                  .ValueType = GetType(String),
                  .HeaderText = "rt(min)"})

            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn() With {
                  .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                  .ValueType = GetType(String),
                  .HeaderText = "intensity"})

            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn() With {
                  .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                  .ValueType = GetType(String),
                  .HeaderText = "M"})

            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn() With {
                  .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                  .ValueType = GetType(String),
                  .HeaderText = "adducts"})

            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn() With {
                  .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                  .ValueType = GetType(String),
                  .HeaderText = "charge"})

            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn() With {
                  .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                  .ValueType = GetType(String),
                  .HeaderText = "precursor_type"})

            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn() With {
                  .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                  .ValueType = GetType(String),
                  .HeaderText = "ppm"})

            ' C25H40N4O5
            Dim pos = MzCalculator.EvaluateAll(exact_mass, "+", False).ToArray
            Dim neg = MzCalculator.EvaluateAll(exact_mass, "-", False).ToArray

            For Each scan As ScanEntry In raw.scans
                If scan.polarity > 0 Then
                    For Each mode In pos
                        If PPMmethod.PPM(scan.mz, Val(mode.mz)) <= ppm Then
                            DataGridView1.Rows.Add(
                                scan.id,
                                scan.mz.ToString("F4"),
                                CInt(scan.rt),
                                (scan.rt / 60).ToString("F2"),
                                scan.XIC.ToString("G3"),
                                mode.M,
                                mode.adduct,
                                mode.charge,
                                mode.precursor_type,
                                PPMmethod.PPM(scan.mz, Val(mode.mz)).ToString("F2"))
                        End If
                    Next
                ElseIf scan.polarity < 0 Then
                    For Each mode In neg
                        If PPMmethod.PPM(scan.mz, Val(mode.mz)) <= ppm Then
                            DataGridView1.Rows.Add(
                                scan.id,
                                scan.mz.ToString("F4"),
                                CInt(scan.rt),
                                (scan.rt / 60).ToString("F2"),
                                scan.XIC.ToString("G3"),
                                mode.M,
                                mode.adduct,
                                mode.charge,
                                mode.precursor_type,
                                PPMmethod.PPM(scan.mz, Val(mode.mz)).ToString("F2"))
                        End If
                    Next
                End If
            Next

            CustomTabControl1.SelectedTab = TabPage6

            MyApplication.host.ribbonItems.TabGroupExactMassSearchTools.ContextAvailable = ContextAvailability.Active
        End If
    End Sub

    Public Sub ExportExactMassSearchTable()
        Call DataGridView1.SaveDataGrid
    End Sub

    Private Sub searchInFileByMz(mz As Double)
        Dim ppm As Double = Val(RibbonItems.PPMSpinner.DecimalValue)
        Dim raw = TreeView1.CurrentRawFile.raw
        Dim ms2Hits = raw.scans.Where(Function(m) PPMmethod.PPM(m.mz, mz) <= ppm).ToArray

        ListBox1.Items.Clear()

        For Each hit As ScanEntry In ms2Hits
            ListBox1.Items.Add(hit)
        Next

        If ms2Hits.Length = 0 Then
            MyApplication.host.searchList.Label2.Text = "no hits!"
            MessageBox.Show($"Sorry, no hits was found for m/z={mz} with tolerance {ppm}ppm...", "No hits found!", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            MyApplication.host.searchList.Label2.Text = $"{ms2Hits.Length} ms2 hits for m/z={mz} with tolerance {ppm}ppm"
        End If

        Dim dockLeft As Boolean = MyApplication.host.searchList.DockState = DockState.Hidden OrElse MyApplication.host.searchList.DockState = DockState.Unknown

        MyApplication.host.searchList.Show(MyApplication.host.dockPanel)
        MyApplication.host.searchList.Activate()

        If dockLeft Then
            MyApplication.host.searchList.DockState = DockState.DockLeft
        End If
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim scanId As ScanEntry = ListBox1.SelectedItem
        Dim raw = TreeView1.CurrentRawFile.raw

        Call showSpectrum(scanId.id, raw)
    End Sub

    Private Sub SearchInFileToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Call runMzSearch(Sub(mz) Call searchInFileByMz(mz))
    End Sub

    Public Sub SaveMatrixToolStripMenuItem_Click()
        If matrix Is Nothing Then
            MyApplication.host.showStatusMessage("No matrix data for save, please select one spectrum to start!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If
        If matrix Like GetType(ms2()) Then
            Using file As New SaveFileDialog() With {.Filter = "Excel Table(*.xls)|*.xls", .FileName = matrixName.NormalizePathString(False)}
                If file.ShowDialog = DialogResult.OK Then
                    Call matrix.TryCast(Of ms2()).SaveTo(file.FileName)
                End If
            End Using
        ElseIf matrix Like GetType(ChromatogramTick()) Then
            Using file As New SaveFileDialog() With {.Filter = "Excel Table(*.xls)|*.xls", .FileName = matrixName.NormalizePathString(False)}
                If file.ShowDialog = DialogResult.OK Then
                    Call matrix.TryCast(Of ChromatogramTick()).SaveTo(file.FileName)
                End If
            End Using
        ElseIf matrix Like GetType(SSM2MatrixFragment()) Then
            Using file As New SaveFileDialog() With {.Filter = "Excel Table(*.xls)|*.xls", .FileName = matrixName.NormalizePathString(False)}
                If file.ShowDialog = DialogResult.OK Then
                    Call matrix.TryCast(Of SSM2MatrixFragment()).SaveTo(file.FileName)
                End If
            End Using
        End If
    End Sub

    Private Sub runMzSearch(searchAction As Action(Of Double))
        Dim current = TreeView1.CurrentRawFile
        Dim node = TreeView1.SelectedNode

        If Not node Is Nothing AndAlso current.raw.cache.FileExists Then
            Dim mz = current.raw.scans.Where(Function(scan) scan.id = node.Text).FirstOrDefault

            If Not mz Is Nothing AndAlso mz.mz > 0 Then
                Call searchAction(mz.mz)
            End If
        End If
    End Sub

    Private Sub SearchFormulaToolStripMenuItem_Click(sender As Object, e As EventArgs) ' Handles SearchFormulaToolStripMenuItem.Click
        Dim current = TreeView1.CurrentRawFile
        Dim node = TreeView1.SelectedNode

        If Not node Is Nothing AndAlso current.raw.cache.FileExists Then
            Dim mz = current.raw.scans.Where(Function(scan) scan.id = node.Text).FirstOrDefault

            If Not mz Is Nothing AndAlso mz.mz > 0 Then
                Dim charge As Double = mz.charge
                Dim ionMode As Integer = mz.polarity

                If charge = 0 Then
                    charge = 1
                End If

                MyApplication.host.mzkitSearch.doMzSearch(mz.mz, charge, ionMode)
                MyApplication.host.ShowPage(MyApplication.host.mzkitSearch)
            End If
        End If
    End Sub

    Private Sub MolecularNetworkingToolStripMenuItem_Click(sender As Object, e As EventArgs)
        'If TreeView1.CurrentRawFile.raw Is Nothing Then
        '    Return
        'End If

        ' Dim raw As Raw = TreeView1.CurrentRawFile.raw
        Dim similarityCutoff As Double = MyApplication.host.ribbonItems.SpinnerSimilarity.DecimalValue
        Dim progress As New frmTaskProgress
        Dim runTask As New Thread(
            Sub()

                Thread.Sleep(1000)

                progress.Invoke(Sub() progress.Label1.Text = "loading cache ms2 scan data...")

                Dim raw = getSelectedIonSpectrums().ToArray

                If raw.Length = 0 Then
                    MyApplication.host.showStatusMessage("No spectrum data, please select a file or some spectrum...", My.Resources.StatusAnnotations_Warning_32xLG_color)
                    progress.Invoke(Sub() progress.Close())
                    Return
                End If

                Dim protocol As New Protocols(Tolerance.PPM(15), Tolerance.DeltaMass(0.3), 0.85, 0.7, 0.05)
                Dim progressMsg As Action(Of String) =
                    Sub(msg)
                        progress.Invoke(Sub() progress.Label2.Text = msg)
                    End Sub


                'Dim run As New List(Of PeakMs2)
                'Dim nodes As New Dictionary(Of String, ScanEntry)
                'Dim idList As New Dictionary(Of String, Integer)



                'Using cache As New netCDFReader(raw.cache)
                '    For Each scan In raw.scans.Where(Function(s) s.mz > 0)
                '        Dim uid As String = $"M{CInt(scan.mz)}T{CInt(scan.rt)}"

                '        If idList.ContainsKey(uid) Then
                '            idList(uid) += 1
                '            uid = uid & "_" & (idList(uid) - 1)
                '        Else
                '            idList.Add(uid, 1)
                '        End If

                '        run += New PeakMs2 With {
                '            .rt = scan.rt,
                '            .mz = scan.mz,
                '            .lib_guid = uid,
                '            .mzInto = cache.getDataVariable(scan.id).numerics.AsMs2.ToArray.Centroid(Tolerance.DeltaMass(0.3)).ToArray
                '        }

                '        progress.Invoke(Sub() progress.Label2.Text = scan.id)
                '        nodes.Add(run.Last.lib_guid, scan)
                '    Next
                'End Using

                progress.Invoke(Sub() progress.Label2.Text = "run molecular networking....")

                ' Call tree.doCluster(run)
                Dim links = protocol.RunProtocol(raw, progressMsg).ProduceNodes.Networking.ToArray
                Dim net As IO.DataSet() = links _
                    .Select(Function(a)
                                Return New IO.DataSet With {
                                    .ID = a.Name,
                                    .Properties = a.Value _
                                        .ToDictionary(Function(t) t.Key,
                                                      Function(t)
                                                          Return stdNum.Min(t.Value.forward, t.Value.reverse)
                                                      End Function)
                                }
                            End Function) _
                    .ToArray   ' MoleculeNetworking.CreateMatrix(run, 0.8, Tolerance.DeltaMass(0.3), Sub(msg) progress.Invoke(Sub() progress.Label1.Text = msg)).ToArray

                progress.Invoke(Sub() progress.Label1.Text = "run family clustering....")

                Dim clusters = net.ToKMeansModels.Kmeans(expected:=10, debug:=False)
                Dim rawLinks = links.ToDictionary(Function(a) a.Name, Function(a) a.Value)

                progress.Invoke(Sub() progress.Label1.Text = "initialize result output...")

                MyApplication.host.Invoke(
                    Sub()
                        Call MyApplication.host.mzkitMNtools.loadNetwork(clusters, protocol, rawLinks, similarityCutoff)
                        Call MyApplication.host.ShowPage(MyApplication.host.mzkitMNtools)
                    End Sub)

                progress.Invoke(Sub() progress.Close())
            End Sub)

        runTask.Start()
        progress.ShowDialog()
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        If e.ColumnIndex = Scan0 AndAlso e.RowIndex >= 0 Then
            Dim scanId As String = DataGridView1.Rows(e.RowIndex).Cells(0).Value?.ToString

            If Not scanId.StringEmpty Then
                Call showSpectrum(scanId, TreeView1.CurrentRawFile.raw)
            End If
        End If
    End Sub

    Private Sub CustomToolStripMenuItem_Click(sender As Object, e As EventArgs)
        MyApplication.host.mzkitSearch.ComboBox1.SelectedIndex = 0
        SearchFormulaToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub DefaultToolStripMenuItem_Click(sender As Object, e As EventArgs)
        MyApplication.host.mzkitSearch.ComboBox1.SelectedIndex = 1
        SearchFormulaToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub SmallMoleculeToolStripMenuItem_Click(sender As Object, e As EventArgs)
        MyApplication.host.mzkitSearch.ComboBox1.SelectedIndex = 2
        SearchFormulaToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub NatureProductToolStripMenuItem_Click(sender As Object, e As EventArgs)
        MyApplication.host.mzkitSearch.ComboBox1.SelectedIndex = 3
        SearchFormulaToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub GeneralFlavoneToolStripMenuItem_Click(sender As Object, e As EventArgs)
        MyApplication.host.mzkitSearch.ComboBox1.SelectedIndex = 4
        SearchFormulaToolStripMenuItem_Click(sender, e)
    End Sub

    Sub showMatrix(matrix As ms2(), name As String)
        Me.matrix = matrix
        matrixName = name

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "m/z"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "intensity"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "relative"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "annotation"})

        Dim max As Double = matrix.Select(Function(a) a.intensity).Max

        For Each tick In matrix
            tick.quantity = CInt(tick.intensity / max * 100)
            DataGridView1.Rows.Add({tick.mz, tick.intensity, tick.quantity, tick.Annotation})
        Next
    End Sub

    Sub showMatrix(matrix As SSM2MatrixFragment(), name As String)
        Me.matrix = matrix
        matrixName = name

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "m/z"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "intensity(query)"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "intensity(target)"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "tolerance"})

        For Each tick In matrix
            DataGridView1.Rows.Add({tick.mz, tick.query, tick.ref, tick.da})
        Next
    End Sub

    Sub showMatrix(matrix As ChromatogramTick(), name As String)
        Me.matrix = matrix
        matrixName = name

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "time"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "intensity"})

        For Each tick In matrix
            DataGridView1.Rows.Add({tick.Time, tick.Intensity})
        Next
    End Sub

    Public Sub ShowXICToolStripMenuItem_Click()
        If TypeOf TreeView1.SelectedNode.Tag Is Raw AndAlso MyApplication.host.fileExplorer.GetSelectedNodes.Count = 0 Then
            Return
        End If

        ' scan节点
        Dim raw As Task.Raw = TreeView1.CurrentRawFile.raw
        Dim ppm As Double = Val(RibbonItems.PPMSpinner.DecimalValue)
        Dim plotTIC = getXICMatrix(raw, TreeView1.SelectedNode.Text, ppm, relativeInto)
        Dim maxY As Double = raw.scans _
            .Where(Function(a) a.mz > 0) _
            .Select(Function(a) a.XIC) _
            .Max

        If plotTIC.value.IsNullOrEmpty Then
            ' 当前没有选中MS2，但是可以显示选中的XIC
            If MyApplication.host.fileExplorer.GetSelectedNodes.Count > 0 Then
            Else
                MyApplication.host.showStatusMessage("No ion was selected for XIC plot...", My.Resources.StatusAnnotations_Warning_32xLG_color)
                Return
            End If
        Else
            showMatrix(plotTIC.value, Name)
        End If

        Dim progress As New frmProgressSpinner
        Dim plotImage As Image = Nothing
        Dim relative As Boolean = relativeInto()

        Call New Thread(
            Sub()
                Dim XICPlot As New List(Of NamedCollection(Of ChromatogramTick))

                If Not plotTIC.IsEmpty Then
                    XICPlot.Add(plotTIC)
                End If

                XICPlot.AddRange(GetXICCollection(ppm))

                plotImage = XICPlot.ToArray.TICplot(intensityMax:=maxY, isXIC:=True, colorsSchema:=Globals.GetColors).AsGDIImage
                progress.Invoke(Sub() progress.Close())
            End Sub).Start()

        progress.ShowDialog()

        PictureBox1.BackgroundImage = plotImage
        ShowTabPage(TabPage5)
    End Sub

    Public Iterator Function GetXICCollection(ppm As Double) As IEnumerable(Of NamedCollection(Of ChromatogramTick))
        Dim explorer = MyApplication.host.fileExplorer
        Dim files As IGrouping(Of String, TreeNode)() = explorer.Invoke(Function() explorer.GetSelectedNodes.GroupBy(Function(a) a.Parent.Text).ToArray)

        For Each file In files
            Dim scans = file.Select(Function(a) DirectCast(a.Tag, ScanEntry)) _
                .Where(Function(a) a.mz > 0) _
                .GroupBy(Function(a) a.mz, Tolerance.DeltaMass(0.3)) _
                .ToArray
            Dim Raw = file.First.Parent.Tag

            For Each scanId In scans.Select(Function(a) a.value.First.id)
                Yield getXICMatrix(Raw, scanId, ppm, relativeInto)
            Next
        Next
    End Function

    Private Iterator Function getSelectedIonSpectrums() As IEnumerable(Of PeakMs2)
        Dim explorer = MyApplication.host.fileExplorer

        For i As Integer = 0 To explorer.treeView1.Nodes.Count - 1
            Dim file = explorer.treeView1.Nodes(i)
            Dim raw As Raw = file.Tag
            Dim rawScans As New Dictionary(Of String, ScanEntry)

            For Each scan In raw.scans
                rawScans.Add(scan.id, scan)
            Next

            Using cache As New netCDFReader(raw.cache)
                For j As Integer = 0 To file.Nodes.Count - 1
                    Dim scan = file.Nodes(j)
                    Dim scanId As String = scan.Text

                    If scan.Checked AndAlso rawScans(scanId).mz > 0 Then
                        Dim entry = cache.getDataVariableEntry(scanId)
                        Dim mztemp = cache.getDataVariable(entry).numerics.AsMs2.ToArray
                        Dim attrs = cache.getDataVariableEntry(scanId).attributes
                        Dim info As New SpectrumProperty(scanId, attrs)

                        Yield New PeakMs2 With {
                            .mz = info.precursorMz,
                            .scan = 0,
                            .activation = info.activationMethod,
                            .collisionEnergy = info.collisionEnergy,
                            .file = raw.source.FileName,
                            .lib_guid = $"{ .file}#{scanId}",
                            .mzInto = mztemp,
                            .precursor_type = "n/a",
                            .rt = info.retentionTime
                        }
                    End If
                Next
            End Using
        Next
    End Function

    Private Function relativeInto() As Boolean
        Return False ' MyApplication.host.ribbonItems.CheckBoxXICRelative.BooleanValue
    End Function

    Private Function getXICMatrix(raw As Raw, scanId As String, ppm As Double, relativeInto As Boolean) As NamedCollection(Of ChromatogramTick)
        Dim ms2 As ScanEntry = raw.scans.Where(Function(a) a.id = scanId).FirstOrDefault
        Dim name As String

        If ms2 Is Nothing OrElse ms2.mz = 0.0 Then
            MyApplication.host.showStatusMessage("XIC plot is not avaliable for MS1 parent!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return Nothing
        Else
            name = $"XIC [m/z={ms2.mz.ToString("F4")}, {ppm}ppm]"
            MyApplication.host.showStatusMessage(name, Nothing)
        End If

        Dim XIC As ChromatogramTick() = raw.scans _
            .Where(Function(a) PPMmethod.PPM(a.mz, ms2.mz) <= ppm) _
            .Select(Function(a)
                        Return New ChromatogramTick With {
                            .Time = a.rt,
                            .Intensity = a.XIC
                        }
                    End Function) _
            .ToArray

        If Not relativeInto Then
            XIC = {
                  New ChromatogramTick With {.Time = raw.rtmin},
                  New ChromatogramTick With {.Time = raw.rtmax}
              }.JoinIterates(XIC) _
               .OrderBy(Function(c) c.Time) _
               .ToArray
        End If

        Dim plotTIC As New NamedCollection(Of ChromatogramTick) With {
            .name = name,
            .value = XIC,
            .description = ms2.mz & " " & raw.source.FileName
        }

        Return plotTIC
    End Function

    'Private Sub AddToolStripMenuItem_Click(sender As Object, e As EventArgs)
    '    Dim XIC = getXICMatrix(TreeView1.CurrentRawFile.raw)

    '    If Not XIC.IsEmpty Then
    '        XICCollection.Add(XIC)
    '    End If
    'End Sub

    Private Sub ClearToolStripMenuItem_Click(sender As Object, e As EventArgs)
        ' XICCollection.Clear()
        MyApplication.host.fileExplorer.Clear()
        MyApplication.host.fileExplorer.ClearToolStripMenuItem.Text = "Clear"
    End Sub

    Private Sub PictureBox1_DoubleClick(sender As Object, e As EventArgs) Handles PictureBox1.DoubleClick
        If Not PictureBox1.BackgroundImage Is Nothing Then
            Dim temp As String = App.GetAppSysTempFile(".png", App.PID, "imagePlot_")

            Call PictureBox1.BackgroundImage.SaveAs(temp)
            Call Process.Start(temp)
        End If
    End Sub

    Private Sub ExportToolStripMenuItem_Click(sender As Object, e As EventArgs)
        If MyApplication.host.fileExplorer.GetSelectedNodes.Count = 0 Then
            MessageBox.Show("No chromatogram data for XIC plot, please use XIC -> Add for add data!", "No data save", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            Using file As New SaveFileDialog With {.Filter = "Mgf ASCII spectrum data(*.mgf)|*.mgf", .FileName = "XIC.mgf"}
                If file.ShowDialog = DialogResult.OK Then
                    Using OutFile As StreamWriter = file.FileName.OpenWriter()
                        Dim ppm As Double = Val(RibbonItems.PPMSpinner.DecimalValue)

                        For Each xic As NamedCollection(Of ChromatogramTick) In GetXICCollection(ppm)
                            Dim parent As New NamedValue With {.name = xic.description.Split.First, .text = xic.value.Select(Function(a) a.Intensity).Max}
                            Dim ion As New MGF.Ions With {
                                .Title = xic.name,
                                .Peaks = xic.value _
                                    .Select(Function(a)
                                                Return New ms2 With {
                                                    .mz = a.Time,
                                                    .intensity = a.Intensity,
                                                    .quantity = a.Intensity
                                                }
                                            End Function) _
                                    .ToArray,
                                .PepMass = parent,
                                .Rawfile = xic.description.GetTagValue(" ").Value
                            }

                            ion.WriteAsciiMgf(OutFile)
                        Next
                    End Using
                End If
            End Using
        End If
    End Sub

    Private Sub PictureBox1_MouseClick(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseClick
        If e.Button = MouseButtons.Right Then
            Dim p As Point = PictureBox1.PointToScreen(e.Location)
            MyApplication.host.Ribbon1.ShowContextPopup(CUInt(RibbonItems.cmdContextMap), p.X, p.Y)
        End If
    End Sub

    Private Sub CustomTabControl1_TabClosing(sender As Object, e As TabControlCancelEventArgs) Handles CustomTabControl1.TabClosing
        e.Cancel = True

        If CustomTabControl1.Controls.Count = 1 Then
            If e.TabPage Is TabPage5 Then

            Else
                CustomTabControl1.Controls.Clear()
                ShowTabPage(TabPage5)
            End If
        Else
            CustomTabControl1.Controls.Remove(e.TabPage)
            e.TabPage.Hide()
        End If
    End Sub

    Public Sub ShowTabPage(tabpage As TabPage)
        If Not CustomTabControl1.Controls.Contains(tabpage) Then
            CustomTabControl1.Controls.Add(tabpage)
        End If

        MyApplication.host.panelMain.Show(MyApplication.host.dockPanel)

        CustomTabControl1.SelectedTab = tabpage
        tabpage.Visible = True
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        RibbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active
    End Sub
End Class
