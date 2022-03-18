#Region "Microsoft.VisualBasic::c37a95dbbd1d71a59ccc9f66e0157503, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmFeatureSearch.vb"

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

    '   Total Lines: 424
    '    Code Lines: 315
    ' Comment Lines: 28
    '   Blank Lines: 81
    '     File Size: 17.52 KB


    ' Class frmFeatureSearch
    ' 
    '     Properties: FilePath, MimeType
    ' 
    '     Function: (+2 Overloads) Save
    ' 
    '     Sub: (+2 Overloads) AddFileMatch, ApplyFeatureFilterToolStripMenuItem_Click, frmFeatureSearch_Load, RunMs2ClusteringToolStripMenuItem_Click, ViewToolStripMenuItem_Click
    '          ViewXICToolStripMenuItem_Click
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Threading
Imports System.Windows.Forms.ListViewItem
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.mzkit_win32.My
Imports ControlLibrary
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Text
Imports RibbonLib.Controls.Events
Imports RibbonLib.Interop
Imports Task

Public Class frmFeatureSearch : Implements ISaveHandle, IFileReference

    Dim appendHeader As Boolean = False

    ''' <summary>
    ''' raw source list for ms1 search
    ''' </summary>
    Dim list1 As New List(Of (File As String, matches As ParentMatch()))
    ''' <summary>
    ''' raw source list for ms2 search
    ''' </summary>
    Dim list2 As New List(Of (file As String, targetMz As Double, matches As ScanMS2()))
    Dim rangeMin As Double = 999999999
    Dim rangeMax As Double = -99999999999999

    Public Sub AddFileMatch(file As String, matches As ParentMatch())
        list1.Add((file, matches))

        If Not appendHeader Then
            Dim matchHeaders = {
                New ColumnHeader() With {.Text = "Precursor Type"},
                New ColumnHeader() With {.Text = "Adducts"},
                New ColumnHeader() With {.Text = "M"}
            }

            Me.TreeListView1.Columns.AddRange(matchHeaders)
            Me.appendHeader = True
        End If

        Dim row As New TreeListViewItem With {.Text = file.FileName, .ImageIndex = 0, .ToolTipText = file}
        Dim i As i32 = 1

        For Each member As ParentMatch In matches
            Dim ion As New TreeListViewItem(member.scan_id) With {.ImageIndex = 1, .ToolTipText = member.scan_id, .Tag = member}

            ion.SubItems.Add(New ListViewSubItem With {.Text = $"#{++i}"})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.parentMz.ToString("F4")})
            ion.SubItems.Add(New ListViewSubItem With {.Text = CInt(member.rt)})
            ion.SubItems.Add(New ListViewSubItem With {.Text = (member.rt / 60).ToString("F1")})
            ion.SubItems.Add(New ListViewSubItem With {.Text = CInt(member.ppm)})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.polarity})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.charge})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.into.Max.ToString("G3")})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.into.Sum.ToString("G3")})

            ion.SubItems.Add(New ListViewSubItem With {.Text = member.precursor_type})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.adducts})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.M})

            If rangeMin > member.rt Then
                rangeMin = member.rt
            End If
            If rangeMax < member.rt Then
                rangeMax = member.rt
            End If

            row.Items.Add(ion)
        Next

        row.SubItems.Add(New ListViewSubItem With {.Text = matches.Length})

        TreeListView1.Items.Add(row)
    End Sub

    Public Sub AddFileMatch(file As String, targetMz As Double, matches As ScanMS2())
        Dim row As New TreeListViewItem With {.Text = file.FileName, .ImageIndex = 0, .ToolTipText = file}
        Dim i As i32 = 1

        list2.Add((file, targetMz, matches))

        For Each member As ScanMS2 In matches
            Dim ion As New TreeListViewItem(member.scan_id) With {.ImageIndex = 1, .ToolTipText = member.scan_id, .Tag = member}

            ion.SubItems.Add(New ListViewSubItem With {.Text = $"#{++i}"})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.parentMz.ToString("F4")})
            ion.SubItems.Add(New ListViewSubItem With {.Text = CInt(member.rt)})
            ion.SubItems.Add(New ListViewSubItem With {.Text = (member.rt / 60).ToString("F1")})
            ion.SubItems.Add(New ListViewSubItem With {.Text = CInt(PPMmethod.PPM(member.parentMz, targetMz))})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.polarity})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.charge})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.into.Max.ToString("G3")})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.into.Sum.ToString("G3")})

            If rangeMin > member.rt Then
                rangeMin = member.rt
            End If
            If rangeMax < member.rt Then
                rangeMax = member.rt
            End If

            row.Items.Add(ion)
        Next

        row.SubItems.Add(New ListViewSubItem With {.Text = matches.Length})

        TreeListView1.Items.Add(row)
    End Sub

    Friend directRaw As Raw

    Public Property FilePath As String Implements IFileReference.FilePath

    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
        Get
            Return {
                New ContentType With {.Details = "Microsoft Excel Table", .FileExt = ".csv", .MIMEType = "application/csv", .Name = "Microsoft Excel Table"}
            }
        End Get
    End Property

    Private Sub ViewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewToolStripMenuItem.Click
        Dim cluster As TreeListViewItem
        Dim host = MyApplication.host

        If TreeListView1.SelectedItems.Count = 0 Then
            Return
        Else
            cluster = TreeListView1.SelectedItems(0)
        End If

        ' 当没有feature搜索结果的时候， children count也是零
        ' 但是raw文件的parent是空的
        ' 所以还需要加上parent是否为空的判断来避免无结果产生的冲突
        If cluster.ChildrenCount > 0 OrElse cluster.Parent Is Nothing Then
            ' 选择的是一个文件节点
            Dim filePath As String = cluster.ToolTipText
            Dim raw As Raw

            If Not directRaw Is Nothing Then
                raw = directRaw
            Else
                raw = Globals.workspace.FindRawFile(filePath)
            End If

            If Not raw Is Nothing Then
                Call MyApplication.mzkitRawViewer.showScatter(raw, XIC:=False, directSnapshot:=True, contour:=False)
            End If
        Else
            ' 选择的是一个scan数据节点
            Dim parentFile = cluster.Parent.ToolTipText
            Dim scan_id As String = cluster.Text

            MyApplication.host.ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active

            ' scan节点
            Dim raw As Task.Raw

            If directRaw Is Nothing Then
                raw = Globals.workspace.FindRawFile(parentFile)
            Else
                raw = directRaw
            End If

            Call MyApplication.host.mzkitTool.showSpectrum(scan_id, raw)
            Call MyApplication.host.mzkitTool.ShowPage()
        End If
    End Sub

    Private Sub frmFeatureSearch_Load(sender As Object, e As EventArgs) Handles Me.Load
        Text = "Feature Search Result"
        TabText = Text
        Icon = My.Resources.Search

        OpenContainingFolderToolStripMenuItem.Enabled = False
        CopyFullPathToolStripMenuItem.Enabled = False
        SaveDocumentToolStripMenuItem.Enabled = False

        Call ApplyVsTheme(ContextMenuStrip1)

        Static proxy As EventHandler(Of ExecuteEventArgs)

        ' 20220218 makes bugs fixed of the event handler
        proxy = Sub()
                    ppm = 30
                    rtmin = rangeMin
                    rtmax = rangeMax
                    types.Clear()

                    Call ApplyFeatureFilterToolStripMenuItem_Click(Nothing, Nothing)

                    MessageBox.Show("All feature filter condition has been clear!", "Reset Feature Filter", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Sub

        AddHandler ribbonItems.ButtonResetFeatureFilter.ExecuteEvent, proxy
    End Sub

    Private Sub ViewXICToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewXICToolStripMenuItem.Click
        Dim cluster As TreeListViewItem
        Dim host = MyApplication.host

        If TreeListView1.SelectedItems.Count = 0 Then
            Return
        Else
            cluster = TreeListView1.SelectedItems(0)
        End If

        ' 当没有feature搜索结果的时候， children count也是零
        ' 但是raw文件的parent是空的
        ' 所以还需要加上parent是否为空的判断来避免无结果产生的冲突
        If cluster.ChildrenCount > 0 OrElse cluster.Parent Is Nothing Then
            Call MyApplication.host.showStatusMessage("Select a ms2 feature for view XIC plot!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            ' 选择的是一个scan数据节点
            Dim parentFile = cluster.Parent.ToolTipText
            Dim scan_id As String = cluster.Text

            MyApplication.host.ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active

            ' scan节点
            Dim raw As Task.Raw

            If directRaw Is Nothing Then
                raw = Globals.workspace.FindRawFile(parentFile)
            Else
                raw = directRaw
            End If

            Dim scan = raw.FindMs2Scan(scan_id)

            If scan Is Nothing Then
                Call MyApplication.host.showStatusMessage($"no scan data was found for scan id: {scan_id}!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Else
                Dim mz As Double = scan.parentMz
                Dim ppm As New PPMmethod(30)
                Dim GetXICCollection =
                    Iterator Function() As IEnumerable(Of NamedCollection(Of ChromatogramTick))
                        Dim ticks As ChromatogramTick() = raw _
                            .GetMs1Scans _
                            .Select(Function(s)
                                        Return New ChromatogramTick With {
                                            .Intensity = s.GetIntensity(mz, ppm),
                                            .Time = s.rt
                                        }
                                    End Function) _
                            .ToArray

                        Yield New NamedCollection(Of ChromatogramTick) With {
                            .name = $"{mz.ToString("F4")} @ {raw.source.FileName}",
                            .value = ticks
                        }
                    End Function

                ' Call MyApplication.host.mzkitTool.showSpectrum(scan_id, raw)
                Call MyApplication.mzkitRawViewer.ShowXIC(ppm.DeltaTolerance, Nothing, GetXICCollection, 0)
                Call MyApplication.host.mzkitTool.ShowPage()
            End If
        End If
    End Sub

    Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        Dim file As New File
        Dim row As New List(Of String)

        For i As Integer = 0 To TreeListView1.Columns.Count - 1
            row.Add(TreeListView1.Columns(i).Text)
        Next

        file.Add(New RowObject(row))

        For Each item As TreeListViewItem In TreeListView1.Items
            Dim tag As String = item.Text

            For Each feature As ListViewItem In item.Items
                row.Clear()
                row.Add(tag)

                For Each cell As ListViewSubItem In feature.SubItems
                    row.Add(cell.Text)
                Next

                Call file.Add(New RowObject(row))
            Next
        Next

        Return file.Save(path, encoding)
    End Function

    Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
        Return Save(path, encoding.CodePage)
    End Function

    Dim rtmin As Double = Double.NaN
    Dim rtmax As Double = Double.NaN
    Dim ppm As Double = 30
    Dim types As New Dictionary(Of String, Boolean)

    Private Sub ApplyFeatureFilterToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ApplyFeatureFilterToolStripMenuItem.Click
        Dim getFilters As New InputFeatureFilter
        Dim mask As New MaskForm(MyApplication.host.Location, MyApplication.host.Size)

        If rtmin.IsNaNImaginary Then
            rtmin = rangeMin
        End If
        If rtmax.IsNaNImaginary Then
            rtmax = rangeMax
        End If

        If Not list1.IsNullOrEmpty Then
            If types.IsNullOrEmpty Then
                types = list1 _
                    .Select(Function(f) f.matches) _
                    .IteratesALL _
                    .Select(Function(a) a.precursor_type) _
                    .Distinct _
                    .ToDictionary(Function(type) type,
                                  Function(any)
                                      Return True
                                  End Function)
            End If

            getFilters.AddTypes(types)
        End If

        getFilters.txtPPM.Text = ppm
        getFilters.txtRtMax.Text = rtmax
        getFilters.txtRtMin.Text = rtmin

        If mask.ShowDialogForm(getFilters) = DialogResult.OK Then
            rtmin = Val(getFilters.txtRtMin.Text)
            rtmax = Val(getFilters.txtRtMax.Text)
            ppm = Val(getFilters.txtPPM.Text)

            If rtmin = rtmax OrElse (rtmin = rtmax AndAlso rtmin = 0.0) OrElse rtmin > rtmax Then
                Call MyApplication.host.showStatusMessage("invalid filter value...", My.Resources.StatusAnnotations_Warning_32xLG_color)
                Return
            Else
                Call TreeListView1.Items.Clear()
            End If

            If Not list1.IsNullOrEmpty Then
                Dim source = list1.ToArray
                Dim requiredTypes As Index(Of String) = getFilters.GetTypes
                Dim filter = list1 _
                    .Select(Function(i)
                                Return (i.File, i.matches.Where(Function(p) p.rt >= rtmin AndAlso p.rt <= rtmax AndAlso p.ppm <= ppm AndAlso p.precursor_type Like requiredTypes).ToArray)
                            End Function) _
                    .ToArray

                For Each type As String In types.Keys.ToArray
                    types(type) = type Like requiredTypes
                Next

                For Each row In filter
                    Call Me.AddFileMatch(row.File, row.ToArray)
                Next

                list1.Clear()
                list1.AddRange(source)
            ElseIf Not list2.IsNullOrEmpty Then
                Dim source = list2.ToArray
                Dim filter = list2 _
                    .Select(Function(i)
                                Return (i.file, i.targetMz, i.matches.Where(Function(p) p.rt >= rtmin AndAlso p.rt <= rtmax).ToArray)
                            End Function) _
                    .ToArray

                For Each row In filter
                    Call Me.AddFileMatch(row.file, row.targetMz, row.ToArray)
                Next

                list2.Clear()
                list2.Add(source)
            End If
        End If
    End Sub

    ''' <summary>
    ''' 进行分子网络的建立来完成二级聚类
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub RunMs2ClusteringToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RunMs2ClusteringToolStripMenuItem.Click
        If list1.IsNullOrEmpty Then
            ' is ms2 search
            ' scanms2
            Throw New NotImplementedException
        Else
            ' is ms1 search
            ' parent match
            Dim parents As New List(Of ParentMatch)

            For Each fileRow As TreeListViewItem In TreeListView1.Items
                For Each feature As TreeListViewItem In fileRow.Items
                    Call parents.Add(feature.Tag)
                Next
            Next

            Dim peaksData As PeakMs2() = parents.Select(Function(p) p.ToMs2).ToArray
            Dim progress As New frmTaskProgress

            progress.ShowProgressTitle("Build Molecular Networking...", directAccess:=True)
            progress.ShowProgressDetails("Run ms2 clustering!", directAccess:=True)

            Call New Thread(Sub()
                                Call Thread.Sleep(500)
                                Call MyApplication.host.mzkitTool.MolecularNetworkingTool(peaksData, progress, 0.8)
                                Call progress.Invoke(Sub() progress.Close())
                            End Sub).Start()

            Call progress.ShowDialog()
        End If
    End Sub
End Class
