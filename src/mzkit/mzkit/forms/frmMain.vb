#Region "Microsoft.VisualBasic::ddb51a2631fc6ed823b1dfb36c3dd6b9, src\mzkit\mzkit\forms\frmMain.vb"

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
'     Sub: _recentItems_ExecuteEvent, _uiCollectionChangedEvent_ChangedEvent, About_Click, CreateNewScript, EnableVSRenderer
'          ExitToolsStripMenuItem_Click, FormulaSearchToolToolStripMenuItem_Click, frmMain_Closing, frmMain_Load, frmMain_ResizeEnd
'          ImportsFiles, InitializeFormulaProfile, initializeVSPanel, InitRecentItems, InitSpinner
'          MoleculeNetworkingToolStripMenuItem_Click, MzCalculatorToolStripMenuItem_Click, NavBack_Click, OpenFile, RawFileViewerToolStripMenuItem_Click
'          RunCurrentScript, saveCurrentFile, saveCurrentScript, SaveScript, SetSchema
'          ShowExplorer, showHelp, showLoggingWindow, ShowMzkitToolkit, ShowPage
'          ShowProperties, showRTerm, ShowSearchList, ShowSettings, showStartPage
'          showStatusMessage, Timer1_Tick, ToolStripStatusLabel2_Click
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports mzkit.Configuration
Imports mzkit.DockSample
Imports mzkit.My
Imports RibbonLib
Imports RibbonLib.Controls.Events
Imports RibbonLib.Interop
Imports Task
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmMain

    Friend mzkitTool As New PageMzkitTools With {.Text = "Raw File Viewer"}
    Friend mzkitSearch As New PageMzSearch With {.Text = "M/Z Formula De-novo Search"}
    Friend mzkitCalculator As New PageMzCalculator With {.Text = "M/Z Calculator"}
    Friend mzkitMNtools As New PageMoleculeNetworking With {.Text = "Molecular Networking"}

    Friend TreeView1 As TreeView

    Dim nav As New Stack(Of Control)

    Public Function GetPPMError() As Double
        Return Val(ribbonItems.PPMSpinner.DecimalValue)
    End Function

    Friend Sub ShowPage(page As Control, Optional pushStack As Boolean = True)
        For Each page2 In panelMain.pages
            If Not page Is page2 Then
                page2.Visible = False
                page2.Hide()
            End If
        Next

        If pushStack Then
            nav.Push(page)
        End If

        Me.Text = $"BioNovoGene Mzkit [{page.Text}]"
        page.Visible = True
        page.Show()

        panelMain.Show(dockPanel)
    End Sub

    Public Sub ShowMRMIons(file As String)
        If Not file.FileExists Then
            Call showStatusMessage($"missing raw data file '{file.GetFullPath}'!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            MRMIons.DockState = DockState.DockLeft
            MRMIons.LoadMRM(file)
        End If
    End Sub

    Public Sub ShowGCMSSIM(file As String)
        If Not file.FileExists Then
            Call showStatusMessage($"missing raw data file '{file.GetFullPath}'!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        ElseIf Not GCMSPeaks.ContainsRaw(file) Then
            GCMSPeaks.LoadRawExplorer(frmGCMS_CDFExplorer.loadCDF(file))
            VisualStudio.Dock(GCMSPeaks, DockState.DockLeft)
        Else
            GCMSPeaks.ShowRaw(file)
        End If
    End Sub

    Public Sub OpenFile()
        Using file As New OpenFileDialog With {
            .Filter = "Raw Data|*.mzXML;*.mzML|Image mzML(*.imzML)|*.imzML|GC-MS Targeted(*.cdf)|*.cdf;*.netcdf|GC-MS / LC-MS/MS Targeted(*.mzML)|*.mzML|R# Script(*.R)|*.R"
        }
            If file.ShowDialog = DialogResult.OK Then
                If file.FileName.ExtensionSuffix("R") Then
                    Call fileExplorer.AddScript(file.FileName.GetFullPath)
                    Call openRscript(file.FileName)
                ElseIf file.FileName.ExtensionSuffix("imzML") Then
                    Call showMsImaging(file.FileName)
                ElseIf file.FileName.ExtensionSuffix("mzml") AndAlso RawScanParser.IsMRMData(file.FileName) Then
                    Call ShowMRMIons(file.FileName)
                ElseIf file.FileName.ExtensionSuffix("mzml") AndAlso RawScanParser.IsSIMData(file.FileName) Then
                    Call ShowGCMSSIM(file.FileName)
                ElseIf file.FileName.ExtensionSuffix("cdf") OrElse file.FileName.ExtensionSuffix("netcdf") Then
                    Call ShowGCMSSIM(file.FileName)
                Else
                    Call fileExplorer.ImportsRaw(file.FileName)
                End If

                Globals.AddRecentFileHistory(file.FileName)
            End If
        End Using
    End Sub

    Public Sub openRscript(fileName As String)
        Dim newScript As New frmRScriptEdit With {.scriptFile = fileName}

        scriptFiles.Add(newScript)

        newScript.Show(dockPanel)
        newScript.DockState = DockState.Document
        newScript.LoadScript(fileName)
    End Sub

    ''' <summary>
    ''' imports raw data files
    ''' </summary>
    Public Sub ImportsFiles()
        Using file As New OpenFileDialog With {
            .Filter = "Raw Data(*.mzXML; *.mzML)|*.mzXML;*.mzML",
            .Multiselect = True
        }
            If file.ShowDialog = DialogResult.OK Then
                For Each path As String In file.FileNames
                    Call fileExplorer.ImportsRaw(path)
                Next
            End If
        End Using
    End Sub

    Private Sub ExitToolsStripMenuItem_Click(sender As Object, e As ExecuteEventArgs)
        Me.Close()
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
        AddHandler ribbonItems.ButtonImportsRawFiles.ExecuteEvent, AddressOf ImportsFiles
        AddHandler ribbonItems.ButtonAbout.ExecuteEvent, AddressOf About_Click
        AddHandler ribbonItems.ButtonPageNavBack.ExecuteEvent, AddressOf NavBack_Click
        AddHandler ribbonItems.ButtonNew.ExecuteEvent, AddressOf CreateNewScript

        AddHandler ribbonItems.TweaksImage.ExecuteEvent, AddressOf mzkitTool.ShowPlotTweaks
        AddHandler ribbonItems.ShowProperty.ExecuteEvent, AddressOf ShowProperties

        AddHandler ribbonItems.ButtonMzCalculator.ExecuteEvent, Sub(sender, e) Call ShowPage(mzkitCalculator)
        AddHandler ribbonItems.ButtonSettings.ExecuteEvent, AddressOf ShowSettings
        AddHandler ribbonItems.ButtonMzSearch.ExecuteEvent, Sub(sender, e) Call ShowPage(mzkitSearch)
        AddHandler ribbonItems.ButtonRsharp.ExecuteEvent, AddressOf showRTerm

        AddHandler ribbonItems.ButtonDropA.ExecuteEvent, Sub(sender, e) ShowPage(mzkitTool)
        AddHandler ribbonItems.ButtonDropB.ExecuteEvent, Sub(sender, e) ShowPage(mzkitCalculator)
        AddHandler ribbonItems.ButtonDropC.ExecuteEvent, Sub(sender, e) ShowPage(mzkitSearch)
        AddHandler ribbonItems.ButtonDropD.ExecuteEvent, Sub(sender, e) ShowPage(mzkitMNtools)
        AddHandler ribbonItems.ButtonShowSpectrumSearchPage.ExecuteEvent, Sub(sender, e) Call New frmSpectrumSearch().Show(dockPanel)

        AddHandler ribbonItems.ButtonCalculatorExport.ExecuteEvent, Sub(sender, e) Call mzkitCalculator.ExportToolStripMenuItem_Click()
        AddHandler ribbonItems.ButtonExactMassSearchExport.ExecuteEvent, Sub(sender, e) Call mzkitTool.ExportExactMassSearchTable()
        AddHandler ribbonItems.ButtonSave.ExecuteEvent, Sub(sender, e) Call saveCurrentFile()
        AddHandler ribbonItems.ButtonNetworkExport.ExecuteEvent, Sub(sender, e) Call mzkitMNtools.saveNetwork()
        AddHandler ribbonItems.ButtonFormulaSearchExport.ExecuteEvent, Sub(sender, e) Call mzkitSearch.SaveSearchResultTable()

        AddHandler ribbonItems.ButtonBioDeep.ExecuteEvent, Sub(sender, e) Call Process.Start("http://www.biodeep.cn/")
        AddHandler ribbonItems.ButtonLicense.ExecuteEvent, Sub(sender, e) Call New frmLicense().ShowDialog()

        AddHandler ribbonItems.ButtonExportImage.ExecuteEvent, Sub(sender, e) Call mzkitTool.SaveImageToolStripMenuItem_Click()
        AddHandler ribbonItems.ButtonExportMatrix.ExecuteEvent, Sub(sender, e) Call mzkitTool.SaveMatrixToolStripMenuItem_Click()

        AddHandler ribbonItems.ButtonLayout1.ExecuteEvent, Sub(sender, e) Call mzkitTool.SaveImageToolStripMenuItem_Click()
        AddHandler ribbonItems.ButtonLayout2.ExecuteEvent, Sub(sender, e) Call mzkitTool.SaveMatrixToolStripMenuItem_Click()

        AddHandler ribbonItems.ButtonShowStartPage.ExecuteEvent, AddressOf showStartPage
        AddHandler ribbonItems.ButtonShowLogWindow.ExecuteEvent, AddressOf showLoggingWindow

        AddHandler ribbonItems.ButtonShowExplorer.ExecuteEvent, AddressOf ShowExplorer
        AddHandler ribbonItems.ButtonShowSearchList.ExecuteEvent, AddressOf ShowSearchList
        AddHandler ribbonItems.ButtonShowProperties.ExecuteEvent, AddressOf ShowProperties

        AddHandler ribbonItems.ButtonShowPlotViewer.ExecuteEvent, Sub(sender, e) Call mzkitTool.ShowTabPage(mzkitTool.TabPage5)
        AddHandler ribbonItems.ButtonShowMatrixViewer.ExecuteEvent, Sub(sender, e) Call mzkitTool.ShowTabPage(mzkitTool.TabPage6)
        AddHandler ribbonItems.ButtonNetworkRender.ExecuteEvent, Sub(sender, e) Call mzkitMNtools.RenderNetwork()
        AddHandler ribbonItems.ButtonRefreshNetwork.ExecuteEvent, Sub(sender, e) Call mzkitMNtools.RefreshNetwork()

        AddHandler ribbonItems.ButtonRunScript.ExecuteEvent, AddressOf RunCurrentScript
        AddHandler ribbonItems.ButtonSaveScript.ExecuteEvent, AddressOf saveCurrentScript

        AddHandler ribbonItems.HelpButton.ExecuteEvent, AddressOf showHelp

        AddHandler ribbonItems.ButtonTIC.ExecuteEvent, Sub(sender, e) Call mzkitTool.TIC(isBPC:=False)
        AddHandler ribbonItems.ButtonBPC.ExecuteEvent, Sub(sender, e) Call mzkitTool.TIC(isBPC:=True)
        AddHandler ribbonItems.ButtonXIC.ExecuteEvent, AddressOf rawFeaturesList.ShowXICToolStripMenuItem_Click

        AddHandler ribbonItems.ButtonResetLayout.ExecuteEvent, AddressOf resetLayout

        AddHandler ribbonItems.RecentItems.ExecuteEvent, AddressOf _recentItems_ExecuteEvent
        ' AddHandler ribbonItems.ButtonMsImaging.ExecuteEvent, AddressOf showMsImaging
        AddHandler ribbonItems.ButtonMsDemo.ExecuteEvent, Sub() msDemo.ShowPage()
        AddHandler ribbonItems.Targeted.ExecuteEvent, AddressOf showTargetedQuantification

        AddHandler ribbonItems.LogInBioDeep.ExecuteEvent, Sub() Call New frmLogin().ShowDialog()

        _uiCollectionChangedEvent = New UICollectionChangedEvent()

        MyApplication.RegisterHost(Me)
    End Sub

    Private Sub showTargetedQuantification()
        Dim targeted As frmTargetedQuantification = dockPanel.Documents _
            .Where(Function(doc) TypeOf doc Is frmTargetedQuantification) _
            .FirstOrDefault

        If targeted Is Nothing Then
            targeted = New frmTargetedQuantification
        End If

        VisualStudio.Dock(targetedFeatures, DockState.DockLeft)

        targeted.Show(dockPanel)
        targeted.DockState = DockState.Document
    End Sub

    Dim viewer As New frmMsImagingViewer

    Private Sub showMsImaging(imzML As String)
        Dim progress As New frmProgressSpinner

        Call viewer.Show(dockPanel)
        Call msImageParameters.Show(dockPanel)
        Call New Thread(
            Sub()
                Dim canvas As New Drawer(imzML)

                Call viewer.Invoke(Sub() viewer.LoadRender(canvas, imzML))
                Call viewer.Invoke(Sub() viewer.DockState = DockState.Document)

                Call progress.Invoke(Sub() progress.Close())

                Invoke(Sub() Text = $"BioNovoGene Mzkit [{viewer.Text} {imzML.FileName}]")
            End Sub).Start()

        msImageParameters.DockState = DockState.DockLeft

        Call progress.ShowDialog()
    End Sub

    Private Sub resetLayout()
        fileExplorer.DockState = DockState.DockLeft
        rawFeaturesList.DockState = DockState.DockLeftAutoHide
        output.DockState = DockState.DockBottomAutoHide
        propertyWin.DockState = DockState.DockRightAutoHide
    End Sub

    Private Sub showHelp(sender As Object, e As ExecuteEventArgs)
        For Each dir As String In {App.HOME, $"{App.HOME}/docs", $"{App.HOME}/../", $"{App.HOME}/../docs/"}
            If $"{dir}/readme.pdf".FileExists Then
                Call Process.Start($"{dir}/readme.pdf")
                Return
            End If
        Next

        ' try open online page
        Call Process.Start("https://mzkit.org/dist/README.pdf")

        ' Me.showStatusMessage("Manul pdf file is missing...", My.Resources.StatusAnnotations_Warning_32xLG_color)
    End Sub

    Friend Sub RunCurrentScript(sender As Object, e As ExecuteEventArgs)
        Dim active = dockPanel.ActiveDocument

        If Not active Is Nothing AndAlso TypeOf CObj(active) Is frmRScriptEdit Then
            Dim editor = DirectCast(CObj(active), frmRScriptEdit)
            Dim script As String = editor.ScriptText

            If Not editor.scriptFile.StringEmpty Then
                script.SaveTo(editor.scriptFile)
                script = editor.scriptFile
            End If

            Call MyApplication.ExecuteRScript(script, isFile:=Not editor.scriptFile.StringEmpty, AddressOf MyApplication.host.output.AppendRoutput)
            Call VisualStudio.Dock(output, DockState.DockBottom)
        End If
    End Sub

    Friend ReadOnly scriptFiles As New List(Of frmRScriptEdit)

    Public Sub CreateNewScript(sender As Object, e As ExecuteEventArgs)
        Dim newScript As New frmRScriptEdit

        newScript.Show(dockPanel)
        newScript.DockState = DockState.Document
        newScript.Text = "New R# Script"

        scriptFiles.Add(newScript)

        Me.Text = $"BioNovoGene Mzkit [{newScript.Text}]"
    End Sub

    Private Sub showRTerm(sender As Object, e As ExecuteEventArgs)
        RtermPage.Show(dockPanel)
        RtermPage.DockState = DockState.Document

        Me.Text = $"BioNovoGene Mzkit [{RtermPage.Text}]"
    End Sub

    Private Sub ShowSettings(sender As Object, e As ExecuteEventArgs)
        settingsPage.Show(dockPanel)
        settingsPage.DockState = DockState.Document

        Me.Text = $"BioNovoGene Mzkit [{settingsPage.Text}]"
    End Sub

    Private Sub ShowExplorer(sender As Object, e As ExecuteEventArgs)
        fileExplorer.Show(dockPanel)
        fileExplorer.DockState = DockState.DockLeft
    End Sub

    Private Sub ShowSearchList(sender As Object, e As ExecuteEventArgs)
        rawFeaturesList.Show(dockPanel)
        rawFeaturesList.DockState = DockState.DockLeft
    End Sub

    Private Sub ShowProperties(sender As Object, e As ExecuteEventArgs)
        ShowPropertyWindow()
    End Sub

    Private Sub showLoggingWindow(sender As Object, e As ExecuteEventArgs)
        output.Show(dockPanel)
        output.DockState = DockState.DockBottom
    End Sub

    Private Sub showStartPage(sender As Object, e As ExecuteEventArgs)
        If Not Globals.CheckFormOpened(startPage) Then
            startPage = New frmStartPage
        End If

        startPage.Show(dockPanel)
        startPage.DockState = DockState.Document

        Me.Text = $"BioNovoGene Mzkit [{startPage.Text}]"
    End Sub

    Private Sub saveCurrentScript()
        Dim active = dockPanel.ActiveDocument

        If Not active Is Nothing AndAlso TypeOf CObj(active) Is frmRScriptEdit Then
            Call SaveScript(DirectCast(CObj(active), frmRScriptEdit))
        End If
    End Sub

    Public Sub SaveScript(script As frmRScriptEdit)
        If script.scriptFile.StringEmpty Then
            Using save As New SaveFileDialog With {.Filter = "R# script file(*.R)|*.R"}

                If save.ShowDialog = DialogResult.OK Then
                    script.scriptFile = save.FileName
                    script.Save(save.FileName)
                    script.Text = save.FileName.FileName

                    fileExplorer.AddScript(save.FileName)
                End If
            End Using
        Else
            Call script.Save(script.scriptFile)
        End If

        If Not script.scriptFile.StringEmpty Then
            Globals.AddRecentFileHistory(script.scriptFile)
            Me.showStatusMessage($"Save R# script file at location {script.scriptFile.GetFullPath}!")
        End If
    End Sub

    Private Sub saveCurrentFile()
        Dim active = dockPanel.ActiveDocument

        If Not active Is Nothing Then
            Call saveCurrentDocument(active)
        Else
            TreeView1.SaveRawFileCache(
                Sub()
                    ' do nothing
                End Sub)

            Me.showStatusMessage("The workspace was saved!")
        End If
    End Sub

    Private Sub saveCurrentDocument(active As IDockContent)
        If TypeOf CObj(active) Is frmSettings Then

            Call DirectCast(CObj(active), frmSettings).SaveSettings()

        ElseIf TypeOf active Is frmRScriptEdit Then

            Call saveCurrentScript()

        ElseIf CObj(active).GetType.ImplementInterface(Of ISaveHandle) Then
            Dim file As String = Nothing
            Dim content As ContentType() = Nothing

            If CObj(active).GetType.ImplementInterface(Of IFileReference) Then
                file = DirectCast(CObj(active), IFileReference).FilePath
                content = DirectCast(CObj(active), IFileReference).MimeType
            End If

            If file.StringEmpty Then
                Using save As New SaveFileDialog

                    If Not content.IsNullOrEmpty Then
                        save.Filter = content.Select(Function(a) $"{a.Name}(*{a.FileExt})|*{a.FileExt}").JoinBy("|")
                    End If

                    If save.ShowDialog = DialogResult.OK Then
                        file = save.FileName
                        Call DirectCast(CObj(active), ISaveHandle).Save(save.FileName)
                    End If
                End Using
            Else
                Call DirectCast(CObj(active), ISaveHandle).Save(file)
            End If

            If Not file.StringEmpty Then
                Globals.AddRecentFileHistory(file)
                Me.showStatusMessage($"Current file saved at {file.GetFullPath}!")
            End If
        End If
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

    Private Sub NavBack_Click(sender As Object, e As ExecuteEventArgs)
        If nav.Count > 0 Then
            Call ShowPage(nav.Pop, pushStack:=False)
        End If
    End Sub

    Private Sub About_Click(sender As Object, e As ExecuteEventArgs)
        Call New frmSplashScreen() With {.isAboutScreen = True, .TopMost = True}.Show()
    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim splashScreen As frmSplashScreen = MyApplication.GetSplashScreen

        If splashScreen Is Nothing Then
            MessageBox.Show("The program is corrupt, please re-install and then run again...", "Program File Damaged!", MessageBoxButtons.OK, MessageBoxIcon.Error)
            App.Exit()
        Else
            Globals.sharedProgressUpdater = AddressOf splashScreen.UpdateInformation
        End If

        splashScreen.UpdateInformation("Initialize of the ribbon UI...")

        InitSpinner()
        InitializeFormulaProfile()

        splashScreen.UpdateInformation("Initialize of the VisualStudio UI...")

        ' 20200829 因为有一组控件需要放置在这里
        ' 所以这个基础的panel需要首先进行初始化
        Call initializeVSPanel()

        splashScreen.UpdateInformation("Load recent items...")

        InitRecentItems()
        ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active

        splashScreen.UpdateInformation("Create mzkit toolkit pages...")

        panelMain.addPage(mzkitTool, mzkitSearch, mzkitCalculator, mzkitMNtools)
        ShowPage(mzkitTool)

        mzkitTool.Ribbon_Load(Ribbon1)
        ribbonItems.CheckBoxXICRelative.BooleanValue = True

        splashScreen.UpdateInformation("Load configurations...")

        If Globals.Settings.ui Is Nothing Then
            Globals.Settings.ui = New UISettings
        End If

        If (Not (Globals.Settings.ui.width = 0 OrElse Globals.Settings.ui.height = 0)) AndAlso
            Globals.Settings.ui.window <> FormWindowState.Minimized AndAlso
            Globals.Settings.ui.rememberWindowsLocation Then

            splashScreen.UpdateInformation("Apply UI layout...")

            Me.Location = Globals.Settings.ui.getLocation
            Me.Size = Globals.Settings.ui.getSize
            Me.WindowState = Globals.Settings.ui.window

            ' Call Globals.Settings.ui.setColors(Ribbon1)
        End If

        splashScreen.UpdateInformation("Fetch news from bionovogene...")

        MyApplication.host.startPage.Show(MyApplication.host.dockPanel)

        splashScreen.UpdateInformation("Initialize of the R# automation scripting engine...")

        MyApplication.InitializeREngine()

        Timer1.Enabled = True
        Timer1.Start()

        Dim text As New StringBuilder

        Using output As New StringWriter(text)
            Call GetType(MyApplication).Assembly.FromAssembly.AppSummary("Welcome to the BioNovoGene M/z Data Toolkit!", "", output)
        End Using

        Call MyApplication.LogText(text.ToString)

        splashScreen.UpdateInformation("Ready!")
        showStatusMessage("Ready!")
    End Sub

    Private Sub _uiCollectionChangedEvent_ChangedEvent(sender As Object, e As UICollectionChangedEventArgs)
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

    Sub showStatusMessage(message As String, Optional icon As Image = Nothing)
        MyApplication.host.Invoke(
            Sub()
                If icon Is Nothing Then
                    icon = My.Resources.preferences_system_notifications
                End If

                ToolStripStatusLabel1.Image = icon
                ToolStripStatusLabel1.Text = message

                Call MyApplication.LogText(message)
            End Sub)
    End Sub

    Sub UpdateCacheSize(newSize As String)
        Me.Invoke(Sub() ToolStripStatusLabel2.Text = newSize)
    End Sub

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


        _spinner = ribbonItems.SpinnerSimilarity

        _spinner.MaxValue = 1
        _spinner.MinValue = 0
        _spinner.Increment = 0.01
        _spinner.DecimalPlaces = 2
        _spinner.DecimalValue = 0.6

        _spinner.TooltipTitle = "Spectrum Similarity"
        _spinner.TooltipDescription = "Enter similarity filter value for filtering of the node links."
        _spinner.RepresentativeString = "XXXXXX"
    End Sub

    Private Sub InitRecentItems()
        ' prepare list of recent items
        recentItems = New List(Of RecentItemsPropertySet)()

        For Each file In Globals.Settings.recentFiles.SafeQuery
            recentItems.Add(
                item:=New RecentItemsPropertySet() With {
                    .Label = file.FileName,
                    .LabelDescription = $"Location at {file.ParentPath}",
                    .Pinned = True
            })
        Next

        ribbonItems.RecentItems.RecentItems = recentItems
    End Sub

    Private Sub _recentItems_ExecuteEvent(sender As Object, e As ExecuteEventArgs)
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
            Dim sourceFile As String = Nothing

            For Each file As String In Globals.Settings.recentFiles.SafeQuery
                If label = file.FileName Then
                    sourceFile = file
                    Exit For
                End If
            Next

            ' get selected item label description
            Dim propLabelDescription As PropVariant
            e.CommandExecutionProperties.GetValue(RibbonProperties.LabelDescription, propLabelDescription)
            Dim labelDescription As String = CStr(propLabelDescription.Value)

            ' get selected item pinned value
            Dim propPinned As PropVariant
            e.CommandExecutionProperties.GetValue(RibbonProperties.Pinned, propPinned)
            Dim pinned As Boolean = CBool(propPinned.Value)

            If label.ExtensionSuffix("R") Then
                If Not sourceFile.FileExists Then
                    MessageBox.Show($"The given R# script file [{label}] is not exists on your file system!", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    Call openRscript(sourceFile)
                End If
            Else
                Dim raw As Raw() = Globals.FindRaws(fileExplorer.treeView1, label).ToArray

                If raw Is Nothing Then
                    MessageBox.Show($"The given raw data file [{label}] is not exists on your file system!", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    Call mzkitTool.TIC(raw)
                End If
            End If
        End If
    End Sub

    Private Sub frmMain_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Dim progress As New frmTaskProgress

        progress.ShowProgressTitle("App Exit...", directAccess:=True)
        progress.ShowProgressDetails("Save raw data file viewer workspace...", directAccess:=True)

        Call New Thread(
            Sub()
                Call Thread.Sleep(100)
                Call fileExplorer.Invoke(Sub() fileExplorer.SaveFileCache(AddressOf progress.ShowProgressDetails))
                Call progress.ShowProgressDetails("Save app settings...")
                Call Invoke(Sub() Call SaveSettings())
                Call progress.Invoke(Sub() progress.Close())
            End Sub).Start()

        Call progress.ShowDialog()
        Call App.Exit()
    End Sub

    Friend Sub SaveSettings()
        If Globals.Settings.ui Is Nothing Then
            Globals.Settings.ui = New UISettings
        End If

        Globals.Settings.ui = New UISettings With {
            .height = Height,
            .width = Width,
            .x = Location.X,
            .y = Location.Y,
            .window = WindowState,
            .rememberWindowsLocation = Globals.Settings.ui.rememberWindowsLocation,
            .rememberLayouts = Globals.Settings.ui.rememberLayouts,
            .fileExplorerDock = fileExplorer.DockState,
            .OutputDock = output.DockState,
            .propertyWindowDock = propertyWin.DockState,
            .featureListDock = rawFeaturesList.DockState,
            .taskListDock = taskWin.DockState
        }

        Globals.Settings.Save()
        App.Exit()
    End Sub

#Region "vs2015"

    Friend WithEvents dockPanel As New WeifenLuo.WinFormsUI.Docking.DockPanel
    Private vS2015LightTheme1 As New WeifenLuo.WinFormsUI.Docking.VS2015LightTheme
    Private vsToolStripExtender1 As New WeifenLuo.WinFormsUI.Docking.VisualStudioToolStripExtender
    Private ReadOnly _toolStripProfessionalRenderer As ToolStripRenderer = New ToolStripProfessionalRenderer()

    Friend fileExplorer As New frmFileExplorer
    Friend rawFeaturesList As New frmRawFeaturesList
    Friend UVScansList As New frmUVScans
    Friend spectrumTreeExplorer As New frmTreeExplorer

    Friend output As New OutputWindow
    Friend WithEvents panelMain As New frmDockDocument
    Friend startPage As New frmStartPage
    Friend settingsPage As New frmSettings
    Friend RtermPage As New frmRsharp
    Friend propertyWin As New PropertyWindow
    Friend taskWin As New TaskListWindow
    Friend plotParams As New frmTweaks
    Friend msImageParameters As New frmMsImagingTweaks
    Friend msDemo As New frmDemo
    Friend MRMIons As New frmSRMIonsExplorer
    Friend GCMSPeaks As New frmGCMSPeaks
    Friend targetedFeatures As New frmTargetedFeatureExplorer

    Public Sub ShowPropertyWindow()
        VisualStudio.Dock(propertyWin, DockState.DockRight)
    End Sub

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

        output.Show(dockPanel)
        MyApplication.RegisterOutput(output)
        fileExplorer.Show(dockPanel)

        TreeView1 = fileExplorer.treeView1

        UVScansList.Show(dockPanel)
        UVScansList.DockState = DockState.Hidden

        spectrumTreeExplorer.Show(dockPanel)
        spectrumTreeExplorer.DockState = DockState.Hidden

        plotParams.Show(dockPanel)
        plotParams.DockState = DockState.Hidden

        rawFeaturesList.Show(dockPanel)
        propertyWin.Show(dockPanel)

        startPage.Show(dockPanel)
        startPage.DockState = DockState.Document

        panelMain.Show(dockPanel)
        panelMain.DockState = DockState.Document

        settingsPage.Show(dockPanel)
        settingsPage.DockState = DockState.Hidden

        GCMSPeaks.Show(dockPanel)
        GCMSPeaks.DockState = DockState.Hidden

        MRMIons.Show(dockPanel)
        MRMIons.DockState = DockState.Hidden

        RtermPage.Show(dockPanel)
        RtermPage.DockState = DockState.Hidden

        taskWin.Show(dockPanel)
        taskWin.DockState = DockState.DockBottomAutoHide

        msImageParameters.Show(dockPanel)
        msImageParameters.DockState = DockState.Hidden

        msDemo.Show(dockPanel)
        msDemo.DockState = DockState.Hidden

        targetedFeatures.Show(dockPanel)
        targetedFeatures.DockState = DockState.Hidden

        If Globals.Settings.ui.rememberLayouts Then
            fileExplorer.DockState = Globals.Settings.ui.fileExplorerDock
            rawFeaturesList.DockState = Globals.Settings.ui.featureListDock
            output.DockState = Globals.Settings.ui.OutputDock
            propertyWin.DockState = Globals.Settings.ui.propertyWindowDock
        Else
            fileExplorer.DockState = DockState.DockLeftAutoHide
            rawFeaturesList.DockState = DockState.DockLeftAutoHide
            output.DockState = DockState.DockBottomAutoHide
            propertyWin.DockState = DockState.DockRightAutoHide
        End If
    End Sub

    Public Sub ShowMzkitToolkit()
        panelMain.Show(dockPanel)
        panelMain.DockState = DockState.Document
    End Sub

    Private Sub SetSchema(sender As Object, e As EventArgs)
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

    Private Sub EnableVSRenderer(version As VisualStudioToolStripExtender.VsVersion, theme As ThemeBase)
        ' vsToolStripExtender1.SetStyle(MainMenu, version, theme)
        ' vsToolStripExtender1.SetStyle(ToolBar, version, theme)
        vsToolStripExtender1.SetStyle(StatusStrip, version, theme)
    End Sub

    Private Sub frmMain_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        Ribbon1.Refresh()

        startPage.ResumeLayout(performLayout:=False)
        startPage.PerformLayout()
    End Sub

    Dim mzkitApp As Process = Process.GetCurrentProcess()

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ToolStripStatusLabel3.Text = $"Memory: {StringFormats.Lanudry(mzkitApp.WorkingSet64)}"
        mzkitApp.Refresh()
    End Sub

    Private Sub ToolStripStatusLabel2_Click(sender As Object, e As EventArgs) Handles ToolStripStatusLabel2.Click

    End Sub

    Private Sub frmMain_ResizeBegin(sender As Object, e As EventArgs) Handles Me.ResizeBegin
        startPage.SuspendLayout()
    End Sub

    Private Sub frmMain_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp

    End Sub

    ''' <summary>
    ''' 不太清楚为什么<see cref="App.Exit(Integer)"/>没有
    ''' 正常通过<see cref="App.Running"/>中断R终端线程
    ''' 在这里强制退出
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub frmMain_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        End
    End Sub

    Private Sub frmMain_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Ribbon1.Update()
        Ribbon1.Refresh()
    End Sub


#End Region

End Class
