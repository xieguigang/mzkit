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
End Class