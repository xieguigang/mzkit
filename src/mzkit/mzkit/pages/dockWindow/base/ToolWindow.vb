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
        TabPageContextMenuStrip = ContextMenuStrip1

        AutoScaleMode = AutoScaleMode.Dpi
        DoubleBuffered = True
        VisualStudioToolStripExtender1 = New VisualStudioToolStripExtender(components)
    End Sub

End Class