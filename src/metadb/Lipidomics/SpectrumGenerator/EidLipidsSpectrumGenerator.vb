Imports CompMs.Common.Components
Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.FormulaGenerator.DataObj
Imports CompMs.Common.Enum
Imports System
Imports System.Collections.Generic
Imports System.Linq


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

                    spectrum.AddRange({New SpectrumPeak(C5H14NO, 50R, "C5H14NO") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
                Case Else
                    spectrum.Add(New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999R, "Precursor") With {
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
            Dim chains As SeparatedChains = Nothing

            If CSharpImpl.__Assign(chains, TryCast(lipid.Chains, SeparatedChains)) IsNot Nothing Then
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

