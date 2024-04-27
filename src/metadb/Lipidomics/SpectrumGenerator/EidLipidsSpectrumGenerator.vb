#Region "Microsoft.VisualBasic::2b986cd92724cebca4f638b35f052f3e, G:/mzkit/src/metadb/Lipidomics//SpectrumGenerator/EidLipidsSpectrumGenerator.vb"

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

    '   Total Lines: 89
    '    Code Lines: 75
    ' Comment Lines: 0
    '   Blank Lines: 14
    '     File Size: 4.94 KB


    ' Class EidLipidSpectrumGenerator
    ' 
    '     Function: doublebondPositions, EidSpecificSpectrum, GetClassEidFragmentSpectrum
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

Public Class EidLipidSpectrumGenerator
    Public Function GetClassEidFragmentSpectrum(lipid As Lipid, adduct As AdductIon) As List(Of SpectrumPeak)
        Dim spectrum = New List(Of SpectrumPeak)()

        Select Case lipid.LipidClass

            Case LbmClass.TG
                Dim TGSpectrumGenerator = New TGSpectrumGenerator()
                spectrum.AddRange(TGSpectrumGenerator.Generate(lipid, adduct).Spectrum)
            Case LbmClass.DG
                Dim DGSpectrumGenerator = New DGSpectrumGenerator()
                spectrum.AddRange(DGSpectrumGenerator.Generate(lipid, adduct).Spectrum)
            Case LbmClass.MG
                Dim MGSpectrumGenerator = New MGSpectrumGenerator()
                spectrum.AddRange(MGSpectrumGenerator.Generate(lipid, adduct).Spectrum)
            Case LbmClass.BMP
                Dim BMPSpectrumGenerator = New BMPSpectrumGenerator()
                spectrum.AddRange(BMPSpectrumGenerator.Generate(lipid, adduct).Spectrum)
            Case LbmClass.HBMP
                Dim HBMPSpectrumGenerator = New HBMPSpectrumGenerator()
                spectrum.AddRange(HBMPSpectrumGenerator.Generate(lipid, adduct).Spectrum)
            Case LbmClass.CL
                Dim CLSpectrumGenerator = New CLEidSpectrumGenerator() ' Eid 
                spectrum.AddRange(CLSpectrumGenerator.Generate(lipid, adduct).Spectrum)
            Case LbmClass.Cer_NS
                Dim CeramideSpectrumGenerator = New CeramideSpectrumGenerator()
                spectrum.AddRange(CeramideSpectrumGenerator.Generate(lipid, adduct).Spectrum)
            Case LbmClass.HexCer_NS
                Dim HexCerSpectrumGenerator = New HexCerSpectrumGenerator()
                spectrum.AddRange(HexCerSpectrumGenerator.Generate(lipid, adduct).Spectrum)
            Case LbmClass.SM
                Dim SMSpectrumGenerator = New SMSpectrumGenerator()
                spectrum.AddRange(SMSpectrumGenerator.Generate(lipid, adduct).Spectrum)

                spectrum.AddRange({New SpectrumPeak(C5H14NO, 50.0R, "C5H14NO") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
            Case Else
                spectrum.Add(New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
                    .SpectrumComment = SpectrumComment.precursor
                })
        End Select
        Return spectrum
    End Function

    Private Shared Function doublebondPositions(chain As IChain) As List(Of Integer)
        Dim bondPositions = New List(Of Integer)()
        Dim dbPosition = chain.DoubleBond.Bonds
        bondPositions.AddRange(From bond In dbPosition Select bond.Position)
        Return bondPositions
    End Function

    Private Shared Function EidSpecificSpectrum(lipid As Lipid, adduct As AdductIon, nlMass As Double, intensity As Double) As SpectrumPeak()
        Dim spectrum = New List(Of SpectrumPeak)()
        Dim chains As SeparatedChains = TryCast(lipid.Chains, SeparatedChains)

        If chains IsNot Nothing Then
            For Each chain In chains.GetDeterminedChains()
                If chain.DoubleBond.Count = 0 OrElse chain.DoubleBond.UnDecidedCount > 0 Then Continue For
                Dim dbPosition = doublebondPositions(chain)
                intensity = intensity / dbPosition.Count
                For Each db In dbPosition
                    spectrum.AddRange(EidSpecificSpectrumGenerator.EidSpecificSpectrumGen(lipid, chain, adduct, nlMass, intensity))
                Next
            Next
        End If
        Return spectrum.ToArray()
    End Function


    Private Shared ReadOnly Electron As Double = 0.00054858026

    Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()
    Private Shared ReadOnly NH3 As Double = {HydrogenMass * 3, NitrogenMass}.Sum()
    Private Shared ReadOnly C5H14NO4P As Double = {CarbonMass * 5, HydrogenMass * 14, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum() 'PC SM Header
    Private Shared ReadOnly C5H14NO As Double = {CarbonMass * 5, HydrogenMass * 13, NitrogenMass, OxygenMass, ProtonMass}.Sum() 'PC 104.107

    Private Shared ReadOnly C2H8NO4P As Double = {CarbonMass * 2, HydrogenMass * 8, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum() 'PE Header
    Private Shared ReadOnly C3H9O6P As Double = {CarbonMass * 3, HydrogenMass * 9, OxygenMass * 6, PhosphorusMass}.Sum() ' PG Header
    Private Shared ReadOnly C3H8NO6P As Double = {CarbonMass * 3, HydrogenMass * 8, NitrogenMass, OxygenMass * 6, PhosphorusMass}.Sum() ' PS Header
    Private Shared ReadOnly C6H13O9P As Double = {CarbonMass * 6, HydrogenMass * 13, OxygenMass * 9, PhosphorusMass}.Sum() ' PI Header


End Class


