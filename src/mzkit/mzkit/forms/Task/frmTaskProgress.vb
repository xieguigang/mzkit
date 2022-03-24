#Region "Microsoft.VisualBasic::f352aacddb5d9ad2191c6c8ef7ce4a75, mzkit\src\mzkit\mzkit\forms\Task\frmTaskProgress.vb"

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

    '   Total Lines: 164
    '    Code Lines: 120
    ' Comment Lines: 9
    '   Blank Lines: 35
    '     File Size: 5.21 KB


    ' Class frmTaskProgress
    ' 
    '     Function: (+2 Overloads) LoadData
    ' 
    '     Sub: CloseWindow, frmImportTaskProgress_Paint, frmTaskProgress_Closed, frmTaskProgress_KeyDown, frmTaskProgress_Load
    '          RunAction, (+2 Overloads) SetProgress, SetProgressMode, ShowProgressDetails, ShowProgressTitle
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Threading
Imports BioNovoGene.mzkit_win32.My

Public Class frmTaskProgress

    Dim dialogClosed As Boolean = False

    Public TaskCancel As Action

    Public Sub SetProgressMode()
        ProgressBar1.Maximum = 100
        ProgressBar1.Style = ProgressBarStyle.Continuous
        TaskbarStatus.SetProgress(0)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="p">[0,100]</param>
    Public Sub SetProgress(p As Integer)
        Call Invoke(
            Sub()
                ProgressBar1.Value = p
                TaskbarStatus.SetProgress(p)
            End Sub)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="p">[0,100]</param>
    ''' <param name="message"></param>
    Public Sub SetProgress(p As Integer, message As String)
        Call Invoke(
            Sub()
                ProgressBar1.Value = p
                Label1.Text = message
                TaskbarStatus.SetProgress(p)
            End Sub)
    End Sub

    Private Sub frmImportTaskProgress_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        e.Graphics.DrawRectangle(New Pen(Color.Black, 1), New Rectangle(0, 0, Width - 1, Height - 1))
    End Sub

    Public Sub ShowProgressTitle(title As String, Optional directAccess As Boolean = False)
        If directAccess Then
            If TaskCancel Is Nothing Then
                Label2.Text = title
            Else
                Label2.Text = $"{title} [Press ESC for cancel task]"
            End If
        ElseIf Not dialogClosed Then
            Invoke(Sub()
                       If TaskCancel Is Nothing Then
                           Label2.Text = title
                       Else
                           Label2.Text = $"{title} [Press ESC for cancel task]"
                       End If
                   End Sub)
        End If
    End Sub

    Public Sub ShowProgressDetails(message As String, Optional directAccess As Boolean = False)
        If directAccess Then
            Label1.Text = message
        ElseIf Not dialogClosed Then
            Invoke(Sub() Label1.Text = message)
        End If

        Call MyApplication.host.showStatusMessage(message)
    End Sub

    Private Sub frmTaskProgress_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If TaskCancel Is Nothing Then
            Return
        End If

        SyncLock Label1
            SyncLock Label2
                If e.KeyCode = Keys.Escape Then
                    Label2.Text = "Task Cancel..."
                    dialogClosed = True
                    TaskCancel()
                End If
            End SyncLock
        End SyncLock
    End Sub

    Private Sub frmTaskProgress_Load(sender As Object, e As EventArgs) Handles Me.Load
        DoubleBuffered = True
        TaskbarStatus.SetLoopStatus()
    End Sub

    Public Shared Function LoadData(Of T)(streamLoad As Func(Of Action(Of String), T),
                                          Optional title$ = "Loading data...",
                                          Optional info$ = "Open a large raw data file...") As T
        Dim tmp As T
        Dim progress As New frmTaskProgress

        Call New Thread(
            Sub()
                Call Thread.Sleep(100)

                Call progress.ShowProgressTitle(title)
                Call progress.ShowProgressDetails(info)

                tmp = streamLoad(AddressOf progress.ShowProgressDetails)

                Call progress.CloseWindow()
            End Sub).Start()

        Call progress.ShowDialog()

        Return tmp
    End Function

    Public Shared Sub RunAction(run As Action(Of Action(Of String)), Optional title$ = "Loading data...", Optional info$ = "Open a large raw data file...")
        Dim progress As New frmTaskProgress

        Call New Thread(
            Sub()
                Call Thread.Sleep(100)

                Call progress.ShowProgressTitle(title)
                Call progress.ShowProgressDetails(info)

                Call run(AddressOf progress.ShowProgressDetails)

                Call progress.Invoke(Sub() progress.Close())
            End Sub).Start()

        Call progress.ShowDialog()
    End Sub

    Private Sub frmTaskProgress_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        TaskbarStatus.Stop()
    End Sub

    Public Sub CloseWindow()
        Call Me.Invoke(Sub() Call Me.Close())
    End Sub
End Class
