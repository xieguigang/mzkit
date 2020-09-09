Imports mzkit.DockSample

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmFileTree
    Inherits ToolWindow

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(disposing As Boolean)
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFileTree))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ShowXICToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowTICToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MS1ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MS2ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MolecularNetworkingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.SearchInFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SearchFormulaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CustomToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DefaultToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SmallMoleculeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NatureProductToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GeneralFlavoneToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ClearToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExportToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SelectAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearSelectionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripSeparator()
        Me.DeleteFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CollapseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Panel1.SuspendLayout()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.Button1)
        Me.Panel1.Controls.Add(Me.TextBox2)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(446, 28)
        Me.Panel1.TabIndex = 0
        '
        'Button1
        '
        Me.Button1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button1.BackColor = System.Drawing.Color.Transparent
        Me.Button1.BackgroundImage = CType(resources.GetObject("Button1.BackgroundImage"), System.Drawing.Image)
        Me.Button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.Button1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.Button1.FlatAppearance.BorderSize = 0
        Me.Button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button1.Location = New System.Drawing.Point(419, 4)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(21, 19)
        Me.Button1.TabIndex = 2
        Me.Button1.UseVisualStyleBackColor = False
        '
        'TextBox2
        '
        Me.TextBox2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBox2.Font = New System.Drawing.Font("微软雅黑", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox2.Location = New System.Drawing.Point(2, 2)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(441, 25)
        Me.TextBox2.TabIndex = 1
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ShowXICToolStripMenuItem, Me.ShowTICToolStripMenuItem, Me.MS1ToolStripMenuItem, Me.MS2ToolStripMenuItem, Me.MolecularNetworkingToolStripMenuItem, Me.ToolStripMenuItem2, Me.SearchInFileToolStripMenuItem, Me.SearchFormulaToolStripMenuItem, Me.ToolStripMenuItem1, Me.ClearToolStripMenuItem, Me.ExportToolStripMenuItem, Me.ToolStripMenuItem3, Me.FileToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(207, 264)
        '
        'ShowXICToolStripMenuItem
        '
        Me.ShowXICToolStripMenuItem.Name = "ShowXICToolStripMenuItem"
        Me.ShowXICToolStripMenuItem.Size = New System.Drawing.Size(206, 22)
        Me.ShowXICToolStripMenuItem.Text = "Show XIC"
        '
        'ShowTICToolStripMenuItem
        '
        Me.ShowTICToolStripMenuItem.Name = "ShowTICToolStripMenuItem"
        Me.ShowTICToolStripMenuItem.Size = New System.Drawing.Size(206, 22)
        Me.ShowTICToolStripMenuItem.Text = "Show TIC"
        '
        'MS1ToolStripMenuItem
        '
        Me.MS1ToolStripMenuItem.Checked = True
        Me.MS1ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.MS1ToolStripMenuItem.Name = "MS1ToolStripMenuItem"
        Me.MS1ToolStripMenuItem.Size = New System.Drawing.Size(206, 22)
        Me.MS1ToolStripMenuItem.Text = "MS1"
        '
        'MS2ToolStripMenuItem
        '
        Me.MS2ToolStripMenuItem.Checked = True
        Me.MS2ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.MS2ToolStripMenuItem.Name = "MS2ToolStripMenuItem"
        Me.MS2ToolStripMenuItem.Size = New System.Drawing.Size(206, 22)
        Me.MS2ToolStripMenuItem.Text = "MS2"
        '
        'MolecularNetworkingToolStripMenuItem
        '
        Me.MolecularNetworkingToolStripMenuItem.Image = CType(resources.GetObject("MolecularNetworkingToolStripMenuItem.Image"), System.Drawing.Image)
        Me.MolecularNetworkingToolStripMenuItem.Name = "MolecularNetworkingToolStripMenuItem"
        Me.MolecularNetworkingToolStripMenuItem.Size = New System.Drawing.Size(206, 22)
        Me.MolecularNetworkingToolStripMenuItem.Text = "Molecular Networking"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(203, 6)
        '
        'SearchInFileToolStripMenuItem
        '
        Me.SearchInFileToolStripMenuItem.Name = "SearchInFileToolStripMenuItem"
        Me.SearchInFileToolStripMenuItem.Size = New System.Drawing.Size(206, 22)
        Me.SearchInFileToolStripMenuItem.Text = "Search In File"
        '
        'SearchFormulaToolStripMenuItem
        '
        Me.SearchFormulaToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CustomToolStripMenuItem, Me.DefaultToolStripMenuItem, Me.SmallMoleculeToolStripMenuItem, Me.NatureProductToolStripMenuItem, Me.GeneralFlavoneToolStripMenuItem})
        Me.SearchFormulaToolStripMenuItem.Image = CType(resources.GetObject("SearchFormulaToolStripMenuItem.Image"), System.Drawing.Image)
        Me.SearchFormulaToolStripMenuItem.Name = "SearchFormulaToolStripMenuItem"
        Me.SearchFormulaToolStripMenuItem.Size = New System.Drawing.Size(206, 22)
        Me.SearchFormulaToolStripMenuItem.Text = "Search Formula"
        '
        'CustomToolStripMenuItem
        '
        Me.CustomToolStripMenuItem.Name = "CustomToolStripMenuItem"
        Me.CustomToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.CustomToolStripMenuItem.Text = "Custom"
        '
        'DefaultToolStripMenuItem
        '
        Me.DefaultToolStripMenuItem.Name = "DefaultToolStripMenuItem"
        Me.DefaultToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.DefaultToolStripMenuItem.Text = "Default"
        '
        'SmallMoleculeToolStripMenuItem
        '
        Me.SmallMoleculeToolStripMenuItem.Name = "SmallMoleculeToolStripMenuItem"
        Me.SmallMoleculeToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.SmallMoleculeToolStripMenuItem.Text = "Small Molecule"
        '
        'NatureProductToolStripMenuItem
        '
        Me.NatureProductToolStripMenuItem.Name = "NatureProductToolStripMenuItem"
        Me.NatureProductToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.NatureProductToolStripMenuItem.Text = "Nature Product"
        '
        'GeneralFlavoneToolStripMenuItem
        '
        Me.GeneralFlavoneToolStripMenuItem.Name = "GeneralFlavoneToolStripMenuItem"
        Me.GeneralFlavoneToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.GeneralFlavoneToolStripMenuItem.Text = "General Flavone"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(203, 6)
        '
        'ClearToolStripMenuItem
        '
        Me.ClearToolStripMenuItem.Name = "ClearToolStripMenuItem"
        Me.ClearToolStripMenuItem.Size = New System.Drawing.Size(206, 22)
        Me.ClearToolStripMenuItem.Text = "Clear"
        '
        'ExportToolStripMenuItem
        '
        Me.ExportToolStripMenuItem.Name = "ExportToolStripMenuItem"
        Me.ExportToolStripMenuItem.Size = New System.Drawing.Size(206, 22)
        Me.ExportToolStripMenuItem.Text = "Export"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(203, 6)
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SelectAllToolStripMenuItem, Me.CollapseToolStripMenuItem, Me.ClearSelectionsToolStripMenuItem, Me.ToolStripMenuItem4, Me.DeleteFileToolStripMenuItem})
        Me.FileToolStripMenuItem.Image = CType(resources.GetObject("FileToolStripMenuItem.Image"), System.Drawing.Image)
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(206, 22)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'SelectAllToolStripMenuItem
        '
        Me.SelectAllToolStripMenuItem.Image = CType(resources.GetObject("SelectAllToolStripMenuItem.Image"), System.Drawing.Image)
        Me.SelectAllToolStripMenuItem.Name = "SelectAllToolStripMenuItem"
        Me.SelectAllToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.SelectAllToolStripMenuItem.Text = "Select All"
        '
        'ClearSelectionsToolStripMenuItem
        '
        Me.ClearSelectionsToolStripMenuItem.Name = "ClearSelectionsToolStripMenuItem"
        Me.ClearSelectionsToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.ClearSelectionsToolStripMenuItem.Text = "Clear Selection"
        '
        'ToolStripMenuItem4
        '
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        Me.ToolStripMenuItem4.Size = New System.Drawing.Size(177, 6)
        '
        'DeleteFileToolStripMenuItem
        '
        Me.DeleteFileToolStripMenuItem.Image = CType(resources.GetObject("DeleteFileToolStripMenuItem.Image"), System.Drawing.Image)
        Me.DeleteFileToolStripMenuItem.Name = "DeleteFileToolStripMenuItem"
        Me.DeleteFileToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.DeleteFileToolStripMenuItem.Text = "Delete File"
        '
        'CollapseToolStripMenuItem
        '
        Me.CollapseToolStripMenuItem.Name = "CollapseToolStripMenuItem"
        Me.CollapseToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.CollapseToolStripMenuItem.Text = "Collapse"
        '
        'frmFileTree
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(446, 434)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "frmFileTree"
        Me.Text = "Form1"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents Button1 As Button
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ShowXICToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ShowTICToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MS1ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MS2ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MolecularNetworkingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
    Friend WithEvents SearchInFileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SearchFormulaToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CustomToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DefaultToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SmallMoleculeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents NatureProductToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GeneralFlavoneToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents ClearToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExportToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As ToolStripSeparator
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SelectAllToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ClearSelectionsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem4 As ToolStripSeparator
    Friend WithEvents DeleteFileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CollapseToolStripMenuItem As ToolStripMenuItem
End Class
