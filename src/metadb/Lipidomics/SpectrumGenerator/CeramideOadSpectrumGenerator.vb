#Region "Microsoft.VisualBasic::e186e830f4fee7912271d09395d46e35, metadb\Lipidomics\SpectrumGenerator\CeramideOadSpectrumGenerator.vb"

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

'   Total Lines: 136
'    Code Lines: 97
' Comment Lines: 21
'   Blank Lines: 18
'     File Size: 6.17 KB


' Class CeramideOadSpectrumGenerator
' 
'     Constructor: (+2 Overloads) Sub New
'     Function: CanGenerate, CreateReference, Generate, GetAcylSpectrum, GetCerNsOadSpectrum
'               GetSphingoSpectrum
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports BioNovoGene.BioDeep.MSEngine

Public Class CeramideOadSpectrumGenerator
    Implements ILipidSpectrumGenerator
    Private Shared ReadOnly C5H14NO4P As Double = {CarbonMass * 5, HydrogenMass * 14, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()

    Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()

    Private Shared ReadOnly C2H2N As Double = {CarbonMass * 2, HydrogenMass * 2, NitrogenMass * 1}.Sum()

    Private Shared ReadOnly Electron As Double = 0.00054858026

    Private ReadOnly spectrumGenerator As IOadSpectrumPeakGenerator
    Public Sub New()
        spectrumGenerator = New OadSpectrumPeakGenerator()
    End Sub

    Public Sub New(spectrumGenerator As IOadSpectrumPeakGenerator)
        Me.spectrumGenerator = spectrumGenerator
    End Sub

    Public Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
        'adduct.AdductIonName == "[M+Na]+" ||
        'adduct.AdductIonName == "[M+HCOO]-" ||
        'adduct.AdductIonName == "[M+CH3COO]-"
        If Equals(adduct.AdductIonName, "[M+H]+") Then '||
            Return True
        End If
        Return False
    End Function

    Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
        Dim abundance = 40.0
        Dim nlMass = 0.0
        Dim spectrum = New List(Of SpectrumPeak)()
        spectrum.AddRange(GetCerNsOadSpectrum(lipid, adduct))
        '"OAD01",
        '"OAD02+O",
        '"OAD04",
        '"OAD09",
        '"OAD10",
        '"OAD11",
        '"OAD12",
        '"OAD13",
        '"OAD14",
        '"OAD15+O",
        '"OAD17",
        '"OAD12+O",
        '"OAD12+O+H",
        '"OAD12+O+2H",
        '"OAD01+H"
        '"SphOAD+H",
        '"SphOAD+2H",
        Dim oadId = New String() {"OAD02", "OAD03", "OAD05", "OAD06", "OAD07", "OAD08", "OAD15", "OAD16", "SphOAD", "SphOAD-CO"}

        Dim sphingo As SphingoChain = TryCast(lipid.Chains.GetChainByPosition(1), SphingoChain)

        If sphingo IsNot Nothing Then
            spectrum.AddRange(GetSphingoSpectrum(lipid, sphingo, adduct))
            spectrum.AddRange(spectrumGenerator.GetSphingoDoubleBondSpectrum(lipid, sphingo, adduct, nlMass, 30.0R, oadId))
        End If
        Dim acyl As AcylChain = TryCast(lipid.Chains.GetChainByPosition(2), AcylChain)

        If acyl IsNot Nothing Then
            'spectrum.AddRange(GetAcylSpectrum(lipid, acyl, adduct));
            spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acyl, adduct, nlMass, 30.0R, oadId))
        End If
        spectrum = spectrum.GroupBy(Function(spec) spec, comparer).[Select](Function(specs) New SpectrumPeak(Enumerable.First(specs).mz, specs.Sum(Function(n) n.intensity), String.Join(", ", specs.[Select](Function(spec) spec.Annotation)), specs.Aggregate(SpectrumComment.none, Function(a, b) a Or b.SpectrumComment))).OrderBy(Function(peak) peak.mz).ToList()
        Return CreateReference(lipid, adduct, spectrum, molecule)
    End Function

    Private Function GetCerNsOadSpectrum(lipid As Lipid, adduct As AdductIon) As SpectrumPeak()
        Dim spectrum = New List(Of SpectrumPeak)()

        If Equals(adduct.AdductIonName, "[M+H]+") Then
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - H2O, 500.0R, "Precursor -H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
        Else
            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}})
        End If
        Return spectrum.ToArray()
    End Function
    Private Function GetSphingoSpectrum(lipid As ILipid, sphingo As SphingoChain, adduct As AdductIon) As SpectrumPeak()
        Dim chainMass = sphingo.Mass + HydrogenMass
        Dim spectrum = New List(Of SpectrumPeak)()
        If Equals(adduct.AdductIonName, "[M+H]+") Then
            spectrum.AddRange({New SpectrumPeak(chainMass + ProtonMass - H2O, 30.0R, "[sph+H]+ -H2O") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + ProtonMass - H2O * 2, 50.0R, "[sph+H]+ -H2O*2") With {
.SpectrumComment = SpectrumComment.acylchain
}})
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetAcylSpectrum(lipid As ILipid, acyl As AcylChain, adduct As AdductIon) As SpectrumPeak()
        Dim chainMass = acyl.Mass + HydrogenMass
        Dim spectrum = New List(Of SpectrumPeak)() From {
    New SpectrumPeak(adduct.ConvertToMz(chainMass) + C2H2N, 200.0R, "[FAA+C2H+adduct]+") With {
        .SpectrumComment = SpectrumComment.acylchain
    },
    New SpectrumPeak(adduct.ConvertToMz(chainMass) + C5H14NO4P + C2H2N - HydrogenMass, 200.0R, "[FAA+C2H+Header+adduct]+") With {
        .SpectrumComment = SpectrumComment.acylchain
    }
}
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
