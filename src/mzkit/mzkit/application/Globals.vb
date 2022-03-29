#Region "Microsoft.VisualBasic::39d746b4c99fdce964e733247d3678cf, mzkit\src\mzkit\mzkit\application\Globals.vb"

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

'   Total Lines: 510
'    Code Lines: 379
' Comment Lines: 40
'   Blank Lines: 91
'     File Size: 18.71 KB


' Module Globals
' 
'     Properties: loadedSettings, Settings, workspace
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: CheckFormOpened, CurrentRawFile, FindRaws, GetColors, GetTotalCacheSize
'               GetXICMaxYAxis, loadBackground, LoadIonLibrary, LoadKEGG, LoadLipidMaps
'               (+2 Overloads) LoadRawFileCache, RawFileNodeTemplate
' 
'     Sub: AddRecentFileHistory, AddScript, InitExplorerUI, loadRawFile, loadRStudioScripts
'          loadWorkspace, SaveRawFileCache
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.sciexWiffReader
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemistry
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.MetaDNA
Imports BioNovoGene.BioDeep.MSEngine
Imports BioNovoGene.mzkit_win32.Configuration
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Distributions.BinBox
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports Task
Imports WeifenLuo.WinFormsUI.Docking
Imports stdNum = System.Math

Module Globals

    ''' <summary>
    ''' 这个是未进行任何工作区保存所保存的一个默认的临时文件的位置
    ''' </summary>
    Friend ReadOnly defaultWorkspace As String = App.LocalData & "/.defaultWorkspace"

    Dim currentWorkspace As ViewerProject

    Public ReadOnly Property Settings As Settings

    Public Property loadedSettings As Boolean = False

    Public ReadOnly Property workspace As ViewerProject
        Get
            Return currentWorkspace
        End Get
    End Property

    Sub New()
        Settings = Settings.GetConfiguration()

        Call LicenseFile.ApplyLicense()
    End Sub

    Public Function loadBackground(Optional ByRef maps As Map() = Nothing) As Background
        maps = KEGGRepo.RequestKEGGMaps
        Dim background = MSJointConnection.ImportsBackground(maps)

        Return background
    End Function

    Public Function LoadChebi(println As Action(Of String), mode As Integer, mzdiff As Tolerance) As MSSearch(Of MetaboliteAnnotation)
        Dim key As String = $"[{mzdiff.ToString}]{mode}"

        Static dataPack As MetaboliteAnnotation() = KEGGRepo.RequestChebi
        Static cache As New Dictionary(Of String, MSSearch(Of MetaboliteAnnotation))

        Return cache.ComputeIfAbsent(key,
            lazyValue:=Function()
                           If mode = 1 Then
                               Return MSSearch(Of MetaboliteAnnotation).CreateIndex(dataPack, Provider.Positives.Where(Function(t) stdNum.Abs(t.charge) = 1).ToArray, mzdiff)
                           Else
                               Return MSSearch(Of MetaboliteAnnotation).CreateIndex(dataPack, Provider.Positives.Where(Function(t) stdNum.Abs(t.charge) = 1).ToArray, mzdiff)
                           End If
                       End Function)
    End Function

    Public Function LoadLipidMaps(println As Action(Of String), mode As Integer, mzdiff As Tolerance) As MSSearch(Of LipidMaps.MetaData)
        Dim key As String = $"[{mzdiff.ToString}]{mode}"

        Static dataPack As LipidMaps.MetaData() = KEGGRepo.RequestLipidMaps
        Static cache As New Dictionary(Of String, MSSearch(Of LipidMaps.MetaData))

        Return cache.ComputeIfAbsent(key,
            lazyValue:=Function()
                           If mode = 1 Then
                               Return MSSearch(Of LipidMaps.MetaData).CreateIndex(dataPack, Provider.Positives.Where(Function(t) stdNum.Abs(t.charge) = 1).ToArray, mzdiff)
                           Else
                               Return MSSearch(Of LipidMaps.MetaData).CreateIndex(dataPack, Provider.Positives.Where(Function(t) stdNum.Abs(t.charge) = 1).ToArray, mzdiff)
                           End If
                       End Function)
    End Function

    Public Function LoadKEGG(println As Action(Of String), mode As Integer, mzdiff As Tolerance) As MSJointConnection
        Static background As Background = loadBackground()
        Static compounds = KEGGHandler.Wraps(KEGGRepo.RequestKEGGCompounds).ToArray
        Static cache As New Dictionary(Of String, KEGGHandler)

        Dim key As String = $"[{mzdiff.ToString}]{mode}"
        Dim handler As KEGGHandler = cache.ComputeIfAbsent(key,
            lazyValue:=Function()
                           If mode = 1 Then
                               Return KEGGHandler.CreateIndex(compounds, Provider.Positives.Where(Function(t) stdNum.Abs(t.charge) = 1).ToArray, mzdiff)
                           Else
                               Return KEGGHandler.CreateIndex(compounds, Provider.Negatives.Where(Function(t) stdNum.Abs(t.charge) = 1).ToArray, mzdiff)
                           End If
                       End Function)

        Return New MSJointConnection(handler, background)
    End Function

    Public Sub AddRecentFileHistory(file As String)
        Settings.recentFiles = {file}.JoinIterates(Settings.recentFiles).Distinct.ToArray
        Settings.Save()
    End Sub

    Public Function GetColors() As String
        Return Globals.Settings.viewer.colorSet.JoinBy(",")
    End Function

    <Extension>
    Public Sub SaveRawFileCache(explorer As TreeView, progress As Action(Of String))
        Dim files As New List(Of Task.Raw)
        Dim scripts As New List(Of String)

        If explorer.Nodes.Count > 0 Then
            Dim rawFileNodes = explorer.Nodes(Scan0)

            For Each node As TreeNode In rawFileNodes.Nodes
                files.Add(node.Tag)
                progress(files.Last.source)
                Application.DoEvents()
            Next

            If explorer.Nodes.Count > 1 Then
                For Each node As TreeNode In explorer.Nodes(1).Nodes
                    scripts.Add(DirectCast(node.Tag, String).GetFullPath)
                Next
            End If
        End If

        Dim opened As New List(Of String)
        Dim unsaved As New List(Of NamedValue)

        For Each doc As IDockContent In MyApplication.host.dockPanel.Documents
            If TypeOf doc Is frmRScriptEdit Then
                opened.Add(DirectCast(doc, frmRScriptEdit).scriptFile)

                If DirectCast(doc, frmRScriptEdit).IsUnsaved Then
                    unsaved.Add(New NamedValue With {.name = DirectCast(doc, frmRScriptEdit).scriptFile, .text = DirectCast(doc, frmRScriptEdit).ScriptText})
                End If
            End If
        Next

        Call currentWorkspace.SaveAs(files, scripts, opened, unsaved).Save(defaultWorkspace)
    End Sub

    <Extension>
    Public Iterator Function FindRaws(explorer As TreeView, sourceName As String) As IEnumerable(Of Raw)
        For Each node As TreeNode In explorer.Nodes.Item(0).Nodes
            Dim raw As Raw = DirectCast(node.Tag, Raw)

            If raw.source.FileName = sourceName Then
                Yield raw
            End If
        Next
    End Function

    <Extension>
    Public Function GetTotalCacheSize(explorer As TreeNode) As String
        Dim size As Double

        For Each node As TreeNode In explorer.Nodes
            size += DirectCast(node.Tag, Raw).GetCacheFileSize
        Next

        If size = 0.0 Then
            Return "0 KB"
        Else
            Return Lanudry(size)
        End If
    End Function

    Friend sharedProgressUpdater As Action(Of String)

    Public Sub loadWorkspace(mzwork As String, fromStartup As Boolean)
        If Not fromStartup Then
            If MessageBox.Show("Load new workspace will overrides current MZKit workspace, continute to process?", "Load New Workspace", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Return
            End If
        End If

        Dim work As WorkspaceFile = frmTaskProgress.LoadData(
            streamLoad:=Function(msg) Task.MZWork.ImportWorkspace(mzwork, msg),
            info:="Loading MZKit workspace..."
        )
        Dim project As New ViewerProject With {
            .FilePath = Globals.defaultWorkspace,
            .work = work
        }
        Dim explorer = WindowModules.fileExplorer

        sharedProgressUpdater = Sub(text) MyApplication.host.showStatusMessage(text)

        Call project.LoadRawFileCache(
            explorer:=explorer.treeView1,
            rawMenu:=explorer.ctxMenuFiles,
            targetRawMenu:=explorer.ctxMenuRawFile,
            scriptMenu:=explorer.ctxMenuScript
        )
        Call MyApplication.host.showStatusMessage("Ready!")
    End Sub

    Public Sub InitExplorerUI(explorer As TreeView, rawMenu As ContextMenuStrip, scriptMenu As ContextMenuStrip,
                              Optional ByRef scripts As TreeNode = Nothing,
                              Optional ByRef rawFiles As TreeNode = Nothing)

        scripts = New TreeNode("R# Automation") With {
            .ImageIndex = 1,
            .SelectedImageIndex = 1,
            .StateImageIndex = 1,
            .ContextMenuStrip = scriptMenu
        }
        rawFiles = New TreeNode("Raw Data Files") With {
            .ImageIndex = 0,
            .StateImageIndex = 0,
            .SelectedImageIndex = 0,
            .ContextMenuStrip = rawMenu
        }

        explorer.Nodes.Clear()
        explorer.Nodes.Add(rawFiles)
        explorer.Nodes.Add(scripts)
    End Sub

    Public Function RawFileNodeTemplate(raw As Raw, targetRawMenu As ContextMenuStrip) As TreeNode
        Dim sourcePath As String = raw.source.GetFullPath(throwEx:=False)
        Dim name As String = raw.source.FileName

        If name.Contains(vbCr) OrElse name.Contains(vbLf) Then
            name = name.LineTokens.Last

            If name.Length > 24 Then
                name = "..." & name.Substring(name.Length - 21)
            End If
        End If

        Dim rawFileNode As New TreeNode(name) With {
            .Checked = True,
            .Tag = raw,
            .ImageIndex = 2,
            .SelectedImageIndex = 2,
            .StateImageIndex = 2,
            .ContextMenuStrip = targetRawMenu,
            .ToolTipText = If(sourcePath.StringEmpty, "source file is missing!", sourcePath)
        }

        Return rawFileNode
    End Function

    ''' <summary>
    ''' two root nodes:
    ''' 
    ''' 1. Raw Data Files
    ''' 2. R# Automation
    ''' </summary>
    ''' <param name="explorer"></param>
    ''' <returns></returns>
    <Extension>
    Public Function LoadRawFileCache(files As ViewerProject,
                                     explorer As TreeView,
                                     rawMenu As ContextMenuStrip,
                                     targetRawMenu As ContextMenuStrip,
                                     scriptMenu As ContextMenuStrip) As Integer
        Dim i As Integer
        Dim scripts, rawFiles As TreeNode

#Disable Warning
        Call InitExplorerUI(explorer, rawMenu, scriptMenu, scripts, rawFiles)
#Enable Warning

        For Each raw As Raw In files.GetRawDataFiles
            Dim rawFileNode = RawFileNodeTemplate(raw, targetRawMenu)
            Dim name As String = rawFileNode.Text

            Call sharedProgressUpdater($"[Raw File Viewer] Loading {name}...")

            rawFiles.Nodes.Add(rawFileNode)
            'rawFileNode.addRawFile(raw, True, True)
            rawFileNode.Checked = False

            i += 1
        Next

        currentWorkspace = files

        If files.GetAutomationScripts.SafeQuery.Count > 0 Then
            For Each script As String In files.GetAutomationScripts
                Dim fileNode As New TreeNode(script.FileName) With {
                    .Checked = False,
                    .Tag = script,
                    .ImageIndex = 3,
                    .StateImageIndex = 3,
                    .SelectedImageIndex = 3,
                    .ContextMenuStrip = scriptMenu,
                    .ToolTipText = script.GetFullPath(throwEx:=False)
                }

                scripts.Nodes.Add(fileNode)
            Next
        End If

        Call loadRStudioScripts(explorer, scriptMenu)

        Return i
    End Function

    ''' <summary>
    ''' two root nodes:
    ''' 
    ''' 1. Raw Data Files
    ''' 2. R# Automation
    ''' </summary>
    ''' <param name="explorer"></param>
    ''' <param name="defaultWorkspace"></param>
    ''' <returns></returns>
    <Extension>
    Public Function LoadRawFileCache(explorer As TreeView,
                                     rawMenu As ContextMenuStrip,
                                     targetRawMenu As ContextMenuStrip,
                                     scriptMenu As ContextMenuStrip,
                                     Optional defaultWorkspace As String = Nothing) As Integer

        If defaultWorkspace.StringEmpty Then
            defaultWorkspace = Globals.defaultWorkspace
        End If

        If Not defaultWorkspace.FileExists Then
            currentWorkspace = New ViewerProject With {
                .FilePath = defaultWorkspace
            }

            Return 0
        Else
            Call sharedProgressUpdater("Load raw file list...")
        End If

        Dim files As ViewerProject = ViewerProject.LoadWorkspace(defaultWorkspace, sharedProgressUpdater)
        Dim i As Integer = files.LoadRawFileCache(explorer, rawMenu, targetRawMenu, scriptMenu)

        Return i
    End Function

    Private Sub loadRStudioScripts(explorer As TreeView, scriptMenu As ContextMenuStrip)
        Dim scripts As New TreeNode("R# Studio") With {
            .ImageIndex = 1,
            .SelectedImageIndex = 1,
            .StateImageIndex = 1,
            .ContextMenuStrip = scriptMenu
        }

        Dim folder As String = $"{App.HOME}/Rstudio/R"

        If Not folder.DirectoryExists Then
            ' development test
            folder = $"{App.HOME}/../../src/mzkit/setup/demo_script/"
        End If

        Call explorer.Nodes.Add(scripts)
        Call AddScript(scripts, dir:=folder, scriptMenu:=scriptMenu)
    End Sub

    Private Sub AddScript(folder As TreeNode, dir As String, scriptMenu As ContextMenuStrip)
        For Each subfolder As String In dir.ListDirectory
            Dim fileNode As New TreeNode(subfolder.DirectoryName) With {
                .Checked = False,
                .Tag = Nothing,
                .ImageIndex = 1,
                .StateImageIndex = 1,
                .SelectedImageIndex = 1,
                .ContextMenuStrip = scriptMenu
            }

            folder.Nodes.Add(fileNode)
            AddScript(fileNode, subfolder, scriptMenu)
        Next

        For Each script As String In dir.EnumerateFiles("*.R")
            Dim fileNode As New TreeNode(script.FileName) With {
                .Checked = False,
                .Tag = script,
                .ImageIndex = 3,
                .StateImageIndex = 3,
                .SelectedImageIndex = 3,
                .ContextMenuStrip = scriptMenu
            }

            folder.Nodes.Add(fileNode)
        Next
    End Sub

    ''' <summary>
    ''' 加载原始数据文件之中的ms1和ms2 scan树
    ''' 
    ''' ```
    ''' + ms1
    '''    + ms2
    '''    + ms2
    ''' + ms1
    '''    + ms2
    ''' ```
    ''' </summary>
    ''' <param name="rawFileNode"></param>
    ''' <param name="raw"></param>
    <Extension>
    Public Sub loadRawFile(rawFileNode As TreeView, raw As Raw, ByRef hasUVscans As Boolean, rtmin As Double, rtmax As Double)
        rawFileNode.Nodes.Clear()

        If Not raw.isLoaded Then
            Call raw.LoadMzpack(Sub(src, cache) frmFileExplorer.getRawCache(src,, cache))
        End If

        For Each scan As ScanMS1 In raw.GetMs1Scans.Where(Function(t) t.rt >= rtmin AndAlso t.rt <= rtmax)
            Dim scanNode As New TreeNode(scan.scan_id) With {
                .Tag = scan,
                .ImageIndex = 0
            }

            rawFileNode.Nodes.Add(scanNode)

            For Each ms2 As ScanMS2 In scan.products.SafeQuery
                Dim productNode As New TreeNode(ms2.scan_id) With {
                    .Tag = ms2,
                    .ImageIndex = 1,
                    .SelectedImageIndex = 1
                }

                scanNode.Nodes.Add(productNode)
            Next
        Next

        Dim UVscans As UVScan() = raw _
            .GetUVscans _
            .Where(Function(t)
                       Return t.scan_time >= rtmin AndAlso t.scan_time <= rtmax
                   End Function) _
            .ToArray

        If Not UVscans.IsNullOrEmpty Then
            WindowModules.UVScansList.DockState = DockState.DockLeftAutoHide
            WindowModules.UVScansList.Win7StyleTreeView1.Nodes.Clear()
            WindowModules.UVScansList.Clear()

            hasUVscans = True

            For Each scan As DataBinBox(Of UVScan) In CutBins _
                .FixedWidthBins(UVscans, 99, Function(x) x.scan_time) _
                .Where(Function(b) b.Count > 0)

                Dim scan_time = scan.Sample
                Dim spanNode As New TreeNode With {
                    .Text = $"scan_time: {CInt(scan_time.min)} ~ {CInt(scan_time.max)} sec"
                }

                For Each spectrum In scan.Raw
                    Call New TreeNode With {
                        .Text = spectrum.ToString,
                        .ImageIndex = 1,
                        .SelectedImageIndex = 1,
                        .Tag = spectrum
                    }.DoCall(AddressOf spanNode.Nodes.Add)
                Next

                Call WindowModules.UVScansList.Win7StyleTreeView1.Nodes.Add(spanNode)
            Next
        Else
            hasUVscans = False
        End If
    End Sub

    ''' <summary>
    ''' 这个函数总是返回当前选中的节点的文件根节点相关的数据
    ''' </summary>
    ''' <param name="explorer"></param>
    ''' <returns></returns>
    <Extension>
    Public Function CurrentRawFile(explorer As TreeView) As (raw As Raw, tree As TreeNode)
        Dim node = explorer.SelectedNode

        If node Is Nothing Then
            Return Nothing
        ElseIf TypeOf node.Tag Is Raw Then
            Return (DirectCast(node.Tag, Raw), node)
        Else
            Return (DirectCast(node.Parent.Tag, Raw), node.Parent)
        End If
    End Function

    Public Function CheckFormOpened(form As Form) As Boolean
        For i As Integer = 0 To Application.OpenForms.Count - 1
            If Application.OpenForms.Item(i) Is form Then
                Return True
            End If
        Next

        Return False
    End Function

    <Extension>
    Public Function GetXICMaxYAxis(raw As Raw) As Double
        Dim XIC As Double() = raw _
            .GetMs2Scans _
            .Select(Function(a) a.intensity) _
            .ToArray

        If XIC.Length = 0 Then
            Return 0
        Else
            Return XIC.Max
        End If
    End Function

    Public Function LoadIonLibrary() As IonLibrary
        If Not Globals.Settings.MRMLibfile.FileExists Then
            Globals.Settings.MRMLibfile = New Configuration.Settings().MRMLibfile
        End If

        Dim ionsLib As New IonLibrary(Globals.Settings.MRMLibfile.LoadCsv(Of IonPair))

        Return ionsLib
    End Function
End Module
