#Region "Microsoft.VisualBasic::b779e090477e4c4fbc498868b3e9c0ca, metadb\Lipidomics\SpectrumGenerator\SMSpectrumGenerator.vb"

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

'   Total Lines: 137
'    Code Lines: 126
' Comment Lines: 0
'   Blank Lines: 11
'     File Size: 7.50 KB


' Class SMSpectrumGenerator
' 
'     Constructor: (+2 Overloads) Sub New
'     Function: CanGenerate, CreateReference, Generate, GetAcylSpectrum, GetSMSpectrum
'               GetSphingoSpectrum
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports BioNovoGene.BioDeep.MSEngine

Public Class SMSpectrumGenerator
    Implements ILipidSpectrumGenerator
    Private Shared ReadOnly C5H14NO4P As Double = {CarbonMass * 5, HydrogenMass * 14, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()
    Private Shared ReadOnly C3H9N As Double = {CarbonMass * 3, HydrogenMass * 9, NitrogenMass}.Sum()
    Private Shared ReadOnly C5H11N As Double = {CarbonMass * 5, HydrogenMass * 11, NitrogenMass}.Sum()
    Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()
    Private Shared ReadOnly CH4O2 As Double = {CarbonMass * 1, HydrogenMass * 4, OxygenMass * 2}.Sum()
    Private Shared ReadOnly C2H3NO As Double = {CarbonMass * 2, HydrogenMass * 3, NitrogenMass * 1, OxygenMass * 1}.Sum()
    Private Shared ReadOnly C2H2N As Double = {CarbonMass * 2, HydrogenMass * 2, NitrogenMass * 1}.Sum()
    Private Shared ReadOnly NH4 As Double = {HydrogenMass * 4, NitrogenMass * 1}.Sum()

    Public Sub New()
        spectrumGenerator = New SpectrumPeakGenerator()
    End Sub
    Public Sub New(spectrumGenerator As ISpectrumPeakGenerator)
        Me.spectrumGenerator = spectrumGenerator
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
        Dim plChains As PositionLevelChains = TryCast(lipid.Chains, PositionLevelChains)
        Dim sphingo As SphingoChain = Nothing
        Dim acyl As AcylChain = Nothing

        If plChains IsNot Nothing Then
            sphingo = TryCast(lipid.Chains.GetChainByPosition(1), SphingoChain)
            If sphingo IsNot Nothing Then
                spectrum.AddRange(GetSphingoSpectrum(lipid, sphingo, adduct))
                spectrum.AddRange(spectrumGenerator.GetSphingoDoubleBondSpectrum(lipid, sphingo, adduct, nlmass, 30.0R))
            End If
            acyl = TryCast(lipid.Chains.GetChainByPosition(2), AcylChain)
            If acyl IsNot Nothing Then
                spectrum.AddRange(GetAcylSpectrum(lipid, acyl, adduct))
                spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acyl, adduct, nlmass, 30.0R))
            End If
        End If
        spectrum = spectrum.GroupBy(Function(spec) spec, comparer).[Select](Function(specs) New SpectrumPeak(Enumerable.First(specs).mz, specs.Sum(Function(n) n.intensity), String.Join(", ", specs.[Select](Function(spec) spec.Annotation)), specs.Aggregate(SpectrumComment.none, Function(a, b) a Or b.SpectrumComment))).OrderBy(Function(peak) peak.mz).ToList()
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
    New SpectrumPeak(adduct.ConvertToMz(C5H14NO4P + C2H3NO - OxygenMass), 400.0R, "C5H14NO4P(Header) + C2H3N") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    }
}
        If Equals(adduct.AdductIonName, "[M+Na]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - C3H9N, 300.0R, "Precursor-C3H9N") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - C5H11N, 150.0R, "Precursor-C5H11N") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - C5H14NO4P, 150.0R, "Precursor-Header") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
        ElseIf Equals(adduct.AdductIonName, "[M+H]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(C5H14NO4P + C2H3NO + CarbonMass), 150.0R, "C5H14NO4P(Header) + C3H3NO") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}}) ' need to consider
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetSphingoSpectrum(lipid As ILipid, sphingo As SphingoChain, adduct As AdductIon) As SpectrumPeak()
        Dim chainMass = sphingo.Mass + HydrogenMass
        Dim spectrum = New List(Of SpectrumPeak)()
        If Equals(adduct.AdductIonName, "[M+H]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(chainMass - H2O * 2), 100.0R, "[sph+H]+ -H2O") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(chainMass + C5H14NO4P - H2O - NH4), 100.0R, "Header+sph -NH4") With {
.SpectrumComment = SpectrumComment.acylchain'new SpectrumPeak(chainMass + MassDiffDictionary.ProtonMass - CH4O2, 100d, "[sph+H]+ -CH4O2"),
}})
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetAcylSpectrum(lipid As ILipid, acyl As AcylChain, adduct As AdductIon) As SpectrumPeak()
        Dim chainMass = acyl.Mass + HydrogenMass
        Dim spectrum = New List(Of SpectrumPeak)() From {
    New SpectrumPeak(adduct.ConvertToMz(chainMass) + C2H2N, 150.0R, "[FAA+C2H+adduct]+") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(adduct.ConvertToMz(chainMass) + C5H14NO4P + C2H2N - HydrogenMass, 200.0R, "[FAA+C2H+Header+adduct]+") With {
        .SpectrumComment = SpectrumComment.acylchain
    }
}
        Return spectrum.ToArray()
    End Function

End Class
