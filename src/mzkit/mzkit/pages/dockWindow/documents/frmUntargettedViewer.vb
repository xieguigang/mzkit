Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Task

Public Class frmUntargettedViewer

    Dim raw As Raw

    Public Sub loadRaw(raw As Raw)
        Me.raw = raw
        Me.raw.LoadMzpack()
        Me.TabText = raw.source.FileName
    End Sub

    Private Sub RtRangeSelector1_RangeSelect(min As Double, max As Double) Handles RtRangeSelector1.RangeSelect

    End Sub

    Private Sub TICToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TICToolStripMenuItem.Click
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
End Class