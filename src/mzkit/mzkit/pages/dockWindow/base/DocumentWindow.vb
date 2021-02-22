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

    Private Sub CloseAllButThisToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CloseAllButThisToolStripMenuItem.Click
        For Each tab As IDockContent In MyApplication.host.dockPanel.Documents
            If Not TypeOf tab Is ToolWindow Then
                If Not tab Is Me Then
                    Call DirectCast(tab, Form).Close()
                End If
            End If
        Next
    End Sub

    Private Sub CloseAllDocumentsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CloseAllDocumentsToolStripMenuItem.Click
        For Each tab As IDockContent In MyApplication.host.dockPanel.Documents
            If Not TypeOf tab Is ToolWindow Then
                Call DirectCast(tab, Form).Close()
            End If
        Next
    End Sub

    Protected Overridable Sub CopyFullPath() Handles CopyFullPathToolStripMenuItem.Click

    End Sub

    Protected Overridable Sub OpenContainingFolder() Handles OpenContainingFolderToolStripMenuItem.Click

    End Sub

    Protected Overridable Sub SaveDocument() Handles SaveDocumentToolStripMenuItem.Click

    End Sub
End Class