#Region "Microsoft.VisualBasic::00f232f01c8d16991f136d56bbc6c137, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmUntargettedViewer.Designer.vb"

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

    '   Total Lines: 121
    '    Code Lines: 87
    ' Comment Lines: 29
    '   Blank Lines: 5
    '     File Size: 5.79 KB


    ' Class frmUntargettedViewer
    ' 
    '     Sub: Dispose, InitializeComponent
    ' 
    ' /********************************************************************************/

#End Region

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
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ShowMatrixToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.MS1ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MS2ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MsSelector1 = New ControlLibrary.MSSelector()
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
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ShowMatrixToolStripMenuItem, Me.ToolStripMenuItem1, Me.MS1ToolStripMenuItem, Me.MS2ToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(181, 98)
        '
        'ShowMatrixToolStripMenuItem
        '
        Me.ShowMatrixToolStripMenuItem.Image = CType(resources.GetObject("ShowMatrixToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ShowMatrixToolStripMenuItem.Name = "ShowMatrixToolStripMenuItem"
        Me.ShowMatrixToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.ShowMatrixToolStripMenuItem.Text = "Show Matrix"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(177, 6)
        '
        'MS1ToolStripMenuItem
        '
        Me.MS1ToolStripMenuItem.Checked = True
        Me.MS1ToolStripMenuItem.CheckOnClick = True
        Me.MS1ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.MS1ToolStripMenuItem.Name = "MS1ToolStripMenuItem"
        Me.MS1ToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.MS1ToolStripMenuItem.Text = "MS1"
        '
        'MS2ToolStripMenuItem
        '
        Me.MS2ToolStripMenuItem.Checked = True
        Me.MS2ToolStripMenuItem.CheckOnClick = True
        Me.MS2ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.MS2ToolStripMenuItem.Name = "MS2ToolStripMenuItem"
        Me.MS2ToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.MS2ToolStripMenuItem.Text = "MS2"
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
        'frmUntargettedViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
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
    Friend WithEvents MS1ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MS2ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
End Class
