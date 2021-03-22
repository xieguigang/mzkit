#Region "Microsoft.VisualBasic::47d55ba4eea518ca4590d35055061cb2, src\mzkit\mzkit\pages\toolkit\PageMzSearch.vb"

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

    ' Class PageMzSearch
    ' 
    '     Function: GetFormulaSearchProfileName, GetProfile
    ' 
    '     Sub: Button1_Click, DataGridView1_CellContentClick, doExactMassSearch, doMzSearch, PageMzSearch_Load
    '          PageMzSearch_VisibleChanged, (+2 Overloads) runSearchInternal, SaveSearchResultTable, (+2 Overloads) ShowFormulaFinderResults
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.Language
Imports mzkit.Configuration
Imports mzkit.My
Imports RibbonLib.Interop
Imports stdNum = System.Math

Public Class PageMzSearch

    Private Sub doExactMassSearch(exact_mass As Double, ppm As Double)
        Dim progress As New frmTaskProgress
        Dim cancel As Value(Of Boolean) = False

        progress.TaskCancel = Sub() cancel.Value = True
        progress.ShowProgressTitle("Do M/z Search", directAccess:=True)
        progress.ShowProgressDetails("Initialized...", directAccess:=True)

        Call New Thread(
            Sub()
                Call runSearchInternal(exact_mass, ppm, progress, cancel)
            End Sub).Start()
        Call progress.ShowDialog()
    End Sub

    Public Sub doMzSearch(mz As Double, charge As Integer, ionMode As Integer)
        Dim progress As New frmTaskProgress
        Dim cancel As Value(Of Boolean) = False

        progress.TaskCancel = Sub() cancel.Value = True
        progress.ShowProgressTitle("Do M/z Search", directAccess:=True)
        progress.ShowProgressDetails("Initialized...", directAccess:=True)

        Call New Thread(
            Sub()
                Call runSearchInternal(mz, charge, ionMode, progress, cancel)
            End Sub).Start()
        Call progress.ShowDialog()
    End Sub

    Public Function GetFormulaSearchProfileName() As FormulaSearchProfiles
        ' get selected item index from combo box 1
        ' Dim selectedItemIndex As UInteger = ribbonItems.ComboFormulaSearchProfile.SelectedItem

        ' If selectedItemIndex = Constants.UI_Collection_InvalidIndex Then
        'Return FormulaSearchProfiles.Custom
        'Else
        ' Dim selectedItem As Object = Nothing
        ' ribbonItems.ComboFormulaSearchProfile.ItemsSource.GetItem(selectedItemIndex, selectedItem)
        ' Dim uiItem As IUISimplePropertySet = CType(selectedItem, IUISimplePropertySet)
        'Dim itemLabel As PropVariant
        'uiItem.GetValue(RibbonProperties.Label, itemLabel)

        'Dim selected As String = Strings.LCase(CStr(itemLabel.Value))
        '  Dim selected As String = Strings.LCase(ribbonItems.ComboFormulaSearchProfile3.StringValue)
        Dim selected As FormulaSearchProfiles = ComboBox1.SelectedIndex
        '  MsgBox(selected)

        If selected < 0 Then
            selected = FormulaSearchProfiles.Default
        End If

        Return selected

        'If selected = FormulaSearchProfiles.Default.Description.ToLower Then
        '    Return FormulaSearchProfiles.Default
        'ElseIf selected = FormulaSearchProfiles.SmallMolecule.Description.ToLower Then
        '    Return FormulaSearchProfiles.SmallMolecule
        'ElseIf selected = FormulaSearchProfiles.NaturalProduct.Description.ToLower Then
        '    Return FormulaSearchProfiles.NaturalProduct
        'Else
        '    Return FormulaSearchProfiles.Custom
        'End If
        ' End If
    End Function

    Private Function GetProfile() As SearchOption
        Select Case GetFormulaSearchProfileName()
            Case FormulaSearchProfiles.Default
                Return SearchOption.DefaultMetaboliteProfile
            Case FormulaSearchProfiles.NaturalProduct
                Return SearchOption.NaturalProduct(
                    Globals.Settings.formula_search.naturalProductProfile.type,
                    Globals.Settings.formula_search.naturalProductProfile.isCommon
                )
            Case FormulaSearchProfiles.SmallMolecule
                Return SearchOption.SmallMolecule(
                    Globals.Settings.formula_search.smallMoleculeProfile.type,
                    Globals.Settings.formula_search.smallMoleculeProfile.isCommon
                )
            Case FormulaSearchProfiles.GeneralFlavone
                Return SearchOption.GeneralFlavone
            Case Else
                If Globals.Settings.formula_search Is Nothing Then
                    Return SearchOption.DefaultMetaboliteProfile
                Else
                    Return Globals.Settings.formula_search.CreateOptions
                End If
        End Select
    End Function

    Private Sub runSearchInternal(mz As Double, charge As Integer, ionMode As Integer, progress As frmTaskProgress, cancel As Value(Of Boolean))
        Thread.Sleep(100)
        progress.ShowProgressTitle("initialize workspace...")

        Dim config As PrecursorSearchSettings = Globals.Settings.precursor_search
        Dim opts = DirectCast(Invoke(Function() GetProfile()), SearchOption).AdjustPpm(config.ppm)
        Dim oMwtWin As New PrecursorIonSearch(
            opts:=opts,
            progress:=AddressOf progress.ShowProgressDetails,
            precursorTypeProgress:=AddressOf progress.ShowProgressTitle
        )

        oMwtWin.AddPrecursorTypeRanges(config.precursor_types)

        progress.ShowProgressTitle("running formula search...")

        Dim searchResults = oMwtWin.SearchByPrecursorMz(mz, charge, ionMode, cancel).ToArray

        progress.ShowProgressTitle("output search result...")
        MyApplication.host.Invoke(
            Sub()
                MyApplication.host.ToolStripStatusLabel1.Text = $"Run formula search for m/z {mz} with tolerance error {config.ppm} ppm, have {searchResults.Length} formula found!"
            End Sub)

        Call Me.Invoke(Sub() Call ShowFormulaFinderResults(searchResults))
        Call progress.Invoke(Sub() Call progress.Close())
    End Sub

    Private Sub runSearchInternal(exact_mass As Double, ppm As Double, progress As frmTaskProgress, cancel As Value(Of Boolean))
        Thread.Sleep(100)
        progress.ShowProgressTitle("initialize workspace...")

        Dim opts = DirectCast(Invoke(Function() GetProfile()), SearchOption).AdjustPpm(ppm)
        Dim oMwtWin As New FormulaSearch(
            opts:=opts,
            progress:=AddressOf progress.ShowProgressDetails
        )

        progress.ShowProgressTitle("running formula search...")

        Dim searchResults = oMwtWin.SearchByExactMass(exact_mass, cancel:=cancel).ToArray

        progress.ShowProgressTitle("output search result...")
        MyApplication.host.Invoke(
            Sub()
                MyApplication.host.ToolStripStatusLabel1.Text = $"Run formula search for exact mass {exact_mass} with tolerance error {ppm} ppm, have {searchResults.Length} formula found!"
            End Sub)

        Call Me.Invoke(Sub() Call ShowFormulaFinderResults(searchResults))
        Call progress.Invoke(Sub() Call progress.Close())
    End Sub

    Private Sub ShowFormulaFinderResults(lstResults As IEnumerable(Of PrecursorIonComposition))
        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        ' Add coluns to the table
        DataGridView1.Columns.Add(New DataGridViewLinkColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .ValueType = GetType(String), .HeaderText = "Formula"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .ValueType = GetType(String), .HeaderText = "Exact Mass"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .ValueType = GetType(String), .HeaderText = "PPM"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .ValueType = GetType(String), .HeaderText = "Charge"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .ValueType = GetType(String), .HeaderText = "Adducts"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .ValueType = GetType(String), .HeaderText = "M"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .ValueType = GetType(String), .HeaderText = "Precursor Type"})

        For Each result As PrecursorIonComposition In lstResults
            DataGridView1.Rows.Add(
                result.EmpiricalFormula,
                result.ExactMass,
                result.ppm,
                result.charge,
                result.adducts,
                result.M,
                result.precursor_type
            )
        Next
    End Sub

    Private Sub ShowFormulaFinderResults(lstResults As IEnumerable(Of FormulaComposition))
        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        ' Add coluns to the table
        DataGridView1.Columns.Add(New DataGridViewLinkColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .ValueType = GetType(String), .HeaderText = "Formula"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .ValueType = GetType(String), .HeaderText = "Exact Mass"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .ValueType = GetType(String), .HeaderText = "PPM"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .ValueType = GetType(String), .HeaderText = "Charge"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .ValueType = GetType(String), .HeaderText = "m/z"})

        For Each result As FormulaComposition In lstResults
            DataGridView1.Rows.Add(result.EmpiricalFormula, result.ExactMass, result.ppm, result.charge, stdNum.Abs(result.ExactMass / result.charge))
        Next
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        If e.ColumnIndex = Scan0 AndAlso e.RowIndex >= 0 Then
            Dim formula As String = DataGridView1.Rows(e.RowIndex).Cells(0).Value?.ToString

            If Not formula.StringEmpty Then
                Call Process.Start($"https://query.biodeep.cn/search?expression=[formula]&category=metabolite&formula={formula}")
            End If
        End If
    End Sub

    Public Sub SaveSearchResultTable()
        Call DataGridView1.SaveDataGrid($"Search result table is saved at location:{vbCrLf}%s")
    End Sub

    Private Sub PageMzSearch_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBox1.SelectedIndex = 0
    End Sub

    Private Sub PageMzSearch_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        If Visible Then
            MyApplication.host.ribbonItems.TabGroupFormulaSearchTools.ContextAvailable = ContextAvailability.Active
        Else
            MyApplication.host.ribbonItems.TabGroupFormulaSearchTools.ContextAvailable = ContextAvailability.NotAvailable
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim mz As Double = Val(TextBox1.Text)
        Dim ppm As Double = 1

        Call doExactMassSearch(mz, ppm)
    End Sub
End Class
