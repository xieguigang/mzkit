Imports BioNovoGene.Analytical.MassSpectrometry.Math.UV
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports mzkit.My
Imports Task

Public Class frmUVScans

    Private Sub frmUVScans_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.TabText = "UV Scans"

        Call ApplyVsTheme(ContextMenuStrip1, ToolStrip1)
    End Sub

    Private Iterator Function getAllScans() As IEnumerable(Of UVScan)
        For Each span As TreeNode In Win7StyleTreeView1.Nodes
            For Each item As TreeNode In span.Nodes
                Yield DirectCast(item.Tag, UVScan)
            Next
        Next
    End Function

    Private Sub ShowPDAToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowPDAToolStripMenuItem.Click
        Dim allScans = getAllScans.ToArray
        Dim PDA_time = (From item In allScans Select DirectCast(item, UVScan)).Select(Function(a) a.scan_time).ToArray
        Dim PDA_ions = (From item In allScans Select DirectCast(item, UVScan)).Select(Function(a) a.total_ion_current).ToArray
        Dim PDA As New GeneralSignal With {
            .Measures = PDA_time,
            .measureUnit = "seconds",
            .meta = New Dictionary(Of String, String),
            .Strength = PDA_ions
        }

        Call MyApplication.host.mzkitTool.showUVscans({PDA}, $"UV scan PDA plot", "scan_time (seconds)")
        Call MyApplication.host.mzkitTool.ShowMatrix(PDAPoint.FromSignal(PDA).ToArray, $"PDAplot")
    End Sub

    Private Sub ShowUVOverlapToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowUVOverlapToolStripMenuItem.Click
        If UVchecked > 0 Then
            Dim selects = UVchecked.Select(Function(i) DirectCast(i.Tag, UVScan).GetSignalModel).ToArray
            Dim rtRange As DoubleRange = UVchecked.Select(Function(i) DirectCast(i.Tag, UVScan).scan_time).ToArray
            Dim title As String = $"UV scan at scan_time range [{rtRange.Min.ToString("F2")}, {rtRange.Max.ToString("F2")}]"

            Call MyApplication.host.mzkitTool.showUVscans(selects, title, "wavelength (nm)")
            Call MyApplication.host.mzkitTool.ShowMatrix(selects.Select(AddressOf UVScanPoint.FromSignal).IteratesALL.OrderBy(Function(a) a.wavelength).ToArray, "UVOverlaps")
        End If
    End Sub

    Dim UVchecked As New List(Of TreeNode)

    Public Iterator Function GetSelectedScans() As IEnumerable(Of UVScan)
        If UVchecked.Count > 0 Then
            For Each node In UVchecked
                Yield DirectCast(node.Tag, UVScan)
            Next
        Else
            If Not Win7StyleTreeView1.SelectedNode Is Nothing Then
                If Win7StyleTreeView1.SelectedNode.Tag Is Nothing Then
                    For Each node As TreeNode In Win7StyleTreeView1.SelectedNode.Nodes
                        Yield DirectCast(node.Tag, UVScan)
                    Next
                Else
                    Yield DirectCast(Win7StyleTreeView1.SelectedNode.Tag, UVScan)
                End If
            End If
        End If
    End Function

    Public Sub Clear() Handles ClearToolStripMenuItem.Click
        For Each item In UVchecked.ToArray
            item.Checked = False
        Next

        UVchecked.Clear()
    End Sub

    Private Sub Win7StyleTreeView1_AfterCheck(sender As Object, e As TreeViewEventArgs) Handles Win7StyleTreeView1.AfterCheck
        If e.Node.Checked Then
            If e.Node.Tag Is Nothing Then
                For Each mz As TreeNode In e.Node.Nodes
                    mz.Checked = True
                    UVchecked.Add(mz)
                Next
            Else
                UVchecked.Add(e.Node)
            End If
        Else
            If e.Node.Tag Is Nothing Then
                For Each mz As TreeNode In e.Node.Nodes
                    mz.Checked = False
                    UVchecked.Remove(mz)
                Next
            Else
                UVchecked.Remove(e.Node)
            End If
        End If
    End Sub

    Private Sub Win7StyleTreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles Win7StyleTreeView1.AfterSelect
        If e.Node.Tag Is Nothing Then
            Return
        End If

        Dim scan As UVScan = e.Node.Tag
        Dim signals = {scan.GetSignalModel}
        Dim title = $"UV scan at {scan.scan_time.ToString("F2")} sec"

        Call VisualStudio.ShowProperties(New UVScanProperty(scan))
        Call MyApplication.host.mzkitTool.showUVscans(signals, title, "wavelength (nm)")
        Call MyApplication.host.mzkitTool.ShowMatrix(UVScanPoint.FromSignal(signals(Scan0)).ToArray, title.Replace(" ", "_"))
    End Sub
End Class