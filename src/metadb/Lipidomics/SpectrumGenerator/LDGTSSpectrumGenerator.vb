Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Public Class LDGTSSpectrumGenerator
    Implements ILipidSpectrumGenerator
    Private Shared ReadOnly C7H13NO2 As Double = {CarbonMass * 7, HydrogenMass * 13, NitrogenMass, OxygenMass * 2}.Sum()

    Private Shared ReadOnly C8H16NO3 As Double = {CarbonMass * 8, HydrogenMass * 16, NitrogenMass, OxygenMass * 3}.Sum() '175[M+H]+

    Private Shared ReadOnly CHO2 As Double = {CarbonMass * 1, HydrogenMass * 1, OxygenMass * 2}.Sum()

    Private Shared ReadOnly Gly_C As Double = {CarbonMass * 10, HydrogenMass * 19, NitrogenMass, OxygenMass * 3}.Sum()

    Private Shared ReadOnly Gly_O As Double = {CarbonMass * 9, HydrogenMass * 17, NitrogenMass, OxygenMass * 4}.Sum()

    Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()

    Private Shared ReadOnly CH2 As Double = {HydrogenMass * 2, CarbonMass}.Sum()

    Public Sub New()
        spectrumGenerator = New SpectrumPeakGenerator()
    End Sub

    Public Sub New(spectrumGenerator As ISpectrumPeakGenerator)
        Me.spectrumGenerator = If(spectrumGenerator, CSharpImpl.__Throw(Of ISpectrumPeakGenerator)(New ArgumentNullException(NameOf(spectrumGenerator))))
    End Sub

    Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator

    Public Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
        If lipid.LipidClass = LbmClass.LDGTS Then
            If Equals(adduct.AdductIonName, "[M+H]+") Then
                Return True
            End If
        End If
        Return False
    End Function

    Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
        Dim spectrum = New List(Of SpectrumPeak)()
        spectrum.AddRange(GetLDGTSSpectrum(lipid, adduct))
        If lipid.Description.Has(LipidDescription.Chain) Then
            spectrum.AddRange(GetAcylLevelSpectrum(lipid, lipid.Chains.GetDeterminedChains(), adduct))
            lipid.Chains.ApplyToChain(1, Sub(chain) spectrum.AddRange(GetAcylPositionSpectrum(lipid, chain, adduct)))
            spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, lipid.Chains.GetTypedChains(Of AcylChain)().Where(Function(c) c.DoubleBond.UnDecidedCount = 0 AndAlso c.Oxidized.UnDecidedCount = 0), adduct))
        End If
        spectrum = spectrum.GroupBy(Function(spec) spec, comparer).[Select](Function(specs) New SpectrumPeak(Enumerable.First(specs).Mass, specs.Sum(Function(n) n.Intensity), String.Join(", ", specs.[Select](Function(spec) spec.Comment)), specs.Aggregate(SpectrumComment.none, Function(a, b) a Or b.SpectrumComment))).OrderBy(Function(peak) peak.Mass).ToList()
        Return CreateReference(lipid, adduct, spectrum, molecule)
    End Function

    Private Function CreateReference(lipid As ILipid, adduct As AdductIon, spectrum As List(Of SpectrumPeak), molecule As IMoleculeProperty) As MoleculeMsReference
        Return New MoleculeMsReference With {
.PrecursorMz = adduct.ConvertToMz(lipid.Mass),
.IonMode = adduct.IonMode,
.spectrum = spectrum,
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

    Private Function GetLDGTSSpectrum(lipid As ILipid, adduct As AdductIon) As SpectrumPeak()
        Dim spectrum = New List(Of SpectrumPeak) From {
New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
    .SpectrumComment = SpectrumComment.precursor
},
New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - CHO2), 200.0R, "Precursor - CO2") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
},
New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - H2O), 200.0R, "Precursor - H2O") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
},
New SpectrumPeak(adduct.ConvertToMz(C8H16NO3), 100.0R, "C8H16NO3") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
}, '175[M+H]+
New SpectrumPeak(adduct.ConvertToMz(C7H13NO2), 100.0R, "Header") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
}, '144[M+H]+
New SpectrumPeak(adduct.ConvertToMz(C7H13NO2 + H2O), 200.0R, "Header + H2O") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
}, '162[M+H]+
New SpectrumPeak(adduct.ConvertToMz(C7H13NO2 - CH2), 200.0R, "Header - CH2") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
}, ' 130[M+H]+
New SpectrumPeak(adduct.ConvertToMz(C7H13NO2 - CH2 * 2 + HydrogenMass), 200.0R, "Header - C2H3") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
}, ' 117[M+H]+
    'new SpectrumPeak(adduct.ConvertToMz(Gly_C), 150d, "Gly-C")  { SpectrumComment = SpectrumComment.metaboliteclass },
    New SpectrumPeak(adduct.ConvertToMz(Gly_O), 50R, "Gly-O") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    }
}
            Return spectrum.ToArray()
    End Function

    Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChains As IEnumerable(Of IChain), adduct As AdductIon) As IEnumerable(Of SpectrumPeak)
        Return acylChains.SelectMany(Function(acylChain) GetAcylLevelSpectrum(lipid, acylChain, adduct))
    End Function

    Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
        Dim lipidMass = lipid.Mass
        Dim chainMass = acylChain.Mass - HydrogenMass
        Return {New SpectrumPeak(adduct.ConvertToMz(lipidMass - chainMass), 300.0R, $"-{acylChain}") With {
.SpectrumComment = SpectrumComment.acylchain,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}, New SpectrumPeak(adduct.ConvertToMz(lipidMass - chainMass - H2O - HydrogenMass), 100.0R, $"-{acylChain}-O") With {
.SpectrumComment = SpectrumComment.acylchain
}}
    End Function

    Private Function GetAcylPositionSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
        Dim lipidMass = lipid.Mass
        Dim chainMass = acylChain.Mass
        Return {New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - chainMass - OxygenMass - CH2 - HydrogenMass), 100.0R, "-CH2(Sn1)") With {
.SpectrumComment = SpectrumComment.snposition
}}
    End Function

    Private Function GetAcylDoubleBondSpectrum(lipid As ILipid, acylChains As IEnumerable(Of AcylChain), adduct As AdductIon) As IEnumerable(Of SpectrumPeak)
        Return acylChains.SelectMany(Function(acylChain) spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acylChain, adduct, 0R, 50.0R))
    End Function

    Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()

End Class

