Imports System.ComponentModel
Imports mzkit.My
Imports WeifenLuo.WinFormsUI.Docking

Public Class DocumentWindow

    Friend WithEvents VS2015LightTheme1 As New VS2015LightTheme
    Friend WithEvents VisualStudioToolStripExtender1 As VisualStudioToolStripExtender

    Public Event CloseDocument()

    Protected Sub ApplyVsTheme(ParamArray items As ToolStrip())
        For Each item In items
            Call VisualStudioToolStripExtender1.SetStyle(item, VisualStudioToolStripExtender.VsVersion.Vs2015, VS2015LightTheme1)
        Next
    End Sub

    Private Sub ToolWindow_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabPageContextMenuStrip = DockContextMenuStrip1

        AutoScaleMode = AutoScaleMode.Dpi
        DoubleBuffered = True
        VisualStudioToolStripExtender1 = New VisualStudioToolStripExtender(components)

        Call ApplyVsTheme(DockContextMenuStrip1)
    End Sub

    Private Sub FloatToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FloatToolStripMenuItem.Click
        DockState = DockState.Float
    End Sub

    Private Sub CloseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CloseToolStripMenuItem.Click
        RaiseEvent CloseDocument()

        Call Me.Close()
    End Sub
End Class