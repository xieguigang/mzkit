Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

''' <summary>
''' This class is the main program to find the molecular formula candidates from the mass spectra and to rank them.
''' </summary>
Public Class FormulaGenerator
    Private Const cMass As Double = 12.0
    Private Const hMass As Double = 1.00782503207
    Private Const nMass As Double = 14.0030740048
    Private Const oMass As Double = 15.99491461956
    Private Const sMass As Double = 31.972071
    Private Const pMass As Double = 30.97376163
    Private Const fMass As Double = 18.99840322
    Private Const siMass As Double = 27.9769265325
    Private Const clMass As Double = 34.96885268
    Private Const brMass As Double = 78.9183371
    Private Const iMass As Double = 126.904473

    Private isCheckedNum As Integer

    Private hMinFold As Double
    Private hMaxFold As Double
    Private fMaxFold As Double
    Private clMaxFold As Double
    Private brMaxFold As Double
    Private nMaxFold As Double
    Private oMaxFold As Double
    Private pMaxFold As Double
    Private sMaxFold As Double
    Private iMaxFold As Double
    Private siMaxFold As Double

    Private maxMassFoldChange As Double

    Private oCheck As Boolean
    Private nCheck As Boolean
    Private pCheck As Boolean
    Private sCheck As Boolean
    Private fCheck As Boolean
    Private brCheck As Boolean
    Private clCheck As Boolean
    Private iCheck As Boolean
    Private siCheck As Boolean
    Private valenceCheck As Boolean
    Private probabilityCheck As Boolean
    Private coverRange As CoverRange

    ''' <summary>
    ''' This is the constructor of this program.
    ''' The parameters will be set by the user-defined paramerters.
    ''' </summary>
    ''' <param name="param"></param>
    Public Sub New(ByVal param As AnalysisParamOfMsfinder)
        Me.oCheck = param.IsOcheck
        Me.nCheck = param.IsNcheck
        Me.pCheck = param.IsPcheck
        Me.sCheck = param.IsScheck
        Me.fCheck = param.IsFcheck
        Me.clCheck = param.IsClCheck
        Me.brCheck = param.IsBrCheck
        Me.iCheck = param.IsIcheck

        Me.siCheck = param.IsSiCheck
        Me.valenceCheck = param.IsLewisAndSeniorCheck
        Me.coverRange = param.CoverRange
        Me.probabilityCheck = param.IsElementProbabilityCheck
        Me.isCheckedNum = param.TryTopNmolecularFormulaSearch

        Me.maxFoldInitialize(Me.coverRange, Me.fCheck, Me.clCheck, Me.brCheck, Me.iCheck, Me.siCheck)
    End Sub

    Private Sub maxFoldInitialize(ByVal coverRange As CoverRange, ByVal fCheck As Boolean, ByVal clCheck As Boolean, ByVal brCheck As Boolean, ByVal iCheck As Boolean, ByVal siCheck As Boolean)
        Select Case coverRange
            Case CoverRange.CommonRange
                Me.hMinFold = 0
                Me.hMaxFold = 4.0
                Me.fMaxFold = 1.5
                Me.clMaxFold = 1.0
                Me.brMaxFold = 1.0
                Me.nMaxFold = 2.0
                Me.oMaxFold = 2.5
                Me.pMaxFold = 0.5
                Me.sMaxFold = 1.0
                Me.iMaxFold = 0.5
                Me.siMaxFold = 0.5
            Case CoverRange.ExtendedRange
                Me.hMinFold = 0
                Me.hMaxFold = 6.0
                Me.fMaxFold = 3.0
                Me.clMaxFold = 3.0
                Me.brMaxFold = 2.0
                Me.nMaxFold = 4.0
                Me.oMaxFold = 6.0
                Me.pMaxFold = 1.9
                Me.sMaxFold = 3.0
                Me.iMaxFold = 1.9
                Me.siMaxFold = 1.0
            Case Else
                Me.hMinFold = 0
                Me.hMaxFold = 8.0
                Me.fMaxFold = 4.0
                Me.clMaxFold = 4.0
                Me.brMaxFold = 4.0
                Me.nMaxFold = 4.0
                Me.oMaxFold = 10.0
                Me.pMaxFold = 3.0
                Me.sMaxFold = 6.0
                Me.iMaxFold = 3.0
                Me.siMaxFold = 3.0
        End Select

        Me.maxMassFoldChange = Me.hMaxFold * hMass
        If fCheck Then Me.maxMassFoldChange += Me.fMaxFold * fMass
        If clCheck Then Me.maxMassFoldChange += Me.clMaxFold * clMass
        If brCheck Then Me.maxMassFoldChange += Me.brMaxFold * brMass
        If iCheck Then Me.maxMassFoldChange += Me.iMaxFold * iMass
        If siCheck Then Me.maxMassFoldChange += Me.siMaxFold * siMass
    End Sub


    ''' <summary>
    ''' This is the main method to find the formula candidates.
    ''' MS-FINDER program now utilizes three internal databases including formulaDB, neutralLossDB, and existFormulaDB.
    ''' </summary>
    ''' <param name="formulaDB"></param>
    ''' <param name="neutralLossDB"></param>
    ''' <param name="existFormulaDB"></param>
    ''' <param name="mass"></param>
    ''' <param name="ms2Tol"></param>
    ''' <param name="m1Intensity"></param>
    ''' <param name="m2Intensity"></param>
    ''' <param name="rawData"></param>
    ''' <param name="adductIon"></param>
    ''' <returns></returns>
    Public Function GetFormulaCandidateList(ByVal productIonDB As System.Collections.Generic.List(Of ProductIon), ByVal neutralLossDB As System.Collections.Generic.List(Of NeutralLoss), ByVal existFormulaDB As System.Collections.Generic.List(Of ExistFormulaQuery), ByVal param As AnalysisParamOfMsfinder, ByVal mass As Double, ByVal m1Intensity As Double, ByVal m2Intensity As Double, ByVal rawData As CompMs.Common.DataObj.RawData, ByVal adductIon As AdductIon, ByVal isotopeCheck As Boolean) As System.Collections.Generic.List(Of FormulaResult)

        'param set
        Dim ms1Tol = param.Mass1Tolerance
        Dim ms2Tol = param.Mass2Tolerance
        Dim massTolType = param.MassTolType
        Dim relativeAbundanceCutOff = param.RelativeAbundanceCutOff
        Dim maxReportNumber = param.FormulaMaximumReportNumber

        If massTolType = MassToleranceType.Ppm Then ms1Tol = PPMmethod.ConvertPpmToMassAccuracy(mass, ms1Tol)

        Dim formulaResults = New System.Collections.Generic.List(Of FormulaResult)()
        Dim ms2Peaklist = CompMs.Common.FormulaGenerator.[Function].FragmentAssigner.GetCentroidMsMsSpectrum(rawData)
        Dim refinedPeaklist = CompMs.Common.FormulaGenerator.[Function].FragmentAssigner.GetRefinedPeaklist(ms2Peaklist, relativeAbundanceCutOff, 0.0, (mass * CDbl(adductIon.AdductIonXmer) + adductIon.AdductIonAccurateMass) / CDbl(adductIon.ChargeNumber), ms2Tol, massTolType, 1000, False, Not param.CanExcuteMS2AdductSearch)
        Dim neutralLosslist = CompMs.Common.FormulaGenerator.[Function].FragmentAssigner.GetNeutralLossList(refinedPeaklist, rawData.PrecursorMz, ms1Tol)

        Dim syncObj = New Object()
        'var endID = getFormulaDbLastIndex(formulaDB, mass + ms1Tol);

        formulaResults = Me.getFormulaResults(rawData, param, mass, ms1Tol, m1Intensity, m2Intensity, adductIon, isotopeCheck, maxReportNumber, existFormulaDB, refinedPeaklist, neutralLosslist, productIonDB, neutralLossDB)

