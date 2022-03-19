#Region "Microsoft.VisualBasic::1095541f242e96fe255327b67924d2f7, mzkit\src\mzkit\mzkit\pages\dockWindow\explorer\frmRawFeaturesList.Designer.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 461
    '    Code Lines: 309
    ' Comment Lines: 147
    '   Blank Lines: 5
    '     File Size: 26.49 KB


    ' Class frmRawFeaturesList
    ' 
    '     Sub: Dispose, InitializeComponent
    ' 
    ' /********************************************************************************/

#End Region

Imports ControlLibrary
Imports ControlLibrary.Kesoft.Windows.Forms.Win7StyleTreeView
Imports BioNovoGene.mzkit_win32.DockSample

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmRawFeaturesList
    ' Inherits System.Windows.Forms.Form
    Inherits ToolWindow

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmRawFeaturesList))
        Me.ImageList2 = New System.Windows.Forms.ImageList(Me.components)
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ChromatogramPlotToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowBPCToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowTICToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowXICToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.XICViewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowPropertiesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenViewerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.DIAAnnotationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MolecularNetworkingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MetaDNASearchToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopyIonsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SpectrumSearchToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
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
        Me.IonTableToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CollapseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.SelectAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExportMzPackToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DeleteFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripSpringTextBox1 = New ControlLibrary.ToolStripSpringTextBox()
        Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButton5 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButton3 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButton4 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripButton2 = New System.Windows.Forms.ToolStripButton()
        Me.treeView1 = New ControlLibrary.Kesoft.Windows.Forms.Win7StyleTreeView.Win7StyleTreeView(Me.components)
        Me.ContextMenuStrip1.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ImageList2
        '
        Me.ImageList2.ImageStream = CType(resources.GetObject("ImageList2.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList2.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList2.Images.SetKeyName(0, "folder-pictures.png")
        Me.ImageList2.Images.SetKeyName(1, "application-x-object.png")
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ChromatogramPlotToolStripMenuItem, Me.ShowXICToolStripMenuItem, Me.ShowPropertiesToolStripMenuItem, Me.OpenViewerToolStripMenuItem, Me.ToolStripMenuItem1, Me.DIAAnnotationToolStripMenuItem, Me.CopyIonsToolStripMenuItem, Me.SpectrumSearchToolStripMenuItem, Me.IonSearchToolStripMenuItem, Me.SearchFormulaToolStripMenuItem, Me.ToolStripMenuItem2, Me.FileToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(181, 258)
        '
        'ChromatogramPlotToolStripMenuItem
        '
        Me.ChromatogramPlotToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ShowBPCToolStripMenuItem, Me.ShowTICToolStripMenuItem})
        Me.ChromatogramPlotToolStripMenuItem.Image = CType(resources.GetObject("ChromatogramPlotToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ChromatogramPlotToolStripMenuItem.Name = "ChromatogramPlotToolStripMenuItem"
        Me.ChromatogramPlotToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
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
        Me.ShowXICToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.XICViewToolStripMenuItem})
        Me.ShowXICToolStripMenuItem.Name = "ShowXICToolStripMenuItem"
        Me.ShowXICToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.ShowXICToolStripMenuItem.Text = "Show XIC"
        '
        'XICViewToolStripMenuItem
        '
        Me.XICViewToolStripMenuItem.Name = "XICViewToolStripMenuItem"
        Me.XICViewToolStripMenuItem.Size = New System.Drawing.Size(120, 22)
        Me.XICViewToolStripMenuItem.Text = "XIC View"
        '
        'ShowPropertiesToolStripMenuItem
        '
        Me.ShowPropertiesToolStripMenuItem.Image = CType(resources.GetObject("ShowPropertiesToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ShowPropertiesToolStripMenuItem.Name = "ShowPropertiesToolStripMenuItem"
        Me.ShowPropertiesToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.ShowPropertiesToolStripMenuItem.Text = "Show Properties"
        '
        'OpenViewerToolStripMenuItem
        '
        Me.OpenViewerToolStripMenuItem.Image = CType(resources.GetObject("OpenViewerToolStripMenuItem.Image"), System.Drawing.Image)
        Me.OpenViewerToolStripMenuItem.Name = "OpenViewerToolStripMenuItem"
        Me.OpenViewerToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.OpenViewerToolStripMenuItem.Text = "Open Viewer"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(177, 6)
        '
        'DIAAnnotationToolStripMenuItem
        '
        Me.DIAAnnotationToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MolecularNetworkingToolStripMenuItem, Me.MetaDNASearchToolStripMenuItem})
        Me.DIAAnnotationToolStripMenuItem.Name = "DIAAnnotationToolStripMenuItem"
        Me.DIAAnnotationToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.DIAAnnotationToolStripMenuItem.Text = "DIA Annotation"
        '
        'MolecularNetworkingToolStripMenuItem
        '
        Me.MolecularNetworkingToolStripMenuItem.Image = CType(resources.GetObject("MolecularNetworkingToolStripMenuItem.Image"), System.Drawing.Image)
        Me.MolecularNetworkingToolStripMenuItem.Name = "MolecularNetworkingToolStripMenuItem"
        Me.MolecularNetworkingToolStripMenuItem.Size = New System.Drawing.Size(192, 22)
        Me.MolecularNetworkingToolStripMenuItem.Text = "Molecular Networking"
        '
        'MetaDNASearchToolStripMenuItem
        '
        Me.MetaDNASearchToolStripMenuItem.Name = "MetaDNASearchToolStripMenuItem"
        Me.MetaDNASearchToolStripMenuItem.Size = New System.Drawing.Size(192, 22)
        Me.MetaDNASearchToolStripMenuItem.Text = "MetaDNA Search"
        '
        'CopyIonsToolStripMenuItem
        '
        Me.CopyIonsToolStripMenuItem.Name = "CopyIonsToolStripMenuItem"
        Me.CopyIonsToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.CopyIonsToolStripMenuItem.Text = "Copy Ions"
        '
        'SpectrumSearchToolStripMenuItem
        '
        Me.SpectrumSearchToolStripMenuItem.Name = "SpectrumSearchToolStripMenuItem"
        Me.SpectrumSearchToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.SpectrumSearchToolStripMenuItem.Text = "Spectrum Search"
        '
        'IonSearchToolStripMenuItem
        '
        Me.IonSearchToolStripMenuItem.Image = CType(resources.GetObject("IonSearchToolStripMenuItem.Image"), System.Drawing.Image)
        Me.IonSearchToolStripMenuItem.Name = "IonSearchToolStripMenuItem"
        Me.IonSearchToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.IonSearchToolStripMenuItem.Text = "Ion Search"
        '
        'SearchFormulaToolStripMenuItem
        '
        Me.SearchFormulaToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CustomToolStripMenuItem, Me.ToolStripMenuItem5, Me.DefaultToolStripMenuItem, Me.SmallMoleculeToolStripMenuItem, Me.NaturalProductToolStripMenuItem, Me.GeneralFlavoneToolStripMenuItem})
        Me.SearchFormulaToolStripMenuItem.Image = CType(resources.GetObject("SearchFormulaToolStripMenuItem.Image"), System.Drawing.Image)
        Me.SearchFormulaToolStripMenuItem.Name = "SearchFormulaToolStripMenuItem"
        Me.SearchFormulaToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.SearchFormulaToolStripMenuItem.Text = "Search Formula"
        '
        'CustomToolStripMenuItem
        '
        Me.CustomToolStripMenuItem.Image = CType(resources.GetObject("CustomToolStripMenuItem.Image"), System.Drawing.Image)
        Me.CustomToolStripMenuItem.Name = "CustomToolStripMenuItem"
        Me.CustomToolStripMenuItem.Size = New System.Drawing.Size(158, 22)
        Me.CustomToolStripMenuItem.Text = "Custom"
        '
        'ToolStripMenuItem5
        '
        Me.ToolStripMenuItem5.Name = "ToolStripMenuItem5"
        Me.ToolStripMenuItem5.Size = New System.Drawing.Size(155, 6)
        '
        'DefaultToolStripMenuItem
        '
        Me.DefaultToolStripMenuItem.Name = "DefaultToolStripMenuItem"
        Me.DefaultToolStripMenuItem.Size = New System.Drawing.Size(158, 22)
        Me.DefaultToolStripMenuItem.Text = "Default"
        '
        'SmallMoleculeToolStripMenuItem
        '
        Me.SmallMoleculeToolStripMenuItem.Name = "SmallMoleculeToolStripMenuItem"
        Me.SmallMoleculeToolStripMenuItem.Size = New System.Drawing.Size(158, 22)
        Me.SmallMoleculeToolStripMenuItem.Text = "Small Molecule"
        '
        'NaturalProductToolStripMenuItem
        '
        Me.NaturalProductToolStripMenuItem.Name = "NaturalProductToolStripMenuItem"
        Me.NaturalProductToolStripMenuItem.Size = New System.Drawing.Size(158, 22)
        Me.NaturalProductToolStripMenuItem.Text = "Natural Product"
        '
        'GeneralFlavoneToolStripMenuItem
        '
        Me.GeneralFlavoneToolStripMenuItem.Name = "GeneralFlavoneToolStripMenuItem"
        Me.GeneralFlavoneToolStripMenuItem.Size = New System.Drawing.Size(158, 22)
        Me.GeneralFlavoneToolStripMenuItem.Text = "General Flavone"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(177, 6)
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExportIonsToolStripMenuItem, Me.CollapseToolStripMenuItem, Me.ToolStripMenuItem3, Me.SelectAllToolStripMenuItem, Me.ClearToolStripMenuItem, Me.ToolStripMenuItem4, Me.ExportMzPackToolStripMenuItem, Me.DeleteFileToolStripMenuItem})
        Me.FileToolStripMenuItem.Image = CType(resources.GetObject("FileToolStripMenuItem.Image"), System.Drawing.Image)
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'ExportIonsToolStripMenuItem
        '
        Me.ExportIonsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.XICToolStripMenuItem, Me.IonScansToolStripMenuItem, Me.IonTableToolStripMenuItem})
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
        Me.IonScansToolStripMenuItem.Text = "Ion Scans MGF"
        '
        'IonTableToolStripMenuItem
        '
        Me.IonTableToolStripMenuItem.Name = "IonTableToolStripMenuItem"
        Me.IonTableToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.IonTableToolStripMenuItem.Text = "Ion Table"
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
        Me.SelectAllToolStripMenuItem.Image = Global.BioNovoGene.mzkit_win32.My.Resources.Resources.preferences_system_notifications
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
        'ExportMzPackToolStripMenuItem
        '
        Me.ExportMzPackToolStripMenuItem.Name = "ExportMzPackToolStripMenuItem"
        Me.ExportMzPackToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.ExportMzPackToolStripMenuItem.Text = "Export mzPack"
        '
        'DeleteFileToolStripMenuItem
        '
        Me.DeleteFileToolStripMenuItem.Image = CType(resources.GetObject("DeleteFileToolStripMenuItem.Image"), System.Drawing.Image)
        Me.DeleteFileToolStripMenuItem.Name = "DeleteFileToolStripMenuItem"
        Me.DeleteFileToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.DeleteFileToolStripMenuItem.Text = "Delete File!"
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabel1, Me.ToolStripSpringTextBox1, Me.ToolStripButton1, Me.ToolStripButton5, Me.ToolStripButton3, Me.ToolStripButton4, Me.ToolStripSeparator1, Me.ToolStripButton2})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(445, 25)
        Me.ToolStrip1.TabIndex = 1
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        Me.ToolStripLabel1.Size = New System.Drawing.Size(45, 22)
        Me.ToolStripLabel1.Text = "Search:"
        '
        'ToolStripSpringTextBox1
        '
        Me.ToolStripSpringTextBox1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.ToolStripSpringTextBox1.Name = "ToolStripSpringTextBox1"
        Me.ToolStripSpringTextBox1.Size = New System.Drawing.Size(236, 25)
        '
        'ToolStripButton1
        '
        Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton1.Image = CType(resources.GetObject("ToolStripButton1.Image"), System.Drawing.Image)
        Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton1.Name = "ToolStripButton1"
        Me.ToolStripButton1.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton1.Text = "Feature Search"
        '
        'ToolStripButton5
        '
        Me.ToolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton5.Image = CType(resources.GetObject("ToolStripButton5.Image"), System.Drawing.Image)
        Me.ToolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton5.Name = "ToolStripButton5"
        Me.ToolStripButton5.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton5.Text = "Find Ions Ms2 Fragment"
        '
        'ToolStripButton3
        '
        Me.ToolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton3.Image = CType(resources.GetObject("ToolStripButton3.Image"), System.Drawing.Image)
        Me.ToolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton3.Name = "ToolStripButton3"
        Me.ToolStripButton3.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton3.Text = "Filter By M/z"
        '
        'ToolStripButton4
        '
        Me.ToolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton4.Image = CType(resources.GetObject("ToolStripButton4.Image"), System.Drawing.Image)
        Me.ToolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton4.Name = "ToolStripButton4"
        Me.ToolStripButton4.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton4.Text = "XIC"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripButton2
        '
        Me.ToolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton2.Image = CType(resources.GetObject("ToolStripButton2.Image"), System.Drawing.Image)
        Me.ToolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton2.Name = "ToolStripButton2"
        Me.ToolStripButton2.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton2.Text = "Reload"
        '
        'treeView1
        '
        Me.treeView1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.treeView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.treeView1.Font = New System.Drawing.Font("Microsoft YaHei UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.treeView1.FullRowSelect = True
        Me.treeView1.HotTracking = True
        Me.treeView1.ImageIndex = 0
        Me.treeView1.ImageList = Me.ImageList2
        Me.treeView1.Location = New System.Drawing.Point(0, 25)
        Me.treeView1.Name = "treeView1"
        Me.treeView1.SelectedImageIndex = 0
        Me.treeView1.ShowLines = False
        Me.treeView1.Size = New System.Drawing.Size(445, 425)
        Me.treeView1.TabIndex = 2
        '
        'frmRawFeaturesList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(445, 450)
        Me.Controls.Add(Me.treeView1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.DoubleBuffered = True
        Me.Name = "frmRawFeaturesList"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ImageList2 As ImageList
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ShowXICToolStripMenuItem As ToolStripMenuItem
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
    Friend WithEvents SpectrumSearchToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStrip1 As ToolStrip
    Friend WithEvents ToolStripLabel1 As ToolStripLabel
    Friend WithEvents ToolStripSpringTextBox1 As ToolStripSpringTextBox
    Friend WithEvents ToolStripButton1 As ToolStripButton
    Friend WithEvents treeView1 As Win7StyleTreeView
    Friend WithEvents ShowPropertiesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents IonTableToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenViewerToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents XICViewToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DIAAnnotationToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MolecularNetworkingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MetaDNASearchToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExportMzPackToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripButton2 As ToolStripButton
    Friend WithEvents ToolStripButton3 As ToolStripButton
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents ToolStripButton4 As ToolStripButton
    Friend WithEvents CopyIonsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripButton5 As ToolStripButton
End Class
