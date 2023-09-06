Imports CompMs.Common.Components
Imports CompMs.Common.Interfaces

Namespace CompMs.Common.Lipidomics
    Public Module PEEadMsCharacterization
        Public Function Characterize(ByVal scan As IMSScanProperty, ByVal molecule As ILipid, ByVal reference As MoleculeMsReference, ByVal tolerance As Single, ByVal mzBegin As Single, ByVal mzEnd As Single) As (ILipid, Double())
            Dim class_cutoff = 2
            Dim chain_cutoff = 2
            Dim position_cutoff = 1
            Dim double_cutoff = 0.5

            Dim chains = molecule.Chains.GetDeterminedChains()
            If chains.Length = 2 Then
                If chains(0).CarbonCount = chains(1).CarbonCount AndAlso chains(0).DoubleBond Is chains(1).DoubleBond Then
                    chain_cutoff = 1
                End If
            End If
            If Equals(reference.AdductType.AdductIonName, "[M+H]+") Then
                position_cutoff = 0
            End If

            Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, class_cutoff, chain_cutoff, position_cutoff, double_cutoff)
            Return GetDefaultCharacterizationResultForGlycerophospholipid(molecule, defaultResult)
        End Function
    End Module
End Namespace
