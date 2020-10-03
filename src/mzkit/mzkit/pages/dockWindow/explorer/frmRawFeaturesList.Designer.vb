Imports mzkit.DockSample

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmRawFeaturesList
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmRawFeaturesList))
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.ImageList2 = New System.Windows.Forms.ImageList(Me.components)
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ChromatogramPlotToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowBPCToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowTICToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowXICToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.MolecularNetworkingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.IonSearchToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SearchFormulaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CustomToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem5 = New System.Windows.Forms.ToolStripSeparator()
        Me.DefaultToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SmallMoleculeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NaturalProductToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GeneralFlavoneToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExportIonsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.XICToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.IonScansToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CollapseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.SelectAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripSeparator()
        Me.DeleteFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Panel1.SuspendLayout()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TextBox2
        '
        Me.TextBox2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox2.Font = New System.Drawing.Font("Microsoft YaHei", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox2.Location = New System.Drawing.Point(7, 5)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(403, 22)
        Me.TextBox2.TabIndex = 0
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.White
        Me.Panel1.Controls.Add(Me.Button1)
        Me.Panel1.Controls.Add(Me.TextBox2)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(445, 33)
        Me.Panel1.TabIndex = 2
        '
        'Button1
        '
        Me.Button1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button1.BackColor = System.Drawing.Color.White
        Me.Button1.FlatAppearance.BorderSize = 0
        Me.Button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button1.ImageIndex = 0
        Me.Button1.ImageList = Me.ImageList2
        Me.Button1.Location = New System.Drawing.Point(415, 4)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(24, 24)
        Me.Button1.TabIndex = 1
        Me.Button1.UseVisualStyleBackColor = False
        '
        'ImageList2
        '
        Me.ImageList2.ImageStream = CType(resources.GetObject("ImageList2.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList2.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList2.Images.SetKeyName(0, "edit-find.png")
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ChromatogramPlotToolStripMenuItem, Me.ShowXICToolStripMenuItem, Me.ToolStripMenuItem1, Me.MolecularNetworkingToolStripMenuItem, Me.IonSearchToolStripMenuItem, Me.SearchFormulaToolStripMenuItem, Me.ToolStripMenuItem2, Me.FileToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(193, 148)
        '
        'ChromatogramPlotToolStripMenuItem
        '
        Me.ChromatogramPlotToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ShowBPCToolStripMenuItem, Me.ShowTICToolStripMenuItem})
        Me.ChromatogramPlotToolStripMenuItem.Image = CType(resources.GetObject("ChromatogramPlotToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ChromatogramPlotToolStripMenuItem.Name = "ChromatogramPlotToolStripMenuItem"
        Me.ChromatogramPlotToolStripMenuItem.Size = New System.Drawing.Size(192, 22)
        Me.ChromatogramPlotToolStripMenuItem.Text = "Chromatogram Plot"
        '
        'ShowBPCToolStripMenuItem
        '
        Me.ShowBPCToolStripMenuItem.Name = "ShowBPCToolStripMenuItem"
        Me.ShowBPCToolStripMenuItem.Size = New System.Drawing.Size(128, 22)
        Me.ShowBPCToolStripMenuItem.Text = "Show BPC"
        '
        'ShowTICToolStripMenuItem
        '
        Me.ShowTICToolStripMenuItem.Name = "ShowTICToolStripMenuItem"
        Me.ShowTICToolStripMenuItem.Size = New System.Drawing.Size(128, 22)
        Me.ShowTICToolStripMenuItem.Text = "Show TIC"
        '
        'ShowXICToolStripMenuItem
        '
        Me.ShowXICToolStripMenuItem.Name = "ShowXICToolStripMenuItem"
        Me.ShowXICToolStripMenuItem.Size = New System.Drawing.Size(192, 22)
        Me.ShowXICToolStripMenuItem.Text = "Show XIC"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(189, 6)
        '
        'MolecularNetworkingToolStripMenuItem
        '
        Me.MolecularNetworkingToolStripMenuItem.Image = Global.mzkit.My.Resources.Resources.preferences_system_sharing
        Me.MolecularNetworkingToolStripMenuItem.Name = "MolecularNetworkingToolStripMenuItem"
        Me.MolecularNetworkingToolStripMenuItem.Size = New System.Drawing.Size(192, 22)
        Me.MolecularNetworkingToolStripMenuItem.Text = "Molecular Networking"
        '
        'IonSearchToolStripMenuItem
        '
        Me.IonSearchToolStripMenuItem.Name = "IonSearchToolStripMenuItem"
        Me.IonSearchToolStripMenuItem.Size = New System.Drawing.Size(192, 22)
        Me.IonSearchToolStripMenuItem.Text = "Ion Search"
        '
        'SearchFormulaToolStripMenuItem
        '
        Me.SearchFormulaToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CustomToolStripMenuItem, Me.ToolStripMenuItem5, Me.DefaultToolStripMenuItem, Me.SmallMoleculeToolStripMenuItem, Me.NaturalProductToolStripMenuItem, Me.GeneralFlavoneToolStripMenuItem})
        Me.SearchFormulaToolStripMenuItem.Image = CType(resources.GetObject("SearchFormulaToolStripMenuItem.Image"), System.Drawing.Image)
        Me.SearchFormulaToolStripMenuItem.Name = "SearchFormulaToolStripMenuItem"
        Me.SearchFormulaToolStripMenuItem.Size = New System.Drawing.Size(192, 22)
        Me.SearchFormulaToolStripMenuItem.Text = "Search Formula"
        '
        'CustomToolStripMenuItem
        '
        Me.CustomToolStripMenuItem.Name = "CustomToolStripMenuItem"
        Me.CustomToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.CustomToolStripMenuItem.Text = "Custom"
        '
        'ToolStripMenuItem5
        '
        Me.ToolStripMenuItem5.Name = "ToolStripMenuItem5"
        Me.ToolStripMenuItem5.Size = New System.Drawing.Size(177, 6)
        '
        'DefaultToolStripMenuItem
        '
        Me.DefaultToolStripMenuItem.Name = "DefaultToolStripMenuItem"
        Me.DefaultToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.DefaultToolStripMenuItem.Text = "Default"
        '
        'SmallMoleculeToolStripMenuItem
        '
        Me.SmallMoleculeToolStripMenuItem.Name = "SmallMoleculeToolStripMenuItem"
        Me.SmallMoleculeToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.SmallMoleculeToolStripMenuItem.Text = "Small Molecule"
        '
        'NaturalProductToolStripMenuItem
        '
        Me.NaturalProductToolStripMenuItem.Name = "NaturalProductToolStripMenuItem"
        Me.NaturalProductToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.NaturalProductToolStripMenuItem.Text = "Natural Product"
        '
        'GeneralFlavoneToolStripMenuItem
        '
        Me.GeneralFlavoneToolStripMenuItem.Name = "GeneralFlavoneToolStripMenuItem"
        Me.GeneralFlavoneToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.GeneralFlavoneToolStripMenuItem.Text = "General Flavone"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(189, 6)
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExportIonsToolStripMenuItem, Me.CollapseToolStripMenuItem, Me.ToolStripMenuItem3, Me.SelectAllToolStripMenuItem, Me.ClearToolStripMenuItem, Me.ToolStripMenuItem4, Me.DeleteFileToolStripMenuItem})
        Me.FileToolStripMenuItem.Image = CType(resources.GetObject("FileToolStripMenuItem.Image"), System.Drawing.Image)
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(192, 22)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'ExportIonsToolStripMenuItem
        '
        Me.ExportIonsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.XICToolStripMenuItem, Me.IonScansToolStripMenuItem})
        Me.ExportIonsToolStripMenuItem.Image = CType(resources.GetObject("ExportIonsToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ExportIonsToolStripMenuItem.Name = "ExportIonsToolStripMenuItem"
        Me.ExportIonsToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.ExportIonsToolStripMenuItem.Text = "Export Ions"
        '
        'XICToolStripMenuItem
        '
        Me.XICToolStripMenuItem.Name = "XICToolStripMenuItem"
        Me.XICToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.XICToolStripMenuItem.Text = "XIC"
        '
        'IonScansToolStripMenuItem
        '
        Me.IonScansToolStripMenuItem.Name = "IonScansToolStripMenuItem"
        Me.IonScansToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.IonScansToolStripMenuItem.Text = "Ion Scans"
        '
        'CollapseToolStripMenuItem
        '
        Me.CollapseToolStripMenuItem.Name = "CollapseToolStripMenuItem"
        Me.CollapseToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.CollapseToolStripMenuItem.Text = "Collapse"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(177, 6)
        '
        'SelectAllToolStripMenuItem
        '
        Me.SelectAllToolStripMenuItem.Image = Global.mzkit.My.Resources.Resources.preferences_system_notifications
        Me.SelectAllToolStripMenuItem.Name = "SelectAllToolStripMenuItem"
        Me.SelectAllToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.SelectAllToolStripMenuItem.Text = "Select All"
        '
        'ClearToolStripMenuItem
        '
        Me.ClearToolStripMenuItem.Image = CType(resources.GetObject("ClearToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ClearToolStripMenuItem.Name = "ClearToolStripMenuItem"
        Me.ClearToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.ClearToolStripMenuItem.Text = "Clear"
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
        Me.DeleteFileToolStripMenuItem.Text = "Delete File!"
        '
        'frmRawFeaturesList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(445, 450)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "frmRawFeaturesList"
        Me.Text = "Form1"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Button1 As Button
    Friend WithEvents ImageList2 As ImageList
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ShowXICToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MolecularNetworkingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents SearchFormulaToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExportIonsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CollapseToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As ToolStripSeparator
    Friend WithEvents SelectAllToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ClearToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem4 As ToolStripSeparator
    Friend WithEvents DeleteFileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents IonSearchToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ChromatogramPlotToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ShowTICToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ShowBPCToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CustomToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem5 As ToolStripSeparator
    Friend WithEvents DefaultToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SmallMoleculeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents NaturalProductToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GeneralFlavoneToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents XICToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents IonScansToolStripMenuItem As ToolStripMenuItem
End Class
