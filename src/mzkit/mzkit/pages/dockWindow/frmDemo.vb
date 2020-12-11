Imports mzkit.My
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmDemo

    Dim demoPage As New DemoDataPage

    Private Sub frmDemo_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(demoPage)
        demoPage.Dock = DockStyle.Fill
        Me.Icon = My.Resources.chemistry

        Me.ShowIcon = True
        Me.TabText = "MS Demo Data"
    End Sub

    Public Sub ShowPage()
        Me.Show(MyApplication.host.dockPanel)
        DockState = DockState.Document
    End Sub
End Class