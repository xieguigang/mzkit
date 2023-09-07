Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemistry.MetaLib
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

Public Class CeramidePhytoSphSpectrumGenerator
    Implements ILipidSpectrumGenerator
    Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()
    Private Shared ReadOnly CH6O2 As Double = {CarbonMass * 1, HydrogenMass * 6, OxygenMass * 2}.Sum()
    Private Shared ReadOnly C3H4NO As Double = {CarbonMass * 3, HydrogenMass * 4, NitrogenMass * 1, OxygenMass * 1}.Sum()
    Private Shared ReadOnly C2H3NO As Double = {CarbonMass * 2, HydrogenMass * 3, NitrogenMass * 1, OxygenMass * 1}.Sum()
    Private Shared ReadOnly NH As Double = {NitrogenMass * 1, HydrogenMass * 1}.Sum()
    Private Shared ReadOnly CH3O As Double = {CarbonMass * 1, HydrogenMass * 3, OxygenMass * 1}.Sum()
    Private Shared ReadOnly CH4O2 As Double = {CarbonMass * 1, HydrogenMass * 4, OxygenMass * 2}.Sum()

    Public Sub New()
        spectrumGenerator = New SpectrumPeakGenerator()
    End Sub
    Public Sub New(spectrumGenerator As ISpectrumPeakGenerator)
        Me.spectrumGenerator = If(spectrumGenerator, CSharpImpl.__Throw(Of ISpectrumPeakGenerator)(New ArgumentNullException(NameOf(spectrumGenerator))))
    End Sub

    Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator

    Public Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
        If lipid.LipidClass = LbmClass.Cer_NP OrElse lipid.LipidClass = LbmClass.Cer_AP Then
            If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+H-H2O]+") Then
                Return True
            End If
        End If
        Return False
    End Function

    Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
        Dim spectrum = New List(Of SpectrumPeak)()
        Dim nlmass = 0.0
        spectrum.AddRange(GetCerNSSpectrum(lipid, adduct))
        Dim sphingo As SphingoChain = Nothing

        If CSharpImpl.__Assign(sphingo, TryCast(lipid.Chains.GetChainByPosition(1), SphingoChain)) IsNot Nothing Then
            spectrum.AddRange(GetSphingoSpectrum(lipid, sphingo, adduct))
            spectrum.AddRange(spectrumGenerator.GetSphingoDoubleBondSpectrum(lipid, sphingo, adduct, nlmass, 30.0R))
        End If
        Dim acyl As AcylChain = Nothing

        If CSharpImpl.__Assign(acyl, TryCast(lipid.Chains.GetChainByPosition(2), AcylChain)) IsNot Nothing Then
            spectrum.AddRange(GetAcylSpectrum(lipid, acyl, adduct))
            spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acyl, adduct, nlmass, 30.0R))
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

    Private Function GetCerNSSpectrum(lipid As ILipid, adduct As AdductIon) As SpectrumPeak()
        Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
        .SpectrumComment = SpectrumComment.precursor
    }
}
        If Equals(adduct.AdductIonName, "[M+Na]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - CH3O, 150.0R, "Precursor-CH3O") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}})
        Else
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - H2O, 200.0R, "Precursor-H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}, New SpectrumPeak(lipid.Mass + ProtonMass - CH6O2, 100.0R, "[M+H]+ -CH6O2") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}})
            If Equals(adduct.AdductIonName, "[M+H]+") Then
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - H2O * 2, 100.0R, "Precursor-2H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
            End If
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetSphingoSpectrum(lipid As ILipid, sphingo As SphingoChain, adduct As AdductIon) As SpectrumPeak()
        Dim chainMass = sphingo.Mass + HydrogenMass
        Dim spectrum = New List(Of SpectrumPeak)()
        If Not Equals(adduct.AdductIonName, "[M+Na]+") Then
            spectrum.AddRange({New SpectrumPeak(chainMass + ProtonMass - H2O, 200.0R, "[sph+H]+ -H2O") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + ProtonMass - H2O * 2, 500.0R, "[sph+H]+ -2H2O") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + ProtonMass - CH6O2, 150.0R, "[sph+H]+ -CH6O2") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + ProtonMass, 150.0R, "[sph+H]+ ") With {
.SpectrumComment = SpectrumComment.acylchain
}})
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetAcylSpectrum(lipid As ILipid, acyl As AcylChain, adduct As AdductIon) As SpectrumPeak()
        Dim chainMass = acyl.Mass + HydrogenMass
        Dim spectrum = New List(Of SpectrumPeak)()
        If Equals(adduct.AdductIonName, "[M+Na]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(chainMass) + C2H3NO + HydrogenMass, 150.0R, "[FAA+C2H2O+H]+") With {
.SpectrumComment = SpectrumComment.acylchain
}})
        Else
            spectrum.AddRange({New SpectrumPeak(chainMass + ProtonMass + NH, 200.0R, "[FAA+H]+") With {
.SpectrumComment = SpectrumComment.acylchain
}})
            If Equals(adduct.AdductIonName, "[M+H]+") Then
                spectrum.AddRange({New SpectrumPeak(chainMass + ProtonMass + C2H3NO + H2O + 12, 150.0R, "[FAA+C3H4O2+H]+") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + ProtonMass + C3H4NO, 150.0R, "[FAA+C3H3O+H]+") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + ProtonMass + C2H3NO, 150.0R, "[FAA+C2H2O+H]+") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + ProtonMass + C2H3NO - HydrogenMass - OxygenMass, 200.0R, "[FAA+C2H+H]+") With {
.SpectrumComment = SpectrumComment.acylchain
}})
            End If
        End If
        Return spectrum.ToArray()
    End Function

    Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()

End Class
