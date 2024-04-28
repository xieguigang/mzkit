#Region "Microsoft.VisualBasic::7b25f19f9922b2561853949380238116, G:/mzkit/src/metadb/Lipidomics//SpectrumGenerator/LPCOadSpectrumGenerator.vb"

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

    '   Total Lines: 115
    '    Code Lines: 89
    ' Comment Lines: 10
    '   Blank Lines: 16
    '     File Size: 5.39 KB


    ' Class LPCOadSpectrumGenerator
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: CanGenerate, CreateReference, Generate, GetLPCOadSpectrum
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports BioNovoGene.BioDeep.MSEngine


Public Class LPCOadSpectrumGenerator
    Implements ILipidSpectrumGenerator
    Private Shared ReadOnly C5H14NO4P As Double = {CarbonMass * 5, HydrogenMass * 14, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()
    Private Shared ReadOnly C5H14NO As Double = {CarbonMass * 5, HydrogenMass * 13, NitrogenMass, OxygenMass, ProtonMass}.Sum() 'PC 104.107

    Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()

    Private Shared ReadOnly CH3 As Double = {HydrogenMass * 3, CarbonMass}.Sum()
    Private Shared ReadOnly Electron As Double = 0.00054858026

    Private ReadOnly spectrumGenerator As IOadSpectrumPeakGenerator
    Public Sub New()
        spectrumGenerator = New OadSpectrumPeakGenerator()
    End Sub

    Public Sub New(spectrumGenerator As IOadSpectrumPeakGenerator)
        Me.spectrumGenerator = spectrumGenerator
    End Sub

    Public Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
        If Equals(adduct.AdductIonName, "[M+H]+") Then
            Return True
        End If
        Return False
    End Function

    Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
        Dim abundance = 40.0
        Dim nlMass = 0.0
        Dim spectrum = New List(Of SpectrumPeak)()
        spectrum.AddRange(GetLPCOadSpectrum(lipid, adduct))
        '"OAD05",
        '"OAD06",
        '"OAD07",
        '"OAD09",
        '"OAD10",
        '"OAD11",
        '"OAD12",
        '"OAD14",
        '"OAD15+O",
        '"OAD12+O+2H",
        Dim oadId = New String() {"OAD01", "OAD02", "OAD02+O", "OAD03", "OAD04", "OAD08", "OAD13", "OAD15", "OAD16", "OAD17", "OAD12+O", "OAD12+O+H", "OAD01+H"}

        Dim plChains As PositionLevelChains = TryCast(lipid.Chains, PositionLevelChains)

        If plChains IsNot Nothing Then
            For Each chain As AcylChain In lipid.Chains.GetDeterminedChains()
                spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, chain, adduct, nlMass, abundance, oadId))
            Next
        End If
        spectrum = spectrum.GroupBy(Function(spec) spec, comparer).[Select](Function(specs) New SpectrumPeak(Enumerable.First(specs).mz, specs.Sum(Function(n) n.Intensity), String.Join(", ", specs.[Select](Function(spec) spec.Annotation)), specs.Aggregate(SpectrumComment.none, Function(a, b) a Or b.SpectrumComment))).OrderBy(Function(peak) peak.mz).ToList()
        Return CreateReference(lipid, adduct, spectrum, molecule)
    End Function

    Private Function GetLPCOadSpectrum(lipid As Lipid, adduct As AdductIon) As SpectrumPeak()
        Dim spectrum = New List(Of SpectrumPeak)()
        Dim Chains As SeparatedChains = Nothing

        If Equals(adduct.AdductIonName, "[M+H]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}, New SpectrumPeak(adduct.ConvertToMz(C5H14NO4P), 500.0R, "Header") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - H2O), 100.0R, "Precursor - H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(C5H14NO, 50.0R, "C5H14NO") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
            Chains = TryCast(lipid.Chains, SeparatedChains)
            If Chains IsNot Nothing Then
                For Each chain As AcylChain In lipid.Chains.GetDeterminedChains()
                    If chain.CarbonCount <> 0 Then
                        spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - chain.Mass + HydrogenMass), 30.0R, $"-{chain}") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - chain.Mass + HydrogenMass * 2), 10.0R, $"-{chain}+H") With {
.SpectrumComment = SpectrumComment.acylchain
}})
                    End If
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

    Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()

End Class
