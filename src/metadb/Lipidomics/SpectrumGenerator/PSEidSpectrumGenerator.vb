Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports BioNovoGene.BioDeep.MSEngine

Public Class PSEidSpectrumGenerator
    Implements ILipidSpectrumGenerator

    Private Shared ReadOnly C3H8NO6P As Double = {CarbonMass * 3, HydrogenMass * 8, NitrogenMass, OxygenMass * 6, PhosphorusMass}.Sum()

    Private Shared ReadOnly CHO2 As Double = {CarbonMass * 1, HydrogenMass * 1, OxygenMass * 2}.Sum()

    Private Shared ReadOnly C3H5NO2 As Double = {CarbonMass * 3, HydrogenMass * 5, NitrogenMass, OxygenMass * 2}.Sum()

    Private Shared ReadOnly Gly_C As Double = {CarbonMass * 6, HydrogenMass * 12, NitrogenMass, OxygenMass * 6, PhosphorusMass}.Sum()

    Private Shared ReadOnly Gly_O As Double = {CarbonMass * 5, HydrogenMass * 10, NitrogenMass, OxygenMass * 7, PhosphorusMass}.Sum()

    Private Shared ReadOnly CH2 As Double = {HydrogenMass * 2, CarbonMass}.Sum()

    Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()

    Public Sub New()
        spectrumGenerator = New SpectrumPeakGenerator()
    End Sub

    Public Sub New(spectrumGenerator As ISpectrumPeakGenerator)
        Me.spectrumGenerator = If(spectrumGenerator, CSharpImpl.__Throw(Of ISpectrumPeakGenerator)(New ArgumentNullException(NameOf(spectrumGenerator))))
    End Sub

    Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator

    Public Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
        If lipid.LipidClass = LbmClass.PS Then
            If Equals(adduct.AdductIonName, "[M+H]+") Then
                Return True
            End If
        End If
        Return False
    End Function

    Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
        Dim spectrum = New List(Of SpectrumPeak)()
        spectrum.AddRange(GetPSSpectrum(lipid, adduct))
        If lipid.Description.Has(LipidDescription.Chain) Then
            spectrum.AddRange(GetAcylLevelSpectrum(lipid, lipid.Chains.GetDeterminedChains(), adduct))
            lipid.Chains.ApplyToChain(1, Sub(chain) spectrum.AddRange(GetAcylPositionSpectrum(lipid, chain, adduct)))
            spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, lipid.Chains.GetTypedChains(Of AcylChain)(), adduct, nlMass:=C3H8NO6P))
            spectrum.AddRange(EidSpecificSpectrum(lipid, adduct, nlMass:=C3H8NO6P, 200.0R))
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

    Private Function GetPSSpectrum(lipid As ILipid, adduct As AdductIon) As SpectrumPeak()
        Dim spectrum = New List(Of SpectrumPeak) From {
New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
    .SpectrumComment = SpectrumComment.precursor
},
New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - H2O), 100.0R, "Precursor -H2O") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
},
New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - CHO2), 200.0R, "Precursor -CHO2") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
},
New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - C3H8NO6P), 500.0R, "Precursor -C3H8NO6P") With {
    .SpectrumComment = SpectrumComment.metaboliteclass,
    .IsAbsolutelyRequiredFragmentForAnnotation = True
},
New SpectrumPeak(adduct.ConvertToMz(C3H8NO6P), 100.0R, "Header") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
},
New SpectrumPeak(adduct.ConvertToMz(Gly_C), 100.0R, "Gly-C") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
},
New SpectrumPeak(adduct.ConvertToMz(Gly_O), 100.0R, "Gly-O") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
}
}

        Return spectrum.ToArray()
    End Function

    Private Function GetAcylDoubleBondSpectrum(lipid As ILipid, acylChains As IEnumerable(Of AcylChain), adduct As AdductIon, Optional nlMass As Double = 0.0) As IEnumerable(Of SpectrumPeak)
        Return acylChains.SelectMany(Function(acylChain) spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acylChain, adduct, nlMass, 30.0R))
    End Function

    Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChains As IEnumerable(Of IChain), adduct As AdductIon) As IEnumerable(Of SpectrumPeak)
        Return acylChains.SelectMany(Function(acylChain) GetAcylLevelSpectrum(lipid, acylChain, adduct))
    End Function

    Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
        Dim chainMass = acylChain.Mass - HydrogenMass
        Return {New SpectrumPeak(chainMass + ProtonMass, 100.0R, $"{acylChain} acyl") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - chainMass), 50.0R, $"-{acylChain}") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - chainMass - HydrogenMass - OxygenMass), 50.0R, $"-{acylChain}-O") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - chainMass - C3H8NO6P), 150.0R, $"-Header -{acylChain}") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - chainMass - C3H8NO6P - HydrogenMass - OxygenMass), 50.0R, $"-Header -{acylChain}-O") With {
.SpectrumComment = SpectrumComment.acylchain
}}
    End Function

    Private Function GetAcylPositionSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
        Dim chainMass = acylChain.Mass - HydrogenMass
        Return {New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - chainMass - HydrogenMass - OxygenMass - CH2), 100.0R, "-CH2(Sn1)") With {
.SpectrumComment = SpectrumComment.snposition'new SpectrumPeak(adduct.ConvertToMz(lipid.Mass - chainMass - C3H8NO6P - MassDiffDictionary.HydrogenMass - MassDiffDictionary.OxygenMass - CH2), 100d, "-Header -CH2(Sn1)") { SpectrumComment = SpectrumComment.snposition },
}}
    End Function
    Private Shared Function EidSpecificSpectrum(lipid As Lipid, adduct As AdductIon, nlMass As Double, intensity As Double) As SpectrumPeak()
        Dim spectrum = New List(Of SpectrumPeak)()
        Dim chains As SeparatedChains = Nothing

        If CSharpImpl.__Assign(chains, TryCast(lipid.Chains, SeparatedChains)) IsNot Nothing Then
            For Each chain In lipid.Chains.GetDeterminedChains()
                If chain.DoubleBond.Count = 0 OrElse chain.DoubleBond.UnDecidedCount > 0 Then Continue For
                If chain.DoubleBond.Count <= 3 Then
                    intensity = intensity * 0.1
                End If
                spectrum.AddRange(EidSpecificSpectrumGenerator.EidSpecificSpectrumGen(lipid, chain, adduct, nlMass, intensity))
            Next
        End If
        Return spectrum.ToArray()
    End Function


    Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()

End Class

