#Region "Microsoft.VisualBasic::c2f8adb4e0f853ec9b96290311e9a449, G:/mzkit/src/metadb/Lipidomics//SpectrumGenerator/EtherPESpectrumGenerator.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 176
    '    Code Lines: 149
    ' Comment Lines: 3
    '   Blank Lines: 24
    '     File Size: 10.16 KB


    ' Class EtherPESpectrumGenerator
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: CanGenerate, CreateReference, Generate, GetEtherPEOSpectrum, GetEtherPEPSpectrum
    '               GetEtherPESpectrum, GetSn1PositionSpectrum
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MSEngine
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

Public Class EtherPESpectrumGenerator
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
        spectrumGenerator = peakGenerator
    End Sub

    Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator


    Public Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
        If lipid.LipidClass = LbmClass.EtherPE Then
            If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+Na]+") Then
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
        With lipid.Chains.Deconstruct(Of AlkylChain, AcylChain)()
            alkyl = .Item1
            acyl = .Item2
        End With
        If alkyl IsNot Nothing AndAlso acyl IsNot Nothing Then
            If alkyl.DoubleBond.Bonds.Any(Function(b) b.Position = 1) Then
                spectrum.AddRange(GetEtherPEPSpectrum(lipid, alkyl, acyl, adduct))
            Else
                spectrum.AddRange(GetEtherPEOSpectrum(lipid, alkyl, acyl, adduct))
            End If
            spectrum.AddRange(spectrumGenerator.GetAlkylDoubleBondSpectrum(lipid, alkyl, adduct, 0R, 30.0R))
            'spectrum.AddRange(spectrumGenerator.GetAlkylDoubleBondSpectrum(lipid, alkyl, adduct, nlMass: C2H8NO4P, 30d));

            spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acyl, adduct, 0R, 50.0R))
            'spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acyl, adduct, nlMass: C2H8NO4P, 50d));
        End If
        spectrum = spectrum.GroupBy(Function(spec) spec, comparer).[Select](Function(specs) New SpectrumPeak(Enumerable.First(specs).mz, specs.Sum(Function(n) n.Intensity), String.Join(", ", specs.[Select](Function(spec) spec.Annotation)), specs.Aggregate(SpectrumComment.none, Function(a, b) a Or b.SpectrumComment))).OrderBy(Function(peak) peak.mz).ToList()
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
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
        .SpectrumComment = SpectrumComment.precursor
    },
    New SpectrumPeak(adduct.ConvertToMz(C2H8NO4P), 100.0R, "Header") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    },
    New SpectrumPeak(adduct.ConvertToMz(Gly_C), 300.0R, "Gly-C") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    },
    New SpectrumPeak(adduct.ConvertToMz(Gly_O), 300.0R, "Gly-O") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    },
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) / 2, 100.0R, "[Precursor]2+") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    }
}
        If Equals(adduct.AdductIonName, "[M+Na]+") Then
            'new SpectrumPeak(adduct.ConvertToMz(lipid.Mass - C2H8NO4P), 150d, "Precursor -C2H8NO4P") { SpectrumComment = SpectrumComment.metaboliteclass },
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - C2H5N), 150.0R, "Precursor -C2H5N") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
                         }})
        Else
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - C2H8NO4P), 500.0R, "Precursor -C2H8NO4P") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})

        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetEtherPEPSpectrum(lipid As ILipid, alkylChain As IChain, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
        Dim spectrum = New List(Of SpectrumPeak) From {
New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass + HydrogenMass), 30.0R, $"-{alkylChain}") With {
    .SpectrumComment = SpectrumComment.acylchain
},
New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass), 30.0R, $"-{acylChain}") With {
    .SpectrumComment = SpectrumComment.acylchain
},
New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass - OxygenMass), 150, $"-{alkylChain}-O") With {
    .SpectrumComment = SpectrumComment.acylchain
},
New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass - H2O), 30.0R, $"-{acylChain}-O") With {
    .SpectrumComment = SpectrumComment.acylchain
}
}
        If Equals(adduct.AdductIonName, "[M+H]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(alkylChain.Mass + C2H8NO4P - HydrogenMass), 250.0R, "Sn1Ether+C2H8NO3P") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(alkylChain.Mass + C2H8NO4P - H3PO4 - HydrogenMass), 150.0R, "Sn1Ether+C2H8NO3P-H3PO4") With { ' Sn1 + O + C2H8NO3P
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - C2H8NO4P - alkylChain.Mass + HydrogenMass), 300, "NL of C2H8NO4P+Sn1") With {
.SpectrumComment = SpectrumComment.acylchain
}})
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetEtherPEOSpectrum(lipid As ILipid, alkylChain As IChain, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
        Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass + HydrogenMass), 50.0R, $"-{alkylChain}") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass + HydrogenMass), 50.0R, $"-{acylChain}") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass - OxygenMass), 200.0R, $"-{alkylChain}-O") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass - H2O), 200.0R, $"-{acylChain}-O") With {
        .SpectrumComment = SpectrumComment.acylchain
    }'new SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass- C2H8NO4P + MassDiffDictionary.HydrogenMass), 50d, $"- Header -{alkylChain}") { SpectrumComment = SpectrumComment.acylchain },'new SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass- C2H8NO4P + MassDiffDictionary.HydrogenMass), 50d, $"- Header -{acylChain}") { SpectrumComment = SpectrumComment.acylchain },'new SpectrumPeak(adduct.ConvertToMz(lipid.Mass - alkylChain.Mass- C2H8NO4P - MassDiffDictionary.OxygenMass - MassDiffDictionary.HydrogenMass), 200d, $"- Header -{alkylChain}-O") { SpectrumComment = SpectrumComment.acylchain },'new SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass- C2H8NO4P - H2O), 200d, $"- Header -{acylChain}-O") { SpectrumComment = SpectrumComment.acylchain },
}
        If Equals(adduct.AdductIonName, "[M+Na]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - C2H5N - H2O + HydrogenMass), 500.0R, "Precursor -C2H6NO")})
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetSn1PositionSpectrum(lipid As ILipid, chain As IChain, adduct As AdductIon) As SpectrumPeak()
        Return {New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - chain.Mass - OxygenMass - CH2), 50.0R, "-CH2(Sn1)") With {
.SpectrumComment = SpectrumComment.snposition'new SpectrumPeak(adduct.ConvertToMz(lipid.Mass - chain.Mass - C2H8NO4P - MassDiffDictionary.OxygenMass - CH2), 50d, "- Header -CH2(Sn1)") { SpectrumComment = SpectrumComment.snposition },
}}
    End Function

    Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()

End Class


