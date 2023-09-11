Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

Public Class OadLipidSpectrumGenerator
    Public Function GetClassFragmentSpectrum(lipid As ILipid, adduct As AdductIon) As OadClassFragment
        Dim spectrum = New List(Of SpectrumPeak)()
        Dim nlMass = 0.0
        Dim alkyl As AlkylChain = Nothing, acyl As AcylChain = Nothing, plChains As PositionLevelChains = Nothing, tgChains As PositionLevelChains = Nothing, dgChains As PositionLevelChains = Nothing

        Select Case lipid.LipidClass
            Case LbmClass.PC, LbmClass.EtherPC, LbmClass.SM
                nlMass = 0.0
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}, New SpectrumPeak(C5H14NO, 50.0R, "C5H14NO") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(C5H14NO4P), 100.0R, "Header") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}})
                End If
            Case LbmClass.LPC, LbmClass.EtherLPC
                nlMass = 0.0
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
            Case LbmClass.PE
                nlMass = C2H8NO4P
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - C2H8NO4P), 800.0R, "Precursor -C2H8NO4P") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}})
            Case LbmClass.EtherPE
                nlMass = 0.0

                plChains = TryCast(lipid.Chains, PositionLevelChains)

                If plChains IsNot Nothing Then
                    Dim nil As (alkyl As AlkylChain, acyl As AcylChain) = lipid.Chains.Deconstruct(Of AlkylChain, AcylChain)()
                    alkyl = nil.alkyl
                    acyl = nil.acyl
                    If alkyl IsNot Nothing AndAlso acyl IsNot Nothing AndAlso alkyl.DoubleBond.Bonds.Any(Function(b) b.Position = 1) Then
                        spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}, New SpectrumPeak(adduct.ConvertToMz(alkyl.Mass + C2H8NO4P - HydrogenMass), 500.0R, "Precursor") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - C2H8NO4P), 100.0R, "Precursor -C2H8NO4P") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}})
                    Else
                        spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - C2H8NO4P), 100.0R, "Precursor -C2H8NO4P") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}})
                    End If
                End If
            Case LbmClass.LPE, LbmClass.EtherLPE
                nlMass = C2H8NO4P
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - C2H8NO4P), 100.0R, "Precursor -C2H8NO4P") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - H2O), 200.0R, "Precursor - H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}})
            Case LbmClass.PG
                If Equals(adduct.AdductIonName, "[M+NH4]+") Then
                    nlMass = C3H9O6P + NH3
                End If
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 100.0R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}, New SpectrumPeak(lipid.Mass - C3H9O6P + ProtonMass, 999.0R, "Precursor -C3H9O6P") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}})
            Case LbmClass.PS
                nlMass = C3H8NO6P
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 200.0R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}, New SpectrumPeak(lipid.Mass - C3H8NO6P + ProtonMass, 999.0R, "Precursor -C3H8NO6P") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}})
            Case LbmClass.PI
                If Equals(adduct.AdductIonName, "[M+NH4]+") Then
                    nlMass = C6H13O9P + NH3
                End If
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 200.0R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}, New SpectrumPeak(lipid.Mass - C6H13O9P + ProtonMass, 999.0R, "Precursor -C6H13O9P") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}})

            Case LbmClass.TG
                nlMass = 0.0
                spectrum.Add(New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
                    .SpectrumComment = SpectrumComment.precursor
                })
                tgChains = TryCast(lipid.Chains, PositionLevelChains)
                If tgChains IsNot Nothing Then
                    For Each chain In lipid.Chains.GetDeterminedChains()
                        spectrum.Add(New SpectrumPeak(lipid.Mass - chain.Mass - OxygenMass, 200.0R, $"{chain} loss") With {
                                .SpectrumComment = SpectrumComment.acylchain
                            })
                    Next
                End If
            Case LbmClass.DG
                If Equals(adduct.AdductIonName, "[M+NH4]+") Then
                    nlMass = H2O + NH3
                Else
                    nlMass = H2O
                End If
                spectrum.Add(New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 100.0R, "Precursor") With {
                    .SpectrumComment = SpectrumComment.precursor
                })
                spectrum.Add(New SpectrumPeak(lipid.Mass + ProtonMass, 200.0R, "[M+H]+") With {
                    .SpectrumComment = SpectrumComment.metaboliteclass
                })
                spectrum.Add(New SpectrumPeak(lipid.Mass - H2O + ProtonMass, 999.0R, "[M+H]+ -H2O") With {
                    .SpectrumComment = SpectrumComment.metaboliteclass
                })
                dgChains = TryCast(lipid.Chains, PositionLevelChains)
                If dgChains IsNot Nothing Then
                    For Each chain In lipid.Chains.GetDeterminedChains()
                        spectrum.Add(New SpectrumPeak(lipid.Mass - chain.Mass - OxygenMass - Electron, 800.0R, $"{chain} loss") With {
                                .SpectrumComment = SpectrumComment.acylchain
                            })
                    Next
                End If

            Case Else
                spectrum.Add(New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999.0R, "Precursor") With {
                    .SpectrumComment = SpectrumComment.precursor
                })
        End Select
        Return New OadClassFragment() With {
            .nlMass = nlMass,
            .spectrum = spectrum
        }
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

