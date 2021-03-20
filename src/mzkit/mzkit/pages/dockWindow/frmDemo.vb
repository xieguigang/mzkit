#Region "Microsoft.VisualBasic::c53013b12eb26943429182b98a86c654, pages\dockWindow\frmDemo.vb"

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

    ' Class frmDemo
    ' 
    '     Sub: frmDemo_Closing, frmDemo_Load, ListView1_DoubleClick, ListView1_SelectedIndexChanged, OpenContainingFolder
    '          ShowInExplorerToolStripMenuItem_Click, ShowPage
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports mzkit.My
Imports Task
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmDemo

    Private Sub frmDemo_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Icon = My.Resources.chemistry

        Me.ShowIcon = True
        Me.TabText = "MS Demo Data"
        ' Me.ListView1.View = View.Tile

        SaveDocumentToolStripMenuItem.Enabled = False
        CopyFullPathToolStripMenuItem.Enabled = False
    End Sub

    Protected Overrides Sub OpenContainingFolder()
        Call Process.Start($"{App.HOME}/demo")
    End Sub

    Public Sub ShowPage()
        Me.Show(MyApplication.host.dockPanel)
        DockState = DockState.Document
    End Sub

    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged
        If ListView1.SelectedItems.Count = 0 Then
            Return
        End If

        Dim i As Integer = ListView1.SelectedIndices.Item(0)
        Dim info As New DemoItem

        Select Case i
            Case 0

                info.Title = "MS-Imaging demo data"
                info.Url = "https://ms-imaging.org/wp/imzml/example-files-test/"
                info.Information = "imzML dataset available as ‘open data’ in public data repository PRIDE by the European Bioinformatics Institute"
                info.Application = "Mzkit MS-Imaging Viewer"

            Case 1

                info.Title = "LC-MS raw data file demo"
                info.Url = $"{App.HOME}/demo/003_Ex2_Orbitrap_CID.mzXML".GetFullPath
                info.Information = ""
                info.Application = "Mzkit Raw Data Viewer"

            Case 2


                ' LC-MSMS
                info.Title = "LC-MS/MS targeted data demo"
                info.Url = $"{App.HOME}/demo/MRM-Data20190222-QCH.mzML".GetFullPath
                info.Information = ""
                info.Application = "Mzkit LC-MS/MS Viewer"

            Case 3

                ' GC-MS
                info.Title = "GC-MS targeted data demo"
                info.Url = $"{App.HOME}/demo/5ppm.CDF".GetFullPath
                info.Information = ""
                info.Application = "Mzkit GC-MS Targeted Viewer"

        End Select

        PropertyGrid1.SelectedObject = info
        PropertyGrid1.Refresh()
    End Sub

    Private Sub ListView1_DoubleClick() Handles ListView1.DoubleClick, OpenToolStripMenuItem.Click
        If ListView1.SelectedItems.Count = 0 Then
            Return
        End If

        Dim i As Integer = ListView1.SelectedIndices.Item(0)

        Select Case i
            Case 0

                Call Process.Start("https://ms-imaging.org/wp/imzml/example-files-test/")

            Case 1

                Dim findRaw = MyApplication.fileExplorer.findRawFileNode("003_Ex2_Orbitrap_CID.mzXML")
                Dim demoPath As String = $"{App.HOME}/demo/003_Ex2_Orbitrap_CID.mzXML"

                If findRaw Is Nothing Then
                    If Not demoPath.FileExists Then
                        MyApplication.host.showStatusMessage("the demo data file is missing!", My.Resources.StatusAnnotations_Warning_32xLG_color)
                        Return
                    End If
                    MyApplication.fileExplorer.addFileNode(MyApplication.fileExplorer.getRawCache(demoPath))
                    findRaw = MyApplication.fileExplorer.findRawFileNode("003_Ex2_Orbitrap_CID.mzXML")
                End If

                MyApplication.fileExplorer.treeView1.SelectedNode = findRaw
                MyApplication.fileExplorer.showRawFile(DirectCast(findRaw.Tag, Raw), False, directSnapshot:=True)
                MyApplication.host.ShowMzkitToolkit()

            Case 2


                ' LC-MSMS
                Dim demoPath As String = $"{App.HOME}/demo/MRM-Data20190222-QCH.mzML"
                MyApplication.host.ShowMRMIons(demoPath)

            Case 3

                ' GC-MS
                Dim demoPath As String = $"{App.HOME}/demo/5ppm.CDF"
                MyApplication.host.ShowGCMSSIM(demoPath, isBackground:=False, showExplorer:=True)

        End Select
    End Sub

    Private Sub ShowInExplorerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowInExplorerToolStripMenuItem.Click
        If ListView1.SelectedItems.Count = 0 Then
            Return
        End If

        Dim i As Integer = ListView1.SelectedIndices.Item(0)

        Select Case i
            Case 0

                Call Process.Start("https://ms-imaging.org/wp/imzml/example-files-test/")

            Case Else

                Dim demoPath As String = $"{App.HOME}/demo/"

                If demoPath.DirectoryExists Then
                    Call Process.Start(demoPath)
                Else
                    Call MyApplication.host.showStatusMessage("missing demo data directory...", My.Resources.StatusAnnotations_Warning_32xLG_color)
                End If
        End Select
    End Sub

    Private Sub frmDemo_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.DockState = DockState.Hidden
    End Sub
End Class
