#Region "Microsoft.VisualBasic::3d0967576022f3ee815431efa3e70337, src\mzkit\mzkit\pages\PageMzkitTools.vb"

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
'     Function: getRawCache, getSelectedIonSpectrums, GetXICCollection, getXICMatrix, missingCacheFile
'               rawTIC, relativeInto
' 
'     Sub: applyLevelFilter, ClearToolStripMenuItem_Click, CustomTabControl1_TabClosing, CustomToolStripMenuItem_Click, DataGridView1_CellContentClick
'          DefaultToolStripMenuItem_Click, ExportExactMassSearchTable, ExportToolStripMenuItem_Click, GeneralFlavoneToolStripMenuItem_Click, ImportsRaw
'          InitializeFileTree, ListBox1_SelectedIndexChanged, MolecularNetworkingToolStripMenuItem_Click, MS1ToolStripMenuItem_Click, MS2ToolStripMenuItem_Click
'          NatureProductToolStripMenuItem_Click, PageMzkitTools_Load, PictureBox1_Click, PictureBox1_DoubleClick, PictureBox1_MouseClick
'          Ribbon_Load, runMzSearch, SaveFileCache, SaveImageToolStripMenuItem_Click, SaveMatrixToolStripMenuItem_Click
'          SearchByMz, SearchFormulaToolStripMenuItem_Click, searchInFileByMz, SearchInFileToolStripMenuItem_Click, setCurrentFile
'          (+3 Overloads) showMatrix, ShowPropertyWindow, showSpectrum, ShowTabPage, ShowTICToolStripMenuItem_Click
'          ShowXICToolStripMenuItem_Click, SmallMoleculeToolStripMenuItem_Click, TIC, TreeView1_AfterSelect
' 
' /********************************************************************************/

#End Region

Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports mzkit.My
Imports RibbonLib
Imports RibbonLib.Interop
Imports Task
Imports stdNum = System.Math

