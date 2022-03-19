#Region "Microsoft.VisualBasic::da118f049c5edafb8cffe71f8c214ecc, mzkit\src\mzkit\mzkit\pages\dockWindow\explorer\frmSRMIonsExplorer.Designer.vb"

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

    '   Total Lines: 262
    '    Code Lines: 177
    ' Comment Lines: 80
    '   Blank Lines: 5
    '     File Size: 14.40 KB


    ' Class frmSRMIonsExplorer
    ' 
    '     Sub: Dispose, InitializeComponent
    ' 
    ' /********************************************************************************/

#End Region

Imports ControlLibrary
Imports BioNovoGene.mzkit_win32.DockSample

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmSRMIonsExplorer
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSRMIonsExplorer))
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ShowTICOverlapToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TICToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BPCToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowTICOverlap3DToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.SelectAllIonsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearIonSelectionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ClearFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Win7StyleTreeView1 = New Kesoft.Windows.Forms.Win7StyleTreeView.Win7StyleTreeView(Me.components)
        Me.ContextMenuStrip3 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ImportsFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearFilesToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.ContextMenuStrip2 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ShowTICOverlapToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowTICOverlap3DToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.ClearFileSelectionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SelectAllFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripSeparator()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.ContextMenuStrip3.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.ContextMenuStrip2.SuspendLayout()
        Me.SuspendLayout()
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ShowTICOverlapToolStripMenuItem, Me.ShowTICOverlap3DToolStripMenuItem, Me.ToolStripMenuItem1, Me.SelectAllIonsToolStripMenuItem, Me.ClearIonSelectionsToolStripMenuItem, Me.ToolStripMenuItem2, Me.ClearFilesToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(192, 126)
        '
        'ShowTICOverlapToolStripMenuItem
        '
        Me.ShowTICOverlapToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TICToolStripMenuItem, Me.BPCToolStripMenuItem})
        Me.ShowTICOverlapToolStripMenuItem.Image = CType(resources.GetObject("ShowTICOverlapToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ShowTICOverlapToolStripMenuItem.Name = "ShowTICOverlapToolStripMenuItem"
        Me.ShowTICOverlapToolStripMenuItem.Size = New System.Drawing.Size(191, 22)
        Me.ShowTICOverlapToolStripMenuItem.Text = "Show Overlaps"
        '
        'TICToolStripMenuItem
        '
        Me.TICToolStripMenuItem.Name = "TICToolStripMenuItem"
        Me.TICToolStripMenuItem.Size = New System.Drawing.Size(99, 22)
        Me.TICToolStripMenuItem.Text = "TIC"
        '
        'BPCToolStripMenuItem
        '
        Me.BPCToolStripMenuItem.Name = "BPCToolStripMenuItem"
        Me.BPCToolStripMenuItem.Size = New System.Drawing.Size(99, 22)
        Me.BPCToolStripMenuItem.Text = "BPC"
        '
        'ShowTICOverlap3DToolStripMenuItem
        '
        Me.ShowTICOverlap3DToolStripMenuItem.Name = "ShowTICOverlap3DToolStripMenuItem"
        Me.ShowTICOverlap3DToolStripMenuItem.Size = New System.Drawing.Size(191, 22)
        Me.ShowTICOverlap3DToolStripMenuItem.Text = "Overlap In 3D"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(188, 6)
        '
        'SelectAllIonsToolStripMenuItem
        '
        Me.SelectAllIonsToolStripMenuItem.Name = "SelectAllIonsToolStripMenuItem"
        Me.SelectAllIonsToolStripMenuItem.Size = New System.Drawing.Size(191, 22)
        Me.SelectAllIonsToolStripMenuItem.Text = "Select All Ions"
        '
        'ClearIonSelectionsToolStripMenuItem
        '
        Me.ClearIonSelectionsToolStripMenuItem.Name = "ClearIonSelectionsToolStripMenuItem"
        Me.ClearIonSelectionsToolStripMenuItem.Size = New System.Drawing.Size(191, 22)
        Me.ClearIonSelectionsToolStripMenuItem.Text = "Clear Ion Selections"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(188, 6)
        '
        'ClearFilesToolStripMenuItem
        '
        Me.ClearFilesToolStripMenuItem.Image = CType(resources.GetObject("ClearFilesToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ClearFilesToolStripMenuItem.Name = "ClearFilesToolStripMenuItem"
        Me.ClearFilesToolStripMenuItem.Size = New System.Drawing.Size(191, 22)
        Me.ClearFilesToolStripMenuItem.Text = "Clear Files"
        '
        'Win7StyleTreeView1
        '
        Me.Win7StyleTreeView1.CheckBoxes = True
        Me.Win7StyleTreeView1.ContextMenuStrip = Me.ContextMenuStrip3
        Me.Win7StyleTreeView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Win7StyleTreeView1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Win7StyleTreeView1.FullRowSelect = True
        Me.Win7StyleTreeView1.HotTracking = True
        Me.Win7StyleTreeView1.ImageIndex = 0
        Me.Win7StyleTreeView1.ImageList = Me.ImageList1
        Me.Win7StyleTreeView1.Location = New System.Drawing.Point(0, 25)
        Me.Win7StyleTreeView1.Name = "Win7StyleTreeView1"
        Me.Win7StyleTreeView1.SelectedImageIndex = 0
        Me.Win7StyleTreeView1.ShowLines = False
        Me.Win7StyleTreeView1.Size = New System.Drawing.Size(387, 447)
        Me.Win7StyleTreeView1.TabIndex = 1
        '
        'ContextMenuStrip3
        '
        Me.ContextMenuStrip3.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ImportsFilesToolStripMenuItem, Me.ToolStripMenuItem3, Me.SelectAllFilesToolStripMenuItem, Me.ClearFileSelectionsToolStripMenuItem, Me.ToolStripMenuItem4, Me.ClearFilesToolStripMenuItem1})
        Me.ContextMenuStrip3.Name = "ContextMenuStrip3"
        Me.ContextMenuStrip3.Size = New System.Drawing.Size(192, 126)
        '
        'ImportsFilesToolStripMenuItem
        '
        Me.ImportsFilesToolStripMenuItem.Image = CType(resources.GetObject("ImportsFilesToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ImportsFilesToolStripMenuItem.Name = "ImportsFilesToolStripMenuItem"
        Me.ImportsFilesToolStripMenuItem.Size = New System.Drawing.Size(191, 22)
        Me.ImportsFilesToolStripMenuItem.Text = "Imports Files"
        '
        'ClearFilesToolStripMenuItem1
        '
        Me.ClearFilesToolStripMenuItem1.Image = CType(resources.GetObject("ClearFilesToolStripMenuItem1.Image"), System.Drawing.Image)
        Me.ClearFilesToolStripMenuItem1.Name = "ClearFilesToolStripMenuItem1"
        Me.ClearFilesToolStripMenuItem1.Size = New System.Drawing.Size(191, 22)
        Me.ClearFilesToolStripMenuItem1.Text = "Clear Files"
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "folder-pictures.png")
        Me.ImageList1.Images.SetKeyName(1, "office_chart_area_256px_540041_easyicon.net.png")
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabel1})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(387, 25)
        Me.ToolStrip1.TabIndex = 2
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        Me.ToolStripLabel1.Size = New System.Drawing.Size(72, 22)
        Me.ToolStripLabel1.Text = "MRM Ions:"
        '
        'ContextMenuStrip2
        '
        Me.ContextMenuStrip2.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ShowTICOverlapToolStripMenuItem1, Me.ShowTICOverlap3DToolStripMenuItem1})
        Me.ContextMenuStrip2.Name = "ContextMenuStrip2"
        Me.ContextMenuStrip2.Size = New System.Drawing.Size(201, 48)
        '
        'ShowTICOverlapToolStripMenuItem1
        '
        Me.ShowTICOverlapToolStripMenuItem1.Name = "ShowTICOverlapToolStripMenuItem1"
        Me.ShowTICOverlapToolStripMenuItem1.Size = New System.Drawing.Size(200, 22)
        Me.ShowTICOverlapToolStripMenuItem1.Text = "Show TIC Overlap"
        '
        'ShowTICOverlap3DToolStripMenuItem1
        '
        Me.ShowTICOverlap3DToolStripMenuItem1.Name = "ShowTICOverlap3DToolStripMenuItem1"
        Me.ShowTICOverlap3DToolStripMenuItem1.Size = New System.Drawing.Size(200, 22)
        Me.ShowTICOverlap3DToolStripMenuItem1.Text = "Show TIC Overlap 3D"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(188, 6)
        '
        'ClearFileSelectionsToolStripMenuItem
        '
        Me.ClearFileSelectionsToolStripMenuItem.Name = "ClearFileSelectionsToolStripMenuItem"
        Me.ClearFileSelectionsToolStripMenuItem.Size = New System.Drawing.Size(191, 22)
        Me.ClearFileSelectionsToolStripMenuItem.Text = "Clear File Selections"
        '
        'SelectAllFilesToolStripMenuItem
        '
        Me.SelectAllFilesToolStripMenuItem.Name = "SelectAllFilesToolStripMenuItem"
        Me.SelectAllFilesToolStripMenuItem.Size = New System.Drawing.Size(191, 22)
        Me.SelectAllFilesToolStripMenuItem.Text = "Select All Files"
        '
        'ToolStripMenuItem4
        '
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        Me.ToolStripMenuItem4.Size = New System.Drawing.Size(188, 6)
        '
        'frmSRMIonsExplorer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(387, 472)
        Me.Controls.Add(Me.Win7StyleTreeView1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.DoubleBuffered = True
        Me.Name = "frmSRMIonsExplorer"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ContextMenuStrip3.ResumeLayout(False)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ContextMenuStrip2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ShowTICOverlapToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Win7StyleTreeView1 As Kesoft.Windows.Forms.Win7StyleTreeView.Win7StyleTreeView
    Friend WithEvents ImageList1 As ImageList
    Friend WithEvents ToolStrip1 As ToolStrip
    Friend WithEvents ToolStripLabel1 As ToolStripLabel
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents ClearFilesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ShowTICOverlap3DToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ContextMenuStrip2 As ContextMenuStrip
    Friend WithEvents ShowTICOverlapToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents ShowTICOverlap3DToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents ClearIonSelectionsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
    Friend WithEvents SelectAllIonsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ContextMenuStrip3 As ContextMenuStrip
    Friend WithEvents ImportsFilesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ClearFilesToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents TICToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BPCToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As ToolStripSeparator
    Friend WithEvents SelectAllFilesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ClearFileSelectionsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem4 As ToolStripSeparator
End Class
