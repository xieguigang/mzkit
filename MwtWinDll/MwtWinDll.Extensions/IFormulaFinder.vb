Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Serialization.JSON
Imports MwtWinDll.FormulaFinderOptions

Public Module IFormulaFinder

    Public Structure AtomProfiles

        Dim Atoms As Dictionary(Of String, Integer)

        Public Sub SetAtoms(ByRef finder As MolecularWeightCalculator)
            If Atoms.Values.Sum <> 100% Then
                Dim ex As New Exception(Atoms.GetJson)
                ex = New ArgumentException($"SUM({Atoms.Values.Sum}) <> 100!", ex)
                Throw ex
            End If

            For Each atom In Atoms
                Call finder.FormulaFinder.AddCandidateElement(atom.Key, atom.Value)
            Next
        End Sub
    End Structure

    Private Function FormulaFinderOptions() As (mwt As MolecularWeightCalculator, opts As FormulaFinderOptions)
        Dim mwtWin As New MolecularWeightCalculator()

        Call mwtWin.SetElementMode(MWElementAndMassRoutines.emElementModeConstants.emIsotopicMass)
        Call mwtWin.FormulaFinder.CandidateElements.Clear()

        Dim searchOptions As New FormulaFinderOptions()

        searchOptions.LimitChargeRange = False
        searchOptions.ChargeMin = 1
        searchOptions.ChargeMax = 1
        searchOptions.FindTargetMZ = False

        Return (mwtWin, searchOptions)
    End Function

    ''' <summary>
    ''' Example as ``Search for 200 Da, +/- 0.05 Da``
    ''' </summary>
    ''' <param name="candidateAtoms"></param>
    ''' <param name="Da!"></param>
    ''' <param name="deltaDa!"></param>
    ''' <returns></returns>
    <Extension>
    Public Function SearchByLimitDaMassDelta(candidateAtoms As AtomProfiles, Da!, Optional deltaDa! = 0.05) As FormulaFinderResult()
        With FormulaFinderOptions()
            ' Search for 200 Da, +/- 0.05 Da
            Call candidateAtoms.SetAtoms(.mwt)
            Return .mwt.FormulaFinder.FindMatchesByMass(Da, deltaDa, .opts)
        End With
    End Function

    ''' <summary>
    ''' Example as ``Search for 200 Da, +/- 250 ppm``
    ''' </summary>
    ''' <param name="Da">Target mass</param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function SearchByLimitDaMass(candidateAtoms As AtomProfiles, Da!, Optional deltaPPM! = 250) As FormulaFinderResult()
        With FormulaFinderOptions()
            Call candidateAtoms.SetAtoms(.mwt)
            Return .mwt.FormulaFinder.FindMatchesByMassPPM(Da, deltaPPM, .opts)
        End With
    End Function

    ''' <summary>
    ''' Example as ``Search for 200 Da, +/- 250 ppm``
    ''' </summary>
    ''' <param name="candidateAtoms"></param>
    ''' <param name="chargeRange"></param>
    ''' <param name="Da!"></param>
    ''' <param name="deltaPPM!"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function SearchByLimitChargeLimits(candidateAtoms As AtomProfiles, chargeRange As IntRange, Da!, Optional deltaPPM! = 250) As FormulaFinderResult()
        With FormulaFinderOptions()
            Call candidateAtoms.SetAtoms(.mwt)

            With .opts
                .LimitChargeRange = True
                .ChargeMin = chargeRange.Min
                .ChargeMax = chargeRange.Max
            End With

            Return .mwt.FormulaFinder.FindMatchesByMassPPM(Da, deltaPPM, .opts)
        End With
    End Function

    ''' <summary>
    ''' Example as ``Search for 100 m/z, +/- 250 ppm``
    ''' </summary>
    ''' <param name="candidateAtoms"></param>
    ''' <param name="chargeRange"></param>
    ''' <param name="mz!">m/z</param>
    ''' <param name="deltaPPM!"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function SearchByMZAndLimitCharges(candidateAtoms As AtomProfiles, chargeRange As IntRange, mz!, Optional deltaPPM! = 250) As FormulaFinderResult()
        With FormulaFinderOptions()
            Call candidateAtoms.SetAtoms(.mwt)

            With .opts
                .LimitChargeRange = True
                .ChargeMin = chargeRange.Min
                .ChargeMax = chargeRange.Max
                .FindTargetMZ = True
            End With

            ' Search for 100 m/z, +/- 250 ppm
            Return .mwt.FormulaFinder.FindMatchesByMassPPM(mz, deltaPPM, .opts)
        End With
    End Function

    ''' <summary>
    ''' Example as ``Search for percent composition results, maximum mass 400 Da``
    ''' </summary>
    ''' <param name="candidateAtoms"></param>
    ''' <param name="Da!"></param>
    ''' <param name="deltaPPM!"></param>
    ''' <returns></returns>
    <Extension>
    Public Function SearchByLimitMaxMass(candidateAtoms As AtomProfiles, Da!, Optional deltaPPM! = 1)
        With FormulaFinderOptions()
            Call candidateAtoms.SetAtoms(.mwt)
            ' Search for percent composition results, maximum mass 400 Da
            Return .mwt.FormulaFinder.FindMatchesByPercentComposition(Da, deltaPPM, .opts)
        End With
    End Function

    ''' <summary>
    ''' Example as ``Search for 200 Da, +/- 250 ppm``
    ''' </summary>
    ''' <param name="candidateAtoms"></param>
    ''' <param name="Da!"></param>
    ''' <param name="deltaPPM!"></param>
    ''' <returns></returns>
    <Extension>
    Public Function SearchByLimitDaMassAndBounded(candidateAtoms As AtomProfiles, Da!, Optional deltaPPM! = 250) As FormulaFinderResult()
        With FormulaFinderOptions()
            Call candidateAtoms.SetAtoms(.mwt)
            .opts.SearchMode = eSearchMode.Bounded
            ' Search for 200 Da, +/- 250 ppm
            Return .mwt.FormulaFinder.FindMatchesByMassPPM(Da, deltaPPM, .opts)
        End With
    End Function
End Module
