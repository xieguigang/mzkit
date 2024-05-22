#Region "Microsoft.VisualBasic::407d1bfef50a2d9ac20e43114c808c05, metadb\Lipidomics\SpectrumGenerator\HBMPSpectrumGenerator.vb"

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

    '   Total Lines: 168
    '    Code Lines: 132 (78.57%)
    ' Comment Lines: 13 (7.74%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 23 (13.69%)
    '     File Size: 9.71 KB


    ' Class HBMPSpectrumGenerator
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: CanGenerate, CreateReference, Generate, GetAcylDoubleBondSpectrum, GetAcylLevelSpectrum
    '               GetAcylPositionSpectrum, GetHBMPSpectrum, GetLysoAcylLevelSpectrum
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports BioNovoGene.BioDeep.MSEngine

Public Class HBMPSpectrumGenerator
    Implements ILipidSpectrumGenerator
    'HBMP explain rule -> HBMP 1 chain(sn1)/2 chain(sn2,sn3)
    'HBMP sn1_sn2_sn3 (follow the rules of alignment) -- MolecularSpeciesLevelChains
    'HBMP sn1/sn2_sn3 -- MolecularSpeciesLevelChains <- cannot generate now
    'HBMP sn1/sn2/sn3 (sn4= 0:0)  -- MolecularSpeciesLevelChains
    'HBMP sn1/sn4(or sn4/sn1)/sn2/sn3 (sn4= 0:0)  -- PositionLevelChains <- cannot generate now

    Private Shared ReadOnly C3H9O6P As Double = {CarbonMass * 3, HydrogenMass * 9, OxygenMass * 6, PhosphorusMass}.Sum()

    Private Shared ReadOnly C3H6O2 As Double = {CarbonMass * 3, HydrogenMass * 6, OxygenMass * 2}.Sum()

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
        If lipid.LipidClass = LbmClass.HBMP Then
            If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+NH4]+") Then
                Return True
            End If
        End If
        Return False
    End Function
    Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
        'var nlMass = adduct.AdductIonName == "[M+NH4]+" ? adduct.AdductIonAccurateMass + H2O : H2O;
        Dim spectrum = New List(Of SpectrumPeak)()
        spectrum.AddRange(GetHBMPSpectrum(lipid, adduct))
        ' GetChain(1) = lyso
        If lipid.Description.Has(LipidDescription.Chain) Then
            lipid.Chains.ApplyToChain(1, Sub(chain) spectrum.AddRange(GetLysoAcylLevelSpectrum(lipid, chain, adduct)))
            lipid.Chains.ApplyToChain(2, Sub(chain) spectrum.AddRange(GetAcylLevelSpectrum(lipid, chain, adduct)))
            lipid.Chains.ApplyToChain(3, Sub(chain) spectrum.AddRange(GetAcylLevelSpectrum(lipid, chain, adduct)))
            'lipid.Chains.ApplyToChain(1, chain => spectrum.AddRange(GetAcylPositionSpectrum(lipid, chain, adduct)));
            'lipid.Chains.ApplyToChain(2, chain => spectrum.AddRange(GetAcylPositionSpectrum(lipid, chain, adduct)));
            spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, lipid.Chains.GetTypedChains(Of AcylChain)(), adduct, nlMass:=0.0))
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
    Private Function GetHBMPSpectrum(lipid As ILipid, adduct As AdductIon) As SpectrumPeak()
        Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
        Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
        .SpectrumComment = SpectrumComment.precursor
    },
    New SpectrumPeak(lipid.Mass + adductmass - H2O, 100.0R, "Precursor -H2O") With {
        .SpectrumComment = SpectrumComment.metaboliteclass,
        .IsAbsolutelyRequiredFragmentForAnnotation = True
    }'new SpectrumPeak(C3H9O6P + adductmass, 50d, "C3H9O6P") { SpectrumComment = SpectrumComment.metaboliteclass },'new SpectrumPeak(C3H9O6P -H2O + adductmass, 50d, "C3H9O6P - H2O") { SpectrumComment = SpectrumComment.metaboliteclass },
}
        If Equals(adduct.AdductIonName, "[M+NH4]+") Then
            spectrum.Add(New SpectrumPeak(lipid.Mass + ProtonMass, 100.0R, "[M+H]+") With {
.SpectrumComment = SpectrumComment.metaboliteclass
})
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetAcylDoubleBondSpectrum(lipid As ILipid, acylChains As IEnumerable(Of AcylChain), adduct As AdductIon, Optional nlMass As Double = 0.0) As IEnumerable(Of SpectrumPeak)
        Dim spectrum = New List(Of SpectrumPeak)()
        Dim chains = acylChains.ToList()
        nlMass = chains(0).Mass + C3H9O6P - HydrogenMass + adduct.AdductIonAccurateMass - ProtonMass
        spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, chains(1), adduct, nlMass, 10.0R))
        spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, chains(2), adduct, nlMass, 10.0R))
        nlMass = chains(1).Mass + chains(2).Mass + C3H6O2 - HydrogenMass + adduct.AdductIonAccurateMass - ProtonMass
        spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, chains(0), adduct, nlMass, 10.0R))
        Return spectrum.ToArray()
    End Function

    'private IEnumerable<SpectrumPeak> GetAcylLevelSpectrum(ILipid lipid, IEnumerable<IChain> acylChains, AdductIon adduct)
    '{
    '    return acylChains.SelectMany(acylChain => GetAcylLevelSpectrum(lipid, acylChain, adduct));
    '}

    Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
        Dim lipidMass = lipid.Mass
        Dim chainMass = acylChain.Mass - HydrogenMass
        Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)

        Dim spectrum = New List(Of SpectrumPeak) From {    'new SpectrumPeak(chainMass + C3H6O2 + adductmass, 100d, $"{acylChain}+C3H4O2+H"),
New SpectrumPeak(chainMass + ProtonMass, 50.0R, $"{acylChain} Acyl+") With {
    .SpectrumComment = SpectrumComment.acylchain
},
New SpectrumPeak(lipidMass - chainMass - HydrogenMass + adductmass, 50.0R, $"-{acylChain}") With {
    .SpectrumComment = SpectrumComment.acylchain
},
New SpectrumPeak(lipidMass - chainMass - H2O + adductmass, 50.0R, $"-{acylChain}-H2O") With {
    .SpectrumComment = SpectrumComment.acylchain
},
New SpectrumPeak(lipidMass - chainMass - C3H9O6P + adductmass, 200.0R, $"-C3H9O6P -{acylChain}") With {
    .SpectrumComment = SpectrumComment.acylchain
} 'new SpectrumPeak(lipidMass - chainMass - C3H9O6P - H2O + adductmass, 50d, $"-C3H9O6P -{acylChain}-H2O") { SpectrumComment = SpectrumComment.acylchain },
}

        Return spectrum.ToArray()
    End Function

    Private Function GetLysoAcylLevelSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
        Dim lipidMass = lipid.Mass
        Dim chainMass = acylChain.Mass - HydrogenMass
        Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)

        Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(chainMass + C3H6O2 + adductmass, 100.0R, $"{acylChain}+C3H4O2+H") With {
        .SpectrumComment = SpectrumComment.acylchain
    },    'new SpectrumPeak(chainMass + C3H6O2 - H2O + adductmass, 100d, $"{acylChain} + C3H4O2 -H2O +H"){ SpectrumComment = SpectrumComment.acylchain },
    New SpectrumPeak(chainMass + ProtonMass, 50.0R, $"{acylChain} Acyl+") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(lipidMass - chainMass - HydrogenMass + adductmass, 50.0R, $"-{acylChain}") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(lipidMass - chainMass - H2O + adductmass, 50.0R, $"-{acylChain}-H2O") With {
        .SpectrumComment = SpectrumComment.acylchain
    } 'new SpectrumPeak(lipidMass - chainMass - C3H9O6P + adductmass, 200d, $"-C3H9O6P -{acylChain}") { SpectrumComment = SpectrumComment.acylchain }, 'new SpectrumPeak(lipidMass - chainMass - C3H9O6P - H2O + adductmass, 50d, $"-C3H9O6P -{acylChain}-H2O") { SpectrumComment = SpectrumComment.acylchain },
 }

        Return spectrum.ToArray()
    End Function


    Private Function GetAcylPositionSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
        Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
        Dim lipidMass = lipid.Mass + adductmass
        Dim chainMass = acylChain.Mass - HydrogenMass

        Dim spectrum = New List(Of SpectrumPeak) From {
New SpectrumPeak(lipidMass - chainMass - H2O - CH2, 50.0R, "-CH2(Sn1)") With {
    .SpectrumComment = SpectrumComment.snposition
}
}
        Return spectrum.ToArray()
    End Function


End Class
