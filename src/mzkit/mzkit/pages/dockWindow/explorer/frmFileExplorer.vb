#Region "Microsoft.VisualBasic::0dfabc76be04a6868d54717ab8b21c5d, src\mzkit\mzkit\pages\dockWindow\explorer\frmFileExplorer.vb"

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

    ' Class frmFileExplorer
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: CurrentRawFile, deleteFileNode, (+2 Overloads) findRawFileNode, getRawCache, GetRawFiles
    '               GetSelectedRaws, GetTotalCacheSize
    ' 
    '     Sub: addFileNode, AddScript, BPCOverlapToolStripMenuItem_Click, Button1_Click, DeleteToolStripMenuItem_Click
    '          frmFileExplorer_Activated, frmFileExplorer_Closing, frmFileExplorer_Load, ImportsRaw, ImportsToolStripMenuItem_Click
    '          InitializeFileTree, OpenViewerToolStripMenuItem_Click, RawScatterToolStripMenuItem_Click, RunAutomationToolStripMenuItem_Click, SaveFileCache
    '          selectRawFile, showRawFile, TICOverlapToolStripMenuItem_Click, treeView1_AfterCheck, treeView1_AfterSelect
    '          treeView1_Click, treeView1_GotFocus, UpdateMainTitle, XICPeaksToolStripMenuItem_Click
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Threading
Imports Microsoft.VisualBasic.Text
Imports mzkit.Kesoft.Windows.Forms.Win7StyleTreeView
Imports mzkit.My
Imports RibbonLib.Interop
Imports Task
Imports Vip.Notification

