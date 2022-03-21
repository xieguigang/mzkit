#Region "Microsoft.VisualBasic::532fb26acb6765746b463a88f8bbd9d7, mzkit\src\mzkit\mzkit\pages\toolkit\PageSpectrumSearch.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 226
    '    Code Lines: 183
    ' Comment Lines: 3
    '   Blank Lines: 40
    '     File Size: 9.14 KB


    ' Class PageSpectrumSearch
    ' 
    '     Function: getSpectrumInput
    ' 
    '     Sub: Button1_Click, DataGridView1_CellEndEdit, loadFromMgfIon, loadMs2, PageSpectrumSearch_Load
    '          PasteMgfTextToolStripMenuItem_Click, refreshPreviews, runSearch, SavePreviewPlotToolStripMenuItem_Click, SearchThread
    '          TabPage1_KeyDown, ViewAlignmentToolStripMenuItem_Click
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.IsotopicPatterns
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports BioNovoGene.mzkit_win32.My
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
                .intensity = Val(DataGridView1.Rows(i).Cells(1).Value)
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

        Dim ion As MGF.Ions

        Try
            ion = MGF.MgfReader.StreamParser(textLines).FirstOrDefault
        Catch ex As Exception
            Call MyApplication.host.showStatusMessage("invalid mgf text format!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End Try

        If ion Is Nothing OrElse ion.Peaks.IsNullOrEmpty Then
            Call MyApplication.host.showStatusMessage("invalid mgf text format!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Call loadMs2(ion.Peaks)
        End If
    End Sub

    Public Sub loadMs2(products As IEnumerable(Of ms2))
        DataGridView1.Rows.Clear()

        For Each ms2 As ms2 In products
            DataGridView1.Rows.Add(ms2.mz, ms2.intensity)
        Next

        Call refreshPreviews()
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
        Call runSearch()
    End Sub

    Public Sub runSearch(Optional isotopic As IsotopeDistribution = Nothing)
        Dim raws As IEnumerable(Of Task.Raw) = Globals.workspace.GetRawDataFiles
        Dim progress As New frmTaskProgress
        Dim query As [Variant](Of LibraryMatrix, IsotopeDistribution)

        If isotopic Is Nothing Then
            query = getSpectrumInput()
        Else
            query = isotopic
        End If

        progress.ShowProgressTitle("Run spectrum similarity search...", directAccess:=True)
        progress.ShowProgressDetails("Running...", directAccess:=True)

        Call TreeListView1.Items.Clear()
        Call New Thread(Sub() Call SearchThread(query, raws, progress)).Start()
        Call progress.ShowDialog()

        TabControl1.SelectedTab = TabPage2
    End Sub

    Private Sub SearchThread(query As [Variant](Of LibraryMatrix, IsotopeDistribution), raws As IEnumerable(Of Task.Raw), progress As frmTaskProgress)
        Dim runSearchResult As IEnumerable(Of NamedCollection(Of AlignmentOutput))

        If query Like GetType(LibraryMatrix) Then
            runSearchResult = query.TryCast(Of LibraryMatrix).SearchFiles(
                files:=raws,
                tolerance:=Tolerance.DeltaMass(0.3),
                dotcutoff:=0.8,
                progress:=AddressOf progress.ShowProgressDetails,
                reload:=Sub(src, cache)
                            frmFileExplorer.getRawCache(src,, cache)
                        End Sub
            )
        Else
            runSearchResult = query.TryCast(Of IsotopeDistribution).SearchFiles(
                files:=raws,
                tolerance:=Tolerance.DeltaMass(0.05),
                dotcutoff:=0.3,
                progress:=AddressOf progress.ShowProgressDetails,
                reload:=Sub(src, cache)
                            frmFileExplorer.getRawCache(src,, cache)
                        End Sub
            )
        End If

        For Each fileSearch As NamedCollection(Of AlignmentOutput) In runSearchResult
            Dim fileRow As New TreeListViewItem With {
                .Text = fileSearch.name,
                .ToolTipText = fileSearch.description,
                .ImageIndex = 0,
                .StateImageIndex = 0
            }
            Dim i As i32 = 1

            fileRow.SubItems.Add(If(fileSearch.Count = 0, "no hits", fileSearch.Count))

            For Each result As AlignmentOutput In fileSearch
                Dim alignRow As New TreeListViewItem With {
                    .Text = result.reference.id,
                    .Tag = result,
                    .ImageIndex = 1,
                    .StateImageIndex = 1
                }

                alignRow.SubItems.Add(++i)
                alignRow.SubItems.Add(result.forward)
                alignRow.SubItems.Add(result.reverse)
                alignRow.SubItems.Add(result.reference.mz)
                alignRow.SubItems.Add(result.reference.scan_time)

                fileRow.Items.Add(alignRow)
            Next

            Me.Invoke(Sub() Call TreeListView1.Items.Add(fileRow))
        Next

        Call progress.ShowProgressDetails("Search job done!")
        Call progress.Invoke(Sub() Call progress.Close())
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
            Dim raw As Task.Raw = Globals.workspace.FindRawFile(filePath)

            If Not raw Is Nothing Then
                Call MyApplication.mzkitRawViewer.showScatter(raw, False, directSnapshot:=True, contour:=False)
            End If
        Else
            ' 选择的是一个scan数据节点
            Call MyApplication.host.mzkitTool.showAlignment(DirectCast(cluster.Tag, AlignmentOutput))
            Call MyApplication.host.mzkitTool.ShowPage()
        End If

        host.mzkitTool.CustomTabControl1.SelectedTab = host.mzkitTool.TabPage5
        host.ShowPage(host.mzkitTool)
    End Sub
End Class