#Region "old"
        'Parallel.For(0, endID, i => {

        '    var tempResults = getFormulaSearchResults(formulaDB[i], rawData,
        '        param, mass, ms1Tol,
        '        m1Intensity, m2Intensity, adductIon, isotopeCheck, maxReportNumber,
        '        existFormulaDB, refinedPeaklist, neutralLosslist, productIonDB, neutralLossDB);

        '    if (tempResults != null && tempResults.Count != 0) {
        '        lock (syncObj) {
        '            foreach (var result in tempResults) {
        '                formulaResults = getFormulaResultCandidates(formulaResults, result, 100000000);
        '            }
        '        }
        '    }
        '});
        '''for (int i = 0; i < formulaDB.Count; i++) {
        '''    if (formulaDB[i].Mass + formulaDB[i].Cnum * this.maxMassFoldChange < mass - ms1Tol) continue;
        '''    if (formulaDB[i].Onum > 0 && !this.oCheck) continue;
        '''    if (formulaDB[i].Nnum > 0 && !this.nCheck) continue;
        '''    if (formulaDB[i].Pnum > 0 && !this.pCheck) continue;
        '''    if (formulaDB[i].Snum > 0 && !this.sCheck) continue;

        '''    if (rawData.CarbonNumberFromLabeledExperiment >= 0 && formulaDB[i].Cnum != rawData.CarbonNumberFromLabeledExperiment)
        '''        continue;
        '''    if (rawData.NitrogenNumberFromLabeledExperiment >= 0 && formulaDB[i].Nnum != rawData.NitrogenNumberFromLabeledExperiment)
        '''        continue;
        '''    if (rawData.OxygenNumberFromLabeledExperiment >= 0 && formulaDB[i].Onum != rawData.OxygenNumberFromLabeledExperiment)
        '''        continue;
        '''    if (rawData.SulfurNumberFromLabeledExperiment >= 0 && formulaDB[i].Snum != rawData.SulfurNumberFromLabeledExperiment)
        '''        continue;
        '''    if (rawData.CarbonNitrogenNumberFromLabeledExperiment >= 0 && formulaDB[i].Cnum + formulaDB[i].Nnum != rawData.CarbonNitrogenNumberFromLabeledExperiment)
        '''        continue;
        '''    if (rawData.CarbonSulfurNumberFromLabeledExperiment >= 0 && formulaDB[i].Cnum + formulaDB[i].Snum != rawData.CarbonSulfurNumberFromLabeledExperiment)
        '''        continue;
        '''    if (rawData.NitrogenSulfurNumberFromLabeledExperiment >= 0 && formulaDB[i].Nnum + formulaDB[i].Snum != rawData.NitrogenSulfurNumberFromLabeledExperiment)
        '''        continue;
        '''    if (rawData.CarbonNitrogenSulfurNumberFromLabeledExperiment >= 0 && formulaDB[i].Cnum + formulaDB[i].Nnum + formulaDB[i].Snum != rawData.CarbonNumberFromLabeledExperiment)
        '''        continue;

        '''    if (formulaDB[i].Mass > mass + ms1Tol) break;

        '''    formulaResults = getFormulaSearchResults(formulaResults, formulaDB[i], param, mass, ms1Tol,
        '''        m1Intensity, m2Intensity, adductIon, isotopeCheck, maxReportNumber, 
        '''        existFormulaDB, refinedPeaklist, neutralLosslist, productIonDB, neutralLossDB);
        '''}
#End Region
        If formulaResults.Count > 0 Then
            formulaResults = formulaResults.OrderByDescending(Function(n) System.std.Abs(n.TotalScore)).ToList()

            For i As Integer = 0 To Me.isCheckedNum - 1
                If i > formulaResults.Count - 1 Then Exit For
                formulaResults(CInt((i))).IsSelected = True
            Next

            While formulaResults.Count > maxReportNumber
                formulaResults.RemoveAt(formulaResults.Count - 1)
            End While
        End If

        Return formulaResults
    End Function

    Private Function getFormulaResults(ByVal rawData As CompMs.Common.DataObj.RawData, ByVal param As AnalysisParamOfMsfinder, ByVal mass As Double, ByVal ms1Tol As Double, ByVal m1Intensity As Double, ByVal m2Intensity As Double, ByVal adduct As AdductIon, ByVal isotopeCheck As Boolean, ByVal maxReportNumber As Integer, ByVal existFormulaDB As System.Collections.Generic.List(Of ExistFormulaQuery), ByVal refinedPeaklist As System.Collections.Generic.List(Of SpectrumPeak), ByVal neutralLosses As System.Collections.Generic.List(Of NeutralLoss), ByVal productIonDB As System.Collections.Generic.List(Of ProductIon), ByVal neutralLossDB As System.Collections.Generic.List(Of NeutralLoss)) As System.Collections.Generic.List(Of FormulaResult)


        Dim maxCnum = CInt((mass / 12.0))
        Dim formulaResultsMaster = New System.Collections.Generic.List(Of FormulaResult)()

        Dim syncObj = New Object()
        Dim sw = New System.Diagnostics.Stopwatch()
        sw.Start()

        Call System.Threading.Tasks.Parallel.[For](1, maxCnum, Sub(c, state)
                                                                   Dim formulaResults = Me.getFormulaResults(c, rawData, param, mass, ms1Tol, m1Intensity, m2Intensity, adduct, isotopeCheck, maxReportNumber, existFormulaDB, refinedPeaklist, neutralLosses, productIonDB, neutralLossDB)

                                                                   If formulaResults IsNot Nothing AndAlso formulaResults.Count <> 0 Then
                                                                       SyncLock syncObj
                                                                           Dim lap = sw.ElapsedMilliseconds * 0.001 / 60.0 ' min
                                                                           If param.FormulaPredictionTimeOut > 0 AndAlso param.FormulaPredictionTimeOut < lap Then
                                                                               Call System.Diagnostics.Debug.WriteLine("Calculation stopped.")
                                                                               state.[Stop]()
                                                                           End If

                                                                           For Each result In formulaResults
                                                                               formulaResultsMaster = Me.getFormulaResultCandidates(formulaResultsMaster, result, 100000000)
                                                                           Next
                                                                       End SyncLock
                                                                   End If
                                                               End Sub)
        sw.[Stop]()
        Return formulaResultsMaster
    End Function

    Private Function getFormulaResults(ByVal c As Integer, ByVal rawData As CompMs.Common.DataObj.RawData, ByVal param As AnalysisParamOfMsfinder, ByVal mass As Double, ByVal ms1Tol As Double, ByVal m1Intensity As Double, ByVal m2Intensity As Double, ByVal adduct As AdductIon, ByVal isotopeCheck As Boolean, ByVal maxReportNumber As Integer, ByVal existFormulaDB As System.Collections.Generic.List(Of ExistFormulaQuery), ByVal refinedPeaklist As System.Collections.Generic.List(Of SpectrumPeak), ByVal neutralLosses As System.Collections.Generic.List(Of NeutralLoss), ByVal productIonDB As System.Collections.Generic.List(Of ProductIon), ByVal neutralLossDB As System.Collections.Generic.List(Of NeutralLoss)) As System.Collections.Generic.List(Of FormulaResult)



        Dim formulaResults = New System.Collections.Generic.List(Of FormulaResult)()
        Dim xMer = adduct.AdductIonXmer
        If rawData.CarbonNumberFromLabeledExperiment >= 0 AndAlso c <> rawData.CarbonNumberFromLabeledExperiment / xMer Then Return formulaResults

        Dim maxHnum = CInt(System.Math.Ceiling(c * Me.hMaxFold)) ' 1.00782503207
        Dim maxNnum = If(Me.nCheck, CInt(System.Math.Ceiling(c * Me.nMaxFold)), 0) ' 14.00307400480
        Dim maxOnum = If(Me.oCheck, CInt(System.Math.Ceiling(c * Me.oMaxFold)), 0) ' 15.99491461956
        Dim maxPnum = If(Me.pCheck, CInt(System.Math.Ceiling(c * Me.pMaxFold)), 0) ' 30.97376163000
        Dim maxSnum = If(Me.sCheck, CInt(System.Math.Ceiling(c * Me.sMaxFold)), 0) ' 31.97207100000

        Dim maxFnum = If(Me.fCheck, CInt(System.Math.Ceiling(c * Me.fMaxFold)), 0) ' 18.99840322000;
        Dim maxSinum = If(Me.siCheck, CInt(System.Math.Ceiling(c * Me.siMaxFold)), 0) ' 27.97692653250;
        Dim maxClnum = If(Me.clCheck, CInt(System.Math.Ceiling(c * Me.clMaxFold)), 0) ' 34.96885268000;
        Dim maxBrnum = If(Me.brCheck, CInt(System.Math.Ceiling(c * Me.brMaxFold)), 0) ' 78.91833710000;
        Dim maxInum = If(Me.iCheck, CInt(System.Math.Ceiling(c * Me.iMaxFold)), 0) ' 126.90447300000;


        'for lool is used by mass weight order: I > Br > Cl > S > P > Si > F > O > N > H
        Dim maxHmass = maxHnum * hMass
        Dim maxNHmass = maxHmass + maxNnum * nMass
        Dim maxONHmass = maxNHmass + maxOnum * oMass
        Dim maxFONHmass = maxONHmass + maxFnum * fMass
        Dim maxSiFONHmass = maxFONHmass + maxSinum * siMass
        Dim maxPSiFONHmass = maxSiFONHmass + maxPnum * pMass
        Dim maxSPSiFONHmass = maxPSiFONHmass + maxSnum * sMass
        Dim maxClSPSiFONHmass = maxSPSiFONHmass + maxClnum * clMass
        Dim maxBrClSPSiFONHmass = maxClSPSiFONHmass + maxBrnum * brMass

        Dim sw = New System.Diagnostics.Stopwatch()
        sw.Start()
        For i As Integer = 0 To maxInum
            Dim ciMass = CDbl(c) * cMass + CDbl(i) * iMass
            If ciMass + maxBrClSPSiFONHmass < mass - ms1Tol Then Continue For
            If ciMass > mass + ms1Tol Then Exit For

            For br As Integer = 0 To maxBrnum
                Dim cibrMass = ciMass + CDbl(br) * brMass
                If cibrMass + maxClSPSiFONHmass < mass - ms1Tol Then Continue For
                If cibrMass > mass + ms1Tol Then Exit For

                For cl As Integer = 0 To maxClnum
                    Dim cibrclMass = cibrMass + CDbl(cl) * clMass
                    If cibrclMass + maxSPSiFONHmass < mass - ms1Tol Then Continue For
                    If cibrclMass > mass + ms1Tol Then Exit For

                    For s As Integer = 0 To maxSnum

                        If rawData.SulfurNumberFromLabeledExperiment >= 0 AndAlso s <> rawData.SulfurNumberFromLabeledExperiment / xMer Then Continue For

                        Dim cibrclsMass = cibrclMass + CDbl(s) * sMass
                        If cibrclsMass + maxPSiFONHmass < mass - ms1Tol Then Continue For
                        If cibrclsMass > mass + ms1Tol Then Exit For

                        For p As Integer = 0 To maxPnum
                            Dim cibrclspMass = cibrclsMass + CDbl(p) * pMass
                            If cibrclspMass + maxSiFONHmass < mass - ms1Tol Then Continue For
                            If cibrclspMass > mass + ms1Tol Then Exit For

                            For si As Integer = 0 To maxSinum
                                If param.IsTmsMeoxDerivative AndAlso si < param.MinimumTmsCount Then Continue For

                                Dim cibrclspsiMass = cibrclspMass + CDbl(si) * siMass
                                If cibrclspsiMass + maxFONHmass < mass - ms1Tol Then Continue For
                                If cibrclspsiMass > mass + ms1Tol Then Exit For

                                For f As Integer = 0 To maxFnum
                                    Dim cibrclspsifMass = cibrclspsiMass + CDbl(f) * fMass
                                    If cibrclspsifMass + maxONHmass < mass - ms1Tol Then Continue For
                                    If cibrclspsifMass > mass + ms1Tol Then Exit For

                                    For o As Integer = 0 To maxOnum

                                        If rawData.OxygenNumberFromLabeledExperiment >= 0 AndAlso o <> rawData.OxygenNumberFromLabeledExperiment / xMer Then Continue For

                                        Dim cibrclspsifoMass = cibrclspsifMass + CDbl(o) * oMass
                                        If cibrclspsifoMass + maxNHmass < mass - ms1Tol Then Continue For
                                        If cibrclspsifoMass > mass + ms1Tol Then Exit For

                                        For n As Integer = 0 To maxNnum

                                            If rawData.NitrogenNumberFromLabeledExperiment >= 0 AndAlso n <> rawData.NitrogenNumberFromLabeledExperiment / xMer Then Continue For

                                            Dim cibrclspsifonMass = cibrclspsifoMass + CDbl(n) * nMass
                                            If cibrclspsifonMass + maxHmass < mass - ms1Tol Then Continue For
                                            If cibrclspsifonMass > mass + ms1Tol Then Exit For

                                            For h As Integer = 0 To maxHnum
                                                Dim cibrclspsifonhMass = cibrclspsifonMass + CDbl(h) * hMass
                                                If cibrclspsifonhMass < mass - ms1Tol Then Continue For
                                                If cibrclspsifonhMass > mass + ms1Tol Then Exit For

                                                If param.IsTmsMeoxDerivative Then
                                                    For meox As Integer = param.MinimumMeoxCount To n
                                                        Dim formula = New Formula(c, h, n, o, p, s, f, cl, br, i, si, si, meox)
                                                        If formula!C - si * 3 - meox <= 0 Then Continue For

                                                        Dim convertedFormula = FormulaCalculateUtility.ConvertTmsMeoxSubtractedFormula(formula)
                                                        If Not SevenGoldenRulesCheck.Check(convertedFormula, Me.valenceCheck, Me.coverRange, Me.probabilityCheck, adduct) Then Continue For
                                                        Dim formulaResult = Me.tryGetFormulaResultCandidate(formula, param, mass, ms1Tol, m1Intensity, m2Intensity, isotopeCheck, adduct, refinedPeaklist, neutralLosses, existFormulaDB, productIonDB, neutralLossDB)
                                                        If formulaResult IsNot Nothing Then formulaResults.Add(formulaResult)
                                                    Next
                                                Else
                                                    Dim formula = New Formula(c, h, n, o, p, s, f, cl, br, i, si)
                                                    Dim formulaResult = Me.tryGetFormulaResultCandidate(formula, param, mass, ms1Tol, m1Intensity, m2Intensity, isotopeCheck, adduct, refinedPeaklist, neutralLosses, existFormulaDB, productIonDB, neutralLossDB)
                                                    If formulaResult IsNot Nothing Then formulaResults.Add(formulaResult)
                                                End If

                                                Dim lap = sw.ElapsedMilliseconds * 0.001 / 60.0 ' min
                                                If param.FormulaPredictionTimeOut > 0 AndAlso param.FormulaPredictionTimeOut < lap Then
                                                    Call System.Diagnostics.Debug.WriteLine("Calculation stopped.")
                                                    sw.[Stop]()
                                                    Return formulaResults
                                                End If
                                            Next
                                        Next
                                    Next
                                Next
                            Next
                        Next
                    Next
                Next
            Next
        Next

        sw.[Stop]()
        Return formulaResults
    End Function

    Private Function getFormulaSearchResults(ByVal formula As Formula, ByVal rawData As CompMs.Common.DataObj.RawData, ByVal param As AnalysisParamOfMsfinder, ByVal mass As Double, ByVal ms1Tol As Double, ByVal m1Intensity As Double, ByVal m2Intensity As Double, ByVal adductIon As AdductIon, ByVal isotopeCheck As Boolean, ByVal maxReportNumber As Integer, ByVal existFormulaDB As System.Collections.Generic.List(Of ExistFormulaQuery), ByVal refinedPeaklist As System.Collections.Generic.List(Of SpectrumPeak), ByVal neutralLosslist As System.Collections.Generic.List(Of NeutralLoss), ByVal productIonDB As System.Collections.Generic.List(Of ProductIon), ByVal neutralLossDB As System.Collections.Generic.List(Of NeutralLoss)) As System.Collections.Generic.List(Of FormulaResult)


        If formula.ExactMass + formula!C * Me.maxMassFoldChange < mass - ms1Tol Then Return Nothing
        If formula!O > 0 AndAlso Not Me.oCheck Then Return Nothing
        If formula!N > 0 AndAlso Not Me.nCheck Then Return Nothing
        If formula!P > 0 AndAlso Not Me.pCheck Then Return Nothing
        If formula!S > 0 AndAlso Not Me.sCheck Then Return Nothing

        Dim xMer = adductIon.AdductIonXmer
        If rawData.CarbonNumberFromLabeledExperiment >= 0 AndAlso formula!C <> rawData.CarbonNumberFromLabeledExperiment / xMer Then Return Nothing
        If rawData.NitrogenNumberFromLabeledExperiment >= 0 AndAlso formula!N <> rawData.NitrogenNumberFromLabeledExperiment / xMer Then Return Nothing
        If rawData.OxygenNumberFromLabeledExperiment >= 0 AndAlso formula!O <> rawData.OxygenNumberFromLabeledExperiment / xMer Then Return Nothing
        If rawData.SulfurNumberFromLabeledExperiment >= 0 AndAlso formula!S <> rawData.SulfurNumberFromLabeledExperiment / xMer Then Return Nothing
        If rawData.CarbonNitrogenNumberFromLabeledExperiment >= 0 AndAlso formula!C + formula!N <> rawData.CarbonNitrogenNumberFromLabeledExperiment / xMer Then Return Nothing
        If rawData.CarbonSulfurNumberFromLabeledExperiment >= 0 AndAlso formula!C + formula!S <> rawData.CarbonSulfurNumberFromLabeledExperiment / xMer Then Return Nothing
        If rawData.NitrogenSulfurNumberFromLabeledExperiment >= 0 AndAlso formula!N + formula!S <> rawData.NitrogenSulfurNumberFromLabeledExperiment / xMer Then Return Nothing
        If rawData.CarbonNitrogenSulfurNumberFromLabeledExperiment >= 0 AndAlso formula!C + formula!N + formula!S <> rawData.CarbonNumberFromLabeledExperiment / xMer Then Return Nothing

        '''test for ms-dial and ms-finder integration project
        'if (FormulaStringParcer.OrganicElementsReader(formula.FormulaString).Cnum != formula.Cnum) return null;


        Dim tempResults = Me.getFormulaSearchResults(formula, param, mass, ms1Tol, m1Intensity, m2Intensity, adductIon, isotopeCheck, maxReportNumber, existFormulaDB, refinedPeaklist, neutralLosslist, productIonDB, neutralLossDB)
        Return tempResults
    End Function

    'private int getFormulaDbLastIndex(List<Formula> formulaDB, double targetMass) {
    '    int startIndex = 0, endIndex = formulaDB.Count - 1;

    '    int counter = 0;
    '    while (counter < 10) {
    '        if (formulaDB[startIndex].Mass <= targetMass && 
    '            targetMass < formulaDB[(startIndex + endIndex) / 2].Mass) {
    '            endIndex = (startIndex + endIndex) / 2;
    '        }
    '        else if (formulaDB[(startIndex + endIndex) / 2].Mass <= targetMass && 
    '            targetMass < formulaDB[endIndex].Mass) {
    '            startIndex = (startIndex + endIndex) / 2;
    '        }
    '        counter++;
    '    }

    '    return endIndex;
    '}

    ''' <summary>
    ''' This is the main method to find the formula candidates.
    ''' MS-FINDER program now utilizes three internal databases including formulaDB, neutralLossDB, and existFormulaDB.
    ''' </summary>
    ''' <param name="formulaDB"></param>
    ''' <param name="existFormulaDB"></param>
    ''' <param name="mass"></param>
    ''' <param name="m1Intensity"></param>
    ''' <param name="m2Intensity"></param>
    ''' <param name="adductIon"></param>
    ''' <param name="isotopeCheck"></param>
    ''' <returns></returns>
    'public List<FormulaResult> GetFormulaCandidateList(List<Formula> formulaDB, List<ExistFormulaQuery> existFormulaDB, AnalysisParamOfMsfinder param, double mass, double ms1Tol, double ms2Tol, MassToleranceType massTolType, double m1Intensity, double m2Intensity, double isotopeRatioTolAsPercentage, int maxReportNumber, AdductIon adductIon, bool isotopeCheck) {
    'public List<FormulaResult> GetFormulaCandidateList(RawData rawData, List<Formula> formulaDB, List<ExistFormulaQuery> existFormulaDB,
    Public Function GetFormulaCandidateList(ByVal rawData As CompMs.Common.DataObj.RawData, ByVal existFormulaDB As System.Collections.Generic.List(Of ExistFormulaQuery), ByVal param As AnalysisParamOfMsfinder, ByVal mass As Double, ByVal m1Intensity As Double, ByVal m2Intensity As Double, ByVal adductIon As AdductIon, ByVal isotopeCheck As Boolean) As System.Collections.Generic.List(Of FormulaResult)

        'param set
        Dim ms1Tol = param.Mass1Tolerance
        Dim massTolType = param.MassTolType
        Dim maxReportNumber = param.FormulaMaximumReportNumber

        Dim formulaResults = New System.Collections.Generic.List(Of FormulaResult)()
        If massTolType = MassToleranceType.Ppm Then ms1Tol = PPMmethod.ConvertPpmToMassAccuracy(mass, ms1Tol)

        formulaResults = Me.getFormulaResults(rawData, param, mass, ms1Tol, m1Intensity, m2Intensity, adductIon, isotopeCheck, maxReportNumber, existFormulaDB, Nothing, Nothing, Nothing, Nothing)


        'for (int i = 0; i < formulaDB.Count; i++) {
        '    if (formulaDB[i].Mass + formulaDB[i].Cnum * this.maxMassFoldChange < mass - ms1Tol) continue;
        '    if (formulaDB[i].Onum > 0 && !this.oCheck) continue;
        '    if (formulaDB[i].Nnum > 0 && !this.nCheck) continue;
        '    if (formulaDB[i].Pnum > 0 && !this.pCheck) continue;
        '    if (formulaDB[i].Snum > 0 && !this.sCheck) continue;

        '    if (formulaDB[i].Mass > mass + ms1Tol) break;

        '    //formulaResults = getFormulaSearchResults(formulaResults, formulaDB[i], param, mass, ms1Tol, m1Intensity, m2Intensity, adductIon, isotopeCheck, maxReportNumber, existFormulaDB, null, null, null, null);
        '    var tempResults = getFormulaSearchResults(formulaDB[i], param, mass, ms1Tol, m1Intensity, m2Intensity, adductIon, isotopeCheck, maxReportNumber, existFormulaDB, null, null, null, null);
        '    if (tempResults == null || tempResults.Count == 0) continue;
        '    foreach (var result in tempResults) {
        '        formulaResults.Add(result);
        '    }
        '}

        If formulaResults.Count > 0 Then
            formulaResults = formulaResults.OrderByDescending(Function(n) System.std.Abs(n.TotalScore)).ToList()

            For i As Integer = 0 To Me.isCheckedNum - 1
                If i > formulaResults.Count - 1 Then Exit For
                formulaResults(CInt((i))).IsSelected = True
            Next

            While formulaResults.Count > maxReportNumber
                formulaResults.RemoveAt(formulaResults.Count - 1)
            End While
        End If

        Return formulaResults
    End Function

    'public FormulaResult GetFormulaScore(string formulaString, List<NeutralLoss> neutralLossDB, List<ExistFormulaQuery> existFormulaDB, double mass, AnalysisParamOfMsfinder param, double ms1Tol, double ms2Tol, MassToleranceType massTolType, double relativeAbundanceCutOff, double subtractedM1Intensity, double subtractedM2Intensity, double isotopeRatioTolAsPercentage, RawData rawData, AdductIon adductIon, bool isotopeCheck) {
    Public Function GetFormulaScore(ByVal formulaString As String, ByVal productIonDB As System.Collections.Generic.List(Of ProductIon), ByVal neutralLossDB As System.Collections.Generic.List(Of NeutralLoss), ByVal existFormulaDB As System.Collections.Generic.List(Of ExistFormulaQuery), ByVal param As AnalysisParamOfMsfinder, ByVal mass As Double, ByVal subtractedM1Intensity As Double, ByVal subtractedM2Intensity As Double, ByVal rawData As CompMs.Common.DataObj.RawData, ByVal adductIon As AdductIon, ByVal isotopeCheck As Boolean) As FormulaResult

        'param set
        Dim ms1Tol = param.Mass1Tolerance
        Dim ms2Tol = param.Mass2Tolerance
        Dim massTolType = param.MassTolType
        Dim isotopicAbundanceTolerance = param.IsotopicAbundanceTolerance
        Dim relativeAbundanceCutOff = param.RelativeAbundanceCutOff

        Dim formula = FormulaStringParcer.OrganicElementsReader(formulaString)
        If massTolType = MassToleranceType.Ppm Then ms1Tol = PPMmethod.ConvertPpmToMassAccuracy(mass, ms1Tol)
        Dim formulaResult = Me.getFormulaResult(formula, param, mass, ms1Tol, subtractedM1Intensity, subtractedM2Intensity, isotopeCheck, existFormulaDB)

        Dim ms2Peaklist = CompMs.Common.FormulaGenerator.[Function].FragmentAssigner.GetCentroidMsMsSpectrum(rawData)

        If ms2Peaklist IsNot Nothing AndAlso ms2Peaklist.Count <> 0 Then
            Dim refinedPeaklist = CompMs.Common.FormulaGenerator.[Function].FragmentAssigner.GetRefinedPeaklist(ms2Peaklist, relativeAbundanceCutOff, 0.0, (mass * CDbl(adductIon.AdductIonXmer) + adductIon.AdductIonAccurateMass) / CDbl(adductIon.ChargeNumber), ms2Tol, massTolType, 1000, False, Not param.CanExcuteMS2AdductSearch)
            Dim neutralLosslist = CompMs.Common.FormulaGenerator.[Function].FragmentAssigner.GetNeutralLossList(refinedPeaklist, rawData.PrecursorMz, ms1Tol)

            If param.CanExcuteMS2AdductSearch Then
                If adductIon.IonMode = IonModes.Positive Then
                    formulaResult.AnnotatedIonResult = CompMs.Common.FormulaGenerator.[Function].FragmentAssigner.GetAnnotatedIon(refinedPeaklist, adductIon, param.MS2PositiveAdductIonList, rawData.PrecursorMz, param.Mass2Tolerance, param.MassTolType)
                Else
                    formulaResult.AnnotatedIonResult = CompMs.Common.FormulaGenerator.[Function].FragmentAssigner.GetAnnotatedIon(refinedPeaklist, adductIon, param.MS2NegativeAdductIonList, rawData.PrecursorMz, param.Mass2Tolerance, param.MassTolType)
                End If
                refinedPeaklist = CompMs.Common.FormulaGenerator.[Function].FragmentAssigner.GetRefinedPeaklist(refinedPeaklist, rawData.PrecursorMz)
            End If
            Me.setFragmentProperties(formulaResult, refinedPeaklist, neutralLosslist, productIonDB, neutralLossDB, ms2Tol, massTolType, adductIon)
        End If

        formulaResult.TotalScore = System.Math.Round(CompMs.Common.FormulaGenerator.[Function].Scoring.TotalScore(formulaResult), 3)
        formulaResult.IsSelected = True

        Return formulaResult
    End Function

    Private Sub setExistFormulaDbInfo(ByVal formulaResult As FormulaResult, ByVal existFormulaDB As System.Collections.Generic.List(Of ExistFormulaQuery))
        Dim resourceNames As String
        Dim resourceRecords As Integer
        Dim pubchemCids As System.Collections.Generic.List(Of Integer)

        Me.tryExistFormulaDbSearch(formulaResult.Formula, existFormulaDB, resourceNames, resourceRecords, pubchemCids)
        formulaResult.ResourceNames = resourceNames
        formulaResult.ResourceRecords = resourceRecords
        formulaResult.PubchemResources = pubchemCids
    End Sub

    Private Sub tryExistFormulaDbSearch(ByVal formula As Formula, ByVal queryDB As System.Collections.Generic.List(Of ExistFormulaQuery), <Out> ByRef resourceNames As String, <Out> ByRef resourceRecords As Integer, <Out> ByRef pubchemCIDs As System.Collections.Generic.List(Of Integer))
        pubchemCIDs = New System.Collections.Generic.List(Of Integer)()
        resourceNames = String.Empty
        resourceRecords = 0

        Dim cFormula = FormulaCalculateUtility.ConvertTmsMeoxSubtractedFormula(formula)
        Dim mass = cFormula.Mass
        Dim tol = 0.00005
        Dim startID = Me.getQueryStartIndex(mass, tol, queryDB)

        For i As Integer = startID To queryDB.Count - 1
            If queryDB(CInt((i))).Formula.Mass < mass - tol Then Continue For
            If queryDB(CInt((i))).Formula.Mass > mass + tol Then Exit For

            Dim qFormula = queryDB(CInt((i))).Formula

            If cFormula.EqualsTo(qFormula) Then
                resourceNames = queryDB(CInt((i))).ResourceNames
                resourceRecords = queryDB(CInt((i))).ResourceNumber
                pubchemCIDs = queryDB(CInt((i))).PubchemCidList
                Exit For
            End If
        Next
    End Sub

    Private Function getQueryStartIndex(ByVal mass As Double, ByVal tol As Double, ByVal queryDB As System.Collections.Generic.List(Of ExistFormulaQuery)) As Integer
        If queryDB Is Nothing OrElse queryDB.Count = 0 Then Return 0
        Dim targetMass As Double = mass - tol
        Dim startIndex As Integer = 0, endIndex As Integer = queryDB.Count - 1
        Dim counter As Integer = 0

        While counter < 10
            If queryDB(CInt((startIndex))).Formula.Mass <= targetMass AndAlso targetMass < queryDB(CInt(((startIndex + endIndex) / 2))).Formula.Mass Then
                endIndex = (startIndex + endIndex) / 2
            ElseIf queryDB(CInt(((startIndex + endIndex) / 2))).Formula.Mass <= targetMass AndAlso targetMass < queryDB(CInt((endIndex))).Formula.Mass Then
                startIndex = (startIndex + endIndex) / 2
            End If
            counter += 1
        End While
        Return startIndex
    End Function

    Private Sub setFragmentProperties(ByVal formulaResult As FormulaResult, ByVal refinedPeaklist As System.Collections.Generic.List(Of SpectrumPeak), ByVal neutralLosslist As System.Collections.Generic.List(Of NeutralLoss), ByVal productIonDB As System.Collections.Generic.List(Of ProductIon), ByVal neutralLossDB As System.Collections.Generic.List(Of NeutralLoss), ByVal ms2Tol As Double, ByVal massTolType As MassToleranceType, ByVal adductIon As AdductIon)
        If refinedPeaklist Is Nothing OrElse neutralLosslist Is Nothing Then Return

        formulaResult.ProductIonResult = CompMs.Common.FormulaGenerator.[Function].FragmentAssigner.FastFragmnetAssigner(refinedPeaklist, productIonDB, formulaResult.Formula, ms2Tol, massTolType, adductIon)
        formulaResult.NeutralLossResult = CompMs.Common.FormulaGenerator.[Function].FragmentAssigner.FastNeutralLossAssigner(neutralLosslist, neutralLossDB, formulaResult.Formula, ms2Tol, massTolType, adductIon)

        formulaResult.ProductIonNum = formulaResult.ProductIonResult.Count
        formulaResult.ProductIonHits = formulaResult.ProductIonResult.Where(Function(n) n.CandidateOntologies.Count > 0).Count

        formulaResult.NeutralLossHits = Me.getUniqueNeutralLossCount(formulaResult.NeutralLossResult)
        formulaResult.NeutralLossNum = Me.getUniqueNeutralLossCountByMass(neutralLosslist, ms2Tol, massTolType)

        formulaResult.ProductIonScore = System.Math.Round(CompMs.Common.FormulaGenerator.[Function].Scoring.FragmentHitsScore(refinedPeaklist, formulaResult.ProductIonResult, ms2Tol, massTolType), 3)
        formulaResult.NeutralLossScore = System.Math.Round(CompMs.Common.FormulaGenerator.[Function].Scoring.NeutralLossScore(formulaResult.NeutralLossHits, formulaResult.NeutralLossNum), 3)
    End Sub


    Private Function getUniqueNeutralLossCount(ByVal neutralLosses As System.Collections.Generic.List(Of NeutralLoss)) As Integer

        If neutralLosses.Count = 0 Then Return 0

        Dim formulas = New List(Of Formula)() From {
                neutralLosses(CInt((0))).Formula
            }
        If neutralLosses.Count = 1 Then Return 1

        For i As Integer = 1 To neutralLosses.Count - 1
            Dim lossFormula = neutralLosses(CInt((i))).Formula

            Dim flg = False
            For Each formula In formulas
                If formula.EqualsTo(lossFormula, True) Then
                    flg = True
                    Exit For
                End If
            Next
            If Not flg Then
                formulas.Add(lossFormula)
            End If
        Next

        Return formulas.Count
    End Function

    Private Function getUniqueNeutralLossCountByMass(ByVal neutralLosses As List(Of NeutralLoss), ByVal ms2Tol As Double, ByVal massTolType As MassToleranceType) As Integer

        If neutralLosses.Count = 0 Then Return 0

        Dim masses = New List(Of Double)() From {
                neutralLosses(CInt((0))).MassLoss
            }
        If neutralLosses.Count = 1 Then Return 1

        For i As Integer = 1 To neutralLosses.Count - 1
            Dim lossMass = neutralLosses(CInt((i))).MassLoss
            Dim massTol = ms2Tol

            If massTolType = MassToleranceType.Ppm Then massTol = PPMmethod.ConvertPpmToMassAccuracy(neutralLosses(CInt((i))).PrecursorMz, ms2Tol)
            Dim flg = False
            For Each mass In masses
                If System.std.Abs(mass - lossMass) < massTol Then
                    flg = True
                    Exit For
                End If
            Next
            If Not flg Then
                masses.Add(lossMass)
            End If
        Next
        Return masses.Count
    End Function

    'private List<FormulaResult> getFormulaSearchResults(List<FormulaResult> formulaCandidate, Formula formulaBean, AnalysisParamOfMsfinder param, 
    '    double mass, double ms1Tol, double m1Intensity, double m2Intensity, AdductIon adduct, bool isotopeCheck, int maxFormulaNum, 
    '    List<ExistFormulaQuery> existFormulaDB, List<Peak> refinedPeaklist, List<NeutralLoss> neutralLosses, List<ProductIon> productIonDB, List<NeutralLoss> neutralLossDB){

    '    if (param.IsTmsMeoxDerivative && formulaBean.Nnum < param.MinimumMeoxCount) return formulaCandidate;

    '    int minHnum = (int)(this.hMinFold * formulaBean.Cnum);
    '    int maxHnum = (int)(this.hMaxFold * formulaBean.Cnum);

    '    int maxFnum = (int)(this.fMaxFold * formulaBean.Cnum);
    '    int maxClnum = (int)(this.clMaxFold * formulaBean.Cnum);
    '    int maxBrnum = (int)(this.brMaxFold * formulaBean.Cnum);
    '    int maxInum = (int)(this.iMaxFold * formulaBean.Cnum);
    '    int maxSinum = (int)(this.siMaxFold * formulaBean.Cnum);

    '    if (!this.fCheck) maxFnum = 0;
    '    if (!this.clCheck) maxClnum = 0;
    '    if (!this.brCheck) maxBrnum = 0;
    '    if (!this.iCheck) maxInum = 0;
    '    if (!this.siCheck) maxSinum = 0;

    '    double maxBrClSiFHmass = maxBrnum * brMass + maxClnum * clMass + maxSinum * siMass + maxFnum * fMass + maxHnum * hMass;
    '    double maxClSiFHmass = maxClnum * clMass + maxSinum * siMass + maxFnum * fMass + maxHnum * hMass;
    '    double maxSiFHmass = maxSinum * siMass + maxFnum * fMass + maxHnum * hMass;
    '    double maxFHmass = maxFnum * fMass + maxHnum * hMass;
    '    double maxHmass = maxHnum * hMass;

    '    var formulaResult = new FormulaResult();

    '    #region
    '    for (int h = 0; h <= maxInum; h++) {
    '        if (formulaBean.Mass + (double)h * iMass + maxBrClSiFHmass < mass - ms1Tol) continue;
    '        if (formulaBean.Mass + (double)h * iMass > mass + ms1Tol) break;

    '        for (int i = 0; i <= maxBrnum; i++) {
    '            if (formulaBean.Mass + (double)h * iMass + (double)i * brMass + maxClSiFHmass < mass - ms1Tol) continue;
    '            if (formulaBean.Mass + (double)h * iMass + (double)i * brMass > mass + ms1Tol) break;

    '            for (int j = 0; j <= maxClnum; j++) {
    '                if (formulaBean.Mass + (double)h * iMass + (double)i * brMass + (double)j * clMass + maxSiFHmass < mass - ms1Tol) continue;
    '                if (formulaBean.Mass + (double)h * iMass + (double)i * brMass + (double)j * clMass > mass + ms1Tol) break;

    '                for (int k = 0; k <= maxSinum; k++) {
    '                    if (param.IsTmsMeoxDerivative && k < param.MinimumTmsCount) continue;
    '                    if (formulaBean.Mass + (double)h * iMass + (double)i * brMass + (double)j * clMass + (double)k * siMass + maxFHmass < mass - ms1Tol) continue;
    '                    if (formulaBean.Mass + (double)h * iMass + (double)i * brMass + (double)j * clMass + (double)k * siMass > mass + ms1Tol) break;

    '                    for (int l = 0; l <= maxFnum; l++) {
    '                        if (formulaBean.Mass + (double)h * iMass + (double)i * brMass + (double)j * clMass + (double)k * siMass + (double)l * fMass + maxHmass < mass - ms1Tol) continue;
    '                        if (formulaBean.Mass + (double)h * iMass + (double)i * brMass + (double)j * clMass + (double)k * siMass + (double)l * fMass > mass + ms1Tol) break;

    '                        for (int m = minHnum; m <= maxHnum; m++) {
    '                            if (formulaBean.Mass + (double)h * iMass + (double)i * brMass + (double)j * clMass + (double)k * siMass + (double)l * fMass + (double)m * hMass < mass - ms1Tol) continue;
    '                            if (formulaBean.Mass + (double)h * iMass + (double)i * brMass + (double)j * clMass + (double)k * siMass + (double)l * fMass + (double)m * hMass > mass + ms1Tol) break;

    '                            if (param.IsTmsMeoxDerivative) {

    '                                var tmsCount = k;
    '                                for (int n = param.MinimumMeoxCount; n <= formulaBean.Nnum; n++) {
    '                                    var meoxCount = n;
    '                                    var formula = getCandidateFormulaBean(formulaBean, m, l, j, i, h, k, tmsCount, meoxCount);

    '                                    if (formula.Cnum - tmsCount * 3 - meoxCount <= 0) continue;
    '                                    var convertedFormula = MolecularFormulaUtility.ConvertTmsMeoxSubtractedFormula(formula);
    '                                    if (!SevenGoldenRulesCheck.Check(convertedFormula, this.valenceCheck, this.coverRange, this.probabilityCheck, adduct)) continue;

    '                                    formulaCandidate = tryGetFormulaResultCandidate(formulaCandidate, formula, param,
    '                                        mass, ms1Tol, m1Intensity, m2Intensity, isotopeCheck, adduct,
    '                                        refinedPeaklist, neutralLosses, existFormulaDB, productIonDB, neutralLossDB);
    '                                }
    '                            }
    '                            else {

    '                                var formula = getCandidateFormulaBean(formulaBean, m, l, j, i, h, k);

    '                                formulaCandidate = tryGetFormulaResultCandidate(formulaCandidate, formula, param,
    '                                        mass, ms1Tol, m1Intensity, m2Intensity, isotopeCheck, adduct,
    '                                        refinedPeaklist, neutralLosses, existFormulaDB, productIonDB, neutralLossDB);
    '                            }
    '                        }
    '                    }
    '                }
    '            }
    '        }
    '    }
    '    #endregion
    '    return formulaCandidate;
    '}

    Private Function getFormulaSearchResults(ByVal formulaBean As Formula, ByVal param As AnalysisParamOfMsfinder, ByVal mass As Double, ByVal ms1Tol As Double, ByVal m1Intensity As Double, ByVal m2Intensity As Double, ByVal adduct As AdductIon, ByVal isotopeCheck As Boolean, ByVal maxFormulaNum As Integer, ByVal existFormulaDB As System.Collections.Generic.List(Of ExistFormulaQuery), ByVal refinedPeaklist As System.Collections.Generic.List(Of SpectrumPeak), ByVal neutralLosses As System.Collections.Generic.List(Of NeutralLoss), ByVal productIonDB As System.Collections.Generic.List(Of ProductIon), ByVal neutralLossDB As System.Collections.Generic.List(Of NeutralLoss)) As System.Collections.Generic.List(Of FormulaResult)

        If param.IsTmsMeoxDerivative AndAlso formulaBean!N < param.MinimumMeoxCount Then Return New System.Collections.Generic.List(Of FormulaResult)()

        Dim minHnum As Integer = CInt((Me.hMinFold * formulaBean!C))
        Dim maxHnum As Integer = CInt((Me.hMaxFold * formulaBean!C))

        Dim maxFnum As Integer = CInt((Me.fMaxFold * formulaBean!C))
        Dim maxClnum As Integer = CInt((Me.clMaxFold * formulaBean!C))
        Dim maxBrnum As Integer = CInt((Me.brMaxFold * formulaBean!C))
        Dim maxInum As Integer = CInt((Me.iMaxFold * formulaBean!C))
        Dim maxSinum As Integer = CInt((Me.siMaxFold * formulaBean!C))

        If Not Me.fCheck Then maxFnum = 0
        If Not Me.clCheck Then maxClnum = 0
        If Not Me.brCheck Then maxBrnum = 0
        If Not Me.iCheck Then maxInum = 0
        If Not Me.siCheck Then maxSinum = 0

        Dim maxBrClSiFHmass As Double = maxBrnum * brMass + maxClnum * clMass + maxSinum * siMass + maxFnum * fMass + maxHnum * hMass
        Dim maxClSiFHmass As Double = maxClnum * clMass + maxSinum * siMass + maxFnum * fMass + maxHnum * hMass
        Dim maxSiFHmass As Double = maxSinum * siMass + maxFnum * fMass + maxHnum * hMass
        Dim maxFHmass As Double = maxFnum * fMass + maxHnum * hMass
        Dim maxHmass As Double = maxHnum * hMass

        Dim formulaCandidates = New System.Collections.Generic.List(Of FormulaResult)()

