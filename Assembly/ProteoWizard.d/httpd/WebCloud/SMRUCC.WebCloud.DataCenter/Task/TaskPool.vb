#Region "Microsoft.VisualBasic::5641da2bf2a93e183767d13c2cc2e7a7, ..\httpd\WebCloud\SMRUCC.HTTPInternal\Platform\Task\TaskPool.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Parallel.Tasks
Imports Microsoft.VisualBasic.SecurityString
Imports Oracle.LinuxCompatibility.MySQL
Imports err = SMRUCC.WebCloud.DataCenter.mysql.task_errors
Imports mysqlClient = Oracle.LinuxCompatibility.MySQL.MySQL

Namespace Platform

    ''' <summary>
    ''' 用户任务池
    ''' </summary>
    Public Class TaskPool : Implements IDisposable

        ''' <summary>
        ''' 处于排队状态的用户任务队列
        ''' </summary>
        Friend Class __internalQueue : Implements IEnumerable(Of Task)

            ''' <summary>
            ''' 处于排队状态的用户任务队列
            ''' </summary>
            ReadOnly _taskQueue As New Queue(Of Task)
            ''' <summary>
            ''' 这个字典表对象的作用主要是用于获取任务在队列之中的位置
            ''' </summary>
            ReadOnly _taskTable As New Dictionary(Of String, Task)

            ''' <summary>
            ''' 用户使用uid查询任务在队列之中的位置，返回-1表示不在任务队列之中
            ''' </summary>
            ''' <param name="uid$"></param>
            ''' <returns></returns>
            Public Function GetQueuePosition(uid$) As Integer
                If _taskTable.ContainsKey(uid) Then
                    Return _taskQueue.IndexOf(_taskTable(uid))
                Else
                    Return -1
                End If
            End Function

            ''' <summary>
            ''' 这个函数是线程安全的
            ''' </summary>
            ''' <returns></returns>
            Public Function Dequeue() As Task
                SyncLock _taskQueue
                    Dim task As Task = _taskQueue.Dequeue

                    SyncLock _taskTable
                        Call _taskTable.Remove(task.uid)
                    End SyncLock

                    Return task
                End SyncLock
            End Function

            ''' <summary>
            ''' 这个函数是线程安全的
            ''' </summary>
            ''' <param name="task"></param>
            Public Sub Enqueue(task As Task)
                SyncLock _taskQueue
                    Call _taskQueue.Enqueue(task)
                    SyncLock _taskTable
                        Call _taskTable.Add(task.uid, task)
                    End SyncLock
                End SyncLock
            End Sub

            Public Iterator Function GetEnumerator() As IEnumerator(Of Task) Implements IEnumerable(Of Task).GetEnumerator
                For Each task As Task In _taskTable.Values
                    Yield task
                Next
            End Function

            Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
                Yield GetEnumerator()
            End Function
        End Class

        ''' <summary>
        ''' 允许同时运行的任务的数量
        ''' </summary>
        ''' <returns></returns>
        Public Property NumThreads As Integer
        Public Property TimeInterval As Integer = 1000

        ReadOnly TaskPool As New List(Of AsyncHandle(Of Task))
        ReadOnly _runningTask As New List(Of Task)
        ReadOnly _hashProvider As New Md5HashProvider

        Friend ReadOnly _taskQueue As New __internalQueue
        Friend _mysql As mysqlClient

        ''' <summary>
        ''' Assign the task uid: <see cref="Task.uid"/>
        ''' </summary>
        ''' <param name="task"></param>
        ''' <param name="url$"></param>
        Public Sub Assign(ByRef task As Task, url$)
            Dim uid$ = _hashProvider.GetMd5Hash(Now.ToString & App.NextTempName & url)
            SyncLock task
                task.uid = uid
            End SyncLock
        End Sub

        ''' <summary>
        ''' Write the exception information into database
        ''' </summary>
        ''' <param name="ex"></param>
        Public Sub WriteFailure(ex As err)
            If Not _mysql Is Nothing Then
                SyncLock _mysql
                    Call _mysql.ExecInsert(ex)
                End SyncLock
            Else
                Dim err As New Exception(ex.exception) With {
                    .Source = ex.stack_trace
                }
                Call err.PrintException
                Call App.LogException(err)
            End If
        End Sub

        ''' <summary>
        ''' Set up mysql connection uri 
        ''' </summary>
        ''' <param name="mysql"></param>
        ''' <returns></returns>
        Public Function Connect(mysql As ConnectionUri) As Boolean
            _mysql = New mysqlClient
            Return (_mysql <= mysql) > -1
        End Function

        ''' <summary>
        ''' 当不存在的时候，说明正在运行，或者已经运行完毕了
        ''' </summary>
        ''' <param name="uid"></param>
        ''' <returns></returns>
        Public Function GetQueueTask(uid As String) As Task
            Dim LQuery As Task =
                LinqAPI.DefaultFirst(Of Task) <=
 _
                From x As Task
                In _taskQueue
                Where String.Equals(uid, x.uid, StringComparison.OrdinalIgnoreCase)
                Select x

            Return LQuery
        End Function

        ''' <summary>
        ''' 获取正在执行的用户任务
        ''' </summary>
        ''' <param name="uid$"></param>
        ''' <returns></returns>
        Public Function GetRunningTask(uid$) As Task
            Return LinqAPI.DefaultFirst(Of Task) <=
 _
                From x As Task
                In _runningTask
                Where uid.TextEquals(x.uid)
                Select x

        End Function

        ''' <summary>
        ''' 返回-1表示不在等待队列之中，但是不保证是处于运行状态，因为完成状态的任务也是返回-1值的
        ''' </summary>
        ''' <param name="uid$"></param>
        ''' <returns></returns>
        Public Function GetTaskQueuePosition(uid$) As Integer
            Return _taskQueue.GetQueuePosition(uid)
        End Function

        ''' <summary>
        ''' 任务还处在运行的状态？
        ''' </summary>
        ''' <param name="uid"></param>
        ''' <returns></returns>
        Public Function TaskRunning(uid As String) As Boolean
            Dim task As Task = GetRunningTask(uid)

            If task Is Nothing Then
                Return False
            Else
                Return Not task.Complete
            End If
        End Function

        Sub New()
            Call RunTask(AddressOf __taskInvoke)
        End Sub

        ''' <summary>
        ''' If this task pool object is already connect to the mysql throught <see cref="Connect"/> function, 
        ''' then this function is also commit an INSERT task into the mysql.
        ''' (将用户任务添加到任务执行队列之中，在这个函数之中已经包含有了任务写入数据库的语句了)
        ''' </summary>
        ''' <param name="task"></param>
        ''' <returns></returns>
        Public Function Queue(task As Task) As Integer
            Call _taskQueue.Enqueue(task)

            With task
                ._innerTaskPool = Me

                If Not _mysql Is Nothing AndAlso Not .TaskData Is Nothing Then
                    .TaskData.status = 0

                    Call _mysql.ExecInsert(.TaskData)
                End If
            End With

            Return _taskQueue.Count
        End Function

        ''' <summary>
        ''' View how much task in queue.
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Dim s As String = _taskQueue.Count & " tasks in queue..."
            Call s.__DEBUG_ECHO
            Return s
        End Function

        ''' <summary>
        ''' Internal run task in the queue.
        ''' </summary>
        Private Sub __taskInvoke()
            If NumThreads <= 0 Then
                NumThreads = LQuerySchedule.CPU_NUMBER * 2
            End If

            Do While Not disposedValue
                If TaskPool.Count < NumThreads AndAlso _taskQueue.Count > 0 Then  ' 向任务池里面添加新的并行任务
                    Dim task As Task = _taskQueue.Dequeue
                    Call TaskPool.Add(New AsyncHandle(Of Task)(AddressOf task.Start).Run)
                    Call _runningTask.Add(task)
                End If

                Dim LQuery = (From task As AsyncHandle(Of Task)
                              In TaskPool
                              Where task.IsCompleted ' 在这里获得完成的任务
                              Select task).ToArray
                For Each completeTask As AsyncHandle(Of Task) In LQuery
                    Dim task As Task = completeTask.GetValue
                    Call TaskPool.Remove(completeTask)  '  将完成的任务从任务池之中移除然后获取返回值
                    Call _runningTask.Remove(task)
                Next

                Call Threading.Thread.Sleep(TimeInterval)
            Loop

            Call (From task
                  In TaskPool.AsParallel  ' 等待剩余的计算任务完成计算过程
                  Let cli As Task = task.GetValue
                  Select cli).ToArray
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
