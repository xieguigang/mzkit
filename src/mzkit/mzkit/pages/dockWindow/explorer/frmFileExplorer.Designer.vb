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
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ChromatogramOverlapToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BPCOverlapToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TICOverlapToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImportsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RunAutomationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.DeleteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImageList2 = New System.Windows.Forms.ImageList(Me.components)
        Me.treeView1 = New mzkit.Kesoft.Windows.Forms.Win7StyleTreeView.Win7StyleTreeView(Me.components)
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripTextBox1 = New System.Windows.Forms.ToolStripTextBox()
        Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
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
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(201, 98)
        '
        'ChromatogramOverlapToolStripMenuItem
        '
        Me.ChromatogramOverlapToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BPCOverlapToolStripMenuItem, Me.TICOverlapToolStripMenuItem})
        Me.ChromatogramOverlapToolStripMenuItem.Image = CType(resources.GetObject("ChromatogramOverlapToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ChromatogramOverlapToolStripMenuItem.Name = "ChromatogramOverlapToolStripMenuItem"
        Me.ChromatogramOverlapToolStripMenuItem.Size = New System.Drawing.Size(200, 22)
        Me.ChromatogramOverlapToolStripMenuItem.Text = "Chromatogram Overlap"
        '
        'BPCOverlapToolStripMenuItem
        '
        Me.BPCOverlapToolStripMenuItem.Name = "BPCOverlapToolStripMenuItem"
        Me.BPCOverlapToolStripMenuItem.Size = New System.Drawing.Size(140, 22)
        Me.BPCOverlapToolStripMenuItem.Text = "BPC Overlap"
        '
        'TICOverlapToolStripMenuItem
        '
        Me.TICOverlapToolStripMenuItem.Name = "TICOverlapToolStripMenuItem"
        Me.TICOverlapToolStripMenuItem.Size = New System.Drawing.Size(140, 22)
        Me.TICOverlapToolStripMenuItem.Text = "TIC Overlap"
        '
        'ImportsToolStripMenuItem
        '
        Me.ImportsToolStripMenuItem.Image = CType(resources.GetObject("ImportsToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ImportsToolStripMenuItem.Name = "ImportsToolStripMenuItem"
        Me.ImportsToolStripMenuItem.Size = New System.Drawing.Size(200, 22)
        Me.ImportsToolStripMenuItem.Text = "Imports"
        '
        'RunAutomationToolStripMenuItem
        '
        Me.RunAutomationToolStripMenuItem.Image = Global.mzkit.My.Resources.Resources._42082
        Me.RunAutomationToolStripMenuItem.Name = "RunAutomationToolStripMenuItem"
        Me.RunAutomationToolStripMenuItem.Size = New System.Drawing.Size(200, 22)
        Me.RunAutomationToolStripMenuItem.Text = "Run Automation"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(197, 6)
        '
        'DeleteToolStripMenuItem
        '
        Me.DeleteToolStripMenuItem.Image = CType(resources.GetObject("DeleteToolStripMenuItem.Image"), System.Drawing.Image)
        Me.DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem"
        Me.DeleteToolStripMenuItem.Size = New System.Drawing.Size(200, 22)
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
        'treeView1
        '
        Me.treeView1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.treeView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.treeView1.HotTracking = True
        Me.treeView1.Location = New System.Drawing.Point(0, 25)
        Me.treeView1.Name = "treeView1"
        Me.treeView1.ShowLines = False
        Me.treeView1.Size = New System.Drawing.Size(800, 425)
        Me.treeView1.TabIndex = 2
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabel1, Me.ToolStripTextBox1, Me.ToolStripButton1})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(800, 25)
        Me.ToolStrip1.TabIndex = 3
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        Me.ToolStripLabel1.Size = New System.Drawing.Size(45, 22)
        Me.ToolStripLabel1.Text = "Search:"
        '
        'ToolStripTextBox1
        '
        Me.ToolStripTextBox1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.ToolStripTextBox1.Name = "ToolStripTextBox1"
        Me.ToolStripTextBox1.Size = New System.Drawing.Size(150, 25)
        '
        'ToolStripButton1
        '
        Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton1.Image = CType(resources.GetObject("ToolStripButton1.Image"), System.Drawing.Image)
        Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton1.Name = "ToolStripButton1"
        Me.ToolStripButton1.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton1.Text = "ToolStripButton1"
        '
        'frmFileExplorer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.treeView1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Name = "frmFileExplorer"
        Me.Text = "Form1"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
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
    Friend WithEvents treeView1 As Kesoft.Windows.Forms.Win7StyleTreeView.Win7StyleTreeView
    Friend WithEvents ToolStrip1 As ToolStrip
    Friend WithEvents ToolStripLabel1 As ToolStripLabel
    Friend WithEvents ToolStripTextBox1 As ToolStripTextBox
    Friend WithEvents ToolStripButton1 As ToolStripButton
End Class
