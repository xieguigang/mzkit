Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

Module DataControlHandler

    <Extension>
    Public Sub SaveDataGrid(table As DataGridView)
        Using file As New SaveFileDialog With {.Filter = "Excel Table(*.xls)|*.xls"}
            If file.ShowDialog = DialogResult.OK Then
                Using writeTsv As StreamWriter = file.FileName.OpenWriter
                    Dim row As New List(Of String)

                    For i As Integer = 0 To table.Columns.Count - 1
                        row.Add(table.Columns(i).HeaderText)
                    Next

                    writeTsv.WriteLine(row.PopAll.JoinBy(","))

                    For j As Integer = 0 To table.Rows.Count - 1
                        Dim rowObj = table.Rows(j)

                        For i As Integer = 0 To rowObj.Cells.Count - 1
                            row.Add(Microsoft.VisualBasic.Scripting.ToString(rowObj.Cells(i).Value))
                        Next

                        writeTsv.WriteLine(row.PopAll.Select(Function(s) $"""{s}""").JoinBy(","))
                    Next
                End Using

                MessageBox.Show($"Exact Mass Search Result table export to [{file.FileName}] successfully!", "Export Table", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End Using
    End Sub
End Module
