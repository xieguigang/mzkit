#Region "Microsoft.VisualBasic::981b42a546fe90eb3f3f9a44bc227c83, mzkit\src\mzkit\mzkit\pages\dockWindow\base\ToolWindow.vb"

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

    '   Total Lines: 98
    '    Code Lines: 62
    ' Comment Lines: 20
    '   Blank Lines: 16
    '     File Size: 3.41 KB


    ' Class ToolWindow
    ' 
    '     Sub: ApplyVsTheme, option1ToolStripMenuItem_Click, option2ToolStripMenuItem_Click, option3ToolStripMenuItem_Click, option4ToolStripMenuItem_Click
    '          ToolWindow_Closing, ToolWindow_Load
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports WeifenLuo.WinFormsUI.Docking

Public Class ToolWindow

    Friend WithEvents VS2015LightTheme1 As New VS2015LightTheme
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
    Private Sub option1ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FloatToolStripMenuItem.Click
        DockState = DockState.Float

        DockToolStripMenuItem.Enabled = True
        AutoHideToolStripMenuItem.Enabled = True
        FloatToolStripMenuItem.Enabled = False
    End Sub

    ''' <summary>
    ''' dock
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub option2ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DockToolStripMenuItem.Click
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

        DockToolStripMenuItem.Enabled = False
        AutoHideToolStripMenuItem.Enabled = True
        FloatToolStripMenuItem.Enabled = True
    End Sub

    ''' <summary>
    ''' auto hide
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub option3ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AutoHideToolStripMenuItem.Click
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

        DockToolStripMenuItem.Enabled = True
        AutoHideToolStripMenuItem.Enabled = False
        FloatToolStripMenuItem.Enabled = True
    End Sub

    ''' <summary>
    ''' close
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub option4ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CloseToolStripMenuItem.Click
        DockState = DockState.Hidden
    End Sub

    Private Sub ToolWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        DockState = DockState.Hidden
        e.Cancel = True
    End Sub

    Private Sub ToolWindow_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabPageContextMenuStrip = DockContextMenuStrip1

        AutoScaleMode = AutoScaleMode.Dpi
        DoubleBuffered = True
        VisualStudioToolStripExtender1 = New VisualStudioToolStripExtender(components)

        Call ApplyVsTheme(DockContextMenuStrip1)
    End Sub

End Class
