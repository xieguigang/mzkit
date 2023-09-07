Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemistry.MetaLib
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

Public Class MGSpectrumGenerator
        Implements ILipidSpectrumGenerator

        Private Shared ReadOnly CH2 As Double = {HydrogenMass * 2, CarbonMass}.Sum()

        Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()

        Private Shared ReadOnly CH4O As Double = {CarbonMass, HydrogenMass * 4, OxygenMass}.Sum()

        Public Sub New()
            spectrumGenerator = New SpectrumPeakGenerator()
        End Sub

        Public Sub New(spectrumGenerator As ISpectrumPeakGenerator)
            Me.spectrumGenerator = If(spectrumGenerator, CSharpImpl.__Throw(Of ISpectrumPeakGenerator)(New ArgumentNullException(NameOf(spectrumGenerator))))
        End Sub

        Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator

        Public Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
            If lipid.LipidClass = LbmClass.MG Then
                If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+NH4]+") OrElse Equals(adduct.AdductIonName, "[M+Na]+") Then
                    Return True
                End If
            End If
            Return False
        End Function

        Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
            Dim spectrum = New List(Of SpectrumPeak)()
            Dim nlMass = If(Equals(adduct.AdductIonName, "[M+Na]+"), 0.0, adduct.AdductIonAccurateMass - ProtonMass)
            spectrum.AddRange(GetMGSpectrum(lipid, adduct))
            Dim plChains As PositionLevelChains = Nothing
            If lipid.Description.Has(LipidDescription.Chain) Then
                spectrum.AddRange(GetAcylLevelSpectrum(lipid, lipid.Chains.GetDeterminedChains(), adduct))

                If CSharpImpl.__Assign(plChains, TryCast(lipid.Chains, PositionLevelChains)) IsNot Nothing Then
                    'spectrum.AddRange(GetAcylPositionSpectrum(lipid, plChains.Chains[0], adduct));
                End If
                ' can't find spectrum rule TBC
                'spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, lipid.Chains.GetTypedChains<AcylChain>(), adduct, nlMass));
                If Equals(adduct.AdductIonName, "[M+Na]+") Then
                    spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, lipid.Chains.GetTypedChains(Of AcylChain)(), adduct, nlMass))
                End If
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

        Private Function GetMGSpectrum(lipid As ILipid, adduct As AdductIon) As SpectrumPeak()
            Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999R, "Precursor") With {
        .SpectrumComment = SpectrumComment.precursor
    }
}
            If Equals(adduct.AdductIonName, "[M+NH4]+") Then
                spectrum.AddRange({New SpectrumPeak(lipid.Mass + ProtonMass, 750R, "[M+H]+") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(lipid.Mass + ProtonMass - H2O, 250R, "[M+H]+ -H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
            ElseIf Equals(adduct.AdductIonName, "[M+H]+") Then
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - H2O, 150R, "Precursor-H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
            ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - CH4O, 300R, $"CH4O Loss") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
            End If

            Return spectrum.ToArray()
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
                'spectrum.AddRange
                '(
                '     new[]
                '     {
                '    //new SpectrumPeak(lipidMass - chainMass - MassDiffDictionary.HydrogenMass * 2, 50d, $"-{acylChain}"){ SpectrumComment = SpectrumComment.acylchain },
                '    //new SpectrumPeak(lipidMass - chainMass - MassDiffDictionary.HydrogenMass - MassDiffDictionary.OxygenMass, 200d, $"-{acylChain}-O"){ SpectrumComment = SpectrumComment.acylchain },
                '    //new SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - CH4O, 300d, $"CH4O Loss") { SpectrumComment = SpectrumComment.acylchain },
                '     }
                ');
                If Equals(adduct.AdductIonName, "[M+Na]+") Then
                Else
                    'new SpectrumPeak(lipidMass - chainMass , 50d, $"-{acylChain}"){ SpectrumComment = SpectrumComment.acylchain },
                    'new SpectrumPeak(lipidMass - chainMass - H2O, 200d, $"-{acylChain}-O"){ SpectrumComment = SpectrumComment.acylchain },
                    spectrum.AddRange({New SpectrumPeak(chainMass + ProtonMass, 300R, $"{acylChain} acyl+") With {
                    .SpectrumComment = SpectrumComment.acylchain
                                         }})
                End If
            End If
            Return spectrum.ToArray()
        End Function

        Private Function GetAcylPositionSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
            Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
            Dim lipidMass = lipid.Mass + adductmass
            Dim chainMass = acylChain.Mass - HydrogenMass
            Return {New SpectrumPeak(lipidMass - chainMass - H2O - CH2, 100R, "-CH2(Sn1)")}
        End Function

        Private Function GetAcylDoubleBondSpectrum(lipid As ILipid, acylChains As IEnumerable(Of AcylChain), adduct As AdductIon, Optional nlMass As Double = 0.0) As IEnumerable(Of SpectrumPeak)
            Return acylChains.SelectMany(Function(acylChain) spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acylChain, adduct, nlMass, 25R))
        End Function

        Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()

End Class

