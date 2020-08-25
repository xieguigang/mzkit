Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.visualize.Network

Public Class PageMoleculeNetworking

    Public Sub loadNetwork(MN As SpectrumTreeCluster)
        DataGridView1.Rows.Clear()
        DataGridView2.Rows.Clear()
        PictureBox1.BackgroundImage = Nothing

        Dim g = TreeGraph(Of PeakMs2, PeakMs2).CreateGraph(MN.getRoot, Function(a) a.lib_guid)


    End Sub

    Private Sub PageMoleculeNetworking_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub
End Class
