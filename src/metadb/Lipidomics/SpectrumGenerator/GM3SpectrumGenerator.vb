Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Public Class GM3SpectrumGenerator
    Implements ILipidSpectrumGenerator
    Private Shared ReadOnly C11H15NO7 As Double = {CarbonMass * 11, HydrogenMass * 15, NitrogenMass, OxygenMass * 7}.Sum()
    Private Shared ReadOnly C6H10O5 As Double = {CarbonMass * 6, HydrogenMass * 10, OxygenMass * 5}.Sum()
    Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()
    Private Shared ReadOnly CH4O2 As Double = {CarbonMass * 1, HydrogenMass * 4, OxygenMass * 2}.Sum()
    Private Shared ReadOnly C2H3NO As Double = {CarbonMass * 2, HydrogenMass * 3, NitrogenMass * 1, OxygenMass * 1}.Sum()
    Private Shared ReadOnly C2H3N As Double = {CarbonMass * 2, HydrogenMass * 3, NitrogenMass * 1}.Sum()

    Public Sub New()
        spectrumGenerator = New SpectrumPeakGenerator()
    End Sub
    Public Sub New(spectrumGenerator As ISpectrumPeakGenerator)
        Me.spectrumGenerator = If(spectrumGenerator, CSharpImpl.__Throw(Of ISpectrumPeakGenerator)(New ArgumentNullException(NameOf(spectrumGenerator))))
    End Sub

    Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator

    Public Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
        If lipid.LipidClass = LbmClass.GM3 Then
            If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+NH4]+") OrElse Equals(adduct.AdductIonName, "[M+Na]+") Then
                Return True
            End If
        End If
        Return False
    End Function

    Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
        Dim spectrum = New List(Of SpectrumPeak)()
        Dim nlmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), adduct.AdductIonAccurateMass, 0.0)
        spectrum.AddRange(GetGM3Spectrum(lipid, adduct))
        Dim sphingo As SphingoChain = Nothing

        If CSharpImpl.__Assign(sphingo, TryCast(lipid.Chains.GetChainByPosition(1), SphingoChain)) IsNot Nothing Then
            spectrum.AddRange(GetSphingoSpectrum(lipid, sphingo, adduct))
            spectrum.AddRange(spectrumGenerator.GetSphingoDoubleBondSpectrum(lipid, sphingo, adduct, nlmass, 10.0R))
        End If
        Dim acyl As AcylChain = Nothing

        If CSharpImpl.__Assign(acyl, TryCast(lipid.Chains.GetChainByPosition(2), AcylChain)) IsNot Nothing Then
            'spectrum.AddRange(GetAcylSpectrum(lipid, acyl, adduct));
            spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acyl, adduct, nlmass, 10.0R))
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

    Private Function GetGM3Spectrum(lipid As ILipid, adduct As AdductIon) As SpectrumPeak()
        Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
        Dim h2oLossMass = lipid.Mass + adductmass - H2O
        Dim spectrum = New List(Of SpectrumPeak) From {
New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
    .SpectrumComment = SpectrumComment.precursor
},
New SpectrumPeak(h2oLossMass - HydrogenMass, 100.0R, "[M-H2O-H+H]+") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
},
New SpectrumPeak(h2oLossMass - C11H15NO7 - C6H10O5 - H2O, 150.0R, "[M-H2O-C17H27NO13+adduct]+") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
},
New SpectrumPeak(C11H15NO7 + C6H10O5 * 2 + H2O * 2 + C2H3N + adductmass, 300.0R, "[C23H35NO17 +C2H3N +2H2O +adduct]+") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
} '675
}
        If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+NH4]+") Then
            spectrum.AddRange({New SpectrumPeak(h2oLossMass - C11H15NO7, 100.0R, "[M-H2O-C11H15NO7+H]+") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(h2oLossMass - C11H15NO7 - H2O, 100.0R, "[M-H2O-C11H17NO8+H]+") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(h2oLossMass - C11H15NO7 - C6H10O5, 150.0R, "[M-H2O-C17H25NO12+H]+") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(h2oLossMass - C11H15NO7 - C6H10O5 * 2, 150.0R, "[M-H2O-C23H35NO17+H]+") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(h2oLossMass - C11H15NO7 - C6H10O5 * 2 - H2O, 300.0R, "[M-H2O-C23H37NO18+H]+") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}, New SpectrumPeak(C11H15NO7 + C6H10O5 + H2O + adductmass, 300.0R, "[C17H27NO13+H]+") With {'548
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}, New SpectrumPeak(C11H15NO7 + H2O + adductmass, 300.0R, "[C11H17NO8+H]+") With { '454
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}, New SpectrumPeak(C11H15NO7 + adductmass, 300.0R, "[C11H15NO7+H]+") With { '292
.SpectrumComment = SpectrumComment.metaboliteclass
}}) '274
        End If
        If Equals(adduct.AdductIonName, "[M+NH4]+") Then
            spectrum.AddRange({New SpectrumPeak(lipid.Mass + adductmass, 500.0R, "[M+H]+") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
        ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
            spectrum.AddRange({New SpectrumPeak(h2oLossMass - C11H15NO7 - HydrogenMass * 2, 100.0R, "[M-H2O-C11H17NO7+Na]+") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(h2oLossMass - C11H15NO7 - H2O + HydrogenMass, 100.0R, "[M-H2O-C11H16NO8+Na]+") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(h2oLossMass - C11H15NO7 - C6H10O5 + CarbonMass + OxygenMass, 150.0R, "[M-H2O-C16H25NO11+Na]+") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(adduct.ConvertToMz(C11H15NO7 + C6H10O5 + H2O + OxygenMass), 300.0R, "[C17H27NO14+Na]+") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}, New SpectrumPeak(adduct.ConvertToMz(C11H15NO7 - CarbonMass + HydrogenMass * 2), 500.0R, "[C10H17NO7+Na]+") With { '454
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}, New SpectrumPeak(adduct.ConvertToMz(C11H15NO7 - CarbonMass + HydrogenMass * 2), 200.0R, "[C10H17NO7+Na]+") With { '292
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}}) '292
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetSphingoSpectrum(lipid As ILipid, sphingo As SphingoChain, adduct As AdductIon) As SpectrumPeak()
        Dim chainMass = sphingo.Mass + HydrogenMass
        Dim spectrum = New List(Of SpectrumPeak)()
        If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+NH4]+") Then
            spectrum.AddRange({New SpectrumPeak(chainMass + ProtonMass - H2O * 2, 300.0R, "[sph+H]+ -Header -H2O") With {
.SpectrumComment = SpectrumComment.acylchain
'new SpectrumPeak(chainMass + MassDiffDictionary.ProtonMass - CH4O2, 100d, "[sph+H]+ -CH4O2"),
}})
            End If
        Return spectrum.ToArray()
    End Function

    Private Function GetAcylSpectrum(lipid As ILipid, acyl As AcylChain, adduct As AdductIon) As SpectrumPeak()
        Dim chainMass = acyl.Mass + HydrogenMass
        Dim spectrum = New List(Of SpectrumPeak)() From {
'new SpectrumPeak(adduct.ConvertToMz(chainMass) +C2H2N , 200d, "[FAA+C2H+adduct]+") { SpectrumComment = SpectrumComment.acylchain },
}
            Return spectrum.ToArray()
    End Function

    Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()

End Class

