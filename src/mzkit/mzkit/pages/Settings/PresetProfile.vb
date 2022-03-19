#Region "Microsoft.VisualBasic::e39eae53ed5b0d56608a661e82da147f, mzkit\src\mzkit\mzkit\pages\Settings\PresetProfile.vb"

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


    ' Code Statistics:

    '   Total Lines: 58
    '    Code Lines: 43
    ' Comment Lines: 0
    '   Blank Lines: 15
    '     File Size: 2.43 KB


    ' Class PresetProfile
    ' 
    '     Sub: LoadSettings, SaveSettings, ShowPage, TextBox1_TextChanged
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.mzkit_win32.Configuration
Imports BioNovoGene.mzkit_win32.My

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
