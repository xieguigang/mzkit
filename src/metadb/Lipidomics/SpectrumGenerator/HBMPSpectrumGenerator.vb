Imports CompMs.Common.Components
Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.Enum
Imports CompMs.Common.FormulaGenerator.DataObj
Imports CompMs.Common.Interfaces
Imports System
Imports System.Collections.Generic
Imports System.Linq

    Public Class HBMPSpectrumGenerator
        Implements ILipidSpectrumGenerator
        'HBMP explain rule -> HBMP 1 chain(sn1)/2 chain(sn2,sn3)
        'HBMP sn1_sn2_sn3 (follow the rules of alignment) -- MolecularSpeciesLevelChains
        'HBMP sn1/sn2_sn3 -- MolecularSpeciesLevelChains <- cannot generate now
        'HBMP sn1/sn2/sn3 (sn4= 0:0)  -- MolecularSpeciesLevelChains
        'HBMP sn1/sn4(or sn4/sn1)/sn2/sn3 (sn4= 0:0)  -- PositionLevelChains <- cannot generate now

        Private Shared ReadOnly C3H9O6P As Double = {CarbonMass * 3, HydrogenMass * 9, OxygenMass * 6, PhosphorusMass}.Sum()

        Private Shared ReadOnly C3H6O2 As Double = {CarbonMass * 3, HydrogenMass * 6, OxygenMass * 2}.Sum()

        Private Shared ReadOnly CH2 As Double = {HydrogenMass * 2, CarbonMass}.Sum()

        Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()

        Public Sub New()
            spectrumGenerator = New SpectrumPeakGenerator()
        End Sub

        Public Sub New(ByVal spectrumGenerator As ISpectrumPeakGenerator)
            Me.spectrumGenerator = If(spectrumGenerator, CSharpImpl.__Throw(Of ISpectrumPeakGenerator)(New ArgumentNullException(NameOf(spectrumGenerator))))
        End Sub

        Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator

        Public Function CanGenerate(ByVal lipid As ILipid, ByVal adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
            If lipid.LipidClass = LbmClass.HBMP Then
                If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+NH4]+") Then
                    Return True
                End If
            End If
            Return False
        End Function
        Public Function Generate(ByVal lipid As Lipid, ByVal adduct As AdductIon, ByVal Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
            'var nlMass = adduct.AdductIonName == "[M+NH4]+" ? adduct.AdductIonAccurateMass + H2O : H2O;
            Dim spectrum = New List(Of SpectrumPeak)()
            spectrum.AddRange(GetHBMPSpectrum(lipid, adduct))
            ' GetChain(1) = lyso
            If lipid.Description.Has(LipidDescription.Chain) Then
                lipid.Chains.ApplyToChain(1, Sub(chain) spectrum.AddRange(GetLysoAcylLevelSpectrum(lipid, chain, adduct)))
                lipid.Chains.ApplyToChain(2, Sub(chain) spectrum.AddRange(GetAcylLevelSpectrum(lipid, chain, adduct)))
                lipid.Chains.ApplyToChain(3, Sub(chain) spectrum.AddRange(GetAcylLevelSpectrum(lipid, chain, adduct)))
                'lipid.Chains.ApplyToChain(1, chain => spectrum.AddRange(GetAcylPositionSpectrum(lipid, chain, adduct)));
                'lipid.Chains.ApplyToChain(2, chain => spectrum.AddRange(GetAcylPositionSpectrum(lipid, chain, adduct)));
                spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, lipid.Chains.GetTypedChains(Of AcylChain)(), adduct, nlMass:=0.0))
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
        Private Function GetHBMPSpectrum(ByVal lipid As ILipid, ByVal adduct As AdductIon) As SpectrumPeak()
            Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
            Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999R, "Precursor") With {
        .SpectrumComment = SpectrumComment.precursor
    },
    New SpectrumPeak(lipid.Mass + adductmass - H2O, 100R, "Precursor -H2O") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    }
