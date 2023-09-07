Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemistry.MetaLib
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

Public Class PGd5SpectrumGenerator
        Implements ILipidSpectrumGenerator

        Private Shared ReadOnly C3H9O6P As Double = {CarbonMass * 3, HydrogenMass * 9, OxygenMass * 6, PhosphorusMass}.Sum()

        Private Shared ReadOnly C3H6O2 As Double = {CarbonMass * 3, HydrogenMass * 6, OxygenMass * 2}.Sum()

        Private Shared ReadOnly Gly_C As Double = {CarbonMass * 6, HydrogenMass * 8, OxygenMass * 6, PhosphorusMass, Hydrogen2Mass * 5}.Sum()

        Private Shared ReadOnly Gly_O As Double = {CarbonMass * 5, HydrogenMass * 8, OxygenMass * 7, PhosphorusMass, Hydrogen2Mass * 3}.Sum()

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
            If lipid.LipidClass = LbmClass.PG_d5 Then
                If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+Na]+") OrElse Equals(adduct.AdductIonName, "[M+NH4]+") Then
                    Return True
                End If
            End If
            Return False
        End Function

        Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
            Dim nlMass = If(Equals(adduct.AdductIonName, "[M+Na]+"), 0.0, adduct.ConvertToMz(C3H9O6P) - ProtonMass)
            Dim spectrum = New List(Of SpectrumPeak)()
            spectrum.AddRange(GetPGSpectrum(lipid, adduct))
            If lipid.Description.Has(LipidDescription.Chain) Then
                spectrum.AddRange(GetAcylLevelSpectrum(lipid, lipid.Chains.GetDeterminedChains(), adduct))
                lipid.Chains.ApplyToChain(1, Sub(chain) spectrum.AddRange(GetAcylPositionSpectrum(lipid, chain, adduct)))
                spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, lipid.Chains.GetTypedChains(Of AcylChain)(), adduct, nlMass:=nlMass))
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

        Private Function GetPGSpectrum(lipid As ILipid, adduct As AdductIon) As SpectrumPeak()
            Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
            Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999R, "Precursor") With {
        .SpectrumComment = SpectrumComment.precursor
    },
    New SpectrumPeak(lipid.Mass - C3H9O6P + adductmass, 500R, "Precursor -C3H9O6P") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    },
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - C3H6O2), 100R, "Precursor -C3H6O2") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(C3H9O6P + adductmass, 100R, "Header") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(Gly_C + adductmass, 400R, "Gly-C") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(Gly_O + adductmass, 350R, "Gly-O") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    }
}
            If Equals(adduct.AdductIonName, "[M+NH4]+") Then
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - H2O), 100R, "Precursor -H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(lipid.Mass + ProtonMass, 200R, "[M+H]+") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
            ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) / 2, 50R, "Precursor2+") With {
.SpectrumComment = SpectrumComment.precursor
}})
            End If
            Return spectrum.ToArray()
        End Function

        Private Function GetAcylDoubleBondSpectrum(lipid As ILipid, acylChains As IEnumerable(Of AcylChain), adduct As AdductIon, Optional nlMass As Double = 0.0) As IEnumerable(Of SpectrumPeak)
            Return acylChains.SelectMany(Function(acylChain) spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acylChain, adduct, nlMass, 30R))
        End Function

        Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChains As IEnumerable(Of IChain), adduct As AdductIon) As IEnumerable(Of SpectrumPeak)
            Return acylChains.SelectMany(Function(acylChain) GetAcylLevelSpectrum(lipid, acylChain, adduct))
        End Function

        Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
            Dim lipidMass = lipid.Mass
            Dim chainMass = acylChain.Mass - HydrogenMass
            Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
            Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(chainMass + ProtonMass, 100R, $"{acylChain} acyl") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(lipidMass - chainMass + adductmass, 100R, $"-{acylChain}") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(lipidMass - chainMass - H2O + adductmass, 100R, $"-{acylChain}-H2O") With {
        .SpectrumComment = SpectrumComment.acylchain
    }
}
            If Equals(adduct.AdductIonName, "[M+NH4]+") Then
                spectrum.AddRange({New SpectrumPeak(lipidMass - chainMass - C3H9O6P + adductmass, 200R, $"-Header -{acylChain}") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass - C3H9O6P - H2O + adductmass, 100R, $"-Header -{acylChain}-H2O") With {
.SpectrumComment = SpectrumComment.acylchain
}})
            End If
            Return spectrum.ToArray()
        End Function

        Private Function GetAcylPositionSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
            Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
            Dim lipidMass = lipid.Mass + adductmass
            Dim chainMass = acylChain.Mass - HydrogenMass * 2
            Return {New SpectrumPeak(lipidMass - chainMass - H2O - CD2, 100R, "-CD2(Sn1)") With {
.SpectrumComment = SpectrumComment.snposition
}, New SpectrumPeak(lipidMass - chainMass - H2O * 2 - CD2, 100R, "-H2O -CD2(Sn1)") With {
.SpectrumComment = SpectrumComment.snposition
}, New SpectrumPeak(lipidMass - chainMass - C3H9O6P - H2O - CD2, 100R, "-Header -CD2(Sn1)") With {
.SpectrumComment = SpectrumComment.snposition
}}
        End Function


        Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()

End Class

