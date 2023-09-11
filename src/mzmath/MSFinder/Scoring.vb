Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports Microsoft.VisualBasic.Math.Distributions
Imports std = System.Math

Public NotInheritable Class Scoring
    Private Sub New()
    End Sub

    Public Shared Function MassDifferenceScore(diff As Double, devi As Double) As Double
        Return Gaussian.StandadizedGaussianFunction(diff, devi)
    End Function

    Public Shared Function IsotopicDifferenceScore(m1Diff As Double, m2Diff As Double, devi As Double) As Double
        Return (Gaussian.StandadizedGaussianFunction(m1Diff, devi) + Gaussian.StandadizedGaussianFunction(m2Diff, devi)) * 0.5
    End Function

    Public Shared Function FragmentHitsScore(peaklist As List(Of SpectrumPeak), productIons As List(Of ProductIon), ms2Tol As Double, massTolType As MassToleranceType) As Double
        Dim devi = 0.0
        If peaklist.Count = 0 Then Return 0

        Dim monoisotopicCount = peaklist.Where(Function(n) Equals(n.Annotation, "M")).Count

        If peaklist IsNot Nothing AndAlso peaklist.Count <> 0 Then
            Dim totalDiff As Double = 0
            For Each fragment In productIons
                totalDiff += std.Abs(fragment.MassDiff)
                If massTolType = MassToleranceType.Da Then
                    devi += ms2Tol
                Else
                    devi += PPMmethod.ConvertPpmToMassAccuracy(fragment.Mass, ms2Tol)
                End If
            Next
            If productIons.Count = 0 Then
                Return 0
            Else
                Return productIons.Count / monoisotopicCount
                'return (double)productIons.Count / (double)peaklist.Count * BasicMathematics.StandadizedGaussianFunction(totalDiff, devi);
            End If
        Else
            Return 0
        End If
    End Function

    Public Shared Function NeutralLossScore(neutralLossResult As List(Of NeutralLoss), ms2Tol As Double, massTolType As MassToleranceType, neutralLossNum As Double) As Double
        Dim devi = 0.0
        If neutralLossResult.Count <> 0 Then
            Dim totalScore As Double = 0
            For Each nloss In neutralLossResult
                If massTolType = MassToleranceType.Da Then
                    devi = ms2Tol
                Else
                    devi = PPMmethod.ConvertPpmToMassAccuracy(nloss.PrecursorMz, ms2Tol)
                End If

                totalScore += Gaussian.StandadizedGaussianFunction(nloss.MassError, devi)
            Next
            'return totalScore / (double)neutralLossNum;
            Return neutralLossResult.Count / neutralLossNum
        Else
            Return 0
        End If
    End Function

    Public Shared Function NeutralLossScore(hits As Integer, totalCount As Integer) As Double
        If totalCount <= 0 Then Return 0.0

        Dim hitsDouble = CDbl(hits)
        Dim countDouble = CDbl(totalCount)
        Return hitsDouble / countDouble
    End Function


    Public Shared Function DatabaseScore(recordNum As Integer, recordName As String) As Double
        Dim lDatabaseScore = 0.0
        Dim isMineIncluded = False
        If Not Equals(recordName, Nothing) AndAlso recordName.Contains("MINE") Then isMineIncluded = True

        Dim recordNumber = recordNum
        If isMineIncluded Then recordNumber -= 1

        If recordNumber > 0 Then lDatabaseScore = 0.5 * (1.0 + recordNum / 21.0)

        If isMineIncluded Then lDatabaseScore += 0.2
        Return lDatabaseScore
    End Function

    Public Shared Function TotalScore(result As FormulaResult) As Double
        'double score = 6.4305 * result.MassDiffScore + 0.3285 * result.IsotopicScore + 2.9103 * result.ProductIonScore + 0.7190 * result.NeutralLossScore + 10.7509 * DatabaseScore(result.ResourceRecords);
        Dim score = result.MassDiffScore + result.IsotopicScore + result.ProductIonScore + result.NeutralLossScore + DatabaseScore(result.ResourceRecords, result.ResourceNames)
        'double score = DatabaseScore(result.ResourceRecords, result.ResourceNames);
        Return score
    End Function
End Class
