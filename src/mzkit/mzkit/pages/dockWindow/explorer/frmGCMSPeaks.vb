Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS

Public Class frmGCMSPeaks

    Dim gcmsRaw As Raw

    Private Sub frmGCMSPeaks_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = "GCMS Feature Peaks"

        ApplyVsTheme(ToolStrip1)
    End Sub

    Public Sub LoadRaw(raw As Raw)
        Dim TIC = raw.GetTIC
        Dim TICRoot = Win7StyleTreeView1.Nodes.Add("TIC")

        gcmsRaw = raw

        TICRoot.Tag = TIC


    End Sub
End Class