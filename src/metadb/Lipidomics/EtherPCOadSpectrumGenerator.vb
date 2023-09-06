Imports CompMs.Common.Components
Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.FormulaGenerator.DataObj
Imports CompMs.Common.Interfaces
Imports System
Imports System.Collections.Generic
Imports System.Linq


Public Class EtherPCOadSpectrumGenerator
        Implements ILipidSpectrumGenerator
        Private Shared ReadOnly C5H14NO4P As Double = {CarbonMass * 5, HydrogenMass * 14, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()

        Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()

        Private Shared ReadOnly CH3 As Double = {HydrogenMass * 3, CarbonMass}.Sum()
        Private Shared ReadOnly Electron As Double = 0.00054858026

        Private ReadOnly spectrumGenerator As IOadSpectrumPeakGenerator
        Public Sub New()
            spectrumGenerator = New OadSpectrumPeakGenerator()
        End Sub

        Public Sub New(ByVal spectrumGenerator As IOadSpectrumPeakGenerator)
            Me.spectrumGenerator = If(spectrumGenerator, CSharpImpl.__Throw(Of IOadSpectrumPeakGenerator)(New ArgumentNullException(NameOf(spectrumGenerator))))
        End Sub

        Public Function CanGenerate(ByVal lipid As ILipid, ByVal adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
            If Equals(adduct.AdductIonName, "[M+H]+") Then
                Return True
            End If
            Return False
        End Function

        Public Function Generate(ByVal lipid As Lipid, ByVal adduct As AdductIon, ByVal Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
            Dim abundance = 40.0
            Dim nlMass = 0.0
            Dim spectrum = New List(Of SpectrumPeak)()
            spectrum.AddRange(GetEtherPCOadSpectrum(lipid, adduct))
            '"OAD02+O",
            '"OAD05",
            '"OAD06",
            '"OAD07",
            '"OAD09",
            '"OAD10",
            '"OAD11",
            '"OAD12",
            '"OAD13",
            '"OAD14",
            '"OAD15",
            '"OAD15+O",
            '"OAD16",
            '"OAD17",
            '"OAD01+H"
            Dim oadId = New String() {"OAD01", "OAD02", "OAD03", "OAD04", "OAD08", "OAD12+O", "OAD12+O+H", "OAD12+O+2H"}
            Dim alkyl As AlkylChain = Nothing, acyl As AcylChain = Nothing
            (alkyl, acyl) = lipid.Chains.Deconstruct(Of AlkylChain, AcylChain)()
            If alkyl IsNot Nothing AndAlso acyl IsNot Nothing Then
                If alkyl.DoubleBond.Bonds.Any(Function(b) b.Position = 1) Then
                    spectrum.AddRange(GetEtherPCPSpectrum(lipid, alkyl, acyl, adduct))
                Else
                    spectrum.AddRange(GetEtherPCOSpectrum(lipid, alkyl, acyl, adduct))
                End If
                spectrum.AddRange(spectrumGenerator.GetAlkylDoubleBondSpectrum(lipid, alkyl, adduct, nlMass, abundance, oadId))
                spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acyl, adduct, nlMass, abundance, oadId))
            End If

            spectrum = spectrum.GroupBy(Function(spec) spec, comparer).[Select](Function(specs) New SpectrumPeak(Enumerable.First(specs).Mass, specs.Sum(Function(n) n.Intensity), String.Join(", ", specs.[Select](Function(spec) spec.Comment)), specs.Aggregate(SpectrumComment.none, Function(a, b) a Or b.SpectrumComment))).OrderBy(Function(peak) peak.Mass).ToList()
            Return CreateReference(lipid, adduct, spectrum, molecule)
        End Function

        Private Function GetEtherPCOadSpectrum(ByVal lipid As Lipid, ByVal adduct As AdductIon) As SpectrumPeak()
            Dim spectrum = New List(Of SpectrumPeak)()

            If Equals(adduct.AdductIonName, "[M+H]+") Then
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 500R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}, New SpectrumPeak(adduct.ConvertToMz(C5H14NO4P), 999R, "Header") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}})
            Else
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}})
            End If
            Return spectrum.ToArray()
        End Function

        Private Function GetEtherPCPSpectrum(ByVal lipid As ILipid, ByVal alkylChain As IChain, ByVal acylChain As IChain, ByVal adduct As AdductIon) As SpectrumPeak()
            'new SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass), 100d, $"-{alkylChain}"),
            'new SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass), 100d, $"-{acylChain}"),
            Return {New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass - OxygenMass - HydrogenMass), 50R, $"-{alkylChain}-O") With {
            .SpectrumComment = SpectrumComment.acylchain
        'new SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass - MassDiffDictionary.OxygenMass), 200d, $"-{acylChain}-O") { SpectrumComment = SpectrumComment.acylchain },
        }}
        End Function

        Private Function GetEtherPCOSpectrum(ByVal lipid As ILipid, ByVal alkylChain As IChain, ByVal acylChain As IChain, ByVal adduct As AdductIon) As SpectrumPeak()
            'new SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass), 100d, $"-{alkylChain}"){ SpectrumComment = SpectrumComment.acylchain },
            'new SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass - MassDiffDictionary.OxygenMass), 100d, $"-{alkylChain}-O") { SpectrumComment = SpectrumComment.acylchain },
            Return {New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass + HydrogenMass), 100R, $"-{acylChain}") With {
            .SpectrumComment = SpectrumComment.acylchain
            }, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass + HydrogenMass * 2), 50R, $"-{acylChain} +H") With {
            .SpectrumComment = SpectrumComment.acylchain
            }, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass - H2O + HydrogenMass), 80R, $"-{acylChain}-O") With {
            .SpectrumComment = SpectrumComment.acylchain
            }, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass - H2O + HydrogenMass * 2), 50R, $"-{acylChain}-O +H") With {
            .SpectrumComment = SpectrumComment.acylchain
        }}
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

        Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()


End Class

