#Region "Microsoft.VisualBasic::a5776bc74fa6aa680af48fce8704e4ea, src\mzkit\mzkit\pages\dockWindow\documents\frmUntargettedViewer.vb"

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

        Call showTIC()

        If Not MsSelector1.ContextMenuStrip1 Is Nothing Then
            Call ApplyVsTheme(MsSelector1.ContextMenuStrip1)
        End If
    End Sub

    Dim matrix As LibraryMatrix

    Private Sub RtRangeSelector1_RangeSelect(min As Double, max As Double) Handles MsSelector1.RangeSelect
        Dim MS1 = raw.GetMs1Scans _
            .Where(Function(m1) m1.rt >= min AndAlso m1.rt <= max) _
            .Select(Function(t) t.GetMs) _
            .IteratesALL _
            .ToArray _
            .Centroid(Tolerance.DeltaMass(0.1), LowAbundanceTrimming.intoCutff) _
            .ToArray

        If MS1.Length > 0 Then
            Dim msLib As New LibraryMatrix With {
                .centroid = True,
                .ms2 = MS1,
                .name = $"Rt: {CInt(min)} ~ {CInt(max)} sec"
            }
            Dim plot As Image = PeakAssign _
                .DrawSpectrumPeaks(msLib, size:=$"{PictureBox1.Width},{PictureBox1.Height}") _
                .AsGDIImage

            matrix = msLib
            PictureBox1.BackgroundImage = plot
        End If
    End Sub

    Private Sub showTIC() Handles MsSelector1.ShowTIC
        Dim TIC As ChromatogramTick() = raw _
            .GetMs1Scans _
            .Select(Function(t)
                        Return New ChromatogramTick With {.Time = t.rt, .Intensity = t.TIC}
                    End Function) _
            .ToArray

        Call MsSelector1.SetTIC(TIC)
        Call MsSelector1.RefreshRtRangeSelector()
    End Sub

    Private Sub BPCToolStripMenuItem_Click() Handles MsSelector1.ShowBPC
        Dim BPC As ChromatogramTick() = raw _
            .GetMs1Scans _
            .Select(Function(t)
                        Return New ChromatogramTick With {.Time = t.rt, .Intensity = t.BPC}
                    End Function) _
            .ToArray

        Call MsSelector1.SetTIC(BPC)
    End Sub

    Private Sub frmUntargettedViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Icon = My.Resources.xobject
        Me.SaveDocumentToolStripMenuItem.Enabled = False

        Call ApplyVsTheme(ContextMenuStrip1)
    End Sub

    Private Sub FilterMs2ToolStripMenuItem_Click(rtmin As Double, rtmax As Double) Handles MsSelector1.FilterMs2
        Call WindowModules.rawFeaturesList.LoadRaw(raw, rtmin, rtmax)
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

    Private Sub ShowMatrixToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowMatrixToolStripMenuItem.Click
        If matrix Is Nothing Then
            Call MyApplication.host.showStatusMessage("You should select data below at first!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Call MyApplication.mzkitRawViewer.showMatrix(matrix.ms2, matrix.name)
            Call MyApplication.mzkitRawViewer.PlotSpectrum(scanData:=matrix, focusOn:=True)
        End If
    End Sub
End Class
