Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MSEngine
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

Public Class CerNSd7SpectrumGenerator
    Implements ILipidSpectrumGenerator
    Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()
    Private Shared ReadOnly CH6O2 As Double = {CarbonMass * 1, HydrogenMass * 6, OxygenMass * 2}.Sum()
    Private Shared ReadOnly CH4O2 As Double = {CarbonMass * 1, HydrogenMass * 4, OxygenMass * 2}.Sum()
    Private Shared ReadOnly C2H3NO As Double = {CarbonMass * 2, HydrogenMass * 3, NitrogenMass * 1, OxygenMass * 1}.Sum()
    Private Shared ReadOnly NH As Double = {NitrogenMass * 1, HydrogenMass * 1}.Sum()
    Private Shared ReadOnly CH3O As Double = {CarbonMass * 1, HydrogenMass * 3, OxygenMass * 1}.Sum()
    Private Shared ReadOnly CH2 As Double = {HydrogenMass * 2, CarbonMass}.Sum()
    Private Shared ReadOnly sphD7MassBalance As Double = {Hydrogen2Mass * 7, -HydrogenMass * 7}.Sum()
    Private Shared ReadOnly CD2 As Double = {Hydrogen2Mass * 2, CarbonMass}.Sum()

    Public Sub New()
        spectrumGenerator = New SpectrumPeakGenerator()
    End Sub
    Public Sub New(spectrumGenerator As ISpectrumPeakGenerator)
        Me.spectrumGenerator = spectrumGenerator
    End Sub

    Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator

    Public Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
        If lipid.LipidClass = LbmClass.Cer_NS_d7 Then
            If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+H-H2O]+") OrElse Equals(adduct.AdductIonName, "[M+Na]+") Then
                Return True
            End If
        End If
        Return False
    End Function

    Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
        Dim spectrum = New List(Of SpectrumPeak)()
        Dim nlmass = If(Equals(adduct.AdductIonName, "[M+H]+"), H2O, 0.0)
        spectrum.AddRange(GetCerNSd7Spectrum(lipid, adduct))
        Dim sphingo As SphingoChain = TryCast(lipid.Chains.GetChainByPosition(1), SphingoChain)

        If sphingo IsNot Nothing Then
            spectrum.AddRange(GetSphingoSpectrum(lipid, sphingo, adduct))
            spectrum.AddRange(GetSphingoDoubleBondSpectrum(lipid, sphingo, adduct, nlmass, 100.0R))
        End If
        Dim acyl As AcylChain = TryCast(lipid.Chains.GetChainByPosition(2), AcylChain)

        If acyl IsNot Nothing Then
            spectrum.AddRange(GetAcylSpectrum(lipid, acyl, adduct))
            spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acyl, adduct, nlmass - sphD7MassBalance, 30.0R))
        End If
        spectrum = spectrum.GroupBy(Function(spec) spec, comparer).[Select](Function(specs) New SpectrumPeak(Enumerable.First(specs).mz, specs.Sum(Function(n) n.Intensity), String.Join(", ", specs.[Select](Function(spec) spec.Comment)), specs.Aggregate(SpectrumComment.none, Function(a, b) a Or b.SpectrumComment))).OrderBy(Function(peak) peak.Mass).ToList()
        Return CreateReference(lipid, adduct, spectrum, molecule)
    End Function

    Private Function CreateReference(lipid As ILipid, adduct As AdductIon, spectrum As List(Of SpectrumPeak), molecule As IMoleculeProperty) As MoleculeMsReference
        Return New MoleculeMsReference With {
    .PrecursorMz = adduct.ConvertToMz(lipid.Mass + sphD7MassBalance),
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

    Private Function GetCerNSd7Spectrum(lipid As ILipid, adduct As AdductIon) As SpectrumPeak()
        Dim lipidD7mass = lipid.Mass + sphD7MassBalance
        Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(adduct.ConvertToMz(lipidD7mass), 999.0R, "Precursor") With {
        .SpectrumComment = SpectrumComment.precursor
    }
}
        If Equals(adduct.AdductIonName, "[M+Na]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipidD7mass) - CH3O, 150.0R, "Precursor-CH3O") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}})
        Else
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipidD7mass) - H2O, 200.0R, "Precursor-H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}, New SpectrumPeak(lipidD7mass + ProtonMass - CH6O2, 100.0R, "[M+H]+ -CH6O2") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
            If Equals(adduct.AdductIonName, "[M+H]+") Then
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipidD7mass) - H2O * 2, 100.0R, "Precursor-2H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
            End If
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetSphingoSpectrum(lipid As ILipid, sphingo As SphingoChain, adduct As AdductIon) As SpectrumPeak()
        Dim chainMass = sphingo.Mass + HydrogenMass + sphD7MassBalance
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
    Public Function GetSphingoDoubleBondSpectrum(lipid As ILipid, sphingo As SphingoChain, adduct As AdductIon, nlMass As Double, abundance As Double) As IEnumerable(Of SpectrumPeak)
        Dim sphingoD7mass = sphingo.Mass + sphD7MassBalance
        If sphingo.DoubleBond.UnDecidedCount <> 0 OrElse sphingo.CarbonCount = 0 OrElse sphingo.Oxidized.UnDecidedCount <> 0 Then
            Return Enumerable.Empty(Of SpectrumPeak)()
        End If

        '+ MassDiffDictionary.OxygenMass * (sphingo.OxidizedCount > 2 ? 2 : sphingo.OxidizedCount)
        Dim chainLoss = lipid.Mass + sphD7MassBalance - sphingoD7mass - nlMass + NitrogenMass + 12 * 2 + OxygenMass * 1 + HydrogenMass * 5
        Dim diffs = New Double(sphingo.CarbonCount - 1) {}
        For i = 0 To sphingo.CarbonCount - 1
            diffs(i) = CH2
        Next
        For i = sphingo.CarbonCount - 2 To sphingo.CarbonCount - 1
            diffs(i) = CD2
        Next
        If sphingo.Oxidized IsNot Nothing Then
            For Each ox In sphingo.Oxidized.Oxidises
                If ox > 2 Then
                    diffs(ox - 1) = diffs(ox - 1) + OxygenMass
                End If
            Next
        End If

        For Each bond In sphingo.DoubleBond.Bonds
            diffs(bond.Position - 1) -= HydrogenMass
            diffs(bond.Position) -= HydrogenMass
        Next
        For i = 3 To sphingo.CarbonCount - 1
            diffs(i) += diffs(i - 1)
        Next

        Dim peaks = New List(Of SpectrumPeak)()
        Dim bondPositions = New List(Of Integer)()
        For i = 2 To sphingo.CarbonCount - 1 - 1
            Dim speccomment = SpectrumComment.doublebond
            Dim factor = 1.0
            Dim factorHLoss = 0.5
            Dim factorHGain = 0.2

            If bondPositions.Contains(i - 1) Then ' in the case of 18:2(9,12), Radical is big, and H loss is next
                factor = 4.0
                factorHLoss = 2.0
                speccomment = speccomment Or SpectrumComment.doublebond_high
                ' now no modification
            ElseIf bondPositions.Contains(i) Then
            ElseIf bondPositions.Contains(i + 1) Then
                factor = 0.25
                factorHLoss = 0.5
                factorHGain = 0.05
                speccomment = speccomment Or SpectrumComment.doublebond_low
                ' now no modification
            ElseIf bondPositions.Contains(i + 2) Then
            ElseIf bondPositions.Contains(i + 3) Then
                If bondPositions.Contains(i) Then
                    factor = 4.0
                    factorHLoss = 0.5
                    factorHGain = 2.0
                Else
                    factorHLoss = 4.0
                    speccomment = speccomment Or SpectrumComment.doublebond_high
                End If
                speccomment = speccomment Or SpectrumComment.doublebond_high
            End If

            peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(i) - HydrogenMass), factorHLoss * abundance, $"{sphingo} C{i + 1}-H") With {
                    .SpectrumComment = speccomment
                })
            peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(i)), factor * abundance, $"{sphingo} C{i + 1}") With {
                    .SpectrumComment = speccomment
                })
            peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(i) + HydrogenMass), factorHGain * abundance, $"{sphingo} C{i + 1}+H") With {
                    .SpectrumComment = speccomment
                })

            '    for (int i = 2; i < sphingo.CarbonCount - 1; i++)
            '{
            '    peaks.Add(new SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs[i] - MassDiffDictionary.HydrogenMass), abundance * 0.5, $"{sphingo} C{i + 1}-H") { SpectrumComment = SpectrumComment.doublebond });
            '    peaks.Add(new SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs[i]), abundance, $"{sphingo} C{i + 1}") { SpectrumComment = SpectrumComment.doublebond });
            '    peaks.Add(new SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs[i] + MassDiffDictionary.HydrogenMass), abundance * 0.5, $"{sphingo} C{i + 1}+H") { SpectrumComment = SpectrumComment.doublebond });
            '}
        Next
        Return peaks
    End Function

    Private Function GetAcylSpectrum(lipid As ILipid, acyl As AcylChain, adduct As AdductIon) As SpectrumPeak()
        Dim chainMass = acyl.Mass + HydrogenMass
        Dim spectrum = New List(Of SpectrumPeak)()
        If Equals(adduct.AdductIonName, "[M+Na]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(chainMass) + C2H3NO + HydrogenMass, 150.0R, "[FAA+C2H2O+H]+") With {
.SpectrumComment = SpectrumComment.acylchain
}})
        Else
            spectrum.AddRange({New SpectrumPeak(chainMass + ProtonMass + NH, 200.0R, "[FAA+H]+") With {
.SpectrumComment = SpectrumComment.acylchain
}})
            If Equals(adduct.AdductIonName, "[M+H]+") Then
                spectrum.AddRange({New SpectrumPeak(chainMass + ProtonMass + C2H3NO, 150.0R, "[FAA+C2H2O+H]+") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + ProtonMass + C2H3NO - HydrogenMass - OxygenMass, 200.0R, "[FAA+C2H+H]+") With {
.SpectrumComment = SpectrumComment.acylchain
}})
            End If
        End If
        Return spectrum.ToArray()
    End Function

    Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()

End Class
