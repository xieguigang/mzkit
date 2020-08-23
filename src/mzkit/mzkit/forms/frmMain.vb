Imports System.ComponentModel
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports RibbonLib
Imports RibbonLib.Controls.Events
Imports RibbonLib.Interop
Imports Task

Public Class frmMain

    Private Sub ShowNewForm(ByVal sender As Object, ByVal e As EventArgs)
        ' Create a new instance of the child form.
        Dim ChildForm As New System.Windows.Forms.Form
        ' Make it a child of this MDI form before showing it.
        ChildForm.MdiParent = Me

        m_ChildFormNumber += 1
        ChildForm.Text = "Window " & m_ChildFormNumber

        ChildForm.Show()
    End Sub

    Private Sub OpenFile(ByVal sender As Object, ByVal e As ExecuteEventArgs)
        Using file As New OpenFileDialog With {.Filter = "Raw Data|*.mzXML;*.mzML"}
            If file.ShowDialog = DialogResult.OK Then
                Dim progress As New frmImportTaskProgress() With {.Text = $"Imports raw data [{file.FileName}]"}
                Dim showProgress As Action(Of String) = Sub(text) progress.Invoke(Sub() progress.Label1.Text = text)
                Dim task As New Task.ImportsRawData(file.FileName, showProgress, Sub() Call progress.Invoke(Sub() progress.Close()))
                Dim runTask As New Thread(AddressOf task.RunImports)

                ToolStripStatusLabel.Text = "Run Raw Data Imports"
                progress.Label2.Text = progress.Text

                Call runTask.Start()
                Call progress.ShowDialog()

                ToolStripStatusLabel.Text = "Ready!"

                'Call New frmRawViewer() With {
                '    .MdiParent = Me,
                '    .Text = file.FileName,
                '    .rawFile = task.raw
                '}.Show()
                Call TreeView1.addRawFile(task.raw)
            End If
        End Using
    End Sub

    Private Sub SaveAsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim SaveFileDialog As New SaveFileDialog
        SaveFileDialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        SaveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"

        If (SaveFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
            Dim FileName As String = SaveFileDialog.FileName
            ' TODO: Add code here to save the current contents of the form to a file.
        End If
    End Sub


    Private Sub ExitToolsStripMenuItem_Click(ByVal sender As Object, ByVal e As ExecuteEventArgs)
        Me.Close()
    End Sub

    Private Sub CutToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Use My.Computer.Clipboard to insert the selected text or images into the clipboard
    End Sub

    Private Sub CopyToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Use My.Computer.Clipboard to insert the selected text or images into the clipboard
    End Sub

    Private Sub PasteToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        'Use My.Computer.Clipboard.GetText() or My.Computer.Clipboard.GetData to retrieve information from the clipboard.
    End Sub

    Private Sub ToolBarToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Me.ToolStrip.Visible = Me.ToolBarToolStripMenuItem.Checked
    End Sub

    Private Sub StatusBarToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Me.StatusStrip.Visible = Me.StatusBarToolStripMenuItem.Checked
    End Sub

    Private Sub CascadeToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        Me.LayoutMdi(MdiLayout.Cascade)
    End Sub

    Private Sub TileVerticalToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        Me.LayoutMdi(MdiLayout.TileVertical)
    End Sub

    Private Sub TileHorizontalToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        Me.LayoutMdi(MdiLayout.TileHorizontal)
    End Sub

    Private Sub ArrangeIconsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        Me.LayoutMdi(MdiLayout.ArrangeIcons)
    End Sub

    Private Sub CloseAllToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Close all child forms of the parent.
        For Each ChildForm As Form In Me.MdiChildren
            ChildForm.Close()
        Next
    End Sub

    Private m_ChildFormNumber As Integer
    Dim ribbonItems As RibbonItems
    Private _recentItems As List(Of RecentItemsPropertySet)

    Public Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        ribbonItems = New RibbonItems(Ribbon1)

        AddHandler ribbonItems.ButtonExit.ExecuteEvent, AddressOf ExitToolsStripMenuItem_Click
        AddHandler ribbonItems.ButtonOpenRaw.ExecuteEvent, AddressOf OpenFile
        AddHandler ribbonItems.ButtonMzCalculator.ExecuteEvent, AddressOf MzCalculatorToolStripMenuItem_Click
        AddHandler ribbonItems.ButtonAbout.ExecuteEvent, AddressOf About_Click
    End Sub

    Private Sub About_Click(ByVal sender As Object, ByVal e As ExecuteEventArgs)
        Call New frmSplashScreen() With {.isAboutScreen = True, .TopMost = True}.Show()
    End Sub

    Private Sub MzCalculatorToolStripMenuItem_Click(ByVal sender As Object, ByVal e As ExecuteEventArgs)
        Call New frmCalculator().ShowDialog()
    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitRecentItems()
        InitializeFileTree()
        ToolStripStatusLabel.Text = "Ready!"
    End Sub

    Private Sub InitRecentItems()
        ' prepare list of recent items
        _recentItems = New List(Of RecentItemsPropertySet)()
        _recentItems.Add(New RecentItemsPropertySet() With {.Label = "Recent item 1", .LabelDescription = "Recent item 1 description", .Pinned = True})
        _recentItems.Add(New RecentItemsPropertySet() With {.Label = "Recent item 2", .LabelDescription = "Recent item 2 description", .Pinned = False})

        ribbonItems.RecentItems.RecentItems = _recentItems
    End Sub

    Private Sub _recentItems_ExecuteEvent(ByVal sender As Object, ByVal e As ExecuteEventArgs)
        If e.Key.PropertyKey = RibbonProperties.RecentItems Then
            ' go over recent items
            Dim objectArray() As Object = CType(e.CurrentValue.PropVariant.Value, Object())
            For i As Integer = 0 To objectArray.Length - 1
                Dim propertySet As IUISimplePropertySet = TryCast(objectArray(i), IUISimplePropertySet)

                If propertySet IsNot Nothing Then
                    Dim propLabel As PropVariant
                    propertySet.GetValue(RibbonProperties.Label, propLabel)
                    Dim label As String = CStr(propLabel.Value)

                    Dim propLabelDescription As PropVariant
                    propertySet.GetValue(RibbonProperties.LabelDescription, propLabelDescription)
                    Dim labelDescription As String = CStr(propLabelDescription.Value)

                    Dim propPinned As PropVariant
                    propertySet.GetValue(RibbonProperties.Pinned, propPinned)
                    Dim pinned As Boolean = CBool(propPinned.Value)

                    ' update pinned value
                    _recentItems(i).Pinned = pinned
                End If
            Next i
        ElseIf e.Key.PropertyKey = RibbonProperties.SelectedItem Then
            ' get selected item index
            Dim selectedItem As UInteger = CUInt(e.CurrentValue.PropVariant.Value)

            ' get selected item label
            Dim propLabel As PropVariant
            e.CommandExecutionProperties.GetValue(RibbonProperties.Label, propLabel)
            Dim label As String = CStr(propLabel.Value)

            ' get selected item label description
            Dim propLabelDescription As PropVariant
            e.CommandExecutionProperties.GetValue(RibbonProperties.LabelDescription, propLabelDescription)
            Dim labelDescription As String = CStr(propLabelDescription.Value)

            ' get selected item pinned value
            Dim propPinned As PropVariant
            e.CommandExecutionProperties.GetValue(RibbonProperties.Pinned, propPinned)
            Dim pinned As Boolean = CBool(propPinned.Value)
        End If
    End Sub

    Sub InitializeFileTree()
        If TreeView1.LoadRawFileCache = 0 Then
            ' MessageBox.Show($"It seems that you don't have any raw file opended. {vbCrLf}You could open raw file through [File] -> [Open Raw File].", "Tips", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub TreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TreeView1.AfterSelect
        If TypeOf e.Node.Tag Is Task.Raw Then
            ' 原始文件节点
        ElseIf TypeOf e.Node.Tag Is ms2() Then
            ' TIC 图绘制
            Dim raw = DirectCast(e.Node.Tag, ms2())
            Dim TIC As New NamedCollection(Of ChromatogramTick) With {
                .name = $"m/z {raw.Select(Function(m) m.mz).Min.ToString("F3")} - {raw.Select(Function(m) m.mz).Max.ToString("F3")}",
                .value = raw.Select(Function(a) New ChromatogramTick With {.Time = Val(a.Annotation), .Intensity = a.intensity}).ToArray
            }

            PictureBox1.BackgroundImage = TIC.TICplot.AsGDIImage
        ElseIf e.Node.Tag Is Nothing AndAlso e.Node.Text = "TIC" Then
            Dim raw = TreeView1.CurrentRawFile.raw
            Dim TIC As New NamedCollection(Of ChromatogramTick) With {.name = "TIC", .value = raw.scans.Where(Function(a) a.mz = 0R).Select(Function(m) New ChromatogramTick With {.Time = m.rt, .Intensity = m.intensity}).ToArray}

            PictureBox1.BackgroundImage = TIC.TICplot.AsGDIImage
        Else
            ' scan节点
            Dim raw As Task.Raw = e.Node.Parent.Tag
            Dim scanId As String = e.Node.Text
            Dim scanData As LibraryMatrix

            Using cache As New netCDFReader(raw.cache)
                Dim data As CDFData = cache.getDataVariable(cache.getDataVariableEntry(scanId))
                Dim attrs = cache.getDataVariableEntry(scanId).attributes

                scanData = New LibraryMatrix With {
                    .name = scanId,
                    .centroid = False,
                    .ms2 = data.numerics.AsMs2.ToArray
                }

                Dim draw As Image = scanData.MirrorPlot.AsGDIImage

                PictureBox1.BackgroundImage = draw
            End Using
        End If
    End Sub

    Private Sub frmMain_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Call TreeView1.SaveRawFileCache
    End Sub

    Private Sub ShowTICToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowTICToolStripMenuItem.Click
        Dim raw = TreeView1.CurrentRawFile

        ShowTICToolStripMenuItem.Checked = Not ShowTICToolStripMenuItem.Checked

        If raw.raw Is Nothing Then
            Return
        Else
            raw.tree.Nodes.Clear()
        End If

        If ShowTICToolStripMenuItem.Checked Then
            raw.tree.Nodes.Add(New TreeNode("TIC"))

            Using cache As New netCDFReader(raw.raw.cache)
                Dim progress As New frmImportTaskProgress() With {.Text = $"Reading TIC raw data [{raw.raw.source}]"}
                Dim showProgress As Action(Of String) = Sub(text) progress.Invoke(Sub() progress.Label1.Text = text)
                Dim mzgroups As NamedCollection(Of ms2)() = {}
                Dim runTask As New Thread(
                        Sub()
                            Dim ms1n = raw.raw.scans.Where(Function(a) a.mz = 0R).Count
                            Dim i As i32 = 1
                            Dim allMz As New List(Of ms2)
                            Dim mztemp As ms2()

                            For Each scan In raw.raw.scans
                                If scan.mz = 0 Then
                                    Dim entry = cache.getDataVariableEntry(scan.id)
                                    Dim rt As String = entry.attributes.Where(Function(a) a.name = "retentionTime").FirstOrDefault?.value

                                    mztemp = cache.getDataVariable(entry).numerics.AsMs2.ToArray

                                    For i2 As Integer = 0 To mztemp.Length - 1
                                        mztemp(i2).Annotation = rt
                                    Next

                                    allMz.AddRange(mztemp)
                                    showProgress($"[{++i}/{ms1n}] {scan.id}")
                                End If
                            Next

                            showProgress("Run m/z group....")
                            mzgroups = allMz _
                                .GroupBy(Function(mz) mz.mz, Tolerance.DeltaMass(5)) _
                                .Select(Function(a)
                                            Dim max = a.Select(Function(m) m.intensity).Max

                                            Return New NamedCollection(Of ms2) With {.value = a.value.Where(Function(m) m.intensity / max >= 0.05).OrderBy(Function(m) Val(m.Annotation)).ToArray}
                                        End Function) _
                                .ToArray
                            progress.Invoke(Sub() progress.Close())
                        End Sub)

                ToolStripStatusLabel.Text = "Run Raw Data Imports"
                progress.Label2.Text = progress.Text

                Call runTask.Start()
                Call progress.ShowDialog()

                For Each mzblock In mzgroups
                    Dim range As New DoubleRange(mzblock.Select(Function(m) m.mz))

                    raw.tree.Nodes.Add(New TreeNode($"m/z {range.Min.ToString("F3")} - {range.Max.ToString("F3")}") With {.Tag = mzblock.ToArray})
                Next

                ToolStripStatusLabel.Text = "Ready!"
            End Using
        Else
            Call applyLevelFilter()
        End If
    End Sub

    Private Sub SaveImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveImageToolStripMenuItem.Click
        If Not PictureBox1.BackgroundImage Is Nothing Then
            Using file As New SaveFileDialog With {.Filter = "image(*.png)|*.png"}
                If file.ShowDialog = DialogResult.OK Then
                    Call PictureBox1.BackgroundImage.SaveAs(file.FileName)
                    Call Process.Start(file.FileName)
                End If
            End Using
        End If
    End Sub

    Private Sub applyLevelFilter()
        Dim raw = TreeView1.CurrentRawFile

        If Not raw.raw Is Nothing Then
            raw.tree.Nodes.Clear()
            raw.tree.addRawFile(raw.raw, MS1ToolStripMenuItem.Checked, MS2ToolStripMenuItem.Checked)
        End If
    End Sub

    Private Sub MS1ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MS1ToolStripMenuItem.Click
        MS1ToolStripMenuItem.Checked = Not MS1ToolStripMenuItem.Checked
        applyLevelFilter()
    End Sub

    Private Sub MS2ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MS2ToolStripMenuItem.Click
        MS2ToolStripMenuItem.Checked = Not MS2ToolStripMenuItem.Checked
        applyLevelFilter()
    End Sub

    Private Sub DeleteFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteFileToolStripMenuItem.Click

    End Sub
End Class
