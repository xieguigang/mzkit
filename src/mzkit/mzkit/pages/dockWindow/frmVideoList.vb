Imports System.ComponentModel
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Task

Public Class frmVideoList

    Dim WithEvents BackgroundWorker As New BackgroundWorker

    Private Sub frmVideoList_Load(sender As Object, e As EventArgs) Handles Me.Load
        BackgroundWorker.RunWorkerAsync()

        CopyFullPathToolStripMenuItem.Enabled = False
        OpenContainingFolderToolStripMenuItem.Enabled = False
        SaveDocumentToolStripMenuItem.Enabled = False
    End Sub

    Const source As String = "http://education.biodeep.cn/api/getMzkit"

    Private Sub loadVideoList()
        Dim data As MessageInfo = source.GET.LoadJSON(Of MessageInfo)

        For Each video In data.info.list
            Dim card As New VideoCards With {.url = video.short_link}

            card.PictureBox1.BackgroundImage = Image.FromStream(SingletonHolder(Of BioDeepSession).Instance.RequestStream(video.pic))
            card.LinkLabel1.Text = video.title
            card.Label2.Text = video.desc

            FlowLayoutPanel1.Controls.Add(card)
        Next
    End Sub

    Private Sub BackgroundWorker_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker.DoWork
        Call Invoke(Sub() loadVideoList())
    End Sub
End Class

Public Class VideoContent
    Public Property bvid As String
    Public Property desc As String
    Public Property pic As String
    Public Property short_link As String
    Public Property title As String
    Public Property pubdate As Long
    Public Property duration As Integer
End Class

Public Class VideoList
    Public Property count As Integer
    Public Property list As VideoContent()
End Class

Public Class MessageInfo
    Public Property code As Integer
    Public Property info As VideoList
End Class