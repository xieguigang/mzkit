#Region "Microsoft.VisualBasic::9028ad3de18e89ab066f734d9e7dfe0e, mzkit\src\mzkit\mzkit\forms\frmMain.Designer.vb"

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

    '   Total Lines: 188
    '    Code Lines: 131
    ' Comment Lines: 51
    '   Blank Lines: 6
    '     File Size: 9.99 KB


    ' Class frmMain
    ' 
    '     Sub: Dispose, InitializeComponent
    ' 
    ' /********************************************************************************/

#End Region

Imports RibbonLib

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    ' Inherits System.Windows.Forms.Form
    Inherits Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.StatusStrip = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripDropDownButton1 = New System.Windows.Forms.ToolStripDropDownButton()
        Me.MoleculeNetworkingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FormulaSearchToolToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MzCalculatorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RawFileViewerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripStatusLabel2 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel3 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripProgressBar1 = New System.Windows.Forms.ToolStripProgressBar()
        Me.ToolStripStatusLabel4 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.Ribbon1 = New Global.RibbonLib.Ribbon()
        Me.PanelBase = New System.Windows.Forms.Panel()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.StatusStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'StatusStrip
        '
        resources.ApplyResources(Me.StatusStrip, "StatusStrip")
        Me.StatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1, Me.ToolStripDropDownButton1, Me.ToolStripStatusLabel2, Me.ToolStripStatusLabel3, Me.ToolStripProgressBar1, Me.ToolStripStatusLabel4})
        Me.StatusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
        Me.ToolTip.SetToolTip(Me.StatusStrip, resources.GetString("StatusStrip.ToolTip"))
        '
        'ToolStripStatusLabel1
        '
        resources.ApplyResources(Me.ToolStripStatusLabel1, "ToolStripStatusLabel1")
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        '
        'ToolStripDropDownButton1
        '
        resources.ApplyResources(Me.ToolStripDropDownButton1, "ToolStripDropDownButton1")
        Me.ToolStripDropDownButton1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MoleculeNetworkingToolStripMenuItem, Me.FormulaSearchToolToolStripMenuItem, Me.MzCalculatorToolStripMenuItem, Me.RawFileViewerToolStripMenuItem})
        Me.ToolStripDropDownButton1.Name = "ToolStripDropDownButton1"
        '
        'MoleculeNetworkingToolStripMenuItem
        '
        resources.ApplyResources(Me.MoleculeNetworkingToolStripMenuItem, "MoleculeNetworkingToolStripMenuItem")
        Me.MoleculeNetworkingToolStripMenuItem.Name = "MoleculeNetworkingToolStripMenuItem"
        '
        'FormulaSearchToolToolStripMenuItem
        '
        resources.ApplyResources(Me.FormulaSearchToolToolStripMenuItem, "FormulaSearchToolToolStripMenuItem")
        Me.FormulaSearchToolToolStripMenuItem.Name = "FormulaSearchToolToolStripMenuItem"
        '
        'MzCalculatorToolStripMenuItem
        '
        resources.ApplyResources(Me.MzCalculatorToolStripMenuItem, "MzCalculatorToolStripMenuItem")
        Me.MzCalculatorToolStripMenuItem.Name = "MzCalculatorToolStripMenuItem"
        '
        'RawFileViewerToolStripMenuItem
        '
        resources.ApplyResources(Me.RawFileViewerToolStripMenuItem, "RawFileViewerToolStripMenuItem")
        Me.RawFileViewerToolStripMenuItem.Name = "RawFileViewerToolStripMenuItem"
        '
        'ToolStripStatusLabel2
        '
        resources.ApplyResources(Me.ToolStripStatusLabel2, "ToolStripStatusLabel2")
        Me.ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        '
        'ToolStripStatusLabel3
        '
        resources.ApplyResources(Me.ToolStripStatusLabel3, "ToolStripStatusLabel3")
        Me.ToolStripStatusLabel3.Name = "ToolStripStatusLabel3"
        '
        'ToolStripProgressBar1
        '
        resources.ApplyResources(Me.ToolStripProgressBar1, "ToolStripProgressBar1")
        Me.ToolStripProgressBar1.Name = "ToolStripProgressBar1"
        Me.ToolStripProgressBar1.Value = 100
        '
        'ToolStripStatusLabel4
        '
        resources.ApplyResources(Me.ToolStripStatusLabel4, "ToolStripStatusLabel4")
        Me.ToolStripStatusLabel4.IsLink = True
        Me.ToolStripStatusLabel4.Name = "ToolStripStatusLabel4"
        '
        'Ribbon1
        '
        resources.ApplyResources(Me.Ribbon1, "Ribbon1")
        Me.Ribbon1.Name = "Ribbon1"
        Me.Ribbon1.ResourceIdentifier = Nothing
        Me.Ribbon1.ResourceName = "BioNovoGene.mzkit_win32.RibbonMarkup.ribbon"
        Me.Ribbon1.ShortcutTableResourceName = Nothing
        Me.ToolTip.SetToolTip(Me.Ribbon1, resources.GetString("Ribbon1.ToolTip"))
        '
        'PanelBase
        '
        resources.ApplyResources(Me.PanelBase, "PanelBase")
        Me.PanelBase.Name = "PanelBase"
        Me.ToolTip.SetToolTip(Me.PanelBase, resources.GetString("PanelBase.ToolTip"))
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 1000
        '
        'frmMain
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.Controls.Add(Me.PanelBase)
        Me.Controls.Add(Me.StatusStrip)
        Me.Controls.Add(Me.Ribbon1)
        Me.Name = "frmMain"
        Me.ToolTip.SetToolTip(Me, resources.GetString("$this.ToolTip"))
        Me.StatusStrip.ResumeLayout(False)
        Me.StatusStrip.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ToolTip As System.Windows.Forms.ToolTip
    Friend WithEvents StatusStrip As System.Windows.Forms.StatusStrip
    Friend WithEvents Ribbon1 As Ribbon
    Friend WithEvents ToolStripDropDownButton1 As ToolStripDropDownButton
    Friend WithEvents FormulaSearchToolToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MzCalculatorToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RawFileViewerToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
    Friend WithEvents MoleculeNetworkingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripStatusLabel2 As ToolStripStatusLabel
    Friend WithEvents PanelBase As Panel
    Friend WithEvents ToolStripStatusLabel3 As ToolStripStatusLabel
    Friend WithEvents Timer1 As Timer
    Friend WithEvents ToolStripProgressBar1 As ToolStripProgressBar
    Friend WithEvents ToolStripStatusLabel4 As ToolStripStatusLabel
End Class
