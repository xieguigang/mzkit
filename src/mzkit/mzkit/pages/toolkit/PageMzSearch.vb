#Region "Microsoft.VisualBasic::f89f41de0c4852f9323b29207d9c7ed5, mzkit\src\mzkit\mzkit\pages\toolkit\PageMzSearch.vb"

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

    '   Total Lines: 505
    '    Code Lines: 392
    ' Comment Lines: 30
    '   Blank Lines: 83
    '     File Size: 22.94 KB


    ' Class PageMzSearch
    ' 
    '     Function: getDatabase, getDatabaseNames, GetFormulaSearchProfileName, GetIsotopeGaussianLine, GetIsotopeMS1
    '               GetProfile
    ' 
    '     Sub: Button1_Click, Button2_Click, Button3_Click, DataGridView1_CellContentClick, doExactMassSearch
    '          doMzSearch, ExportToolStripMenuItem_Click, GaussianPlotToolStripMenuItem_Click, MS1PlotToolStripMenuItem_Click, MSISearchToolStripMenuItem_Click
    '          PageMzSearch_Load, PageMzSearch_VisibleChanged, ReloadMetaDatabase, (+2 Overloads) runSearchInternal, SaveSearchResultTable
    '          (+2 Overloads) ShowFormulaFinderResults, TextBox3_TextChanged
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing.Drawing2D
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.IsotopicPatterns
Imports BioNovoGene.BioDeep.MSEngine
Imports BioNovoGene.mzkit_win32.Configuration
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports RibbonLib.Interop
Imports WeifenLuo.WinFormsUI.Docking
Imports stdNum = System.Math

