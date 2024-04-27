#Region "Microsoft.VisualBasic::8c685bf2ce0a81ec0a823030ff451f03, G:/mzkit/src/metadb/Lipidomics//SpectrumGenerator/TGEidSpectrumGenerator.vb"

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

    '   Total Lines: 196
    '    Code Lines: 176
    ' Comment Lines: 0
    '   Blank Lines: 20
    '     File Size: 10.86 KB


    ' Class TGEidSpectrumGenerator
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: CanGenerate, CreateReference, EidSpecificSpectrum, Generate, GetAcylDoubleBondSpectrum
    '               (+2 Overloads) GetAcylLevelSpectrum, GetAcylPositionSpectrum, GetTGSpectrum
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MSEngine
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

Public Class TGEidSpectrumGenerator : Implements ILipidSpectrumGenerator

    Private Shared ReadOnly C2H3O As Double = {CarbonMass * 2, HydrogenMass * 3, OxygenMass}.Sum()

    Private Shared ReadOnly C3H5O2 As Double = {CarbonMass * 3, HydrogenMass * 5, OxygenMass * 2}.Sum()

    Private Shared ReadOnly CH2 As Double = {HydrogenMass * 2, CarbonMass}.Sum()

    Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()

    Private Shared ReadOnly Electron As Double = 0.00054858026

    Public Sub New()
        spectrumGenerator = New SpectrumPeakGenerator()
    End Sub

    Public Sub New(spectrumGenerator As ISpectrumPeakGenerator)
        Me.spectrumGenerator = spectrumGenerator
    End Sub

    Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator

    Public Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
        If lipid.LipidClass = LbmClass.TG Then
            If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+NH4]+") Then
                Return True
            End If
        End If
        Return False
    End Function

    Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
        Dim spectrum = New List(Of SpectrumPeak)()
        spectrum.AddRange(GetTGSpectrum(lipid, adduct))
        Dim sn2 As IChain = Nothing
        If lipid.Description.Has(LipidDescription.Chain) Then
            Dim chains = lipid.Chains.GetDeterminedChains().ToList()
            sn2 = TryCast(lipid.Chains.GetChainByPosition(2), IChain)
            If sn2 IsNot Nothing Then
                spectrum.AddRange(GetAcylPositionSpectrum(lipid, sn2, adduct))
                chains.Remove(sn2)
            End If
            spectrum.AddRange(GetAcylLevelSpectrum(lipid, chains, adduct))
            spectrum.AddRange(EidSpecificSpectrum(lipid, adduct, 0R, 200.0R))
            spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, lipid.Chains.GetTypedChains(Of AcylChain)(), adduct))
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

    Private Function GetTGSpectrum(lipid As ILipid, adduct As AdductIon) As SpectrumPeak()
        Dim spectrum = New List(Of SpectrumPeak) From {
New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
    .SpectrumComment = SpectrumComment.precursor
},
New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) / 2, 150.0R, "[Precursor]2+") With {
    .SpectrumComment = SpectrumComment.precursor
}
}
        If Equals(adduct.AdductIonName, "[M+NH4]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - H2O, 150.0R, "Precursor-H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(lipid.Mass + ProtonMass, 150.0R, "[M+H]+") With {
.SpectrumComment = SpectrumComment.metaboliteclass'new SpectrumPeak(lipid.Mass + MassDiffDictionary.ProtonMass-H2O, 150d, "[M+H]+ -H2O") { SpectrumComment = SpectrumComment.metaboliteclass },
}})
        ElseIf Equals(adduct.AdductIonName, "[M+H]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - H2O, 100.0R, "Precursor-H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
        End If

        Return spectrum.ToArray()
    End Function

    Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChains As IEnumerable(Of IChain), adduct As AdductIon) As IEnumerable(Of SpectrumPeak)
        Return acylChains.SelectMany(Function(acylChain) GetAcylLevelSpectrum(lipid, acylChain, adduct))
    End Function

    Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
        Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
        Dim lipidMass = lipid.Mass + adductmass
        Dim chainMass = acylChain.Mass - HydrogenMass
        Dim chainMass2 = acylChain.Mass + adductmass
        Dim spectrum = New List(Of SpectrumPeak)()
        If Equals(adduct.AdductIonName, "[M+Na]+") Then
            spectrum.AddRange({New SpectrumPeak(chainMass2 + C2H3O + OxygenMass, 100.0R, $"{acylChain}+C2H3O2") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass2 + C2H3O + CH2, 100.0R, $"{acylChain}+C3H5O") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(acylChain.Mass + Electron, 100.0R, $"{acylChain}+") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass - HydrogenMass * 2, 50.0R, $"-{acylChain}") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass - H2O, 200.0R, $"-{acylChain}-O") With {
.SpectrumComment = SpectrumComment.acylchain
}})
        Else
            spectrum.AddRange({New SpectrumPeak(acylChain.Mass + Electron, 100.0R, $"{acylChain}+") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass2 + C3H5O2, 100.0R, $"{acylChain}+C3H5O2") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass2 + C3H5O2 - H2O, 100.0R, $"{acylChain}+C3H3O") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass, 50.0R, $"-{acylChain}") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass - H2O, 200.0R, $"-{acylChain}-O") With {
.SpectrumComment = SpectrumComment.acylchain
}})
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetAcylPositionSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
        Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
        Dim chainMass = acylChain.Mass + adductmass
        Dim lipidMass = lipid.Mass + adductmass
        Dim spectrum = New List(Of SpectrumPeak)()
        If Equals(adduct.AdductIonName, "[M+Na]+") Then
            spectrum.AddRange({New SpectrumPeak(chainMass + C2H3O, 100.0R, "Sn2 diagnostics") With {
.SpectrumComment = SpectrumComment.snposition,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}, New SpectrumPeak(acylChain.Mass + Electron, 100.0R, $"{acylChain}+ Sn2") With {
.SpectrumComment = SpectrumComment.acylchain'new SpectrumPeak(lipidMass - chainMass + MassDiffDictionary.HydrogenMass*2, 50d, $"-{acylChain}") { SpectrumComment = SpectrumComment.acylchain },
}, New SpectrumPeak(lipidMass - chainMass - OxygenMass, 200.0R, $"-{acylChain}-O Sn2") With {
.SpectrumComment = SpectrumComment.acylchain
}})
        Else
            spectrum.AddRange({New SpectrumPeak(chainMass + C2H3O, 100.0R, "Sn2 diagnostics") With {
.SpectrumComment = SpectrumComment.snposition
}, New SpectrumPeak(acylChain.Mass + Electron, 100.0R, $"{acylChain}+ Sn2") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + C3H5O2, 100.0R, $"{acylChain}+C3H5O2 Sn2") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + C3H5O2 - H2O, 100.0R, $"{acylChain}+C3H3O Sn2") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass + HydrogenMass * 2, 50.0R, $"-{acylChain} Sn2") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass - OxygenMass, 200.0R, $"-{acylChain}-O Sn2") With {
.SpectrumComment = SpectrumComment.acylchain
}})
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetAcylDoubleBondSpectrum(lipid As ILipid, acylChains As IEnumerable(Of AcylChain), adduct As AdductIon, Optional nlMass As Double = 0.0) As IEnumerable(Of SpectrumPeak)
        Dim spectrum = New List(Of SpectrumPeak)()
        For Each lossChain In acylChains
            nlMass = lossChain.Mass + OxygenMass + adduct.AdductIonAccurateMass - ProtonMass
            Dim chains = acylChains.Where(Function(c) Not c.Equals(lossChain)).ToList()
            spectrum.AddRange(chains.SelectMany(Function(acylChain) spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acylChain, adduct, nlMass, 5.0R)))
        Next
        Return spectrum.ToArray()
    End Function
    Private Shared Function EidSpecificSpectrum(lipid As Lipid, adduct As AdductIon, nlMass As Double, intensity As Double) As SpectrumPeak()
        Dim spectrum = New List(Of SpectrumPeak)()
        Dim acylChains As SeparatedChains = TryCast(lipid.Chains, SeparatedChains)

        If acylChains IsNot Nothing Then
            For Each lossChain In lipid.Chains.GetDeterminedChains()
                nlMass = lossChain.Mass + OxygenMass + adduct.AdductIonAccurateMass - ProtonMass
                Dim chains = lipid.Chains.GetDeterminedChains().Where(Function(c) Not c.Equals(lossChain)).ToList()
                For Each chain In chains
                    If chain.DoubleBond.Count = 0 OrElse chain.DoubleBond.UnDecidedCount > 0 Then Continue For
                    If chain.DoubleBond.Count <= 3 Then
                        intensity = intensity * 0.5
                    End If
                    spectrum.AddRange(EidSpecificSpectrumGenerator.EidSpecificSpectrumGen(lipid, chain, adduct, nlMass, intensity))
                Next
            Next
        End If
        Return spectrum.ToArray()
    End Function

    Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()
End Class

