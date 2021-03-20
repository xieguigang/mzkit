Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports mzkit.My
Imports Task
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmUntargettedViewer

    Dim raw As Raw

    Public Sub loadRaw(raw As Raw)
        Me.raw = raw
        Me.raw.LoadMzpack()
        Me.TabText = raw.source.FileName

        Call showTIC()
    End Sub

    Private Sub RtRangeSelector1_RangeSelect(min As Double, max As Double) Handles RtRangeSelector1.RangeSelect
        Dim MS1 = raw.GetMs1Scans _
            .Where(Function(m1) m1.rt >= min AndAlso m1.rt <= max) _
            .Select(Function(t) t.GetMs) _
            .IteratesALL _
            .ToArray _
            .Centroid(Tolerance.DeltaMass(0.1), LowAbundanceTrimming.intoCutff) _
            .ToArray
        Dim msLib As New LibraryMatrix With {.centroid = True, .ms2 = MS1, .name = "MS1"}
        Dim plot As Image = msLib.MirrorPlot(titles:={"MS1", $"Rt: {CInt(min)} ~ {CInt(max)} sec"}).AsGDIImage

        PictureBox1.BackgroundImage = plot
    End Sub

    Private Sub showTIC() Handles TICToolStripMenuItem.Click
        TICToolStripMenuItem.Checked = True
        BPCToolStripMenuItem.Checked = False

        Dim TIC As ChromatogramTick() = raw _
            .GetMs1Scans _
            .Select(Function(t)
                        Return New ChromatogramTick With {.Time = t.rt, .Intensity = t.TIC}
                    End Function) _
            .ToArray

        Call RtRangeSelector1.SetTIC(TIC)
    End Sub

    Private Sub BPCToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BPCToolStripMenuItem.Click
        TICToolStripMenuItem.Checked = False
        BPCToolStripMenuItem.Checked = True

        Dim BPC As ChromatogramTick() = raw _
            .GetMs1Scans _
            .Select(Function(t)
                        Return New ChromatogramTick With {.Time = t.rt, .Intensity = t.BPC}
                    End Function) _
            .ToArray

        Call RtRangeSelector1.SetTIC(BPC)
    End Sub

    Private Sub frmUntargettedViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call ApplyVsTheme(ContextMenuStrip1)
    End Sub

    Private Sub FilterMs2ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FilterMs2ToolStripMenuItem.Click
        Call MyApplication.host.rawFeaturesList.LoadRaw(raw, RtRangeSelector1.rtmin, RtRangeSelector1.rtmax)
        Call VisualStudio.Dock(MyApplication.host.rawFeaturesList, DockState.DockLeft)
    End Sub
End Class