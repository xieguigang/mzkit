#Region "Microsoft.VisualBasic::32e09ae48d57171f68c1c252249148e9, src\mzkit\mzkit\pages\dockWindow\explorer\frmSearchList.vb"

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

    ' Class frmSearchList
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetXICCollection
    ' 
    '     Sub: frmSearchList_Closing, ListBox1_SelectedIndexChanged, searchInFileByMz, ShowSummaryMatrixToolStripMenuItem_Click, ShowXICToolStripMenuItem_Click
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports mzkit.My
Imports Task
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmSearchList

    Dim raw As Raw
    Dim ppm As Double
    Dim mz As Double

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        DoubleBuffered = True
        Me.TabText = "Feature List"

        ContextMenuStrip1.RenderMode = ToolStripRenderMode.System

    End Sub

    Private Sub frmSearchList_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        Dim scanId As ScanEntry = TryCast(ListBox1.SelectedItem, ScanEntry)

        If Not scanId Is Nothing Then
            Call MyApplication.host.mzkitTool.showSpectrum(scanId.id, raw)
        Else
            MyApplication.host.showStatusMessage("no ion scan was selected...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        End If
    End Sub

    Friend Sub searchInFileByMz(mz As Double, ppm As Double, raw As Raw)
        Dim ms2Hits = raw.GetMs2Scans.Where(Function(m) PPMmethod.PPM(m.mz, mz) <= ppm).ToArray

        ListBox1.Items.Clear()

        For Each hit As ScanEntry In ms2Hits
            ListBox1.Items.Add(hit)
        Next

        If ms2Hits.Length = 0 Then
            Label2.Text = "no hits!"
            MessageBox.Show($"Sorry, no hits was found for m/z={mz} with tolerance {ppm}ppm...", "No hits found!", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Me.Label2.Text = $"{ms2Hits.Length} ms2 hits for m/z={mz} with tolerance {ppm}ppm"
            Me.raw = raw
            Me.ppm = ppm
            Me.mz = mz
        End If

        Dim dockLeft As Boolean = DockState = DockState.Hidden OrElse DockState = DockState.Unknown

        Call Show(MyApplication.host.dockPanel)
        Call Activate()

        If dockLeft Then
            DockState = DockState.DockLeft
        End If
    End Sub

    Private Sub ShowXICToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowXICToolStripMenuItem.Click
        If ListBox1.Items.Count = 0 Then
            MyApplication.host.showStatusMessage("no ion scan was selected...", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        Call MyApplication.host.mzkitTool.ShowXIC(ppm, Nothing, AddressOf GetXICCollection, raw.GetXICMaxYAxis)
    End Sub

    Public Iterator Function GetXICCollection(ppm As Double) As IEnumerable(Of NamedCollection(Of ChromatogramTick))
        Dim ticks As New List(Of ChromatogramTick)

        For i As Integer = 0 To ListBox1.Items.Count - 1
            Dim scanId As String = ListBox1.Items(i).ToString
            Dim scan As ScanEntry = raw.scans.Where(Function(a) a.id = scanId).First

            ticks.Add(New ChromatogramTick With {.Time = scan.rt, .Intensity = scan.XIC})
        Next

        ticks.Add(New ChromatogramTick With {.Time = 0, .Intensity = 0})
        ticks.Add(New ChromatogramTick With {.Time = raw.rtmax, .Intensity = 0})

        Yield New NamedCollection(Of ChromatogramTick) With {
            .name = $"{mz.ToString("F4")}[{ppm} ppm] @ {raw.source.FileName}",
            .value = ticks _
                .OrderBy(Function(t) t.Time) _
                .ToArray
        }
    End Function

    Private Sub ShowSummaryMatrixToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowSummaryMatrixToolStripMenuItem.Click
        Dim ticks As New List(Of ChromatogramTick)

        For i As Integer = 0 To ListBox1.Items.Count - 1
            Dim scanId As String = ListBox1.Items(i).ToString
            Dim scan As ScanEntry = raw.scans.Where(Function(a) a.id = scanId).First

            ticks.Add(New ChromatogramTick With {.Time = scan.rt, .Intensity = scan.XIC})
        Next

        Call MyApplication.host.mzkitTool.showMatrix(ticks _
                .OrderBy(Function(t) t.Time) _
                .ToArray, $"{mz.ToString("F4")}[{ppm} ppm] @ {raw.source.FileName}")
        Call MyApplication.host.mzkitTool.ShowTabPage(MyApplication.host.mzkitTool.TabPage6)
    End Sub
End Class
