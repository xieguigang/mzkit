<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmUntargettedViewer
    Inherits DocumentWindow

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
        Me.RtRangeSelector1 = New mzkit.RtRangeSelector()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.TICToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BPCToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.FilterMs2ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuStrip1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RtRangeSelector1
        '
        Me.RtRangeSelector1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.RtRangeSelector1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.RtRangeSelector1.FillColor = System.Drawing.Color.DodgerBlue
        Me.RtRangeSelector1.Location = New System.Drawing.Point(0, 318)
        Me.RtRangeSelector1.Name = "RtRangeSelector1"
        Me.RtRangeSelector1.SelectedColor = System.Drawing.Color.Black
        Me.RtRangeSelector1.Size = New System.Drawing.Size(800, 132)
        Me.RtRangeSelector1.TabIndex = 0
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TICToolStripMenuItem, Me.BPCToolStripMenuItem, Me.ToolStripMenuItem1, Me.FilterMs2ToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(181, 98)
        '
        'TICToolStripMenuItem
        '
        Me.TICToolStripMenuItem.Checked = True
        Me.TICToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.TICToolStripMenuItem.Name = "TICToolStripMenuItem"
        Me.TICToolStripMenuItem.Size = New System.Drawing.Size(96, 22)
        Me.TICToolStripMenuItem.Text = "TIC"
        '
        'BPCToolStripMenuItem
        '
        Me.BPCToolStripMenuItem.Name = "BPCToolStripMenuItem"
        Me.BPCToolStripMenuItem.Size = New System.Drawing.Size(96, 22)
        Me.BPCToolStripMenuItem.Text = "BPC"
        '
        'PictureBox1
        '
        Me.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PictureBox1.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(800, 318)
        Me.PictureBox1.TabIndex = 1
        Me.PictureBox1.TabStop = False
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(177, 6)
        '
        'FilterMs2ToolStripMenuItem
        '
        Me.FilterMs2ToolStripMenuItem.Name = "FilterMs2ToolStripMenuItem"
        Me.FilterMs2ToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.FilterMs2ToolStripMenuItem.Text = "Filter Ms2"
        '
        'frmUntargettedViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.RtRangeSelector1)
        Me.DoubleBuffered = True
        Me.Name = "frmUntargettedViewer"
        Me.ContextMenuStrip1.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents RtRangeSelector1 As RtRangeSelector
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents TICToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BPCToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents FilterMs2ToolStripMenuItem As ToolStripMenuItem
End Class