''' <summary>
''' 显示一个workspace对象里面所包含有的文件列表
''' </summary>
Public Class frmFileExplorer

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        DoubleBuffered = True
    End Sub

    Private Sub frmFileExplorer_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        MyApplication.host.ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active
        Call treeView1_AfterSelect(Nothing, Nothing)
    End Sub

    Private Sub frmFileExplorer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.Hide()
    End Sub

    Public Sub selectRawFile(index As Integer)
        Dim raw As TreeNode = treeView1.Nodes.Item(0)

        treeView1.SelectedNode = raw.Nodes.Item(index)
        showRawFile(treeView1.SelectedNode.Tag, XIC:=False, directSnapshot:=True)
    End Sub

    Public Function GetTotalCacheSize() As String
        If treeView1.Nodes.Count > 0 Then
            Return treeView1.Nodes.Item(0).GetTotalCacheSize
        Else
            Return "0 KB"
        End If
    End Function

    Public Function CurrentRawFile() As Raw
        If treeView1.SelectedNode Is Nothing Then
            Return Nothing
        ElseIf treeView1.SelectedNode.Tag Is Nothing Then
            Return Nothing
        ElseIf TypeOf treeView1.SelectedNode.Tag Is String Then
            Return Nothing
        Else
            Return treeView1.SelectedNode.Tag
        End If
    End Function

    Public Iterator Function GetRawFiles() As IEnumerable(Of Raw)
        Dim rawList = treeView1.Nodes.Item(Scan0)

        For i As Integer = 0 To rawList.Nodes.Count - 1
            Yield DirectCast(rawList.Nodes(i).Tag, Raw)
        Next
    End Function

    Public Iterator Function GetSelectedRaws() As IEnumerable(Of Raw)
        Dim rawList = treeView1.Nodes.Item(Scan0)

        For i As Integer = 0 To rawList.Nodes.Count - 1
            If Not rawList.Nodes(i).Checked Then
                If Not rawList.Nodes(i) Is treeView1.SelectedNode Then
                    Continue For
                End If
            End If

            Dim raw As Raw = rawList.Nodes(i).Tag

            Yield raw
        Next
    End Function

    Sub InitializeFileTree()
        If treeView1.LoadRawFileCache(ctxMenuFiles, ctxMenuRawFile, ctxMenuScript, Globals.Settings.workspaceFile) = 0 Then
            MyApplication.host.showStatusMessage($"It seems that you don't have any raw file opended. You could open raw file through [File] -> [Open Raw File].", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            ' selectRawFile(Scan0)
            ' setCurrentFile()
        End If

        MyApplication.host.ToolStripStatusLabel2.Text = GetTotalCacheSize()
    End Sub

    Private Sub frmFileExplorer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(treeView1)

        treeView1.HotTracking = True
        treeView1.BringToFront()
        treeView1.CheckBoxes = True
        treeView1.ContextMenuStrip = ctxMenuFiles
        treeView1.ShowLines = True
        treeView1.ShowRootLines = True
        treeView1.BorderStyle = BorderStyle.FixedSingle
        treeView1.Dock = DockStyle.Fill
        treeView1.ImageList = ImageList2

        ToolStrip1.Stretch = True

        '   ExportToolStripMenuItem.Text = "Export XIC Ions"

        Me.TabText = "File Explorer"

        Call InitializeFileTree()
        Call ApplyVsTheme(ctxMenuFiles, ToolStrip1, ctxMenuScript, ctxMenuRawFile)
    End Sub

    Public Sub ImportsRaw(fileName As String)
        If treeView1.Nodes.Item(0).Nodes.Count = 0 Then
            Call addFileNode(getRawCache(fileName))
        Else
            ' work in background
            Dim taskList As TaskListWindow = MyApplication.host.taskWin
            Dim task As TaskUI = taskList.Add("Imports Raw Data", fileName)

            Call taskList.Show(MyApplication.host.dockPanel)
            ' Call Alert.ShowSucess($"Imports raw data files in background,{vbCrLf}you can open [Task List] panel for view task progress.")
            Call MyApplication.TaskQueue.AddToQueue(
                Sub()
                    Call task.Running()

                    Dim importsTask As New Task.ImportsRawData(
                        file:=fileName,
                        progress:=Sub(msg)
                                      ' do nothing
                                      Call task.ProgressMessage(msg)
                                  End Sub,
                        finished:=Sub()
                                      Call task.Finish()
                                  End Sub)

                    importsTask.RunImports()
                    addFileNode(importsTask.raw)
                End Sub)
        End If
    End Sub

    Public Sub addFileNode(newRaw As Raw)
        Me.Invoke(Sub()
                      treeView1.Nodes(0).Nodes.Add(New TreeNode(newRaw.source.FileName) With {.Tag = newRaw, .ImageIndex = 1, .SelectedImageIndex = 1})
                  End Sub)

        Globals.workspace.Add(newRaw)

        MyApplication.host.showStatusMessage("Ready!")
        MyApplication.host.UpdateCacheSize(GetTotalCacheSize)
    End Sub

    ''' <summary>
    ''' do raw data file imports task
    ''' </summary>
    ''' <param name="fileName"></param>
    ''' <returns></returns>
    Public Function getRawCache(fileName As String) As Raw
        Dim progress As New frmTaskProgress() With {.Text = $"Imports raw data [{fileName}]"}
        Dim showProgress As Action(Of String) = AddressOf progress.ShowProgressDetails
        Dim task As New Task.ImportsRawData(fileName, showProgress, Sub() Call progress.Invoke(Sub() progress.Close()))
        Dim runTask As New Thread(AddressOf task.RunImports)

        MyApplication.host.showStatusMessage("Run Raw Data Imports")
        progress.ShowProgressTitle(progress.Text, directAccess:=True)

        Call runTask.Start()
        Call progress.ShowDialog()

        'Call New frmRawViewer() With {
        '    .MdiParent = Me,
        '    .Text = file.FileName,
        '    .rawFile = task.raw
        '}.Show()
        Return task.raw
    End Function


    Public Sub SaveFileCache(progress As Action(Of String))
        Call treeView1.SaveRawFileCache(progress)
    End Sub

    Dim lockFileDelete As Boolean = False

    Public Sub showRawFile(raw As Raw, XIC As Boolean, directSnapshot As Boolean)
        If lockFileDelete Then
            Return
        End If

        Call MyApplication.host.rawFeaturesList.LoadRaw(raw)
        Call MyApplication.host.mzkitTool.showScatter(raw, XIC, directSnapshot)

        Call VisualStudio.ShowProperties(New RawFileProperty(raw))
        Call UpdateMainTitle(raw.source)
    End Sub

    Private Sub RawScatterToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RawScatterToolStripMenuItem.Click
        If treeView1.SelectedNode Is Nothing Then
            Return
        End If

        If TypeOf treeView1.SelectedNode.Tag Is Raw Then
            Call showRawFile(DirectCast(treeView1.SelectedNode.Tag, Raw), XIC:=False, directSnapshot:=False)
        End If
    End Sub

    Private Sub XICPeaksToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles XICPeaksToolStripMenuItem.Click
        If treeView1.SelectedNode Is Nothing Then
            Return
        End If

        If TypeOf treeView1.SelectedNode.Tag Is Raw Then
            Call showRawFile(DirectCast(treeView1.SelectedNode.Tag, Raw), XIC:=True, directSnapshot:=False)
        End If
    End Sub

    Public Sub UpdateMainTitle(source As String)
        If source.Any(Function(c) c = ASCII.NUL) Then
            MyApplication.host.Text = $"BioNovoGene Mzkit [{source.Where(Function(c) AscW(c) >= 32).CharString}]"
        Else
            MyApplication.host.Text = $"BioNovoGene Mzkit [{source.GetFullPath}]"
        End If
    End Sub

    Public Sub AddScript(script As String)
        treeView1.Nodes(1).Nodes.Add(New TreeNode(script.FileName) With {.Tag = script})
    End Sub

    Private Sub treeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles treeView1.AfterSelect
        If treeView1.SelectedNode Is Nothing Then
            Return
        End If

        'If treeView1.SelectedNode Is treeView1.Nodes(0) Then
        '    treeView1.ContextMenuStrip = ContextMenuStrip1
        'ElseIf treeView1.SelectedNode Is treeView1.Nodes(1) Then
        '    treeView1.ContextMenuStrip = ContextMenuStrip2
        'End If

        If TypeOf treeView1.SelectedNode.Tag Is Raw Then
            Call showRawFile(DirectCast(treeView1.SelectedNode.Tag, Raw), XIC:=False, directSnapshot:=True)

            '  treeView1.ContextMenuStrip = ContextMenuStrip1

        ElseIf TypeOf treeView1.SelectedNode.Tag Is String Then
            ' 选择了一个脚本文件
            Dim path As String = DirectCast(treeView1.SelectedNode.Tag, String).GetFullPath
            Dim script = MyApplication.host.scriptFiles _
                .Where(Function(a) a.scriptFile.GetFullPath = path) _
                .FirstOrDefault

            '  treeView1.ContextMenuStrip = ContextMenuStrip2

            If Not script Is Nothing Then
                script.Show(MyApplication.host.dockPanel)
                MyApplication.host.Text = $"BioNovoGene Mzkit [{path.GetFullPath}]"
            ElseIf path.FileExists Then
                ' 脚本文件还没有被打开
                ' 在这里打开脚本文件
                MyApplication.host.openRscript(path)
                MyApplication.host.Text = $"BioNovoGene Mzkit [{path.GetFullPath}]"
            Else
                MyApplication.host.showStatusMessage($"script file '{path.FileName}' is not exists...", My.Resources.StatusAnnotations_Warning_32xLG_color)
                e.Node.ImageIndex = 4
                e.Node.SelectedImageIndex = 4
                e.Node.StateImageIndex = 4
            End If
        End If
    End Sub

    Private Sub treeView1_AfterCheck(sender As Object, e As TreeViewEventArgs) Handles treeView1.AfterCheck
        If e.Node.Tag Is Nothing Then
            ' 是顶层的节点
            For Each fileNode As TreeNode In e.Node.Nodes
                fileNode.Checked = e.Node.Checked
            Next
        Else
            ' do nothing
        End If
    End Sub

    Private Sub treeView1_GotFocus(sender As Object, e As EventArgs) Handles treeView1.GotFocus
        ' Call treeView1_AfterSelect(Nothing, Nothing)
    End Sub

    Private Sub TICOverlapToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TICOverlapToolStripMenuItem.Click
        MyApplication.host.mzkitTool.TIC(isBPC:=False)
    End Sub

    Private Sub BPCOverlapToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BPCOverlapToolStripMenuItem.Click
        MyApplication.host.mzkitTool.TIC(isBPC:=True)
    End Sub

    ''' <summary>
    ''' 将原始数据文件从当前工作区移除
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub DeleteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteToolStripMenuItem.Click
        Dim fileList As New List(Of TreeNode)

        For Each node As TreeNode In treeView1.Nodes(0).Nodes
            If node.Checked Then
                fileList.Add(node)
            End If
        Next

        For Each node As TreeNode In treeView1.Nodes(1).Nodes
            If node.Checked Then
                fileList.Add(node)
            End If
        Next

        If fileList.Count = 0 AndAlso treeView1.SelectedNode Is Nothing Then
            Return
        ElseIf fileList.Count = 0 Then
            Call deleteFileNode(node:=treeView1.SelectedNode, confirmDialog:=True)
        ElseIf MessageBox.Show($"Confirm to remove {fileList.Count} files from current workspace?", "File Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            lockFileDelete = True

            For Each file In fileList
                Call deleteFileNode(file, confirmDialog:=False)
            Next

            lockFileDelete = False
        End If
    End Sub

    Public Function findRawFileNode(sourceName As String) As TreeNode
        If treeView1.Nodes.Count = 0 Then
            Return Nothing
        End If

        For Each node As TreeNode In treeView1.Nodes(0).Nodes
            If DirectCast(node.Tag, Raw).source.FileName = sourceName Then
                Return node
            End If
        Next

        Return Nothing
    End Function

    Public Function findRawFileNode(raw As Raw) As TreeNode
        For Each node As TreeNode In treeView1.Nodes(0).Nodes
            If node.Tag Is raw Then
                Return node
            End If
        Next

        Return Nothing
    End Function

    Public Function deleteFileNode(node As TreeNode, confirmDialog As Boolean) As DialogResult
        ' 跳过根节点
        If node Is Nothing OrElse node.Tag Is Nothing Then
            Return DialogResult.No
        End If

        Dim fileName As String

        If TypeOf node.Tag Is Raw Then
            fileName = DirectCast(node.Tag, Raw).source.FileName
        Else
            fileName = DirectCast(node.Tag, String).FileName
        End If

        Dim opt As DialogResult

        If confirmDialog Then
            opt = MessageBox.Show(
                text:=$"Going to removes {fileName} from your workspace?",
                caption:="Delete workspace file",
                buttons:=MessageBoxButtons.YesNo,
                icon:=MessageBoxIcon.Question
            )
        Else
            opt = DialogResult.Yes
        End If

        If opt = DialogResult.Yes Then
            If TypeOf node.Tag Is Raw Then
                treeView1.Nodes(0).Nodes.Remove(node)
            Else
                treeView1.Nodes(1).Nodes.Remove(node)
            End If
        End If

        MyApplication.host.ToolStripStatusLabel2.Text = GetTotalCacheSize()

        Return opt
    End Function

    Private Sub RunAutomationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RunAutomationToolStripMenuItem1.Click
        If treeView1.SelectedNode Is Nothing OrElse treeView1.SelectedNode.Tag Is Nothing OrElse Not TypeOf treeView1.SelectedNode.Tag Is String Then
            Return
        End If

        Dim scriptFile As String = DirectCast(treeView1.SelectedNode.Tag, String)

        Call MyApplication.RtermPage.ShowPage()
        Call MyApplication.ExecuteRScript(scriptFile, isFile:=True, AddressOf MyApplication.host.output.AppendRoutput)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Dim raws As New List(Of Raw)

        For Each node As TreeNode In treeView1.Nodes(0).Nodes
            raws.Add(node.Tag)
        Next

        Call FeatureSearchHandler.SearchByMz(ToolStripSpringTextBox1.Text, raws, False)
    End Sub

    Private Sub ImportsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImportsToolStripMenuItem.Click
        Call MyApplication.host.ImportsFiles()
    End Sub

    Private Sub treeView1_Click(sender As Object, e As EventArgs) Handles treeView1.Click

    End Sub

    Private Sub OpenViewerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenViewerToolStripMenuItem.Click
        Dim node = treeView1.SelectedNode

        If node Is Nothing OrElse TypeOf node.Tag IsNot Raw Then
            Return
        End If

        Dim raw As Raw = DirectCast(node.Tag, Raw).LoadMzpack
        Dim viewer = VisualStudio.ShowDocument(Of frmUntargettedViewer)()

        viewer.loadRaw(raw)
    End Sub
End Class
