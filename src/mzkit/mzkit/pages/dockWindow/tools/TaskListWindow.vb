Imports System.ComponentModel

Public Class TaskListWindow

    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        DoubleBuffered = True
    End Sub

    Private Sub TaskListWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.Hide()
    End Sub
End Class