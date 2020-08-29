Imports System.ComponentModel
Imports Task
Imports WeifenLuo.WinFormsUI.Docking

Public Class PageStart

    Dim WithEvents BackgroundWorker As New BackgroundWorker

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        DirectCast(ParentForm, frmMain).fileExplorer.DockState = DockState.DockLeft
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
End Class
