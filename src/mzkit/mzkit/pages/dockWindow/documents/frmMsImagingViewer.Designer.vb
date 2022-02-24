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
        Me.SamplesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AddSampleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ImageProcessingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.SaveImageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopyImageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExportPlotToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExportMatrixToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PixelSelector1 = New ControlLibrary.PixelSelector()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PinToolStripMenuItem, Me.ClearToolStripMenuItem, Me.ToolStripMenuItem1, Me.SamplesToolStripMenuItem, Me.ToolStripMenuItem2, Me.ImageProcessingToolStripMenuItem, Me.ToolStripMenuItem3, Me.SaveImageToolStripMenuItem, Me.CopyImageToolStripMenuItem, Me.ToolStripMenuItem4, Me.ExportPlotToolStripMenuItem, Me.ExportMatrixToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(182, 226)
        '
        'PinToolStripMenuItem
        '
        Me.PinToolStripMenuItem.Image = CType(resources.GetObject("PinToolStripMenuItem.Image"), System.Drawing.Image)
        Me.PinToolStripMenuItem.Name = "PinToolStripMenuItem"
        Me.PinToolStripMenuItem.Size = New System.Drawing.Size(181, 22)
        Me.PinToolStripMenuItem.Text = "Pin Pixel"
        '
        'ClearToolStripMenuItem
        '
        Me.ClearToolStripMenuItem.Image = CType(resources.GetObject("ClearToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ClearToolStripMenuItem.Name = "ClearToolStripMenuItem"
        Me.ClearToolStripMenuItem.Size = New System.Drawing.Size(181, 22)
        Me.ClearToolStripMenuItem.Text = "Clear"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(178, 6)
        '
        'SamplesToolStripMenuItem
        '
        Me.SamplesToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AddSampleToolStripMenuItem, Me.ClearToolStripMenuItem1})
        Me.SamplesToolStripMenuItem.Name = "SamplesToolStripMenuItem"
        Me.SamplesToolStripMenuItem.Size = New System.Drawing.Size(181, 22)
        Me.SamplesToolStripMenuItem.Text = "Samples"
        '
        'AddSampleToolStripMenuItem
        '
        Me.AddSampleToolStripMenuItem.Image = CType(resources.GetObject("AddSampleToolStripMenuItem.Image"), System.Drawing.Image)
        Me.AddSampleToolStripMenuItem.Name = "AddSampleToolStripMenuItem"
        Me.AddSampleToolStripMenuItem.Size = New System.Drawing.Size(138, 22)
        Me.AddSampleToolStripMenuItem.Text = "Add Sample"
        '
        'ClearToolStripMenuItem1
        '
        Me.ClearToolStripMenuItem1.Image = CType(resources.GetObject("ClearToolStripMenuItem1.Image"), System.Drawing.Image)
        Me.ClearToolStripMenuItem1.Name = "ClearToolStripMenuItem1"
        Me.ClearToolStripMenuItem1.Size = New System.Drawing.Size(138, 22)
        Me.ClearToolStripMenuItem1.Text = "Clear"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(178, 6)
        '
        'ImageProcessingToolStripMenuItem
        '
        Me.ImageProcessingToolStripMenuItem.Image = CType(resources.GetObject("ImageProcessingToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ImageProcessingToolStripMenuItem.Name = "ImageProcessingToolStripMenuItem"
        Me.ImageProcessingToolStripMenuItem.Size = New System.Drawing.Size(181, 22)
        Me.ImageProcessingToolStripMenuItem.Text = "Image Processing"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(178, 6)
        '
        'SaveImageToolStripMenuItem
        '
        Me.SaveImageToolStripMenuItem.Image = CType(resources.GetObject("SaveImageToolStripMenuItem.Image"), System.Drawing.Image)
        Me.SaveImageToolStripMenuItem.Name = "SaveImageToolStripMenuItem"
        Me.SaveImageToolStripMenuItem.Size = New System.Drawing.Size(181, 22)
        Me.SaveImageToolStripMenuItem.Text = "Save Image"
        '
        'CopyImageToolStripMenuItem
        '
        Me.CopyImageToolStripMenuItem.Image = CType(resources.GetObject("CopyImageToolStripMenuItem.Image"), System.Drawing.Image)
        Me.CopyImageToolStripMenuItem.Name = "CopyImageToolStripMenuItem"
        Me.CopyImageToolStripMenuItem.Size = New System.Drawing.Size(181, 22)
        Me.CopyImageToolStripMenuItem.Text = "Copy"
        '
        'ToolStripMenuItem4
        '
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        Me.ToolStripMenuItem4.Size = New System.Drawing.Size(178, 6)
        '
        'ExportPlotToolStripMenuItem
        '
        Me.ExportPlotToolStripMenuItem.Image = CType(resources.GetObject("ExportPlotToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ExportPlotToolStripMenuItem.Name = "ExportPlotToolStripMenuItem"
        Me.ExportPlotToolStripMenuItem.Size = New System.Drawing.Size(181, 22)
        Me.ExportPlotToolStripMenuItem.Text = "Export Plot"
        '
        'ExportMatrixToolStripMenuItem
        '
        Me.ExportMatrixToolStripMenuItem.Name = "ExportMatrixToolStripMenuItem"
        Me.ExportMatrixToolStripMenuItem.Size = New System.Drawing.Size(181, 22)
        Me.ExportMatrixToolStripMenuItem.Text = "Export Image Matrix"
        '
        'PixelSelector1
        '
        Me.PixelSelector1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.PixelSelector1.Cursor = System.Windows.Forms.Cursors.Cross
        Me.PixelSelector1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PixelSelector1.Location = New System.Drawing.Point(0, 0)
        Me.PixelSelector1.Name = "PixelSelector1"
        Me.PixelSelector1.SelectPolygonMode = False
        Me.PixelSelector1.ShowPointInform = True
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
    Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
    Friend WithEvents ExportPlotToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ClearToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SamplesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AddSampleToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ClearToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents ImageProcessingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As ToolStripSeparator
    Friend WithEvents CopyImageToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem4 As ToolStripSeparator
End Class