Public Class PageMzSearch

    Private Sub doExactMassSearch(mz As Double, ppm As Double)
        Dim progress As New frmTaskProgress
        Dim cancel As Value(Of Boolean) = False

        progress.TaskCancel = Sub() cancel.Value = True
        progress.ShowProgressTitle("Do M/z Search", directAccess:=True)
        progress.ShowProgressDetails("Initialized...", directAccess:=True)

        Call New Thread(
            Sub()
                Call runSearchInternal(mz, ppm, progress, cancel)
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
                Call Process.Start($"http://query.biodeep.cn/search?expression=[formula]&category=metabolite&formula={formula}")
            End If
        End If
    End Sub

    Public Sub SaveSearchResultTable()
        Call DataGridView1.SaveDataGrid($"Search result table is saved at location:{vbCrLf}%s")
    End Sub

    Private Sub PageMzSearch_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim vs_win As DocumentWindow = DirectCast(ParentForm, DocumentWindow)

        CheckedListBox1.SetItemChecked(0, True)
        ComboBox1.SelectedIndex = 0

        Call vs_win.VisualStudioToolStripExtender1.SetStyle(ContextMenuStrip1, VisualStudioToolStripExtender.VsVersion.Vs2015, vs_win.VS2015LightTheme1)
        Call ReloadMetaDatabase()
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
        Dim ppm As Double = 5

        Call doExactMassSearch(mz, ppm)
    End Sub

    Dim isotope As IsotopeDistribution

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim formulaStr As String = Strings.Trim(TextBox2.Text)

        If formulaStr.StringEmpty Then
            Call MyApplication.host.showStatusMessage("No formula input!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        Else
            isotope = FormulaScanner _
                .ScanFormula(formulaStr) _
                .DoCall(AddressOf IsotopicPatterns.Calculator.GenerateDistribution)
        End If

        Call DataGridView2.Rows.Clear()

        For i As Integer = 0 To isotope.Size - 1
            If isotope.intensity(i) > 0 Then
                DataGridView2.Rows.Add({isotope.mz(i), isotope.intensity(i)})
            End If
        Next

        Dim peakPlot As Image = PeakAssign.DrawSpectrumPeaks(GetIsotopeMS1, labelIntensity:=0.01).AsGDIImage

        MS1PlotToolStripMenuItem.Checked = True
        GaussianPlotToolStripMenuItem.Checked = False
        PictureBox1.BackgroundImage = peakPlot
    End Sub

    Private Function GetIsotopeMS1() As LibraryMatrix
        Return New LibraryMatrix With {
            .ms2 = isotope.data _
                .Select(Function(mzi)
                            Return New ms2 With {
                                .mz = mzi.abs_mass,
                                .intensity = mzi.abundance
                            }
                        End Function) _
                .ToArray,
            .name = $"{isotope.formula} [MS1, {isotope.exactMass.ToString("F4")}]"
        }
    End Function

    Private Function GetIsotopeGaussianLine() As SerialData
        Return New SerialData With {
            .color = Color.SteelBlue,
            .lineType = DashStyle.Dash,
            .pointSize = 5,
            .pts = isotope.mz _
                .Select(Function(mzi, i)
                            Return New PointData(mzi, isotope.intensity(i))
                        End Function) _
                .Where(Function(p) p.pt.Y > 0) _
                .ToArray,
            .shape = LegendStyles.Diamond,
            .title = $"{isotope.formula}'s Gaussian Plot",
            .width = 3
        }
    End Function

    Private Sub MS1PlotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MS1PlotToolStripMenuItem.Click
        If MS1PlotToolStripMenuItem.Checked Then
            Return
        End If

        If Not isotope Is Nothing Then
            PictureBox1.BackgroundImage = PeakAssign.DrawSpectrumPeaks(GetIsotopeMS1, labelIntensity:=0.01).AsGDIImage
        End If

        GaussianPlotToolStripMenuItem.Checked = MS1PlotToolStripMenuItem.Checked
        MS1PlotToolStripMenuItem.Checked = Not MS1PlotToolStripMenuItem.Checked
    End Sub

    Private Sub GaussianPlotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GaussianPlotToolStripMenuItem.Click
        If GaussianPlotToolStripMenuItem.Checked Then
            Return
        End If

        If Not isotope Is Nothing Then
            PictureBox1.BackgroundImage = Scatter.Plot(
                c:={GetIsotopeGaussianLine()},
                fill:=True,
                size:="2100,1400",
                gridFill:="white",
                Xlabel:="M/Z",
                Ylabel:="Gaussian Probability",
                axisLabelCSS:=CSSFont.Win7LargeBold
            ).AsGDIImage
        End If

        MS1PlotToolStripMenuItem.Checked = GaussianPlotToolStripMenuItem.Checked
        GaussianPlotToolStripMenuItem.Checked = Not GaussianPlotToolStripMenuItem.Checked
    End Sub

    Private Sub ExportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportToolStripMenuItem.Click
        If isotope Is Nothing Then
            Return
        End If

        If MS1PlotToolStripMenuItem.Checked Then
            Dim ion As New MGF.Ions With {
                .Accession = isotope.formula,
                .Charge = 1,
                .Database = "IsotopeDistribution",
                .Locus = isotope.formula,
                .Title = $"{isotope.ToString} [MS1]",
                .PepMass = New NamedValue(FormulaScanner.ScanFormula(isotope.formula).ExactMass, 1),
                .Peaks = isotope.data _
                    .Select(Function(i)
                                Return New ms2 With {
                                    .mz = i.abs_mass,
                                    .intensity = i.abundance
                                }
                            End Function) _
                    .ToArray
            }

            Using file As New SaveFileDialog With {
                .Filter = "MGF Ion(*.mgf)|*.mgf"
            }
                If file.ShowDialog = DialogResult.OK Then
                    Call ion.SaveTo(file.FileName)
                End If
            End Using
        Else
            Call DataGridView2.SaveDataGrid("Save Gaussian Data")
        End If
    End Sub

    Private Sub MSISearchToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MSISearchToolStripMenuItem.Click
        If isotope Is Nothing Then
            Return
        End If

        Dim searchPage As New frmSpectrumSearch

        searchPage.Show(MyApplication.host.dockPanel)
        searchPage.page.loadMs2(isotope.GetMS)
        searchPage.page.runSearch(isotope)
    End Sub

    Public Iterator Function getDatabaseNames() As IEnumerable(Of String)
        For Each check In CheckedListBox2.CheckedItems
            Yield check.ToString
        Next
    End Function

    Public Sub ReloadMetaDatabase()
        CheckedListBox2.Items.Clear()

        CheckedListBox2.Items.Add("kegg")
        CheckedListBox2.Items.Add("lipidmaps")
        CheckedListBox2.Items.Add("chebi")

        CheckedListBox2.SetItemChecked(0, True)
        CheckedListBox2.SetItemChecked(1, True)
        CheckedListBox2.SetItemChecked(2, True)
    End Sub

    Private Function getDatabase(name As String, ionMode As Integer, tolerance As Tolerance) As IMzQuery
        Select Case name
            Case "kegg"
                Return Globals.LoadKEGG(AddressOf MyApplication.LogText, ionMode, tolerance)
            Case "lipidmaps"
                Return Globals.LoadLipidMaps(AddressOf MyApplication.LogText, ionMode, tolerance)
            Case "chebi"
                Return Globals.LoadChebi(AddressOf MyApplication.LogText, ionMode, tolerance)
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' do ms1 peak list annotation
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim modes As String() = (From x As Object
                                 In CheckedListBox1.CheckedItems
                                 Let str = x.ToString
                                 Select str).ToArray
        Dim mzset As Double() = TextBox3.Text.LineTokens.Select(AddressOf Val).ToArray
        Dim result As New List(Of NamedCollection(Of MzQuery))
        Dim tolerance As Tolerance = Tolerance.PPM(NumericUpDown1.Value)
        Dim keggMeta As DBPool = Nothing
        Dim dbNames As String() = getDatabaseNames.ToArray

        For Each mode As String In modes
            Dim modeValue As Integer = Provider.ParseIonMode(mode)

            keggMeta = frmTaskProgress.LoadData(
                Function(print)
                    Dim database As New DBPool

                    For Each db As String In dbNames
                        Call print($"Load annotation database repository data... [{db}]")
                        Call database.Register(db, getDatabase(db, modeValue, tolerance))
                    Next

                    Return database
                End Function, info:="Load annotation database repository data...")

            Dim anno As NamedCollection(Of MzQuery)() = frmTaskProgress.LoadData(
                streamLoad:=Function(print) keggMeta.MSetAnnotation(mzset, print).ToArray,
                title:="Peak List Annotation",
                info:="Run ms1 peak list data annotation..."
            )

            Call result.AddRange(anno)
        Next

        Dim table As frmTableViewer = VisualStudio.ShowDocument(Of frmTableViewer)

        Call table.LoadTable(
            Sub(grid)
                Call grid.Columns.Add("mz", GetType(Double))
                Call grid.Columns.Add("mz_ref", GetType(Double))
                Call grid.Columns.Add("ppm", GetType(Double))
                Call grid.Columns.Add("precursorType", GetType(String))
                Call grid.Columns.Add("kegg_id", GetType(String))
                Call grid.Columns.Add("name", GetType(String))
                Call grid.Columns.Add("formula", GetType(String))
                Call grid.Columns.Add("exact_mass", GetType(Double))
                Call grid.Columns.Add("score", GetType(Double))
                Call grid.Columns.Add("metadb", GetType(String))

                For Each setList In result
                    For Each ion As MzQuery In setList
                        Dim kegg = keggMeta.getAnnotation(ion.unique_id)

                        Call grid.Rows.Add(
                            ion.mz.ToString("F4"),
                            ion.mz_ref.ToString("F4"),
                            ion.ppm.ToString("F1"),
                            ion.precursorType,
                            ion.unique_id,
                            If(kegg.name, ion.unique_id),
                            kegg.formula,
                            FormulaScanner.ScanFormula(kegg.formula).ExactMass,
                            ion.score.ToString("F2"),
                            setList.name
                        )

                        Call Application.DoEvents()
                    Next
                Next
            End Sub)

        Call MyApplication.host.showStatusMessage($"get {result.Count} annotation result for {mzset.Length} m/z ions list!")
    End Sub

    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged

    End Sub
End Class
