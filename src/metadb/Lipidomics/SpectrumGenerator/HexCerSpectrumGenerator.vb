Imports CompMs.Common.Components
Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.Enum
Imports CompMs.Common.FormulaGenerator.DataObj
Imports CompMs.Common.Interfaces
Imports System
Imports System.Collections.Generic
Imports System.Linq

Public Class HexCerSpectrumGenerator
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
    Public Sub New(spectrumGenerator As ISpectrumPeakGenerator)
        Me.spectrumGenerator = If(spectrumGenerator, CSharpImpl.__Throw(Of ISpectrumPeakGenerator)(New ArgumentNullException(NameOf(spectrumGenerator))))
    End Sub

    Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator

    Public Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
        If lipid.LipidClass = LbmClass.HexCer_NS OrElse lipid.LipidClass = LbmClass.HexCer_NDS Then
            If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+Na]+") OrElse Equals(adduct.AdductIonName, "[M+H-H2O]+") Then
                Return True
            End If
        End If
        Return False
    End Function

    Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
        Dim spectrum = New List(Of SpectrumPeak)()
        Dim nlmass = If(Equals(adduct.AdductIonName, "[M+H]+"), H2O + C6H10O5, If(Equals(adduct.AdductIonName, "[M+H-H2O]+"), C6H10O5, 0.0))
        spectrum.AddRange(GetHexCerNSSpectrum(lipid, adduct))
        Dim plChains As PositionLevelChains = Nothing, sphingo As SphingoChain = Nothing, acyl As AcylChain = Nothing

        If CSharpImpl.__Assign(plChains, TryCast(lipid.Chains, PositionLevelChains)) IsNot Nothing Then
            If CSharpImpl.__Assign(sphingo, TryCast(lipid.Chains.GetChainByPosition(1), SphingoChain)) IsNot Nothing Then
                spectrum.AddRange(GetSphingoSpectrum(lipid, sphingo, adduct))
                If Equals(adduct.AdductIonName, "[M+Na]+") Then
                    spectrum.AddRange(spectrumGenerator.GetSphingoDoubleBondSpectrum(lipid, sphingo, adduct, nlmass, 30.0R)) '-header
                End If
            End If

            If CSharpImpl.__Assign(acyl, TryCast(lipid.Chains.GetChainByPosition(2), AcylChain)) IsNot Nothing Then
                spectrum.AddRange(GetAcylSpectrum(lipid, acyl, adduct))
                If Equals(adduct.AdductIonName, "[M+Na]+") Then
                    spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acyl, adduct, nlmass, 30.0R)) '-header
                End If
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

    Private Function GetHexCerNSSpectrum(lipid As ILipid, adduct As AdductIon) As SpectrumPeak()
        Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
        .SpectrumComment = SpectrumComment.precursor
    },
    New SpectrumPeak(adduct.ConvertToMz(C6H10O5 + C2H5NO), 400.0R, "Hex + C2H5NO") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    }
}
        If Equals(adduct.AdductIonName, "[M+Na]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - C5H10O4, 150.0R, "Precursor-C5H10O4") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(adduct.ConvertToMz(C5H10O4) - HydrogenMass, 100.0R, "[C5H9O4+adduct]+") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(adduct.ConvertToMz(C6H10O5 + H2O), 100.0R, "[Hex+adduct+H]+") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(adduct.ConvertToMz(C6H10O5 + OxygenMass), 100.0R, "[Hex+adduct-H]+") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - C6H10O5 - H2O, 100.0R, "Precursor-Hex-H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}})
        Else
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - H2O, 200.0R, "Precursor-H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - C6H10O5, 300.0R, "Precursor-Hex") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - C6H10O5 - H2O * 2, 200.0R, "Precursor-Hex-2H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - C6H10O5 - H2O, 400.0R, "Precursor-Hex-H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}})
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
}, New SpectrumPeak(chainMass + ProtonMass - CH4O2, 150.0R, "[sph+H]+ -CH4O2") With {
.SpectrumComment = SpectrumComment.acylchain
}})
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetAcylSpectrum(lipid As ILipid, acyl As AcylChain, adduct As AdductIon) As SpectrumPeak()
        Dim chainMass = acyl.Mass + HydrogenMass
        Dim spectrum = New List(Of SpectrumPeak)() From {
                 New SpectrumPeak(adduct.ConvertToMz(chainMass) + C2H3NO + C6H10O5, 150.0R, "[FAA+C2H4O+Hex+adduct]+") With {
        .SpectrumComment = SpectrumComment.acylchain
    }

}
            If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+H-H2O]+") Then
            spectrum.AddRange({New SpectrumPeak(chainMass + ProtonMass + C2H3NO - HydrogenMass - OxygenMass, 200.0R, "[FAA+C2H+H]+") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + ProtonMass + NH, 200.0R, "[FAA+H]+") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + ProtonMass + C2H3NO - HydrogenMass - OxygenMass, 200.0R, "[FAA+C2H+H]+") With {
.SpectrumComment = SpectrumComment.acylchain
}})
            If Equals(adduct.AdductIonName, "[M+H]+") Then
                spectrum.AddRange({New SpectrumPeak(chainMass + ProtonMass + C2H3NO, 150.0R, "[FAA+C2H2O+H]+") With {
.SpectrumComment = SpectrumComment.acylchain
}})
            End If
        ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(chainMass + C2H3NO - OxygenMass), 200.0R, "[FAA+C2H+Na]+") With {
.SpectrumComment = SpectrumComment.acylchain
}})
        End If
        Return spectrum.ToArray()
    End Function

    Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()

End Class

