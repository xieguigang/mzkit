Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports BioNovoGene.BioDeep.MSEngine

Public Class CLSpectrumGenerator
    Implements ILipidSpectrumGenerator
    'CL explain rule -> CL 2 chain(sn1,sn2)/2 chain(sn3,sn4)
    'CL sn1_sn2_sn3_sn4 (follow the rules of alignment) -- MolecularSpeciesLevelChains
    'CL sn1_sn2/sn3_sn4 -- MolecularSpeciesLevelChains <- cannot parsing now
    'CL sn1/sn2/sn3/sn4  -- PositionLevelChains 

    Private Shared ReadOnly C3H6O2 As Double = {CarbonMass * 3, HydrogenMass * 6, OxygenMass * 2}.Sum()

    Private Shared ReadOnly C3H3O2 As Double = {CarbonMass * 3, HydrogenMass * 3, OxygenMass * 2}.Sum()

    Private Shared ReadOnly CH2 As Double = {HydrogenMass * 2, CarbonMass}.Sum()

    Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()

    Public Sub New()
        spectrumGenerator = New SpectrumPeakGenerator()
    End Sub

    Public Sub New(spectrumGenerator As ISpectrumPeakGenerator)
        Me.spectrumGenerator = spectrumGenerator
    End Sub

    Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator

    Public Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
        If lipid.LipidClass = LbmClass.CL Then
            If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+NH4]+") Then
                Return True
            End If
        End If
        Return False
    End Function
    Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
        Dim nlMass = adduct.AdductIonAccurateMass - ProtonMass
        Dim spectrum = New List(Of SpectrumPeak)()
        spectrum.AddRange(GetCLSpectrum(lipid, adduct))

        Dim c1 As AcylChain = TryCast(lipid.Chains.GetChainByPosition(1), AcylChain)
        Dim c2 As AcylChain = TryCast(lipid.Chains.GetChainByPosition(2), AcylChain)
        Dim c3 As AcylChain = TryCast(lipid.Chains.GetChainByPosition(3), AcylChain)
        Dim c4 As AcylChain = TryCast(lipid.Chains.GetChainByPosition(4), AcylChain)

        If c1 IsNot Nothing AndAlso c2 IsNot Nothing AndAlso c3 IsNot Nothing AndAlso c4 IsNot Nothing Then
            Dim sn1sn2 = {c1, c2}
            Dim sn3sn4 = {c3, c4}
            spectrum.AddRange(GetAcylLevelSpectrum(lipid, lipid.Chains.GetDeterminedChains(), adduct, nlMass + H2O))
            'spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, lipid.Chains.GetTypedChains<AcylChain>().Where(c => c.DoubleBond.UnDecidedCount == 0 && c.Oxidized.UnDecidedCount == 0), adduct));
            Dim sn1sn2mass = lipid.Mass - (c1.Mass + c2.Mass + C3H3O2 + HydrogenMass)
            Dim sn3sn4mass = lipid.Mass - (c3.Mass + c4.Mass + C3H3O2 + HydrogenMass)
            spectrum.AddRange(GetAcylPositionSpectrum(lipid, c1, adduct, sn3sn4mass + nlMass))
            spectrum.AddRange(GetAcylPositionSpectrum(lipid, c3, adduct, sn1sn2mass + nlMass))
            spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, sn1sn2.Where(Function(c) c.DoubleBond.UnDecidedCount = 0 AndAlso c.Oxidized.UnDecidedCount = 0), adduct, sn3sn4mass + nlMass))
            spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, sn3sn4.Where(Function(c) c.DoubleBond.UnDecidedCount = 0 AndAlso c.Oxidized.UnDecidedCount = 0), adduct, sn1sn2mass + nlMass))
        End If
        spectrum = spectrum.GroupBy(Function(spec) spec, comparer).[Select](Function(specs) New SpectrumPeak(Enumerable.First(specs).mz, specs.Sum(Function(n) n.Intensity), String.Join(", ", specs.[Select](Function(spec) spec.Annotation)), specs.Aggregate(SpectrumComment.none, Function(a, b) a Or b.SpectrumComment))).OrderBy(Function(peak) peak.mz).ToList()
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
    Private Function GetCLSpectrum(lipid As ILipid, adduct As AdductIon) As SpectrumPeak()
        Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
        Dim spectrum = New List(Of SpectrumPeak) From {
New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
    .SpectrumComment = SpectrumComment.precursor
},
New SpectrumPeak(lipid.Mass + adductmass - H2O, 100.0R, "Precursor -H2O") With {
    .SpectrumComment = SpectrumComment.metaboliteclass,
    .IsAbsolutelyRequiredFragmentForAnnotation = True
}
}
        If Equals(adduct.AdductIonName, "[M+NH4]+") Then
            spectrum.Add(New SpectrumPeak(lipid.Mass + ProtonMass, 200.0R, "[M+H]+") With {
.SpectrumComment = SpectrumComment.metaboliteclass
})
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetAcylDoubleBondSpectrum(lipid As ILipid, acylChains As IEnumerable(Of AcylChain), adduct As AdductIon, Optional nlMass As Double = 0.0) As IEnumerable(Of SpectrumPeak)
        Return acylChains.SelectMany(Function(acylChain) spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acylChain, adduct, nlMass, 10.0R))
    End Function

    Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChains As IEnumerable(Of IChain), adduct As AdductIon, nlMass As Double) As IEnumerable(Of SpectrumPeak)
        Dim lipidMass = lipid.Mass
        Dim acylChainsArr = acylChains.ToArray()
        Dim sn1sn2mass = acylChainsArr(0).Mass + acylChainsArr(1).Mass + C3H3O2 + HydrogenMass
        Dim sn3sn4mass = acylChainsArr(2).Mass + acylChainsArr(3).Mass + C3H3O2 + HydrogenMass

        Dim spectrum = New List(Of SpectrumPeak) From {
New SpectrumPeak(lipidMass - sn1sn2mass + ProtonMass, 80.0R, $"[M-Sn1-Sn2-C3H3O2+H]+") With {
    .SpectrumComment = SpectrumComment.acylchain
},
New SpectrumPeak(lipidMass - sn3sn4mass + ProtonMass, 80.0R, $"[M-Sn3-Sn4-C3H3O2+H]+") With {
    .SpectrumComment = SpectrumComment.acylchain
},
New SpectrumPeak(lipidMass - sn1sn2mass + ProtonMass - H2O, 80.0R, $"[M-Sn1-Sn2-C3H3O2-H2O+H]+") With {
    .SpectrumComment = SpectrumComment.acylchain
},
New SpectrumPeak(lipidMass - sn3sn4mass + ProtonMass - H2O, 80.0R, $"[M-Sn3-Sn4-C3H3O2-H2O+H]+") With {
    .SpectrumComment = SpectrumComment.acylchain
},
New SpectrumPeak(sn1sn2mass + ProtonMass, 300.0R, $"[Sn1+Sn2+C3H3O2+H]+") With {
    .SpectrumComment = SpectrumComment.acylchain
},
New SpectrumPeak(sn3sn4mass + ProtonMass, 300.0R, $"[Sn3+Sn4+C3H3O2+H]+") With {
    .SpectrumComment = SpectrumComment.acylchain
}
}
        spectrum.AddRange(acylChains.SelectMany(Function(acylChain) GetAcylLevelSpectrum(lipid, acylChain, adduct, nlMass)))

        Return spectrum.ToArray()
    End Function

    Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon, nlMass As Double) As SpectrumPeak()
        Dim lipidMass = lipid.Mass
        Dim chainMass = acylChain.Mass - HydrogenMass

        Dim spectrum = New List(Of SpectrumPeak) From {
New SpectrumPeak(chainMass + ProtonMass, 50.0R, $"{acylChain} acyl+") With {
    .SpectrumComment = SpectrumComment.acylchain
},
New SpectrumPeak(chainMass + C3H6O2 + ProtonMass, 50.0R, $"{acylChain} +C3H6O2") With {
    .SpectrumComment = SpectrumComment.acylchain
},
New SpectrumPeak(chainMass + C3H6O2 - OxygenMass + ProtonMass, 50.0R, $"{acylChain} +C3H6O") With {
    .SpectrumComment = SpectrumComment.acylchain
},
New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - chainMass - nlMass), 50.0R, $"-{acylChain}") With {
    .SpectrumComment = SpectrumComment.acylchain
}
}

        Return spectrum.ToArray()
    End Function


    Private Function GetAcylPositionSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon, nlMass As Double) As SpectrumPeak()
        Dim lipidMass = lipid.Mass + adduct.AdductIonAccurateMass
        Dim chainMass = acylChain.Mass - HydrogenMass
        Dim spectrum = New List(Of SpectrumPeak) From {
New SpectrumPeak(lipidMass - nlMass - chainMass - CH2, 20.0R, "-CH2(Sn1)") With {
    .SpectrumComment = SpectrumComment.snposition
}
}
        Return spectrum.ToArray()
    End Function

    Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()

End Class
