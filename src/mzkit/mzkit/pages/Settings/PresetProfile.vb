Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports mzkit.My

Public Class PresetProfile : Implements ISaveSettings, IPageSettings

    Public Sub LoadSettings() Implements ISaveSettings.LoadSettings
        Dim profile = Globals.Settings.formula_search

        If profile Is Nothing Then
            Globals.Settings.formula_search = New FormulaSearchProfile
            profile = Globals.Settings.formula_search
        End If

        If profile.smallMoleculeProfile Is Nothing Then
            profile.smallMoleculeProfile = New PresetProfileSettings With {.isCommon = True, .type = DNPOrWileyType.Wiley}
        End If
        If profile.naturalProductProfile Is Nothing Then
            profile.naturalProductProfile = New PresetProfileSettings With {.isCommon = True, .type = DNPOrWileyType.Wiley}
        End If

        ComboBox1.SelectedIndex = profile.smallMoleculeProfile.type
        CheckBox1.Checked = profile.smallMoleculeProfile.isCommon

        ComboBox2.SelectedIndex = profile.naturalProductProfile.type
        CheckBox2.Checked = profile.naturalProductProfile.isCommon

        Dim precursorInfo = Globals.Settings.precursor_search

        If precursorInfo Is Nothing Then
            Globals.Settings.precursor_search = New PrecursorSearchSettings
        End If

        NumericUpDown1.Value = precursorInfo.ppm
        TextBox1.Text = precursorInfo.precursor_types.JoinBy(vbCrLf)
    End Sub

    Public Sub SaveSettings() Implements ISaveSettings.SaveSettings
        Globals.Settings.formula_search.smallMoleculeProfile = New PresetProfileSettings With {.type = ComboBox1.SelectedIndex, .isCommon = CheckBox1.Checked}
        Globals.Settings.formula_search.naturalProductProfile = New PresetProfileSettings With {.type = ComboBox2.SelectedIndex, .isCommon = CheckBox2.Checked}

        Globals.Settings.precursor_search = New PrecursorSearchSettings With {
            .ppm = NumericUpDown1.Value,
            .precursor_types = TextBox1.Text.LineTokens
        }

        Globals.Settings.Save()
    End Sub

    Public Sub ShowPage() Implements IPageSettings.ShowPage
        Call MyApplication.host.ShowPage(MyApplication.host.mzkitSearch)
        Call MyApplication.host.ShowMzkitToolkit()
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

    End Sub
End Class
