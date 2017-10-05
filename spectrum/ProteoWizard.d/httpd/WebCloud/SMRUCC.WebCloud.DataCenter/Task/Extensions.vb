Imports System.Runtime.CompilerServices
Imports SMRUCC.WebCloud.DataCenter.mysql
Imports mysqliEnd = Oracle.LinuxCompatibility.MySQL.MySQL

Namespace Platform

    Public Module Extensions

        ''' <summary>
        ''' 服务器重新开机之后尝试恢复执行未完成的任务流程
        ''' </summary>
        ''' <param name="mysql_data"></param>
        ''' <param name="app"><see cref="Task"/>模板的具体的实现定义</param>
        ''' <returns></returns>
        <Extension> Public Function Restore(mysql_data As mysql.task_pool, app As Type) As Task

        End Function

        Const mysqli_taskExpiredTime$ = "SELECT * FROM `smrucc-cloud`.sys_config WHERE `variable` = 'retention_time' LIMIT 1;"
        Const mysqli_expiredTasks$ = "SELECT * FROM `smrucc-cloud`.task_pool WHERE `status` <> 0 AND task_expired(time_complete, '{0}');"

        ''' <summary>
        ''' 对已经过期的任务进行工作区的清理工作，以减少服务器的硬盘空间的占用
        ''' </summary>
        ''' <param name="mysqli"></param>
        <Extension> Public Sub Cleanup(mysqli As mysqliEnd)
            Dim retention_time As Integer = Scripting.CTypeDynamic(Of Single)(mysqli.ExecuteScalar(Of sys_config)(mysqli_taskExpiredTime)?.value, 24.0!)
            Dim SQL$ = String.Format(mysqli_expiredTasks, retention_time)
            Dim tasks = mysqli.Query(Of task_pool)(SQL)

            For Each task As task_pool In tasks
                If task.workspace.DirectoryExists Then
                    Call FileSystem.RmDir(task.workspace)
                End If
            Next
        End Sub
    End Module
End Namespace