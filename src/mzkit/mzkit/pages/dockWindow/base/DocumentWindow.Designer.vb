Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DocumentWindow
    Inherits DockContent

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DocumentWindow))
        Me.DockContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.SaveDocumentToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CloseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CloseAllDocumentsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CloseAllButThisToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.CopyFullPathToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenContainingFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.FloatToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DockContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ContextMenuStrip1
        '
        Me.DockContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SaveDocumentToolStripMenuItem, Me.CloseToolStripMenuItem, Me.CloseAllDocumentsToolStripMenuItem, Me.CloseAllButThisToolStripMenuItem, Me.ToolStripMenuItem1, Me.CopyFullPathToolStripMenuItem, Me.OpenContainingFolderToolStripMenuItem, Me.ToolStripMenuItem2, Me.FloatToolStripMenuItem})
        Me.DockContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.DockContextMenuStrip1.Size = New System.Drawing.Size(202, 192)
        '
        'SaveDocumentToolStripMenuItem
        '
        Me.SaveDocumentToolStripMenuItem.Image = CType(resources.GetObject("SaveDocumentToolStripMenuItem.Image"), System.Drawing.Image)
        Me.SaveDocumentToolStripMenuItem.Name = "SaveDocumentToolStripMenuItem"
        Me.SaveDocumentToolStripMenuItem.Size = New System.Drawing.Size(201, 22)
        Me.SaveDocumentToolStripMenuItem.Text = "Save Document"
        '
        'CloseToolStripMenuItem
        '
        Me.CloseToolStripMenuItem.Image = CType(resources.GetObject("CloseToolStripMenuItem.Image"), System.Drawing.Image)
        Me.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem"
        Me.CloseToolStripMenuItem.Size = New System.Drawing.Size(201, 22)
        Me.CloseToolStripMenuItem.Text = "Close"
        '
        'CloseAllDocumentsToolStripMenuItem
        '
        Me.CloseAllDocumentsToolStripMenuItem.Name = "CloseAllDocumentsToolStripMenuItem"
        Me.CloseAllDocumentsToolStripMenuItem.Size = New System.Drawing.Size(201, 22)
        Me.CloseAllDocumentsToolStripMenuItem.Text = "Close All Documents"
        '
        'CloseAllButThisToolStripMenuItem
        '
        Me.CloseAllButThisToolStripMenuItem.Name = "CloseAllButThisToolStripMenuItem"
        Me.CloseAllButThisToolStripMenuItem.Size = New System.Drawing.Size(201, 22)
        Me.CloseAllButThisToolStripMenuItem.Text = "Close All But This"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(198, 6)
        '
        'CopyFullPathToolStripMenuItem
        '
        Me.CopyFullPathToolStripMenuItem.Name = "CopyFullPathToolStripMenuItem"
        Me.CopyFullPathToolStripMenuItem.Size = New System.Drawing.Size(201, 22)
        Me.CopyFullPathToolStripMenuItem.Text = "Copy Full Path"
        '
        'OpenContainingFolderToolStripMenuItem
        '
        Me.OpenContainingFolderToolStripMenuItem.Image = CType(resources.GetObject("OpenContainingFolderToolStripMenuItem.Image"), System.Drawing.Image)
        Me.OpenContainingFolderToolStripMenuItem.Name = "OpenContainingFolderToolStripMenuItem"
        Me.OpenContainingFolderToolStripMenuItem.Size = New System.Drawing.Size(201, 22)
        Me.OpenContainingFolderToolStripMenuItem.Text = "Open Containing Folder"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(198, 6)
        '
        'FloatToolStripMenuItem
        '
        Me.FloatToolStripMenuItem.Image = CType(resources.GetObject("FloatToolStripMenuItem.Image"), System.Drawing.Image)
        Me.FloatToolStripMenuItem.Name = "FloatToolStripMenuItem"
        Me.FloatToolStripMenuItem.Size = New System.Drawing.Size(201, 22)
        Me.FloatToolStripMenuItem.Text = "Float"
        '
        'DocumentWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(650, 502)
        Me.Name = "DocumentWindow"
        Me.Text = "Form1"
        Me.DockContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents DockContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents SaveDocumentToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CloseToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CloseAllDocumentsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CloseAllButThisToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents CopyFullPathToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenContainingFolderToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
    Friend WithEvents FloatToolStripMenuItem As ToolStripMenuItem
End Class
