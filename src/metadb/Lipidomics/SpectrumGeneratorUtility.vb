Imports CompMs.Common.Components
Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.FormulaGenerator.DataObj
Imports System
Imports System.Collections.Generic
Imports System.Linq

Public Module SpectrumGeneratorUtility

    Private ReadOnly CH2 As Double = {HydrogenMass * 2, CarbonMass}.Sum()

    <Obsolete>
    Public Function GetAcylDoubleBondSpectrum(ByVal lipid As ILipid, ByVal acylChain As AcylChain, ByVal adduct As AdductIon, ByVal Optional NLMass As Double = 0.0, ByVal Optional abundance As Double = 50.0) As IEnumerable(Of SpectrumPeak)
        If acylChain.DoubleBond.UnDecidedCount <> 0 OrElse acylChain.CarbonCount = 0 Then
            Return Enumerable.Empty(Of SpectrumPeak)()
        End If
        Dim chainLoss = lipid.Mass - acylChain.Mass - NLMass
        Dim diffs = New Double(acylChain.CarbonCount - 1) {}
        For i = 0 To acylChain.CarbonCount - 1
            diffs(i) = CH2
        Next

        diffs(0) += OxygenMass - HydrogenMass * 2

        For Each bond In acylChain.DoubleBond.Bonds
            diffs(bond.Position - 1) -= HydrogenMass
            diffs(bond.Position) -= HydrogenMass
        Next
        For i = 1 To acylChain.CarbonCount - 1
            diffs(i) += diffs(i - 1)
        Next

        Dim peaks = New List(Of SpectrumPeak)()
        For i = 0 To acylChain.CarbonCount - 1 - 1
            peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(i)), abundance, $"{acylChain} C{i + 1}"))
            peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(i) - HydrogenMass), abundance * 0.5, $"{acylChain} C{i + 1}-H"))
            peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(i) + HydrogenMass), abundance * 0.5, $"{acylChain} C{i + 1}+H"))
        Next

        Return peaks
    End Function

    <Obsolete>
    Public Function GetAlkylDoubleBondSpectrum(ByVal lipid As ILipid, ByVal alkylChain As AlkylChain, ByVal adduct As AdductIon, ByVal Optional NLMass As Double = 0.0, ByVal Optional abundance As Double = 50.0) As IEnumerable(Of SpectrumPeak)
        Dim chainLoss = lipid.Mass - alkylChain.Mass - NLMass
        Dim diffs = New Double(alkylChain.CarbonCount - 1) {}
        For i = 0 To alkylChain.CarbonCount - 1
            diffs(i) = CH2
        Next

        For Each bond In alkylChain.DoubleBond.Bonds
            diffs(bond.Position - 1) -= HydrogenMass
            diffs(bond.Position) -= HydrogenMass
        Next
        For i = 1 To alkylChain.CarbonCount - 1
            diffs(i) += diffs(i - 1)
        Next

        Dim peaks = New List(Of SpectrumPeak)()
        For i = 0 To alkylChain.CarbonCount - 1 - 1
            peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(i)), abundance, $"{alkylChain} C{i + 1}"))
            peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(i) - HydrogenMass), abundance * 0.5, $"{alkylChain} C{i + 1}-H"))
            peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(i) + HydrogenMass), abundance * 0.5, $"{alkylChain} C{i + 1}+H"))
        Next

        Return peaks
    End Function
End Module
