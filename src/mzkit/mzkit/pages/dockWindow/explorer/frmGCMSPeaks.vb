Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports mzkit.My
Imports Task
Imports Raw = BioNovoGene.Analytical.MassSpectrometry.Math.GCMS.Raw

Public Class frmGCMSPeaks

    Dim gcmsRaw As Raw
    Dim explorer As frmGCMS_CDFExplorer

    Private Sub frmGCMSPeaks_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = "GCMS Feature Peaks"

        ApplyVsTheme(ToolStrip1)
    End Sub

    Public Sub LoadExplorer(viewer As frmGCMS_CDFExplorer)
        Dim TIC = viewer.gcms.GetTIC
        Dim TICRoot = Win7StyleTreeView1.Nodes.Add("TIC")

        gcmsRaw = viewer.gcms
        explorer = viewer
        TICRoot.Tag = TIC

        Dim ROIlist As ROI() = TIC.Shadows _
            .PopulateROI(
                baselineQuantile:=0.65,
                angleThreshold:=5,
                peakwidth:=New Double() {8, 30},
                snThreshold:=3
            ) _
            .ToArray

        For Each peak As ROI In ROIlist
            Call New TreeNode With {
                .Text = $"{CInt(peak.time.Min)} ~ {CInt(peak.time.Max)} [{peak.integration.ToString("F3")}]",
                .Tag = peak,
                .ImageIndex = 1,
                .SelectedImageIndex = 1
            }.DoCall(AddressOf TICRoot.Nodes.Add)
        Next
    End Sub

    Private Sub Win7StyleTreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles Win7StyleTreeView1.AfterSelect
        If TypeOf e.Node.Tag Is ROI Then
            Dim rtRange As DoubleRange = DirectCast(e.Node.Tag, ROI).time
            Dim proper As New ROIProperty(e.Node.Tag)

            Call explorer.RtRangeSelector1_RangeSelect(rtRange.Min, rtRange.Max)
            Call explorer.Show(MyApplication.host.dockPanel)
            Call explorer.SetRange(rtRange.Min, rtRange.Max)

            Call VisualStudio.ShowProperties(proper)
        Else
            Dim TIC As NamedCollection(Of ChromatogramTick) = e.Node.Tag

            Call MyApplication.host.mzkitTool.ShowMRMTIC(TIC.name, TIC.value)
        End If
    End Sub

    Private Sub Win7StyleTreeView1_AfterCheck(sender As Object, e As TreeViewEventArgs) Handles Win7StyleTreeView1.AfterCheck

    End Sub
End Class