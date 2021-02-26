Imports mzkit.DockSample

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmSRMIonsExplorer
    Inherits ToolWindow

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSRMIonsExplorer))
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ShowTICOverlapToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ClearFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Win7StyleTreeView1 = New mzkit.Kesoft.Windows.Forms.Win7StyleTreeView.Win7StyleTreeView(Me.components)
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.ShowTICOverlap3DToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ShowTICOverlapToolStripMenuItem, Me.ShowTICOverlap3DToolStripMenuItem, Me.ToolStripMenuItem1, Me.ClearFilesToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(201, 98)
        '
        'ShowTICOverlapToolStripMenuItem
        '
        Me.ShowTICOverlapToolStripMenuItem.Image = CType(resources.GetObject("ShowTICOverlapToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ShowTICOverlapToolStripMenuItem.Name = "ShowTICOverlapToolStripMenuItem"
        Me.ShowTICOverlapToolStripMenuItem.Size = New System.Drawing.Size(200, 22)
        Me.ShowTICOverlapToolStripMenuItem.Text = "Show TIC Overlap"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(197, 6)
        '
        'ClearFilesToolStripMenuItem
        '
        Me.ClearFilesToolStripMenuItem.Image = CType(resources.GetObject("ClearFilesToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ClearFilesToolStripMenuItem.Name = "ClearFilesToolStripMenuItem"
        Me.ClearFilesToolStripMenuItem.Size = New System.Drawing.Size(200, 22)
        Me.ClearFilesToolStripMenuItem.Text = "Clear Files"
        '
        'Win7StyleTreeView1
        '
        Me.Win7StyleTreeView1.CheckBoxes = True
        Me.Win7StyleTreeView1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.Win7StyleTreeView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Win7StyleTreeView1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Win7StyleTreeView1.FullRowSelect = True
        Me.Win7StyleTreeView1.HotTracking = True
        Me.Win7StyleTreeView1.ImageIndex = 0
        Me.Win7StyleTreeView1.ImageList = Me.ImageList1
        Me.Win7StyleTreeView1.Location = New System.Drawing.Point(0, 25)
        Me.Win7StyleTreeView1.Name = "Win7StyleTreeView1"
        Me.Win7StyleTreeView1.SelectedImageIndex = 0
        Me.Win7StyleTreeView1.ShowLines = False
        Me.Win7StyleTreeView1.Size = New System.Drawing.Size(387, 447)
        Me.Win7StyleTreeView1.TabIndex = 1
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "folder-pictures.png")
        Me.ImageList1.Images.SetKeyName(1, "office_chart_area_256px_540041_easyicon.net.png")
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabel1})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(387, 25)
        Me.ToolStrip1.TabIndex = 2
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        Me.ToolStripLabel1.Size = New System.Drawing.Size(72, 22)
        Me.ToolStripLabel1.Text = "MRM Ions:"
        '
        'ShowTICOverlap3DToolStripMenuItem
        '
        Me.ShowTICOverlap3DToolStripMenuItem.Image = CType(resources.GetObject("ShowTICOverlap3DToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ShowTICOverlap3DToolStripMenuItem.Name = "ShowTICOverlap3DToolStripMenuItem"
        Me.ShowTICOverlap3DToolStripMenuItem.Size = New System.Drawing.Size(200, 22)
        Me.ShowTICOverlap3DToolStripMenuItem.Text = "Show TIC Overlap 3D"
        '
        'frmSRMIonsExplorer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(387, 472)
        Me.Controls.Add(Me.Win7StyleTreeView1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.DoubleBuffered = True
        Me.Name = "frmSRMIonsExplorer"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ShowTICOverlapToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Win7StyleTreeView1 As Kesoft.Windows.Forms.Win7StyleTreeView.Win7StyleTreeView
    Friend WithEvents ImageList1 As ImageList
    Friend WithEvents ToolStrip1 As ToolStrip
    Friend WithEvents ToolStripLabel1 As ToolStripLabel
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents ClearFilesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ShowTICOverlap3DToolStripMenuItem As ToolStripMenuItem
End Class
