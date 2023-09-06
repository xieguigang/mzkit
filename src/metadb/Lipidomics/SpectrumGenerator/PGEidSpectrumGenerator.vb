Imports CompMs.Common.Components
Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.Enum
Imports CompMs.Common.FormulaGenerator.DataObj
Imports CompMs.Common.Interfaces
Imports System
Imports System.Collections.Generic
Imports System.Linq


    Public Class PGEidSpectrumGenerator
        Implements ILipidSpectrumGenerator

        Private Shared ReadOnly C3H9O6P As Double = {CarbonMass * 3, HydrogenMass * 9, OxygenMass * 6, PhosphorusMass}.Sum()

        Private Shared ReadOnly C3H6O2 As Double = {CarbonMass * 3, HydrogenMass * 6, OxygenMass * 2}.Sum()

        Private Shared ReadOnly Gly_C As Double = {CarbonMass * 6, HydrogenMass * 13, OxygenMass * 6, PhosphorusMass}.Sum()

        Private Shared ReadOnly Gly_O As Double = {CarbonMass * 5, HydrogenMass * 11, OxygenMass * 7, PhosphorusMass}.Sum()

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
            If lipid.LipidClass = LbmClass.PG Then
                If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+NH4]+") Then
                    Return True
                End If
            End If
            Return False
        End Function

        Public Function Generate(ByVal lipid As Lipid, ByVal adduct As AdductIon, ByVal Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
            Dim nlMass = If(Equals(adduct.AdductIonName, "[M+Na]+"), C3H9O6P, adduct.ConvertToMz(C3H9O6P) - ProtonMass)
            Dim spectrum = New List(Of SpectrumPeak)()
            spectrum.AddRange(GetPGSpectrum(lipid, adduct))
            If lipid.Description.Has(LipidDescription.Chain) Then
                spectrum.AddRange(GetAcylLevelSpectrum(lipid, lipid.Chains.GetDeterminedChains(), adduct))
                lipid.Chains.ApplyToChain(1, Sub(chain) spectrum.AddRange(GetAcylPositionSpectrum(lipid, chain, adduct)))
                spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, lipid.Chains.GetTypedChains(Of AcylChain)(), adduct, nlMass:=nlMass))
                spectrum.AddRange(EidSpecificSpectrum(lipid, adduct, nlMass:=nlMass, 200R))
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

        Private Function GetPGSpectrum(ByVal lipid As ILipid, ByVal adduct As AdductIon) As SpectrumPeak()
            Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
            Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999R, "Precursor") With {
        .SpectrumComment = SpectrumComment.precursor
    },
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - H2O), 100R, "Precursor -H2O") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(lipid.Mass - C3H9O6P + adductmass, 999R, "Precursor -C3H9O6P") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    },
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - C3H6O2), 100R, "Precursor -C3H6O2") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(C3H9O6P + adductmass, 100R, "Header") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak((lipid.Mass - C3H9O6P + ProtonMass) / 2, 150R, "[Precursor -C3H9O6P]2+") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(Gly_C + adductmass, 100R, "Gly-C") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(Gly_O + adductmass, 100R, "Gly-O") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    }
}
            If Equals(adduct.AdductIonName, "[M+NH4]+") Then
                spectrum.Add(New SpectrumPeak(lipid.Mass + ProtonMass, 200R, "[M+H]+"))
            End If

            Return spectrum.ToArray()
        End Function

        Private Function GetAcylDoubleBondSpectrum(ByVal lipid As ILipid, ByVal acylChains As IEnumerable(Of AcylChain), ByVal adduct As AdductIon, ByVal Optional nlMass As Double = 0.0) As IEnumerable(Of SpectrumPeak)
            Return acylChains.SelectMany(Function(acylChain) spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acylChain, adduct, nlMass, 20R))
        End Function

        Private Function GetAcylLevelSpectrum(ByVal lipid As ILipid, ByVal acylChains As IEnumerable(Of IChain), ByVal adduct As AdductIon) As IEnumerable(Of SpectrumPeak)
            Return acylChains.SelectMany(Function(acylChain) GetAcylLevelSpectrum(lipid, acylChain, adduct))
        End Function

        Private Function GetAcylLevelSpectrum(ByVal lipid As ILipid, ByVal acylChain As IChain, ByVal adduct As AdductIon) As SpectrumPeak()
            Dim lipidMass = lipid.Mass
            Dim chainMass = acylChain.Mass - HydrogenMass
            Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
            Return {New SpectrumPeak(chainMass + ProtonMass, 100R, $"{acylChain} acyl") With {
.SpectrumComment = SpectrumComment.acylchain
'new SpectrumPeak(lipidMass - chainMass + adductmass, 100d, $"-{acylChain}") { SpectrumComment = SpectrumComment.acylchain },
}, New SpectrumPeak(lipidMass - chainMass - H2O + adductmass, 100R, $"-{acylChain}-H2O") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass - C3H9O6P + adductmass, 400R, $"-Header -{acylChain}") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass - C3H9O6P - H2O + adductmass, 100R, $"-Header -{acylChain}-H2O") With {
.SpectrumComment = SpectrumComment.acylchain
}}
        End Function

        Private Function GetAcylPositionSpectrum(ByVal lipid As ILipid, ByVal acylChain As IChain, ByVal adduct As AdductIon) As SpectrumPeak()
            Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
            Dim lipidMass = lipid.Mass + adductmass
            Dim chainMass = acylChain.Mass - HydrogenMass
            Return {New SpectrumPeak(lipidMass - chainMass - H2O - CH2, 100R, "-CH2(Sn1)") With {
.SpectrumComment = SpectrumComment.snposition
}, New SpectrumPeak(lipidMass - chainMass - H2O * 2 - CH2, 100R, "-H2O -CH2(Sn1)") With {
.SpectrumComment = SpectrumComment.snposition
'new SpectrumPeak(lipidMass - chainMass - C3H9O6P - H2O - CH2 , 100d, "-Header -CH2(Sn1)") { SpectrumComment = SpectrumComment.snposition },
}}
        End Function

        Private Shared Function EidSpecificSpectrum(ByVal lipid As Lipid, ByVal adduct As AdductIon, ByVal nlMass As Double, ByVal intensity As Double) As SpectrumPeak()
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

