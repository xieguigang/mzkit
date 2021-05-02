Public Class MSSelector : Inherits RtRangeSelector

    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Private components As System.ComponentModel.IContainer
    Friend WithEvents TICToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BPCToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents PinToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
    Friend WithEvents FilterMs2ToolStripMenuItem As ToolStripMenuItem

    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MSSelector))
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.TICToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BPCToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.FilterMs2ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PinToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PinToolStripMenuItem, Me.ToolStripMenuItem2, Me.TICToolStripMenuItem, Me.BPCToolStripMenuItem, Me.ToolStripMenuItem1, Me.FilterMs2ToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(181, 126)
        '
        'TICToolStripMenuItem
        '
        Me.TICToolStripMenuItem.Checked = True
        Me.TICToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.TICToolStripMenuItem.Name = "TICToolStripMenuItem"
        Me.TICToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.TICToolStripMenuItem.Text = "TIC"
        '
        'BPCToolStripMenuItem
        '
        Me.BPCToolStripMenuItem.Name = "BPCToolStripMenuItem"
        Me.BPCToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.BPCToolStripMenuItem.Text = "BPC"
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
        'PinToolStripMenuItem
        '
        Me.PinToolStripMenuItem.Image = CType(resources.GetObject("PinToolStripMenuItem.Image"), System.Drawing.Image)
        Me.PinToolStripMenuItem.Name = "PinToolStripMenuItem"
        Me.PinToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.PinToolStripMenuItem.Text = "Pin"
        Me.PinToolStripMenuItem.ToolTipText = "Pin of RT Range"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(177, 6)
        '
        'MSSelector
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.Name = "MSSelector"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
End Class
