Imports mzkit.DockSample

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmFileExplorer
    Inherits ToolWindow
    ' Inherits Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFileExplorer))
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ChromatogramOverlapToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BPCOverlapToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TICOverlapToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RunAutomationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.DeleteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImageList2 = New System.Windows.Forms.ImageList(Me.components)
        Me.ImportsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Panel1.SuspendLayout()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TextBox2
        '
        Me.TextBox2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox2.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox2.Location = New System.Drawing.Point(4, 6)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(759, 22)
        Me.TextBox2.TabIndex = 0
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.White
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.Button1)
        Me.Panel1.Controls.Add(Me.TextBox2)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(800, 34)
        Me.Panel1.TabIndex = 1
        '
        'Button1
        '
        Me.Button1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button1.BackColor = System.Drawing.Color.White
        Me.Button1.FlatAppearance.BorderSize = 0
        Me.Button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button1.ImageIndex = 0
        Me.Button1.ImageList = Me.ImageList1
        Me.Button1.Location = New System.Drawing.Point(768, 5)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(24, 24)
        Me.Button1.TabIndex = 1
        Me.Button1.UseVisualStyleBackColor = False
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "edit-find.png")
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ChromatogramOverlapToolStripMenuItem, Me.ImportsToolStripMenuItem, Me.RunAutomationToolStripMenuItem, Me.ToolStripMenuItem1, Me.DeleteToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(216, 120)
        '
        'ChromatogramOverlapToolStripMenuItem
        '
        Me.ChromatogramOverlapToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BPCOverlapToolStripMenuItem, Me.TICOverlapToolStripMenuItem})
        Me.ChromatogramOverlapToolStripMenuItem.Image = CType(resources.GetObject("ChromatogramOverlapToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ChromatogramOverlapToolStripMenuItem.Name = "ChromatogramOverlapToolStripMenuItem"
        Me.ChromatogramOverlapToolStripMenuItem.Size = New System.Drawing.Size(215, 22)
        Me.ChromatogramOverlapToolStripMenuItem.Text = "Chromatogram Overlap"
        '
        'BPCOverlapToolStripMenuItem
        '
        Me.BPCOverlapToolStripMenuItem.Name = "BPCOverlapToolStripMenuItem"
        Me.BPCOverlapToolStripMenuItem.Size = New System.Drawing.Size(149, 22)
        Me.BPCOverlapToolStripMenuItem.Text = "BPC Overlap"
        '
        'TICOverlapToolStripMenuItem
        '
        Me.TICOverlapToolStripMenuItem.Name = "TICOverlapToolStripMenuItem"
        Me.TICOverlapToolStripMenuItem.Size = New System.Drawing.Size(149, 22)
        Me.TICOverlapToolStripMenuItem.Text = "TIC Overlap"
        '
        'RunAutomationToolStripMenuItem
        '
        Me.RunAutomationToolStripMenuItem.Image = Global.mzkit.My.Resources.Resources._42082
        Me.RunAutomationToolStripMenuItem.Name = "RunAutomationToolStripMenuItem"
        Me.RunAutomationToolStripMenuItem.Size = New System.Drawing.Size(215, 22)
        Me.RunAutomationToolStripMenuItem.Text = "Run Automation"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(212, 6)
        '
        'DeleteToolStripMenuItem
        '
        Me.DeleteToolStripMenuItem.Image = CType(resources.GetObject("DeleteToolStripMenuItem.Image"), System.Drawing.Image)
        Me.DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem"
        Me.DeleteToolStripMenuItem.Size = New System.Drawing.Size(215, 22)
        Me.DeleteToolStripMenuItem.Text = "Delete"
        '
        'ImageList2
        '
        Me.ImageList2.ImageStream = CType(resources.GetObject("ImageList2.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList2.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList2.Images.SetKeyName(0, "folder-pictures.png")
        Me.ImageList2.Images.SetKeyName(1, "folder-documents.png")
        Me.ImageList2.Images.SetKeyName(2, "application-x-object.png")
        Me.ImageList2.Images.SetKeyName(3, "text-x-generic.png")
        Me.ImageList2.Images.SetKeyName(4, "StatusAnnotations_Warning_32xLG_color.png")
        '
        'ImportsToolStripMenuItem
        '
        Me.ImportsToolStripMenuItem.Image = CType(resources.GetObject("ImportsToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ImportsToolStripMenuItem.Name = "ImportsToolStripMenuItem"
        Me.ImportsToolStripMenuItem.Size = New System.Drawing.Size(215, 22)
        Me.ImportsToolStripMenuItem.Text = "Imports"
        '
        'frmFileExplorer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "frmFileExplorer"
        Me.Text = "Form1"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Button1 As Button
    Friend WithEvents ImageList1 As ImageList
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents DeleteToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ChromatogramOverlapToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BPCOverlapToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TICOverlapToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RunAutomationToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ImageList2 As ImageList
    Friend WithEvents ImportsToolStripMenuItem As ToolStripMenuItem
End Class
