<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    ' Inherits System.Windows.Forms.Form
    Inherits Form

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.StatusStrip = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripDropDownButton1 = New System.Windows.Forms.ToolStripDropDownButton()
        Me.MoleculeNetworkingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FormulaSearchToolToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MzCalculatorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RawFileViewerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripStatusLabel2 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.Ribbon1 = New RibbonLib.Ribbon()
        Me.PanelBase = New System.Windows.Forms.Panel()
        Me.StatusStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'StatusStrip
        '
        Me.StatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1, Me.ToolStripDropDownButton1, Me.ToolStripStatusLabel2})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 691)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(1260, 22)
        Me.StatusStrip.TabIndex = 7
        Me.StatusStrip.Text = "StatusStrip"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(119, 17)
        Me.ToolStripStatusLabel1.Text = "ToolStripStatusLabel1"
        '
        'ToolStripDropDownButton1
        '
        Me.ToolStripDropDownButton1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MoleculeNetworkingToolStripMenuItem, Me.FormulaSearchToolToolStripMenuItem, Me.MzCalculatorToolStripMenuItem, Me.RawFileViewerToolStripMenuItem})
        Me.ToolStripDropDownButton1.Image = CType(resources.GetObject("ToolStripDropDownButton1.Image"), System.Drawing.Image)
        Me.ToolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButton1.Name = "ToolStripDropDownButton1"
        Me.ToolStripDropDownButton1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.ToolStripDropDownButton1.Size = New System.Drawing.Size(149, 20)
        Me.ToolStripDropDownButton1.Text = "Use m/z Data Toolkits"
        '
        'MoleculeNetworkingToolStripMenuItem
        '
        Me.MoleculeNetworkingToolStripMenuItem.Image = CType(resources.GetObject("MoleculeNetworkingToolStripMenuItem.Image"), System.Drawing.Image)
        Me.MoleculeNetworkingToolStripMenuItem.Name = "MoleculeNetworkingToolStripMenuItem"
        Me.MoleculeNetworkingToolStripMenuItem.Size = New System.Drawing.Size(188, 22)
        Me.MoleculeNetworkingToolStripMenuItem.Text = "Molecule Networking"
        '
        'FormulaSearchToolToolStripMenuItem
        '
        Me.FormulaSearchToolToolStripMenuItem.Image = CType(resources.GetObject("FormulaSearchToolToolStripMenuItem.Image"), System.Drawing.Image)
        Me.FormulaSearchToolToolStripMenuItem.Name = "FormulaSearchToolToolStripMenuItem"
        Me.FormulaSearchToolToolStripMenuItem.Size = New System.Drawing.Size(188, 22)
        Me.FormulaSearchToolToolStripMenuItem.Text = "Formula Search Tool"
        '
        'MzCalculatorToolStripMenuItem
        '
        Me.MzCalculatorToolStripMenuItem.Image = CType(resources.GetObject("MzCalculatorToolStripMenuItem.Image"), System.Drawing.Image)
        Me.MzCalculatorToolStripMenuItem.Name = "MzCalculatorToolStripMenuItem"
        Me.MzCalculatorToolStripMenuItem.Size = New System.Drawing.Size(188, 22)
        Me.MzCalculatorToolStripMenuItem.Text = "M/z Calculator"
        '
        'RawFileViewerToolStripMenuItem
        '
        Me.RawFileViewerToolStripMenuItem.Image = CType(resources.GetObject("RawFileViewerToolStripMenuItem.Image"), System.Drawing.Image)
        Me.RawFileViewerToolStripMenuItem.Name = "RawFileViewerToolStripMenuItem"
        Me.RawFileViewerToolStripMenuItem.Size = New System.Drawing.Size(188, 22)
        Me.RawFileViewerToolStripMenuItem.Text = "Raw File Viewer"
        '
        'ToolStripStatusLabel2
        '
        Me.ToolStripStatusLabel2.Image = CType(resources.GetObject("ToolStripStatusLabel2.Image"), System.Drawing.Image)
        Me.ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        Me.ToolStripStatusLabel2.Size = New System.Drawing.Size(135, 17)
        Me.ToolStripStatusLabel2.Text = "ToolStripStatusLabel2"
        '
        'Ribbon1
        '
        Me.Ribbon1.Location = New System.Drawing.Point(0, 0)
        Me.Ribbon1.Name = "Ribbon1"
        Me.Ribbon1.ResourceIdentifier = Nothing
        Me.Ribbon1.ResourceName = "mzkit.RibbonMarkup.ribbon"
        Me.Ribbon1.ShortcutTableResourceName = Nothing
        Me.Ribbon1.Size = New System.Drawing.Size(1260, 166)
        Me.Ribbon1.TabIndex = 9
        '
        'PanelBase
        '
        Me.PanelBase.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelBase.Location = New System.Drawing.Point(0, 166)
        Me.PanelBase.Name = "PanelBase"
        Me.PanelBase.Size = New System.Drawing.Size(1260, 525)
        Me.PanelBase.TabIndex = 10
        '
        'frmMain
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.ClientSize = New System.Drawing.Size(1260, 713)
        Me.Controls.Add(Me.PanelBase)
        Me.Controls.Add(Me.StatusStrip)
        Me.Controls.Add(Me.Ribbon1)
        Me.DoubleBuffered = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "BioNovoGene Mzkit"
        Me.StatusStrip.ResumeLayout(False)
        Me.StatusStrip.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ToolTip As System.Windows.Forms.ToolTip
    Friend WithEvents StatusStrip As System.Windows.Forms.StatusStrip
    Friend WithEvents Ribbon1 As RibbonLib.Ribbon
    Friend WithEvents ToolStripDropDownButton1 As ToolStripDropDownButton
    Friend WithEvents FormulaSearchToolToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MzCalculatorToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RawFileViewerToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
    Friend WithEvents MoleculeNetworkingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripStatusLabel2 As ToolStripStatusLabel
    Friend WithEvents PanelBase As Panel
End Class
