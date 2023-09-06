Imports CompMs.Common.Components
Imports CompMs.Common.Interfaces

Public Module TGEadMsCharacterization
    Public Function Characterize(scan As IMSScanProperty, molecule As ILipid, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As (ILipid, Double())
        Dim class_cutoff = 0
        Dim chain_cutoff = 2
        Dim position_cutoff = 1
        Dim double_cutoff = 0.5

        Dim chains = molecule.Chains.GetDeterminedChains()
        If chains.Length = 3 Then
            If ChainsEqual(chains(0), chains(1)) AndAlso ChainsEqual(chains(1), chains(2)) Then
                chain_cutoff = 1
            ElseIf ChainsEqual(chains(0), chains(1)) AndAlso Not ChainsEqual(chains(1), chains(2)) Then
                chain_cutoff = 2
            ElseIf Not ChainsEqual(chains(0), chains(1)) AndAlso ChainsEqual(chains(1), chains(2)) Then
                chain_cutoff = 2
            ElseIf ChainsEqual(chains(0), chains(2)) AndAlso Not ChainsEqual(chains(1), chains(2)) Then
                chain_cutoff = 2
            Else
                chain_cutoff = 3
            End If
        End If
        If Equals(reference.AdductType.AdductIonName, "[M+NH4]+") Then
            position_cutoff = 0
        End If

        Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, class_cutoff, chain_cutoff, position_cutoff, double_cutoff)
        Return GetDefaultCharacterizationResultForTriacylGlycerols(molecule, defaultResult)
    End Function

    Private Function ChainsEqual(a As IChain, b As IChain) As Boolean
        Return a.CarbonCount = b.CarbonCount AndAlso a.DoubleBond Is b.DoubleBond
    End Function
End Module
