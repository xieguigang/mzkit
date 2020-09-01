Imports System.ComponentModel
Imports mzkit.Kesoft.Windows.Forms.Win7StyleTreeView
Imports mzkit.My
Imports RibbonLib.Interop
Imports Task

Public Class frmFileTree

    Friend WithEvents treeView1 As New Win7StyleTreeView

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        DoubleBuffered = True
    End Sub

    Private Sub frmFileTree_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        MyApplication.host.ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active
    End Sub

    Private Sub frmFileTree_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub frmFileTree_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(treeView1)

        treeView1.Location = New Point(1, TextBox2.Height + 5)
        treeView1.Size = New Size(Width - 2, Me.Height - TextBox2.Height - 25)
        treeView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        treeView1.HotTracking = True
        treeView1.CheckBoxes = True
        treeView1.ContextMenuStrip = ContextMenuStrip1
        treeView1.ShowLines = True
        treeView1.ShowRootLines = True

        ExportToolStripMenuItem.Text = "Export XIC Ions"

        Me.TabText = "File Explorer"
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    Dim checked As New List(Of TreeNode)

    ''' <summary>
    ''' 不包含 root node
    ''' </summary>
    ''' <returns></returns>
    Public Function GetSelectedNodes() As IEnumerable(Of TreeNode)
        Return checked.AsEnumerable
    End Function

    Dim lockCheckList As Boolean

    Public Sub Clear()
        lockCheckList = True

        For i As Integer = 0 To checked.Count - 1
            checked(i).Checked = False
        Next

        checked.Clear()
        lockCheckList = False
    End Sub

    Private Sub TreeView1_AfterCheck(sender As Object, e As TreeViewEventArgs) Handles treeView1.AfterCheck
        If lockCheckList Then
            Return
        End If

        If TypeOf e.Node.Tag Is Raw Then
            Dim checked As Boolean = e.Node.Checked
            Dim node As TreeNode

            For i As Integer = 0 To e.Node.Nodes.Count - 1
                node = e.Node.Nodes(i)
                node.Checked = checked

                If checked Then
                    Me.checked.Add(node)
                    Me.checked.Remove(node)
                End If
            Next
        Else
            If e.Node.Checked Then
                checked.Add(e.Node)
            Else
                checked.Remove(e.Node)
            End If
        End If

        ClearToolStripMenuItem.Text = $"Clear [{checked.Count} XIC Ions]"
    End Sub

    Private Sub ClearSelectionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearSelectionsToolStripMenuItem.Click
        lockCheckList = True
        checked.Clear()

        For i As Integer = 0 To treeView1.Nodes.Count - 1
            treeView1.Nodes(i).Checked = False

            If Not treeView1.Nodes(i).Nodes Is Nothing Then
                For j As Integer = 0 To treeView1.Nodes(i).Nodes.Count - 1
                    treeView1.Nodes(i).Nodes(j).Checked = False
                Next
            End If
        Next

        lockCheckList = False
    End Sub

    Private Sub DeleteFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteFileToolStripMenuItem.Click
        Dim current = treeView1.CurrentRawFile

        If Not current.raw Is Nothing Then
            If MessageBox.Show($"Going to remove the raw data file [{current.raw.source.FileName}]?", "Delete File", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.OK Then
                treeView1.Nodes.Remove(current.tree)
                treeView1.SaveRawFileCache

                Call MyApplication.host.mzkitTool.setCurrentFile()
            End If
        End If

        If treeView1.Nodes.Count = 0 Then
            MyApplication.host.showStatusMessage("No raw file for removes!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Do While treeView1.Nodes.Count > 0
                Dim clearOne As Boolean = False

                For i As Integer = 0 To treeView1.Nodes.Count - 1
                    If treeView1.Nodes(i).Checked Then
                        treeView1.Nodes.Remove(treeView1.Nodes(i))
                        clearOne = True
                        Exit For
                    End If
                Next

                If Not clearOne Then
                    Exit Do
                End If
            Loop
        End If

        MyApplication.host.ToolStripStatusLabel2.Text = treeView1.GetTotalCacheSize
    End Sub

    Private Sub SelectAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectAllToolStripMenuItem.Click
        lockCheckList = True
        checked.Clear()

        For i As Integer = 0 To treeView1.Nodes.Count - 1
            treeView1.Nodes(i).Checked = True

            If Not treeView1.Nodes(i).Nodes Is Nothing Then
                For j As Integer = 0 To treeView1.Nodes(i).Nodes.Count - 1
                    treeView1.Nodes(i).Nodes(j).Checked = True
                    checked.Add(treeView1.Nodes(i).Nodes(j))
                Next
            End If
        Next

        lockCheckList = False
    End Sub

    Private Sub treeView1_BeforeCollapse(sender As Object, e As TreeViewCancelEventArgs) Handles treeView1.BeforeCollapse
        e.Node.Nodes.Clear()
        e.Node.Nodes.Add(New TreeNode With {.Text = "n/a"})
    End Sub

    Private Sub treeView1_BeforeExpand(sender As Object, e As TreeViewCancelEventArgs) Handles treeView1.BeforeExpand
        e.Node.Nodes.Clear()

        For Each scan In DirectCast(e.Node.Tag, Raw).scans
            e.Node.Nodes.Add(New TreeNode With {.Tag = scan.id, .Text = scan.id, .Checked = e.Node.Checked})
        Next
    End Sub
End Class