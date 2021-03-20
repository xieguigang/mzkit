Imports System.ComponentModel
Imports mzkit.My
Imports Vip.Notification

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

    ''' <summary>
    ''' 切换为执行中的状态
    ''' </summary>
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
        Dim main = MyApplication.host

        window.Invoke(Sub()
                          status.Text = "Finished"
                          progress.Text = ""
                          status.BackColor = Color.SkyBlue
                      End Sub)
        TaskListWindow.pending -= 1

        Call main.Invoke(Sub()
                             main.ToolStripProgressBar1.Value += 1
                             main.taskWin.UpdateProgress()

                             If main.ToolStripProgressBar1.Value = main.ToolStripProgressBar1.Maximum Then
                                 main.ToolStripStatusLabel4.Image = My.Resources._1200px_Checked_svg
                                 main.ToolStripStatusLabel4.Text = "Job Done!"

                                 main.ToolStripProgressBar1.Value = 0
                                 main.ToolStripProgressBar1.Maximum = 0
                             End If
                         End Sub)

        ' Call Alert.ShowSucess(message)
    End Sub
End Class
