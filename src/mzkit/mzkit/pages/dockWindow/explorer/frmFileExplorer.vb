Imports System.ComponentModel
Imports mzkit.Kesoft.Windows.Forms.Win7StyleTreeView
Imports mzkit.My
Imports RibbonLib.Interop
Imports Task

''' <summary>
''' 显示一个workspace对象里面所包含有的文件列表
''' </summary>
Public Class frmFileExplorer

    Friend WithEvents treeView1 As New Win7StyleTreeView

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        DoubleBuffered = True
    End Sub

    Private Sub frmFileExplorer_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        MyApplication.host.ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active
    End Sub

    Private Sub frmFileExplorer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.Hide()
    End Sub

    Public Sub selectRawFile(index As Integer)
        Dim raw As TreeNode = treeView1.Nodes.Item(0)

        treeView1.SelectedNode = raw.Nodes.Item(index)
    End Sub

    Public Function GetTotalCacheSize() As String
        Return treeView1.Nodes.Item(0).GetTotalCacheSize
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

    Private Sub frmFileExplorer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(treeView1)

        treeView1.Location = New Point(1, TextBox2.Height + 5)
        treeView1.Size = New Size(Width - 2, Me.Height - TextBox2.Height - 25)
        treeView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        treeView1.HotTracking = True
        treeView1.CheckBoxes = True
        '    treeView1.ContextMenuStrip = contextMenuStrip1
        treeView1.ShowLines = True
        treeView1.ShowRootLines = True

        '   ExportToolStripMenuItem.Text = "Export XIC Ions"

        Me.TabText = "File Explorer"
    End Sub
End Class