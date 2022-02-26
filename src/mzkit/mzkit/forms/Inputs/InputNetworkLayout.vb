Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.SpringForce

Public Class InputNetworkLayout

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim args As ForceDirectedArgs = Globals.Settings.network.layout

        If args Is Nothing Then
            args = ForceDirectedArgs.DefaultNew
        End If

        args.Damping = damping.Value
        args.Stiffness = stiffness.Value
        args.Repulsion = repulsion.Value

        Globals.Settings.Save()

        DialogResult = DialogResult.OK
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub InputNetworkLayout_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim args As ForceDirectedArgs = Globals.Settings.network.layout

        If args Is Nothing Then
            args = ForceDirectedArgs.DefaultNew
        End If

        damping.Value = args.Damping
        stiffness.Value = args.Stiffness
        repulsion.Value = args.Repulsion
    End Sub
End Class