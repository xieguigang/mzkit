Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemistry.MetaLib
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

    Public Class EtherPEEidSpectrumGenerator
        Implements ILipidSpectrumGenerator
        Private Shared ReadOnly C2H8NO4P As Double = {CarbonMass * 2, HydrogenMass * 8, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()

        Private Shared ReadOnly C2H5N As Double = {CarbonMass * 2, HydrogenMass * 5, NitrogenMass}.Sum()

        Private Shared ReadOnly H3PO4 As Double = {HydrogenMass * 3, PhosphorusMass, OxygenMass * 4}.Sum()

        Private Shared ReadOnly Gly_C As Double = {CarbonMass * 5, HydrogenMass * 12, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()

        Private Shared ReadOnly Gly_O As Double = {CarbonMass * 4, HydrogenMass * 10, NitrogenMass, OxygenMass * 5, PhosphorusMass}.Sum()

        Private Shared ReadOnly CH2 As Double = {HydrogenMass * 2, CarbonMass}.Sum()

        Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()

        Public Sub New()
            spectrumGenerator = New SpectrumPeakGenerator()
        End Sub

        Public Sub New(peakGenerator As ISpectrumPeakGenerator)
            spectrumGenerator = If(peakGenerator, CSharpImpl.__Throw(Of ISpectrumPeakGenerator)(New ArgumentNullException(NameOf(peakGenerator))))
        End Sub

        Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator


        Public Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
            If lipid.LipidClass = LbmClass.EtherPE Then
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    Return True
                End If
            End If
            Return False
        End Function

        Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
            Dim spectrum = New List(Of SpectrumPeak)()
            spectrum.AddRange(GetEtherPESpectrum(lipid, adduct))
            lipid.Chains.ApplyToChain(1, Sub(chain) spectrum.AddRange(GetSn1PositionSpectrum(lipid, chain, adduct)))

            Dim alkyl As AlkylChain = Nothing, acyl As AcylChain = Nothing
            (alkyl, acyl) = lipid.Chains.Deconstruct(Of AlkylChain, AcylChain)()
            If alkyl IsNot Nothing AndAlso acyl IsNot Nothing Then
                If alkyl.DoubleBond.Bonds.Any(Function(b) b.Position = 1) Then
                    spectrum.AddRange(GetEtherPEPSpectrum(lipid, alkyl, acyl, adduct))
                Else
                    spectrum.AddRange(GetEtherPEOSpectrum(lipid, alkyl, acyl, adduct))
                End If
                spectrum.AddRange(spectrumGenerator.GetAlkylDoubleBondSpectrum(lipid, alkyl, adduct, 0R, 30R))
                'spectrum.AddRange(spectrumGenerator.GetAlkylDoubleBondSpectrum(lipid, alkyl, adduct, nlMass: C2H8NO4P, 30d));

                spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acyl, adduct, 0R, 30R))
                'spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acyl, adduct, nlMass: C2H8NO4P, 50d));
            End If
            spectrum.AddRange(EidSpecificSpectrum(lipid, adduct, 0R, 50R))
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

        Private Function GetEtherPESpectrum(lipid As ILipid, adduct As AdductIon) As SpectrumPeak()
            Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999R, "Precursor") With {
        .SpectrumComment = SpectrumComment.precursor
    },
    New SpectrumPeak(adduct.ConvertToMz(C2H8NO4P), 100R, "Header") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    },
    New SpectrumPeak(adduct.ConvertToMz(Gly_C), 100R, "Gly-C") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    },
    New SpectrumPeak(adduct.ConvertToMz(Gly_O), 100R, "Gly-O") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    },
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) / 2, 100R, "[Precursor]2+") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - C2H8NO4P), 500R, "Precursor -C2H8NO4P") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    }
}
            Return spectrum.ToArray()
        End Function

        Private Function GetEtherPEPSpectrum(lipid As ILipid, alkylChain As IChain, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
            Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass + HydrogenMass), 30R, $"-{alkylChain}") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass), 30R, $"-{acylChain}") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass - OxygenMass), 150, $"-{alkylChain}-O") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass - H2O), 30R, $"-{acylChain}-O") With {
        .SpectrumComment = SpectrumComment.acylchain
    }
}
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(alkylChain.Mass + C2H8NO4P - HydrogenMass), 250R, "Sn1Ether+C2H8NO3P") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(alkylChain.Mass + C2H8NO4P - H3PO4 - HydrogenMass), 150R, "Sn1Ether+C2H8NO3P-H3PO4") With { ' Sn1 + O + C2H8NO3P
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - C2H8NO4P - alkylChain.Mass + HydrogenMass), 300, "NL of C2H8NO4P+Sn1") With {
.SpectrumComment = SpectrumComment.acylchain
}})
            Return spectrum.ToArray()
        End Function

        Private Function GetEtherPEOSpectrum(lipid As ILipid, alkylChain As IChain, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
            Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass + HydrogenMass), 50R, $"-{alkylChain}") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass + HydrogenMass), 50R, $"-{acylChain}") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass - OxygenMass), 200R, $"-{alkylChain}-O") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass - H2O), 200R, $"-{acylChain}-O") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(acylChain.Mass, 50R, $"{acylChain} acyl+") With {
        .SpectrumComment = SpectrumComment.acylchain
    },

    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass - C2H8NO4P + HydrogenMass), 50R, $"- Header -{alkylChain}") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass - C2H8NO4P + HydrogenMass), 50R, $"- Header -{acylChain}") With {
        .SpectrumComment = SpectrumComment.acylchain
    }
'new SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass- C2H8NO4P - MassDiffDictionary.OxygenMass - MassDiffDictionary.HydrogenMass), 200d, $"- Header -{alkylChain}-O") { SpectrumComment = SpectrumComment.acylchain },
'new SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass- C2H8NO4P - H2O), 200d, $"- Header -{acylChain}-O") { SpectrumComment = SpectrumComment.acylchain },
}
            Return spectrum.ToArray()
        End Function

        Private Function GetSn1PositionSpectrum(lipid As ILipid, alkylChain As IChain, adduct As AdductIon) As SpectrumPeak()
            Return {New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass - OxygenMass - CH2), 50R, "-CH2(Sn1)") With {
.SpectrumComment = SpectrumComment.snposition
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass - C2H8NO4P - OxygenMass - CH2), 200R, "- Header -CH2(Sn1)") With {
.SpectrumComment = SpectrumComment.snposition
}}
        End Function
        Private Shared Function EidSpecificSpectrum(lipid As Lipid, adduct As AdductIon, nlMass As Double, intensity As Double) As SpectrumPeak()
            Dim spectrum = New List(Of SpectrumPeak)()
            Dim chains As SeparatedChains = Nothing

            If CSharpImpl.__Assign(chains, TryCast(lipid.Chains, SeparatedChains)) IsNot Nothing Then
                For Each chain In chains.GetDeterminedChains()
                    If chain.DoubleBond.Count = 0 OrElse chain.DoubleBond.UnDecidedCount > 0 Then Continue For
                    spectrum.AddRange(EidSpecificSpectrumGenerator.EidSpecificSpectrumGen(lipid, chain, adduct, nlMass, intensity))
                Next
            End If
            Return spectrum.ToArray()
        End Function

        Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()


    End Class

