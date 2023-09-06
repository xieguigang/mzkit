Imports CompMs.Common.Components
Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.Enum
Imports CompMs.Common.FormulaGenerator.DataObj
Imports CompMs.Common.Interfaces
Imports System
Imports System.Collections.Generic
Imports System.Linq

Public Class SMEidSpectrumGenerator
    Implements ILipidSpectrumGenerator
    Private Shared ReadOnly C5H14NO4P As Double = {CarbonMass * 5, HydrogenMass * 14, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()
    Private Shared ReadOnly C3H9N As Double = {CarbonMass * 3, HydrogenMass * 9, NitrogenMass}.Sum()
    Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()
    Private Shared ReadOnly CH4O2 As Double = {CarbonMass * 1, HydrogenMass * 4, OxygenMass * 2}.Sum()
    Private Shared ReadOnly C2H3NO As Double = {CarbonMass * 2, HydrogenMass * 3, NitrogenMass * 1, OxygenMass * 1}.Sum()
    Private Shared ReadOnly C2H2N As Double = {CarbonMass * 2, HydrogenMass * 2, NitrogenMass * 1}.Sum()

    Public Sub New()
        spectrumGenerator = New SpectrumPeakGenerator()
    End Sub
    Public Sub New(spectrumGenerator As ISpectrumPeakGenerator)
        Me.spectrumGenerator = If(spectrumGenerator, CSharpImpl.__Throw(Of ISpectrumPeakGenerator)(New ArgumentNullException(NameOf(spectrumGenerator))))
    End Sub

    Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator

    Public Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
        If lipid.Chains.OxidizedCount = 3 Then
            Return False
        End If
        If lipid.LipidClass = LbmClass.SM Then
            If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+Na]+") Then
                Return True
            End If
        End If
        Return False
    End Function

    Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
        Dim spectrum = New List(Of SpectrumPeak)()
        Dim nlmass = 0.0
        spectrum.AddRange(GetSMSpectrum(lipid, adduct))
        Dim plChains As PositionLevelChains = Nothing, sphingo As SphingoChain = Nothing, acyl As AcylChain = Nothing

        If CSharpImpl.__Assign(plChains, TryCast(lipid.Chains, PositionLevelChains)) IsNot Nothing Then
            If CSharpImpl.__Assign(sphingo, TryCast(lipid.Chains.GetChainByPosition(1), SphingoChain)) IsNot Nothing Then
                spectrum.AddRange(GetSphingoSpectrum(lipid, sphingo, adduct))
                spectrum.AddRange(spectrumGenerator.GetSphingoDoubleBondSpectrum(lipid, sphingo, adduct, nlmass, 30.0R))
            End If

            If CSharpImpl.__Assign(acyl, TryCast(lipid.Chains.GetChainByPosition(2), AcylChain)) IsNot Nothing Then
                spectrum.AddRange(GetAcylSpectrum(lipid, acyl, adduct))
                spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acyl, adduct, nlmass, 30.0R))
                spectrum.AddRange(EidSpecificSpectrum(lipid, adduct, nlmass, 200.0R))
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

    Private Function GetSMSpectrum(lipid As ILipid, adduct As AdductIon) As SpectrumPeak()
        Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
        .SpectrumComment = SpectrumComment.precursor
    },
    New SpectrumPeak(adduct.ConvertToMz(C5H14NO4P), 500.0R, "C5H14NO4P (Header)") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    },
    New SpectrumPeak(adduct.ConvertToMz(C5H14NO4P + C2H3NO - OxygenMass), 150.0R, "C7H18N2O4P (Header+C2H3N)") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    }
}
        If Equals(adduct.AdductIonName, "[M+Na]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - C3H9N, 150.0R, "Precursor-C3H9N") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - C5H14NO4P, 150.0R, "Precursor-Header") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
        ElseIf Equals(adduct.AdductIonName, "[M+H]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(C5H14NO4P + C2H3NO + CarbonMass), 150.0R, "C7H18N2O4P (Header+C3H3NO)") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}}) ' need to consider
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetSphingoSpectrum(lipid As ILipid, sphingo As SphingoChain, adduct As AdductIon) As SpectrumPeak()
        Dim chainMass = sphingo.Mass + HydrogenMass
        Dim spectrum = New List(Of SpectrumPeak)()
        If Equals(adduct.AdductIonName, "[M+H]+") Then
            spectrum.AddRange({New SpectrumPeak(chainMass + ProtonMass - H2O * 2, 100.0R, "[sph+H]+ -Header -H2O") With {
.SpectrumComment = SpectrumComment.acylchain
'new SpectrumPeak(chainMass + MassDiffDictionary.ProtonMass - CH4O2, 100d, "[sph+H]+ -CH4O2"),
}})
            End If
        Return spectrum.ToArray()
    End Function

    Private Function GetAcylSpectrum(lipid As ILipid, acyl As AcylChain, adduct As AdductIon) As SpectrumPeak()
        Dim chainMass = acyl.Mass + HydrogenMass
        Dim spectrum = New List(Of SpectrumPeak)() From {
    New SpectrumPeak(adduct.ConvertToMz(chainMass) + C2H2N, 200.0R, "[FAA+C2H+adduct]+") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(adduct.ConvertToMz(chainMass) + C5H14NO4P + C2H2N - HydrogenMass, 200.0R, "[FAA+C2H+Header+adduct]+") With {
        .SpectrumComment = SpectrumComment.acylchain
    }
}
        Return spectrum.ToArray()
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
