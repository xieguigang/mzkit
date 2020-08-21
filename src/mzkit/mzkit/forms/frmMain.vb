Imports System.ComponentModel
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Imaging
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
                Dim progress As New frmProgress() With {.Text = $"Imports raw data [{file.FileName}]"}
                Dim showProgress As Action(Of String) = Sub(text) progress.Invoke(Sub() progress.Label1.Text = text)
                Dim task As New Task.ImportsRawData(file.FileName, showProgress, Sub() Call progress.Invoke(Sub() progress.Close()))
                Dim runTask As New Thread(AddressOf task.RunImports)

                ToolStripStatusLabel.Text = "Run Raw Data Imports"

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
        Else
            ' scan节点
            Dim raw As Task.Raw = e.Node.Parent.Tag
            Dim scanId As String = e.Node.Text
            Dim scanData As LibraryMatrix

            Using cache As New netCDFReader(raw.cache)
                Dim data As CDFData = cache.getDataVariable(cache.getDataVariableEntry(scanId))

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
End Class
