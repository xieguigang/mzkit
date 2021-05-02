<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MSSelector
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MSSelector))
        Me.RtRangeSelector1 = New ControlLibrary.RtRangeSelector()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ResetToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PinToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.TICToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BPCToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.FilterMs2ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'RtRangeSelector1
        '
        Me.RtRangeSelector1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.RtRangeSelector1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RtRangeSelector1.FillColor = System.Drawing.Color.Blue
        Me.RtRangeSelector1.Location = New System.Drawing.Point(0, 0)
        Me.RtRangeSelector1.Name = "RtRangeSelector1"
        Me.RtRangeSelector1.rtmax = 0R
        Me.RtRangeSelector1.rtmin = 0R
        Me.RtRangeSelector1.SelectedColor = System.Drawing.Color.Green
        Me.RtRangeSelector1.Size = New System.Drawing.Size(621, 241)
        Me.RtRangeSelector1.TabIndex = 0
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ResetToolStripMenuItem, Me.PinToolStripMenuItem, Me.ToolStripMenuItem2, Me.TICToolStripMenuItem, Me.BPCToolStripMenuItem, Me.ToolStripMenuItem1, Me.FilterMs2ToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(126, 126)
        '
        'ResetToolStripMenuItem
        '
        Me.ResetToolStripMenuItem.Image = CType(resources.GetObject("ResetToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ResetToolStripMenuItem.Name = "ResetToolStripMenuItem"
        Me.ResetToolStripMenuItem.Size = New System.Drawing.Size(125, 22)
        Me.ResetToolStripMenuItem.Text = "Reset"
        '
        'PinToolStripMenuItem
        '
        Me.PinToolStripMenuItem.Image = CType(resources.GetObject("PinToolStripMenuItem.Image"), System.Drawing.Image)
        Me.PinToolStripMenuItem.Name = "PinToolStripMenuItem"
        Me.PinToolStripMenuItem.Size = New System.Drawing.Size(125, 22)
        Me.PinToolStripMenuItem.Text = "Pin"
        Me.PinToolStripMenuItem.ToolTipText = "Pin of RT Range"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(122, 6)
        '
        'TICToolStripMenuItem
        '
        Me.TICToolStripMenuItem.Checked = True
        Me.TICToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.TICToolStripMenuItem.Name = "TICToolStripMenuItem"
        Me.TICToolStripMenuItem.Size = New System.Drawing.Size(125, 22)
        Me.TICToolStripMenuItem.Text = "TIC"
        '
        'BPCToolStripMenuItem
        '
        Me.BPCToolStripMenuItem.Name = "BPCToolStripMenuItem"
        Me.BPCToolStripMenuItem.Size = New System.Drawing.Size(125, 22)
        Me.BPCToolStripMenuItem.Text = "BPC"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(122, 6)
        '
        'FilterMs2ToolStripMenuItem
        '
        Me.FilterMs2ToolStripMenuItem.Image = CType(resources.GetObject("FilterMs2ToolStripMenuItem.Image"), System.Drawing.Image)
        Me.FilterMs2ToolStripMenuItem.Name = "FilterMs2ToolStripMenuItem"
        Me.FilterMs2ToolStripMenuItem.Size = New System.Drawing.Size(125, 22)
        Me.FilterMs2ToolStripMenuItem.Text = "Filter Ms2"
        '
        'MSSelector
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.RtRangeSelector1)
        Me.Name = "MSSelector"
        Me.Size = New System.Drawing.Size(621, 241)
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents RtRangeSelector1 As RtRangeSelector
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ResetToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PinToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
    Friend WithEvents TICToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BPCToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents FilterMs2ToolStripMenuItem As ToolStripMenuItem
    Private components As System.ComponentModel.IContainer
End Class
