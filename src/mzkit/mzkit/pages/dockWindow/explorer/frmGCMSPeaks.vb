Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports mzkit.My
Imports Task
Imports WeifenLuo.WinFormsUI.Docking
Imports Raw = BioNovoGene.Analytical.MassSpectrometry.Math.GCMS.Raw

Public Class frmGCMSPeaks

    ''' <summary>
    ''' [filename -> viewer]
    ''' </summary>
    Dim gcmsRaw As New Dictionary(Of String, frmGCMS_CDFExplorer)

    Private Sub frmGCMSPeaks_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = "GCMS Feature Peaks"

        ApplyVsTheme(ToolStrip1)
    End Sub

    Public Function ContainsRaw(filepath As String) As Boolean
        Return gcmsRaw.ContainsKey(filepath.GetFullPath)
    End Function

    Public Sub LoadRawExplorer(gcmsRaw As Raw)
        Dim TICRoot = Win7StyleTreeView1.Nodes.Add(gcmsRaw.fileName.FileName)
        Dim CDFExplorer As New frmGCMS_CDFExplorer
        Dim TIC = gcmsRaw.GetTIC

        TICRoot.Tag = gcmsRaw

        CDFExplorer.Show(DockPanel)
        CDFExplorer.DockState = DockState.Document
        CDFExplorer.loadCDF(gcmsRaw)

        Me.gcmsRaw(gcmsRaw.fileName.GetFullPath) = CDFExplorer

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
            Dim parent = e.Node.Parent
            Dim explorer = Me.gcmsRaw(DirectCast(parent.Tag, Raw).fileName.GetFullPath)

            Call explorer.RtRangeSelector1_RangeSelect(rtRange.Min, rtRange.Max)
            Call explorer.Show(MyApplication.host.dockPanel)
            Call explorer.SetRange(rtRange.Min, rtRange.Max)

            Call VisualStudio.ShowProperties(proper)
        ElseIf TypeOf e.Node.Tag Is Raw Then
            Call ShowRaw(DirectCast(e.Node.Tag, Raw).fileName)
        End If
    End Sub

    Public Sub ShowRaw(file As String)
        Dim gcmsRaw As frmGCMS_CDFExplorer = Me.gcmsRaw(file.GetFullPath)
        Dim TIC As NamedCollection(Of ChromatogramTick) = gcmsRaw.gcms.GetTIC

        Call MyApplication.host.mzkitTool.ShowMRMTIC(TIC.name, TIC.value)
        Call VisualStudio.ShowProperties(New agilentGCMSMeta(gcmsRaw.gcms.attributes))

        gcmsRaw.Show(DockPanel)
        gcmsRaw.DockState = DockState.Document
    End Sub

    Private Sub Win7StyleTreeView1_AfterCheck(sender As Object, e As TreeViewEventArgs) Handles Win7StyleTreeView1.AfterCheck

    End Sub
End Class