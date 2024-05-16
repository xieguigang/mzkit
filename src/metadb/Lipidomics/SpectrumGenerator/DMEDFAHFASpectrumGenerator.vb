#Region "Microsoft.VisualBasic::8733ca6054a43e7e2add45b4b8ef55c7, metadb\Lipidomics\SpectrumGenerator\DMEDFAHFASpectrumGenerator.vb"

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

    '   Total Lines: 134
    '    Code Lines: 116
    ' Comment Lines: 1
    '   Blank Lines: 17
    '     File Size: 7.09 KB


    ' Class DMEDFAHFASpectrumGenerator
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: CanGenerate, CreateReference, Generate, GetAcylDoubleBondSpectrum, GetAcylLevelSpectrum
    '               GetDMEDFAHFASpectrum, GetOxPositionSpectrum
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MSEngine
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

Public Class DMEDFAHFASpectrumGenerator
    Implements ILipidSpectrumGenerator

    Private Shared ReadOnly CH2 As Double = {HydrogenMass * 2, CarbonMass}.Sum()

    Private Shared ReadOnly C2NH7 As Double = {CarbonMass * 2, NitrogenMass * 1, HydrogenMass * 7}.Sum()

    Private Shared ReadOnly C4N2H10_O As Double = {CarbonMass * 4, NitrogenMass * 2, HydrogenMass * 10, -OxygenMass}.Sum()

    Private Shared ReadOnly C4N2H10 As Double = {CarbonMass * 4, NitrogenMass * 2, HydrogenMass * 10}.Sum()

    Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()

    Public Sub New()
        spectrumGenerator = New SpectrumPeakGenerator()
    End Sub

    Public Sub New(spectrumGenerator As ISpectrumPeakGenerator)
        Me.spectrumGenerator = spectrumGenerator
    End Sub

    Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator

    Public Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
        If lipid.LipidClass = LbmClass.DMEDFAHFA Then
            If Equals(adduct.AdductIonName, "[M+H]+") Then
                Return True
            End If
        End If
        Return False
    End Function

    Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
        Dim nlMass = 0.0
        Dim spectrum = New List(Of SpectrumPeak)()
        spectrum.AddRange(GetDMEDFAHFASpectrum(lipid, adduct))
        If lipid.Description.Has(LipidDescription.Chain) Then
            lipid.Chains.ApplyToChain(Of AcylChain)(2, Sub(acylChain) spectrum.AddRange(GetAcylLevelSpectrum(lipid, acylChain, adduct)))
            lipid.Chains.ApplyToChain(Of AcylChain)(2, Sub(acylChain) spectrum.AddRange(GetOxPositionSpectrum(lipid, acylChain, adduct)))
            spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, lipid.Chains.GetTypedChains(Of AcylChain)(), adduct, nlMass))
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

    Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChain As AcylChain, adduct As AdductIon) As SpectrumPeak()
        Dim adductmass = adduct.AdductIonAccurateMass
        Dim chainMass = acylChain.Mass - HydrogenMass + C4N2H10_O
        Dim spectrum = New List(Of SpectrumPeak)()
        spectrum.AddRange({New SpectrumPeak(chainMass + OxygenMass + ProtonMass, 50.0R, $"{acylChain}(+DMED)") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + ProtonMass, 700.0R, $"{acylChain}(+DMED) -H2O") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass - C2NH7 + ProtonMass, 700.0R, $"{acylChain}(+DMED) -H2O -C2NH7") With {
.SpectrumComment = SpectrumComment.acylchain,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}})
        Return spectrum.ToArray()
    End Function
    Private Function GetOxPositionSpectrum(lipid As ILipid, acylChain As AcylChain, adduct As AdductIon) As SpectrumPeak()
        Dim adductmass = adduct.AdductIonAccurateMass
        Dim lipidMass = lipid.Mass + adductmass
        Dim spectrum = New List(Of SpectrumPeak)()
        If acylChain.Oxidized.UnDecidedCount > 0 OrElse acylChain.Oxidized.Count > 1 Then
            Return spectrum.ToArray()
        End If
        Dim separatedChainMass = CH2 * (acylChain.Oxidized.Oxidises(0) - 2) - HydrogenMass * 2 * (acylChain.DoubleBond.Bonds.Where(Function(n) n.Position < acylChain.Oxidized.Oxidises(0)).Count())
        spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(C4N2H10 + HydrogenMass * 2 + 12 * 2 + OxygenMass * 2 + separatedChainMass), 300.0R, $"{acylChain} O position") With {
.SpectrumComment = SpectrumComment.snposition,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}})
        Return spectrum.ToArray()
    End Function
    Private Function GetDMEDFAHFASpectrum(lipid As ILipid, adduct As AdductIon) As SpectrumPeak()
        Dim spectrum = New List(Of SpectrumPeak) From {
New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor(DMED derv.)") With {
    .SpectrumComment = SpectrumComment.precursor
},
New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - C2NH7), 50.0R, "Precursor - C2NH7") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
}
}
        Return spectrum.ToArray()
    End Function

    Private Function GetAcylDoubleBondSpectrum(lipid As ILipid, acylChains As IEnumerable(Of AcylChain), adduct As AdductIon, Optional nlMass As Double = 0.0) As IEnumerable(Of SpectrumPeak)
        nlMass = 0.0
        Dim spectrum = New List(Of SpectrumPeak)()
        Dim acylChain = acylChains.ToList()
        Dim abundance = 25.0R
        spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acylChain(0), adduct, nlMass, abundance))

        ' Dehydroxy HFA chain spectrum 
        If acylChain(1).Oxidized.Oxidises.Count = 1 Then
            nlMass = acylChain(0).Mass + H2O * acylChain(1).Oxidized.Count - HydrogenMass
            Dim hfaOx = acylChain(1).Oxidized.Oxidises
            Dim HfaDb = acylChain(1).DoubleBond
            For Each ox In hfaOx
                If ox = acylChain(1).CarbonCount Then
                    HfaDb = HfaDb.Add(DoubleBondInfo.Create(ox - 1))
                Else
                    HfaDb = HfaDb.Add(DoubleBondInfo.Create(ox))
                End If
            Next
            Dim HfaNoDBChain = New AcylChain(acylChain(1).CarbonCount, HfaDb, New Oxidized(0))
            spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, HfaNoDBChain, adduct, nlMass, abundance))
        End If
        Return spectrum.ToArray()
    End Function

    Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()

End Class