'new SpectrumPeak(C3H9O6P + adductmass, 50d, "C3H9O6P") { SpectrumComment = SpectrumComment.metaboliteclass },
'new SpectrumPeak(C3H9O6P -H2O + adductmass, 50d, "C3H9O6P - H2O") { SpectrumComment = SpectrumComment.metaboliteclass },
}
            If Equals(adduct.AdductIonName, "[M+NH4]+") Then
                spectrum.Add(New SpectrumPeak(lipid.Mass + ProtonMass, 100R, "[M+H]+") With {
.SpectrumComment = SpectrumComment.metaboliteclass
})
            End If
            Return spectrum.ToArray()
        End Function

        Private Function GetAcylDoubleBondSpectrum(ByVal lipid As ILipid, ByVal acylChains As IEnumerable(Of AcylChain), ByVal adduct As AdductIon, ByVal Optional nlMass As Double = 0.0) As IEnumerable(Of SpectrumPeak)
            Dim spectrum = New List(Of SpectrumPeak)()
            Dim chains = acylChains.ToList()
            nlMass = chains(0).Mass + C3H9O6P - HydrogenMass + adduct.AdductIonAccurateMass - ProtonMass
            spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, chains(1), adduct, nlMass, 10R))
            spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, chains(2), adduct, nlMass, 10R))
            nlMass = chains(1).Mass + chains(2).Mass + C3H6O2 - HydrogenMass + adduct.AdductIonAccurateMass - ProtonMass
            spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, chains(0), adduct, nlMass, 10R))
            Return spectrum.ToArray()
        End Function

        'private IEnumerable<SpectrumPeak> GetAcylLevelSpectrum(ILipid lipid, IEnumerable<IChain> acylChains, AdductIon adduct)
        '{
        '    return acylChains.SelectMany(acylChain => GetAcylLevelSpectrum(lipid, acylChain, adduct));
        '}

        Private Function GetAcylLevelSpectrum(ByVal lipid As ILipid, ByVal acylChain As IChain, ByVal adduct As AdductIon) As SpectrumPeak()
            Dim lipidMass = lipid.Mass
            Dim chainMass = acylChain.Mass - HydrogenMass
            Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)

            Dim spectrum = New List(Of SpectrumPeak) From {
    'new SpectrumPeak(chainMass + C3H6O2 + adductmass, 100d, $"{acylChain}+C3H4O2+H"),
    New SpectrumPeak(chainMass + ProtonMass, 50R, $"{acylChain} Acyl+") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(lipidMass - chainMass - HydrogenMass + adductmass, 50R, $"-{acylChain}") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(lipidMass - chainMass - H2O + adductmass, 50R, $"-{acylChain}-H2O") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(lipidMass - chainMass - C3H9O6P + adductmass, 200R, $"-C3H9O6P -{acylChain}") With {
        .SpectrumComment = SpectrumComment.acylchain
    }
 'new SpectrumPeak(lipidMass - chainMass - C3H9O6P - H2O + adductmass, 50d, $"-C3H9O6P -{acylChain}-H2O") { SpectrumComment = SpectrumComment.acylchain },
 }

            Return spectrum.ToArray()
        End Function

        Private Function GetLysoAcylLevelSpectrum(ByVal lipid As ILipid, ByVal acylChain As IChain, ByVal adduct As AdductIon) As SpectrumPeak()
            Dim lipidMass = lipid.Mass
            Dim chainMass = acylChain.Mass - HydrogenMass
            Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)

            Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(chainMass + C3H6O2 + adductmass, 100R, $"{acylChain}+C3H4O2+H") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    'new SpectrumPeak(chainMass + C3H6O2 - H2O + adductmass, 100d, $"{acylChain} + C3H4O2 -H2O +H"){ SpectrumComment = SpectrumComment.acylchain },
    New SpectrumPeak(chainMass + ProtonMass, 50R, $"{acylChain} Acyl+") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(lipidMass - chainMass - HydrogenMass + adductmass, 50R, $"-{acylChain}") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(lipidMass - chainMass - H2O + adductmass, 50R, $"-{acylChain}-H2O") With {
        .SpectrumComment = SpectrumComment.acylchain
    }
 'new SpectrumPeak(lipidMass - chainMass - C3H9O6P + adductmass, 200d, $"-C3H9O6P -{acylChain}") { SpectrumComment = SpectrumComment.acylchain },
 'new SpectrumPeak(lipidMass - chainMass - C3H9O6P - H2O + adductmass, 50d, $"-C3H9O6P -{acylChain}-H2O") { SpectrumComment = SpectrumComment.acylchain },
 }

            Return spectrum.ToArray()
        End Function


        Private Function GetAcylPositionSpectrum(ByVal lipid As ILipid, ByVal acylChain As IChain, ByVal adduct As AdductIon) As SpectrumPeak()
            Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
            Dim lipidMass = lipid.Mass + adductmass
            Dim chainMass = acylChain.Mass - HydrogenMass

            Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(lipidMass - chainMass - H2O - CH2, 50R, "-CH2(Sn1)") With {
        .SpectrumComment = SpectrumComment.snposition
    }
}
            Return spectrum.ToArray()
        End Function


        Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()

    End Class

