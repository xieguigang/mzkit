#Region "Microsoft.VisualBasic::d934041670f9828af641220a18c6edf7, src\mzkit\mzkit\pages\PageMzSearch.vb"

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
'     Function: FormulaFinderTest4
' 
'     Sub: doMzSearch, PageMzSearch_Load, runSearchInternal, ShowFormulaFinderResults
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports System.Threading
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports RibbonLib.Interop
Imports RowObject = Microsoft.VisualBasic.Data.csv.IO.RowObject

Public Class PageMzSearch

    Dim host As frmMain
    Dim tDataTable As DataTable

    Public Sub doMzSearch(mz As Double, ppm As Double)
        Dim progress As New frmTaskProgress

        Call New Thread(Sub() Call runSearchInternal(mz, ppm, progress)).Start()
        Call progress.ShowDialog()
    End Sub

    Private Sub runSearchInternal(mz As Double, ppm As Double, progress As frmTaskProgress)
        Dim oMwtWin As New FormulaSearch(opts:=New Formula.SearchOption)

        progress.Invoke(Sub() progress.Label2.Text = "initialize workspace...")

        oMwtWin.SetElementMode(MWElementAndMassRoutines.emElementModeConstants.emIsotopicMass)

        oMwtWin.FormulaFinder.CandidateElements.Clear()

        oMwtWin.FormulaFinder.AddCandidateElement("C")
        oMwtWin.FormulaFinder.AddCandidateElement("H")
        oMwtWin.FormulaFinder.AddCandidateElement("O")
        oMwtWin.FormulaFinder.AddCandidateElement("N")
        oMwtWin.FormulaFinder.AddCandidateElement("P")

        Dim searchOptions As New FormulaFinderOptions() With {
            .LimitChargeRange = True,
            .ChargeMin = 0,
            .ChargeMax = 2,
            .FindTargetMZ = True,
            .FindCharge = True,
            .SearchMode = eSearchMode.Bounded,
            .VerifyHydrogens = False
        }

        progress.Invoke(Sub() progress.Label2.Text = "running formula search...")

        Dim searchResults = FormulaFinderTest4(mz, ppm, oMwtWin, searchOptions).ToArray

        progress.Invoke(Sub() progress.Label2.Text = "output search result...")
        host.Invoke(Sub() host.ToolStripStatusLabel1.Text = $"Run formula search for m/z {mz} with tolerance error {ppm} ppm, have {searchResults.Length} formula found!")

        Call ShowFormulaFinderResults(searchOptions, searchResults, True, True)

        Call progress.Invoke(Sub() Call progress.Close())
    End Sub

    Private Function FormulaFinderTest4(mz As Double, ppm As Double, oMwtWin As MolecularWeightCalculator, searchOptions As FormulaFinderOptions) As IEnumerable(Of FormulaFinderResult)

        'searchOptions.LimitChargeRange = True
        'searchOptions.ChargeMin = -4
        'searchOptions.ChargeMax = 6
        'searchOptions.FindTargetMZ = True

        ' Search for 100 m/z, +/- 250 ppm
        Return oMwtWin.FormulaFinder.FindMatchesByMassPPM(mz, ppm, searchOptions)
    End Function

    Private Sub ShowFormulaFinderResults(
     searchOptions As FormulaFinderOptions,
     lstResults As IEnumerable(Of FormulaFinderResult),
     Optional deltaMassIsPPM As Boolean = False,
     Optional percentCompositionSearch As Boolean = False)

        Dim myDataSet = New DataSet("myDataSet")

        ' Create a DataTable.
        tDataTable = New DataTable("DataTable1")

        Dim massColumnName As String
        If deltaMassIsPPM Then
            massColumnName = "DeltaPPM"
        Else
            massColumnName = "DeltaMass"
        End If

        ' Add coluns to the table
        Dim cFormula As New DataColumn("Formula", GetType(String))
        Dim cMass As New DataColumn("Mass", GetType(Double))
        Dim cDeltaMass As New DataColumn(massColumnName, GetType(Double))
        Dim cCharge As New DataColumn("Charge", GetType(Integer))
        Dim cMZ As New DataColumn("M/Z", GetType(Double))
        Dim cPercentComp As New DataColumn("PercentCompInfo", GetType(String))

        tDataTable.Columns.Add(cFormula)
        tDataTable.Columns.Add(cMass)
        tDataTable.Columns.Add(cDeltaMass)
        tDataTable.Columns.Add(cCharge)
        tDataTable.Columns.Add(cMZ)
        tDataTable.Columns.Add(cPercentComp)

        If myDataSet.Tables.Count > 0 Then
            myDataSet.Tables.Clear()
        End If

        ' Add the table to the DataSet.
        myDataSet.Tables.Add(tDataTable)

        ' Populates the table. 
        Dim newRow As DataRow

        Dim sbPercentCompInfo = New StringBuilder()

        For Each result As FormulaFinderResult In lstResults
            newRow = tDataTable.NewRow()
            newRow("Formula") = result.EmpiricalFormula
            newRow("Mass") = Math.Round(result.Mass, 4)

            If deltaMassIsPPM Then
                newRow(massColumnName) = result.DeltaMass.ToString("0.0")
            Else
                newRow(massColumnName) = result.DeltaMass.ToString("0.000")
            End If

            newRow("Charge") = result.ChargeState

            If searchOptions.FindCharge Then
                newRow("M/Z") = Math.Round(result.MZ, 3)
            End If

            If percentCompositionSearch Then

                sbPercentCompInfo.Clear()

                For Each percentCompValue In result.PercentComposition
                    sbPercentCompInfo.Append(" " & percentCompValue.Key & "=" & percentCompValue.Value.ToString("0.00") & "%")
                Next
                newRow("PercentCompInfo") = sbPercentCompInfo.ToString().TrimStart()
            Else
                newRow("PercentCompInfo") = String.Empty
            End If

            tDataTable.Rows.Add(newRow)
        Next

        Invoke(Sub() Call dgDataGrid.SetDataBinding(myDataSet, "DataTable1"))

    End Sub

    Public Sub SaveSearchResultTable()
        If Not tDataTable Is Nothing Then
            Using file As New SaveFileDialog With {.Filter = "Excel Table(*.xls)|*.xls"}
                If file.ShowDialog = DialogResult.OK Then
                    Dim row As New RowObject

                    Using write As StreamWriter = file.FileName.OpenWriter
                        For i As Integer = 0 To tDataTable.Columns.Count - 1
                            row.Add(tDataTable.Columns(i).ColumnName)
                        Next

                        Call write.WriteLine(row.AsLine)

                        For i As Integer = 0 To tDataTable.Rows.Count - 1
                            Dim rdata = tDataTable.Rows(i)
                            row = New RowObject(rdata.ItemArray)
                            Call write.WriteLine(row.AsLine)
                        Next
                    End Using
                End If
            End Using
        End If
    End Sub

    Private Sub PageMzSearch_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        host = DirectCast(ParentForm, frmMain)
    End Sub

    Private Sub PageMzSearch_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        If Visible Then
            host.ribbonItems.TabGroupFormulaSearchTools.ContextAvailable = ContextAvailability.Active
        Else
            host.ribbonItems.TabGroupFormulaSearchTools.ContextAvailable = ContextAvailability.NotAvailable
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim mz As Double = Val(TextBox1.Text)
        Dim ppm As Double = 30

        Call doMzSearch(mz, ppm)
    End Sub
End Class

