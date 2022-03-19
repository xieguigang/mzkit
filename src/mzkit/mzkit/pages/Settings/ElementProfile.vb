#Region "Microsoft.VisualBasic::44bf0d86a0fd25b76033d333b12f0fe2, mzkit\src\mzkit\mzkit\pages\Settings\ElementProfile.vb"

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

    '   Total Lines: 68
    '    Code Lines: 53
    ' Comment Lines: 0
    '   Blank Lines: 15
    '     File Size: 2.79 KB


    ' Class ElementProfile
    ' 
    '     Sub: Button1_Click, ComboBox1_SelectedIndexChanged, loadPresetProfile, LoadSettings, SaveSettings
    '          ShowPage
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.Linq
Imports BioNovoGene.mzkit_win32.Configuration
Imports BioNovoGene.mzkit_win32.My

Public Class ElementProfile : Implements ISaveSettings, IPageSettings

    Public Sub LoadSettings() Implements ISaveSettings.LoadSettings
        DataGridView1.Rows.Clear()

        If Globals.Settings.formula_search Is Nothing Then
            Globals.Settings.formula_search = New FormulaSearchProfile With {.elements = New Dictionary(Of String, ElementRange)}
        End If

        For Each element In Globals.Settings.formula_search.elements.SafeQuery
            DataGridView1.Rows.Add({element.Key, element.Value.min, element.Value.max})
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
        Call MyApplication.host.ShowPage(MyApplication.host.mzkitSearch)
        Call MyApplication.host.ShowMzkitToolkit()
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

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Call LoadSettings()
    End Sub
End Class
