#Region "Microsoft.VisualBasic::aea2f194b3e2ac3082d810490f7b5269, pages\dockWindow\explorer\frmGCMSPeaks.vb"

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

    ' Class frmGCMSPeaks
    ' 
    '     Function: ContainsRaw
    ' 
    '     Sub: frmGCMSPeaks_Load, LoadRawExplorer, ShowRaw, Win7StyleTreeView1_AfterCheck, Win7StyleTreeView1_AfterSelect
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports mzkit.My
Imports Task
Imports WeifenLuo.WinFormsUI.Docking
Imports Raw = BioNovoGene.Analytical.MassSpectrometry.Math.GCMS.Raw

Public Class frmGCMSPeaks

    ''' <summary>
    ''' [filename -> viewer]
    ''' </summary>
    Dim gcmsRaw As New Dictionary(Of String, frmGCMS_CDFExplorer)

    Private Sub frmGCMSPeaks_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = "GCMS Feature Peaks"

        ApplyVsTheme(ToolStrip1)
    End Sub

    Public Function ContainsRaw(filepath As String) As Boolean
        Return gcmsRaw.ContainsKey(filepath.GetFullPath)
    End Function

    Public Sub LoadRawExplorer(gcmsRaw As Raw)
        Dim TICRoot = Win7StyleTreeView1.Nodes.Add(gcmsRaw.fileName.FileName)
        Dim CDFExplorer As New frmGCMS_CDFExplorer
        Dim TIC = gcmsRaw.GetTIC

        TICRoot.Tag = gcmsRaw

        CDFExplorer.Show(DockPanel)
        CDFExplorer.DockState = DockState.Document
        CDFExplorer.loadCDF(gcmsRaw)

        Me.gcmsRaw(gcmsRaw.fileName.GetFullPath) = CDFExplorer

        Dim ROIlist As ROI() = TIC.Shadows _
            .PopulateROI(
                baselineQuantile:=0.65,
                angleThreshold:=5,
                peakwidth:=New Double() {8, 30},
                snThreshold:=3
            ) _
            .ToArray

        Dim peakNode As TreeNode = TICRoot.Nodes.Add("Peaks ROI")

        peakNode.Tag = gcmsRaw

        For Each peak As ROI In ROIlist
            Call New TreeNode With {
                .Text = $"{CInt(peak.time.Min)} ~ {CInt(peak.time.Max)} [{peak.integration.ToString("F3")}]",
                .Tag = peak,
                .ImageIndex = 1,
                .SelectedImageIndex = 1
            }.DoCall(AddressOf peakNode.Nodes.Add)
        Next

        Dim XICNode As TreeNode = TICRoot.Nodes.Add("XIC")

        XICNode.Tag = gcmsRaw

        For Each XIC As NamedCollection(Of ms1_scan) In gcmsRaw.XIC
            Call New TreeNode With {
                .Text = $"m/z {Val(XIC.name).ToString("F4")}",
                .Tag = XIC.ToArray,
                .ImageIndex = 1,
                .SelectedImageIndex = 1
            }.DoCall(AddressOf XICNode.Nodes.Add)
        Next
    End Sub

    Private Sub Win7StyleTreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles Win7StyleTreeView1.AfterSelect
        If TypeOf e.Node.Tag Is ROI Then
            Dim rtRange As DoubleRange = DirectCast(e.Node.Tag, ROI).time
            Dim proper As New ROIProperty(e.Node.Tag)
            Dim parent = e.Node.Parent
            Dim explorer = Me.gcmsRaw(DirectCast(parent.Tag, Raw).fileName.GetFullPath)

            Call explorer.RtRangeSelector1_RangeSelect(rtRange.Min, rtRange.Max)
            Call explorer.Show(MyApplication.host.dockPanel)
            Call explorer.SetRange(rtRange.Min, rtRange.Max)

            Call VisualStudio.ShowProperties(proper)
        ElseIf TypeOf e.Node.Tag Is Raw Then
            Call ShowRaw(DirectCast(e.Node.Tag, Raw).fileName)
        ElseIf TypeOf e.Node.Tag Is ms1_scan() Then
            Dim XIC As New NamedCollection(Of ChromatogramTick) With {
                .name = e.Node.Text,
                .value = DirectCast(e.Node.Tag, ms1_scan()) _
                    .Select(Function(t)
                                Return New ChromatogramTick With {
                                    .Time = t.scan_time,
                                    .Intensity = t.intensity
                                }
                            End Function) _
                    .ToArray
            }

            Call MyApplication.host.mzkitTool.ShowMRMTIC(XIC.name, XIC.value)
            Call VisualStudio.ShowProperties(New agilentGCMSMeta(DirectCast(e.Node.Parent.Tag, Raw).attributes))
        End If
    End Sub

    Public Sub ShowRaw(file As String)
        Dim gcmsRaw As frmGCMS_CDFExplorer = Me.gcmsRaw(file.GetFullPath)
        Dim TIC As NamedCollection(Of ChromatogramTick) = gcmsRaw.gcms.GetTIC

        gcmsRaw.Show(DockPanel)
        gcmsRaw.DockState = DockState.Document

        Call MyApplication.host.mzkitTool.ShowMRMTIC(TIC.name, TIC.value)
        Call VisualStudio.ShowProperties(New agilentGCMSMeta(gcmsRaw.gcms.attributes))
    End Sub

    Private Sub Win7StyleTreeView1_AfterCheck(sender As Object, e As TreeViewEventArgs) Handles Win7StyleTreeView1.AfterCheck

    End Sub
End Class
