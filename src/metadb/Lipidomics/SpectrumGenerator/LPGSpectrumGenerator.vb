#Region "Microsoft.VisualBasic::e1c6e97bcd928f58e435a1dd15fd36fc, metadb\Lipidomics\SpectrumGenerator\LPGSpectrumGenerator.vb"

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

'   Total Lines: 163
'    Code Lines: 141
' Comment Lines: 1
'   Blank Lines: 21
'     File Size: 8.95 KB


' Class LPGSpectrumGenerator
' 
'     Constructor: (+2 Overloads) Sub New
'     Function: CanGenerate, CreateReference, Generate, GetAcylDoubleBondSpectrum, (+2 Overloads) GetAcylLevelSpectrum
'               GetAcylPositionSpectrum, GetLPGSpectrum
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports BioNovoGene.BioDeep.MSEngine

Public Class LPGSpectrumGenerator
    Implements ILipidSpectrumGenerator

    Private Shared ReadOnly C3H9O6P As Double = {CarbonMass * 3, HydrogenMass * 9, OxygenMass * 6, PhosphorusMass}.Sum()

    Private Shared ReadOnly C3H6O2 As Double = {CarbonMass * 3, HydrogenMass * 6, OxygenMass * 2}.Sum()

    Private Shared ReadOnly Gly_C As Double = {CarbonMass * 6, HydrogenMass * 13, OxygenMass * 6, PhosphorusMass}.Sum()

    Private Shared ReadOnly Gly_O As Double = {CarbonMass * 5, HydrogenMass * 11, OxygenMass * 7, PhosphorusMass}.Sum()

    Private Shared ReadOnly CH2 As Double = {HydrogenMass * 2, CarbonMass}.Sum()

    Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()

    Public Sub New()
        spectrumGenerator = New SpectrumPeakGenerator()
    End Sub

    Public Sub New(peakGenerator As ISpectrumPeakGenerator)
        spectrumGenerator = peakGenerator
    End Sub

    Public Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
        If lipid.LipidClass = LbmClass.LPG Then
            If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+NH4]+") OrElse Equals(adduct.AdductIonName, "[M+Na]+") Then
                Return True
            End If
        End If
        Return False
    End Function

    Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
        Dim spectrum = New List(Of SpectrumPeak)()
        Dim nlMass = 0.0
        spectrum.AddRange(GetLPGSpectrum(lipid, adduct))
        If lipid.Description.Has(LipidDescription.Chain) Then
            spectrum.AddRange(GetAcylLevelSpectrum(lipid, lipid.Chains.GetDeterminedChains(), adduct))
            lipid.Chains.ApplyToChain(1, Sub(chain) spectrum.AddRange(GetAcylPositionSpectrum(lipid, chain, adduct)))
            spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, lipid.Chains.GetTypedChains(Of AcylChain)(), adduct))
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

    Private Function GetLPGSpectrum(lipid As ILipid, adduct As AdductIon) As SpectrumPeak()
        Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
        Dim lipidMass = lipid.Mass + adductmass

        Dim spectrum = New List(Of SpectrumPeak)()
        If Equals(adduct.AdductIonName, "[M+H]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}})
        ElseIf Equals(adduct.AdductIonName, "[M+NH4]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 500.0R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}, New SpectrumPeak(lipidMass, 999.0R, "[M+H]+ ") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
        End If
        If Equals(adduct.AdductIonName, "[M+Na]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(C3H9O6P), 300.0R, "Header") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}, New SpectrumPeak(adduct.ConvertToMz(C3H9O6P - H2O), 200.0R, "Header - H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}, New SpectrumPeak(adduct.ConvertToMz(Gly_C), 100.0R, "Gly-C") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(adduct.ConvertToMz(Gly_O), 100.0R, "Gly-O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}})
        End If
        If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+NH4]+") Then
            spectrum.AddRange({New SpectrumPeak(lipidMass - C3H6O2, 100.0R, "Precursor -C3H6O2") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(lipidMass - H2O, 500.0R, "[M+H]+ -H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(lipidMass - H2O * 2, 200.0R, "[M+H]+ -2H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(lipidMass - C3H6O2 - H2O, 100.0R, "[M+H]+ -C3H6O2 -H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(Gly_C + ProtonMass, 100.0R, "Gly-C") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(Gly_O + ProtonMass, 100.0R, "Gly-O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(C3H9O6P + ProtonMass, 300.0R, "Header") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}, New SpectrumPeak(C3H9O6P - H2O + ProtonMass, 200.0R, "Header - H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}, New SpectrumPeak(lipid.Mass - C3H9O6P + ProtonMass, 500.0R, "Precursor -C3H9O6P") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetAcylDoubleBondSpectrum(lipid As ILipid, acylChains As IEnumerable(Of AcylChain), adduct As AdductIon) As IEnumerable(Of SpectrumPeak)
        'var nlMass = adduct.AdductIonName == "[M+NH4]+" ? adduct.AdductIonAccurateMass - MassDiffDictionary.ProtonMass : H2O;
        Dim nlMass = 0.0
        Return acylChains.SelectMany(Function(acylChain) spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acylChain, adduct, nlMass, 20.0R))
    End Function

    Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChains As IEnumerable(Of IChain), adduct As AdductIon) As IEnumerable(Of SpectrumPeak)
        Return acylChains.SelectMany(Function(acylChain) GetAcylLevelSpectrum(lipid, acylChain, adduct))
    End Function

    Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
        Dim chainMass = acylChain.Mass - HydrogenMass
        Dim adductMass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
        Dim spectrum = New List(Of SpectrumPeak)()
        If chainMass <> 0.0 Then
            spectrum.AddRange({New SpectrumPeak(lipid.Mass - chainMass + adductMass, 200.0R, $"-{acylChain}") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + ProtonMass, 100.0R, $"{acylChain} acyl") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - chainMass - H2O), 100.0R, $"-{acylChain}-H2O") With {
.SpectrumComment = SpectrumComment.acylchain
}})
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetAcylPositionSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
        Dim chainMass = acylChain.Mass
        Return {New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - chainMass - HydrogenMass - OxygenMass - CH2), 100.0R, "-CH2(Sn1)") With {
.SpectrumComment = SpectrumComment.snposition
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - chainMass - HydrogenMass - OxygenMass - H2O - CH2), 100.0R, "-H2O -CH2(Sn1)") With {
.SpectrumComment = SpectrumComment.snposition
}}
    End Function

    Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator

End Class