#Region ""
        For h As Integer = 0 To maxInum
            If formulaBean.Mass + CDbl(h) * iMass + maxBrClSiFHmass < mass - ms1Tol Then Continue For
            If formulaBean.Mass + CDbl(h) * iMass > mass + ms1Tol Then Exit For

            For i As Integer = 0 To maxBrnum
                If formulaBean.Mass + CDbl(h) * iMass + CDbl(i) * brMass + maxClSiFHmass < mass - ms1Tol Then Continue For
                If formulaBean.Mass + CDbl(h) * iMass + CDbl(i) * brMass > mass + ms1Tol Then Exit For

                For j As Integer = 0 To maxClnum
                    If formulaBean.Mass + CDbl(h) * iMass + CDbl(i) * brMass + CDbl(j) * clMass + maxSiFHmass < mass - ms1Tol Then Continue For
                    If formulaBean.Mass + CDbl(h) * iMass + CDbl(i) * brMass + CDbl(j) * clMass > mass + ms1Tol Then Exit For

                    For k As Integer = 0 To maxSinum
                        If param.IsTmsMeoxDerivative AndAlso k < param.MinimumTmsCount Then Continue For
                        If formulaBean.Mass + CDbl(h) * iMass + CDbl(i) * brMass + CDbl(j) * clMass + CDbl(k) * siMass + maxFHmass < mass - ms1Tol Then Continue For
                        If formulaBean.Mass + CDbl(h) * iMass + CDbl(i) * brMass + CDbl(j) * clMass + CDbl(k) * siMass > mass + ms1Tol Then Exit For

                        For l As Integer = 0 To maxFnum
                            If formulaBean.Mass + CDbl(h) * iMass + CDbl(i) * brMass + CDbl(j) * clMass + CDbl(k) * siMass + CDbl(l) * fMass + maxHmass < mass - ms1Tol Then Continue For
                            If formulaBean.Mass + CDbl(h) * iMass + CDbl(i) * brMass + CDbl(j) * clMass + CDbl(k) * siMass + CDbl(l) * fMass > mass + ms1Tol Then Exit For

                            For m As Integer = minHnum To maxHnum
                                If formulaBean.Mass + CDbl(h) * iMass + CDbl(i) * brMass + CDbl(j) * clMass + CDbl(k) * siMass + CDbl(l) * fMass + CDbl(m) * hMass < mass - ms1Tol Then Continue For
                                If formulaBean.Mass + CDbl(h) * iMass + CDbl(i) * brMass + CDbl(j) * clMass + CDbl(k) * siMass + CDbl(l) * fMass + CDbl(m) * hMass > mass + ms1Tol Then Exit For

                                If param.IsTmsMeoxDerivative Then

                                    Dim tmsCount = k
                                    For n As Integer = param.MinimumMeoxCount To formulaBean.Nnum
                                        Dim meoxCount = n
                                        Dim formula = Me.getCandidateFormulaBean(formulaBean, m, l, j, i, h, k, tmsCount, meoxCount)

                                        If formula.Cnum - tmsCount * 3 - meoxCount <= 0 Then Continue For
                                        Dim convertedFormula = CompMs.Common.FormulaGenerator.[Function].MolecularFormulaUtility.ConvertTmsMeoxSubtractedFormula(formula)
                                        If Not CompMs.Common.FormulaGenerator.[Function].SevenGoldenRulesCheck.Check(convertedFormula, Me.valenceCheck, Me.coverRange, Me.probabilityCheck, adduct) Then Continue For

                                        Dim formulaResult = Me.tryGetFormulaResultCandidate(formula, param, mass, ms1Tol, m1Intensity, m2Intensity, isotopeCheck, adduct, refinedPeaklist, neutralLosses, existFormulaDB, productIonDB, neutralLossDB)
                                        If formulaResult IsNot Nothing Then formulaCandidates = Me.getFormulaResultCandidates(formulaCandidates, formulaResult, 100000000)
                                    Next
                                Else
                                    Dim formula = Me.getCandidateFormulaBean(formulaBean, m, l, j, i, h, k)
                                    Dim formulaResult = Me.tryGetFormulaResultCandidate(formula, param, mass, ms1Tol, m1Intensity, m2Intensity, isotopeCheck, adduct, refinedPeaklist, neutralLosses, existFormulaDB, productIonDB, neutralLossDB)
                                    If formulaResult IsNot Nothing Then formulaCandidates = Me.getFormulaResultCandidates(formulaCandidates, formulaResult, 100000000)
                                End If
                            Next
                        Next
                    Next
                Next
            Next
        Next
