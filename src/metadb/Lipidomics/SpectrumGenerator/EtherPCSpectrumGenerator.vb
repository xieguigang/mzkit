Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MSEngine
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

    Public Class EtherPCSpectrumGenerator
        Implements ILipidSpectrumGenerator
        Private Shared ReadOnly C5H14NO4P As Double = {CarbonMass * 5, HydrogenMass * 14, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()

        Private Shared ReadOnly C3H9N As Double = {CarbonMass * 3, HydrogenMass * 9, NitrogenMass}.Sum()

        Private Shared ReadOnly C5H11N As Double = {CarbonMass * 5, HydrogenMass * 11, NitrogenMass}.Sum()

        Private Shared ReadOnly Gly_C As Double = {CarbonMass * 8, HydrogenMass * 18, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()

        Private Shared ReadOnly Gly_O As Double = {CarbonMass * 7, HydrogenMass * 16, NitrogenMass, OxygenMass * 5, PhosphorusMass}.Sum()

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
            If lipid.LipidClass = LbmClass.EtherPC Then
                If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+Na]+") Then
                    Return True
                End If
            End If
            Return False
        End Function

        Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
            Dim spectrum = New List(Of SpectrumPeak)()
            spectrum.AddRange(GetEtherPCSpectrum(lipid, adduct))
            lipid.Chains.ApplyToChain(1, Sub(chain) spectrum.AddRange(GetSn1PositionSpectrum(lipid, chain, adduct)))

            Dim alkyl As AlkylChain = Nothing, acyl As AcylChain = Nothing
            (alkyl, acyl) = lipid.Chains.Deconstruct(Of AlkylChain, AcylChain)()
            If alkyl IsNot Nothing AndAlso acyl IsNot Nothing Then
                If alkyl.DoubleBond.Bonds.Any(Function(b) b.Position = 1) Then
                    spectrum.AddRange(GetEtherPCPSpectrum(lipid, alkyl, acyl, adduct))
                Else
                    spectrum.AddRange(GetEtherPCOSpectrum(lipid, alkyl, acyl, adduct))
                End If
                spectrum.AddRange(spectrumGenerator.GetAlkylDoubleBondSpectrum(lipid, alkyl, adduct, 0R, 50R))
                spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acyl, adduct, 0R, 50R))
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

        Private Function GetEtherPCSpectrum(lipid As ILipid, adduct As AdductIon) As SpectrumPeak()
            Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999R, "Precursor") With {
        .SpectrumComment = SpectrumComment.precursor
    },
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) / 2, 40R, "Precursor2+") With {
        .SpectrumComment = SpectrumComment.precursor
    },
    New SpectrumPeak(adduct.ConvertToMz(Gly_C), 400R, "Gly-C") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    },
    New SpectrumPeak(adduct.ConvertToMz(Gly_O), 400R, "Gly-O") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    },
    New SpectrumPeak(adduct.ConvertToMz(C5H14NO4P), 500R, "Header") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    }
}
            If Equals(adduct.AdductIonName, "[M+Na]+") Then
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - C3H9N), 500R, "Precursor -C3H9N") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - C5H11N), 200R, "Precursor -C5H11N") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
            End If
            Return spectrum.ToArray()
        End Function

        Private Function GetEtherPCPSpectrum(lipid As ILipid, alkylChain As IChain, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
            Return {New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass + HydrogenMass), 100R, $"-{alkylChain}") With {
.SpectrumComment = SpectrumComment.acylchain
'new SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass), 100d, $"-{alkylChain} -H") { SpectrumComment = SpectrumComment.acylchain },
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass + HydrogenMass), 100R, $"-{acylChain}") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass - OxygenMass), 300, $"-{alkylChain}-O") With {
.SpectrumComment = SpectrumComment.acylchain,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass - OxygenMass - HydrogenMass), 200R, $"-{acylChain}-O") With {
.SpectrumComment = SpectrumComment.acylchain
}}
        End Function

        Private Function GetEtherPCOSpectrum(lipid As ILipid, alkylChain As IChain, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
            Return {New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass), 100R, $"-{alkylChain}") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass + HydrogenMass), 100R, $"-{acylChain}") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass - OxygenMass), 100R, $"-{alkylChain}-O") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass - H2O), 100R, $"-{acylChain}-O") With {
.SpectrumComment = SpectrumComment.acylchain
}}
        End Function

        Private Function GetSn1PositionSpectrum(lipid As ILipid, alkylChain As IChain, adduct As AdductIon) As SpectrumPeak()
            Return {New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass - OxygenMass - CH2), 150R, "-CH2(Sn1)") With {
.SpectrumComment = SpectrumComment.snposition
}}
        End Function

        Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()

    End Class

