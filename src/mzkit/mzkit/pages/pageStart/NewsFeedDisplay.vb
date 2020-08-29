Imports Task

Public Class NewsFeedDisplay

    Dim news As NewsFeed

    Public Sub ShowNews(news As NewsFeed)
        Me.news = news

        LinkLabel1.Text = news.title
        Label1.Text = news.date
        TextBox1.Text = news.abstract
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process.Start(news.url)
    End Sub
End Class
