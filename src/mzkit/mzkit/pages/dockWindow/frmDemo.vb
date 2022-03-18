#Region "Microsoft.VisualBasic::5e9dad0ef8a0a2f6659df1a7d5db33a7, mzkit\src\mzkit\mzkit\pages\dockWindow\frmDemo.vb"

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

    '   Total Lines: 223
    '    Code Lines: 157
    ' Comment Lines: 8
    '   Blank Lines: 58
    '     File Size: 8.50 KB


    ' Class frmDemo
    ' 
    '     Sub: frmDemo_Closing, frmDemo_Load, ListView1_DoubleClick, ListView1_SelectedIndexChanged, OpenContainingFolder
    '          ShowInExplorerToolStripMenuItem_Click, ShowPage
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.mzkit_win32.My
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

            Case 4

                ' MSI demo2
                info.Title = "HR2MSI mouse urinary bladder S096 - Figure1"
                info.Url = $"{App.HOME}/demo/HR2MSI mouse urinary bladder S096 - Figure1.cdf".GetFullPath
                info.Information = "Red: 743.5482(unknown);
Green: 798.541(PC(34:1) [M+K]+)
Blue: 741.5307(SM(34:1) [M+K]+)
"
                info.Application = "Mzkit MSI viewer"

            Case 5

                ' UV scans
                info.Title = "mzPack DEMO file with UV scans"
                info.Url = $"{App.HOME}/demo/DEMO.mzPack".GetFullPath
                info.Information = "UV scans demo data"
                info.Application = "Mzkit Raw Data Viewer"

            Case 6

                info.Title = "S043_Processed_imzML1.1.1.mzPack"
                info.Url = $"{App.HOME}/demo/S043_Processed_imzML1.1.1.mzPack".GetFullPath
                info.Information = "S043_Processed"
                info.Application = "Mzkit MSI viewer"

        End Select

        PropertyGrid1.SelectedObject = info
        PropertyGrid1.Refresh()
    End Sub

    Private Sub ListView1_DoubleClick() Handles ListView1.DoubleClick, OpenToolStripMenuItem.Click
        If ListView1.SelectedItems.Count = 0 Then
            Return
        End If

        Dim i As Integer = ListView1.SelectedIndices.Item(0)
        Dim fileExplorer = WindowModules.fileExplorer

        Select Case i
            Case 0

                Call Process.Start("https://ms-imaging.org/wp/imzml/example-files-test/")

            Case 1

                Dim findRaw = fileExplorer.findRawFileNode("003_Ex2_Orbitrap_CID.mzXML")
                Dim demoPath As String = $"{App.HOME}/demo/003_Ex2_Orbitrap_CID.mzXML"

                If findRaw Is Nothing Then
                    If Not demoPath.FileExists Then
                        MyApplication.host.showStatusMessage("the demo data file is missing!", My.Resources.StatusAnnotations_Warning_32xLG_color)
                        Return
                    End If
                    fileExplorer.addFileNode(frmFileExplorer.getRawCache(demoPath))
                    findRaw = fileExplorer.findRawFileNode("003_Ex2_Orbitrap_CID.mzXML")
                End If

                fileExplorer.treeView1.SelectedNode = findRaw
                fileExplorer.showRawFile(DirectCast(findRaw.Tag, Raw), False, directSnapshot:=True, contour:=False)
                MyApplication.host.ShowMzkitToolkit()

            Case 2


                ' LC-MSMS
                Dim demoPath As String = $"{App.HOME}/demo/MRM-Data20190222-QCH.mzML"
                MyApplication.host.ShowMRMIons(demoPath)

            Case 3

                ' GC-MS
                Dim demoPath As String = $"{App.HOME}/demo/5ppm.CDF"
                MyApplication.host.ShowGCMSSIM(demoPath, isBackground:=False, showExplorer:=True)

            Case 4

                Dim demopath As String = $"{App.HOME}/demo/HR2MSI mouse urinary bladder S096 - Figure1.cdf".GetFullPath

                If Not demopath.FileExists Then
                    MyApplication.host.showStatusMessage("the demo data file is missing!", My.Resources.StatusAnnotations_Warning_32xLG_color)
                Else
                    Call RibbonEvents.showMsImaging()
                    Call WindowModules.msImageParameters.loadRenderFromCDF(demopath)
                End If


            Case 5

                ' UV scans
                Dim demopath As String = $"{App.HOME}/demo/DEMO.mzPack".GetFullPath
                Dim cache As String
                Dim findRaw = fileExplorer.findRawFileNode("DEMO.mzPack")

                If findRaw Is Nothing Then
                    If Not demopath.FileExists Then
                        MyApplication.host.showStatusMessage("the demo data file is missing!", My.Resources.StatusAnnotations_Warning_32xLG_color)
                        Return
                    End If
                    cache = ImportsRawData.GetCachePath(demopath)
                    demopath.FileCopy(cache)
                    fileExplorer.addFileNode(New Raw With {
                        .cache = cache.GetFullPath,
                        .source = demopath
                    })
                    findRaw = fileExplorer.findRawFileNode("DEMO.mzPack")
                End If

                fileExplorer.treeView1.SelectedNode = findRaw
                fileExplorer.showRawFile(DirectCast(findRaw.Tag, Raw), False, directSnapshot:=True, contour:=False)
                MyApplication.host.ShowMzkitToolkit()

            Case 6

                Dim demopath As String = $"{App.HOME}/demo/S043_Processed_imzML1.1.1.mzPack".GetFullPath

                If Not demopath.FileExists Then
                    MyApplication.host.showStatusMessage("the demo data file is missing!", My.Resources.StatusAnnotations_Warning_32xLG_color)
                Else
                    Call RibbonEvents.showMsImaging()
                    Call MyApplication.host.showMzPackMSI(demopath)
                End If

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
