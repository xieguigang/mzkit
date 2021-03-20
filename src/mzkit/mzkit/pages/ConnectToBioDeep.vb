Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.BioDeep.MetaDNA
Imports Microsoft.VisualBasic.My
Imports mzkit.DockSample
Imports mzkit.My
Imports Task

Public Class ConnectToBioDeep

    Private Sub New()
    End Sub

    Public Shared Sub OpenAdvancedFunction(action As Action)
        If Not SingletonHolder(Of BioDeepSession).Instance.CheckSession Then
            Call New frmLogin().ShowDialog()
        End If

        If SingletonHolder(Of BioDeepSession).Instance.CheckSession Then
            Call action()
        End If
    End Sub

    Public Shared Sub RunMetaDNA(raw As Raw)
        If Not SingletonHolder(Of BioDeepSession).Instance.CheckSession Then
            Call New frmLogin().ShowDialog()
        End If

        If SingletonHolder(Of BioDeepSession).Instance.CheckSession Then
            ' work in background
            Dim taskList As TaskListWindow = MyApplication.host.taskWin
            Dim task As TaskUI = taskList.Add("MetaDNA Search", raw.source.GetFullPath)
            Dim log As OutputWindow = MyApplication.host.output
            Dim println As Action(Of String) =
                Sub(message)
                    Call task.ProgressMessage(message)
                    Call log.AppendMessage(message)
                End Sub

            Call taskList.Show(MyApplication.host.dockPanel)
            ' Call Alert.ShowSucess($"Imports raw data files in background,{vbCrLf}you can open [Task List] panel for view task progress.")
            Call MyApplication.TaskQueue.AddToQueue(
                Sub()
                    Dim result As MetaDNAResult() = Nothing

                    Call task.Running()
                    Call MetaDNASearch.RunDIA(raw, println, result)
                End Sub)
        End If
    End Sub
End Class
