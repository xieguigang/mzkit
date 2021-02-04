Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ToolWindow
    Inherits DockContent

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ToolWindow))
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.FloatToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AutoHideToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DockToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CloseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FloatToolStripMenuItem, Me.DockToolStripMenuItem, Me.AutoHideToolStripMenuItem, Me.ToolStripMenuItem1, Me.CloseToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(181, 120)
        '
        'FloatToolStripMenuItem
        '
        Me.FloatToolStripMenuItem.Image = CType(resources.GetObject("FloatToolStripMenuItem.Image"), System.Drawing.Image)
        Me.FloatToolStripMenuItem.Name = "FloatToolStripMenuItem"
        Me.FloatToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.FloatToolStripMenuItem.Text = "Float"
        '
        'AutoHideToolStripMenuItem
        '
        Me.AutoHideToolStripMenuItem.Name = "AutoHideToolStripMenuItem"
        Me.AutoHideToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.AutoHideToolStripMenuItem.Text = "Auto Hide"
        '
        'DockToolStripMenuItem
        '
        Me.DockToolStripMenuItem.Image = CType(resources.GetObject("DockToolStripMenuItem.Image"), System.Drawing.Image)
        Me.DockToolStripMenuItem.Name = "DockToolStripMenuItem"
        Me.DockToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.DockToolStripMenuItem.Text = "Dock"
        '
        'CloseToolStripMenuItem
        '
        Me.CloseToolStripMenuItem.Image = CType(resources.GetObject("CloseToolStripMenuItem.Image"), System.Drawing.Image)
        Me.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem"
        Me.CloseToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.CloseToolStripMenuItem.Text = "Close"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(177, 6)
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(530, 675)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents FloatToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DockToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AutoHideToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents CloseToolStripMenuItem As ToolStripMenuItem
End Class
