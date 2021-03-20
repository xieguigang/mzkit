Imports BioNovoGene.BioDeep.MetaDNA
Imports Microsoft.VisualBasic.My
Imports mzkit.DockSample
Imports mzkit.My
Imports Task
Imports WeifenLuo.WinFormsUI.Docking

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
            Dim table = VisualStudio.ShowDocument(Of frmTableViewer)

            table.DockState = DockState.Hidden

            Call taskList.Show(MyApplication.host.dockPanel)
            ' Call Alert.ShowSucess($"Imports raw data files in background,{vbCrLf}you can open [Task List] panel for view task progress.")
            Call MyApplication.TaskQueue.AddToQueue(
                Sub()
                    Dim result As MetaDNAResult() = Nothing

                    Call task.Running()
                    Call MetaDNASearch.RunDIA(raw, println, result)
                    Call table.Invoke(Sub() table.DockState = DockState.Document)

                    Call table.Invoke(
                        Sub()
                            Dim grid = table.DataGridView1

                            grid.Columns.Add(NameOf(MetaDNAResult.ROI_id), NameOf(MetaDNAResult.ROI_id))
                            grid.Columns.Add(NameOf(MetaDNAResult.query_id), NameOf(MetaDNAResult.query_id))
                            grid.Columns.Add(NameOf(MetaDNAResult.mz), NameOf(MetaDNAResult.mz))
                            grid.Columns.Add(NameOf(MetaDNAResult.rt), NameOf(MetaDNAResult.rt))
                            grid.Columns.Add(NameOf(MetaDNAResult.intensity), NameOf(MetaDNAResult.intensity))
                            grid.Columns.Add(NameOf(MetaDNAResult.KEGGId), NameOf(MetaDNAResult.KEGGId))
                            grid.Columns.Add(NameOf(MetaDNAResult.exactMass), NameOf(MetaDNAResult.exactMass))
                            grid.Columns.Add(NameOf(MetaDNAResult.formula), NameOf(MetaDNAResult.formula))
                            grid.Columns.Add(NameOf(MetaDNAResult.name), NameOf(MetaDNAResult.name))
                            grid.Columns.Add(NameOf(MetaDNAResult.precursorType), NameOf(MetaDNAResult.precursorType))
                            grid.Columns.Add(NameOf(MetaDNAResult.mzCalc), NameOf(MetaDNAResult.mzCalc))
                            grid.Columns.Add(NameOf(MetaDNAResult.ppm), NameOf(MetaDNAResult.ppm))
                            grid.Columns.Add(NameOf(MetaDNAResult.inferLevel), NameOf(MetaDNAResult.inferLevel))
                            grid.Columns.Add(NameOf(MetaDNAResult.forward), NameOf(MetaDNAResult.forward))
                            grid.Columns.Add(NameOf(MetaDNAResult.reverse), NameOf(MetaDNAResult.reverse))
                            grid.Columns.Add(NameOf(MetaDNAResult.jaccard), NameOf(MetaDNAResult.jaccard))
                            grid.Columns.Add(NameOf(MetaDNAResult.parentTrace), NameOf(MetaDNAResult.parentTrace))
                            grid.Columns.Add(NameOf(MetaDNAResult.inferSize), NameOf(MetaDNAResult.inferSize))
                            grid.Columns.Add(NameOf(MetaDNAResult.score1), NameOf(MetaDNAResult.score1))
                            grid.Columns.Add(NameOf(MetaDNAResult.score2), NameOf(MetaDNAResult.score2))
                            grid.Columns.Add(NameOf(MetaDNAResult.pvalue), NameOf(MetaDNAResult.pvalue))
                            grid.Columns.Add(NameOf(MetaDNAResult.seed), NameOf(MetaDNAResult.seed))
                            grid.Columns.Add(NameOf(MetaDNAResult.partnerKEGGId), NameOf(MetaDNAResult.partnerKEGGId))
                            grid.Columns.Add(NameOf(MetaDNAResult.KEGG_reaction), NameOf(MetaDNAResult.KEGG_reaction))
                            grid.Columns.Add(NameOf(MetaDNAResult.reaction), NameOf(MetaDNAResult.reaction))
                            grid.Columns.Add(NameOf(MetaDNAResult.fileName), NameOf(MetaDNAResult.fileName))

                            For Each line As MetaDNAResult In result
                                Call grid.Rows.Add(line.ROI_id, line.query_id, line.mz, line.rt, line.intensity, line.KEGGId, line.exactMass, line.formula, line.name, line.precursorType, line.mzCalc, line.ppm, line.inferLevel, line.forward, line.reverse, line.jaccard, line.parentTrace, line.inferSize, line.score1, line.score2, line.pvalue, line.seed, line.partnerKEGGId, line.KEGG_reaction, line.reaction, line.fileName)
                            Next
                        End Sub)
                End Sub)
        End If
    End Sub
End Class
