#Region "Microsoft.VisualBasic::d1a66053acb459c240cd0f6263f18427, mzkit\src\mzkit\mzkit\forms\frmMain.vb"

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

    '   Total Lines: 702
    '    Code Lines: 500
    ' Comment Lines: 52
    '   Blank Lines: 150
    '     File Size: 27.94 KB


    ' Class frmMain
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetPPMError, GetXICPPMError
    ' 
    '     Sub: dockPanel_ActiveDocumentChanged, EnableVSRenderer, FormulaSearchToolToolStripMenuItem_Click, frmMain_Closed, frmMain_Closing
    '          frmMain_KeyUp, frmMain_Load, frmMain_Resize, frmMain_ResizeBegin, frmMain_ResizeEnd
    '          ImportsFiles, InitializeFormulaProfile, initializeVSPanel, InitRecentItems, InitSpinner
    '          MoleculeNetworkingToolStripMenuItem_Click, MzCalculatorToolStripMenuItem_Click, OpenFile, openRscript, RawFileViewerToolStripMenuItem_Click
    '          saveCurrentDocument, saveCurrentFile, saveCurrentScript, SaveScript, SaveSettings
    '          SetSchema, ShowGCMSSIM, ShowMRMIons, showMsImaging, ShowMzkitToolkit
    '          showMzPackMSI, ShowPage, ShowPropertyWindow, showStatusMessage, Timer1_Tick
    '          ToolStripStatusLabel2_Click, ToolStripStatusLabel4_Click, UpdateCacheSize, warning
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.IndexedCache
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports BioNovoGene.mzkit_win32.Configuration
Imports BioNovoGene.mzkit_win32.My
Imports BioNovoGene.mzkit_win32.RibbonLib.Controls
Imports RibbonLib
Imports RibbonLib.Interop
Imports Task
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmMain

    Friend mzkitTool As New PageMzkitTools With {.Text = "Raw File Viewer"}
    Friend mzkitSearch As New PageMzSearch With {.Text = "M/Z Formula De-novo Search"}
    Friend mzkitCalculator As New PageMzCalculator With {.Text = "M/Z Calculator"}
    Friend mzkitMNtools As New PageMoleculeNetworking With {.Text = "Molecular Networking"}

    Friend TreeView1 As TreeView

    Public Function GetPPMError() As Double
        Return Val(ribbonItems.PPMSpinner.DecimalValue)
    End Function

    Public Function GetXICPPMError() As Double
        Return Val(ribbonItems.XIC_PPMSpinner.DecimalValue)
    End Function

    Friend Sub ShowPage(page As Control, Optional pushStack As Boolean = True)
        For Each page2 In WindowModules.panelMain.pages
            If Not page Is page2 Then
                page2.Visible = False
                page2.Hide()
            End If
        Next

        If pushStack Then
            RibbonEvents.nav.Push(page)
        End If

        Me.Text = $"BioNovoGene Mzkit [{page.Text}]"
        page.Visible = True
        page.Show()

        WindowModules.panelMain.Show(dockPanel)
    End Sub

    Public Sub ShowMRMIons(file As String)
        If Not file.FileExists Then
            Call showStatusMessage($"missing raw data file '{file.GetFullPath}'!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            WindowModules.MRMIons.DockState = DockState.DockLeft
            WindowModules.MRMIons.LoadMRM(file)
        End If
    End Sub

    Public Sub ShowGCMSSIM(file As String, isBackground As Boolean, showExplorer As Boolean)
        If Not file.FileExists Then
            Call showStatusMessage($"missing raw data file '{file.GetFullPath}'!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        ElseIf Not WindowModules.GCMSPeaks.ContainsRaw(file) Then
            Dim raw = frmGCMS_CDFExplorer.loadCDF(file, isBackground)

            Call WindowModules.GCMSPeaks.Invoke(Sub() WindowModules.GCMSPeaks.LoadRawExplorer(raw, showDocument:=showExplorer))
            Call VisualStudio.Dock(WindowModules.GCMSPeaks, DockState.DockLeft)
        Else
            WindowModules.GCMSPeaks.ShowRaw(file)
        End If
    End Sub

    Friend Sub warning(v As String)
        Call showStatusMessage(v, My.Resources.StatusAnnotations_Warning_32xLG_color)
    End Sub

    Public Sub OpenFile(fileName As String, showDocument As Boolean)
        If fileName.ExtensionSuffix("R") Then
            Call WindowModules.fileExplorer.AddScript(fileName.GetFullPath)
            Call openRscript(fileName)
        ElseIf fileName.ExtensionSuffix("imzML") Then
            Call showMsImaging(fileName)
        ElseIf fileName.ExtensionSuffix("mzml") AndAlso RawScanParser.IsMRMData(fileName) Then
            Call ShowMRMIons(fileName)
        ElseIf fileName.ExtensionSuffix("mzml") AndAlso RawScanParser.IsSIMData(fileName) Then
            Call ShowGCMSSIM(fileName, isBackground:=False, showExplorer:=showDocument)
        ElseIf fileName.ExtensionSuffix("cdf", "netcdf") Then
            Call ShowGCMSSIM(fileName, isBackground:=False, showExplorer:=showDocument)
        ElseIf fileName.ExtensionSuffix("mzpack") Then
            Dim raw As New Raw With {
                .cache = fileName,
                .numOfScan1 = 0,
                .numOfScan2 = 0,
                .rtmax = 0,
                .rtmin = 0,
                .source = fileName
            }

            Call WindowModules.rawFeaturesList.LoadRaw(raw)
            Call VisualStudio.Dock(WindowModules.rawFeaturesList, DockState.DockLeft)
        ElseIf fileName.ExtensionSuffix("wiff") Then
            Dim wiffRaw As New sciexWiffReader.WiffScanFileReader(fileName)
            Dim mzPack As mzPack = frmTaskProgress.LoadData(Function(println) wiffRaw.LoadFromWiffRaw(println))
            Dim cacheFile As String = TempFileSystem.GetAppSysTempFile(".mzPack", App.PID.ToHexString, "WiffRawFile_")
            Dim raw As New Raw With {
               .cache = cacheFile,
               .numOfScan1 = 0,
               .numOfScan2 = 0,
               .rtmax = 0,
               .rtmin = 0,
               .source = fileName
            }

            Using temp As Stream = cacheFile.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                Call mzPack.Write(temp)
            End Using

            Call WindowModules.rawFeaturesList.LoadRaw(raw)
            Call VisualStudio.Dock(WindowModules.rawFeaturesList, DockState.DockLeft)
        ElseIf fileName.ExtensionSuffix("raw") Then
            Dim Xraw As New MSFileReader(fileName)
            Dim mzPack As mzPack = frmTaskProgress.LoadData(Function(println) Xraw.LoadFromXRaw(println))
            Dim cacheFile As String = TempFileSystem.GetAppSysTempFile(".mzPack", App.PID.ToHexString, "MSRawFile_")
            Dim raw As New Raw With {
               .cache = cacheFile,
               .numOfScan1 = 0,
               .numOfScan2 = 0,
               .rtmax = 0,
               .rtmin = 0,
               .source = fileName
            }

            Using temp As Stream = cacheFile.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                Call mzPack.Write(temp)
            End Using

            Call WindowModules.rawFeaturesList.LoadRaw(raw)
            Call VisualStudio.Dock(WindowModules.rawFeaturesList, DockState.DockLeft)
        Else
            Call WindowModules.fileExplorer.ImportsRaw(fileName)
        End If

        Globals.AddRecentFileHistory(fileName)
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
            .Filter = "Raw Data(*.mzXML; *.mzML)|*.mzXML;*.mzML|Thermo MSRaw(*.raw)|*.raw",
            .Multiselect = True
        }
            If file.ShowDialog = DialogResult.OK Then
                For Each path As String In file.FileNames
                    Call WindowModules.fileExplorer.ImportsRaw(path)
                Next
            End If
        End Using
    End Sub

    Friend ribbonItems As RibbonItems
    Friend recentItems As List(Of RecentItemsPropertySet)

    Dim _uiCollectionChangedEvent As UICollectionChangedEvent

    Public Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        Call MyApplication.RegisterHost(Me)

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        ribbonItems = New RibbonItems(Ribbon1)
        ribbonItems.AddHandlers

        _uiCollectionChangedEvent = New UICollectionChangedEvent()
    End Sub

    Friend Sub showMsImaging(imzML As String)
        WindowModules.viewer.Show(dockPanel)
        WindowModules.msImageParameters.Show(dockPanel)

        If imzML.ExtensionSuffix("mzpack") Then
            Call showMzPackMSI(imzML)
        Else
            Dim cachefile As String = RscriptProgressTask.CreateMSIIndex(imzML)

            WindowModules.viewer.LoadRender(cachefile, imzML)

            Text = $"BioNovoGene Mzkit [{WindowModules.viewer.Text} {imzML.FileName}]"
        End If

        WindowModules.viewer.RenderSummary(IntensitySummary.BasePeak)
        WindowModules.viewer.DockState = DockState.Document
        WindowModules.msImageParameters.DockState = DockState.DockLeft
    End Sub

    Friend Sub showMzPackMSI(mzpack As String)
        Dim progress As New frmTaskProgress

        Call progress.ShowProgressTitle("Open mzPack for MSI...", directAccess:=True)
        Call progress.ShowProgressDetails("Loading MSI raw data file into viewer workspace...", directAccess:=True)

        Call New Thread(
           Sub()
               Call ServiceHub.StartMSIService()
               Call Thread.Sleep(100)

               Dim dataPack = ServiceHub.LoadMSI(mzpack, Sub(msg) progress.ShowProgressDetails(msg))

               Call WindowModules.viewer.Invoke(Sub() WindowModules.viewer.LoadRender(dataPack, mzpack))
               Call Invoke(Sub() Text = $"BioNovoGene Mzkit [{WindowModules.viewer.Text} {mzpack.FileName}]")
               Call progress.CloseWindow()
           End Sub).Start()

        Call progress.ShowDialog()
    End Sub

    Friend Sub saveCurrentScript()
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

                    WindowModules.fileExplorer.AddScript(save.FileName)
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

    Friend Sub saveCurrentFile()
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

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim splashScreen As New frmSplashScreen With {
            .TopMost = True,
            .ShowInTaskbar = False,
            .StartPosition = FormStartPosition.CenterScreen
        }

        Call New Thread(AddressOf splashScreen.ShowDialog).Start()

        Globals.loadedSettings = False
        Globals.sharedProgressUpdater = AddressOf splashScreen.UpdateInformation
        Thread.Sleep(2000)

        Do While App.Running AndAlso Globals.loadedSettings
            Thread.Sleep(1)
        Loop

        Globals.loadedSettings = True

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

        WindowModules.panelMain.addPage(mzkitTool, mzkitSearch, mzkitCalculator, mzkitMNtools)
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

        WindowModules.startPage.Show(MyApplication.host.dockPanel)

        splashScreen.UpdateInformation("Initialize of the R# automation scripting engine...")

        MyApplication.InitializeREngine()

        Timer1.Enabled = True
        Timer1.Start()
        ToolStripProgressBar1.Value = 0
        ToolStripProgressBar1.Maximum = 0

        Dim text As New StringBuilder

        Using output As New StringWriter(text)
            Call GetType(MyApplication).Assembly.FromAssembly.AppSummary("Welcome to the BioNovoGene M/z Data Toolkit!", "", output)
        End Using

        Call MyApplication.LogText(text.ToString)

        splashScreen.UpdateInformation("Ready!")
        showStatusMessage("Ready!")
        splashScreen.Invoke(Sub() Call splashScreen.Close())

        If Not MyApplication.afterLoad Is Nothing Then
            Call MyApplication.afterLoad()
        End If
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

    ''' <summary>
    ''' 线程操作安全的消息提示函数
    ''' </summary>
    ''' <param name="message"></param>
    ''' <param name="icon"></param>
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

        _spinner.MaxValue = Decimal.MaxValue
        _spinner.MinValue = 0
        _spinner.Increment = 0.5D
        _spinner.DecimalPlaces = 1
        _spinner.DecimalValue = Globals.Settings.viewer.ppm_error

        _spinner.TooltipTitle = "PPM"
        _spinner.TooltipDescription = "Enter ppm error for search feature by m/z."
        _spinner.RepresentativeString = "XXXXXX"

        _spinner = ribbonItems.XIC_PPMSpinner
        _spinner.MaxValue = Decimal.MaxValue
        _spinner.MinValue = 0
        _spinner.Increment = 0.5D
        _spinner.DecimalPlaces = 1
        _spinner.DecimalValue = Globals.Settings.viewer.XIC_ppm

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

    Private Sub frmMain_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Dim progress As New frmTaskProgress

        progress.ShowProgressTitle("App Exit...", directAccess:=True)
        progress.ShowProgressDetails("Save raw data file viewer workspace...", directAccess:=True)

        Globals.sharedProgressUpdater =
            Sub()
                ' do nothing
            End Sub

        Call New Thread(
            Sub()
                Call Thread.Sleep(100)
                Call WindowModules.fileExplorer.Invoke(
                    Sub()
                        WindowModules.fileExplorer.SaveFileCache(AddressOf progress.ShowProgressDetails)
                    End Sub)
                Call progress.ShowProgressDetails("Save app settings...")
                Call Invoke(Sub() Call SaveSettings())
                Call progress.Invoke(Sub() progress.Close())
            End Sub).Start()

        Call ServiceHub.CloseMSIEngine()
        Call progress.ShowDialog()
        ' Call App.Exit()
        Call Process.GetCurrentProcess.Kill()
    End Sub

    Friend Sub SaveSettings()
        If Globals.Settings.ui Is Nothing Then
            Globals.Settings.ui = New UISettings
        End If
        If Globals.Settings.viewer Is Nothing Then
            Globals.Settings.viewer = New RawFileViewerSettings
        End If

        Globals.Settings.viewer.XIC_ppm = MyApplication.host.GetXICPPMError
        Globals.Settings.viewer.ppm_error = MyApplication.host.GetPPMError
        Globals.Settings.ui = New UISettings With {
            .height = Height,
            .width = Width,
            .x = Location.X,
            .y = Location.Y,
            .window = WindowState,
            .rememberWindowsLocation = Globals.Settings.ui.rememberWindowsLocation,
            .rememberLayouts = Globals.Settings.ui.rememberLayouts,
            .fileExplorerDock = WindowModules.fileExplorer.DockState,
            .OutputDock = WindowModules.output.DockState,
            .propertyWindowDock = WindowModules.propertyWin.DockState,
            .featureListDock = WindowModules.rawFeaturesList.DockState,
            .taskListDock = WindowModules.taskWin.DockState，
            .language = Globals.Settings.ui.language
        }

        Globals.Settings.Save()
    End Sub

#Region "vs2015"

    Friend WithEvents dockPanel As New DockPanel
    Private vS2015LightTheme1 As New VS2015LightTheme
    Private vsToolStripExtender1 As New VisualStudioToolStripExtender
    Private ReadOnly _toolStripProfessionalRenderer As New ToolStripProfessionalRenderer()

    Public Sub ShowPropertyWindow()
        VisualStudio.Dock(WindowModules.propertyWin, DockState.DockRight)
    End Sub

    Private Sub initializeVSPanel()
        PanelBase.Controls.Add(Me.dockPanel)
        dockPanel.ShowDocumentIcon = True

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
        Call WindowModules.initializeVSPanel()

        TreeView1 = WindowModules.fileExplorer.treeView1
    End Sub

    Public Sub ShowMzkitToolkit()
        WindowModules.panelMain.Show(dockPanel)
        WindowModules.panelMain.DockState = DockState.Document
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

        WindowModules.startPage.ResumeLayout(performLayout:=False)
        WindowModules.startPage.PerformLayout()
    End Sub

    Dim mzkitApp As Process = Process.GetCurrentProcess()

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ToolStripStatusLabel3.Text = $"Memory: {StringFormats.Lanudry(mzkitApp.WorkingSet64)}"
        mzkitApp.Refresh()
    End Sub

    Private Sub ToolStripStatusLabel2_Click(sender As Object, e As EventArgs) Handles ToolStripStatusLabel2.Click

    End Sub

    Private Sub frmMain_ResizeBegin(sender As Object, e As EventArgs) Handles Me.ResizeBegin
        WindowModules.startPage.SuspendLayout()
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

    Private Sub ToolStripStatusLabel4_Click(sender As Object, e As EventArgs) Handles ToolStripStatusLabel4.Click
        Call VisualStudio.Dock(WindowModules.taskWin, DockState.DockBottom)
    End Sub

    Private Sub dockPanel_ActiveDocumentChanged(sender As Object, e As EventArgs) Handles dockPanel.ActiveDocumentChanged
        If TypeOf dockPanel.ActiveDocument Is frmTableViewer Then
            ribbonItems.TableGroup.ContextAvailable = ContextAvailability.Active
        Else
            ribbonItems.TableGroup.ContextAvailable = ContextAvailability.NotAvailable
        End If
        If TypeOf dockPanel.ActiveDocument Is frmMsImagingViewer Then
            ribbonItems.TabGroupMSI.ContextAvailable = ContextAvailability.Active
        Else
            ribbonItems.TabGroupMSI.ContextAvailable = ContextAvailability.NotAvailable
        End If
    End Sub
#End Region

End Class
