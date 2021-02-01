Imports System.ComponentModel
Imports mzkit.My
Imports RibbonLib.Interop
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmTargetedFeatureExplorer

    Private Sub frmTargetedFeatureExplorer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call ApplyVsTheme(ToolStrip1)

        TabText = "Targeted Quantification Features"
        MyApplication.ribbon.TargetedContex.ContextAvailable = ContextAvailability.NotAvailable
    End Sub

    Private Sub frmTargetedFeatureExplorer_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        MyApplication.ribbon.TargetedContex.ContextAvailable = ContextAvailability.Active
    End Sub

    Private Sub frmTargetedFeatureExplorer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.DockState = DockState.Hidden
    End Sub

    Private Sub frmTargetedFeatureExplorer_Click(sender As Object, e As EventArgs) Handles Me.Click
        MyApplication.ribbon.TargetedContex.ContextAvailable = ContextAvailability.Active
    End Sub

    Private Sub frmTargetedFeatureExplorer_GotFocus(sender As Object, e As EventArgs) Handles Me.GotFocus
        MyApplication.ribbon.TargetedContex.ContextAvailable = ContextAvailability.Active
    End Sub
End Class