﻿#Region "Microsoft.VisualBasic::2534ed7d3206552ff345103a485a9708, metadb\Lipidomics\SpectrumGenerator\PGOadSpectrumGenerator.vb"

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

    '   Total Lines: 135
    '    Code Lines: 97 (71.85%)
    ' Comment Lines: 26 (19.26%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 12 (8.89%)
    '     File Size: 6.56 KB


    ' Class PGOadSpectrumGenerator
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: CanGenerate, CreateReference, Generate, GetPGOadSpectrum
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports BioNovoGene.BioDeep.MSEngine

Public Class PGOadSpectrumGenerator
    Implements ILipidSpectrumGenerator
    Private Shared ReadOnly C3H9O6P As Double = {CarbonMass * 3, HydrogenMass * 9, OxygenMass * 6, PhosphorusMass}.Sum() ' PG Header
    Private Shared ReadOnly NH3 As Double = {HydrogenMass * 3, NitrogenMass}.Sum()
    Private Shared ReadOnly C3H6O5P As Double = {CarbonMass * 3, HydrogenMass * 6, OxygenMass * 5, PhosphorusMass}.Sum()
    Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()
    Private Shared ReadOnly Electron As Double = 0.00054858026

    Private ReadOnly spectrumGenerator As IOadSpectrumPeakGenerator
    Public Sub New()
        spectrumGenerator = New OadSpectrumPeakGenerator()
    End Sub

    Public Sub New(spectrumGenerator As IOadSpectrumPeakGenerator)
        Me.spectrumGenerator = spectrumGenerator
    End Sub

    Public Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
        If Equals(adduct.AdductIonName, "[M+NH4]+") OrElse Equals(adduct.AdductIonName, "[M-H]-") Then
            Return True
        End If
        Return False
    End Function

    Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
        Dim abundance = 30
        Dim nlMass = If(adduct.IonMode = IonModes.Positive, C3H9O6P + NH3, 0.0)
        Dim spectrum = New List(Of SpectrumPeak)()
        spectrum.AddRange(GetPGOadSpectrum(lipid, adduct, nlMass))
        '"OAD02+O",
        '"OAD05",
        '"OAD06",
        '"OAD07",
        '"OAD08",
        '"OAD09",
        '"OAD10",
        '"OAD11",
        '"OAD12",
        '"OAD13",
        '"OAD15+O",
        '"OAD12+O+2H",
        '"OAD02+O",
        '"OAD05",
        '"OAD06",
        '"OAD07",
        '"OAD08",
        '"OAD09",
        '"OAD10",
        '"OAD11",
        '"OAD12",
        '"OAD13",
        '"OAD15+O",
        '"OAD16",
        '"OAD17",
        '"OAD01+H"
        Dim oadId = If(adduct.IonMode = IonModes.Positive, New String() {"OAD01", "OAD02", "OAD03", "OAD04", "OAD14", "OAD15", "OAD16", "OAD17", "OAD12+O", "OAD12+O+H", "OAD01+H"}, New String() {"OAD01", "OAD02", "OAD03", "OAD04", "OAD14", "OAD15", "OAD12+O", "OAD12+O+H", "OAD12+O+2H"})

        Dim plChains As PositionLevelChains = TryCast(lipid.Chains, PositionLevelChains)

        If plChains IsNot Nothing Then
            For Each chain As AcylChain In lipid.Chains.GetDeterminedChains()
                spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, chain, adduct, nlMass, abundance, oadId))
            Next
        End If
        spectrum = spectrum.GroupBy(Function(spec) spec, comparer).[Select](Function(specs) New SpectrumPeak(Enumerable.First(specs).mz, specs.Sum(Function(n) n.intensity), String.Join(", ", specs.[Select](Function(spec) spec.Annotation)), specs.Aggregate(SpectrumComment.none, Function(a, b) a Or b.SpectrumComment))).OrderBy(Function(peak) peak.mz).ToList()
        Return CreateReference(lipid, adduct, spectrum, molecule)
    End Function

    Private Function GetPGOadSpectrum(lipid As Lipid, adduct As AdductIon, nlMass As Double) As SpectrumPeak()
        Dim spectrum = New List(Of SpectrumPeak)()
        Dim Chains As SeparatedChains = Nothing

        If Equals(adduct.AdductIonName, "[M+NH4]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 100.0R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}, New SpectrumPeak(lipid.Mass - C3H9O6P + ProtonMass, 999.0R, "Precursor -C3H9O6P") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}})
            Chains = TryCast(lipid.Chains, SeparatedChains)
            If Chains IsNot Nothing Then
                For Each chain As AcylChain In lipid.Chains.GetDeterminedChains()
                    spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - nlMass - chain.Mass + HydrogenMass), 50.0R, $"-{chain}") With {
.SpectrumComment = SpectrumComment.acylchain'new SpectrumPeak(adduct.ConvertToMz(chain.Mass - MassDiffDictionary.HydrogenMass), 20d, $"{chain} Acyl+") { SpectrumComment = SpectrumComment.acylchain },'new SpectrumPeak(adduct.ConvertToMz(chain.Mass ), 5d, $"{chain} Acyl+ +H") { SpectrumComment = SpectrumComment.acylchain },
}})
                Next
            End If
        ElseIf Equals(adduct.AdductIonName, "[M-H]-") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}, New SpectrumPeak(C3H6O5P + Electron, 30.0R, "Header-") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
            Chains = TryCast(lipid.Chains, SeparatedChains)
            If Chains IsNot Nothing Then
                For Each chain As AcylChain In lipid.Chains.GetDeterminedChains()
                    spectrum.AddRange({New SpectrumPeak(chain.Mass + OxygenMass + Electron, 50.0R, $"{chain} FA") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(lipid.Mass - chain.Mass + Electron, 20.0R, $"-{chain}") With {
.SpectrumComment = SpectrumComment.acylchain
}})
                Next
            End If
        Else
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}})
        End If
        Return spectrum.ToArray()
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

End Class
