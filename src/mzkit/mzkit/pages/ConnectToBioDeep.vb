#Region "Microsoft.VisualBasic::12b4d01185c057629969d359bb0f1dd2, src\mzkit\mzkit\pages\ConnectToBioDeep.vb"

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

' Class ConnectToBioDeep
' 
'     Constructor: (+1 Overloads) Sub New
'     Sub: OpenAdvancedFunction, RunMetaDNA
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports BioNovoGene.BioDeep.MetaDNA
Imports BioNovoGene.BioDeep.MetaDNA.Infer
Imports Microsoft.VisualBasic.Imaging
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
            Dim taskList As TaskListWindow = WindowModules.taskWin
            Dim task As TaskUI = taskList.Add("MetaDNA Search", raw.source.GetFullPath)
            Dim log As OutputWindow = WindowModules.output
            Dim println As Action(Of String) =
                Sub(message)
                    Call task.ProgressMessage(message)
                    Call log.AppendMessage(message)
                End Sub
            Dim table = VisualStudio.ShowDocument(Of frmTableViewer)

            table.DockState = DockState.Hidden

            taskList.Show(MyApplication.host.dockPanel)
            VisualStudio.Dock(taskList, DockState.DockBottom)

            ' Call Alert.ShowSucess($"Imports raw data files in background,{vbCrLf}you can open [Task List] panel for view task progress.")
            Call MyApplication.TaskQueue.AddToQueue(
                Sub()
                    Dim result As MetaDNAResult() = Nothing
                    Dim infer As CandidateInfer() = Nothing

                    Call task.Running()
                    Call MetaDNASearch.RunDIA(raw, println, result, infer)
                    Call table.Invoke(Sub()
                                          table.DockState = DockState.Document
                                          table.Show(MyApplication.host.dockPanel)
                                          table.TabText = $"[MetaDNA] {raw.source.FileName}"
                                      End Sub)

                    Call println("output result table")

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
                                Call Application.DoEvents()
                            Next
                        End Sub)

                    Call table.Invoke(Sub()
                                          Dim inferIndex As Dictionary(Of String, Candidate) = infer _
                                              .ExportInferRaw(result) _
                                              .Inference _
                                              .ToDictionary(Function(a) $"{a.ROI}|{a.infer.kegg.kegg_id}|{a.precursorType}|{a.infer.reference.id}|{a.infer.rawFile}")

                                          table.ViewRow = Sub(obj)
                                                              Dim uidRef As String = $"{obj!ROI_id}|{obj!KEGGId}|{obj!precursorType}|{obj!seed}|{obj!fileName}"
                                                              Dim align As Candidate = inferIndex(uidRef)

                                                              If align.infer.level <> InferLevel.Ms1 Then
                                                                  Dim qvsref = align.infer.GetAlignmentMirror

                                                                  Call MyApplication.host.Invoke(
                                                                      Sub()
                                                                          Call MyApplication.host.mzkitTool.showAlignment(qvsref.query, qvsref.ref, align.infer)
                                                                      End Sub)
                                                              Else
                                                                  Call MyApplication.host.showStatusMessage($"MS1 level metaDNA infer did'nt have MS/MS alignment data...")
                                                              End If
                                                          End Sub
                                      End Sub)

                    Call println("MetaDNA search job done!")

                    Call MessageBox.Show($"MetaDNA search done!" & vbCrLf & $"Found {result.Length} DIA annotation hits.", "MetaDNA Search", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Sub)
        End If
    End Sub
End Class

