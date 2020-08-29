Imports System.ComponentModel
Imports mzkit.My
Imports RibbonLib.Interop

Public Class frmFileTree

    Dim host As frmMain

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        DoubleBuffered = True
    End Sub

    Public Sub frmFileTree_Load(host As frmMain)

    End Sub

    Private Sub ShowTICToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowTICToolStripMenuItem.Click

    End Sub

    Private Sub frmFileTree_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        MyApplication.host.ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active
    End Sub

    Private Sub frmFileTree_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.Hide()
    End Sub
End Class