Public Class PageMzkitTools

    Dim RibbonItems As RibbonItems
    Dim matrix As [Variant](Of ms2(), ChromatogramTick(), SSM2MatrixFragment())
    Dim matrixName As String

    Friend _ribbonExportDataContextMenuStrip As ExportData

    Public Sub Ribbon_Load(ribbon As Ribbon)
        _ribbonExportDataContextMenuStrip = New ExportData(ribbon, RibbonItems.cmdContextMap)
    End Sub

    'Private Function missingCacheFile(raw As Raw) As DialogResult
    '    Dim options As DialogResult = MessageBox.Show($"The specific raw data cache is missing, run imports again?{vbCrLf}{raw.source.GetFullPath}", $"[{raw.source.FileName}] Cache Not Found!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)

    '    If options = DialogResult.OK Then
    '        Dim newRaw = getRawCache(raw.source)

    '        For i As Integer = 0 To TreeView1.Nodes.Count - 1
    '            If TreeView1.Nodes(i).Tag Is raw Then
    '                TreeView1.Nodes(i).Tag = newRaw
    '            End If
    '        Next

    '        MyApplication.host.showStatusMessage("Ready!")
    '        MyApplication.host.ToolStripStatusLabel2.Text = TreeView1.GetTotalCacheSize
    '    End If

    '    Return options
    'End Function

    Public Sub ShowPage()
        MyApplication.host.ShowPage(Me)
    End Sub

    Public Sub showScatter(raw As Raw)
        If Not raw.cacheFileExists Then
            MessageBox.Show("Sorry, can not view file data, the cache file is missing...", "Cache Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim spinner As New frmProgressSpinner
        Dim task As New Thread(
            Sub()
                Dim image As Image

                If raw.scatter.FileLength < 0 Then
                    raw.scatter = App.AppSystemTemp & "/" & raw.source.GetFullPath.MD5 & ".scatter"
                    raw.scatter = raw.scatter.GetFullPath
                    image = raw.DrawScatter
                    image.SaveAs(raw.scatter)
                Else
                    image = raw.scatter.LoadImage
                End If

                Me.Invoke(Sub() PictureBox1.BackgroundImage = image)
                spinner.Invoke(Sub() Call spinner.Close())
            End Sub)

        Call task.Start()
        Call spinner.ShowDialog()

        MyApplication.host.ShowPage(Me)
        MyApplication.host.Invoke(Sub() RibbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.NotAvailable)
    End Sub

    Private Sub PageMzkitTools_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim host = MyApplication.host
        RibbonItems = host.ribbonItems

        ' Call InitializeFileTree()
        Call Globals.sharedProgressUpdater("Attatch Command Events...")

        'AddHandler TreeView1.AfterSelect, AddressOf TreeView1_AfterSelect
        'AddHandler host.fileExplorer.Button1.Click, Sub(obj, evt) Call SearchByMz(host.fileExplorer.TextBox2.Text)

        'AddHandler host.fileExplorer.ShowTICToolStripMenuItem.Click, AddressOf ShowTICToolStripMenuItem_Click
        'AddHandler host.fileExplorer.ShowXICToolStripMenuItem.Click, AddressOf ShowXICToolStripMenuItem_Click

        'AddHandler host.fileExplorer.ClearToolStripMenuItem.Click, AddressOf ClearToolStripMenuItem_Click
        'AddHandler host.fileExplorer.ExportToolStripMenuItem.Click, AddressOf ExportToolStripMenuItem_Click

        'AddHandler host.fileExplorer.MS1ToolStripMenuItem.Click, AddressOf MS1ToolStripMenuItem_Click
        'AddHandler host.fileExplorer.MS2ToolStripMenuItem.Click, AddressOf MS2ToolStripMenuItem_Click

        'AddHandler host.fileExplorer.MolecularNetworkingToolStripMenuItem.Click, AddressOf MolecularNetworkingToolStripMenuItem_Click

        'AddHandler host.fileExplorer.SearchInFileToolStripMenuItem.Click, AddressOf SearchInFileToolStripMenuItem_Click
        'AddHandler host.fileExplorer.CustomToolStripMenuItem.Click, AddressOf CustomToolStripMenuItem_Click
        'AddHandler host.fileExplorer.DefaultToolStripMenuItem.Click, AddressOf DefaultToolStripMenuItem_Click
        'AddHandler host.fileExplorer.SmallMoleculeToolStripMenuItem.Click, AddressOf SmallMoleculeToolStripMenuItem_Click
        'AddHandler host.fileExplorer.NatureProductToolStripMenuItem.Click, AddressOf NatureProductToolStripMenuItem_Click
        'AddHandler host.fileExplorer.GeneralFlavoneToolStripMenuItem.Click, AddressOf GeneralFlavoneToolStripMenuItem_Click
    End Sub

    Dim currentMatrix As [Variant](Of ms2(), ChromatogramTick())

    Friend Sub showSpectrum(scanId As String, raw As Raw)
        If raw.cacheFileExists Then
            Dim prop As SpectrumProperty = Nothing
            Dim scanData As LibraryMatrix = raw.GetSpectrum(scanId, Globals.Settings.viewer.GetMethod, prop)

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


            ' PropertyGrid1.SelectedObject = prop
            'PropertyGrid1.Refresh()
            propertyWin.propertyGrid.SelectedObject = prop
            propertyWin.propertyGrid.Refresh()

            PictureBox1.BackgroundImage = draw
            ShowTabPage(TabPage5)

            MyApplication.host.ShowPropertyWindow()
        Else
            ' Call missingCacheFile(raw)
        End If
    End Sub

    Public Sub setCurrentFile()
        'If TreeView1.Nodes.Count = 0 Then
        '    MyApplication.host.showStatusMessage("No raw file opened.")
        '    Return
        'End If

        'With TreeView1.CurrentRawFile.raw
        '    Static selectedFile As String

        '    If selectedFile <> MyApplication.host.ToolStripStatusLabel1.Text Then
        '        selectedFile = $"{ .source.FileName} [{ .numOfScans} scans]"
        '        MyApplication.host.showStatusMessage(selectedFile)
        '        MyApplication.host.rawFeaturesList.ListBox1.Items.Clear()
        '    End If

        '    MyApplication.host.Text = $"BioNovoGene Mzkit [{ .source.GetFullPath}]"
        'End With

        'If Not TreeView1.CurrentRawFile.raw.cacheFileExists Then
        '    TreeView1.SelectedNode.ImageIndex = 1
        '    TreeView1.SelectedNode.SelectedImageIndex = 1
        'End If
    End Sub

    Private Function rawTIC(raw As Raw, isBPC As Boolean) As NamedCollection(Of ChromatogramTick)
        Dim TIC As New NamedCollection(Of ChromatogramTick) With {
            .name = $"{If(isBPC, "BPC", "TIC")} [{raw.source.FileName}]",
            .value = raw.scans _
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

    Public Sub TIC(rawList As IEnumerable(Of Raw), Optional isBPC As Boolean = False)
        Dim TICList As New List(Of NamedCollection(Of ChromatogramTick))

        For Each raw As Raw In rawList
            TICList.Add(rawTIC(raw, isBPC))
        Next

        showMatrix(TICList(Scan0).value, TICList(Scan0).name)

        PictureBox1.BackgroundImage = ChromatogramPlot.TICplot(
            ionData:=TICList.ToArray,
            colorsSchema:=Globals.GetColors,
            fillCurve:=Globals.Settings.viewer.fill
        ).AsGDIImage

        MyApplication.host.ShowPage(Me)
    End Sub

    Public Sub TIC(isBPC As Boolean)
        Dim rawList As Raw() = MyApplication.fileExplorer.GetSelectedRaws.ToArray

        If rawList.Length = 0 Then
            MyApplication.host.showStatusMessage("No file data selected for TIC plot...")
        Else
            Call TIC(rawList, isBPC)
        End If
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
        'Dim raw = TreeView1.CurrentRawFile

        'If Not raw.raw Is Nothing Then
        '    raw.tree.Nodes.Clear()
        '    raw.tree.addRawFile(raw.raw, MyApplication.host.fileExplorer.MS1ToolStripMenuItem.Checked, MyApplication.host.fileExplorer.MS2ToolStripMenuItem.Checked)
        'End If
    End Sub

    Private Sub MS1ToolStripMenuItem_Click(sender As Object, e As EventArgs)
        ' MyApplication.host.fileExplorer.MS1ToolStripMenuItem.Checked = Not MyApplication.host.fileExplorer.MS1ToolStripMenuItem.Checked
        applyLevelFilter()
    End Sub

    Private Sub MS2ToolStripMenuItem_Click(sender As Object, e As EventArgs)
        ' MyApplication.host.fileExplorer.MS2ToolStripMenuItem.Checked = Not MyApplication.host.fileExplorer.MS2ToolStripMenuItem.Checked
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
            Dim ppm As Double = MyApplication.host.GetPPMError()
            Dim raw As Raw '= TreeView1.CurrentRawFile.raw

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
        Dim ppm As Double = MyApplication.host.GetPPMError()
        Dim raw = MyApplication.fileExplorer.CurrentRawFile

        ' Call MyApplication.host.rawFeaturesList.searchInFileByMz(mz, ppm, raw)
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
        'Dim current = fileExplorer.CurrentRawFile
        'Dim node = TreeView1.SelectedNode

        'If Not node Is Nothing AndAlso current.raw.cacheFileExists Then
        '    Dim mz = current.raw.GetMs2Scans.Where(Function(scan) scan.id = node.Text).FirstOrDefault

        '    If Not mz Is Nothing AndAlso mz.mz > 0 Then
        '        Call searchAction(mz.mz)
        '    End If
        'End If
    End Sub

    Friend Sub MolecularNetworkingTool(progress As frmTaskProgress, similarityCutoff As Double)
        Thread.Sleep(1000)

        progress.ShowProgressTitle("Load Scan data")
        progress.ShowProgressDetails("loading cache ms2 scan data...")

        Dim raw = getSelectedIonSpectrums(AddressOf progress.ShowProgressTitle).ToArray

        If raw.Length = 0 Then
            MyApplication.host.showStatusMessage("No spectrum data, please select a file or some spectrum...", My.Resources.StatusAnnotations_Warning_32xLG_color)
            progress.Invoke(Sub() progress.Close())
            Return
        End If

        Dim protocol As New Protocols(
                    ms1_tolerance:=Tolerance.PPM(15),
                    ms2_tolerance:=Tolerance.DeltaMass(0.3),
                    treeIdentical:=Globals.Settings.network.treeNodeIdentical,
                    treeSimilar:=Globals.Settings.network.treeNodeSimilar,
                    intoCutoff:=Globals.Settings.viewer.GetMethod
                )
        Dim progressMsg As Action(Of String) = AddressOf progress.ShowProgressTitle

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

        '        progress.Invoke(Sub() progress.ShowProgressTitle (scan.id)
        '        nodes.Add(run.Last.lib_guid, scan)
        '    Next
        'End Using

        progress.ShowProgressTitle("run molecular networking....")

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
                    .ToArray   ' MoleculeNetworking.CreateMatrix(run, 0.8, Tolerance.DeltaMass(0.3), Sub(msg) progress.Invoke(Sub() progress.ShowProgressDetails ( msg)).ToArray

        progress.ShowProgressDetails("run family clustering....")

        If net.Length < 3 Then
            Call MyApplication.host.showStatusMessage("the ions data is not enough for create network!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Dim kn As Integer

            If net.Length > 9 Then
                kn = 9
            Else
                kn = CInt(net.Length / 2)
            End If

            Dim clusters = net.ToKMeansModels.Kmeans(expected:=kn, debug:=False)
            Dim rawLinks = links.ToDictionary(Function(a) a.Name, Function(a) a.Value)

            progress.ShowProgressDetails("initialize result output...")

            MyApplication.host.Invoke(
                        Sub()
                            Call MyApplication.host.mzkitMNtools.loadNetwork(clusters, protocol, rawLinks, similarityCutoff)
                            Call MyApplication.host.ShowPage(MyApplication.host.mzkitMNtools)
                        End Sub)
        End If

        progress.Invoke(Sub() progress.Close())
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        If e.ColumnIndex = Scan0 AndAlso e.RowIndex >= 0 Then
            Dim scanId As String = DataGridView1.Rows(e.RowIndex).Cells(0).Value?.ToString

            If Not scanId.StringEmpty Then
                ' Call showSpectrum(scanId, TreeView1.CurrentRawFile.raw)
            End If
        End If
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

    Public Sub showMatrix(matrix As ChromatogramTick(), name As String)
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

    Public Sub ShowXIC(ppm As Double, plotTIC As NamedCollection(Of ChromatogramTick), getXICCollection As Func(Of Double, IEnumerable(Of NamedCollection(Of ChromatogramTick))), maxY As Double)
        Dim progress As New frmProgressSpinner
        Dim plotImage As Image = Nothing
        Dim relative As Boolean = relativeInto()

        Call New Thread(
            Sub()
                Dim XICPlot As New List(Of NamedCollection(Of ChromatogramTick))

                If Not plotTIC.IsEmpty Then
                    XICPlot.Add(plotTIC)
                End If

                XICPlot.AddRange(getXICCollection(ppm))

                If XICPlot.Count = 0 Then
                    Call MyApplication.host.showStatusMessage("No XIC ions data for generate plot!", My.Resources.StatusAnnotations_Warning_32xLG_color)
                Else
                    plotImage = XICPlot.ToArray.TICplot(
                        intensityMax:=maxY,
                        isXIC:=True,
                        colorsSchema:=Globals.GetColors,
                        fillCurve:=Globals.Settings.viewer.fill
                    ).AsGDIImage
                End If

                Call progress.Invoke(Sub() progress.Close())
            End Sub).Start()

        progress.ShowDialog()

        PictureBox1.BackgroundImage = plotImage
        ShowTabPage(TabPage5)
    End Sub

    Friend Iterator Function getSelectedIonSpectrums(progress As Action(Of String)) As IEnumerable(Of PeakMs2)
        Dim raw = MyApplication.featureExplorer.CurrentRawFile

        Using cache As New netCDFReader(raw.ms2_cache)

            For Each ionNode As TreeNode In MyApplication.featureExplorer.GetSelectedNodes.Where(Function(a) TypeOf a.Tag Is ScanEntry)
                Dim scanId As String = ionNode.Text
                Dim entry = cache.getDataVariableEntry(scanId)
                Dim mztemp = cache.getDataVariable(entry).numerics.AsMs2.ToArray
                Dim attrs = cache.getDataVariableEntry(scanId).attributes
                Dim info As New SpectrumProperty(scanId, raw.source.FileName, attrs)
                Dim guid As String = $"{raw.source.FileName}#{scanId}"

                If Not progress Is Nothing Then
                    Call progress(guid)
                End If

                Yield New PeakMs2 With {
                    .mz = info.precursorMz,
                    .scan = 0,
                    .activation = info.activationMethod,
                    .collisionEnergy = info.collisionEnergy,
                    .file = raw.source.FileName,
                    .lib_guid = guid,
                    .mzInto = mztemp,
                    .precursor_type = "n/a",
                    .rt = info.retentionTime
                }
            Next

        End Using
    End Function

    Private Function relativeInto() As Boolean
        Return False ' MyApplication.host.ribbonItems.CheckBoxXICRelative.BooleanValue
    End Function

    Friend Function getXICMatrix(raw As Raw, scanId As String, ppm As Double, relativeInto As Boolean) As NamedCollection(Of ChromatogramTick)
        Dim ms2 As ScanEntry = raw.FindMs2Scan(scanId)
        Dim name As String

        If ms2 Is Nothing OrElse ms2.mz = 0.0 Then
            MyApplication.host.showStatusMessage("XIC plot is not avaliable for MS1 parent!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return Nothing
        Else
            MyApplication.host.showStatusMessage(name, Nothing)
        End If

        Dim selectedIons = raw _
            .GetMs2Scans _
            .Where(Function(a) PPMmethod.PPM(a.mz, ms2.mz) <= ppm) _
            .ToArray
        Dim XIC As ChromatogramTick() = selectedIons _
            .Select(Function(a)
                        Return New ChromatogramTick With {
                            .Time = a.rt,
                            .Intensity = a.XIC
                        }
                    End Function) _
            .ToArray
        Dim mzmin = Aggregate ion In selectedIons Into Min(ion.mz)
        Dim mzmax = Aggregate ion In selectedIons Into Max(ion.mz)

        name = $"XIC [m/z={ms2.mz.ToString("F4")}] [mzmin={mzmin.ToString("F4")}, mzmax={mzmax.ToString("F4")}]"

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
        ' MyApplication.host.fileExplorer.Clear()
        '  MyApplication.host.fileExplorer.ClearToolStripMenuItem.Text = "Clear"
    End Sub

    Private Sub PictureBox1_DoubleClick(sender As Object, e As EventArgs) Handles PictureBox1.DoubleClick
        If Not PictureBox1.BackgroundImage Is Nothing Then
            Dim temp As String = App.GetAppSysTempFile(".png", App.PID, "imagePlot_")

            Call PictureBox1.BackgroundImage.SaveAs(temp)
            Call Process.Start(temp)
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
