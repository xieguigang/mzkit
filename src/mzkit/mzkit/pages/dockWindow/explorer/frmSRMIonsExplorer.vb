#Region "Microsoft.VisualBasic::0a72fdaf4c65e81f7586319226749536, src\mzkit\mzkit\pages\dockWindow\explorer\frmSRMIonsExplorer.vb"

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

    ' Class frmSRMIonsExplorer
    ' 
    '     Function: GetIonTICOverlaps
    ' 
    '     Sub: frmSRMIonsExplorer_Load, LoadMRM, ShowSpectrumToolStripMenuItem_Click, ShowTICOverlap3DToolStripMenuItem_Click, ShowTICOverlapToolStripMenuItem_Click
    '          Win7StyleTreeView1_AfterSelect
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports mzkit.My
Imports Task

Public Class frmSRMIonsExplorer

    Public Sub LoadMRM(file As String)
        Dim list = file.LoadChromatogramList.ToArray
        Dim TIC = list.Where(Function(i) i.id.TextEquals("TIC")).First

        ' Call Win7StyleTreeView1.Nodes.Clear()

        Dim TICRoot As TreeNode = Win7StyleTreeView1.Nodes.Add(file.FileName)

        TICRoot.Tag = TIC
        TICRoot.ImageIndex = 0

        Dim ionsLib As IonLibrary = Globals.LoadIonLibrary
        Dim display As String

        For Each chr As chromatogram In list.Where(Function(i) Not i.id.TextEquals("TIC"))
            Dim ionRef As New IonPair With {
                .precursor = chr.precursor.MRMTargetMz,
                .product = chr.product.MRMTargetMz
            }

            display = ionsLib.GetDisplay(ionRef)

            With TICRoot.Nodes.Add(display)
                .Tag = chr
                .ImageIndex = 1
                .SelectedImageIndex = 1
            End With
        Next
    End Sub

    Private Sub frmSRMIonsExplorer_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = "MRM Ions"

        Call ApplyVsTheme(ContextMenuStrip1, ToolStrip1)
    End Sub

    Private Sub Win7StyleTreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles Win7StyleTreeView1.AfterSelect
        Dim chr As chromatogram = e.Node.Tag
        Dim ticks As ChromatogramTick() = chr.Ticks
        Dim proper As New MRMROIProperty(chr)

        Call MyApplication.host.mzkitTool.ShowMRMTIC(e.Node.Text, ticks)
        Call VisualStudio.ShowProperties(proper)
    End Sub

    Private Sub ShowSpectrumToolStripMenuItem_Click(sender As Object, e As EventArgs)
        If Win7StyleTreeView1.SelectedNode Is Nothing OrElse DirectCast(Win7StyleTreeView1.SelectedNode.Tag, chromatogram).id = "TIC" Then
            Return
        End If

        Dim chr As chromatogram = Win7StyleTreeView1.SelectedNode.Tag
        Dim spectrum As ms2() = {
            New ms2 With {.mz = chr.precursor.MRMTargetMz, .intensity = 1},
            New ms2 With {.mz = chr.product.MRMTargetMz, .intensity = 0.6}
        }
        Dim scanData As New LibraryMatrix With {.ms2 = spectrum, .name = "SRM ions"}
        Dim q = scanData.OrderByDescending(Function(x) x.intensity).First
        Dim title1$ = $"SRM ion pair"
        Dim title2$ = $"[{spectrum(0).mz.ToString("F4")}:{spectrum(1).intensity.ToString("G3")}]"

        Call MyApplication.host.mzkitTool.showMatrix(spectrum, $"SRM ion pair [{spectrum(0).mz.ToString("F4")}:{spectrum(1).intensity.ToString("G3")}]")
        Call MyApplication.host.mzkitTool.PlotMatrx(title1, title2, scanData)
    End Sub

    Private Sub ShowTICOverlapToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowTICOverlapToolStripMenuItem.Click
        Call MyApplication.host.mzkitTool.TIC(GetIonTICOverlaps.ToArray)
    End Sub

    Private Iterator Function GetIonTICOverlaps() As IEnumerable(Of NamedCollection(Of ChromatogramTick))
        For Each rawfile As TreeNode In Win7StyleTreeView1.Nodes
            Dim fileName As String = rawfile.Text.BaseName

            For Each obj As TreeNode In rawfile.Nodes
                If Not obj.Checked Then
                    Continue For
                End If

                With DirectCast(obj.Tag, chromatogram)
                    Yield New NamedCollection(Of ChromatogramTick)($"[{fileName}] {obj.Text}", .Ticks)
                End With
            Next
        Next
    End Function

    Private Sub ShowTICOverlap3DToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowTICOverlap3DToolStripMenuItem.Click
        Call MyApplication.host.mzkitTool.TIC(GetIonTICOverlaps.ToArray, d3:=True)
    End Sub
End Class
