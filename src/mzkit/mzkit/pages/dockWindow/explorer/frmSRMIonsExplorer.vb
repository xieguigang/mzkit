#Region "Microsoft.VisualBasic::c3bd73918006a5b3b4ba5ca4687b80ae, mzkit\src\mzkit\mzkit\pages\dockWindow\explorer\frmSRMIonsExplorer.vb"

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

    '   Total Lines: 188
    '    Code Lines: 137
    ' Comment Lines: 15
    '   Blank Lines: 36
    '     File Size: 7.97 KB


    ' Class frmSRMIonsExplorer
    ' 
    '     Function: GetFileTICOverlaps, GetIonTICOverlaps
    ' 
    '     Sub: BPCToolStripMenuItem_Click, ClearFileSelectionsToolStripMenuItem_Click, ClearFilesToolStripMenuItem_Click, ClearIonSelectionsToolStripMenuItem_Click, frmSRMIonsExplorer_Load
    '          ImportsFilesToolStripMenuItem_Click, LoadMRM, SelectAllFilesToolStripMenuItem_Click, SelectAllIonsToolStripMenuItem_Click, ShowTICOverlap3DToolStripMenuItem_Click
    '          ShowTICOverlap3DToolStripMenuItem1_Click, ShowTICOverlapToolStripMenuItem1_Click, TICToolStripMenuItem_Click, Win7StyleTreeView1_AfterSelect
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports BioNovoGene.mzkit_win32.My
Imports Task
Imports chromatogram = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.chromatogram

