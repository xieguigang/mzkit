Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports mzkit.My
Imports Task

Public Class frmUVScans

    Dim UVchecked As New List(Of Integer)

    Private Sub frmUVScans_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.TabText = "UV Scans"
    End Sub

    Public Sub Clear()
        For Each i As Integer In UVchecked
            Call CheckedListBox1.SetItemChecked(i, False)
        Next

        UVchecked.Clear()
    End Sub

    Private Sub ShowPDAToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowPDAToolStripMenuItem.Click
        Dim PDA_time = (From item In CheckedListBox1.Items Select DirectCast(item, UVScan)).Select(Function(a) a.scan_time).ToArray
        Dim PDA_ions = (From item In CheckedListBox1.Items Select DirectCast(item, UVScan)).Select(Function(a) a.total_ion_current).ToArray
        Dim PDA As New GeneralSignal With {
            .Measures = PDA_time,
            .measureUnit = "seconds",
            .meta = New Dictionary(Of String, String),
            .Strength = PDA_ions
        }

        Call MyApplication.host.mzkitTool.showUVscans({PDA}, $"UV scan PDA plot", "scan_time (seconds)")
    End Sub

    Private Sub ShowUVOverlapToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowUVOverlapToolStripMenuItem.Click
        If UVchecked > 0 Then
            Dim selects = UVchecked.Select(Function(i) DirectCast(CheckedListBox1.Items(i), UVScan).GetSignalModel).ToArray
            Dim rtRange As DoubleRange = UVchecked.Select(Function(i) DirectCast(CheckedListBox1.Items(i), UVScan).scan_time).ToArray
            Dim title As String = $"UV scan at scan_time range [{rtRange.Min.ToString("F2")}, {rtRange.Max.ToString("F2")}]"

            Call MyApplication.host.mzkitTool.showUVscans(selects, title, "wavelength (nm)")
        End If
    End Sub

    Private Sub CheckedListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CheckedListBox1.SelectedIndexChanged
        Dim i As Integer = CheckedListBox1.SelectedIndex

        If i = -1 Then
            Return
        End If

        Dim scan As UVScan = CheckedListBox1.Items(i)
        Dim signals = {scan.GetSignalModel}
        Dim title = $"UV scan at {scan.scan_time.ToString("F2")} sec"

        Call MyApplication.host.mzkitTool.showUVscans(signals, title, "wavelength (nm)")
    End Sub

    Private Sub CheckedListBox1_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles CheckedListBox1.ItemCheck
        If e.NewValue = CheckState.Checked Then
            UVchecked.Add(e.Index)
        Else
            UVchecked.Remove(e.Index)
        End If
    End Sub
End Class