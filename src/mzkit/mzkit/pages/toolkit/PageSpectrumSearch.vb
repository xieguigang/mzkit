Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports mzkit.My
Imports RibbonLib.Interop
Imports Task

Public Class PageSpectrumSearch

    Private Sub DataGridView1_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellEndEdit
        Call refreshPreviews()
    End Sub

    Private Function getSpectrumInput() As LibraryMatrix
        Dim ms2 As New List(Of ms2)

        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            ms2 += New ms2 With {
                .mz = Val(DataGridView1.Rows(i).Cells(0).Value),
                .intensity = Val(DataGridView1.Rows(i).Cells(1).Value),
                .quantity = .intensity
            }
        Next

        Return New LibraryMatrix With {
            .centroid = True,
            .ms2 = ms2.Where(Function(a) a.mz > 0).ToArray,
            .name = "custom spectrum"
        }
    End Function

    Private Sub refreshPreviews()
        ' do previews
        Dim previews = getSpectrumInput()

        If previews.All(Function(mz) mz.intensity = 0) OrElse previews.All(Function(mz) mz.mz = 0) Then
            MyApplication.host.showStatusMessage("all of the mass spectrum fragment their intensity or product m/z is ZERO!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            PictureBox1.BackgroundImage = previews.MirrorPlot.AsGDIImage
        End If
    End Sub

    Private Sub PageSpectrumSearch_Load(sender As Object, e As EventArgs) Handles Me.Load
        PictureBox1.BackgroundImageLayout = ImageLayout.Zoom
    End Sub

    Private Sub TabPage1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown, DataGridView1.KeyDown, PictureBox1.KeyDown, TabPage1.KeyDown
        If e.KeyCode = Keys.V AndAlso e.Control AndAlso Clipboard.ContainsText Then
            Call loadFromMgfIon()
        End If
    End Sub

    Private Sub loadFromMgfIon()
        Dim textLines As String() = Clipboard.GetText.LineTokens

        If textLines.IsNullOrEmpty Then
            Return
        End If

        Dim ion As MGF.Ions = MGF.MgfReader.StreamParser(textLines).FirstOrDefault

        If ion Is Nothing OrElse ion.Peaks.IsNullOrEmpty Then
            Call MyApplication.host.showStatusMessage("invalid mgf text format!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            DataGridView1.Rows.Clear()

            For Each ms2 As ms2 In ion.Peaks
                DataGridView1.Rows.Add(ms2.mz, ms2.intensity)
            Next

            Call refreshPreviews()
        End If
    End Sub

    Private Sub PasteMgfTextToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PasteMgfTextToolStripMenuItem.Click
        If Clipboard.ContainsText OrElse Clipboard.GetText.StringEmpty Then
            Call loadFromMgfIon()
        Else
            Call MyApplication.host.showStatusMessage("no content data in your clipboard...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        End If
    End Sub

    Private Sub SavePreviewPlotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SavePreviewPlotToolStripMenuItem.Click
        If PictureBox1.BackgroundImage Is Nothing Then
            Call MyApplication.host.showStatusMessage("no plot image for save...", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        Using file As New SaveFileDialog With {.Filter = "plot image(*.png)|*.png"}
            If file.ShowDialog = DialogResult.OK Then
                Call PictureBox1.BackgroundImage.SaveAs(file.FileName)
            End If
        End Using
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim raws = Globals.workspace.GetRawDataFiles

        TreeListView1.Items.Clear()

        For Each fileSearch In getSpectrumInput.SearchFiles(raws, Tolerance.DeltaMass(0.3), 0.8)
            Dim fileRow As New TreeListViewItem With {.Text = fileSearch.name}
            Dim i As i32 = 1

            fileRow.SubItems.Add(If(fileSearch.Count = 0, "no hits", fileSearch.Count))

            For Each result As AlignmentOutput In fileSearch
                Dim alignRow As New TreeListViewItem With {.Text = result.reference.id, .Tag = result}

                alignRow.SubItems.Add(++i)
                alignRow.SubItems.Add(result.forward)
                alignRow.SubItems.Add(result.reverse)
                alignRow.SubItems.Add(result.reference.mz)
                alignRow.SubItems.Add(result.reference.rt)

                fileRow.Items.Add(alignRow)
            Next

            TreeListView1.Items.Add(fileRow)
        Next

        TabControl1.SelectedTab = TabPage2
    End Sub

    Private Sub ViewAlignmentToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewAlignmentToolStripMenuItem.Click
        Dim cluster As TreeListViewItem
        Dim host = MyApplication.host

        If TreeListView1.SelectedItems.Count = 0 Then
            Return
        Else
            cluster = TreeListView1.SelectedItems(0)
        End If

        If cluster.ChildrenCount > 0 Then
            ' 选择的是一个文件节点
            Dim filePath As String = cluster.ToolTipText
            Dim raw As Raw = Globals.workspace.FindRawFile(filePath)

            If Not raw Is Nothing Then
                Call MyApplication.mzkitRawViewer.showScatter(raw)
            End If
        Else
            ' 选择的是一个scan数据节点
            Dim result As AlignmentOutput = cluster.Tag
            Dim alignment = result.GetAlignmentMirror

            MyApplication.host.ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active



            Call MyApplication.host.mzkitTool.showSpectrum(scan_id, raw)
            Call MyApplication.host.mzkitTool.ShowPage()
        End If

        host.mzkitTool.CustomTabControl1.SelectedTab = host.mzkitTool.TabPage5
        host.ShowPage(host.mzkitTool)
    End Sub
End Class
