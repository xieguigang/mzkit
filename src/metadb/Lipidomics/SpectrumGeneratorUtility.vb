Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

Public Module SpectrumGeneratorUtility

    Private ReadOnly CH2 As Double = {HydrogenMass * 2, CarbonMass}.Sum()

    <Obsolete>
    Public Function GetAcylDoubleBondSpectrum(lipid As ILipid, acylChain As AcylChain, adduct As AdductIon, Optional NLMass As Double = 0.0, Optional abundance As Double = 50.0) As IEnumerable(Of SpectrumPeak)
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
    Public Function GetAlkylDoubleBondSpectrum(lipid As ILipid, alkylChain As AlkylChain, adduct As AdductIon, Optional NLMass As Double = 0.0, Optional abundance As Double = 50.0) As IEnumerable(Of SpectrumPeak)
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
