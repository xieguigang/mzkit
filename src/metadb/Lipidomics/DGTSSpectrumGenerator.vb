Imports CompMs.Common.Components
Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.Enum
Imports CompMs.Common.FormulaGenerator.DataObj
Imports CompMs.Common.Interfaces
Imports System
Imports System.Collections.Generic
Imports System.Linq

Namespace CompMs.Common.Lipidomics
    Public Class DGTSSpectrumGenerator
        Implements ILipidSpectrumGenerator
        Private Shared ReadOnly C7H13NO2 As Double = {CarbonMass * 7, HydrogenMass * 13, NitrogenMass, OxygenMass * 2}.Sum()

        Private Shared ReadOnly C8H16NO3 As Double = {CarbonMass * 8, HydrogenMass * 16, NitrogenMass, OxygenMass * 3}.Sum() '175[M+H]+

        Private Shared ReadOnly CHO2 As Double = {CarbonMass * 1, HydrogenMass * 1, OxygenMass * 2}.Sum()

        Private Shared ReadOnly C3H9N As Double = {CarbonMass * 3, HydrogenMass * 10, NitrogenMass}.Sum()

        Private Shared ReadOnly Gly_C As Double = {CarbonMass * 10, HydrogenMass * 19, NitrogenMass, OxygenMass * 3}.Sum()

        Private Shared ReadOnly Gly_O As Double = {CarbonMass * 9, HydrogenMass * 17, NitrogenMass, OxygenMass * 4}.Sum()

        Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()

        Private Shared ReadOnly CH2 As Double = {HydrogenMass * 2, CarbonMass}.Sum()

        Public Sub New()
            spectrumGenerator = New SpectrumPeakGenerator()
        End Sub

        Public Sub New(ByVal spectrumGenerator As ISpectrumPeakGenerator)
            Me.spectrumGenerator = If(spectrumGenerator, CSharpImpl.__Throw(Of ISpectrumPeakGenerator)(New ArgumentNullException(NameOf(spectrumGenerator))))
        End Sub

        Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator

        Public Function CanGenerate(ByVal lipid As ILipid, ByVal adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
            If lipid.LipidClass = LbmClass.DGTS Then
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    Return True
                End If
            End If
            Return False
        End Function

        Public Function Generate(ByVal lipid As Lipid, ByVal adduct As AdductIon, ByVal Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
            Dim spectrum = New List(Of SpectrumPeak)()
            spectrum.AddRange(GetDGTSSpectrum(lipid, adduct))
            If lipid.Description.Has(LipidDescription.Chain) Then
                spectrum.AddRange(GetAcylLevelSpectrum(lipid, lipid.Chains.GetDeterminedChains(), adduct))
                lipid.Chains.ApplyToChain(1, Sub(chain) spectrum.AddRange(GetAcylPositionSpectrum(lipid, chain, adduct)))
                spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, lipid.Chains.GetTypedChains(Of AcylChain)().Where(Function(c) c.DoubleBond.UnDecidedCount = 0 AndAlso c.Oxidized.UnDecidedCount = 0), adduct))
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

        Private Function GetDGTSSpectrum(ByVal lipid As ILipid, ByVal adduct As AdductIon) As SpectrumPeak()
            Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999R, "Precursor") With {
        .SpectrumComment = SpectrumComment.precursor
    },
    'new SpectrumPeak(adduct.ConvertToMz(C7H13NO2), 100d, "Header") { SpectrumComment = SpectrumComment.metaboliteclass},
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - CHO2), 200R, "Precursor - CO2") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(adduct.ConvertToMz(C8H16NO3), 200R, "C8H16NO3") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    },
    New SpectrumPeak(adduct.ConvertToMz(C7H13NO2 - CH2), 200R, "Header - CH2") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },  '130
    New SpectrumPeak(adduct.ConvertToMz(Gly_C), 150R, "Gly-C") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    },
    New SpectrumPeak(adduct.ConvertToMz(Gly_O), 150R, "Gly-O") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    }
}
            Return spectrum.ToArray()
        End Function

        Private Function GetAcylLevelSpectrum(ByVal lipid As ILipid, ByVal acylChains As IEnumerable(Of IChain), ByVal adduct As AdductIon) As IEnumerable(Of SpectrumPeak)
            Return acylChains.SelectMany(Function(acylChain) GetAcylLevelSpectrum(lipid, acylChain, adduct))
        End Function

        Private Function GetAcylLevelSpectrum(ByVal lipid As ILipid, ByVal acylChain As IChain, ByVal adduct As AdductIon) As SpectrumPeak()
            Dim lipidMass = lipid.Mass
            Dim chainMass = acylChain.Mass - HydrogenMass
            Return {New SpectrumPeak(adduct.ConvertToMz(lipidMass - chainMass), 100R, $"-{acylChain}") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(lipidMass - chainMass - H2O + HydrogenMass), 100R, $"-{acylChain}-O") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(lipidMass - chainMass - C3H9N), 100R, $"-{acylChain} -C3H9N") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(lipidMass - chainMass - H2O + HydrogenMass - C3H9N), 100R, $"-{acylChain}-O -C3H9N") With {
.SpectrumComment = SpectrumComment.acylchain
}}
        End Function

        Private Function GetAcylPositionSpectrum(ByVal lipid As ILipid, ByVal acylChain As IChain, ByVal adduct As AdductIon) As SpectrumPeak()
            Dim lipidMass = lipid.Mass
            Dim chainMass = acylChain.Mass
            Return {New SpectrumPeak(adduct.ConvertToMz(lipidMass - chainMass - OxygenMass - CH2), 100R, "-CH2(Sn1)") With {
.SpectrumComment = SpectrumComment.snposition
}}
        End Function

        Private Function GetAcylDoubleBondSpectrum(ByVal lipid As ILipid, ByVal acylChains As IEnumerable(Of AcylChain), ByVal adduct As AdductIon) As IEnumerable(Of SpectrumPeak)
            Return acylChains.SelectMany(Function(acylChain) spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acylChain, adduct, 0R, 30R))
        End Function

        Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal throw statements")>
            Shared Function __Throw(Of T)(ByVal e As Exception) As T
                Throw e
            End Function
        End Class
    End Class
End Namespace
