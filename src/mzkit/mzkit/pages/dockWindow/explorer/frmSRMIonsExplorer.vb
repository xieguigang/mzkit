Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports mzkit.My

Public Class frmSRMIonsExplorer

    Public Sub LoadMRM(file As String)
        Call CheckedListBox1.Items.Clear()

        For Each chr As chromatogram In file.LoadChromatogramList
            CheckedListBox1.Items.Add(chr)
        Next
    End Sub

    Private Sub CheckedListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CheckedListBox1.SelectedIndexChanged
        If Not CheckedListBox1.SelectedItem Is Nothing Then
            Dim chr As chromatogram = CheckedListBox1.SelectedItem
            Dim ticks As ChromatogramTick() = chr.Ticks

            Call MyApplication.host.mzkitTool.ShowMRMTIC(chr.id, ticks)
        End If
    End Sub

    Private Sub ShowTICOverlapToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowTICOverlapToolStripMenuItem.Click
        Dim list As New List(Of NamedCollection(Of ChromatogramTick))

        For Each obj As Object In CheckedListBox1.CheckedItems
            With DirectCast(obj, chromatogram)
                list += New NamedCollection(Of ChromatogramTick)(.ToString, .Ticks)
            End With
        Next

        Call MyApplication.host.mzkitTool.TIC(list.ToArray)
    End Sub

    Private Sub frmSRMIonsExplorer_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = "MRM Ions"

        Call ApplyVsTheme(ContextMenuStrip1)
    End Sub
End Class