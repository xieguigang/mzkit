#Region "Microsoft.VisualBasic::c8c16c183e0ab1f75355d0875ff2d9eb, metadb\Lipidomics\SpectrumGenerator\BMPEidSpectrumGenerator.vb"

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
    '    Code Lines: 142 (87.12%)
    ' Comment Lines: 1 (0.61%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 20 (12.27%)
    '     File Size: 9.08 KB


    ' Class BMPEidSpectrumGenerator
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: CanGenerate, CreateReference, EidSpecificSpectrum, Generate, GetAcylDoubleBondSpectrum
    '               (+2 Overloads) GetAcylLevelSpectrum, GetAcylPositionSpectrum, GetBMPSpectrum
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports BioNovoGene.BioDeep.MSEngine

Public Class BMPEidSpectrumGenerator
    Implements ILipidSpectrumGenerator

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
        If lipid.LipidClass = LbmClass.BMP Then
            If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+Na]+") OrElse Equals(adduct.AdductIonName, "[M+NH4]+") Then
                Return True
            End If
        End If
        Return False
    End Function

    Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
        Dim nlMass = If(Equals(adduct.AdductIonName, "[M+Na]+"), 0.0, If(Equals(adduct.AdductIonName, "[M+NH4]+"), adduct.AdductIonAccurateMass - ProtonMass + H2O * 2, H2O * 2))
        Dim spectrum = New List(Of SpectrumPeak)()
        spectrum.AddRange(GetBMPSpectrum(lipid, adduct))
        If lipid.Description.Has(LipidDescription.Chain) Then
            spectrum.AddRange(GetAcylLevelSpectrum(lipid, lipid.Chains.GetDeterminedChains(), adduct))
            lipid.Chains.ApplyToChain(1, Sub(chain) spectrum.AddRange(GetAcylPositionSpectrum(lipid, chain, adduct)))
            'spectrum.AddRange(GetAcylDoubleBondSpectrum(lipid, lipid.Chains.GetTypedChains<AcylChain>(), adduct, nlMass: nlMass));
            spectrum.AddRange(EidSpecificSpectrum(lipid, adduct, 0.0, 200.0R))
        End If
        spectrum = spectrum.GroupBy(Function(spec) spec, comparer).[Select](Function(specs) New SpectrumPeak(Enumerable.First(specs).mz, specs.Sum(Function(n) n.intensity), String.Join(", ", specs.[Select](Function(spec) spec.Annotation)), specs.Aggregate(SpectrumComment.none, Function(a, b) a Or b.SpectrumComment))).OrderBy(Function(peak) peak.mz).ToList()
        Return CreateReference(lipid, adduct, spectrum, molecule)
    End Function

    Private Function CreateReference(lipid As ILipid, adduct As AdductIon, spectrum As List(Of SpectrumPeak), molecule As IMoleculeProperty) As MoleculeMsReference
        Return New MoleculeMsReference With {
    .PrecursorMz = adduct.ConvertToMz(lipid.Mass),
    .IonMode = adduct.IonMode,
    .spectrum = spectrum,
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

    Private Function GetBMPSpectrum(lipid As ILipid, adduct As AdductIon) As SpectrumPeak()
        Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
        Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
        .SpectrumComment = SpectrumComment.precursor
    },
    New SpectrumPeak(C3H9O6P + adductmass, 200.0R, "C3H9O6P") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(lipid.Mass - C3H9O6P + adductmass, 100.0R, "Precursor -C3H9O6P") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
    New SpectrumPeak(C3H9O6P - H2O + adductmass, 200.0R, "C3H9O6P - H2O") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    }
}
        If Equals(adduct.AdductIonName, "[M+NH4]+") Then
            spectrum.Add(New SpectrumPeak(lipid.Mass + ProtonMass, 200.0R, "[M+H]+"))
        End If
        If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+NH4]+") Then
            spectrum.AddRange({New SpectrumPeak(lipid.Mass + adductmass - H2O, 100.0R, "Precursor -H2O"), New SpectrumPeak(lipid.Mass + adductmass - H2O * 2, 100.0R, "Precursor -2H2O")})
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetAcylDoubleBondSpectrum(lipid As ILipid, acylChains As IEnumerable(Of AcylChain), adduct As AdductIon, Optional nlMass As Double = 0.0) As IEnumerable(Of SpectrumPeak)
        Return acylChains.SelectMany(Function(acylChain) spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acylChain, adduct, nlMass, 10.0R))
    End Function

    Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChains As IEnumerable(Of IChain), adduct As AdductIon) As IEnumerable(Of SpectrumPeak)
        Return acylChains.SelectMany(Function(acylChain) GetAcylLevelSpectrum(lipid, acylChain, adduct))
    End Function

    Private Function GetAcylLevelSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
        Dim lipidMass = lipid.Mass
        Dim chainMass = acylChain.Mass - HydrogenMass
        Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)

        Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(lipidMass - chainMass + adductmass, 50.0R, $"-{acylChain}")
 }
        If Equals(adduct.AdductIonName, "[M+Na]+") Then
            spectrum.AddRange({New SpectrumPeak(lipidMass - chainMass - HydrogenMass - OxygenMass + adductmass, 50.0R, $"-{acylChain}-OH") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass - C3H6O2 + adductmass, 400.0R, $"-C3H6O2 -{acylChain}") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass - C3H6O2 - H2O + adductmass, 50.0R, $"-C3H6O2 -{acylChain}-H2O") With {
.SpectrumComment = SpectrumComment.acylchain
}})
        Else
            spectrum.AddRange({New SpectrumPeak(lipidMass - chainMass - H2O + adductmass, 50.0R, $"-{acylChain}-H2O") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass - C3H9O6P + adductmass, 400.0R, $"-C3H9O6P -{acylChain}") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipidMass - chainMass - C3H9O6P - H2O + adductmass, 50.0R, $"-C3H9O6P -{acylChain}-H2O") With {
.SpectrumComment = SpectrumComment.acylchain
}})
        End If

        Return spectrum.ToArray()
    End Function

    Private Function GetAcylPositionSpectrum(lipid As ILipid, acylChain As IChain, adduct As AdductIon) As SpectrumPeak()
        Dim adductmass = If(Equals(adduct.AdductIonName, "[M+NH4]+"), ProtonMass, adduct.AdductIonAccurateMass)
        Dim lipidMass = lipid.Mass + adductmass
        Dim chainMass = acylChain.Mass - HydrogenMass

        Dim spectrum = New List(Of SpectrumPeak) From {
    New SpectrumPeak(lipidMass - chainMass - H2O - CH2, 100.0R, "-CH2(Sn1)") With {
        .SpectrumComment = SpectrumComment.snposition
    }
}
        If Not Equals(adduct.AdductIonName, "[M+Na]+") Then
            spectrum.AddRange({New SpectrumPeak(lipidMass - chainMass - H2O * 2 - CH2, 50.0R, "-H2O -CH2(Sn1)") With {
.SpectrumComment = SpectrumComment.snposition
}})
        End If
        Return spectrum.ToArray()
    End Function

    Private Shared Function EidSpecificSpectrum(lipid As Lipid, adduct As AdductIon, nlMass As Double, intensity As Double) As SpectrumPeak()
        Dim spectrum = New List(Of SpectrumPeak)()
        If TypeOf lipid.Chains Is SeparatedChains Then
            For Each lossChain In lipid.Chains.GetDeterminedChains()
                nlMass = lossChain.Mass + C3H9O6P - HydrogenMass + adduct.AdductIonAccurateMass - ProtonMass
                Dim chains = lipid.Chains.GetDeterminedChains().Where(Function(c) Not c.Equals(lossChain)).ToList()
                For Each chain In chains
                    If chain.DoubleBond.Count = 0 OrElse chain.DoubleBond.UnDecidedCount > 0 Then Continue For
                    If chain.DoubleBond.Count < 3 Then
                        intensity = intensity * 0.5
                    End If
                    spectrum.AddRange(EidSpecificSpectrumGenerator.EidSpecificSpectrumGen(lipid, chain, adduct, nlMass, intensity))
                Next
            Next
        End If
        Return spectrum.ToArray()
    End Function
End Class
