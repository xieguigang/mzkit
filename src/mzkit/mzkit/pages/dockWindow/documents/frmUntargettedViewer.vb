#Region "Microsoft.VisualBasic::d6072fc4582c504cc02f034676a0a350, src\mzkit\mzkit\pages\dockWindow\documents\frmUntargettedViewer.vb"

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

    ' Class frmUntargettedViewer
    ' 
    '     Sub: BPCToolStripMenuItem_Click, CopyFullPath, FilterMs2ToolStripMenuItem_Click, frmUntargettedViewer_Load, loadRaw
    '          OpenContainingFolder, RtRangeSelector1_RangeSelect, SaveDocument, showTIC
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports mzkit.My
Imports Task
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmUntargettedViewer

    Dim raw As Raw

    Public Sub loadRaw(raw As Raw)
        Me.raw = raw
        Me.raw.LoadMzpack()
        Me.TabText = raw.source.FileName
        Me.RtRangeSelector1.BackColor = Color.White

        Call showTIC()
    End Sub

    Private Sub RtRangeSelector1_RangeSelect(min As Double, max As Double) Handles RtRangeSelector1.RangeSelect
        Dim MS1 = raw.GetMs1Scans _
            .Where(Function(m1) m1.rt >= min AndAlso m1.rt <= max) _
            .Select(Function(t) t.GetMs) _
            .IteratesALL _
            .ToArray _
            .Centroid(Tolerance.DeltaMass(0.1), LowAbundanceTrimming.intoCutff) _
            .ToArray

        If MS1.Length > 0 Then
            Dim msLib As New LibraryMatrix With {.centroid = True, .ms2 = MS1, .name = "MS1"}
            Dim plot As Image = msLib.MirrorPlot(titles:={"MS1", $"Rt: {CInt(min)} ~ {CInt(max)} sec"}).AsGDIImage

            PictureBox1.BackgroundImage = plot
        End If
    End Sub

    Private Sub showTIC() Handles TICToolStripMenuItem.Click
        TICToolStripMenuItem.Checked = True
        BPCToolStripMenuItem.Checked = False

        Dim TIC As ChromatogramTick() = raw _
            .GetMs1Scans _
            .Select(Function(t)
                        Return New ChromatogramTick With {.Time = t.rt, .Intensity = t.TIC}
                    End Function) _
            .ToArray

        Call RtRangeSelector1.SetTIC(TIC)
        Call RtRangeSelector1.RefreshRtRangeSelector()
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

        Me.SaveDocumentToolStripMenuItem.Enabled = False
    End Sub

    Private Sub FilterMs2ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FilterMs2ToolStripMenuItem.Click
        Call WindowModules.rawFeaturesList.LoadRaw(raw, RtRangeSelector1.rtmin, RtRangeSelector1.rtmax)
        Call VisualStudio.Dock(WindowModules.rawFeaturesList, DockState.DockLeft)
    End Sub

    Protected Overrides Sub CopyFullPath()
        Call Clipboard.SetText(raw.source)
    End Sub

    Protected Overrides Sub OpenContainingFolder()
        Call Process.Start(raw.source.ParentPath)
    End Sub

    Protected Overrides Sub SaveDocument()
        MyBase.SaveDocument()
    End Sub
End Class
