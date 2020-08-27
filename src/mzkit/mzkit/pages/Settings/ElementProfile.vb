Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq

Public Class ElementProfile : Implements ISaveSettings, IPageSettings

    Public Sub LoadSettings() Implements ISaveSettings.LoadSettings
        DataGridView1.Rows.Clear()

        If Globals.Settings.formula_search Is Nothing Then
            Globals.Settings.formula_search = New FormulaSearchProfile With {.elements = New Dictionary(Of String, ElementRange)}
        End If

        For Each element In Globals.Settings.formula_search.elements.SafeQuery
            DataGridView1.Rows.Add({element.Key, element.Value.min, element.Value.max})
        Next

        ComboBox1.SelectedIndex = 0
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

    Private Sub loadPresetProfile(profile As SearchOption)
        DataGridView1.Rows.Clear()

        For Each element In profile.candidateElements
            DataGridView1.Rows.Add({element.Element, element.MinCount, element.MaxCount})
        Next
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Dim index As FormulaSearchProfiles = ComboBox1.SelectedIndex + 1

        Select Case index
            Case FormulaSearchProfiles.Default
                loadPresetProfile(SearchOption.DefaultMetaboliteProfile)
            Case FormulaSearchProfiles.GeneralFlavone
                loadPresetProfile(SearchOption.GeneralFlavone)
            Case FormulaSearchProfiles.NaturalProduct
                loadPresetProfile(SearchOption.NaturalProduct(DNPOrWileyType.DNP, True))
            Case FormulaSearchProfiles.SmallMolecule
                loadPresetProfile(SearchOption.SmallMolecule(DNPOrWileyType.DNP, True))
        End Select
    End Sub
End Class
