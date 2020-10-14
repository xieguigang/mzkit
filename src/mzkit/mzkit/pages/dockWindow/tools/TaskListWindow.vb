Imports System.ComponentModel

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
    Dim row As ListViewItem

    Sub New(task$, content$, list As TaskListWindow)
        row = New ListViewItem With {.Text = task}
        row.SubItems.Add(New ListViewItem.ListViewSubItem With {.Text = content})
        row.SubItems.Add(New ListViewItem.ListViewSubItem With {.Text = Now.ToString})
        row.SubItems.Add(New ListViewItem.ListViewSubItem With {.Text = "Pending", .BackColor = Color.Yellow})

        window.TreeListView1.Items.Add(row)
    End Sub
End Class