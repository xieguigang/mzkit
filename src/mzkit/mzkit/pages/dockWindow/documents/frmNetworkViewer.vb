Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.SpringForce

Public Class frmNetworkViewer

    Private Sub frmNetworkViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Text = "Network Canvas"
        TabText = "Network Canvas"
    End Sub

    Public Sub SetGraph(g As NetworkGraph, layout As ForceDirectedArgs)
        If layout Is Nothing Then
            layout = ForceDirectedArgs.DefaultNew
        End If

        MsClusterNetworkViewer1.LoadModel(g)
        MsClusterNetworkViewer1.SetLayoutArguments(layout)
    End Sub
End Class