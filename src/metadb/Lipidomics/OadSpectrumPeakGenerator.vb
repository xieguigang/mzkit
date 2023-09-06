Public Class OadSpectrumPeakGenerator
    Implements IOadSpectrumPeakGenerator
    Private Shared ReadOnly CH2 As Double = {HydrogenMass * 2, CarbonMass}.Sum()

    Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()

    Private Shared ReadOnly Electron As Double = 0.00054858026


    Private Function GetOadDoubleBondSpectrum(lipid As ILipid, chain As IChain, adduct As AdductIon, nlMass As Double, abundance As Double, oadId As String()) As IEnumerable(Of SpectrumPeak)
        If chain.DoubleBond.UnDecidedCount <> 0 OrElse chain.CarbonCount = 0 Then
            Return Enumerable.Empty(Of SpectrumPeak)()
        End If
        Dim chainLoss = lipid.Mass - chain.Mass - nlMass
        Dim diffs = New Double(chain.CarbonCount - 1) {}
        For i = 0 To chain.CarbonCount - 1 ' numbering from COOH. 18:2(9,12) -> 9 is 8 and 12 is 11 
            diffs(i) = CH2
        Next

        Dim bondPositions = New List(Of Integer)()
        For Each bond In chain.DoubleBond.Bonds ' double bond 18:2(9,12) -> 9 is 9 and 12 is 12 
            diffs(bond.Position - 1) -= HydrogenMass
            diffs(bond.Position) -= HydrogenMass
            bondPositions.Add(bond.Position)
        Next
        For i = 1 To chain.CarbonCount - 1
            diffs(i) += diffs(i - 1)
        Next

        Dim peaks = New List(Of SpectrumPeak)()
        For Each bond In bondPositions
            If bond <> 1 Then
                Dim addPeaks = DoubleBondSpectrum(bond, diffs, chain, adduct, chainLoss, abundance, oadId)
                peaks.AddRange(addPeaks)
            Else
                Dim speccomment = SpectrumComment.doublebond
                Dim factor = 1.0
                Dim dbPeakHigher = chainLoss + diffs(bond) + HydrogenMass + OxygenMass
                peaks.Add(New SpectrumPeak(adduct.ConvertToMz(dbPeakHigher + HydrogenMass), factor * abundance * 0.2, $"{chain} C{bond} +C +O +H OAD01") With {
                    .SpectrumComment = speccomment
                })
                peaks.Add(New SpectrumPeak(adduct.ConvertToMz(dbPeakHigher), factor * abundance * 0.5, $"{chain} C{bond} +O OAD02") With {
                    .SpectrumComment = speccomment
                })
                'peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakHigher - MassDiffDictionary.HydrogenMass), (factor * abundance), $"{chain} C{bond} +C +O -H OAD03") { SpectrumComment = speccomment | SpectrumComment.doublebond_high });
            End If
        Next
        Return peaks
    End Function

    Private Function DoubleBondSpectrum(bond As Integer, diffs As Double(), chain As IChain, adduct As AdductIon, chainLoss As Double, abundance As Double, oadId As String()) As List(Of SpectrumPeak)
        Dim OadPeaks = DoubleBondSpectrumWithId(bond, diffs, chain, adduct, chainLoss, abundance)
        Return OadPeaks.Where(Function(p) oadId.Contains(p.OadId)).[Select](Function(p) p.spectrum).ToList()
    End Function
    'private List<SpectrumPeak> DoubleBondSpectrum(int bond, double[] diffs, IChain chain, AdductIon adduct, double chainLoss, double abundance, ILipid lipid)
    '{
    '    var peaks = new List<SpectrumPeak>();
    '    var speccomment = SpectrumComment.doublebond;
    '    var factor = 1.0;
    '    var dbPeakHigher = chainLoss + diffs[bond] + MassDiffDictionary.HydrogenMass + MassDiffDictionary.OxygenMass;
    '    var dbPeak = chainLoss + diffs[bond - 1];
    '    var dbPeakLower = chainLoss + diffs[bond - 2] + MassDiffDictionary.HydrogenMass;
    '    if (adduct.IonMode == IonModes.Positive)
    '    {
    '        peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakHigher + MassDiffDictionary.HydrogenMass), (factor * abundance * 0.3), $"{chain} C{bond} +C +O +H OAD01") { SpectrumComment = speccomment });
    '        peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakHigher), (factor * abundance), $"{chain} C{bond} +O OAD02") { SpectrumComment = speccomment | SpectrumComment.doublebond_high });
    '        if (lipid.LipidClass != LbmClass.PE)
    '        {
    '            peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakHigher + MassDiffDictionary.OxygenMass), (factor * abundance * 0.3), $"{chain} C{bond} +O OAD02+O") { SpectrumComment = speccomment });
    '        }
    '        peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakHigher - MassDiffDictionary.HydrogenMass), (factor * abundance * 0.5), $"{chain} C{bond} +C +O -H OAD03") { SpectrumComment = speccomment });
    '        peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakHigher - MassDiffDictionary.HydrogenMass * 2), (factor * abundance * 0.2), $"{chain} C{bond} +C +O -2H OAD04") { SpectrumComment = speccomment });

    '        //peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakHigher - H2O + MassDiffDictionary.HydrogenMass), (factor * abundance * 0.15), $"{chain} C{bond} +C +O +H -H2O OAD05") { SpectrumComment = speccomment });
    '        //peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakHigher - H2O), (factor * abundance * 0.3), $"{chain} C{bond} +C +O -H2O OAD06") { SpectrumComment = speccomment });
    '        //peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakHigher - H2O - MassDiffDictionary.HydrogenMass), (factor * abundance * 0.1), $"{chain} C{bond} +C +O -H -H2O OAD07") { SpectrumComment = speccomment });

    '        if (lipid.LipidClass != LbmClass.PE)
    '        {
    '            peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeak + MassDiffDictionary.OxygenMass), (factor * abundance * 0.2), $"{chain} C{bond} +O OAD08") { SpectrumComment = speccomment });
    '        }
    '        //peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeak + MassDiffDictionary.HydrogenMass * 2), (factor * abundance * 0.2), $"{chain} C{bond} OAD09") { SpectrumComment = speccomment });
    '        //peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeak + MassDiffDictionary.HydrogenMass), (factor * abundance * 0.3), $"{chain} C{bond} -H OAD10") { SpectrumComment = speccomment });
    '        //peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeak), (factor * abundance * 0.1), $"{chain} C{bond} -2H OAD11") { SpectrumComment = speccomment });

    '        if (lipid.LipidClass != LbmClass.PE)
    '        {
    '            peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakLower + MassDiffDictionary.OxygenMass - MassDiffDictionary.HydrogenMass), (factor * abundance * 0.3), $"{chain} C{bond} +O -H OAD12") { SpectrumComment = speccomment });
    '        }
    '        //peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakLower + MassDiffDictionary.OxygenMass - MassDiffDictionary.HydrogenMass * 2), (factor * abundance * 0.3), $"{chain} C{bond} +O -2H OAD13") { SpectrumComment = speccomment });
    '        peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakLower + MassDiffDictionary.HydrogenMass), (factor * abundance * 0.2), $"{chain} C{bond} -C +H OAD14") { SpectrumComment = speccomment });
    '        peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakLower), (factor * abundance * 0.8), $"{chain} C{bond} -C OAD15") { SpectrumComment = speccomment | SpectrumComment.doublebond_high });
    '        if (lipid.LipidClass != LbmClass.PE)
    '        {
    '            peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakLower + MassDiffDictionary.OxygenMass), (factor * abundance * 0.2), $"{chain} C{bond} -C OAD15+O") { SpectrumComment = speccomment });
    '        }
    '        peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakLower - MassDiffDictionary.HydrogenMass), (factor * abundance * 0.2), $"{chain} C{bond} -C -H OAD16") { SpectrumComment = speccomment });
    '        peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakLower - MassDiffDictionary.HydrogenMass * 2), (factor * abundance * 0.15), $"{chain} C{bond} -C -2H OAD17") { SpectrumComment = speccomment });

    '        //add 20230330
    '        peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakLower + MassDiffDictionary.OxygenMass * 2 - MassDiffDictionary.HydrogenMass), (factor * abundance * 0.3), $"{chain} C{bond} +O -H OAD12+O") { SpectrumComment = speccomment });
    '        peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakLower + MassDiffDictionary.OxygenMass * 2), (factor * abundance * 0.3), $"{chain} C{bond} +O -H OAD12+O+H") { SpectrumComment = speccomment });
    '        if (lipid.LipidClass != LbmClass.PE)
    '        {
    '            peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakLower + MassDiffDictionary.OxygenMass * 2 + MassDiffDictionary.HydrogenMass), (factor * abundance * 0.2), $"{chain} C{bond} +O -H OAD12+O+2H") { SpectrumComment = speccomment });
    '        }
    '        peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakHigher + MassDiffDictionary.HydrogenMass * 2), (factor * abundance * 0.2), $"{chain} C{bond} +C +O +H OAD01+H") { SpectrumComment = speccomment });
    '    }
    '    else if (adduct.IonMode == IonModes.Negative)
    '    {
    '        //add 20230330
    '        peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakHigher + MassDiffDictionary.HydrogenMass), (factor * abundance * 0.2), $"{chain} C{bond} +C +O +H OAD01") { SpectrumComment = speccomment });
    '        peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakHigher), (factor * abundance * 0.3), $"{chain} C{bond} +O OAD02") { SpectrumComment = speccomment | SpectrumComment.doublebond_high });
    '        //peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakHigher + MassDiffDictionary.OxygenMass), (factor * abundance * 0.3), $"{chain} C{bond} +O OAD02+O") { SpectrumComment = speccomment });
    '        peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakHigher - MassDiffDictionary.HydrogenMass), (factor * abundance * 0.5), $"{chain} C{bond} +C +O -H OAD03") { SpectrumComment = speccomment });
    '        peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakHigher - MassDiffDictionary.HydrogenMass * 2), (factor * abundance * 0.8), $"{chain} C{bond} +C +O -2H OAD04") { SpectrumComment = speccomment });

    '        if (lipid.LipidClass != LbmClass.PE)
    '        {
    '            peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakHigher - H2O + MassDiffDictionary.HydrogenMass), (factor * abundance * 0.2), $"{chain} C{bond} +C +O +H -H2O OAD05") { SpectrumComment = speccomment });
    '        }
    '        //peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakHigher - H2O), (factor * abundance * 0.3), $"{chain} C{bond} +C +O -H2O OAD06") { SpectrumComment = speccomment });
    '        //peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakHigher - H2O - MassDiffDictionary.HydrogenMass), (factor * abundance * 0.1), $"{chain} C{bond} +C +O -H -H2O OAD07") { SpectrumComment = speccomment });

    '        //peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeak + MassDiffDictionary.OxygenMass), (factor * abundance * 0.2), $"{chain} C{bond} +O OAD08") { SpectrumComment = speccomment });
    '        //peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeak + MassDiffDictionary.HydrogenMass * 2), (factor * abundance * 0.2), $"{chain} C{bond} OAD09") { SpectrumComment = speccomment });
    '        if (lipid.LipidClass != LbmClass.PE)
    '        {
    '            peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeak + MassDiffDictionary.HydrogenMass), (factor * abundance * 0.3), $"{chain} C{bond} -H OAD10") { SpectrumComment = speccomment });
    '        }
    '        //peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeak), (factor * abundance * 0.1), $"{chain} C{bond} -2H OAD11") { SpectrumComment = speccomment });

    '        if (lipid.LipidClass != LbmClass.PC)
    '        {
    '            peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakLower + MassDiffDictionary.OxygenMass - MassDiffDictionary.HydrogenMass), (factor * abundance * 0.3), $"{chain} C{bond} +O -H OAD12") { SpectrumComment = speccomment });
    '        }
    '        if (lipid.LipidClass != LbmClass.PE)
    '        {
    '            peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakLower + MassDiffDictionary.OxygenMass - MassDiffDictionary.HydrogenMass * 2), (factor * abundance * 0.3), $"{chain} C{bond} +O -2H OAD13") { SpectrumComment = speccomment });
    '        }
    '        if (lipid.LipidClass != LbmClass.PC)
    '        {
    '            peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakLower + MassDiffDictionary.HydrogenMass), (factor * abundance * 0.3), $"{chain} C{bond} -C +H OAD14") { SpectrumComment = speccomment });
    '        }
    '        peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakLower), (factor * abundance * 0.3), $"{chain} C{bond} -C OAD15") { SpectrumComment = speccomment | SpectrumComment.doublebond_high });
    '        if (lipid.LipidClass != LbmClass.PC)
    '        {
    '            peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakLower + MassDiffDictionary.OxygenMass), (factor * abundance * 0.2), $"{chain} C{bond} -C OAD15+O") { SpectrumComment = speccomment });
    '        }
    '        peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakLower - MassDiffDictionary.HydrogenMass), (factor * abundance * 0.3), $"{chain} C{bond} -C -H OAD16") { SpectrumComment = speccomment });
    '        //peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakLower - MassDiffDictionary.HydrogenMass * 2), (factor * abundance * 0.15), $"{chain} C{bond} -C -2H OAD17") { SpectrumComment = speccomment });

    '        peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakLower + MassDiffDictionary.OxygenMass * 2 - MassDiffDictionary.HydrogenMass), (factor * abundance * 0.5), $"{chain} C{bond} +O -H OAD12+O") { SpectrumComment = speccomment });
    '        peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakLower + MassDiffDictionary.OxygenMass * 2), (factor * abundance * 0.8), $"{chain} C{bond} +O -H OAD12+O+H") { SpectrumComment = speccomment });
    '        peaks.Add(new SpectrumPeak(adduct.ConvertToMz(dbPeakLower + MassDiffDictionary.OxygenMass * 2 + MassDiffDictionary.HydrogenMass), (factor * abundance * 0.2), $"{chain} C{bond} +O -H OAD12+O+2H") { SpectrumComment = speccomment });
    '    }

    '    return peaks;
    '}

    Private Function DoubleBondSpectrumWithId(bond As Integer, diffs As Double(), chain As IChain, adduct As AdductIon, chainLoss As Double, abundance As Double) As List(Of OadFragmentPeaks)
        Dim OadPeaks = New List(Of OadFragmentPeaks)()
        Dim speccomment = SpectrumComment.doublebond
        Dim factor = 1.0
        Dim dbPeakHigher = chainLoss + diffs(bond) + HydrogenMass + OxygenMass
        Dim dbPeak = chainLoss + diffs(bond - 1)
        Dim dbPeakLower = chainLoss + diffs(bond - 2) + HydrogenMass
        OadPeaks.AddRange(New OadFragmentPeaks() {New OadFragmentPeaks With {
.OadId = "OAD01",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeakHigher + HydrogenMass), factor * abundance * 0.3, $"{chain} C{bond} +C +O +H OAD01") With {
.SpectrumComment = speccomment
}
    }, New OadFragmentPeaks With {
.OadId = "OAD02",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeakHigher), factor * abundance, $"{chain} C{bond} +O OAD02") With {
.SpectrumComment = speccomment Or SpectrumComment.doublebond_high
}
    }, New OadFragmentPeaks With {
.OadId = "OAD02+O",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeakHigher + OxygenMass), factor * abundance * 0.3, $"{chain} C{bond} +O OAD02+O") With {
.SpectrumComment = speccomment
}
    }, New OadFragmentPeaks With {
.OadId = "OAD03",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeakHigher - HydrogenMass), factor * abundance * 0.5, $"{chain} C{bond} +C +O -H OAD03") With {
.SpectrumComment = speccomment
}
    }, New OadFragmentPeaks With {
.OadId = "OAD04",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeakHigher - HydrogenMass * 2), factor * abundance * 0.2, $"{chain} C{bond} +C +O -2H OAD04") With {
.SpectrumComment = speccomment
}

}, New OadFragmentPeaks With {
.OadId = "OAD05",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeakHigher - H2O + HydrogenMass), factor * abundance * 0.15, $"{chain} C{bond} +C +O +H -H2O OAD05") With {
.SpectrumComment = speccomment
}
        }, New OadFragmentPeaks With {
.OadId = "OAD06",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeakHigher - H2O), factor * abundance * 0.3, $"{chain} C{bond} +C +O -H2O OAD06") With {
.SpectrumComment = speccomment
}
        }, New OadFragmentPeaks With {
.OadId = "OAD07",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeakHigher - H2O - HydrogenMass), factor * abundance * 0.1, $"{chain} C{bond} +C +O -H -H2O OAD07") With {
.SpectrumComment = speccomment
}

}, New OadFragmentPeaks With {
.OadId = "OAD08",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeak + OxygenMass), factor * abundance * 0.2, $"{chain} C{bond} +O OAD08") With {
.SpectrumComment = speccomment
}
        }, New OadFragmentPeaks With {
.OadId = "OAD09",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeak + HydrogenMass * 2), factor * abundance * 0.2, $"{chain} C{bond} OAD09") With {
.SpectrumComment = speccomment
}
        }, New OadFragmentPeaks With {
.OadId = "OAD10",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeak + HydrogenMass), factor * abundance * 0.3, $"{chain} C{bond} -H OAD10") With {
.SpectrumComment = speccomment
}
        }, New OadFragmentPeaks With {
.OadId = "OAD11",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeak), factor * abundance * 0.1, $"{chain} C{bond} -2H OAD11") With {
.SpectrumComment = speccomment
}

}, New OadFragmentPeaks With {
.OadId = "OAD12",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeakLower + OxygenMass - HydrogenMass), factor * abundance * 0.3, $"{chain} C{bond} +O -H OAD12") With {
.SpectrumComment = speccomment
}

}, New OadFragmentPeaks With {
.OadId = "OAD13",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeakLower + OxygenMass - HydrogenMass * 2), factor * abundance * 0.3, $"{chain} C{bond} +O -2H OAD13") With {
.SpectrumComment = speccomment
}
        }, New OadFragmentPeaks With {
.OadId = "OAD14",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeakLower + HydrogenMass), factor * abundance * 0.2, $"{chain} C{bond} -C +H OAD14") With {
.SpectrumComment = speccomment
}
        }, New OadFragmentPeaks With {
.OadId = "OAD15",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeakLower), factor * abundance * 0.8, $"{chain} C{bond} -C OAD15") With {
.SpectrumComment = speccomment Or SpectrumComment.doublebond_high
}
        }, New OadFragmentPeaks With {
.OadId = "OAD15+O",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeakLower + OxygenMass), factor * abundance * 0.2, $"{chain} C{bond} -C OAD15+O") With {
.SpectrumComment = speccomment
}
        }, New OadFragmentPeaks With {
.OadId = "OAD16",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeakLower - HydrogenMass), factor * abundance * 0.2, $"{chain} C{bond} -C -H OAD16") With {
.SpectrumComment = speccomment
}
        }, New OadFragmentPeaks With {
.OadId = "OAD17",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeakLower - HydrogenMass * 2), factor * abundance * 0.15, $"{chain} C{bond} -C -2H OAD17") With {
.SpectrumComment = speccomment
}

