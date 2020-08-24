Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports RibbonLib.Interop
Imports Task

Public Class PageMzkitTools

    Dim host As frmMain
    Dim status As ToolStripStatusLabel
    Dim RibbonItems As RibbonItems

    Sub showStatusMessage(message As String)
        host.Invoke(Sub() status.Text = message)
    End Sub

    Sub InitializeFileTree()
        If TreeView1.LoadRawFileCache = 0 Then
            ' MessageBox.Show($"It seems that you don't have any raw file opended. {vbCrLf}You could open raw file through [File] -> [Open Raw File].", "Tips", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            TreeView1.SelectedNode = TreeView1.Nodes.Item(Scan0)
            setCurrentFile()
        End If
    End Sub

    Private Sub missingCacheFile(raw As Raw)
        MessageBox.Show($"The specific raw data cache is missing!{vbCrLf}{raw.cache.GetFullPath}", "Cache Not Found!", MessageBoxButtons.OK, MessageBoxIcon.Stop)
    End Sub

    Public Sub SaveFileCache()
        Call TreeView1.SaveRawFileCache
    End Sub

    Public Sub ImportsRaw()
        Using file As New OpenFileDialog With {.Filter = "Raw Data|*.mzXML;*.mzML"}
            If file.ShowDialog = DialogResult.OK Then
                Dim progress As New frmImportTaskProgress() With {.Text = $"Imports raw data [{file.FileName}]"}
                Dim showProgress As Action(Of String) = Sub(text) progress.Invoke(Sub() progress.Label1.Text = text)
                Dim task As New Task.ImportsRawData(file.FileName, showProgress, Sub() Call progress.Invoke(Sub() progress.Close()))
                Dim runTask As New Thread(AddressOf task.RunImports)

                ParentForm.Invoke(Sub() status.Text = "Run Raw Data Imports")
                progress.Label2.Text = progress.Text

                Call runTask.Start()
                Call progress.ShowDialog()

                'Call New frmRawViewer() With {
                '    .MdiParent = Me,
                '    .Text = file.FileName,
                '    .rawFile = task.raw
                '}.Show()
                Call TreeView1.addRawFile(task.raw)
                Call ParentForm.Invoke(Sub() status.Text = "Ready!")
            End If
        End Using
    End Sub

    Private Sub TreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TreeView1.AfterSelect
        If TypeOf e.Node.Tag Is Task.Raw Then
            ' 原始文件节点
        ElseIf TypeOf e.Node.Tag Is ms2() Then
            ' TIC 图绘制
            Dim raw = DirectCast(e.Node.Tag, ms2())
            Dim selects = TreeView1.CurrentRawFile.raw
            Dim TIC As New NamedCollection(Of ChromatogramTick) With {
                .name = $"m/z {raw.Select(Function(m) m.mz).Min.ToString("F3")} - {raw.Select(Function(m) m.mz).Max.ToString("F3")}",
                .value = raw.Select(Function(a) New ChromatogramTick With {.Time = Val(a.Annotation), .Intensity = a.intensity}).ToArray
            }

            TIC.value = {
                New ChromatogramTick With {.Time = selects.rtmin},
                New ChromatogramTick With {.Time = selects.rtmax}
            }.JoinIterates(TIC.value).ToArray

            PictureBox1.BackgroundImage = TIC.TICplot.AsGDIImage
        ElseIf e.Node.Tag Is Nothing AndAlso e.Node.Text = "TIC" Then
            Dim raw = TreeView1.CurrentRawFile.raw
            Dim TIC As New NamedCollection(Of ChromatogramTick) With {
                .name = "TIC",
                .value = raw.scans _
                    .Where(Function(a) a.mz = 0R) _
                    .Select(Function(m)
                                Return New ChromatogramTick With {.Time = m.rt, .Intensity = m.intensity}
                            End Function) _
                    .ToArray
            }

            TIC.value = {
                New ChromatogramTick With {.Time = raw.rtmin},
                New ChromatogramTick With {.Time = raw.rtmax}
            }.JoinIterates(TIC.value).ToArray

            PictureBox1.BackgroundImage = TIC.TICplot.AsGDIImage
        Else
            ' scan节点
            Dim raw As Task.Raw = e.Node.Parent.Tag
            Dim scanId As String = e.Node.Text

            Call showSpectrum(scanId, raw)
        End If

        Call setCurrentFile()
    End Sub

    Private Sub PageMzkitTools_Load(sender As Object, e As EventArgs) Handles Me.Load
        host = DirectCast(ParentForm, frmMain)
        status = host.ToolStripStatusLabel
        RibbonItems = host.ribbonItems

        Call InitializeFileTree()
    End Sub

    Private Sub showSpectrum(scanId As String, raw As Raw)
        Dim scanData As LibraryMatrix

        If raw.cache.FileExists Then
            Using cache As New netCDFReader(raw.cache)
                Dim data As CDFData = cache.getDataVariable(cache.getDataVariableEntry(scanId))
                Dim attrs = cache.getDataVariableEntry(scanId).attributes

                scanData = New LibraryMatrix With {
                    .name = scanId,
                    .centroid = False,
                    .ms2 = data.numerics.AsMs2.ToArray.Centroid(Tolerance.DeltaMass(0.1), 0.01).ToArray
                }

                Dim draw As Image = scanData.MirrorPlot.AsGDIImage

                PropertyGrid1.SelectedObject = New SpectrumProperty(attrs)
                PropertyGrid1.Refresh()

                PictureBox1.BackgroundImage = draw
            End Using
        Else
            Call missingCacheFile(raw)
        End If
    End Sub

    Private Sub setCurrentFile()
        If TreeView1.Nodes.Count = 0 Then
            showStatusMessage("No raw file opened.")
            Return
        End If

        With TreeView1.CurrentRawFile.raw
            Static selectedFile As String

            If selectedFile <> status.Text Then
                selectedFile = $"{ .source.FileName} [{ .numOfScans} scans]"
                showStatusMessage(selectedFile)
                ListBox1.Items.Clear()
            End If
        End With

        If Not TreeView1.CurrentRawFile.raw.cache.FileExists Then
            TreeView1.SelectedNode.ImageIndex = 1
            TreeView1.SelectedNode.SelectedImageIndex = 1
        End If
    End Sub

    Private Sub ShowTICToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowTICToolStripMenuItem.Click
        Dim raw = TreeView1.CurrentRawFile

        ShowTICToolStripMenuItem.Checked = Not ShowTICToolStripMenuItem.Checked

        If raw.raw Is Nothing Then
            Return
        End If

        If ShowTICToolStripMenuItem.Checked Then
            If Not raw.raw.cache.FileExists Then
                Call missingCacheFile(raw.raw)
                Return
            End If

            raw.tree.Nodes.Clear()
            raw.tree.Nodes.Add(New TreeNode("TIC"))

            Using cache As New netCDFReader(raw.raw.cache)
                Dim progress As New frmImportTaskProgress() With {.Text = $"Reading TIC raw data [{raw.raw.source}]"}
                Dim showProgress As Action(Of String) = Sub(text) progress.Invoke(Sub() progress.Label1.Text = text)
                Dim mzgroups As NamedCollection(Of ms2)() = {}
                Dim runTask As New Thread(
                        Sub()
                            Dim ms1n = raw.raw.scans.Where(Function(a) a.mz = 0R).Count
                            Dim i As i32 = 1
                            Dim allMz As New List(Of ms2)
                            Dim mztemp As ms2()

                            For Each scan In raw.raw.scans
                                If scan.mz = 0 Then
                                    Dim entry = cache.getDataVariableEntry(scan.id)
                                    Dim rt As String = entry.attributes.Where(Function(a) a.name = "retentionTime").FirstOrDefault?.value

                                    mztemp = cache.getDataVariable(entry).numerics.AsMs2.ToArray

                                    For i2 As Integer = 0 To mztemp.Length - 1
                                        mztemp(i2).Annotation = rt
                                    Next

                                    allMz.AddRange(mztemp)
                                    showProgress($"[{++i}/{ms1n}] {scan.id}")
                                End If
                            Next

                            showProgress("Run m/z group....")
                            mzgroups = allMz _
                                .GroupBy(Function(mz) mz.mz, Tolerance.DeltaMass(5)) _
                                .Select(Function(a)
                                            Dim max = a.Select(Function(m) m.intensity).Max

                                            Return New NamedCollection(Of ms2) With {.value = a.value.Where(Function(m) m.intensity / max >= 0.05).OrderBy(Function(m) Val(m.Annotation)).ToArray}
                                        End Function) _
                                .ToArray
                            progress.Invoke(Sub() progress.Close())
                        End Sub)

                showStatusMessage("Run Raw Data Imports")
                progress.Label2.Text = progress.Text

                Call runTask.Start()
                Call progress.ShowDialog()

                For Each mzblock In mzgroups
                    Dim range As New DoubleRange(mzblock.Select(Function(m) m.mz))

                    raw.tree.Nodes.Add(New TreeNode($"m/z {range.Min.ToString("F3")} - {range.Max.ToString("F3")}") With {.Tag = mzblock.ToArray})
                Next

                showStatusMessage("Ready!")
            End Using

            host.Invoke(Sub() RibbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.NotAvailable)
        Else
            Call applyLevelFilter()

            host.Invoke(Sub() RibbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active)
        End If
    End Sub

    Private Sub SaveImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveImageToolStripMenuItem.Click
        If Not PictureBox1.BackgroundImage Is Nothing Then
            Using file As New SaveFileDialog With {.Filter = "image(*.png)|*.png"}
                If file.ShowDialog = DialogResult.OK Then
                    Call PictureBox1.BackgroundImage.SaveAs(file.FileName)
                    Call Process.Start(file.FileName)
                End If
            End Using
        End If
    End Sub

    Private Sub applyLevelFilter()
        Dim raw = TreeView1.CurrentRawFile

        If Not raw.raw Is Nothing Then
            raw.tree.Nodes.Clear()
            raw.tree.addRawFile(raw.raw, MS1ToolStripMenuItem.Checked, MS2ToolStripMenuItem.Checked)
        End If
    End Sub

    Private Sub MS1ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MS1ToolStripMenuItem.Click
        MS1ToolStripMenuItem.Checked = Not MS1ToolStripMenuItem.Checked
        applyLevelFilter()
    End Sub

    Private Sub MS2ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MS2ToolStripMenuItem.Click
        MS2ToolStripMenuItem.Checked = Not MS2ToolStripMenuItem.Checked
        applyLevelFilter()
    End Sub

    Private Sub DeleteFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteFileToolStripMenuItem.Click
        Dim current = TreeView1.CurrentRawFile

        TreeView1.Nodes.Remove(current.tree)
        TreeView1.SaveRawFileCache

        Call setCurrentFile()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Call searchInFileByMz(mz:=Val(TextBox1.Text))
    End Sub

    Private Sub searchInFileByMz(mz As Double)
        Dim ppm As Double = Val(RibbonItems.Spinner.DecimalValue)
        Dim raw = TreeView1.CurrentRawFile.raw
        Dim ms2Hits = raw.scans.Where(Function(m) PPMmethod.ppm(m.mz, mz) <= ppm).ToArray

        ListBox1.Items.Clear()

        For Each hit As ScanEntry In ms2Hits
            ListBox1.Items.Add(hit)
        Next
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        Dim scanId As ScanEntry = ListBox1.SelectedItem
        Dim raw = TreeView1.CurrentRawFile.raw

        Call showSpectrum(scanId.id, raw)
    End Sub

    Private Sub SearchInFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SearchInFileToolStripMenuItem.Click
        If Not ShowTICToolStripMenuItem.Checked Then
            Dim current = TreeView1.CurrentRawFile
            Dim node = TreeView1.SelectedNode

            If Not node Is Nothing AndAlso current.raw.cache.FileExists Then
                Dim mz = current.raw.scans.Where(Function(scan) scan.id = node.Text).FirstOrDefault

                If Not mz Is Nothing AndAlso mz.mz > 0 Then
                    Call searchInFileByMz(mz.mz)
                End If
            End If
        End If
    End Sub
End Class
