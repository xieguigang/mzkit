Public Class frmFileTree

    Friend WithEvents Label1 As New Label
    Friend WithEvents TextBox1 As New TextBox
    Friend WithEvents ToolTip1 As New ToolTip
    Friend WithEvents Button1 As New Button
    Friend WithEvents TreeView1 As New TreeView

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)

        ' Add any initialization after the InitializeComponent() call.
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 14)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(63, 13)
        Me.Label1.TabIndex = 12
        Me.Label1.Text = "m/z search:"

        Me.TextBox1.Location = New System.Drawing.Point(83, 11)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(144, 20)
        Me.TextBox1.TabIndex = 13
        Me.ToolTip1.SetToolTip(Me.TextBox1, "Input Exact Mass Or Compound Formula")

        Me.Button1.Location = New System.Drawing.Point(244, 9)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 14
        Me.Button1.Text = "Search"
        Me.Button1.UseVisualStyleBackColor = True

        Me.TreeView1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        '   Me.TreeView1.ContextMenuStrip = Me.contextMenuStrip1
        Me.TreeView1.ImageIndex = 0
        '   Me.TreeView1.ImageList = Me.ImageList1
        Me.TreeView1.Location = New System.Drawing.Point(6, 40)
        Me.TreeView1.Name = "TreeView1"
        Me.TreeView1.SelectedImageIndex = 0
        Me.TreeView1.Size = New System.Drawing.Size(313, 427)
        Me.TreeView1.TabIndex = 11

        Controls.Add(Button1)
        Controls.Add(TextBox1)
        Controls.Add(Label1)
        Controls.Add(TreeView1)
    End Sub
End Class