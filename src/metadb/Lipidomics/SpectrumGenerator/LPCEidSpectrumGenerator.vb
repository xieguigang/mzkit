#Region "Microsoft.VisualBasic::ce16f62f05f92e6e2fe7d869bc851109, metadb\Lipidomics\SpectrumGenerator\LPCEidSpectrumGenerator.vb"

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

    '   Total Lines: 147
    '    Code Lines: 120 (81.63%)
    ' Comment Lines: 3 (2.04%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 24 (16.33%)
    '     File Size: 7.65 KB


    ' Class LPCEidSpectrumGenerator
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: CanGenerate, CreateReference, EidSpecificSpectrum, Generate, GetAcylDoubleBondSpectrum
    '               (+2 Overloads) GetAcylLevelSpectrum, GetAcylPositionSpectrum, GetLPCSpectrum
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports BioNovoGene.BioDeep.MSEngine

Public Class LPCEidSpectrumGenerator
    Implements ILipidSpectrumGenerator
    Private Shared ReadOnly C5H14NO4P As Double = {CarbonMass * 5, HydrogenMass * 14, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()

    Private Shared ReadOnly C3H9N As Double = {CarbonMass * 3, HydrogenMass * 9, NitrogenMass}.Sum()

    Private Shared ReadOnly C5H13NO As Double = {CarbonMass * 5, HydrogenMass * 13, NitrogenMass, OxygenMass}.Sum()

    Private Shared ReadOnly C5H11N As Double = {CarbonMass * 5, HydrogenMass * 11, NitrogenMass}.Sum()

    Private Shared ReadOnly Gly_C As Double = {CarbonMass * 8, HydrogenMass * 18, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()

    Private Shared ReadOnly Gly_O As Double = {CarbonMass * 7, HydrogenMass * 16, NitrogenMass, OxygenMass * 5, PhosphorusMass}.Sum()

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
        If lipid.LipidClass = LbmClass.LPC Then
            If Equals(adduct.AdductIonName, "[M+H]+") Then
                Return True
            End If
        End If
        Return False
    End Function

    Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
        Dim spectrum = New List(Of SpectrumPeak)()
        spectrum.AddRange(GetLPCSpectrum(lipid, adduct))

        If lipid.Description.Has(LipidDescription.Chain) Then
            spectrum.AddRange(GetAcylLevelSpectrum(lipid, lipid.Chains.GetDeterminedChains(), adduct))
            lipid.Chains.ApplyToChain(1, Sub(chain) spectrum.AddRange(GetAcylPositionSpectrum(lipid, chain, adduct)))
            spectrum.AddRange(EidSpecificSpectrum(lipid, adduct, 0R, 250.0R))
            spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, lipid.Chains.GetTypedChains(Of AcylChain)(), adduct))
        End If
        'spectrum.AddRange(GetAcylLevelSpectrum(lipid, lipid.Chains));
        'spectrum.AddRange(GetAcylPositionSpectrum(lipid, lipid.Chains[0]));
        'spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, lipid.Chains.OfType<AcylChain>()));
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

    Private Function GetLPCSpectrum(lipid As ILipid, adduct As AdductIon) As SpectrumPeak()
        Dim spectrum = New List(Of SpectrumPeak) From {
New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
    .SpectrumComment = SpectrumComment.precursor
},
New SpectrumPeak(adduct.ConvertToMz(C5H14NO4P), 250.0R, "Header") With {
    .SpectrumComment = SpectrumComment.metaboliteclass,
    .IsAbsolutelyRequiredFragmentForAnnotation = True
},
New SpectrumPeak(adduct.ConvertToMz(Gly_C), 100.0R, "Gly-C") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
},
New SpectrumPeak(adduct.ConvertToMz(Gly_O), 100.0R, "Gly-O") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
}
}
        If Equals(adduct.AdductIonName, "[M+H]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - H2O), 200.0R, "C5H14NO+") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(adduct.ConvertToMz(C5H13NO), 200.0R, "C5H14NO+") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(adduct.ConvertToMz(C5H14NO4P - H2O), 50.0R, "Header - H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChains As IEnumerable(Of IChain), adduct As AdductIon) As IEnumerable(Of SpectrumPeak)
        Return acylChains.SelectMany(Function(acylChain) GetAcylLevelSpectrum(lipid, acylChain, adduct))
    End Function

    Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
        Dim chainMass = acylChain.Mass - HydrogenMass
        Dim spectrum = New List(Of SpectrumPeak)()
        If chainMass <> 0.0 Then
            spectrum.AddRange({New SpectrumPeak(chainMass + ProtonMass, 100.0R, $"{acylChain} acyl") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - chainMass), 100.0R, $"-{acylChain}") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - chainMass - H2O), 100.0R, $"-{acylChain}-O") With {
.SpectrumComment = SpectrumComment.acylchain
}})
        End If
        Return spectrum.ToArray()
    End Function
    Private Shared Function EidSpecificSpectrum(lipid As Lipid, adduct As AdductIon, nlMass As Double, intensity As Double) As SpectrumPeak()
        Dim spectrum = New List(Of SpectrumPeak)()
        Dim chains As SeparatedChains = TryCast(lipid.Chains, SeparatedChains)

        If chains IsNot Nothing Then
            For Each chain In lipid.Chains.GetDeterminedChains()
                If chain.DoubleBond.Count = 0 OrElse chain.DoubleBond.UnDecidedCount > 0 Then Continue For
                spectrum.AddRange(EidSpecificSpectrumGenerator.EidSpecificSpectrumGen(lipid, chain, adduct, nlMass, intensity))
            Next
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetAcylPositionSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
        Return {New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - acylChain.Mass - OxygenMass - CH2 - HydrogenMass), 100.0R, "-CH2(Sn1)") With {
.SpectrumComment = SpectrumComment.snposition
}}
    End Function

    Private Function GetAcylDoubleBondSpectrum(lipid As ILipid, acylChains As IEnumerable(Of AcylChain), adduct As AdductIon) As IEnumerable(Of SpectrumPeak)
        Return acylChains.SelectMany(Function(acylChain) spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acylChain, adduct, 0R, 20.0R))
    End Function


End Class