Public Class frmSRMIonsExplorer

    Private Sub ImportsFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImportsFilesToolStripMenuItem.Click
        Using openfile As New OpenFileDialog With {.Filter = "LC-MSMS(*.mzML)|*.mzML", .Multiselect = True}
            If openfile.ShowDialog = DialogResult.OK Then
                Dim notMRM As New List(Of String)

                For Each file As String In openfile.FileNames
                    If RawScanParser.IsMRMData(file) Then
                        Call LoadMRM(file)
                    Else
                        Call MyApplication.LogText($"{file} is not a MRM raw data file!")
                        Call notMRM.Add(file.FileName)
                    End If
                Next

                If notMRM.Count > 0 Then
                    MessageBox.Show($"These files are not MRM data files, load of the files was ignored:" & vbCrLf & notMRM.JoinBy(vbCrLf), "mzkit", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If
        End Using
    End Sub

    Public Sub LoadMRM(file As String)
        Dim list = file.LoadChromatogramList.ToArray
        Dim TIC As DataReader.Chromatogram = list.GetIonsChromatogram

        ' Call Win7StyleTreeView1.Nodes.Clear()

        Dim TICRoot As TreeNode = Win7StyleTreeView1.Nodes.Add(file.FileName)

        TICRoot.Tag = TIC
        TICRoot.ImageIndex = 0
        TICRoot.ContextMenuStrip = ContextMenuStrip1

        Dim ionsLib As IonLibrary = Globals.LoadIonLibrary
        Dim display As String

        For Each chr As chromatogram In list.Where(Function(i)
                                                       Return Not (i.id.TextEquals("TIC") OrElse i.id.TextEquals("BPC"))
                                                   End Function)
            Dim ionRef As New IonPair With {
                .precursor = chr.precursor.MRMTargetMz,
                .product = chr.product.MRMTargetMz
            }

            display = ionsLib.GetDisplay(ionRef)

            With TICRoot.Nodes.Add(display)
                .Tag = chr
                .ImageIndex = 1
                .SelectedImageIndex = 1
                .ContextMenuStrip = ContextMenuStrip2
            End With
        Next
    End Sub

    Private Sub frmSRMIonsExplorer_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = "MRM Ions"

        Call ApplyVsTheme(ContextMenuStrip1, ToolStrip1, ContextMenuStrip2, ContextMenuStrip3)
    End Sub

    Private Sub Win7StyleTreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles Win7StyleTreeView1.AfterSelect
        Dim ticks As ChromatogramTick()

        If TypeOf e.Node.Tag Is DataReader.Chromatogram Then
            ticks = DirectCast(e.Node.Tag, DataReader.Chromatogram).GetTicks.ToArray
        Else
            Dim chr As chromatogram = e.Node.Tag
            ticks = chr.Ticks
            Dim proper As New MRMROIProperty(chr)

            Call VisualStudio.ShowProperties(proper)
        End If

        Call MyApplication.host.mzkitTool.ShowMRMTIC(e.Node.Text, ticks)
    End Sub

    Private Sub TICToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TICToolStripMenuItem.Click
        Call MyApplication.host.mzkitTool.TIC(GetFileTICOverlaps(False).ToArray, d3:=ShowTICOverlap3DToolStripMenuItem.Checked)
    End Sub

    Private Sub BPCToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BPCToolStripMenuItem.Click
        Call MyApplication.host.mzkitTool.TIC(GetFileTICOverlaps(True).ToArray, d3:=ShowTICOverlap3DToolStripMenuItem.Checked)
    End Sub

    ''' <summary>
    ''' get ions TIC
    ''' </summary>
    ''' <returns></returns>
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

    Private Iterator Function GetFileTICOverlaps(bpc As Boolean) As IEnumerable(Of NamedCollection(Of ChromatogramTick))
        For Each rawfile As TreeNode In Win7StyleTreeView1.Nodes
            Dim fileName As String = rawfile.Text.BaseName

            If Not rawfile.Checked Then
                Continue For
            End If

            With DirectCast(rawfile.Tag, DataReader.Chromatogram)
                Yield New NamedCollection(Of ChromatogramTick)(fileName, .GetTicks(isbpc:=bpc))
            End With
        Next
    End Function

    Private Sub ShowTICOverlap3DToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowTICOverlap3DToolStripMenuItem.Click
        ShowTICOverlap3DToolStripMenuItem.Checked = Not ShowTICOverlap3DToolStripMenuItem.Checked
    End Sub

    Private Sub ClearFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearFilesToolStripMenuItem.Click, ClearFilesToolStripMenuItem1.Click
        Call Win7StyleTreeView1.Nodes.Clear()
    End Sub

    ''' <summary>
    ''' ions
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ShowTICOverlapToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ShowTICOverlapToolStripMenuItem1.Click
        Call MyApplication.host.mzkitTool.TIC(GetIonTICOverlaps.ToArray)
    End Sub

    ''' <summary>
    ''' ions
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ShowTICOverlap3DToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ShowTICOverlap3DToolStripMenuItem1.Click
        Call MyApplication.host.mzkitTool.TIC(GetIonTICOverlaps.ToArray, d3:=True)
    End Sub

    Private Sub ClearIonSelectionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearIonSelectionsToolStripMenuItem.Click
        Dim file = Win7StyleTreeView1.SelectedNode

        If Not file Is Nothing Then
            For i As Integer = 0 To file.Nodes.Count - 1
                file.Nodes(i).Checked = False
            Next
        End If
    End Sub

    Private Sub SelectAllIonsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectAllIonsToolStripMenuItem.Click
        Dim file = Win7StyleTreeView1.SelectedNode

        If Not file Is Nothing Then
            For i As Integer = 0 To file.Nodes.Count - 1
                file.Nodes(i).Checked = True
            Next
        End If
    End Sub

    Private Sub SelectAllFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectAllFilesToolStripMenuItem.Click
        For Each node As TreeNode In Win7StyleTreeView1.Nodes
            node.Checked = True
        Next
    End Sub

    Private Sub ClearFileSelectionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearFileSelectionsToolStripMenuItem.Click
        For Each node As TreeNode In Win7StyleTreeView1.Nodes
            node.Checked = False
        Next
    End Sub
End Class
