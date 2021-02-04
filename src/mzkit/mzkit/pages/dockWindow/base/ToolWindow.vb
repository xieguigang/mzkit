#Region "Microsoft.VisualBasic::9bf93641ce06daa308c1b19b1ae0063a, src\mzkit\mzkit\pages\dockWindow\ToolWindow.vb"

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

'     Class ToolWindow
' 
'         Constructor: (+1 Overloads) Sub New
' 
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Windows.Forms
Imports WeifenLuo.WinFormsUI.Docking

Namespace DockSample
    Partial Public Class ToolWindow
        Inherits DockContent

        Friend WithEvents VS2015LightTheme1 As New VS2015LightTheme
        Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
        Private components As IContainer
        Friend WithEvents DockToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents AutoHideToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents FloatToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
        Friend WithEvents CloseToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents VisualStudioToolStripExtender1 As VisualStudioToolStripExtender

        Protected Sub ApplyVsTheme(ParamArray items As ToolStrip())
            For Each item In items
                Call VisualStudioToolStripExtender1.SetStyle(item, VisualStudioToolStripExtender.VsVersion.Vs2015, VS2015LightTheme1)
            Next
        End Sub

        ''' <summary>
        ''' float
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub option1ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles floatToolStripMenuItem.Click
            DockState = DockState.Float

            dockToolStripMenuItem.Enabled = True
            autoHideToolStripMenuItem.Enabled = True
            floatToolStripMenuItem.Enabled = False
        End Sub

        ''' <summary>
        ''' dock
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub option2ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles dockToolStripMenuItem.Click
            Select Case DockState
                Case DockState.DockBottomAutoHide
                    DockState = DockState.DockBottom
                Case DockState.DockLeftAutoHide
                    DockState = DockState.DockLeft
                Case DockState.DockRightAutoHide
                    DockState = DockState.DockRight
                Case DockState.DockTopAutoHide
                    DockState = DockState.DockTop
                Case Else

            End Select

            dockToolStripMenuItem.Enabled = False
            autoHideToolStripMenuItem.Enabled = True
            floatToolStripMenuItem.Enabled = True
        End Sub

        ''' <summary>
        ''' auto hide
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub option3ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles autoHideToolStripMenuItem.Click
            Select Case DockState
                Case DockState.DockBottom
                    DockState = DockState.DockBottomAutoHide
                Case DockState.DockLeft
                    DockState = DockState.DockLeftAutoHide
                Case DockState.DockRight
                    DockState = DockState.DockRightAutoHide
                Case DockState.DockTop
                    DockState = DockState.DockTopAutoHide
            End Select

            dockToolStripMenuItem.Enabled = True
            autoHideToolStripMenuItem.Enabled = False
            floatToolStripMenuItem.Enabled = True
        End Sub

        ''' <summary>
        ''' close
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub option4ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles closeToolStripMenuItem.Click
            DockState = DockState.Hidden
        End Sub

        Private Sub ToolWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
            DockState = DockState.Hidden
            e.Cancel = True
        End Sub

        Private Sub ToolWindow_Load(sender As Object, e As EventArgs) Handles Me.Load
            AutoScaleMode = AutoScaleMode.Dpi
            DoubleBuffered = True
            VisualStudioToolStripExtender1 = New VisualStudioToolStripExtender(components)
        End Sub

        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ToolWindow))
            Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.CloseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.DockToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.FloatToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.AutoHideToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
            Me.ContextMenuStrip1.SuspendLayout()
            Me.SuspendLayout()
            '
            'ContextMenuStrip1
            '
            Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DockToolStripMenuItem, Me.AutoHideToolStripMenuItem, Me.FloatToolStripMenuItem, Me.ToolStripMenuItem1, Me.CloseToolStripMenuItem})
            Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
            Me.ContextMenuStrip1.Size = New System.Drawing.Size(181, 120)
            '
            'CloseToolStripMenuItem
            '
            Me.CloseToolStripMenuItem.Image = CType(resources.GetObject("CloseToolStripMenuItem.Image"), System.Drawing.Image)
            Me.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem"
            Me.CloseToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
            Me.CloseToolStripMenuItem.Text = "Close"
            '
            'DockToolStripMenuItem
            '
            Me.DockToolStripMenuItem.Image = CType(resources.GetObject("DockToolStripMenuItem.Image"), System.Drawing.Image)
            Me.DockToolStripMenuItem.Name = "DockToolStripMenuItem"
            Me.DockToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
            Me.DockToolStripMenuItem.Text = "Dock"
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
            'ToolStripMenuItem1
            '
            Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
            Me.ToolStripMenuItem1.Size = New System.Drawing.Size(177, 6)
            '
            'ToolWindow
            '
            Me.ClientSize = New System.Drawing.Size(413, 581)
            Me.Name = "ToolWindow"
            Me.ContextMenuStrip1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
    End Class
End Namespace

