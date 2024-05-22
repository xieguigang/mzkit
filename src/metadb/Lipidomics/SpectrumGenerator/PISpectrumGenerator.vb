#Region "Microsoft.VisualBasic::297553471feac337185cc629956dfa79, metadb\Lipidomics\SpectrumGenerator\PISpectrumGenerator.vb"

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

    '   Total Lines: 145
    '    Code Lines: 123 (84.83%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 22 (15.17%)
    '     File Size: 8.15 KB


    ' Class PISpectrumGenerator
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: CanGenerate, CreateReference, Generate, GetAcylDoubleBondSpectrum, (+2 Overloads) GetAcylLevelSpectrum
    '               GetAcylPositionSpectrum, GetPISpectrum
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports BioNovoGene.BioDeep.MSEngine

Public Class PISpectrumGenerator
    Implements ILipidSpectrumGenerator

    Private Shared ReadOnly C6H13O9P As Double = {CarbonMass * 6, HydrogenMass * 13, OxygenMass * 9, PhosphorusMass}.Sum()

    Private Shared ReadOnly C6H10O5 As Double = {CarbonMass * 6, HydrogenMass * 10, OxygenMass * 5}.Sum()

    Private Shared ReadOnly Gly_C As Double = {CarbonMass * 9, HydrogenMass * 17, OxygenMass * 9, PhosphorusMass}.Sum()

    Private Shared ReadOnly Gly_O As Double = {CarbonMass * 8, HydrogenMass * 15, OxygenMass * 10, PhosphorusMass}.Sum()

    Private Shared ReadOnly CH2 As Double = {HydrogenMass * 2, CarbonMass}.Sum()

    Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()

    Public Sub New()
        spectrumGenerator = New SpectrumPeakGenerator()
    End Sub

    Public Sub New(spectrumGenerator As ISpectrumPeakGenerator)
        Me.spectrumGenerator = spectrumGenerator
    End Sub

    Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator

    Public Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
        If lipid.LipidClass = LbmClass.PI Then
            If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+NH4]+") OrElse Equals(adduct.AdductIonName, "[M+Na]+") Then
                Return True
            End If
        End If
        Return False
    End Function

    Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
        Dim nlMass = If(Equals(adduct.AdductIonName, "[M+Na]+"), 0.0, adduct.ConvertToMz(C6H13O9P) - ProtonMass)
        Dim spectrum = New List(Of SpectrumPeak)()
        spectrum.AddRange(GetPISpectrum(lipid, adduct))
        If lipid.Description.Has(LipidDescription.Chain) Then
            spectrum.AddRange(GetAcylLevelSpectrum(lipid, lipid.Chains.GetDeterminedChains(), adduct))
            lipid.Chains.ApplyToChain(1, Sub(chain) spectrum.AddRange(GetAcylPositionSpectrum(lipid, chain, adduct)))
            spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, lipid.Chains.GetTypedChains(Of AcylChain)(), adduct, nlMass:=nlMass))
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

    Private Function GetPISpectrum(lipid As ILipid, adduct As AdductIon) As SpectrumPeak()
        Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
        Dim spectrum = New List(Of SpectrumPeak) From {
New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
    .SpectrumComment = SpectrumComment.precursor
},
New SpectrumPeak(C6H13O9P + adductmass, 500.0R, "Header") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
},
New SpectrumPeak(Gly_C + adductmass, 250.0R, "Gly-C") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
},
New SpectrumPeak(Gly_O + adductmass, 200.0R, "Gly-O") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
}
}
        If Equals(adduct.AdductIonName, "[M+Na]+") Then
            spectrum.AddRange(New SpectrumPeak() {New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - C6H10O5), 400.0R, "Precursor -C6H10O5") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}})
        Else
            spectrum.AddRange(New SpectrumPeak() {New SpectrumPeak(lipid.Mass - C6H13O9P + ProtonMass, 500.0R, "Precursor -Header") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - C6H10O5), 100.0R, "Precursor -C6H10O5") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}})
        End If
        If Equals(adduct.AdductIonName, "[M+NH4]+") Then
            spectrum.Add(New SpectrumPeak(lipid.Mass + ProtonMass, 200.0R, "[M+H]+"))
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetAcylDoubleBondSpectrum(lipid As ILipid, acylChains As IEnumerable(Of AcylChain), adduct As AdductIon, Optional nlMass As Double = 0.0) As IEnumerable(Of SpectrumPeak)
        Return acylChains.SelectMany(Function(acylChain) spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acylChain, adduct, nlMass, 30.0R))
    End Function


    Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChains As IEnumerable(Of IChain), adduct As AdductIon) As IEnumerable(Of SpectrumPeak)
        Return acylChains.SelectMany(Function(acylChain) GetAcylLevelSpectrum(lipid, acylChain, adduct))
    End Function

    Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
        Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
        Dim lipidMass = lipid.Mass + adductmass
        Dim chainMass = acylChain.Mass - HydrogenMass
        Dim spectrum = New List(Of SpectrumPeak)()

        If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+NH4]+") Then
            spectrum.AddRange({New SpectrumPeak(lipidMass - chainMass, 80.0R, $"-{acylChain}") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass - H2O, 80.0R, $"-{acylChain}-H2O"), New SpectrumPeak(lipidMass - chainMass - C6H13O9P, 100.0R, $"-C6H13O9P -{acylChain}") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass - C6H13O9P - H2O, 80.0R, $"-C6H13O9P -{acylChain}-H2O")})
        ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
            spectrum.AddRange({New SpectrumPeak(lipidMass - chainMass, 100.0R, $"-{acylChain} Acyl") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass - H2O + HydrogenMass, 100.0R, $"-{acylChain} Acyl-O-") With {
.SpectrumComment = SpectrumComment.acylchain'new SpectrumPeak(adduct.ConvertToMz(lipidMass - chainMass - C6H10O5), 100d, $"-C6H10O5 -{acylChain}") { SpectrumComment = SpectrumComment.acylchain },
}})
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetAcylPositionSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
        Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
        Dim lipidMass = lipid.Mass + adductmass
        Dim chainMass = acylChain.Mass - HydrogenMass

        Return {New SpectrumPeak(lipidMass - chainMass - H2O + HydrogenMass - CH2, 50.0R, "-CH2(Sn1)") With {
.SpectrumComment = SpectrumComment.snposition'new SpectrumPeak(lipidMass - chainMass - C6H13O9P-H2O - CH2 , 50d, "-Header -CH2(Sn1)") { SpectrumComment = SpectrumComment.snposition },
}}
    End Function

End Class
