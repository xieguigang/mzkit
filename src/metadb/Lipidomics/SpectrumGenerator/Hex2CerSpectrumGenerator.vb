#Region "Microsoft.VisualBasic::8fdecc935d8fc7d44d91493bd85948c5, metadb\Lipidomics\SpectrumGenerator\Hex2CerSpectrumGenerator.vb"

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

    '   Total Lines: 131
    '    Code Lines: 118
    ' Comment Lines: 3
    '   Blank Lines: 10
    '     File Size: 7.51 KB


    ' Class Hex2CerSpectrumGenerator
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: CanGenerate, CreateReference, Generate, GetAcylSpectrum, GetHex2CerSpectrum
    '               GetSphingoSpectrum
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports BioNovoGene.BioDeep.MSEngine

Public Class Hex2CerSpectrumGenerator
    Implements ILipidSpectrumGenerator
    Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()
    Private Shared ReadOnly C2H5NO As Double = {CarbonMass * 2, HydrogenMass * 5, NitrogenMass * 1, OxygenMass * 1}.Sum()
    Private Shared ReadOnly CH4O2 As Double = {CarbonMass * 1, HydrogenMass * 4, OxygenMass * 2}.Sum()
    Private Shared ReadOnly C2H3NO As Double = {CarbonMass * 2, HydrogenMass * 3, NitrogenMass * 1, OxygenMass * 1}.Sum()
    Private Shared ReadOnly NH As Double = {NitrogenMass * 1, HydrogenMass * 1}.Sum()
    Private Shared ReadOnly C6H10O5 As Double = {CarbonMass * 6, HydrogenMass * 10, OxygenMass * 5}.Sum()
    Private Shared ReadOnly C5H10O4 As Double = {CarbonMass * 5, HydrogenMass * 10, OxygenMass * 4}.Sum()
    Public Sub New()
        spectrumGenerator = New SpectrumPeakGenerator()
    End Sub
    Public Sub New(spectrumGenerator As ISpectrumPeakGenerator)
        Me.spectrumGenerator = spectrumGenerator
    End Sub

    Private ReadOnly spectrumGenerator As ISpectrumPeakGenerator

    Public Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
        If lipid.LipidClass = LbmClass.Hex2Cer Then
            If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+Na]+") Then
                Return True
            End If
        End If
        Return False
    End Function

    Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
        Dim spectrum = New List(Of SpectrumPeak)()
        Dim nlmass = If(Equals(adduct.AdductIonName, "[M+H]+"), H2O * 2 + C6H10O5 * 2, H2O + C6H10O5 * 2)
        spectrum.AddRange(GetHex2CerSpectrum(lipid, adduct))
        Dim plChains As PositionLevelChains = TryCast(lipid.Chains, PositionLevelChains)
        Dim sphingo As SphingoChain = Nothing
        Dim acyl As AcylChain = Nothing

        If plChains IsNot Nothing Then
            sphingo = TryCast(lipid.Chains.GetChainByPosition(1), SphingoChain)
            If sphingo IsNot Nothing Then
                spectrum.AddRange(GetSphingoSpectrum(lipid, sphingo, adduct))
                'spectrum.AddRange(spectrumGenerator.GetSphingoDoubleBondSpectrum(lipid, sphingo, adduct, nlmass, 30d));
            End If
            acyl = TryCast(lipid.Chains.GetChainByPosition(2), AcylChain)
            If acyl IsNot Nothing Then
                spectrum.AddRange(GetAcylSpectrum(lipid, acyl, adduct))
                'spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acyl, adduct, nlmass, 30d));
            End If
        End If
        spectrum = spectrum.GroupBy(Function(spec) spec, comparer).[Select](Function(specs) New SpectrumPeak(Enumerable.First(specs).mz, specs.Sum(Function(n) n.Intensity), String.Join(", ", specs.[Select](Function(spec) spec.Annotation)), specs.Aggregate(SpectrumComment.none, Function(a, b) a Or b.SpectrumComment))).OrderBy(Function(peak) peak.mz).ToList()
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

    Private Function GetHex2CerSpectrum(lipid As ILipid, adduct As AdductIon) As SpectrumPeak()
        Dim spectrum = New List(Of SpectrumPeak) From {
New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
    .SpectrumComment = SpectrumComment.precursor
},
        New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - H2O, 200.0R, "Precursor-H2O") With {
    .SpectrumComment = SpectrumComment.metaboliteclass
},            'new SpectrumPeak(adduct.ConvertToMz(lipid.Mass)-C6H10O5, 300d, "Precursor-Hex") { SpectrumComment = SpectrumComment.metaboliteclass },            'new SpectrumPeak(adduct.ConvertToMz(lipid.Mass)-C6H10O5-H2O, 200d, "Precursor-Hex-H2O") { SpectrumComment = SpectrumComment.metaboliteclass },            'new SpectrumPeak(adduct.ConvertToMz(lipid.Mass)-C6H10O5-H2O*2, 200d, "Precursor-Hex-2H2O") { SpectrumComment = SpectrumComment.metaboliteclass },
            New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - C6H10O5 * 2, 100.0R, "Precursor-2Hex") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
            New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - C6H10O5 * 2 - H2O, 400.0R, "Precursor-2Hex-H2O") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    },
            New SpectrumPeak(adduct.ConvertToMz(lipid.Mass) - C6H10O5 * 2 - H2O * 2, 100.0R, "Precursor-2Hex-2H2O") With {
        .SpectrumComment = SpectrumComment.metaboliteclass
    }
}
        Return spectrum.ToArray()
    End Function

    Private Function GetSphingoSpectrum(lipid As ILipid, sphingo As SphingoChain, adduct As AdductIon) As SpectrumPeak()
        Dim chainMass = sphingo.Mass + HydrogenMass
        Dim spectrum = New List(Of SpectrumPeak)()
        'if (adduct.AdductIonName != "[M+Na]+")
        If True Then
            spectrum.AddRange({New SpectrumPeak(chainMass + ProtonMass - H2O, 200.0R, "[sph+H]+ -H2O") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + ProtonMass - H2O * 2, 500.0R, "[sph+H]+ -2H2O") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + ProtonMass - CH4O2, 150.0R, "[sph+H]+ -CH4O2") With {
.SpectrumComment = SpectrumComment.acylchain
}})
        End If
        Return spectrum.ToArray()
    End Function

    Private Function GetAcylSpectrum(lipid As ILipid, acyl As AcylChain, adduct As AdductIon) As SpectrumPeak()
        Dim chainMass = acyl.Mass + HydrogenMass
        Dim spectrum = New List(Of SpectrumPeak)() From {
             New SpectrumPeak(chainMass + ProtonMass + C2H3NO - HydrogenMass - OxygenMass, 200.0R, "[FAA+C2H+H]+") With {
    .SpectrumComment = SpectrumComment.acylchain
},
             New SpectrumPeak(adduct.ConvertToMz(chainMass) + C2H3NO + HydrogenMass + C6H10O5 * 2, 150.0R, "[FAA+C2H4O+2Hex+adduct]+") With {
    .SpectrumComment = SpectrumComment.acylchain
}
}
        If Equals(adduct.AdductIonName, "[M+H]+") Then
            spectrum.AddRange({New SpectrumPeak(chainMass + ProtonMass + NH, 200.0R, "[FAA+H]+") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(chainMass + ProtonMass + C2H3NO, 150.0R, "[FAA+C2H2O+H]+") With {
.SpectrumComment = SpectrumComment.acylchain
}})
        End If
        Return spectrum.ToArray()
    End Function

    Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()

End Class
