Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq

Public Class ElementProfile : Implements ISaveSettings, IPageSettings

    Public Sub LoadSettings() Implements ISaveSettings.LoadSettings
        DataGridView1.Rows.Clear()

        If Globals.Settings.formula_search Is Nothing Then
            Globals.Settings.formula_search = New FormulaSearchProfile With {.elements = New Dictionary(Of String, ElementRange)}
        End If

        For Each element In Globals.Settings.formula_search.elements.SafeQuery
            DataGridView1.Rows.Add({element.Key, element.Value.Min, element.Value.Max})
        Next
    End Sub

    Public Sub SaveSettings() Implements ISaveSettings.SaveSettings
        Globals.Settings.formula_search.elements = New Dictionary(Of String, ElementRange)

        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            Dim elementProfile = DataGridView1.Rows(i)
            Dim atomName As String = Scripting.ToString(elementProfile.Cells(0).Value)

            If atomName.StringEmpty Then
                Exit For
            End If

            Globals.Settings.formula_search.elements.Add(atomName, New ElementRange With {.min = elementProfile.Cells(1).Value, .max = elementProfile.Cells(2).Value})
        Next

        Globals.Settings.Save()
    End Sub

    Public Sub ShowPage() Implements IPageSettings.ShowPage
        Call DirectCast(ParentForm, frmMain).ShowPage(DirectCast(ParentForm, frmMain).mzkitSearch)
    End Sub
End Class
