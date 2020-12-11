#Region "Microsoft.VisualBasic::516b295df58c9d2a7499761f59e0462d, src\mzkit\mzkit\pages\pageStart\PageStart.vb"

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

    ' Class PageStart
    ' 
    '     Sub: BackgroundWorker_DoWork, LinkLabel1_LinkClicked, LinkLabel2_LinkClicked, LinkLabel3_LinkClicked, LinkLabel4_LinkClicked
    '          PageStart_Load, PageStart_Resize
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports mzkit.My
Imports Task
Imports WeifenLuo.WinFormsUI.Docking

Public Class PageStart

    Dim WithEvents BackgroundWorker As New BackgroundWorker

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        MyApplication.host.fileExplorer.DockState = DockState.DockLeft
        MyApplication.host.rawFeaturesList.DockState = DockState.DockLeft

        If MyApplication.fileExplorer.treeView1.SelectedNode Is Nothing Then
            If MyApplication.fileExplorer.treeView1.Nodes(0).Nodes.Count = 0 Then
                ' imports raw
                Call MyApplication.host.OpenFile()
            End If
            If MyApplication.fileExplorer.treeView1.Nodes(0).Nodes.Count = 0 Then
                ' user cancel imports raw data files
                Return
            End If
            Dim firstFile = MyApplication.fileExplorer.treeView1.Nodes(0).Nodes(0)

            MyApplication.fileExplorer.treeView1.SelectedNode = firstFile
            MyApplication.fileExplorer.showRawFile(DirectCast(firstFile.Tag, Raw))
        End If

        MyApplication.host.ShowMzkitToolkit()
    End Sub

    Private Sub PageStart_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        BackgroundWorker.RunWorkerAsync()
    End Sub

    Private Sub BackgroundWorker_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker.DoWork
        Dim news As NewsFeed() = NewsFeed.ParseLatest().ToArray

        Invoke(Sub()
                   For Each newsItem As NewsFeed In news
                       Dim display As New NewsFeedDisplay
                       FlowLayoutPanel1.Controls.Add(display)
                       display.ShowNews(newsItem)
                   Next
               End Sub)
    End Sub

    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        Process.Start("http://www.bionovogene.com/news/newsFeed.htm")
    End Sub

    Private Sub LinkLabel4_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel4.LinkClicked
        Process.Start("http://www.biodeep.cn/")
    End Sub

    Private Sub PageStart_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If Width < 955 Then
            LinkLabel2.Visible = False
            FlowLayoutPanel1.Visible = False
        Else
            LinkLabel2.Visible = True
            FlowLayoutPanel1.Visible = True
        End If
    End Sub

    Private Sub LinkLabel3_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
        ' 打开R终端页面
        MyApplication.host.CreateNewScript(Nothing, Nothing)
    End Sub

    Private Sub LinkLabel5_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
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
        MyApplication.fileExplorer.showRawFile(DirectCast(findRaw.Tag, Raw))
        MyApplication.host.ShowMzkitToolkit()
    End Sub
End Class

