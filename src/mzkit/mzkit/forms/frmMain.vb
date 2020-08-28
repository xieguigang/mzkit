#Region "Microsoft.VisualBasic::9f4b4c3e87676f6054cdd49f5172d821, src\mzkit\mzkit\forms\frmMain.vb"

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

' Class frmMain
' 
'     Constructor: (+1 Overloads) Sub New
'     Sub: _recentItems_ExecuteEvent, _uiCollectionChangedEvent_ChangedEvent, About_Click, addPage, CopyToolStripMenuItem_Click
'          CutToolStripMenuItem_Click, ExitToolsStripMenuItem_Click, FormulaSearchToolToolStripMenuItem_Click, frmMain_Closing, frmMain_Load
'          InitializeFormulaProfile, InitRecentItems, InitSpinner, MoleculeNetworkingToolStripMenuItem_Click, MzCalculatorToolStripMenuItem_Click
'          NavBack_Click, OpenFile, PasteToolStripMenuItem_Click, RawFileViewerToolStripMenuItem_Click, SaveAsToolStripMenuItem_Click
'          saveCacheList, ShowPage, StatusBarToolStripMenuItem_Click, ToolBarToolStripMenuItem_Click
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports mzkit.DockSample
Imports RibbonLib
Imports RibbonLib.Controls.Events
Imports RibbonLib.Interop
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmMain

    Dim pages As New List(Of Control)

    Friend mzkitTool As New PageMzkitTools
    Friend mzkitSettings As New PageSettings
    Friend mzkitSearch As New PageMzSearch
    Friend mzkitCalculator As New PageMzCalculator
    Friend mzkitMNtools As New PageMoleculeNetworking
    Friend mzkitRsharpScripting As New PageScripting


    Friend TreeView1 As TreeView

    Dim nav As New Stack(Of Control)

    Friend Sub ShowPage(page As Control, Optional pushStack As Boolean = True)
        For Each page2 In pages
            If Not page Is page2 Then
                page2.Hide()
            End If
        Next

        If pushStack Then
            nav.Push(page)
        End If

        page.Show()
    End Sub

    Private Sub OpenFile(ByVal sender As Object, ByVal e As ExecuteEventArgs)
        Call mzkitTool.ImportsRaw()
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

    Friend ribbonItems As RibbonItems
    Friend recentItems As List(Of RecentItemsPropertySet)

    Dim _uiCollectionChangedEvent As UICollectionChangedEvent

    Public Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        ribbonItems = New RibbonItems(Ribbon1)

        AddHandler ribbonItems.ButtonExit.ExecuteEvent, AddressOf ExitToolsStripMenuItem_Click
        AddHandler ribbonItems.ButtonOpenRaw.ExecuteEvent, AddressOf OpenFile
        AddHandler ribbonItems.ButtonAbout.ExecuteEvent, AddressOf About_Click
        AddHandler ribbonItems.ButtonPageNavBack.ExecuteEvent, AddressOf NavBack_Click

        AddHandler ribbonItems.ButtonMzCalculator.ExecuteEvent, Sub(sender, e) Call ShowPage(mzkitCalculator)
        AddHandler ribbonItems.ButtonSettings.ExecuteEvent, Sub(sender, e) Call ShowPage(mzkitSettings)
        AddHandler ribbonItems.ButtonMzSearch.ExecuteEvent, Sub(sender, e) Call ShowPage(mzkitSearch)
        AddHandler ribbonItems.ButtonRsharp.ExecuteEvent, Sub(sender, e) Call ShowPage(mzkitRsharpScripting)

        AddHandler ribbonItems.ButtonDropA.ExecuteEvent, Sub(sender, e) ShowPage(mzkitTool)
        AddHandler ribbonItems.ButtonDropB.ExecuteEvent, Sub(sender, e) ShowPage(mzkitCalculator)
        AddHandler ribbonItems.ButtonDropC.ExecuteEvent, Sub(sender, e) ShowPage(mzkitSearch)
        AddHandler ribbonItems.ButtonDropD.ExecuteEvent, Sub(sender, e) ShowPage(mzkitMNtools)

        AddHandler ribbonItems.ButtonCalculatorExport.ExecuteEvent, Sub(sender, e) Call mzkitCalculator.ExportToolStripMenuItem_Click()
        AddHandler ribbonItems.ButtonExactMassSearchExport.ExecuteEvent, Sub(sender, e) Call mzkitTool.ExportExactMassSearchTable()
        AddHandler ribbonItems.ButtonSave.ExecuteEvent, Sub(sender, e) Call saveCacheList()
        AddHandler ribbonItems.ButtonNetworkExport.ExecuteEvent, Sub(sender, e) Call mzkitMNtools.saveNetwork()
        AddHandler ribbonItems.ButtonFormulaSearchExport.ExecuteEvent, Sub(sender, e) Call mzkitSearch.SaveSearchResultTable()

        AddHandler ribbonItems.ButtonBioDeep.ExecuteEvent, Sub(sender, e) Call Process.Start("http://www.biodeep.cn/")
        AddHandler ribbonItems.ButtonLicense.ExecuteEvent, Sub(sender, e) Call New frmLicense().ShowDialog()

        AddHandler ribbonItems.ButtonExportImage.ExecuteEvent, Sub(sender, e) Call mzkitTool.SaveImageToolStripMenuItem_Click()
        AddHandler ribbonItems.ButtonExportMatrix.ExecuteEvent, Sub(sender, e) Call mzkitTool.SaveMatrixToolStripMenuItem_Click()

        _uiCollectionChangedEvent = New UICollectionChangedEvent()

        InitSpinner()
        InitializeFormulaProfile()
    End Sub

    Private Sub saveCacheList()
        TreeView1.SaveRawFileCache
        ToolStripStatusLabel1.Text = "The raw file cache data was saved!"
    End Sub

    Private Sub MoleculeNetworkingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MoleculeNetworkingToolStripMenuItem.Click
        Call ShowPage(mzkitMNtools)
    End Sub

    Private Sub RawFileViewerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RawFileViewerToolStripMenuItem.Click
        Call ShowPage(mzkitTool)
    End Sub

    Private Sub MzCalculatorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MzCalculatorToolStripMenuItem.Click
        Call ShowPage(mzkitCalculator)
    End Sub

    Private Sub FormulaSearchToolToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FormulaSearchToolToolStripMenuItem.Click
        Call ShowPage(mzkitSearch)
    End Sub

    Private Sub NavBack_Click(ByVal sender As Object, ByVal e As ExecuteEventArgs)
        If nav.Count > 0 Then
            Call ShowPage(nav.Pop, pushStack:=False)
        End If
    End Sub

    Private Sub About_Click(ByVal sender As Object, ByVal e As ExecuteEventArgs)
        Call New frmSplashScreen() With {.isAboutScreen = True, .TopMost = True}.Show()
    End Sub

    Private Sub addPage(ParamArray pageList As Control())
        For Each page As Control In pageList
            dockPanel.Controls.Add(page)
            pages.Add(page)
            page.Dock = DockStyle.Fill
        Next
    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' 20200829 因为有一组控件需要放置在这里
        ' 所以这个基础的panel需要首先进行初始化
        Call initializeVSPanel()

        InitRecentItems()
        ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active

        addPage(mzkitTool, mzkitSettings, mzkitSearch, mzkitCalculator, mzkitMNtools, mzkitRsharpScripting)
        ShowPage(mzkitTool)

        mzkitTool.Ribbon_Load(Ribbon1)

        If (Not Globals.Settings.ui Is Nothing) AndAlso
            Globals.Settings.ui.window <> FormWindowState.Minimized AndAlso
            Globals.Settings.ui.rememberWindowsLocation Then

            Me.Location = Globals.Settings.ui.getLocation
            Me.Size = Globals.Settings.ui.getSize
            Me.WindowState = Globals.Settings.ui.window

            ' Call Globals.Settings.ui.setColors(Ribbon1)
        End If

        ToolStripStatusLabel1.Text = "Ready!"
    End Sub

    Private Sub _uiCollectionChangedEvent_ChangedEvent(ByVal sender As Object, ByVal e As UICollectionChangedEventArgs)
        MessageBox.Show("Got ChangedEvent. Action = " & e.Action.ToString())
    End Sub

    Private Sub InitializeFormulaProfile()
        '  ribbonItems.ComboFormulaSearchProfile3.RepresentativeString = "XXXXXXXXXXXXXX"
        '  ribbonItems.ComboFormulaSearchProfile3.Label = "Preset Profiles:"

        '  AddHandler ribbonItems.ComboFormulaSearchProfile3.ItemsSourceReady, AddressOf InitializeFormulaProfile
    End Sub

    'Private Sub InitializeFormulaProfile(sender As Object, e As EventArgs)
    '    Dim itemsSource3 As IUICollection = ribbonItems.ComboFormulaSearchProfile3.ItemsSource

    '    MsgBox("initialize profile")

    '    itemsSource3.Clear()
    '    itemsSource3.Add(New GalleryItemPropertySet() With {.Label = FormulaSearchProfiles.Custom.Description, .CategoryID = Constants.UI_Collection_InvalidIndex})
    '    itemsSource3.Add(New GalleryItemPropertySet() With {.Label = FormulaSearchProfiles.Default.Description, .CategoryID = Constants.UI_Collection_InvalidIndex})
    '    itemsSource3.Add(New GalleryItemPropertySet() With {.Label = FormulaSearchProfiles.SmallMolecule.Description, .CategoryID = Constants.UI_Collection_InvalidIndex})
    '    itemsSource3.Add(New GalleryItemPropertySet() With {.Label = FormulaSearchProfiles.NaturalProduct.Description, .CategoryID = Constants.UI_Collection_InvalidIndex})

    '    MsgBox("initialize event")

    '    _uiCollectionChangedEvent.Attach(ribbonItems.ComboFormulaSearchProfile3.ItemsSource)
    '    AddHandler _uiCollectionChangedEvent.ChangedEvent, AddressOf _uiCollectionChangedEvent_ChangedEvent
    'End Sub

    Private Sub InitSpinner()
        Dim _spinner = ribbonItems.PPMSpinner

        If Globals.Settings.viewer Is Nothing Then
            Globals.Settings.viewer = New RawFileViewerSettings
        End If

        _spinner.MaxValue = 30D
        _spinner.MinValue = 0
        _spinner.Increment = 0.5D
        _spinner.DecimalPlaces = 1
        _spinner.DecimalValue = Globals.Settings.viewer.XIC_ppm

        _spinner.TooltipTitle = "PPM"
        _spinner.TooltipDescription = "Enter ppm error for search feature by m/z."
        _spinner.RepresentativeString = "XXXXXX"

    End Sub

    Private Sub InitRecentItems()
        ' prepare list of recent items
        recentItems = New List(Of RecentItemsPropertySet)()
        recentItems.Add(New RecentItemsPropertySet() With {.Label = "Recent item 1", .LabelDescription = "Recent item 1 description", .Pinned = True})
        recentItems.Add(New RecentItemsPropertySet() With {.Label = "Recent item 2", .LabelDescription = "Recent item 2 description", .Pinned = False})

        ribbonItems.RecentItems.RecentItems = recentItems
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
                    recentItems(i).Pinned = pinned
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

    Private Sub frmMain_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        mzkitTool.SaveFileCache()

        If Globals.Settings.ui Is Nothing Then
            Globals.Settings.ui = New UISettings
        End If

        Globals.Settings.ui = New UISettings With {
            .height = Height,
            .width = Width,
            .x = Location.X,
            .y = Location.Y,
            .window = WindowState,
            .rememberWindowsLocation = Globals.Settings.ui.rememberWindowsLocation
        }
        Globals.Settings.Save()
    End Sub

