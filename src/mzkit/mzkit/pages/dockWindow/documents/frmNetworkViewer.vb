Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Public Class frmNetworkViewer

    Private Sub frmNetworkViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Text = "Network Canvas"
        TabText = "Network Canvas"
    End Sub

    Public Sub SetGraph(g As NetworkGraph)
        MsClusterNetworkViewer1.LoadModel(g)
    End Sub
End Class