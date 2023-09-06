Imports CompMs.Common.Components
Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.Enum
Imports CompMs.Common.FormulaGenerator.DataObj
Imports CompMs.Common.Interfaces
Imports System
Imports System.Collections.Generic
Imports System.Linq

Namespace CompMs.Common.Lipidomics
    Public Class Hex2CerSpectrumGenerator
        Implements ILipidSpectrumGenerator
        Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()
        Private Shared ReadOnly C2H5NO As Double = {CarbonMass * 2, HydrogenMass * 5, NitrogenMass * 1, OxygenMass * 1}.Sum()
        Private Shared ReadOnly CH4O2 As Double = {CarbonMass * 1, HydrogenMass * 4, OxygenMass * 2}.Sum()
        Private Shared ReadOnly C2H3NO As Double = {CarbonMass * 2, HydrogenMass * 3, NitrogenMass * 1, OxygenMass * 1}.Sum()
        Private Shared ReadOnly NH As Double = {NitrogenMass * 1, HydrogenMass * 1}.Sum()
        Private Shared ReadOnly C6H10O5 As Double = {CarbonMass * 6, HydrogenMass * 10, OxygenMass * 5}.Sum()
        Private Shared ReadOnly C5H10O4 As Double = {CarbonMass * 5, HydrogenMass * 10, OxygenMass * 4}.Sum()
        Public Sub New()
            spectrumGenerator = New SpectrumPeakGenerator()
        End Sub
        Public Sub New(ByVal spectrumGenerator As ISpectrumPeakGenerator)
            Me.spectrumGenerator = If(spectrumGenerator, CSharpImpl.__Throw(Of ISpectrumPeakGenerator)(New ArgumentNullException(NameOf(spectrumGenerator))))
        End Sub

        Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator

        Public Function CanGenerate(ByVal lipid As ILipid, ByVal adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
            If lipid.LipidClass = LbmClass.Hex2Cer Then
                If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+Na]+") Then
                    Return True
                End If
            End If
            Return False
        End Function

        Public Function Generate(ByVal lipid As Lipid, ByVal adduct As AdductIon, ByVal Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
            Dim spectrum = New List(Of SpectrumPeak)()
            Dim nlmass = If(Equals(adduct.AdductIonName, "[M+H]+"), H2O * 2 + C6H10O5 * 2, H2O + C6H10O5 * 2)
            spectrum.AddRange(GetHex2CerSpectrum(lipid, adduct))
            Dim plChains As PositionLevelChains = Nothing, sphingo As SphingoChain = Nothing, acyl As AcylChain = Nothing

            If CSharpImpl.__Assign(plChains, TryCast(lipid.Chains, PositionLevelChains)) IsNot Nothing Then
                If CSharpImpl.__Assign(sphingo, TryCast(lipid.Chains.GetChainByPosition(1), SphingoChain)) IsNot Nothing Then
                    spectrum.AddRange(GetSphingoSpectrum(lipid, sphingo, adduct))
                    'spectrum.AddRange(spectrumGenerator.GetSphingoDoubleBondSpectrum(lipid, sphingo, adduct, nlmass, 30d));
                End If

                If CSharpImpl.__Assign(acyl, TryCast(lipid.Chains.GetChainByPosition(2), AcylChain)) IsNot Nothing Then
                    spectrum.AddRange(GetAcylSpectrum(lipid, acyl, adduct))
                    'spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acyl, adduct, nlmass, 30d));
                End If
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

        Private Function GetHex2CerSpectrum(ByVal lipid As ILipid, ByVal adduct As AdductIon) As SpectrumPeak()
            Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999R, "Precursor") With {
        .SpectrumComment = SpectrumComment.precursor
    },
            New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - H2O, 200R, "Precursor-H2O") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
            'new SpectrumPeak(adduct.ConvertToMz(lipid.Mass)-C6H10O5, 300d, "Precursor-Hex") { SpectrumComment = SpectrumComment.metaboliteclass },
            'new SpectrumPeak(adduct.ConvertToMz(lipid.Mass)-C6H10O5-H2O, 200d, "Precursor-Hex-H2O") { SpectrumComment = SpectrumComment.metaboliteclass },
            'new SpectrumPeak(adduct.ConvertToMz(lipid.Mass)-C6H10O5-H2O*2, 200d, "Precursor-Hex-2H2O") { SpectrumComment = SpectrumComment.metaboliteclass },
            New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - C6H10O5 * 2, 100R, "Precursor-2Hex") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
            New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - C6H10O5 * 2 - H2O, 400R, "Precursor-2Hex-H2O") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
            New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - C6H10O5 * 2 - H2O * 2, 100R, "Precursor-2Hex-2H2O") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    }
}
            Return spectrum.ToArray()
        End Function

        Private Function GetSphingoSpectrum(ByVal lipid As ILipid, ByVal sphingo As SphingoChain, ByVal adduct As AdductIon) As SpectrumPeak()
            Dim chainMass = sphingo.Mass + HydrogenMass
            Dim spectrum = New List(Of SpectrumPeak)()
            'if (adduct.AdductIonName != "[M+Na]+")
            If True Then
                spectrum.AddRange({New SpectrumPeak(chainMass + ProtonMass - H2O, 200R, "[sph+H]+ -H2O") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + ProtonMass - H2O * 2, 500R, "[sph+H]+ -2H2O") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + ProtonMass - CH4O2, 150R, "[sph+H]+ -CH4O2") With {
.SpectrumComment = SpectrumComment.acylchain
}})
            End If
            Return spectrum.ToArray()
        End Function

        Private Function GetAcylSpectrum(ByVal lipid As ILipid, ByVal acyl As AcylChain, ByVal adduct As AdductIon) As SpectrumPeak()
            Dim chainMass = acyl.Mass + HydrogenMass
            Dim spectrum = New List(Of SpectrumPeak)() From {
                 New SpectrumPeak(chainMass + ProtonMass + C2H3NO - HydrogenMass - OxygenMass, 200R, "[FAA+C2H+H]+") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
                 New SpectrumPeak(adduct.ConvertToMz(chainMass) + C2H3NO + HydrogenMass + C6H10O5 * 2, 150R, "[FAA+C2H4O+2Hex+adduct]+") With {
        .SpectrumComment = SpectrumComment.acylchain
    }

}
            If Equals(adduct.AdductIonName, "[M+H]+") Then
                spectrum.AddRange({New SpectrumPeak(chainMass + ProtonMass + NH, 200R, "[FAA+H]+") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + ProtonMass + C2H3NO, 150R, "[FAA+C2H2O+H]+") With {
.SpectrumComment = SpectrumComment.acylchain
}})
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
