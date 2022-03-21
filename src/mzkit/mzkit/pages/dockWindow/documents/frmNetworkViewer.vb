#Region "Microsoft.VisualBasic::500cd72dfefa84c707f0979df79b3f06, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmNetworkViewer.vb"

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

    '   Total Lines: 82
    '    Code Lines: 63
    ' Comment Lines: 2
    '   Blank Lines: 17
    '     File Size: 3.36 KB


    ' Class frmNetworkViewer
    ' 
    '     Sub: Canvas1_DoubleClick, ConfigLayoutToolStripMenuItem_Click, ContextMenuStrip1_Opening, CopyNetworkVisualizeToolStripMenuItem_Click, frmNetworkViewer_Load
    '          PhysicalEngineToolStripMenuItem_Click, PinToolStripMenuItem_Click, SetGraph, ShowLabelsToolStripMenuItem_Click, SnapshotToolStripMenuItem_Click
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.SpringForce
Imports Microsoft.VisualBasic.Imaging

Public Class frmNetworkViewer

    Private Sub frmNetworkViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Text = "Network Canvas"
        TabText = "Network Canvas"

        Call ApplyVsTheme(ContextMenuStrip1)
    End Sub

    Public Sub SetGraph(g As NetworkGraph, layout As ForceDirectedArgs)
        If layout Is Nothing Then
            layout = ForceDirectedArgs.DefaultNew
        End If

        Canvas1.Graph() = g
        Canvas1.SetFDGParams(layout)
    End Sub

    Private Sub PhysicalEngineToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PhysicalEngineToolStripMenuItem.Click
        If PhysicalEngineToolStripMenuItem.Checked Then
            ' turn on engine
            PhysicalEngineToolStripMenuItem.Text = "Physical Engine (On)"
        Else
            ' turn off engine
            PhysicalEngineToolStripMenuItem.Text = "Physical Engine (Off)"
        End If

        Canvas1.SetPhysical(PhysicalEngineToolStripMenuItem.Checked)
    End Sub

    Private Sub ConfigLayoutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConfigLayoutToolStripMenuItem.Click
        Call InputDialog.Input(Of InputNetworkLayout)(
            Sub(config)
                Canvas1.SetFDGParams(Globals.Settings.network.layout)
            End Sub)
    End Sub

    Private Sub ShowLabelsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowLabelsToolStripMenuItem.Click
        Canvas1.ShowLabel = ShowLabelsToolStripMenuItem.Checked
    End Sub

    Private Sub SnapshotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SnapshotToolStripMenuItem.Click
        Using file As New SaveFileDialog With {.Filter = "Image File(*.bmp)|*.bmp"}
            If file.ShowDialog = DialogResult.OK Then
                Call Canvas1.GetSnapshot.SaveAs(file.FileName)
            End If
        End Using
    End Sub

    Private Sub CopyNetworkVisualizeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyNetworkVisualizeToolStripMenuItem.Click
        Clipboard.SetImage(Canvas1.GetSnapshot)
    End Sub

    Private Sub Canvas1_DoubleClick(sender As Object, e As EventArgs) Handles Canvas1.DoubleClick
        Dim tempfile As String = TempFileSystem.GetAppSysTempFile(ext:=".bmp")

        Call Canvas1.GetSnapshot.SaveAs(tempfile)
        Call Process.Start(tempfile)
    End Sub

    Private Sub PinToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PinToolStripMenuItem.Click
        Dim target As Node = Canvas1.GetTargetNode(PointToClient(Cursor.Position))

        If Not target Is Nothing Then
            target.pinned = PinToolStripMenuItem.Checked
        End If
    End Sub

    Private Sub ContextMenuStrip1_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuStrip1.Opening
        Dim target As Node = Canvas1.GetTargetNode(PointToClient(Cursor.Position))

        If Not target Is Nothing Then
            PinToolStripMenuItem.Checked = target.pinned
        End If
    End Sub
End Class
