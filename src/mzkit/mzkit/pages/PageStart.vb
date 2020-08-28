Imports WeifenLuo.WinFormsUI.Docking

Public Class PageStart

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        DirectCast(ParentForm, frmMain).fileExplorer.DockState = DockState.DockLeft
    End Sub
End Class
