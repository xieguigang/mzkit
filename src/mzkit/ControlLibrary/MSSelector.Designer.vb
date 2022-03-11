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
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.TICToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BPCToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ResetToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PinToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FilterMs2ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.XICToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'RtRangeSelector1
        '
        resources.ApplyResources(Me.RtRangeSelector1, "RtRangeSelector1")
        Me.RtRangeSelector1.AllowMoveRange = True
        Me.RtRangeSelector1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.RtRangeSelector1.FillColor = System.Drawing.Color.Blue
        Me.RtRangeSelector1.Name = "RtRangeSelector1"
        Me.RtRangeSelector1.rtmax = 0R
        Me.RtRangeSelector1.rtmin = 0R
        Me.RtRangeSelector1.SelectedColor = System.Drawing.Color.Green
        '
        'ContextMenuStrip1
        '
        resources.ApplyResources(Me.ContextMenuStrip1, "ContextMenuStrip1")
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ResetToolStripMenuItem, Me.PinToolStripMenuItem, Me.ToolStripMenuItem2, Me.TICToolStripMenuItem, Me.BPCToolStripMenuItem, Me.ToolStripMenuItem1, Me.FilterMs2ToolStripMenuItem, Me.XICToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        '
        'ToolStripMenuItem2
        '
        resources.ApplyResources(Me.ToolStripMenuItem2, "ToolStripMenuItem2")
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        '
        'TICToolStripMenuItem
        '
        resources.ApplyResources(Me.TICToolStripMenuItem, "TICToolStripMenuItem")
        Me.TICToolStripMenuItem.Checked = True
        Me.TICToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.TICToolStripMenuItem.Name = "TICToolStripMenuItem"
        '
        'BPCToolStripMenuItem
        '
        resources.ApplyResources(Me.BPCToolStripMenuItem, "BPCToolStripMenuItem")
        Me.BPCToolStripMenuItem.Name = "BPCToolStripMenuItem"
        '
        'ToolStripMenuItem1
        '
        resources.ApplyResources(Me.ToolStripMenuItem1, "ToolStripMenuItem1")
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        '
        'ResetToolStripMenuItem
        '
        resources.ApplyResources(Me.ResetToolStripMenuItem, "ResetToolStripMenuItem")
        Me.ResetToolStripMenuItem.Name = "ResetToolStripMenuItem"
        '
        'PinToolStripMenuItem
        '
        resources.ApplyResources(Me.PinToolStripMenuItem, "PinToolStripMenuItem")
        Me.PinToolStripMenuItem.Name = "PinToolStripMenuItem"
        '
        'FilterMs2ToolStripMenuItem
        '
        resources.ApplyResources(Me.FilterMs2ToolStripMenuItem, "FilterMs2ToolStripMenuItem")
        Me.FilterMs2ToolStripMenuItem.Name = "FilterMs2ToolStripMenuItem"
        '
        'XICToolStripMenuItem
        '
        resources.ApplyResources(Me.XICToolStripMenuItem, "XICToolStripMenuItem")
        Me.XICToolStripMenuItem.Name = "XICToolStripMenuItem"
        '
        'MSSelector
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.RtRangeSelector1)
        Me.Name = "MSSelector"
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
    Friend WithEvents XICToolStripMenuItem As ToolStripMenuItem
End Class