#End Region
        Return formulaCandidates
    End Function

    Private Function tryGetFormulaResultCandidate(ByVal formula As Formula, ByVal param As AnalysisParamOfMsfinder, ByVal mass As Double, ByVal ms1Tol As Double, ByVal m1Intensity As Double, ByVal m2Intensity As Double, ByVal isotopeCheck As Boolean, ByVal adduct As AdductIon, ByVal refinedPeaklist As System.Collections.Generic.List(Of SpectrumPeak), ByVal neutralLosses As System.Collections.Generic.List(Of NeutralLoss), ByVal existFormulaDB As System.Collections.Generic.List(Of ExistFormulaQuery), ByVal productIonDB As System.Collections.Generic.List(Of ProductIon), ByVal neutralLossDB As System.Collections.Generic.List(Of NeutralLoss)) As FormulaResult

        Dim ms2Tol = param.Mass2Tolerance
        Dim massTolType = param.MassTolType

        If SevenGoldenRulesCheck.Check(formula, Me.valenceCheck, Me.coverRange, Me.probabilityCheck, adduct) Then

            Dim formulaResult = Me.getFormulaResult(formula, param, mass, ms1Tol, m1Intensity, m2Intensity, isotopeCheck, existFormulaDB)
            If param.CanExcuteMS2AdductSearch Then
                Dim precursorMz = (mass * CDbl(adduct.AdductIonXmer) + adduct.AdductIonAccurateMass) / CDbl(adduct.ChargeNumber)
                If adduct.IonMode = IonModes.Positive Then
                    formulaResult.AnnotatedIonResult = CompMs.Common.FormulaGenerator.[Function].FragmentAssigner.GetAnnotatedIon(refinedPeaklist, adduct, param.MS2PositiveAdductIonList, precursorMz, param.Mass2Tolerance, param.MassTolType)
                Else
                    formulaResult.AnnotatedIonResult = CompMs.Common.FormulaGenerator.[Function].FragmentAssigner.GetAnnotatedIon(refinedPeaklist, adduct, param.MS2NegativeAdductIonList, precursorMz, param.Mass2Tolerance, param.MassTolType)
                End If
                refinedPeaklist = CompMs.Common.FormulaGenerator.[Function].FragmentAssigner.GetRefinedPeaklist(refinedPeaklist, precursorMz)
            End If
            Me.setFragmentProperties(formulaResult, refinedPeaklist, neutralLosses, productIonDB, neutralLossDB, ms2Tol, massTolType, adduct)
            formulaResult.TotalScore = System.Math.Round(CompMs.Common.FormulaGenerator.[Function].Scoring.TotalScore(formulaResult), 3)
            Return formulaResult
        Else
            Return Nothing
        End If
    End Function



    'private List<FormulaResult> tryGetFormulaResultCandidate(List<FormulaResult> formulaCandidate, Formula formula, AnalysisParamOfMsfinder param, 
    '    double mass, double ms1Tol, double m1Intensity, double m2Intensity, bool isotopeCheck, AdductIon adduct,
    '    List<Peak> refinedPeaklist, List<NeutralLoss> neutralLosses, List<ExistFormulaQuery> existFormulaDB, List<ProductIon> productIonDB, List<NeutralLoss> neutralLossDB) {

    '    var ms2Tol = param.Mass2Tolerance;
    '    var massTolType = param.MassTolType;

    '    if (SevenGoldenRulesCheck.Check(formula, this.valenceCheck, this.coverRange, this.probabilityCheck, adduct)) {

    '        var formulaResult = getFormulaResult(formula, param, mass, ms1Tol, m1Intensity, m2Intensity, isotopeCheck, existFormulaDB);
    '        setFragmentProperties(formulaResult, refinedPeaklist, neutralLosses, productIonDB, neutralLossDB, ms2Tol, massTolType, adduct);
    '        formulaResult.TotalScore = Math.Round(Scoring.TotalScore(formulaResult), 3);
    '        formulaCandidate = getFormulaResultCandidates(formulaCandidate, formulaResult, 100000000);

    '        return formulaCandidate;

    '    }
    '    else return formulaCandidate;
    '}



    Private Function getFormulaResultCandidates(ByVal formulaCandidate As List(Of FormulaResult), ByVal formulaResult As FormulaResult, ByVal maxFormulaNum As Integer) As List(Of FormulaResult)
        If formulaCandidate.Count < maxFormulaNum - 1 Then
            formulaCandidate.Add(formulaResult)
        ElseIf formulaCandidate.Count = maxFormulaNum - 1 Then
            formulaCandidate.Add(formulaResult)
            formulaCandidate = formulaCandidate.OrderByDescending(Function(n) System.std.Abs(n.TotalScore)).ToList()
        ElseIf formulaCandidate.Count > maxFormulaNum - 1 Then
            If formulaCandidate(CInt((formulaCandidate.Count - 1))).TotalScore < formulaResult.TotalScore Then
                Dim startID = Me.getFormulaResultStartIndex(formulaResult.TotalScore, 0.01, formulaCandidate)
                Dim insertID = Me.getFormulaResultInsertID(formulaCandidate, formulaResult, startID)
                formulaCandidate.Insert(insertID, formulaResult)
                formulaCandidate.RemoveAt(formulaCandidate.Count - 1)
            End If
        End If

        Return formulaCandidate
    End Function

    Private Function getFormulaResultStartIndex(ByVal score As Double, ByVal tol As Double, ByVal results As List(Of FormulaResult)) As Integer
        If results Is Nothing OrElse results.Count = 0 Then Return 0
        Dim targetScore As Double = score + tol
        Dim startIndex As Integer = 0, endIndex As Integer = results.Count - 1
        Dim counter As Integer = 0

        While counter < 10
            If results(CInt((startIndex))).TotalScore <= targetScore AndAlso targetScore < results(CInt(((startIndex + endIndex) / 2))).TotalScore Then
                endIndex = (startIndex + endIndex) / 2
            ElseIf results(CInt(((startIndex + endIndex) / 2))).TotalScore <= targetScore AndAlso targetScore < results(CInt((endIndex))).TotalScore Then
                startIndex = (startIndex + endIndex) / 2
            End If
            counter += 1
        End While
        Return startIndex
    End Function

    Private Function getFormulaResultInsertID(ByVal formulaCandidate As System.Collections.Generic.List(Of FormulaResult), ByVal formulaResult As FormulaResult, ByVal startID As Integer) As Integer
        Dim maxID = 0
        For i As Integer = startID To 0 Step -1
            If formulaCandidate(CInt((i))).TotalScore < formulaResult.TotalScore Then
                maxID = i
            Else
                Exit For
            End If
        Next
        Return maxID
    End Function

    Private Function getFormulaResult(ByVal formula As Formula, ByVal param As AnalysisParamOfMsfinder, ByVal mass As Double, ByVal ms1MassTol As Double, ByVal m1Intensity As Double, ByVal m2Intensity As Double, ByVal isotopeCheck As Boolean, ByVal existFormulaDB As System.Collections.Generic.List(Of ExistFormulaQuery)) As FormulaResult
        Dim isotopicAbundanceTolerance = param.IsotopicAbundanceTolerance
        Dim formulaResult = New FormulaResult()

        formulaResult.Formula = formula
        formulaResult.MatchedMass = System.Math.Round(mass, 7)
        formulaResult.MassDiff = System.Math.Round(formula.ExactMass - mass, 7)
        formulaResult.M1IsotopicIntensity = System.Math.Round(SevenGoldenRulesCheck.GetM1IsotopicAbundance(formula), 4)
        formulaResult.M2IsotopicIntensity = System.Math.Round(SevenGoldenRulesCheck.GetM2IsotopicAbundance(formula), 4)
        formulaResult.M1IsotopicDiff = System.Math.Round(SevenGoldenRulesCheck.GetIsotopicDifference(formulaResult.M1IsotopicIntensity, m1Intensity), 4)
        formulaResult.M2IsotopicDiff = System.Math.Round(SevenGoldenRulesCheck.GetIsotopicDifference(formulaResult.M2IsotopicIntensity, m2Intensity), 4)

        formulaResult.MassDiffScore = System.Math.Round(CompMs.Common.FormulaGenerator.[Function].Scoring.MassDifferenceScore(formulaResult.MassDiff, ms1MassTol), 3)

        If isotopeCheck = True Then
            formulaResult.IsotopicScore = System.Math.Round(CompMs.Common.FormulaGenerator.[Function].Scoring.IsotopicDifferenceScore(formulaResult.M1IsotopicDiff, formulaResult.M2IsotopicDiff, isotopicAbundanceTolerance * 0.01), 3)
        Else
            formulaResult.IsotopicScore = 1
        End If

        Me.setExistFormulaDbInfo(formulaResult, existFormulaDB)

        Return formulaResult
    End Function

    Private Function getCandidateFormulaBean(ByVal originalFormula As Formula, ByVal addHnum As Integer, ByVal addFnum As Integer, ByVal addClnum As Integer, ByVal addBrnum As Integer, ByVal addInum As Integer, ByVal addSinum As Integer, ByVal Optional tmsCount As Integer = 0, ByVal Optional meoxCount As Integer = 0) As Formula
        Dim cNum = originalFormula!C
        Dim hNum = originalFormula!H + addHnum
        Dim nNum = originalFormula!N
        Dim oNum = originalFormula!O
        Dim pNum = originalFormula!P
        Dim sNum = originalFormula!S
        Dim fNum = originalFormula!F + addFnum
        Dim clNum = originalFormula!Cl + addClnum
        Dim brNum = originalFormula!Br + addBrnum
        Dim iNum = originalFormula!I + addInum
        Dim siNum = originalFormula!Si + addSinum
        Dim counts As New Dictionary(Of String, Integer) From {
            {"C", cNum}, {"H", hNum}, {"N", nNum}, {"O", oNum}, {"P", pNum},
            {"S", sNum}, {"F", fNum}, {"Cl", clNum}, {"Br", brNum},
            {"I", iNum}, {"Si", siNum}, {"TMS", tmsCount}, {"MEOX", meoxCount}
        }

        Return New Formula(counts)
    End Function
End Class