'add 20230330
}, New OadFragmentPeaks With {
.OadId = "OAD12+O",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeakLower + OxygenMass * 2 - HydrogenMass), factor * abundance * 0.3, $"{chain} C{bond} +O -H OAD12+O") With {
.SpectrumComment = speccomment
}
        }, New OadFragmentPeaks With {
.OadId = "OAD12+O+H",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeakLower + OxygenMass * 2), factor * abundance * 0.3, $"{chain} C{bond} +O -H OAD12+O+H") With {
.SpectrumComment = speccomment
}
        }, New OadFragmentPeaks With {
.OadId = "OAD12+O+2H",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeakLower + OxygenMass * 2 + HydrogenMass), factor * abundance * 0.2, $"{chain} C{bond} +O -H OAD12+O+2H") With {
.SpectrumComment = speccomment
}
        }, New OadFragmentPeaks With {
.OadId = "OAD01+H",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeakHigher + HydrogenMass * 2), factor * abundance * 0.2, $"{chain} C{bond} +C +O +H OAD01+H") With {
.SpectrumComment = speccomment
}
}})
            Return OadPeaks
    End Function

    Private Function SphingoDoubleBondSpectrumWithId(bond As Integer, diffs As Double(), sphingo As IChain, adduct As AdductIon, chainLoss As Double, abundance As Double) As List(Of OadFragmentPeaks)
        Dim OadPeaks = New List(Of OadFragmentPeaks)()
        Dim speccomment = SpectrumComment.doublebond
        Dim factor = 1.0
        Dim dbPeak = chainLoss + diffs(bond - 1 - 1) - HydrogenMass
        OadPeaks.AddRange(New OadFragmentPeaks() {New OadFragmentPeaks With {
.OadId = "SphOAD",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeak + Electron), factor * abundance, $"{sphingo} C{bond} DB ") With {
.SpectrumComment = speccomment
}
    }, New OadFragmentPeaks With {
.OadId = "SphOAD+H",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeak + HydrogenMass), factor * abundance * 0.5, $"{sphingo} C{bond} DB +H") With {
.SpectrumComment = speccomment
}
    }, New OadFragmentPeaks With {
.OadId = "SphOAD+2H",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeak + HydrogenMass * 2), factor * abundance * 0.5, $"{sphingo} C{bond} DB +2H") With {
.SpectrumComment = speccomment
}
   }, New OadFragmentPeaks With {
.OadId = "SphOAD-CO",
.spectrum = New SpectrumPeak(adduct.ConvertToMz(dbPeak + Electron - CarbonMass - OxygenMass), factor * abundance, $"{sphingo} C{bond} DB -C=O ") With {
.SpectrumComment = speccomment
}
}})
        Return OadPeaks
    End Function

    Public Function GetAcylDoubleBondSpectrum(lipid As ILipid, acylChain As AcylChain, adduct As AdductIon, nlMass As Double, abundance As Double, oadId As String()) As IEnumerable(Of SpectrumPeak) Implements IOadSpectrumPeakGenerator.GetAcylDoubleBondSpectrum
        Return GetOadDoubleBondSpectrum(lipid, acylChain, adduct, nlMass - OxygenMass + HydrogenMass * 2, abundance, oadId)
    End Function

    Public Function GetAlkylDoubleBondSpectrum(lipid As ILipid, acylChain As AlkylChain, adduct As AdductIon, nlMass As Double, abundance As Double, oadId As String()) As IEnumerable(Of SpectrumPeak) Implements IOadSpectrumPeakGenerator.GetAlkylDoubleBondSpectrum
        Return GetOadDoubleBondSpectrum(lipid, acylChain, adduct, nlMass, abundance, oadId)
    End Function

    Public Function GetSphingoDoubleBondSpectrum(lipid As ILipid, sphingo As SphingoChain, adduct As AdductIon, nlMass As Double, abundance As Double, oadId As String()) As IEnumerable(Of SpectrumPeak) Implements IOadSpectrumPeakGenerator.GetSphingoDoubleBondSpectrum
        If sphingo.DoubleBond.UnDecidedCount <> 0 OrElse sphingo.CarbonCount = 0 Then
            Return Enumerable.Empty(Of SpectrumPeak)()
        End If
        Dim chainLoss = lipid.Mass - sphingo.Mass - nlMass + NitrogenMass + OxygenMass * 2 + HydrogenMass * 1
        Dim diffs = New Double(sphingo.CarbonCount - 1) {}
        For i = 0 To sphingo.CarbonCount - 1
            diffs(i) = CH2
        Next

        Dim bondPositions = New List(Of Integer)()
        For Each bond In sphingo.DoubleBond.Bonds
            diffs(bond.Position - 1) -= HydrogenMass
            diffs(bond.Position) -= HydrogenMass
            bondPositions.Add(bond.Position)
        Next
        For i = 1 To sphingo.CarbonCount - 1
            diffs(i) += diffs(i - 1)
        Next

        Dim peaks = New List(Of SpectrumPeak)()
        For Each bond In bondPositions
            If bond <> 4 Then
                Dim addPeaks = DoubleBondSpectrum(bond, diffs, sphingo, adduct, chainLoss, abundance, oadId)
                peaks.AddRange(addPeaks)
            Else
                Dim SphOadPeaks = SphingoDoubleBondSpectrumWithId(bond, diffs, sphingo, adduct, chainLoss, abundance)
                peaks.AddRange(SphOadPeaks.Where(Function(p) oadId.Contains(p.OadId)).[Select](Function(p) p.spectrum).ToList())
            End If
        Next
        Return peaks
    End Function
End Class
Public Class OadFragmentPeaks
    Public Property OadId As String
    Public Property spectrum As SpectrumPeak
End Class

