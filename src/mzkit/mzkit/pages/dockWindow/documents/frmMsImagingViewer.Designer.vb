Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMsImagingViewer
    Inherits DocumentWindow

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMsImagingViewer))
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.PinToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ClearSamplesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AddSampleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.SaveImageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExportPlotToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExportMatrixToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PixelSelector1 = New ControlLibrary.PixelSelector()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PinToolStripMenuItem, Me.ToolStripMenuItem1, Me.ClearSamplesToolStripMenuItem, Me.AddSampleToolStripMenuItem, Me.ToolStripMenuItem2, Me.SaveImageToolStripMenuItem, Me.ExportPlotToolStripMenuItem, Me.ExportMatrixToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(197, 170)
        '
        'PinToolStripMenuItem
        '
        Me.PinToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ClearToolStripMenuItem})
        Me.PinToolStripMenuItem.Image = CType(resources.GetObject("PinToolStripMenuItem.Image"), System.Drawing.Image)
        Me.PinToolStripMenuItem.Name = "PinToolStripMenuItem"
        Me.PinToolStripMenuItem.Size = New System.Drawing.Size(196, 22)
        Me.PinToolStripMenuItem.Text = "Pin"
        '
        'ClearToolStripMenuItem
        '
        Me.ClearToolStripMenuItem.Name = "ClearToolStripMenuItem"
        Me.ClearToolStripMenuItem.Size = New System.Drawing.Size(106, 22)
        Me.ClearToolStripMenuItem.Text = "Clear"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(193, 6)
        '
        'ClearSamplesToolStripMenuItem
        '
        Me.ClearSamplesToolStripMenuItem.Name = "ClearSamplesToolStripMenuItem"
        Me.ClearSamplesToolStripMenuItem.Size = New System.Drawing.Size(196, 22)
        Me.ClearSamplesToolStripMenuItem.Text = "Clear Samples"
        '
        'AddSampleToolStripMenuItem
        '
        Me.AddSampleToolStripMenuItem.Name = "AddSampleToolStripMenuItem"
        Me.AddSampleToolStripMenuItem.Size = New System.Drawing.Size(196, 22)
        Me.AddSampleToolStripMenuItem.Text = "Add Sample"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(193, 6)
        '
        'SaveImageToolStripMenuItem
        '
        Me.SaveImageToolStripMenuItem.Image = CType(resources.GetObject("SaveImageToolStripMenuItem.Image"), System.Drawing.Image)
        Me.SaveImageToolStripMenuItem.Name = "SaveImageToolStripMenuItem"
        Me.SaveImageToolStripMenuItem.Size = New System.Drawing.Size(196, 22)
        Me.SaveImageToolStripMenuItem.Text = "Save Image"
        '
        'ExportPlotToolStripMenuItem
        '
        Me.ExportPlotToolStripMenuItem.Image = CType(resources.GetObject("ExportPlotToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ExportPlotToolStripMenuItem.Name = "ExportPlotToolStripMenuItem"
        Me.ExportPlotToolStripMenuItem.Size = New System.Drawing.Size(196, 22)
        Me.ExportPlotToolStripMenuItem.Text = "Export Plot"
        '
        'ExportMatrixToolStripMenuItem
        '
        Me.ExportMatrixToolStripMenuItem.Name = "ExportMatrixToolStripMenuItem"
        Me.ExportMatrixToolStripMenuItem.Size = New System.Drawing.Size(196, 22)
        Me.ExportMatrixToolStripMenuItem.Text = "Export Image Matrix"
        '
        'PixelSelector1
        '
        Me.PixelSelector1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.PixelSelector1.Cursor = System.Windows.Forms.Cursors.Cross
        Me.PixelSelector1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PixelSelector1.Location = New System.Drawing.Point(0, 0)
        Me.PixelSelector1.Name = "PixelSelector1"
        Me.PixelSelector1.Size = New System.Drawing.Size(700, 368)
        Me.PixelSelector1.TabIndex = 1
        '
        'frmMsImagingViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(700, 368)
        Me.Controls.Add(Me.PixelSelector1)
        Me.DoubleBuffered = True
        Me.Name = "frmMsImagingViewer"
        Me.Text = "MS-Imaging Viewer"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents SaveImageToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExportMatrixToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents PixelSelector1 As ControlLibrary.PixelSelector
    Friend WithEvents PinToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ClearSamplesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AddSampleToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
    Friend WithEvents ClearToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExportPlotToolStripMenuItem As ToolStripMenuItem
End Class
