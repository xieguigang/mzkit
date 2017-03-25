Imports System.Text

Public Module IFormulaFinder


    Public Function FormulaFinder()

        Dim oMwtWin = New MolecularWeightCalculator()

        oMwtWin.SetElementMode(MWElementAndMassRoutines.emElementModeConstants.emIsotopicMass)

        oMwtWin.FormulaFinder.CandidateElements.Clear()

        oMwtWin.FormulaFinder.AddCandidateElement("C")
        oMwtWin.FormulaFinder.AddCandidateElement("H")
        oMwtWin.FormulaFinder.AddCandidateElement("N")
        oMwtWin.FormulaFinder.AddCandidateElement("O")

        ' Abbreviations are supported, for example Serine
        oMwtWin.FormulaFinder.AddCandidateElement("Ser")

        Dim searchOptions = New FormulaFinderOptions()

        searchOptions.LimitChargeRange = False
        searchOptions.ChargeMin = 1
        searchOptions.ChargeMax = 1
        searchOptions.FindTargetMZ = False

        If cboFormulaFinderTestMode.SelectedIndex = 0 Then FormulaFinderTest1(oMwtWin, searchOptions)

        If cboFormulaFinderTestMode.SelectedIndex = 1 Then FormulaFinderTest2(oMwtWin, searchOptions)

        If cboFormulaFinderTestMode.SelectedIndex = 2 Then FormulaFinderTest3(oMwtWin, searchOptions)

        If cboFormulaFinderTestMode.SelectedIndex = 3 Then FormulaFinderTest4(oMwtWin, searchOptions)

        If cboFormulaFinderTestMode.SelectedIndex = 4 Then FormulaFinderTest5(oMwtWin, searchOptions)

        If cboFormulaFinderTestMode.SelectedIndex = 5 Then FormulaFinderTest6(oMwtWin, searchOptions)
    End Function

    Private Sub FormulaFinderTest1(oMwtWin As MolecularWeightCalculator, searchOptions As FormulaFinderOptions)

        ' Search for 200 Da, +/- 0.05 Da
        Dim lstResults = oMwtWin.FormulaFinder.FindMatchesByMass(200, 0.05, searchOptions)
        ShowFormulaFinderResults(searchOptions, lstResults)

    End Sub

    Private Sub FormulaFinderTest2(oMwtWin As MolecularWeightCalculator, searchOptions As FormulaFinderOptions)

        ' Search for 200 Da, +/- 250 ppm
        Dim lstResults = oMwtWin.FormulaFinder.FindMatchesByMassPPM(200, 250, searchOptions)
        ShowFormulaFinderResults(searchOptions, lstResults, True)

    End Sub

    Private Sub FormulaFinderTest3(oMwtWin As MolecularWeightCalculator, searchOptions As FormulaFinderOptions)

        searchOptions.LimitChargeRange = True
        searchOptions.ChargeMin = -4
        searchOptions.ChargeMax = 6

        ' Search for 200 Da, +/- 250 ppm
        Dim lstResults = oMwtWin.FormulaFinder.FindMatchesByMassPPM(200, 250, searchOptions)
        ShowFormulaFinderResults(searchOptions, lstResults, True)

    End Sub

    Private Sub FormulaFinderTest4(oMwtWin As MolecularWeightCalculator, searchOptions As FormulaFinderOptions)

        searchOptions.LimitChargeRange = True
        searchOptions.ChargeMin = -4
        searchOptions.ChargeMax = 6
        searchOptions.FindTargetMZ = True

        ' Search for 100 m/z, +/- 250 ppm
        Dim lstResults = oMwtWin.FormulaFinder.FindMatchesByMassPPM(100, 250, searchOptions)
        ShowFormulaFinderResults(searchOptions, lstResults, True)

    End Sub

    Private Sub FormulaFinderTest5(oMwtWin As MolecularWeightCalculator, searchOptions As FormulaFinderOptions)

        oMwtWin.FormulaFinder.CandidateElements.Clear()

        oMwtWin.FormulaFinder.AddCandidateElement("C", 70)
        oMwtWin.FormulaFinder.AddCandidateElement("H", 10)
        oMwtWin.FormulaFinder.AddCandidateElement("N", 10)
        oMwtWin.FormulaFinder.AddCandidateElement("O", 10)

        ' Search for percent composition results, maximum mass 400 Da
        Dim lstResults = oMwtWin.FormulaFinder.FindMatchesByPercentComposition(400, 1, searchOptions)
        ShowFormulaFinderResults(searchOptions, lstResults, False, True)

    End Sub

    Private Sub FormulaFinderTest6(oMwtWin As MolecularWeightCalculator, searchOptions As FormulaFinderOptions)

        searchOptions.SearchMode = FormulaFinderOptions.eSearchMode.Bounded

        ' Search for 200 Da, +/- 250 ppm
        Dim lstResults = oMwtWin.FormulaFinder.FindMatchesByMassPPM(200, 250, searchOptions)
        ShowFormulaFinderResults(searchOptions, lstResults, True)

    End Sub

    Private Sub ShowFormulaFinderResults(
      searchOptions As FormulaFinderOptions,
      lstResults As List(Of FormulaFinderResult),
      Optional deltaMassIsPPM As Boolean = False,
      Optional percentCompositionSearch As Boolean = False)

        myDataSet = New DataSet("myDataSet")

        ' Create a DataTable.
        Dim tDataTable As New DataTable("DataTable1")

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

        For Each result In lstResults
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

        dgDataGrid.SetDataBinding(myDataSet, "DataTable1")

    End Sub

End Module
