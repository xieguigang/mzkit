Imports Microsoft.Windows.Taskbar

Module TaskbarStatus

    ''' <summary>
    ''' Keep a reference to the Taskbar instance
    ''' </summary>
    Dim windowsTaskbar As TaskbarManager = TaskbarManager.Instance

    Public Sub SetLoopStatus()
        windowsTaskbar.SetProgressState(TaskbarProgressBarState.Indeterminate)
    End Sub

    Public Sub [Stop]()
        windowsTaskbar.SetProgressState(TaskbarProgressBarState.NoProgress)
    End Sub

    Public Sub SetProgress(percentage As Integer)
        windowsTaskbar.SetProgressState(TaskbarProgressBarState.Normal)
        windowsTaskbar.SetProgressValue(percentage, 100)
    End Sub

End Module
