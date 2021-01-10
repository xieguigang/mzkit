Imports mzkit.DockSample

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMsImagingTweaks
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMsImagingTweaks))
        Me.PropertyGrid1 = New System.Windows.Forms.PropertyGrid()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.RenderingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearSelectionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.Win7StyleTreeView1 = New mzkit.Kesoft.Windows.Forms.Win7StyleTreeView.Win7StyleTreeView(Me.components)
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripSpringTextBox1 = New mzkit.ToolStripSpringTextBox()
        Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
        Me.ContextMenuStrip1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'PropertyGrid1
        '
        Me.PropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PropertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar
        Me.PropertyGrid1.Location = New System.Drawing.Point(0, 0)
        Me.PropertyGrid1.Name = "PropertyGrid1"
        Me.PropertyGrid1.Size = New System.Drawing.Size(377, 261)
        Me.PropertyGrid1.TabIndex = 0
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RenderingToolStripMenuItem, Me.ClearSelectionToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(153, 48)
        '
        'RenderingToolStripMenuItem
        '
        Me.RenderingToolStripMenuItem.Image = CType(resources.GetObject("RenderingToolStripMenuItem.Image"), System.Drawing.Image)
        Me.RenderingToolStripMenuItem.Name = "RenderingToolStripMenuItem"
        Me.RenderingToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.RenderingToolStripMenuItem.Text = "Rendering"
        '
        'ClearSelectionToolStripMenuItem
        '
        Me.ClearSelectionToolStripMenuItem.Image = CType(resources.GetObject("ClearSelectionToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ClearSelectionToolStripMenuItem.Name = "ClearSelectionToolStripMenuItem"
        Me.ClearSelectionToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.ClearSelectionToolStripMenuItem.Text = "Clear Selection"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 25)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.Win7StyleTreeView1)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.PropertyGrid1)
        Me.SplitContainer1.Size = New System.Drawing.Size(377, 529)
        Me.SplitContainer1.SplitterDistance = 264
        Me.SplitContainer1.TabIndex = 2
        '
        'Win7StyleTreeView1
        '
        Me.Win7StyleTreeView1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.Win7StyleTreeView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Win7StyleTreeView1.FullRowSelect = True
        Me.Win7StyleTreeView1.HotTracking = True
        Me.Win7StyleTreeView1.Location = New System.Drawing.Point(0, 0)
        Me.Win7StyleTreeView1.Name = "Win7StyleTreeView1"
        Me.Win7StyleTreeView1.Size = New System.Drawing.Size(377, 264)
        Me.Win7StyleTreeView1.TabIndex = 0
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabel1, Me.ToolStripSpringTextBox1, Me.ToolStripButton1})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(377, 25)
        Me.ToolStrip1.TabIndex = 3
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        Me.ToolStripLabel1.Size = New System.Drawing.Size(74, 22)
        Me.ToolStripLabel1.Text = "Ion Features:"
        '
        'ToolStripSpringTextBox1
        '
        Me.ToolStripSpringTextBox1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.ToolStripSpringTextBox1.Name = "ToolStripSpringTextBox1"
        Me.ToolStripSpringTextBox1.Size = New System.Drawing.Size(237, 25)
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
        'frmMsImagingTweaks
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(377, 554)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Name = "frmMsImagingTweaks"
        Me.Text = "Form1"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents PropertyGrid1 As PropertyGrid
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents RenderingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents ClearSelectionToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStrip1 As ToolStrip
    Friend WithEvents ToolStripLabel1 As ToolStripLabel
    Friend WithEvents ToolStripSpringTextBox1 As ToolStripSpringTextBox
    Friend WithEvents ToolStripButton1 As ToolStripButton
    Friend WithEvents Win7StyleTreeView1 As Kesoft.Windows.Forms.Win7StyleTreeView.Win7StyleTreeView
End Class
