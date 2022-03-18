#Region "Microsoft.VisualBasic::808f105b45784818f99eb09532e00558, mzkit\src\mzkit\mzkit\pages\dockWindow\frmVideoList.vb"

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

    '   Total Lines: 66
    '    Code Lines: 52
    ' Comment Lines: 0
    '   Blank Lines: 14
    '     File Size: 2.40 KB


    ' Class frmVideoList
    ' 
    '     Sub: BackgroundWorker_DoWork, frmVideoList_Load, loadVideoList
    ' 
    ' Class VideoContent
    ' 
    '     Properties: bvid, desc, duration, pic, pubdate
    '                 short_link, title
    ' 
    ' Class VideoList
    ' 
    '     Properties: count, list
    ' 
    ' Class MessageInfo
    ' 
    '     Properties: code, info
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioDeep
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.Serialization.JSON
Imports BioNovoGene.mzkit_win32.My

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

        If data Is Nothing Then
            Call MyApplication.host.showStatusMessage("Sorry, no network connectivity to education.biodeep.cn...", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        ElseIf data.info Is Nothing OrElse data.info.list.IsNullOrEmpty Then
            Call MyApplication.host.showStatusMessage("Remote server error, no video content can be found...", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

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
