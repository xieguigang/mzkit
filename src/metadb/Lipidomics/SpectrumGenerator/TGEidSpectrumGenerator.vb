Imports CompMs.Common.Components
Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.Enum
Imports CompMs.Common.FormulaGenerator.DataObj
Imports CompMs.Common.Interfaces
Imports System
Imports System.Collections.Generic
Imports System.Linq

    Public Class TGEidSpectrumGenerator
        Implements ILipidSpectrumGenerator

        Private Shared ReadOnly C2H3O As Double = {CarbonMass * 2, HydrogenMass * 3, OxygenMass}.Sum()

        Private Shared ReadOnly C3H5O2 As Double = {CarbonMass * 3, HydrogenMass * 5, OxygenMass * 2}.Sum()

        Private Shared ReadOnly CH2 As Double = {HydrogenMass * 2, CarbonMass}.Sum()

        Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()

        Private Shared ReadOnly Electron As Double = 0.00054858026

        Public Sub New()
            spectrumGenerator = New SpectrumPeakGenerator()
        End Sub

        Public Sub New(ByVal spectrumGenerator As ISpectrumPeakGenerator)
            Me.spectrumGenerator = If(spectrumGenerator, CSharpImpl.__Throw(Of ISpectrumPeakGenerator)(New ArgumentNullException(NameOf(spectrumGenerator))))
        End Sub

        Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator

        Public Function CanGenerate(ByVal lipid As ILipid, ByVal adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
            If lipid.LipidClass = LbmClass.TG Then
                If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+NH4]+") Then
                    Return True
                End If
            End If
            Return False
        End Function

        Public Function Generate(ByVal lipid As Lipid, ByVal adduct As AdductIon, ByVal Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
            Dim spectrum = New List(Of SpectrumPeak)()
            spectrum.AddRange(GetTGSpectrum(lipid, adduct))
            Dim sn2 As IChain = Nothing
            If lipid.Description.Has(LipidDescription.Chain) Then
                Dim chains = lipid.Chains.GetDeterminedChains().ToList()

                If CSharpImpl.__Assign(sn2, TryCast(lipid.Chains.GetChainByPosition(2), IChain)) IsNot Nothing Then
                    spectrum.AddRange(GetAcylPositionSpectrum(lipid, sn2, adduct))
                    chains.Remove(sn2)
                End If
                spectrum.AddRange(GetAcylLevelSpectrum(lipid, chains, adduct))
                spectrum.AddRange(EidSpecificSpectrum(lipid, adduct, 0R, 200R))
                spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, lipid.Chains.GetTypedChains(Of AcylChain)(), adduct))
            End If
            spectrum = spectrum.GroupBy(Function(spec) spec, comparer).[Select](Function(specs) New SpectrumPeak(Enumerable.First(specs).Mass, specs.Sum(Function(n) n.Intensity), String.Join(", ", specs.[Select](Function(spec) spec.Comment)), specs.Aggregate(SpectrumComment.none, Function(a, b) a Or b.SpectrumComment))).OrderBy(Function(peak) peak.Mass).ToList()
            Return CreateReference(lipid, adduct, spectrum, molecule)
        End Function

        Private Function CreateReference(ByVal lipid As ILipid, ByVal adduct As AdductIon, ByVal spectrum As List(Of SpectrumPeak), ByVal molecule As IMoleculeProperty) As MoleculeMsReference
            Return New MoleculeMsReference With {
    .PrecursorMz = adduct.ConvertToMz(lipid.Mass),
    .IonMode = adduct.IonMode,
    .Spectrum = spectrum,
    .Name = lipid.Name,
    .Formula = molecule?.Formula,
    .Ontology = molecule?.Ontology,
    .SMILES = molecule?.SMILES,
    .InChIKey = molecule?.InChIKey,
    .AdductType = adduct,
    .CompoundClass = lipid.LipidClass.ToString(),
    .Charge = adduct.ChargeNumber
}
        End Function

        Private Function GetTGSpectrum(ByVal lipid As ILipid, ByVal adduct As AdductIon) As SpectrumPeak()
            Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999R, "Precursor") With {
        .SpectrumComment = SpectrumComment.precursor
    },
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) / 2, 150R, "[Precursor]2+") With {
        .SpectrumComment = SpectrumComment.precursor
    }
}
            If Equals(adduct.AdductIonName, "[M+NH4]+") Then
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - H2O, 150R, "Precursor-H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(lipid.Mass + ProtonMass, 150R, "[M+H]+") With {
.SpectrumComment = SpectrumComment.metaboliteclass
'new SpectrumPeak(lipid.Mass + MassDiffDictionary.ProtonMass-H2O, 150d, "[M+H]+ -H2O") { SpectrumComment = SpectrumComment.metaboliteclass },
}})
            ElseIf Equals(adduct.AdductIonName, "[M+H]+") Then
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - H2O, 100R, "Precursor-H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
            End If

            Return spectrum.ToArray()
        End Function

        Private Function GetAcylLevelSpectrum(ByVal lipid As ILipid, ByVal acylChains As IEnumerable(Of IChain), ByVal adduct As AdductIon) As IEnumerable(Of SpectrumPeak)
            Return acylChains.SelectMany(Function(acylChain) GetAcylLevelSpectrum(lipid, acylChain, adduct))
        End Function

        Private Function GetAcylLevelSpectrum(ByVal lipid As ILipid, ByVal acylChain As IChain, ByVal adduct As AdductIon) As SpectrumPeak()
            Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
            Dim lipidMass = lipid.Mass + adductmass
            Dim chainMass = acylChain.Mass - HydrogenMass
            Dim chainMass2 = acylChain.Mass + adductmass
            Dim spectrum = New List(Of SpectrumPeak)()
            If Equals(adduct.AdductIonName, "[M+Na]+") Then
                spectrum.AddRange({New SpectrumPeak(chainMass2 + C2H3O + OxygenMass, 100R, $"{acylChain}+C2H3O2") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass2 + C2H3O + CH2, 100R, $"{acylChain}+C3H5O") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(acylChain.Mass + Electron, 100R, $"{acylChain}+") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass - HydrogenMass * 2, 50R, $"-{acylChain}") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass - H2O, 200R, $"-{acylChain}-O") With {
.SpectrumComment = SpectrumComment.acylchain
}})
            Else
                spectrum.AddRange({New SpectrumPeak(acylChain.Mass + Electron, 100R, $"{acylChain}+") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass2 + C3H5O2, 100R, $"{acylChain}+C3H5O2") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass2 + C3H5O2 - H2O, 100R, $"{acylChain}+C3H3O") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass, 50R, $"-{acylChain}") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass - H2O, 200R, $"-{acylChain}-O") With {
.SpectrumComment = SpectrumComment.acylchain
}})
            End If
            Return spectrum.ToArray()
        End Function

        Private Function GetAcylPositionSpectrum(ByVal lipid As ILipid, ByVal acylChain As IChain, ByVal adduct As AdductIon) As SpectrumPeak()
            Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
            Dim chainMass = acylChain.Mass + adductmass
            Dim lipidMass = lipid.Mass + adductmass
            Dim spectrum = New List(Of SpectrumPeak)()
            If Equals(adduct.AdductIonName, "[M+Na]+") Then
                spectrum.AddRange({New SpectrumPeak(chainMass + C2H3O, 100R, "Sn2 diagnostics") With {
.SpectrumComment = SpectrumComment.snposition,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}, New SpectrumPeak(acylChain.Mass + Electron, 100R, $"{acylChain}+ Sn2") With {
.SpectrumComment = SpectrumComment.acylchain
'new SpectrumPeak(lipidMass - chainMass + MassDiffDictionary.HydrogenMass*2, 50d, $"-{acylChain}") { SpectrumComment = SpectrumComment.acylchain },
}, New SpectrumPeak(lipidMass - chainMass - OxygenMass, 200R, $"-{acylChain}-O Sn2") With {
.SpectrumComment = SpectrumComment.acylchain
}})
            Else
                spectrum.AddRange({New SpectrumPeak(chainMass + C2H3O, 100R, "Sn2 diagnostics") With {
.SpectrumComment = SpectrumComment.snposition
}, New SpectrumPeak(acylChain.Mass + Electron, 100R, $"{acylChain}+ Sn2") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + C3H5O2, 100R, $"{acylChain}+C3H5O2 Sn2") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + C3H5O2 - H2O, 100R, $"{acylChain}+C3H3O Sn2") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass + HydrogenMass * 2, 50R, $"-{acylChain} Sn2") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass - OxygenMass, 200R, $"-{acylChain}-O Sn2") With {
.SpectrumComment = SpectrumComment.acylchain
}})
            End If
            Return spectrum.ToArray()
        End Function

        Private Function GetAcylDoubleBondSpectrum(ByVal lipid As ILipid, ByVal acylChains As IEnumerable(Of AcylChain), ByVal adduct As AdductIon, ByVal Optional nlMass As Double = 0.0) As IEnumerable(Of SpectrumPeak)
            Dim spectrum = New List(Of SpectrumPeak)()
            For Each lossChain In acylChains
                nlMass = lossChain.Mass + OxygenMass + adduct.AdductIonAccurateMass - ProtonMass
                Dim chains = acylChains.Where(Function(c) Not c.Equals(lossChain)).ToList()
                spectrum.AddRange(chains.SelectMany(Function(acylChain) spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acylChain, adduct, nlMass, 5R)))
            Next
            Return spectrum.ToArray()
        End Function
        Private Shared Function EidSpecificSpectrum(ByVal lipid As Lipid, ByVal adduct As AdductIon, ByVal nlMass As Double, ByVal intensity As Double) As SpectrumPeak()
            Dim spectrum = New List(Of SpectrumPeak)()
            Dim acylChains As SeparatedChains = Nothing

            If CSharpImpl.__Assign(acylChains, TryCast(lipid.Chains, SeparatedChains)) IsNot Nothing Then
                For Each lossChain In lipid.Chains.GetDeterminedChains()
                    nlMass = lossChain.Mass + OxygenMass + adduct.AdductIonAccurateMass - ProtonMass
                    Dim chains = lipid.Chains.GetDeterminedChains().Where(Function(c) Not c.Equals(lossChain)).ToList()
                    For Each chain In chains
                        If chain.DoubleBond.Count = 0 OrElse chain.DoubleBond.UnDecidedCount > 0 Then Continue For
                        If chain.DoubleBond.Count <= 3 Then
                            intensity = intensity * 0.5
                        End If
                        spectrum.AddRange(EidSpecificSpectrumGenerator.EidSpecificSpectrumGen(lipid, chain, adduct, nlMass, intensity))
                    Next
                Next
            End If
            Return spectrum.ToArray()
        End Function

        Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()
    End Class
