#Region "Microsoft.VisualBasic::4ec6828d664a510fb5b58583437a8b8e, pages\dockWindow\tools\TaskListWindow.vb"

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

    ' Class TaskListWindow
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: Add
    ' 
    '     Sub: TaskListWindow_Closing
    ' 
    ' Class TaskUI
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: Finish, ProgressMessage, Running
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Vip.Notification

Public Class TaskListWindow

    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        DoubleBuffered = True
        TabText = "Task List"
    End Sub

    Private Sub TaskListWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.Hide()
    End Sub

    Public Function Add(task As String, content$) As TaskUI
        Return New TaskUI(task, content, Me)
    End Function
End Class

Public Class TaskUI

    Dim window As TaskListWindow
    Dim row As TreeListViewItem
    Dim status As ListViewItem.ListViewSubItem
    Dim progress As ListViewItem.ListViewSubItem

    Dim taskTitle, taskContent As String

    Sub New(task$, content$, list As TaskListWindow)
        row = New TreeListViewItem With {.Text = task, .ImageIndex = 0}
        status = New ListViewItem.ListViewSubItem With {.Text = "Pending", .BackColor = Color.Yellow}
        progress = New ListViewItem.ListViewSubItem With {.Text = "..."}

        row.SubItems.Add(New ListViewItem.ListViewSubItem With {.Text = content})
        row.SubItems.Add(New ListViewItem.ListViewSubItem With {.Text = Now.ToString})
        row.SubItems.Add(status)
        row.SubItems.Add(progress)

        window = list
        window.TreeListView1.Items.Add(row)

        taskTitle = task
        taskContent = content
    End Sub

    Public Sub Running()
        window.Invoke(Sub()
                          status.Text = "Running..."
                          status.BackColor = Color.Green
                      End Sub)
    End Sub

    Public Sub ProgressMessage(message As String)
        window.Invoke(Sub()
                          progress.Text = message
                      End Sub)
    End Sub

    Public Sub Finish()
        Dim message As String = $"{taskTitle} Job Done!{vbCrLf}{taskContent}"

        window.Invoke(Sub()
                          status.Text = "Finished"
                          progress.Text = ""
                          status.BackColor = Color.SkyBlue
                      End Sub)

        Call Alert.ShowSucess(message)
    End Sub
End Class
