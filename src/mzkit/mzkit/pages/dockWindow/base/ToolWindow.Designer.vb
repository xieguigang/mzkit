#Region "Microsoft.VisualBasic::357c96670e6bd77560009c83f5eddb74, mzkit\src\mzkit\mzkit\pages\dockWindow\base\ToolWindow.Designer.vb"

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

    '   Total Lines: 94
    '    Code Lines: 62
    ' Comment Lines: 26
    '   Blank Lines: 6
    '     File Size: 4.47 KB


    ' Class ToolWindow
    ' 
    '     Sub: Dispose, InitializeComponent
    ' 
    ' /********************************************************************************/

#End Region

Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ToolWindow
    Inherits DockContent

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ToolWindow))
        Me.DockContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.FloatToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AutoHideToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DockToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CloseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.seperatorLine = New System.Windows.Forms.ToolStripSeparator()
        Me.DockContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ContextMenuStrip1
        '
        Me.DockContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FloatToolStripMenuItem, Me.DockToolStripMenuItem, Me.AutoHideToolStripMenuItem, Me.seperatorLine, Me.CloseToolStripMenuItem})
        Me.DockContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.DockContextMenuStrip1.Size = New System.Drawing.Size(181, 120)
        '
        'FloatToolStripMenuItem
        '
        Me.FloatToolStripMenuItem.Image = CType(resources.GetObject("FloatToolStripMenuItem.Image"), System.Drawing.Image)
        Me.FloatToolStripMenuItem.Name = "FloatToolStripMenuItem"
        Me.FloatToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.FloatToolStripMenuItem.Text = "Float"
        '
        'AutoHideToolStripMenuItem
        '
        Me.AutoHideToolStripMenuItem.Name = "AutoHideToolStripMenuItem"
        Me.AutoHideToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.AutoHideToolStripMenuItem.Text = "Auto Hide"
        '
        'DockToolStripMenuItem
        '
        Me.DockToolStripMenuItem.Image = CType(resources.GetObject("DockToolStripMenuItem.Image"), System.Drawing.Image)
        Me.DockToolStripMenuItem.Name = "DockToolStripMenuItem"
        Me.DockToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.DockToolStripMenuItem.Text = "Dock"
        '
        'CloseToolStripMenuItem
        '
        Me.CloseToolStripMenuItem.Image = CType(resources.GetObject("CloseToolStripMenuItem.Image"), System.Drawing.Image)
        Me.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem"
        Me.CloseToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.CloseToolStripMenuItem.Text = "Close"
        '
        'ToolStripMenuItem1
        '
        Me.seperatorLine.Name = "ToolStripMenuItem1"
        Me.seperatorLine.Size = New System.Drawing.Size(177, 6)
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(530, 675)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.DockContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents DockContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents FloatToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DockToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AutoHideToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents seperatorLine As ToolStripSeparator
    Friend WithEvents CloseToolStripMenuItem As ToolStripMenuItem
End Class
