Imports CompMs.Common.Components
Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.Enum
Imports CompMs.Common.FormulaGenerator.DataObj
Imports CompMs.Common.Interfaces
Imports System
Imports System.Collections.Generic
Imports System.Linq

Namespace CompMs.Common.Lipidomics
    Public Class LPEEidSpectrumGenerator
        Implements ILipidSpectrumGenerator

        Private Shared ReadOnly C2H8NO4P As Double = {CarbonMass * 2, HydrogenMass * 8, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()

        Private Shared ReadOnly C2H5N As Double = {CarbonMass * 2, HydrogenMass * 5, NitrogenMass}.Sum()

        Private Shared ReadOnly C3H9O6P As Double = {CarbonMass * 3, HydrogenMass * 9, OxygenMass * 6, PhosphorusMass}.Sum() ' 172.013675 OCC(O)COP(O)(O)=O

        Private Shared ReadOnly Gly_C As Double = {CarbonMass * 5, HydrogenMass * 12, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()

        Private Shared ReadOnly Gly_O As Double = {CarbonMass * 4, HydrogenMass * 10, NitrogenMass, OxygenMass * 5, PhosphorusMass}.Sum()

        Private Shared ReadOnly CH2 As Double = {HydrogenMass * 2, CarbonMass}.Sum()

        Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()

        Public Sub New()
            spectrumGenerator = New SpectrumPeakGenerator()
        End Sub

        Public Sub New(ByVal peakGenerator As ISpectrumPeakGenerator)
            spectrumGenerator = If(peakGenerator, CSharpImpl.__Throw(Of ISpectrumPeakGenerator)(New ArgumentNullException(NameOf(peakGenerator))))
        End Sub

        Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator

        Public Function CanGenerate(ByVal lipid As ILipid, ByVal adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
            If lipid.LipidClass = LbmClass.LPE Then
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    Return True
                End If
            End If
            Return False
        End Function

        Public Function Generate(ByVal lipid As Lipid, ByVal adduct As AdductIon, ByVal Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
            Dim spectrum = New List(Of SpectrumPeak)()
            spectrum.AddRange(GetLPESpectrum(lipid, adduct))
            If lipid.Description.Has(LipidDescription.Chain) Then
                spectrum.AddRange(GetAcylLevelSpectrum(lipid, lipid.Chains.GetDeterminedChains(), adduct))
                lipid.Chains.ApplyToChain(1, Sub(chain) spectrum.AddRange(GetAcylPositionSpectrum(lipid, chain, adduct)))
                spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, lipid.Chains.GetTypedChains(Of AcylChain)(), adduct))
                spectrum.AddRange(EidSpecificSpectrum(lipid, adduct, 0R, 300R))
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

        Private Function GetLPESpectrum(ByVal lipid As ILipid, ByVal adduct As AdductIon) As SpectrumPeak()
            Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999R, "Precursor") With {
        .SpectrumComment = SpectrumComment.precursor
    },
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - H2O), 200R, "Precursor - H2O") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(lipid.Mass - C2H8NO4P + ProtonMass, 250R, "Precursor -C2H8NO4P") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    },
    New SpectrumPeak(adduct.ConvertToMz(C2H8NO4P), 200R, "Header") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(adduct.ConvertToMz(Gly_C), 100R, "Gly-C") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(adduct.ConvertToMz(Gly_O), 200R, "Gly-O") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    }
}
            If Equals(adduct.AdductIonName, "[M+H]+") Then
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(C3H9O6P), 100R, "C3H9O6P") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(adduct.ConvertToMz(C3H9O6P - H2O), 100R, "C3H9O6P - H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(adduct.ConvertToMz(Gly_O - H2O), 100R, "Gly-O - H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
            End If

            Return spectrum.ToArray()
        End Function

        Private Function GetAcylDoubleBondSpectrum(ByVal lipid As ILipid, ByVal acylChains As IEnumerable(Of AcylChain), ByVal adduct As AdductIon) As IEnumerable(Of SpectrumPeak)
            Return acylChains.SelectMany(Function(acylChain) spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acylChain, adduct, 0R, 50R))
        End Function

        Private Function GetAcylLevelSpectrum(ByVal lipid As ILipid, ByVal acylChains As IEnumerable(Of IChain), ByVal adduct As AdductIon) As IEnumerable(Of SpectrumPeak)
            Return acylChains.SelectMany(Function(acylChain) GetAcylLevelSpectrum(lipid, acylChain, adduct))
        End Function

        Private Function GetAcylLevelSpectrum(ByVal lipid As ILipid, ByVal acylChain As IChain, ByVal adduct As AdductIon) As SpectrumPeak()
            Dim chainMass = acylChain.Mass - HydrogenMass
            Dim spectrum = New List(Of SpectrumPeak)()
            If chainMass <> 0.0 Then
                spectrum.AddRange({New SpectrumPeak(chainMass + ProtonMass, 100R, $"{acylChain} acyl") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - chainMass), 50R, $"-{acylChain}") With {
.SpectrumComment = SpectrumComment.acylchain
}})
            End If
            Return spectrum.ToArray()
        End Function

        Private Function GetAcylPositionSpectrum(ByVal lipid As ILipid, ByVal acylChain As IChain, ByVal adduct As AdductIon) As SpectrumPeak()
            Dim chainMass = acylChain.Mass
            Return {New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - chainMass - OxygenMass - CH2 - HydrogenMass), 100R, "-CH2(Sn1)") With {
.SpectrumComment = SpectrumComment.snposition
}}
        End Function

        Private Shared Function EidSpecificSpectrum(ByVal lipid As Lipid, ByVal adduct As AdductIon, ByVal nlMass As Double, ByVal intensity As Double) As SpectrumPeak()
            Dim spectrum = New List(Of SpectrumPeak)()
            Dim chains As SeparatedChains = Nothing

            If CSharpImpl.__Assign(chains, TryCast(lipid.Chains, SeparatedChains)) IsNot Nothing Then
                For Each chain In lipid.Chains.GetDeterminedChains()
                    If chain.DoubleBond.Count = 0 OrElse chain.DoubleBond.UnDecidedCount > 0 Then Continue For
                    spectrum.AddRange(EidSpecificSpectrumGenerator.EidSpecificSpectrumGen(lipid, chain, adduct, nlMass, intensity))
                Next
            End If
            Return spectrum.ToArray()
        End Function

        Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function <Obsolete("Please refactor calling code to use normal throw statements")>
            Shared Function __Throw(Of T)(ByVal e As Exception) As T
                Throw e
            End Function
        End Class
    End Class
End Namespace