#Region "vs2015"

    Friend WithEvents dockPanel As New WeifenLuo.WinFormsUI.Docking.DockPanel
    Private vS2015LightTheme1 As New WeifenLuo.WinFormsUI.Docking.VS2015LightTheme
    Private vsToolStripExtender1 As New WeifenLuo.WinFormsUI.Docking.VisualStudioToolStripExtender
    Private ReadOnly _toolStripProfessionalRenderer As ToolStripRenderer = New ToolStripProfessionalRenderer()

    Friend fileExplorer As New frmFileTree
    Friend searchList As New frmSearchList
    Friend output As New DummyOutputWindow

    Private Sub initializeVSPanel()
        PanelBase.Controls.Add(Me.dockPanel)

        Me.dockPanel.Dock = DockStyle.Fill
        Me.dockPanel.DockBackColor = System.Drawing.Color.FromArgb(CType(CType(41, Byte), Integer), CType(CType(57, Byte), Integer), CType(CType(85, Byte), Integer))
        Me.dockPanel.DockBottomPortion = 150.0R
        Me.dockPanel.DockLeftPortion = 200.0R
        Me.dockPanel.DockRightPortion = 200.0R
        Me.dockPanel.DockTopPortion = 150.0R
        Me.dockPanel.Font = New System.Drawing.Font("Tahoma", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, CType(0, Byte))

        Me.dockPanel.Name = "dockPanel"
        Me.dockPanel.RightToLeftLayout = True
        Me.dockPanel.ShowAutoHideContentOnHover = False

        Me.dockPanel.TabIndex = 0

        Call SetSchema(Nothing, Nothing)

        fileExplorer.Show(dockPanel)
        fileExplorer.DockState = DockState.DockLeftAutoHide
        TreeView1 = fileExplorer.TreeView1

        searchList.Show(dockPanel)
        searchList.DockState = DockState.DockLeftAutoHide

        output.Show(dockPanel)
        output.DockState = DockState.DockBottomAutoHide
    End Sub

    Private Sub SetSchema(ByVal sender As Object, ByVal e As EventArgs)
        'If sender Is menuItemSchemaVS2005 Then
        '    dockPanel.Theme = vS2005Theme1
        '    EnableVSRenderer(VisualStudioToolStripExtender.VsVersion.Vs2005, vS2005Theme1)
        'ElseIf sender Is menuItemSchemaVS2015Blue Then
        '    dockPanel.Theme = vS2015BlueTheme1
        '    EnableVSRenderer(VisualStudioToolStripExtender.VsVersion.Vs2015, vS2015BlueTheme1)
        'ElseIf sender Is menuItemSchemaVS2015Light Then
        dockPanel.Theme = vS2015LightTheme1
        EnableVSRenderer(VisualStudioToolStripExtender.VsVersion.Vs2015, vS2015LightTheme1)
        'ElseIf sender Is menuItemSchemaVS2015Dark Then
        'dockPanel.Theme = vS2015DarkTheme1
        'EnableVSRenderer(VisualStudioToolStripExtender.VsVersion.Vs2015, vS2015DarkTheme1)
        'End If

        If dockPanel.Theme.ColorPalette IsNot Nothing Then
            StatusStrip.BackColor = dockPanel.Theme.ColorPalette.MainWindowStatusBarDefault.Background
        End If
    End Sub

    Private Sub EnableVSRenderer(ByVal version As VisualStudioToolStripExtender.VsVersion, ByVal theme As ThemeBase)
        ' vsToolStripExtender1.SetStyle(MainMenu, version, theme)
        ' vsToolStripExtender1.SetStyle(ToolBar, version, theme)
        vsToolStripExtender1.SetStyle(StatusStrip, version, theme)
    End Sub

    Private Sub frmMain_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        Ribbon1.Refresh()
    End Sub


#End Region

End Class
