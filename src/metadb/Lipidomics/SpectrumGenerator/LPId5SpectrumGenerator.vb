﻿Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemistry.MetaLib
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

Public Class LPId5SpectrumGenerator
        Implements ILipidSpectrumGenerator

        Private Shared ReadOnly C6H13O9P As Double = {CarbonMass * 6, HydrogenMass * 13, OxygenMass * 9, PhosphorusMass}.Sum()

        Private Shared ReadOnly C6H10O5 As Double = {CarbonMass * 6, HydrogenMass * 10, OxygenMass * 5}.Sum()

        Private Shared ReadOnly C3H4D5O6P As Double = {CarbonMass * 3, HydrogenMass * 4, OxygenMass * 6, PhosphorusMass, Hydrogen2Mass * 5}.Sum() '  OCC(O)COP(O)(O)=O

        Private Shared ReadOnly Gly_C As Double = {CarbonMass * 9, HydrogenMass * 12, OxygenMass * 9, PhosphorusMass, Hydrogen2Mass * 5}.Sum()

        Private Shared ReadOnly Gly_O As Double = {CarbonMass * 8, HydrogenMass * 12, OxygenMass * 10, PhosphorusMass, Hydrogen2Mass * 3}.Sum()

        Private Shared ReadOnly CD2 As Double = {Hydrogen2Mass * 2, CarbonMass}.Sum()

        Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()

        Public Sub New()
            spectrumGenerator = New SpectrumPeakGenerator()
        End Sub

        Public Sub New(spectrumGenerator As ISpectrumPeakGenerator)
            Me.spectrumGenerator = If(spectrumGenerator, CSharpImpl.__Throw(Of ISpectrumPeakGenerator)(New ArgumentNullException(NameOf(spectrumGenerator))))
        End Sub

        Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator

        Public Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
            If lipid.LipidClass = LbmClass.LPI_d5 Then
                If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+NH4]+") Then
                    Return True
                End If
            End If
            Return False
        End Function

        Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
            Dim spectrum = New List(Of SpectrumPeak)()
            spectrum.AddRange(GetLPISpectrum(lipid, adduct))
            If lipid.Description.Has(LipidDescription.Chain) Then
                spectrum.AddRange(GetAcylLevelSpectrum(lipid, lipid.Chains.GetDeterminedChains(), adduct))
                lipid.Chains.ApplyToChain(1, Sub(chain) spectrum.AddRange(GetAcylPositionSpectrum(lipid, chain, adduct)))
                spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, lipid.Chains.GetTypedChains(Of AcylChain)(), adduct))
            End If
            spectrum = spectrum.GroupBy(Function(spec) spec, comparer).[Select](Function(specs) New SpectrumPeak(Enumerable.First(specs).Mass, specs.Sum(Function(n) n.Intensity), String.Join(", ", specs.[Select](Function(spec) spec.Comment)), specs.Aggregate(SpectrumComment.none, Function(a, b) a Or b.SpectrumComment))).OrderBy(Function(peak) peak.Mass).ToList()
            Return CreateReference(lipid, adduct, spectrum, molecule)
        End Function

        Private Function CreateReference(lipid As ILipid, adduct As AdductIon, spectrum As List(Of SpectrumPeak), molecule As IMoleculeProperty) As MoleculeMsReference
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

        Private Function GetLPISpectrum(lipid As ILipid, adduct As AdductIon) As SpectrumPeak()
            Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
            Dim lipidMass = lipid.Mass + adductmass
            Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(C6H13O9P + adductmass, 300R, "Header") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    },
    New SpectrumPeak(C6H13O9P + adductmass - H2O, 300R, "Header -H2O") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(Gly_C + adductmass, 100R, "Gly-C") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(Gly_O + adductmass, 200R, "Gly-O") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(C3H4D5O6P + adductmass, 300R, "C3H9O6P") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(C3H4D5O6P - H2O + adductmass, 300R, "C3H9O6P - H2O") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(lipidMass - C6H13O9P, 500R, "[M+H]+ -Header") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(lipidMass - C6H10O5, 100R, "[M+H]+ -C6H10O5") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(lipidMass - C6H10O5 - H2O, 150R, "[M+H]+ -C6H12O6") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(lipidMass - H2O, 800, "[M+H]+ -H2O")
}
            If Equals(adduct.AdductIonName, "[M+H]+") Then
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}})
            ElseIf Equals(adduct.AdductIonName, "[M+NH4]+") Then
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 800R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}, New SpectrumPeak(lipidMass, 999R, "[M+H]+ ") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
            End If
            'else if (adduct.AdductIonName == "[M+Na]+")
            '{
            '    spectrum.Add(
            '        new SpectrumPeak(adduct.ConvertToMz(lipid.Mass - C6H10O5), 500d, "Precursor -C6H10O5") { SpectrumComment = SpectrumComment.metaboliteclass }
            '    );
            '}
            Return spectrum.ToArray()
        End Function

        Private Function GetAcylDoubleBondSpectrum(lipid As ILipid, acylChains As IEnumerable(Of AcylChain), adduct As AdductIon) As IEnumerable(Of SpectrumPeak)
            Dim nlMass = adduct.AdductIonAccurateMass - ProtonMass + H2O
            Return acylChains.SelectMany(Function(acylChain) spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acylChain, adduct, nlMass, 50R))
        End Function

        Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChains As IEnumerable(Of IChain), adduct As AdductIon) As IEnumerable(Of SpectrumPeak)
            Return acylChains.SelectMany(Function(acylChain) GetAcylLevelSpectrum(lipid, acylChain, adduct))
        End Function

        Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
            Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
            Dim lipidMass = lipid.Mass + adductmass
            Dim chainMass = acylChain.Mass - HydrogenMass
            Dim spectrum = New List(Of SpectrumPeak)()
            If chainMass <> 0.0 Then
                spectrum.AddRange({New SpectrumPeak(lipidMass - chainMass, 150R, $"-{acylChain}") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass - H2O, 100R, $"-{acylChain} -H2O") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + ProtonMass, 100R, $"{acylChain} acyl+") With {
.SpectrumComment = SpectrumComment.acylchain
}})
            End If
            Return spectrum.ToArray()
        End Function

        Private Function GetAcylPositionSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
            Dim lipidMass = lipid.Mass - HydrogenMass
            Dim chainMass = acylChain.Mass
            Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)

            Return {New SpectrumPeak(lipidMass - chainMass + adductmass - OxygenMass - CD2, 100R, "-CD2(Sn1)") With {
.SpectrumComment = SpectrumComment.snposition
}}
        End Function


        Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()

End Class
