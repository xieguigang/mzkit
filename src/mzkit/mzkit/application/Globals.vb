#Region "Microsoft.VisualBasic::d2cf903fd8753621a16453fa43907ff7, application\Globals.vb"

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

    ' Module Globals
    ' 
    '     Properties: Settings, workspace
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: CheckFormOpened, CurrentRawFile, FindRaws, GetColors, GetTotalCacheSize
    '               GetXICMaxYAxis, LoadIonLibrary, LoadRawFileCache
    ' 
    '     Sub: AddRecentFileHistory, loadRawFile, SaveRawFileCache
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Distributions.BinBox
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports mzkit.Configuration
Imports mzkit.My
Imports Task
Imports WeifenLuo.WinFormsUI.Docking

Module Globals

    ''' <summary>
    ''' 这个是未进行任何工作区保存所保存的一个默认的临时文件的位置
    ''' </summary>
    Dim defaultWorkspace As String = App.LocalData & "/cacheList.dat"
    Dim currentWorkspace As ViewerProject

    Public ReadOnly Property Settings As Settings

    Public ReadOnly Property workspace As ViewerProject
        Get
            Return currentWorkspace
        End Get
    End Property

    Sub New()
        Settings = Settings.GetConfiguration()
    End Sub

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
        Dim rawFileNodes = explorer.Nodes(Scan0)

        For Each node As TreeNode In rawFileNodes.Nodes
            files.Add(node.Tag)
            progress(files.Last.source)
            Application.DoEvents()
        Next

        Dim scripts As New List(Of String)

        If explorer.Nodes.Count > 1 Then
            For Each node As TreeNode In explorer.Nodes(1).Nodes
                scripts.Add(DirectCast(node.Tag, String).GetFullPath)
            Next
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
        For Each node As TreeNode In explorer.Nodes
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
                                     scriptMenu As ContextMenuStrip,
                                     Optional defaultWorkspace As String = Nothing) As Integer

        Dim scripts As New TreeNode("R# Automation") With {
            .ImageIndex = 1,
            .SelectedImageIndex = 1,
            .StateImageIndex = 1,
            .ContextMenuStrip = rawMenu
        }
        Dim rawFiles As New TreeNode("Raw Data Files") With {
            .ImageIndex = 0,
            .StateImageIndex = 0,
            .SelectedImageIndex = 0,
            .ContextMenuStrip = scriptMenu
        }

        explorer.Nodes.Add(rawFiles)
        explorer.Nodes.Add(scripts)

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
        Dim i As Integer

        For Each raw As Raw In files.GetRawDataFiles
            Call sharedProgressUpdater($"[Raw File Viewer] Loading {raw.source.FileName}...")

            Dim rawFileNode As New TreeNode($"{raw.source.FileName} [{raw.numOfScans} Scans]") With {
                .Checked = True,
                .Tag = raw,
                .ImageIndex = 2,
                .SelectedImageIndex = 2,
                .StateImageIndex = 2,
                .ContextMenuStrip = rawMenu
            }

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
                    .ContextMenuStrip = scriptMenu
                }

                scripts.Nodes.Add(fileNode)
            Next
        End If

        Call loadRStudioScripts(explorer, scriptMenu)

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
    Public Sub loadRawFile(rawFileNode As TreeView, raw As Raw, ByRef hasUVscans As Boolean)
        rawFileNode.Nodes.Clear()

        For Each scan As Ms1ScanEntry In raw.scans
            Dim scanNode As New TreeNode(scan.id) With {
                .Tag = scan,
                .ImageIndex = 0
            }

            rawFileNode.Nodes.Add(scanNode)

            For Each ms2 As ScanEntry In scan.products.SafeQuery
                Dim productNode As New TreeNode(ms2.id) With {
                    .Tag = ms2,
                    .ImageIndex = 1,
                    .SelectedImageIndex = 1
                }

                scanNode.Nodes.Add(productNode)
            Next
        Next

        If Not raw.UVscans.IsNullOrEmpty Then
            MyApplication.host.UVScansList.DockState = DockState.DockLeftAutoHide
            MyApplication.host.UVScansList.Win7StyleTreeView1.Nodes.Clear()
            MyApplication.host.UVScansList.Clear()

            hasUVscans = True

            For Each scan As DataBinBox(Of UVScan) In CutBins.FixedWidthBins(raw.UVscans, 99, Function(x) x.scan_time)
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

                Call MyApplication.host.UVScansList.Win7StyleTreeView1.Nodes.Add(spanNode)
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
            .Select(Function(a) a.XIC) _
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
