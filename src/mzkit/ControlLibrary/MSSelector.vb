Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram

<Assembly: InternalsVisibleTo("mzkit_win32")>

Public Class MSSelector

    Public Event ShowTIC()
    Public Event ShowBPC()
    Public Event FilterMs2(rtmin As Double, rtmax As Double)
    Public Event RangeSelect(rtmin As Double, rtmax As Double)

    Private Sub MSSelector_Load(sender As Object, e As EventArgs) Handles Me.Load
        BackColor = Color.White
    End Sub

    Public Sub SetTIC(TIC As ChromatogramTick())
        Call RtRangeSelector1.SetTIC(TIC)
    End Sub

    Public Sub RefreshRtRangeSelector()
        Call RtRangeSelector1.RefreshRtRangeSelector()
    End Sub

    Private Sub showTICClick() Handles TICToolStripMenuItem.Click
        TICToolStripMenuItem.Checked = True
        BPCToolStripMenuItem.Checked = False

        RaiseEvent ShowTIC()
    End Sub

    Private Sub showBPCClick() Handles BPCToolStripMenuItem.Click
        TICToolStripMenuItem.Checked = False
        BPCToolStripMenuItem.Checked = True

        RaiseEvent ShowBPC()
    End Sub

    Private Sub FilterMs2ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FilterMs2ToolStripMenuItem.Click
        RaiseEvent FilterMs2(RtRangeSelector1.rtmin, RtRangeSelector1.rtmax)
    End Sub

    Private Sub PinToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PinToolStripMenuItem.Click
        PinToolStripMenuItem.Checked = Not PinToolStripMenuItem.Checked
    End Sub

    Private Sub ResetToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ResetToolStripMenuItem.Click
        PinToolStripMenuItem.Checked = False

    End Sub

    Private Sub RtRangeSelector1_RangeSelect(min As Double, max As Double) Handles RtRangeSelector1.RangeSelect
        RaiseEvent RangeSelect(min, max)
    End Sub
End Class
