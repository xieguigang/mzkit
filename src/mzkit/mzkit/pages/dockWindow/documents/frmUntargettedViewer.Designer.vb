Imports ControlLibrary

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmUntargettedViewer))
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.MsSelector1 = New ControlLibrary.MSSelector()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ShowMatrixToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'PictureBox1
        '
        Me.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PictureBox1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PictureBox1.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(904, 392)
        Me.PictureBox1.TabIndex = 1
        Me.PictureBox1.TabStop = False
        '
        'MsSelector1
        '
        Me.MsSelector1.BackColor = System.Drawing.Color.White
        Me.MsSelector1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.MsSelector1.FillColor = System.Drawing.Color.SteelBlue
        Me.MsSelector1.Location = New System.Drawing.Point(0, 392)
        Me.MsSelector1.Name = "MsSelector1"
        Me.MsSelector1.rtmax = 0R
        Me.MsSelector1.rtmin = 0R
        Me.MsSelector1.SelectedColor = System.Drawing.Color.Green
        Me.MsSelector1.Size = New System.Drawing.Size(904, 143)
        Me.MsSelector1.TabIndex = 2
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ShowMatrixToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(181, 48)
        '
        'ShowMatrixToolStripMenuItem
        '
        Me.ShowMatrixToolStripMenuItem.Image = CType(resources.GetObject("ShowMatrixToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ShowMatrixToolStripMenuItem.Name = "ShowMatrixToolStripMenuItem"
        Me.ShowMatrixToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.ShowMatrixToolStripMenuItem.Text = "Show Matrix"
        '
        'frmUntargettedViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(904, 535)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.MsSelector1)
        Me.DoubleBuffered = True
        Me.Name = "frmUntargettedViewer"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents MsSelector1 As MSSelector
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ShowMatrixToolStripMenuItem As ToolStripMenuItem
End Class
