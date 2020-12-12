Imports mzkit.DockSample

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmUVScans
    ' Inherits System.Windows.Forms.Form
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmUVScans))
        Me.CheckedListBox1 = New System.Windows.Forms.CheckedListBox()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ShowPDAToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowUVOverlapToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'CheckedListBox1
        '
        Me.CheckedListBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.CheckedListBox1.CheckOnClick = True
        Me.CheckedListBox1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.CheckedListBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CheckedListBox1.Font = New System.Drawing.Font("Microsoft YaHei UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CheckedListBox1.FormattingEnabled = True
        Me.CheckedListBox1.Location = New System.Drawing.Point(0, 0)
        Me.CheckedListBox1.Name = "CheckedListBox1"
        Me.CheckedListBox1.Size = New System.Drawing.Size(376, 596)
        Me.CheckedListBox1.TabIndex = 0
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ShowPDAToolStripMenuItem, Me.ShowUVOverlapToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(166, 48)
        '
        'ShowPDAToolStripMenuItem
        '
        Me.ShowPDAToolStripMenuItem.Image = CType(resources.GetObject("ShowPDAToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ShowPDAToolStripMenuItem.Name = "ShowPDAToolStripMenuItem"
        Me.ShowPDAToolStripMenuItem.Size = New System.Drawing.Size(165, 22)
        Me.ShowPDAToolStripMenuItem.Text = "Show PDA"
        '
        'ShowUVOverlapToolStripMenuItem
        '
        Me.ShowUVOverlapToolStripMenuItem.Image = CType(resources.GetObject("ShowUVOverlapToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ShowUVOverlapToolStripMenuItem.Name = "ShowUVOverlapToolStripMenuItem"
        Me.ShowUVOverlapToolStripMenuItem.Size = New System.Drawing.Size(165, 22)
        Me.ShowUVOverlapToolStripMenuItem.Text = "Show UV Overlap"
        '
        'frmUVScans
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(376, 596)
        Me.Controls.Add(Me.CheckedListBox1)
        Me.Name = "frmUVScans"
        Me.Text = "Form1"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents CheckedListBox1 As CheckedListBox
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ShowPDAToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ShowUVOverlapToolStripMenuItem As ToolStripMenuItem
End Class
