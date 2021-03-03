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
Imports mzkit.My
Imports Vip.Notification

Public Class TaskListWindow

    Friend Shared n As Integer
    Friend Shared pending As Integer

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
        MyApplication.host.ToolStripStatusLabel4.Image = My.Resources.img_561134
        MyApplication.host.ToolStripStatusLabel4.Text = $"Running Background Task {pending}/{n}"
        MyApplication.host.ToolStripProgressBar1.Maximum += 1

        n += 1
        pending += 1
        Return New TaskUI(task, content, Me)
    End Function
End Class
