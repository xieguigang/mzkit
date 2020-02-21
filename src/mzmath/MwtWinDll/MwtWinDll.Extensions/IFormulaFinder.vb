#Region "Microsoft.VisualBasic::e9b893fffbfbc53c89d173fd1d5c5e1d, src\mzmath\MwtWinDll\MwtWinDll.Extensions\IFormulaFinder.vb"

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

' Module IFormulaFinder
' 
'     Properties: CommonAtoms
' 
'     Function: FormulaFinderOptions, SearchByLimitChargeLimits, SearchByLimitDaMass, SearchByLimitDaMassAndBounded, SearchByLimitDaMassDelta
'               SearchByLimitMaxMass, SearchByMZAndLimitCharges
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports PNNL.OMICS.MwtWinDll.FormulaFinder

#Disable Warning

Public Module IFormulaFinder

    ''' <summary>
    ''' 比较常见的有机化合物的元素构成列表
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property CommonAtoms As New AtomProfiles({"C", "H", "O", "N"})

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
