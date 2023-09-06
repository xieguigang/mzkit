Imports System.Linq
Imports CompMs.Common.Components
Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.Enum
Imports CompMs.Common.FormulaGenerator.DataObj
Imports CompMs.Common.Interfaces
Imports System
Imports System.Collections.Generic


Public NotInheritable Class LipidEieioMsmsCharacterization
        Private Sub New()
        End Sub

        Private Const Electron As Double = 0.00054858026
        Private Const Proton As Double = 1.00727641974
        Private Const H2O As Double = 18.010564684
        Private Const Sugar162 As Double = 162.052823422
        Private Const Na As Double = 22.98977
        Private Shared ReadOnly CH2 As Double = {HydrogenMass * 2, CarbonMass}.Sum()
        Private Shared ReadOnly NH3 As Double = {HydrogenMass * 3, NitrogenMass}.Sum()
        Private Shared ReadOnly CD2 As Double = {Hydrogen2Mass * 2, CarbonMass}.Sum()

        Public Shared Function setSnPositionAtTwoChains(spectrum As List(Of SpectrumPeak), ms2Tolerance As Double, fragment1 As Double, fragment2 As Double) As Integer
            Dim frag1intensity = 0.0
            Dim frag2intensity = 0.0
            For i = 0 To spectrum.Count - 1
                Dim mz = spectrum(i).Mass
                Dim intensity = spectrum(i).Intensity ' should be normalized by max intensity to 100

                If intensity > frag1intensity AndAlso Math.Abs(mz - fragment1) < ms2Tolerance Then
                    frag1intensity = intensity
                End If

                If intensity > frag2intensity AndAlso Math.Abs(mz - fragment2) < ms2Tolerance Then
                    frag2intensity = intensity
                End If
            Next

            If frag1intensity > frag2intensity Then
                Return 0
            Else
                Return 1
            End If
        End Function

        Public Shared Function getTwoChainLipidMoleculeObjAsPositionChain(lipidClass As String, lbmClass As LbmClass, sn1Carbon As Integer, sn1Double As Integer, sn2Carbon As Integer, sn2Double As Integer, score As Double) As LipidMolecule
            Dim totalCarbon = sn1Carbon + sn2Carbon
            Dim totalDB = sn1Double + sn2Double
            Dim totalString = totalCarbon.ToString() & ":" & totalDB.ToString()
            Dim totalName = lipidClass & " " & totalString

            'finally, acyl name ordering is determined by double bond count and acyl length
            Dim acyls = New List(Of Integer())() From {
                New Integer() {sn1Carbon, sn1Double},
                New Integer() {sn2Carbon, sn2Double}
            }
            acyls = acyls.OrderBy(Function(n) n(1)).ThenBy(Function(n) n(0)).ToList()
            Dim sn1CarbonCount = acyls(0)(0)
            Dim sn1DbCount = acyls(0)(1)
            Dim sn2CarbonCount = acyls(1)(0)
            Dim sn2DbCount = acyls(1)(1)

            Dim sn1ChainString = sn1CarbonCount.ToString() & ":" & sn1DbCount.ToString()
            Dim sn2ChainString = sn2CarbonCount.ToString() & ":" & sn2DbCount.ToString()
            Dim chainString = sn1ChainString & "/" & sn2ChainString 'position
            Dim lipidName = lipidClass & " " & chainString

            Return New LipidMolecule() With {
    .LipidClass = lbmClass,
    .AnnotationLevel = 2,
    .SublevelLipidName = totalName,
    .LipidName = lipidName,
    .TotalCarbonCount = totalCarbon,
    .TotalDoubleBondCount = totalDB,
    .TotalChainString = totalString,
    .Score = score,
    .Sn1CarbonCount = sn1CarbonCount,
    .Sn1DoubleBondCount = sn1DbCount,
    .Sn1AcylChainString = sn1ChainString,
    .Sn2CarbonCount = sn2CarbonCount,
    .Sn2DoubleBondCount = sn2DbCount,
    .Sn2AcylChainString = sn2ChainString
}
        End Function
        Public Shared Function getTriacylglycerolMoleculeObjAsPositionChain(lipidClass As String, lbmClass As LbmClass, sn1Carbon As Integer, sn1Double As Integer, sn2Carbon As Integer, sn2Double As Integer, sn3Carbon As Integer, sn3Double As Integer, score As Double) As LipidMolecule

            Dim totalCarbon = sn1Carbon + sn2Carbon + sn3Carbon
            Dim totalDB = sn1Double + sn2Double + sn3Double
            Dim totalString = totalCarbon.ToString() & ":" & totalDB.ToString()
            Dim totalName = lipidClass & " " & totalString

            Dim acyls = New List(Of Integer())() From {
                New Integer() {sn1Carbon, sn1Double},
                New Integer() {sn3Carbon, sn3Double}
            }
            acyls = acyls.OrderBy(Function(n) n(1)).ThenBy(Function(n) n(0)).ToList()

            Dim sn1CarbonCount = acyls(0)(0)
            Dim sn1DbCount = acyls(0)(1)
            Dim sn3CarbonCount = acyls(1)(0)
            Dim sn3DbCount = acyls(1)(1)

            Dim sn2CarbonCount = sn2Carbon
            Dim sn2DbCount = sn2Double

            Dim sn1ChainString = sn1CarbonCount.ToString() & ":" & sn1DbCount.ToString()
            Dim sn2ChainString = sn2Carbon.ToString() & ":" & sn2Double.ToString()
            Dim sn3ChainString = sn3CarbonCount.ToString() & ":" & sn3DbCount.ToString()
            Dim chainString = sn1ChainString & "/" & sn2ChainString & "/" & sn3ChainString 'position
            Dim lipidName = lipidClass & " " & chainString

            Return New LipidMolecule() With {
    .LipidClass = lbmClass,
    .AnnotationLevel = 2,
    .SublevelLipidName = totalName,
    .LipidName = lipidName,
    .TotalCarbonCount = totalCarbon,
    .TotalDoubleBondCount = totalDB,
    .TotalChainString = totalString,
    .Score = score,
    .Sn1CarbonCount = sn1CarbonCount,
    .Sn1DoubleBondCount = sn1DbCount,
    .Sn1AcylChainString = sn1ChainString,
    .Sn2CarbonCount = sn2CarbonCount,
    .Sn2DoubleBondCount = sn2DbCount,
    .Sn2AcylChainString = sn2ChainString,
    .Sn3CarbonCount = sn3CarbonCount,
    .Sn3DoubleBondCount = sn3DbCount,
    .Sn3AcylChainString = sn3ChainString
}
        End Function
        Public Shared Function getEtherPhospholipidMoleculeObjAsPositionChain(lipidClass As String, lbmClass As LbmClass, sn1Carbon As Integer, sn1Double As Integer, sn1ChainString As String, sn2Carbon As Integer, sn2Double As Integer, sn2ChainString As String, score As Double, chainSuffix As String) As LipidMolecule
            Dim chainPrefix = chainSuffix
            Select Case chainSuffix
                Case "e"
                    chainPrefix = "O-"
                Case "p"
                    chainPrefix = "P-"
            End Select
            Dim totalCarbon = sn1Carbon + sn2Carbon
            Dim totalDB = sn1Double + sn2Double
            Dim totalString = totalCarbon.ToString() & ":" & totalDB.ToString()
            Dim totalName = lipidClass & " " & chainPrefix & totalString

            '
            Dim chainString = sn1ChainString & "/" & sn2ChainString
            Dim lipidName = lipidClass & " " & chainString

            Return New LipidMolecule() With {
    .LipidClass = lbmClass,
    .AnnotationLevel = 2,
    .SublevelLipidName = totalName,
    .LipidName = lipidName,
    .TotalCarbonCount = totalCarbon,
    .TotalDoubleBondCount = totalDB,
    .TotalChainString = totalString,
    .Score = score,
    .Sn1CarbonCount = sn1Carbon,
    .Sn1DoubleBondCount = sn1Double,
    .Sn1AcylChainString = sn1ChainString,
    .Sn2CarbonCount = sn2Carbon,
    .Sn2DoubleBondCount = sn2Double,
    .Sn2AcylChainString = sn2ChainString
}
        End Function


        Public Shared Function JudgeIfPhosphatidylcholine(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sn1Carbon As Integer, sn2Carbon As Integer, sn1Double As Integer, sn2Double As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PC 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            Dim candidates = New List(Of LipidMolecule)()
            Dim Gly_C As Double = {CarbonMass * 8, HydrogenMass * 18, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()
            Dim Gly_O As Double = {CarbonMass * 7, HydrogenMass * 16, NitrogenMass, OxygenMass * 5, PhosphorusMass}.Sum()
            Dim C5H14NO4P As Double = {CarbonMass * 5, HydrogenMass * 14, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()
            Dim C3H9N As Double = {CarbonMass * 3, HydrogenMass * 9, NitrogenMass}.Sum()

            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    If sn1Carbon < 10 OrElse sn2Carbon < 10 Then Return Nothing
                    If sn1Double > 6 OrElse sn2Double > 6 Then Return Nothing
                    '  seek PC diagnostic
                    Dim threshold = 3.0
                    Dim isClassIonFound1 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, C5H14NO4P + Proton, threshold)
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C + Proton, threshold)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O + Proton, threshold)
                    If Not isClassIonFound1 OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing
                    ' reject Na+ adduct
                    Dim diagnosticMz3 = theoreticalMz - C3H9N
                    Dim isNaTypicalFound1 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz3, 10.0)
                    ' reject PE
                    Dim PEHeaderLoss = theoreticalMz - 141.019094261
                    Dim isClassIonFoundPe = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, PEHeaderLoss, 5.0)
                    If isNaTypicalFound1 OrElse isClassIonFoundPe Then
                        Return Nothing
                    End If

                    ' from here, acyl level annotation is executed.
                    Dim nl_SN1 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1Carbon, sn1Double) + HydrogenMass
                    Dim nl_SN1_H2O = nl_SN1 - H2O

                    Dim nl_SN2 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2Carbon, sn2Double) + HydrogenMass
                    Dim nl_SN2_H2O = nl_SN2 - H2O

                    Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                            .Mass = nl_SN1,
                            .Intensity = 0.1
                        },
                                New SpectrumPeak() With {
                            .Mass = nl_SN1_H2O,
                            .Intensity = 0.1
                        },
                                New SpectrumPeak() With {
                            .Mass = nl_SN2,
                            .Intensity = 0.1
                        },
                                New SpectrumPeak() With {
                            .Mass = nl_SN2_H2O,
                            .Intensity = 0.1
                        }
                            }

                    Dim foundCount = 0
                    Dim averageIntensity = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                    If foundCount >= 2 Then ' now I set 2 as the correct level
                        'check Sn2 position spectrum
                        Dim nl_SN1_H2O_CH2 = nl_SN1_H2O - CH2 + HydrogenMass
                        Dim nl_SN2_H2O_CH2 = nl_SN2_H2O - CH2 + HydrogenMass
                        Dim queryCh2 = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                .Mass = nl_SN1_H2O_CH2,
                                .Intensity = 0.5
                            },
                                New SpectrumPeak() With {
                                .Mass = nl_SN2_H2O_CH2,
                                .Intensity = 0.5
                            }
                            }

                        Dim foundCountSN2 = 0
                        Dim averageIntensitySN2 = 0.0
                        LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, queryCh2, ms2Tolerance, foundCountSN2, averageIntensitySN2)
                        If foundCountSN2 > 0 Then
                            If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_SN1_H2O_CH2, nl_SN2_H2O_CH2) Then
                                Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PC", LbmClass.PC, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity) ' use averageIntensity (not averageIntensitySN2)
                                candidates.Add(molecule)
                            Else
                                Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PC", LbmClass.PC, sn2Carbon, sn2Double, sn1Carbon, sn1Double, averageIntensity)
                                candidates.Add(molecule)
                            End If
                        Else
                            Dim molecule = LipidMsmsCharacterizationUtility.getPhospholipidMoleculeObjAsLevel2("PC", LbmClass.PC, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity)
                            candidates.Add(molecule)
                        End If
                    Else
                        Return Nothing
                    End If
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PC", LbmClass.PC, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                    ''' seek 184.07332 (C5H15NO4P)
                    'var diagnosticMz = 184.07332;
                    ' seek [M+Na -C5H14NO4P]+
                    Dim diagnosticMz2 = theoreticalMz - C5H14NO4P
                    ' seek [M+Na -C3H9N]+
                    Dim diagnosticMz3 = theoreticalMz - C3H9N
                    Dim threshold = 3.0
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz2, threshold)
                    Dim isClassIon2Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz3, threshold)
                    If Not isClassIonFound OrElse Not isClassIon2Found Then Return Nothing
                    ' seek PC diagnostic
                    Dim isClassIonFound1 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, C5H14NO4P + Na, threshold)
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C + Na, threshold)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O + Na, threshold)
                    If Not isClassIonFound1 OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing

                    ' from here, acyl level annotation is executed.
                    For sn1CarbonNum = 6 To totalCarbon
                        For sn1DoubleNum = 0 To totalDoubleBond
                            Dim sn2CarbonNum = totalCarbon - sn1CarbonNum
                            Dim sn2DoubleNum = totalDoubleBond - sn1DoubleNum
                            If sn2DoubleNum >= 7 Then Continue For
                            Dim nl_SN1 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1CarbonNum, sn1DoubleNum) + HydrogenMass
                            Dim nl_SN1_H2O = nl_SN1 - H2O

                            Dim nl_SN2 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2CarbonNum, sn2DoubleNum) + HydrogenMass
                            Dim nl_SN2_H2O = nl_SN2 - H2O

                            Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                    .Mass = nl_SN1,
                                    .Intensity = 0.1
                                },
                                New SpectrumPeak() With {
                                    .Mass = nl_SN1_H2O,
                                    .Intensity = 0.1
                                },
                                New SpectrumPeak() With {
                                    .Mass = nl_SN2,
                                    .Intensity = 0.1
                                },
                                New SpectrumPeak() With {
                                    .Mass = nl_SN2_H2O,
                                    .Intensity = 0.1
                                }
                            }

                            Dim foundCount = 0
                            Dim averageIntensity = 0.0
                            LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                            If foundCount >= 2 Then ' now I set 2 as the correct level
                                'check Sn2 position spectrum
                                Dim nl_SN1_H2O_CH2 = nl_SN1_H2O - CH2 + HydrogenMass
                                Dim nl_SN2_H2O_CH2 = nl_SN2_H2O - CH2 + HydrogenMass
                                Dim queryCh2 = New List(Of SpectrumPeak) From {
                                        New SpectrumPeak() With {
                                        .Mass = nl_SN1_H2O_CH2,
                                        .Intensity = 0.5
                                    },
                                        New SpectrumPeak() With {
                                        .Mass = nl_SN2_H2O_CH2,
                                        .Intensity = 0.5
                                    }
                                    }

                                Dim foundCountSN2 = 0
                                Dim averageIntensitySN2 = 0.0
                                LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, queryCh2, ms2Tolerance, foundCountSN2, averageIntensitySN2)
                                If foundCountSN2 > 0 Then
                                    If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_SN1_H2O_CH2, nl_SN2_H2O_CH2) Then
                                        Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PC", LbmClass.PC, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity) ' use averageIntensity (not averageIntensitySN2)
                                        candidates.Add(molecule)
                                    Else
                                        Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PC", LbmClass.PC, sn2CarbonNum, sn2DoubleNum, sn1CarbonNum, sn1DoubleNum, averageIntensity)
                                        candidates.Add(molecule)
                                    End If
                                Else
                                    Dim molecule = LipidMsmsCharacterizationUtility.getPhospholipidMoleculeObjAsLevel2("PC", LbmClass.PC, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity)
                                    candidates.Add(molecule)
                                End If
                            End If
                        Next
                    Next
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PC", LbmClass.PC, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                End If
            End If
            Return Nothing
        End Function

        Public Shared Function JudgeIfPhosphatidylethanolamine(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sn1Carbon As Integer, sn2Carbon As Integer, sn1Double As Integer, sn2Double As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PE 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim Gly_C As Double = {CarbonMass * 5, HydrogenMass * 12, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()

            Dim Gly_O As Double = {CarbonMass * 4, HydrogenMass * 10, NitrogenMass, OxygenMass * 5, PhosphorusMass}.Sum()
            Dim C2H8NO4P As Double = {CarbonMass * 2, HydrogenMass * 8, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()
            Dim C2H5N As Double = {CarbonMass * 2, HydrogenMass * 5, NitrogenMass}.Sum()
            Dim spectrum = msScanProp.Spectrum
            Dim candidates = New List(Of LipidMolecule)()

            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    If sn1Carbon < 10 OrElse sn2Carbon < 10 Then Return Nothing
                    If sn1Double > 6 OrElse sn2Double > 6 Then Return Nothing
                    ' seek -141.019094261 (C2H8NO4P) and PE diagnostic
                    Dim threshold = 2.5
                    Dim diagnosticMz = theoreticalMz - C2H8NO4P
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C + Proton, threshold)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O + Proton, threshold)
                    If Not isClassIonFound OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing

                    ' from here, acyl level annotation is executed.
                    Dim sn1 = LipidMsmsCharacterizationUtility.acylCainMass(sn1Carbon, sn1Double) - Electron
                    Dim sn2 = LipidMsmsCharacterizationUtility.acylCainMass(sn2Carbon, sn2Double) - Electron

                    Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                            .Mass = sn1,
                            .Intensity = 0.1
                        },
                                New SpectrumPeak() With {
                            .Mass = sn2,
                            .Intensity = 0.1
                        }
                            }

                    Dim foundCount = 0
                    Dim averageIntensity = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                    If foundCount = 2 Then ' now I set 2 as the correct level
                        'check Sn2 position spectrum
                        Dim nl_SN1_H2O_CH2 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1Carbon, sn1Double) + HydrogenMass * 2 - H2O - CH2
                        Dim nl_SN2_H2O_CH2 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2Carbon, sn2Double) + HydrogenMass * 2 - H2O - CH2
                        Dim queryCh2 = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                .Mass = nl_SN1_H2O_CH2,
                                .Intensity = 1.0
                            },
                                New SpectrumPeak() With {
                                .Mass = nl_SN2_H2O_CH2,
                                .Intensity = 1.0
                            }
                            }

                        Dim foundCountSN2 = 0
                        Dim averageIntensitySN2 = 0.0
                        LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, queryCh2, ms2Tolerance, foundCountSN2, averageIntensitySN2)
                        If foundCountSN2 > 0 Then
                            If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_SN1_H2O_CH2, nl_SN2_H2O_CH2) Then
                                Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PE", LbmClass.PE, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity)
                                candidates.Add(molecule)
                            Else
                                Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PE", LbmClass.PE, sn2Carbon, sn2Double, sn1Carbon, sn1Double, averageIntensity)
                                candidates.Add(molecule)
                            End If
                        Else
                            Dim molecule = LipidMsmsCharacterizationUtility.getPhospholipidMoleculeObjAsLevel2("PE", LbmClass.PE, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity)
                            candidates.Add(molecule)
                        End If
                    Else
                        Return Nothing
                    End If
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PE", LbmClass.PE, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                    ' seek -141.019094261 (C2H8NO4P)
                    Dim threshold = 3.0
                    Dim diagnosticMz = theoreticalMz - C2H8NO4P
                    ' seek - 43.042199 (C2H5N)
                    'var diagnosticMz2 = theoreticalMz - C2H5N;
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C + Na, threshold)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O + Na, threshold)
                    If Not isClassIonFound OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing

                    ' from here, acyl level annotation is executed.
                    For sn1CarbonNum = 6 To totalCarbon
                        For sn1DoubleNum = 0 To totalDoubleBond
                            Dim sn2CarbonNum = totalCarbon - sn1CarbonNum
                            Dim sn2DoubleNum = totalDoubleBond - sn1DoubleNum
                            If sn2DoubleNum >= 7 Then Continue For

                            Dim nl_SN1 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1CarbonNum, sn1DoubleNum) - HydrogenMass * 2
                            Dim nl_SN1_H2O = nl_SN1 - OxygenMass
                            Dim nl_SN2 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2CarbonNum, sn2DoubleNum) - HydrogenMass * 2
                            Dim nl_SN2_H2O = nl_SN2 - OxygenMass


                            Dim query = New List(Of SpectrumPeak) From {
    New SpectrumPeak() With {
        .Mass = nl_SN1,
        .Intensity = 0.1
    },
    New SpectrumPeak() With {
        .Mass = nl_SN1_H2O,
        .Intensity = 0.1
    },
    New SpectrumPeak() With {
        .Mass = nl_SN2,
        .Intensity = 0.1
    },
    New SpectrumPeak() With {
        .Mass = nl_SN2_H2O,
        .Intensity = 0.1
    }
}
                            Dim foundCount = 0
                            Dim averageIntensity = 0.0
                            LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)
                            If foundCount >= 2 Then
                                'check Sn2 position spectrum
                                Dim nl_SN1_H2O_CH2 = nl_SN1_H2O - CH2 + HydrogenMass
                                Dim nl_SN2_H2O_CH2 = nl_SN2_H2O - CH2 + HydrogenMass
                                Dim queryCh2 = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                        .Mass = nl_SN1_H2O_CH2,
                                        .Intensity = 1.0
                                    },
                                New SpectrumPeak() With {
                                        .Mass = nl_SN2_H2O_CH2,
                                        .Intensity = 1.0
                                    }
                                }

                                Dim foundCountSN2 = 0
                                Dim averageIntensitySN2 = 0.0
                                LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, queryCh2, ms2Tolerance, foundCountSN2, averageIntensitySN2)
                                If foundCountSN2 > 0 Then
                                    If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_SN1_H2O_CH2, nl_SN2_H2O_CH2) Then
                                        Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PE", LbmClass.PE, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity)
                                        candidates.Add(molecule)
                                    Else
                                        Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PE", LbmClass.PE, sn2CarbonNum, sn2DoubleNum, sn1CarbonNum, sn1DoubleNum, averageIntensity)
                                        candidates.Add(molecule)
                                    End If
                                Else
                                    Dim molecule = LipidMsmsCharacterizationUtility.getPhospholipidMoleculeObjAsLevel2("PE", LbmClass.PE, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity)
                                    candidates.Add(molecule)

                                End If
                            End If
                        Next
                    Next
                End If
            End If
            If candidates Is Nothing OrElse candidates.Count = 0 Then Return Nothing

            Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PE", LbmClass.PE, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
        End Function

        Public Shared Function JudgeIfPhosphatidylglycerol(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sn1Carbon As Integer, sn2Carbon As Integer, sn1Double As Integer, sn2Double As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PS 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            Dim candidates = New List(Of LipidMolecule)()
            Dim Gly_C As Double = {CarbonMass * 6, HydrogenMass * 13, OxygenMass * 6, PhosphorusMass}.Sum()

            Dim Gly_O As Double = {CarbonMass * 5, HydrogenMass * 11, OxygenMass * 7, PhosphorusMass}.Sum()
            Dim C3H9O6P As Double = {CarbonMass * 3, HydrogenMass * 9, OxygenMass * 6, PhosphorusMass}.Sum()
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+NH4]+") Then
                    ' seek -189.040227 (C3H8O6P+NH4)
                    Dim threshold = 10.0
                    Dim diagnosticMz = theoreticalMz - C3H9O6P - NH3
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    ' PG diagnostic
                    Dim threshold2 = 5.0
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C + Proton, threshold2)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O + Proton, threshold2)
                    If Not isClassIonFound OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing

                    ' from here, acyl level annotation is executed.
                    Dim nl_SN1 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1Carbon, sn1Double) + HydrogenMass
                    Dim nl_SN1_H2O = nl_SN1 - H2O
                    Dim nl_SN2 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2Carbon, sn2Double) + HydrogenMass
                    Dim nl_SN2_H2O = nl_SN2 - H2O

                    If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_SN1, diagnosticMz) AndAlso LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_SN2, diagnosticMz) Then
                        ' meaning high possibility that the spectrum belongs to BMP
                        Return Nothing
                    End If

                    Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                            .Mass = nl_SN1,
                            .Intensity = 1.0
                        },
                                New SpectrumPeak() With {
                            .Mass = nl_SN1_H2O,
                            .Intensity = 1.0
                        },
                                New SpectrumPeak() With {
                            .Mass = nl_SN2,
                            .Intensity = 1.0
                        },
                                New SpectrumPeak() With {
                            .Mass = nl_SN2_H2O,
                            .Intensity = 1.0
                        }
                            }

                    Dim foundCount = 0
                    Dim averageIntensity = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                    If foundCount >= 2 Then
                        'check Sn2 position spectrum
                        Dim nl_SN1_H2O_CH2 = nl_SN1_H2O - CH2 + HydrogenMass
                        Dim nl_SN2_H2O_CH2 = nl_SN1_H2O - CH2 + HydrogenMass
                        Dim queryCh2 = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                .Mass = nl_SN1_H2O_CH2,
                                .Intensity = 1.0
                            },
                                New SpectrumPeak() With {
                                .Mass = nl_SN2_H2O_CH2,
                                .Intensity = 1.0
                            }
                            }

                        Dim foundCountSN2 = 0
                        Dim averageIntensitySN2 = 0.0
                        LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, queryCh2, ms2Tolerance, foundCountSN2, averageIntensitySN2)
                        If foundCountSN2 > 0 Then
                            If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_SN1_H2O_CH2, nl_SN2_H2O_CH2) Then
                                Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PG", LbmClass.PG, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity)
                                candidates.Add(molecule)
                            Else
                                Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PG", LbmClass.PG, sn2Carbon, sn2Double, sn1Carbon, sn1Double, averageIntensity)
                                candidates.Add(molecule)
                            End If
                        Else
                            Dim molecule = LipidMsmsCharacterizationUtility.getPhospholipidMoleculeObjAsLevel2("PG", LbmClass.PG, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity)
                            candidates.Add(molecule)
                        End If
                    End If
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PG", LbmClass.PG, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                    ' seek header loss
                    Dim threshold = 10.0
                    Dim diagnosticMz = theoreticalMz - C3H9O6P
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    ' PG diagnostic
                    Dim threshold2 = 5.0
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C + Na, threshold2)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O + Na, threshold2)
                    If Not isClassIonFound OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing

                    ' from here, acyl level annotation is executed.
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PG", LbmClass.PG, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                End If
            End If
            Return Nothing
        End Function

        Public Shared Function JudgeIfBismonoacylglycerophosphate(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sn1Carbon As Integer, sn2Carbon As Integer, sn1Double As Integer, sn2Double As Integer, adduct As AdductIon) As LipidMolecule
            Dim C3H9O6P As Double = {CarbonMass * 3, HydrogenMass * 9, OxygenMass * 6, PhosphorusMass}.Sum()
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+NH4]+") Then
                    ' seek -189.040227 (C3H8O6P+NH4)
                    Dim threshold = 5.0
                    Dim diagnosticMz = theoreticalMz - C3H9O6P - NH3
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    If isClassIonFound = False Then Return Nothing

                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()
                    Dim nl_SN1 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1Carbon, sn1Double) + HydrogenMass
                    Dim nl_SN2 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2Carbon, sn2Double) + HydrogenMass

                    Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                            .Mass = nl_SN1,
                            .Intensity = 10
                        },
                                New SpectrumPeak() With {
                            .Mass = nl_SN2,
                            .Intensity = 10
                        }
                            }

                    If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, diagnosticMz, nl_SN1) AndAlso LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, diagnosticMz, nl_SN2) Then
                        ' meaning high possibility that the spectrum belongs to PG
                        Return Nothing
                    End If

                    Dim foundCount = 0
                    Dim averageIntensity = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                    If foundCount = 2 Then
                        Dim molecule = LipidMsmsCharacterizationUtility.getPhospholipidMoleculeObjAsLevel2("BMP", LbmClass.BMP, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity)
                        candidates.Add(molecule)
                    End If
                    If candidates.Count = 0 Then Return Nothing
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("BMP", LbmClass.BMP, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                End If
            End If
            Return Nothing
        End Function
        Public Shared Function JudgeIfPhosphatidylserine(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sn1Carbon As Integer, sn2Carbon As Integer, sn1Double As Integer, sn2Double As Integer, adduct As AdductIon) As LipidMolecule
            Dim Gly_C As Double = {CarbonMass * 6, HydrogenMass * 12, NitrogenMass, OxygenMass * 6, PhosphorusMass}.Sum()

            Dim Gly_O As Double = {CarbonMass * 5, HydrogenMass * 10, NitrogenMass, OxygenMass * 7, PhosphorusMass}.Sum()
            Dim C3H8NO6P As Double = {CarbonMass * 3, HydrogenMass * 8, NitrogenMass, OxygenMass * 6, PhosphorusMass}.Sum()
            Dim spectrum = msScanProp.Spectrum
            Dim candidates = New List(Of LipidMolecule)()
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    ' seek -185.008927 (C3H8NO6P)
                    Dim threshold = 10.0
                    Dim diagnosticMz = theoreticalMz - C3H8NO6P
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    ' PS diagnostic
                    Dim threshold2 = 5.0
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C + Proton, threshold2)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O + Proton, threshold2)
                    If Not isClassIonFound OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing

                    ' from here, acyl level annotation is executed.

                    Dim nl_SN1 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1Carbon, sn1Double) + HydrogenMass
                    Dim nl_SN1_H2O = nl_SN1 - H2O

                    Dim nl_SN2 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2Carbon, sn2Double) + HydrogenMass
                    Dim nl_NS2_H2O = nl_SN2 - H2O

                    Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                            .Mass = nl_SN1,
                            .Intensity = 1.0
                        },
                                New SpectrumPeak() With {
                            .Mass = nl_SN1_H2O,
                            .Intensity = 1.0
                        },
                                New SpectrumPeak() With {
                            .Mass = nl_SN2,
                            .Intensity = 1.0
                        },
                                New SpectrumPeak() With {
                            .Mass = nl_NS2_H2O,
                            .Intensity = 1.0
                        }
                            }

                    Dim foundCount = 0
                    Dim averageIntensity = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                    If foundCount >= 2 Then
                        'check Sn2 position spectrum
                        Dim nl_SN1_H2O_CH2 = nl_SN1 - H2O - CH2 + HydrogenMass
                        Dim nl_SN2_H2O_CH2 = nl_SN2 - H2O - CH2 + HydrogenMass
                        Dim queryCh2 = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                .Mass = nl_SN1_H2O_CH2,
                                .Intensity = 2.0
                            },
                                New SpectrumPeak() With {
                                .Mass = nl_SN2_H2O_CH2,
                                .Intensity = 2.0
                            }
                            }

                        Dim foundCountSN2 = 0
                        Dim averageIntensitySN2 = 0.0
                        LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, queryCh2, ms2Tolerance, foundCountSN2, averageIntensitySN2)
                        If foundCountSN2 > 0 Then
                            If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_SN1_H2O_CH2, nl_SN2_H2O_CH2) Then
                                Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PS", LbmClass.PS, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity)
                                candidates.Add(molecule)
                            Else
                                Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PS", LbmClass.PS, sn2Carbon, sn2Double, sn1Carbon, sn1Double, averageIntensity)
                                candidates.Add(molecule)
                            End If
                        Else
                            Dim molecule = LipidMsmsCharacterizationUtility.getPhospholipidMoleculeObjAsLevel2("PS", LbmClass.PS, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity)
                            candidates.Add(molecule)
                        End If
                    End If
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PS", LbmClass.PS, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                    ' seek -185.008927 (C3H8NO6P)
                    Dim threshold = 3.0
                    Dim diagnosticMz = theoreticalMz - C3H8NO6P
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    ' PS diagnostic
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C + Na, threshold)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O + Na, threshold)
                    If Not isClassIonFound OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing

                    ' from here, acyl level annotation is executed.
                    For sn1CarbonNum = 6 To totalCarbon
                        For sn1DoubleNum = 0 To totalDoubleBond
                            Dim sn2CarbonNum = totalCarbon - sn1CarbonNum
                            Dim sn2DoubleNum = totalDoubleBond - sn1DoubleNum
                            If sn2DoubleNum >= 7 Then Continue For

                            Dim nl_SN1 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1CarbonNum, sn1DoubleNum) + HydrogenMass
                            Dim nl_SN1_H2O = nl_SN1 - OxygenMass - HydrogenMass
                            Dim nl_SN2 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2CarbonNum, sn2DoubleNum) + HydrogenMass
                            Dim nl_SN2_H2O = nl_SN2 - OxygenMass - HydrogenMass


                            Dim query = New List(Of SpectrumPeak) From {
    New SpectrumPeak() With {
        .Mass = nl_SN1,
        .Intensity = 0.1
    },
    New SpectrumPeak() With {
        .Mass = nl_SN1_H2O,
        .Intensity = 0.1
    },
    New SpectrumPeak() With {
        .Mass = nl_SN2,
        .Intensity = 0.1
    },
    New SpectrumPeak() With {
        .Mass = nl_SN2_H2O,
        .Intensity = 0.1
    }
}
                            Dim foundCount = 0
                            Dim averageIntensity = 0.0
                            LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)
                            If foundCount >= 2 Then
                                'check Sn2 position spectrum
                                Dim nl_SN1_H2O_CH2 = nl_SN1_H2O - CH2
                                Dim nl_SN2_H2O_CH2 = nl_SN2_H2O - CH2
                                Dim queryCh2 = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                        .Mass = nl_SN1_H2O_CH2,
                                        .Intensity = 0.1
                                    },
                                New SpectrumPeak() With {
                                        .Mass = nl_SN2_H2O_CH2,
                                        .Intensity = 0.1
                                    }
                            }

                                Dim foundCountSN2 = 0
                                Dim averageIntensitySN2 = 0.0
                                LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, queryCh2, ms2Tolerance, foundCountSN2, averageIntensitySN2)
                                If foundCountSN2 > 0 Then
                                    If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_SN1_H2O_CH2, nl_SN2_H2O_CH2) Then
                                        Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PS", LbmClass.PS, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity)
                                        candidates.Add(molecule)
                                    Else
                                        Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PS", LbmClass.PS, sn2CarbonNum, sn2DoubleNum, sn1CarbonNum, sn1DoubleNum, averageIntensity)
                                        candidates.Add(molecule)
                                    End If
                                Else
                                    Dim molecule = LipidMsmsCharacterizationUtility.getPhospholipidMoleculeObjAsLevel2("PS", LbmClass.PS, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity)
                                    candidates.Add(molecule)

                                End If
                            End If
                        Next
                    Next
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PS", LbmClass.PS, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                End If
            End If
            Return Nothing
        End Function
        Public Shared Function JudgeIfPhosphatidylinositol(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sn1Carbon As Integer, sn2Carbon As Integer, sn1Double As Integer, sn2Double As Integer, adduct As AdductIon) As LipidMolecule
            Dim Gly_C As Double = {CarbonMass * 9, HydrogenMass * 17, OxygenMass * 9, PhosphorusMass}.Sum()
            Dim Gly_O As Double = {CarbonMass * 8, HydrogenMass * 15, OxygenMass * 10, PhosphorusMass}.Sum()
            Dim C6H13O9P As Double = {CarbonMass * 6, HydrogenMass * 13, OxygenMass * 9, PhosphorusMass}.Sum()
            Dim C6H10O5 As Double = {CarbonMass * 6, HydrogenMass * 10, OxygenMass * 5}.Sum()
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+NH4]+") Then
                    ' seek -277.056272 (C6H12O9P+NH4)
                    Dim threshold = 10.0
                    Dim diagnosticMz = theoreticalMz - C6H13O9P - NH3
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    ' PI diagnostic
                    Dim threshold2 = 5.0
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C + Proton, threshold2)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O + Proton, threshold2)
                    If Not isClassIonFound OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing

                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()
                    For sn1CarbonNum = 6 To totalCarbon
                        For sn1DoubleNum = 0 To totalDoubleBond
                            Dim sn2CarbonNum = totalCarbon - sn1CarbonNum
                            Dim sn2DoubleNum = totalDoubleBond - sn1DoubleNum

                            Dim nl_SN1 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1CarbonNum, sn1DoubleNum) + HydrogenMass
                            Dim nl_SN1_H2O = nl_SN1 - H2O
                            Dim nl_SN2 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2CarbonNum, sn2DoubleNum) + HydrogenMass
                            Dim nl_SN2_H2O = nl_SN2 - H2O

                            Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                    .Mass = nl_SN1,
                                    .Intensity = 0.1
                                },
                                New SpectrumPeak() With {
                                    .Mass = nl_SN1_H2O,
                                    .Intensity = 0.1
                                },
                                New SpectrumPeak() With {
                                    .Mass = nl_SN2,
                                    .Intensity = 0.1
                                },
                                New SpectrumPeak() With {
                                    .Mass = nl_SN2_H2O,
                                    .Intensity = 0.1
                                }
                            }

                            Dim foundCount = 0
                            Dim averageIntensity = 0.0
                            LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                            If foundCount >= 2 Then
                                'check Sn2 position spectrum
                                Dim nl_SN1_H2O_CH2 = nl_SN1_H2O - CH2
                                Dim nl_SN2_H2O_CH2 = nl_SN2_H2O - CH2
                                Dim queryCh2 = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                        .Mass = nl_SN1_H2O_CH2,
                                        .Intensity = 1.0
                                    },
                                New SpectrumPeak() With {
                                        .Mass = nl_SN2_H2O_CH2,
                                        .Intensity = 1.0
                                    }
                            }

                                Dim foundCountSN2 = 0
                                Dim averageIntensitySN2 = 0.0
                                LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, queryCh2, ms2Tolerance, foundCountSN2, averageIntensitySN2)
                                If foundCountSN2 > 0 Then
                                    If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_SN1_H2O_CH2, nl_SN2_H2O_CH2) Then
                                        Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PI", LbmClass.PI, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity)
                                        candidates.Add(molecule)
                                    Else
                                        Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PI", LbmClass.PI, sn2CarbonNum, sn2DoubleNum, sn1CarbonNum, sn1DoubleNum, averageIntensity)
                                        candidates.Add(molecule)
                                    End If
                                Else
                                    Dim molecule = LipidMsmsCharacterizationUtility.getPhospholipidMoleculeObjAsLevel2("PI", LbmClass.PI, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity)
                                    candidates.Add(molecule)
                                End If
                            End If
                        Next
                    Next

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PI", LbmClass.PI, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                    ' seek -C6H10O5
                    Dim threshold = 5.0
                    Dim diagnosticMz = theoreticalMz - C6H10O5
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    ' PI diagnostic
                    Dim threshold2 = 5.0
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C + Na, threshold2)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O + Na, threshold2)
                    If Not isClassIonFound OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing

                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()
                    For sn1CarbonNum = 6 To totalCarbon
                        For sn1DoubleNum = 0 To totalDoubleBond
                            Dim sn2CarbonNum = totalCarbon - sn1CarbonNum
                            Dim sn2DoubleNum = totalDoubleBond - sn1DoubleNum

                            Dim nl_SN1 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1CarbonNum, sn1DoubleNum) + HydrogenMass
                            Dim nl_SN1_H2O = nl_SN1 - H2O
                            Dim nl_SN2 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2CarbonNum, sn2DoubleNum) + HydrogenMass
                            Dim nl_SN2_H2O = nl_SN2 - H2O

                            Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                    .Mass = nl_SN1,
                                    .Intensity = 0.1
                                },
                                New SpectrumPeak() With {
                                    .Mass = nl_SN1_H2O,
                                    .Intensity = 0.1
                                },
                                New SpectrumPeak() With {
                                    .Mass = nl_SN2,
                                    .Intensity = 0.1
                                },
                                New SpectrumPeak() With {
                                    .Mass = nl_SN2_H2O,
                                    .Intensity = 0.1
                                }
                            }

                            Dim foundCount = 0
                            Dim averageIntensity = 0.0
                            LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                            If foundCount >= 2 Then
                                'check Sn2 position spectrum
                                Dim nl_SN1_H2O_CH2 = nl_SN1_H2O - CH2 + HydrogenMass
                                Dim nl_SN2_H2O_CH2 = nl_SN2_H2O - CH2 + HydrogenMass
                                Dim queryCh2 = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                        .Mass = nl_SN1_H2O_CH2,
                                        .Intensity = 1.0
                                    },
                                New SpectrumPeak() With {
                                        .Mass = nl_SN2_H2O_CH2,
                                        .Intensity = 1.0
                                    }
                                    }

                                Dim foundCountSN2 = 0
                                Dim averageIntensitySN2 = 0.0
                                LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, queryCh2, ms2Tolerance, foundCountSN2, averageIntensitySN2)
                                If foundCountSN2 > 0 Then
                                    If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_SN1_H2O_CH2, nl_SN2_H2O_CH2) Then
                                        Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PI", LbmClass.PI, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity)
                                        candidates.Add(molecule)
                                    Else
                                        Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PI", LbmClass.PI, sn2CarbonNum, sn2DoubleNum, sn1CarbonNum, sn1DoubleNum, averageIntensity)
                                        candidates.Add(molecule)
                                    End If
                                Else
                                    Dim molecule = LipidMsmsCharacterizationUtility.getPhospholipidMoleculeObjAsLevel2("PI", LbmClass.PI, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity)
                                    candidates.Add(molecule)
                                End If
                            End If
                        Next
                    Next

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PI", LbmClass.PI, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                End If
            End If
            Return Nothing
        End Function

        Public Shared Function JudgeIfLysopc(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, snCarbon As Integer, snDoubleBond As Integer, adduct As AdductIon) As LipidMolecule ' 
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If snCarbon > totalCarbon Then snCarbon = totalCarbon
            If snDoubleBond > totalDoubleBond Then snDoubleBond = totalDoubleBond
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    If totalCarbon > 28 Then Return Nothing '  currently carbon > 28 is recognized as EtherPC
                    ' seek 184.07332 (C5H15NO4P)
                    Dim threshold = 3.0
                    Dim diagnosticMz = 184.07332
                    Dim diagnosticMz2 = 104.106990
                    Dim isClassIon1Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    If isClassIon1Found <> True Then Return Nothing
                    ' reject Na+ adduct
                    Dim diagnosticMz3 = theoreticalMz - 59.0735
                    Dim isNaTypicalFound1 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz3, 10.0)
                    If isNaTypicalFound1 Then
                        Return Nothing
                    End If

                    ' for eieio
                    Dim PEHeaderLoss = theoreticalMz - 141.019094261 + ProtonMass
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, PEHeaderLoss, 3.0)
                    If isClassIonFound2 AndAlso LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, PEHeaderLoss, diagnosticMz) Then
                        Return Nothing
                    End If

                    Dim candidates = New List(Of LipidMolecule)()
                    Dim chainSuffix = ""
                    Dim diagnosticMzExist = 0.0
                    Dim diagnosticMzIntensity = 0.0
                    Dim diagnosticMzExist2 = 0.0
                    Dim diagnosticMzIntensity2 = 0.0

                    For i = 0 To spectrum.Count - 1
                        Dim mz = spectrum(i).Mass
                        Dim intensity = spectrum(i).Intensity

                        If intensity > threshold AndAlso Math.Abs(mz - diagnosticMz) < ms2Tolerance Then
                            diagnosticMzExist = mz
                            diagnosticMzIntensity = intensity
                        ElseIf intensity > threshold AndAlso Math.Abs(mz - diagnosticMz2) < ms2Tolerance Then
                            diagnosticMzExist2 = mz
                            diagnosticMzIntensity2 = intensity
                        End If
                    Next

                    If diagnosticMzIntensity2 / diagnosticMzIntensity > 0.3 Then '
                        chainSuffix = "/0:0"
                    End If

                    Dim score = 0.0
                    If totalCarbon < 30 Then score = score + 1.0
                    Dim molecule = LipidMsmsCharacterizationUtility.getSingleacylchainwithsuffixMoleculeObjAsLevel2("LPC", LbmClass.LPC, totalCarbon, totalDoubleBond, score, chainSuffix)
                    candidates.Add(molecule)

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LPC", LbmClass.LPC, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                    If totalCarbon > 28 Then Return Nothing '  currently carbon > 28 is recognized as EtherPC
                    ' seek PreCursor - 59 (C3H9N)
                    Dim threshold = 3.0
                    Dim diagnosticMz = theoreticalMz - 59.072951
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    If isClassIonFound = False Then Return Nothing

                    Dim candidates = New List(Of LipidMolecule)()
                    Dim score = 0.0
                    If totalCarbon < 30 Then score = score + 1.0
                    Dim molecule = LipidMsmsCharacterizationUtility.getSingleacylchainMoleculeObjAsLevel2("LPC", LbmClass.LPC, totalCarbon, totalDoubleBond, score)
                    candidates.Add(molecule)

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LPC", LbmClass.LPC, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                End If
            End If

            Return Nothing
        End Function

        Public Shared Function JudgeIfLysope(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sn1Carbon As Integer, sn1Double As Integer, adduct As AdductIon) As LipidMolecule ' 
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    If totalCarbon > 28 Then Return Nothing '  currently carbon > 28 is recognized as EtherPE
                    Dim Gly_C As Double = {CarbonMass * 5, HydrogenMass * 12, NitrogenMass, OxygenMass * 4, PhosphorusMass, Proton}.Sum()

                    Dim Gly_O As Double = {CarbonMass * 4, HydrogenMass * 10, NitrogenMass, OxygenMass * 5, PhosphorusMass, Proton}.Sum()
                    Dim C2H8NO4P As Double = {CarbonMass * 2, HydrogenMass * 8, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()
                    ' seek -141.019094261 (C2H8NO4P)
                    Dim threshold = 2.5
                    Dim diagnosticMz = theoreticalMz - C2H8NO4P
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C, threshold)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O, threshold)
                    If Not isClassIonFound OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing

                    ''' seek PreCursor -141(C2H8NO4P)
                    'var threshold = 2.5;
                    'var diagnosticMz = theoreticalMz - 141.019094;

                    'var isClassIon1Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold);
                    'if (isClassIon1Found == false) return null;
                    Dim sn1alkyl = CarbonMass * sn1Carbon + HydrogenMass * (sn1Carbon * 2 - sn1Double * 2 + 1) 'sn1(ether)

                    Dim NL_sn1 = diagnosticMz - sn1alkyl + Proton
                    Dim sn1_rearrange = sn1alkyl + HydrogenMass * 2 + 139.00290 'sn1(ether) + C2H5NO4P + proton 

                    ' reject EtherPE 
                    Dim isClassIon2Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, NL_sn1, threshold)
                    Dim isClassIon3Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, sn1_rearrange, threshold)
                    If isClassIon2Found = True OrElse isClassIon3Found = True Then Return Nothing

                    Dim candidates = New List(Of LipidMolecule)()
                    If totalCarbon > 30 Then
                        Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PE", LbmClass.EtherPE, "e", theoreticalMz, adduct, totalCarbon, totalDoubleBond + 1, 0, candidates, 1)
                    Else
                        Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LPE", LbmClass.LPE, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                    End If
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                    ' seek PreCursor -141(C2H8NO4P)
                    Dim threshold = 3.0
                    Dim diagnosticMz = theoreticalMz - 141.019094
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    If isClassIonFound = False Then Return Nothing

                    ' reject EtherPE 
                    Dim sn1alkyl = CarbonMass * sn1Carbon + HydrogenMass * (sn1Carbon * 2 - sn1Double * 2 + 1) 'sn1(ether)

                    Dim NL_sn1 = diagnosticMz - sn1alkyl + Proton
                    Dim sn1_rearrange = sn1alkyl + 139.00290 + HydrogenMass * 2 'sn1(ether) + C2H5NO4P + proton 

                    Dim isClassIon2Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, NL_sn1, threshold)
                    Dim isClassIon3Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, sn1_rearrange, threshold)
                    If isClassIon2Found = True OrElse isClassIon3Found = True Then Return Nothing

                    Dim candidates = New List(Of LipidMolecule)()
                    If totalCarbon > 30 Then
                        Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PE", LbmClass.EtherPE, "e", theoreticalMz, adduct, totalCarbon, totalDoubleBond + 1, 0, candidates, 2)
                    Else
                        Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LPE", LbmClass.LPE, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                    End If
                End If
            End If

            Return Nothing
        End Function
        Public Shared Function JudgeIfLysopg(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, snCarbon As Integer, snDoubleBond As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PE 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Negative Then '
                If Equals(adduct.AdductIonName, "[M-H]-") Then
                    ' seek C3H6O5P-
                    Dim diagnosticMz1 = 152.99583  ' seek C3H6O5P-
                    Dim threshold1 = 1.0
                    Dim diagnosticMz2 = LipidMsmsCharacterizationUtility.fattyacidProductIon(totalCarbon, totalDoubleBond) ' seek [FA-H]-
                    Dim threshold2 = 10.0
                    Dim isClassIon1Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz1, threshold1)
                    Dim isClassIon2Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz2, threshold2)
                    If isClassIon1Found <> True OrElse isClassIon2Found <> True Then Return Nothing

                    '
                    Dim candidates = New List(Of LipidMolecule)()
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LPG", LbmClass.LPG, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                End If
            ElseIf adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    ' seek Header loss (MG+ + chain Acyl) 
                    Dim threshold = 5.0
                    Dim diagnosticMz = LipidMsmsCharacterizationUtility.acylCainMass(snCarbon, snDoubleBond) + (12 * 3 + HydrogenMass * 5 + OxygenMass * 2) + ProtonMass
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    If isClassIonFound = False Then Return Nothing

                    Dim candidates = New List(Of LipidMolecule)()
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LPG", LbmClass.LPG, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                End If
            End If
            Return Nothing
        End Function

        Public Shared Function JudgeIfLysopi(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, snCarbon As Integer, snDoubleBond As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PE 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Negative Then 'negative ion mode only
                If Equals(adduct.AdductIonName, "[M-H]-") Then
                    ' seek C3H6O5P-
                    Dim diagnosticMz1 = 241.0118806 + Electron  ' seek C3H6O5P-
                    Dim threshold1 = 1.0
                    Dim diagnosticMz2 = 315.048656 ' seek C9H16O10P-
                    Dim threshold2 = 1.0
                    Dim diagnosticMz3 = LipidMsmsCharacterizationUtility.fattyacidProductIon(totalCarbon, totalDoubleBond) ' seek [FA-H]-
                    Dim threshold3 = 10.0
                    Dim isClassIon1Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz1, threshold1)
                    Dim isClassIon2Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz2, threshold2)
                    Dim isClassIon3Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz3, threshold3)
                    If isClassIon1Found <> True OrElse isClassIon2Found <> True OrElse isClassIon3Found <> True Then Return Nothing

                    '
                    Dim candidates = New List(Of LipidMolecule)()
                    'var averageIntensity = 0.0;

                    'var molecule = getSingleacylchainMoleculeObjAsLevel2("LPI", LbmClass.LPI, totalCarbon, totalDoubleBond,
                    'averageIntensity);
                    'candidates.Add(molecule);
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LPI", LbmClass.LPI, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                End If
            ElseIf adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    ' seek Header loss (MG+ + chain Acyl) 
                    Dim threshold = 5.0
                    Dim diagnosticMz = LipidMsmsCharacterizationUtility.acylCainMass(snCarbon, snDoubleBond) + (12 * 3 + HydrogenMass * 5 + OxygenMass * 2) + ProtonMass
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    If isClassIonFound = False Then Return Nothing

                    Dim candidates = New List(Of LipidMolecule)()
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LPI", LbmClass.LPI, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                End If
            End If
            Return Nothing
        End Function
        Public Shared Function JudgeIfLysops(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, snCarbon As Integer, snDoubleBond As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PE 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Negative Then 'negative ion mode only
                If Equals(adduct.AdductIonName, "[M-H]-") Then
                    ' seek C3H6O5P-

                    Dim diagnosticMz1 = 152.99583  ' seek C3H6O5P-
                    Dim threshold1 = 10.0
                    Dim diagnosticMz2 = theoreticalMz - 87.032029 ' seek -C3H6NO2-H
                    Dim threshold2 = 5.0
                    Dim isClassIon1Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz1, threshold1)
                    Dim isClassIon2Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz2, threshold2)
                    If isClassIon1Found <> True OrElse isClassIon2Found <> True Then Return Nothing

                    '
                    Dim candidates = New List(Of LipidMolecule)()
                    'var averageIntensity = 0.0;

                    'var molecule = getSingleacylchainMoleculeObjAsLevel2("LPS", LbmClass.LPS, totalCarbon, totalDoubleBond,
                    'averageIntensity);
                    'candidates.Add(molecule);
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LPS", LbmClass.LPS, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                End If
            ElseIf adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    ' seek Header loss (MG+ + chain Acyl) 
                    Dim threshold = 5.0
                    Dim diagnosticMz = LipidMsmsCharacterizationUtility.acylCainMass(snCarbon, snDoubleBond) + (12 * 3 + HydrogenMass * 5 + OxygenMass * 2) + ProtonMass
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    If isClassIonFound = False Then Return Nothing

                    Dim candidates = New List(Of LipidMolecule)()
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LPS", LbmClass.LPS, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                End If
            End If
            Return Nothing
        End Function

        Public Shared Function JudgeIfSphingomyelin(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sphCarbon As Integer, acylCarbon As Integer, sphDouble As Integer, acylDouble As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PC 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            Dim C5H14NO4P As Double = {CarbonMass * 5, HydrogenMass * 14, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()
            Dim C2H2N = 12 * 2 + HydrogenMass * 2 + NitrogenMass
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    ' seek 184.07332 (C5H15NO4P+)
                    Dim threshold = 10.0
                    Dim diagnosticMz = C5H14NO4P + Proton
                    Dim diagnosticMz1 = C5H14NO4P + Proton + 12 * 2 + HydrogenMass * 3 + NitrogenMass
                    Dim diagnosticMz2 = C5H14NO4P + Proton + 12 * 3 + HydrogenMass * 3 + NitrogenMass + OxygenMass
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    Dim isClassIonFound1 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz1, 3.0)
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz2, 3.0)
                    If Not isClassIonFound OrElse Not isClassIonFound1 OrElse Not isClassIonFound2 Then Return Nothing
                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()
                    If sphCarbon <= 13 Then Return Nothing
                    If sphCarbon = 16 AndAlso sphDouble >= 3 Then Return Nothing
                    If acylCarbon < 8 Then Return Nothing

                    Dim diagnosChain1 = LipidMsmsCharacterizationUtility.acylCainMass(acylCarbon, acylDouble) + C2H2N + Proton
                    Dim diagnosChain2 = diagnosChain1 + C5H14NO4P + Proton - HydrogenMass

                    Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                            .Mass = diagnosChain1,
                            .Intensity = 0.5
                        },
                                New SpectrumPeak() With {
                            .Mass = diagnosChain2,
                            .Intensity = 1.0
                        }
                            }

                    Dim foundCount = 0
                    Dim averageIntensity = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                    If foundCount = 2 Then ' the diagnostic acyl ion must be observed for level 2 annotation
                        Dim molecule = LipidMsmsCharacterizationUtility.getCeramideMoleculeObjAsLevel2("SM", LbmClass.SM, "d", sphCarbon, sphDouble, acylCarbon, acylDouble, averageIntensity)
                        candidates.Add(molecule)
                    Else
                        Return Nothing
                    End If

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("SM", LbmClass.SM, "d", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                    ' seek -59.0735 [M-C3H9N+Na]+
                    Dim threshold = 20.0
                    Dim diagnosticMz = theoreticalMz - 59.0735
                    ' seek C5H15NO4P + Na+
                    Dim threshold2 = 30.0
                    Dim diagnosticMz2 = C5H14NO4P + Na

                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    Dim isClassIon2Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz2, threshold2)
                    If isClassIonFound = False OrElse isClassIon2Found = False Then Return Nothing

                    Dim candidates = New List(Of LipidMolecule)()
                    ' from here, acyl level annotation is executed.
                    For sphCarbonNum = 6 To totalCarbon
                        For sphDoubleNum = 0 To totalDoubleBond
                            Dim acylCarbonNum = totalCarbon - sphCarbonNum
                            Dim acylDoubleNum = totalDoubleBond - sphDoubleNum
                            If acylDoubleNum >= 7 Then Continue For

                            Dim diagnosChain1 = LipidMsmsCharacterizationUtility.acylCainMass(acylCarbonNum, acylDoubleNum) + C2H2N + HydrogenMass + Na
                            Dim diagnosChain2 = diagnosChain1 + C5H14NO4P - HydrogenMass

                            Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                    .Mass = diagnosChain1,
                                    .Intensity = 20.0
                                },
                                New SpectrumPeak() With {
                                    .Mass = diagnosChain2,
                                    .Intensity = 20.0
                                }
                            }
                            Dim foundCount = 0
                            Dim averageIntensity = 0.0
                            LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)
                            If foundCount = 2 Then
                                Dim molecule = LipidMsmsCharacterizationUtility.getCeramideMoleculeObjAsLevel2("SM", LbmClass.SM, "d", sphCarbonNum, sphDoubleNum, acylCarbonNum, acylDoubleNum, averageIntensity)
                                candidates.Add(molecule)
                            End If
                        Next
                    Next
                    If candidates Is Nothing OrElse candidates.Count = 0 Then Return Nothing

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("SM", LbmClass.SM, "d", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                End If
            End If
            Return Nothing
        End Function

        Public Shared Function JudgeIfAcylsm(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sphCarbon As Integer, extCarbon As Integer, sphDouble As Integer, extDouble As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PC 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing

            If adduct.IonMode = IonMode.Positive Then
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    Dim threshold1 = 1.0
                    Dim diagnosticMz1 = 184.073
                    Dim isClassIon1Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz1, threshold1)
                    If isClassIon1Found <> True Then Return Nothing
                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()
                    Dim acylCarbon = totalCarbon - sphCarbon - extCarbon
                    Dim acylDouble = totalDoubleBond - sphDouble - extDouble

                    Dim extAcylloss = theoreticalMz - LipidMsmsCharacterizationUtility.fattyacidProductIon(extCarbon, extDouble) - HydrogenMass + Electron  ' 
                    Dim query = New List(Of SpectrumPeak) From {
                                        New SpectrumPeak() With {
                            .Mass = extAcylloss,
                            .Intensity = 1
                        }
                                    }

                    Dim foundCount = 0
                    Dim averageIntensity = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                    If foundCount = 1 Then ' 
                        Dim molecule = LipidMsmsCharacterizationUtility.getAsmMoleculeObjAsLevel2_0("SM", LbmClass.ASM, "d", sphCarbon + acylCarbon, sphDouble + acylDouble, extCarbon, extDouble, averageIntensity)
                        candidates.Add(molecule)
                    End If

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("SM", LbmClass.ASM, "d", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 3)
                End If
            End If
            Return Nothing
        End Function

        Public Shared Function JudgeIfAcylcarnitine(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Single, totalCarbon As Integer, totalDoubleBond As Integer, adduct As AdductIon) As LipidMolecule
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M]+") Then
                    ' seek 85.028405821 (C4H5O2+)
                    Dim threshold = 1.0
                    Dim diagnosticMz = 85.028405821
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)

                    ' seek -CHO2
                    Dim diagnosticMz1 = theoreticalMz - (CarbonMass + HydrogenMass + OxygenMass * 2)
                    Dim isClassIonFound1 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz1, threshold)

                    ' seek Acyl loss
                    Dim acyl = LipidMsmsCharacterizationUtility.acylCainMass(totalCarbon, totalDoubleBond)
                    Dim diagnosticMz2 = theoreticalMz - acyl + ProtonMass
                    Dim threshold2 = 0.01
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz2, threshold2)

                    ' seek 144.1019 (Acyl and H2O loss) // not found at PasefOn case
                    Dim diagnosticMz3 = diagnosticMz2 - H2O
                    Dim threshold3 = 0.01
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz3, threshold3)

                    If Not isClassIonFound AndAlso Not isClassIonFound1 Then Return Nothing
                    If Not isClassIonFound2 AndAlso Not isClassIonFound3 Then Return Nothing

                    Dim candidates = New List(Of LipidMolecule)()
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("CAR", LbmClass.CAR, String.Empty, theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                End If
            End If
            Return Nothing
        End Function

        Public Shared Function JudgeIfEtherpe(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, Sn1Carbon As Integer, Sn2Carbon As Integer, Sn1Double As Integer, Sn2Double As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PE 46:6, totalCarbon = 46 and totalDoubleBond = 6
            If totalCarbon <= 28 Then Return Nothing ' currently carbon <= 28 is recognized as Lyso PE
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    Dim Gly_C As Double = {CarbonMass * 5, HydrogenMass * 12, NitrogenMass, OxygenMass * 4, PhosphorusMass, Proton}.Sum()

                    Dim Gly_O As Double = {CarbonMass * 4, HydrogenMass * 10, NitrogenMass, OxygenMass * 5, PhosphorusMass, Proton}.Sum()
                    Dim C2H8NO4P As Double = {CarbonMass * 2, HydrogenMass * 8, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()
                    ' seek -141.019094261 (C2H8NO4P)
                    Dim threshold = 2.5
                    Dim diagnosticMz = theoreticalMz - C2H8NO4P
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C, threshold)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O, threshold)
                    If Not isClassIonFound OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing

                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()
                    If Sn1Double >= 5 Then Return Nothing
                    Dim sn1alkyl = CarbonMass * Sn1Carbon + HydrogenMass * (Sn1Carbon * 2 - Sn1Double * 2 + 1) '(Ether chain)

                    Dim NL_sn1 = theoreticalMz - sn1alkyl - OxygenMass
                    Dim NL_sn2 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(Sn2Carbon, Sn2Double) - H2O
                    Dim sn1_rearrange = sn1alkyl + C2H8NO4P 'sn1(carbon chain) + C2H8NO4P 

                    Dim query = New List(Of SpectrumPeak) From {
                                    New SpectrumPeak() With {
                            .Mass = NL_sn1,
                            .Intensity = 5.0
                        },
                                    New SpectrumPeak() With {
                            .Mass = NL_sn2,
                            .Intensity = 5.0
                        },
                                    New SpectrumPeak() With {
                            .Mass = sn1_rearrange,
                            .Intensity = 10.0
                        }
                                }

                    Dim foundCount = 0
                    Dim averageIntensity = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                    If foundCount >= 2 Then
                        Dim etherSuffix = "e"
                        Dim sn1Double2 = Sn1Double
                        Dim EtherString = "O-" & Sn1Carbon.ToString() & ":" & Sn1Double.ToString()
                        Dim EsterString = Sn2Carbon.ToString() & ":" & Sn2Double.ToString()
                        If LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, sn1_rearrange, 5.0) Then
                            sn1Double2 = Sn1Double - 1
                            etherSuffix = "p"
                            EtherString = "P-" & Sn1Carbon.ToString() & ":" & sn1Double2.ToString()
                        End If

                        Dim nl_SN1_H2O_CH2 = NL_sn1 - CH2
                        Dim nl_SN2_H2O_CH2 = NL_sn2 - CH2 + HydrogenMass * 2
                        Dim queryCh2 = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                .Mass = nl_SN1_H2O_CH2,
                                .Intensity = 2.0
                            },
                                New SpectrumPeak() With {
                                .Mass = nl_SN2_H2O_CH2,
                                .Intensity = 2.0
                            }
                            }

                        Dim foundCountSN2 = 0
                        Dim averageIntensitySN2 = 0.0
                        LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, queryCh2, ms2Tolerance, foundCountSN2, averageIntensitySN2)
                        If foundCountSN2 > 0 Then
                            If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_SN1_H2O_CH2, nl_SN2_H2O_CH2) Then
                                Dim molecule = getEtherPhospholipidMoleculeObjAsPositionChain("PE", LbmClass.EtherPE, Sn1Carbon, sn1Double2, EtherString, Sn2Carbon, Sn2Double, EsterString, averageIntensity, etherSuffix) ' use averageIntensity (not averageIntensitySN2)
                                candidates.Add(molecule)
                            Else
                                Dim molecule = getEtherPhospholipidMoleculeObjAsPositionChain("PE", LbmClass.EtherPE, Sn2Carbon, Sn2Double, EsterString, Sn1Carbon, sn1Double2, EtherString, averageIntensity, etherSuffix)
                                candidates.Add(molecule)
                            End If
                        Else
                            Dim molecule = LipidMsmsCharacterizationUtility.getEtherPhospholipidMoleculeObjAsLevel2("PE", LbmClass.EtherPE, Sn1Carbon, sn1Double2, Sn2Carbon, Sn2Double, averageIntensity, etherSuffix)
                            candidates.Add(molecule)
                        End If
                    End If
                    If candidates.Count = 0 Then Return Nothing
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PE", LbmClass.EtherPE, "e", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                End If
            Else
                If Equals(adduct.AdductIonName, "[M-H]-") Then
                    ' seek C5H11NO5P-
                    Dim threshold = 5.0
                    Dim diagnosticMz = 196.03803
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)

                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()
                    If Sn1Carbon >= 24 AndAlso Sn1Double >= 5 Then Return Nothing

                    Dim sn2 = LipidMsmsCharacterizationUtility.fattyacidProductIon(Sn2Carbon, Sn2Double)
                    Dim NL_sn2 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(Sn2Carbon, Sn2Double) + HydrogenMass

                    Dim query = New List(Of SpectrumPeak) From {
                            New SpectrumPeak() With {
                            .Mass = sn2,
                            .Intensity = 10.0
                        },
                            New SpectrumPeak() With {
                            .Mass = NL_sn2,
                            .Intensity = 0.1
                        }
                        }

                    Dim foundCount = 0
                    Dim averageIntensity = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                    If foundCount = 2 Then ' now I set 2 as the correct level
                        Dim molecule = LipidMsmsCharacterizationUtility.getEtherPhospholipidMoleculeObjAsLevel2("PE", LbmClass.EtherPE, Sn1Carbon, Sn1Double, Sn2Carbon, Sn2Double, averageIntensity, "e")
                        candidates.Add(molecule)
                    End If

                    If candidates.Count = 0 Then Return Nothing
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PE", LbmClass.EtherPE, "e", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                End If
            End If
            Return Nothing
        End Function
        Public Shared Function JudgeIfShexcer(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sphCarbon As Integer, acylCarbon As Integer, sphDouble As Integer, acylDouble As Integer, adduct As AdductIon, totalOxidized As Integer) As LipidMolecule ' If the candidate PC 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+NH4]+") Then
                    Dim diagnosticMz = If(Equals(adduct.AdductIonName, "[M+NH4]+"), theoreticalMz - (NitrogenMass + HydrogenMass * 3), theoreticalMz)
                    ' seek [M-SO3-H2O+H]+
                    Dim threshold = 1.0
                    Dim diagnosticMz1 = diagnosticMz - SulfurMass - 3 * OxygenMass - H2O - Electron
                    ' seek [M-H2O-SO3-C6H10O5+H]+
                    Dim threshold2 = 1.0
                    Dim diagnosticMz2 = diagnosticMz1 - 162.052833

                    Dim isClassIon1Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz1, threshold)
                    Dim isClassIon2Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz2, threshold2)
                    If isClassIon1Found = False AndAlso isClassIon2Found = False Then Return Nothing

                    Dim hydrogenString = "d"
                    Dim sphOxidized = 2
                    Dim acylOxidized = totalOxidized - sphOxidized

                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()
                    If acylOxidized = 0 Then
                        Dim sph1 = diagnosticMz2 - LipidMsmsCharacterizationUtility.acylCainMass(acylCarbon, acylDouble) + HydrogenMass
                        Dim sph2 = sph1 - H2O
                        Dim sph3 = sph2 - 12
                        Dim acylamide = acylCarbon * 12 + (2 * acylCarbon - 2 * acylDouble + 2) * HydrogenMass + OxygenMass + NitrogenMass - Electron


                        Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                .Mass = sph1,
                                .Intensity = 1
                            },
                                New SpectrumPeak() With {
                                .Mass = sph2,
                                .Intensity = 1
                            },
                                New SpectrumPeak() With {
                                .Mass = sph3,
                                .Intensity = 1
                            }
                            ' new SpectrumPeak() { Mass = acylamide, Intensity = 0.01 }
                            }

                        Dim foundCount = 0
                        Dim averageIntensity = 0.0
                        LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                        If foundCount >= 1 Then ' 
                            Dim molecule = LipidMsmsCharacterizationUtility.getCeramideMoleculeObjAsLevel2("SHexCer", LbmClass.SHexCer, hydrogenString, sphCarbon, sphDouble, acylCarbon, acylDouble, averageIntensity)
                            candidates.Add(molecule)
                        End If
                        Return LipidMsmsCharacterizationUtility.returnAnnotationResult("SHexCer", LbmClass.SHexCer, hydrogenString, theoreticalMz, adduct, totalCarbon, totalDoubleBond, acylOxidized, candidates, 2)   ' case of acyl chain have extra OH
                    Else
                        Dim sph1 = diagnosticMz2 - LipidMsmsCharacterizationUtility.acylCainMass(acylCarbon, acylDouble) + HydrogenMass - OxygenMass * acylOxidized
                        Dim sph2 = sph1 - H2O
                        Dim sph3 = sph2 - 12
                        Dim acylamide = acylCarbon * 12 + (2 * acylCarbon - 2 * acylDouble + 2) * HydrogenMass + OxygenMass + NitrogenMass - Electron

                        'Console.WriteLine("SHexCer {0}:{1};2O/{2}:{3}, sph1={4}, sph2={5}, sph3={6}", sphCarbon, sphDouble, acylCarbon, acylDouble, sph1, sph2, sph3);

                        Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                .Mass = sph1,
                                .Intensity = 1
                            },
                                New SpectrumPeak() With {
                                .Mass = sph2,
                                .Intensity = 1
                            },
                                New SpectrumPeak() With {
                                .Mass = sph3,
                                .Intensity = 1
                            }
                            ' new SpectrumPeak() { Mass = acylamide, Intensity = 0.01 }
                            }

                        Dim foundCount = 0
                        Dim averageIntensity = 0.0
                        LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                        If foundCount >= 1 Then ' 
                            Dim molecule = LipidMsmsCharacterizationUtility.getCeramideoxMoleculeObjAsLevel2("SHexCer", LbmClass.SHexCer, hydrogenString, sphCarbon, sphDouble, acylCarbon, acylDouble, acylOxidized, averageIntensity)
                            candidates.Add(molecule)
                        End If
                        Return LipidMsmsCharacterizationUtility.returnAnnotationResult("SHexCer", LbmClass.SHexCer, hydrogenString, theoreticalMz, adduct, totalCarbon, totalDoubleBond, acylOxidized, candidates, 2)
                    End If
                End If
            Else
                If Equals(adduct.AdductIonName, "[M-H]-") Then
                    ' seek [H2SO4-H]-
                    Dim threshold = 0.1
                    Dim diagnosticMz = 96.960103266

                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    If isClassIonFound <> True Then Return Nothing

                    ' from here, acyl level annotation is executed.
                    '   may be not found fragment to define sphingo and acyl chain
                    Dim candidates = New List(Of LipidMolecule)()
                    'var score = 0;
                    'var molecule0 = LipidMsmsCharacterizationUtility.getCeramideMoleculeObjAsLevel2_0("SHexCer", LbmClass.SHexCer, "d", totalCarbon, totalDoubleBond,
                    '    score);
                    'candidates.Add(molecule0);
                    Dim hydrogenString = "d"
                    Dim sphOxidized = 2
                    Dim acylOxidized = totalOxidized - sphOxidized

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("SHexCer", LbmClass.SHexCer, hydrogenString, theoreticalMz, adduct, totalCarbon, totalDoubleBond, acylOxidized, candidates, 2)
                End If

            End If
            Return Nothing
        End Function
        Public Shared Function JudgeIfPicermide(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sphCarbon As Integer, acylCarbon As Integer, sphDouble As Integer, acylDouble As Integer, adduct As AdductIon, totalOxdyzed As Integer) As LipidMolecule ' If the candidate PE 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If Equals(adduct.AdductIonName, "[M+H]+") Then
                ' seek Header loss (-C6H13O9P)
                Dim threshold = 1.0
                Dim diagnosticMz = theoreticalMz - 260.029722
                Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                If isClassIonFound <> True Then Return Nothing
                'to reject SHexCer
                ' seek [M-SO3-H2O+H]+
                Dim threshold1 = 1.0
                Dim diagnosticMz1 = theoreticalMz - SulfurMass - 3 * OxygenMass - H2O - Electron
                Dim isClassIonFound1 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz1, threshold1)
                If isClassIonFound1 = True Then Return Nothing


                '   may be not found fragment to define sphingo and acyl chain
                Dim candidates = New List(Of LipidMolecule)()

                Dim hydrogenString = "d"
                Dim sphOxidized = 2
                'if (diagnosticMz - (12 * totalCarbon + (totalCarbon * 2 - totalDoubleBond * 2) * MassDiffDictionary.HydrogenMass + MassDiffDictionary.NitrogenMass + MassDiffDictionary.OxygenMass * 3) > 1)
                '{
                '    hydrogenString = "t";
                '    sphOxidized = 3;
                '}
                Dim acylOxidyzed = totalOxdyzed - sphOxidized
                Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PI-Cer", LbmClass.PI_Cer, hydrogenString, theoreticalMz, adduct, totalCarbon, totalDoubleBond, acylOxidyzed, candidates, 2)
            End If
            Return Nothing
        End Function
        Public Shared Function JudgeIfSphingosine(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Single, totalCarbon As Integer, totalDoubleBond As Integer, adduct As AdductIon) As LipidMolecule
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    ' seek -H2O 
                    Dim threshold1 = 10.0
                    Dim diagnosticMz1 = theoreticalMz - H2O
                    ' seek -2H2O 
                    Dim threshold2 = 1.0
                    Dim diagnosticMz2 = diagnosticMz1 - H2O
                    ' seek -H2O -CH2O
                    Dim threshold3 = 1.0
                    Dim diagnosticMz3 = diagnosticMz2 - 12

                    Dim isClassIon1Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz1, threshold1)
                    Dim isClassIon2Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz2, threshold2)
                    Dim isClassIon3Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz3, threshold3)
                    Dim trueCount = 0
                    If isClassIon1Found Then trueCount += 1
                    If isClassIon2Found Then trueCount += 1
                    If isClassIon3Found Then trueCount += 1
                    If trueCount < 3 Then Return Nothing

                    Dim candidates = New List(Of LipidMolecule)()

                    Dim sphOHCount = 2

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("SPB", LbmClass.Sph, String.Empty, theoreticalMz, adduct, totalCarbon, totalDoubleBond, sphOHCount, candidates, 1)
                End If

            End If
            Return Nothing
        End Function
        Public Shared Function JudgeIfNAcylGlyOxFa(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, totalOxidized As Integer, adduct As AdductIon) As LipidMolecule
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' Positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    '  seek 76.039305(gly+)
                    'var threshold = 10.0;
                    'var diagnosticMz = 76.039305;
                    'var isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold);
                    'if (isClassIonFound == false) return null;

                    Dim threshold1 = 5.0
                    Dim diagnosticMz1 = theoreticalMz - (12 + OxygenMass * 2 + HydrogenMass)
                    Dim isClassIonFound1 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz1, threshold1)
                    If isClassIonFound1 = False Then Return Nothing
                    Dim candidates = New List(Of LipidMolecule)()

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("NAGly", LbmClass.NAGly, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, totalOxidized, candidates, 1)
                End If
            ElseIf Equals(adduct.AdductIonName, "[M-H]-") Then
                '  74.024(C2H5NO2-H- Gly)
                Dim threshold = 10.0
                Dim diagnosticMz = 74.024752

                Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                If isClassIonFound = False Then Return Nothing

                Dim candidates = New List(Of LipidMolecule)()

                Return LipidMsmsCharacterizationUtility.returnAnnotationResult("NAGly", LbmClass.NAGly, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, totalOxidized, candidates, 1)
            End If
            Return Nothing
        End Function

        Public Shared Function JudgeIfDag(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sn1Carbon As Integer, sn2Carbon As Integer, sn1Double As Integer, sn2Double As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PS 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If totalCarbon > 52 Then Return Nothing ' currently, very large DAG is excluded.
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+NH4]+") Then
                    ' seek -17.026549 (NH3)
                    Dim threshold = 1.0
                    Dim diagnosticMz = theoreticalMz - 17.026549
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)

                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()
                    If sn2Double >= 7 Then Return Nothing

                    Dim nl_SN1 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1Carbon, sn1Double) - H2O + HydrogenMass
                    Dim nl_SN2 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2Carbon, sn2Double) - H2O + HydrogenMass

                    'Console.WriteLine(sn1Carbon + ":" + sn1Double + "-" + sn2Carbon + ":" + sn2Double + 
                    '    " " + nl_SN1 + " " + nl_SN2);

                    Dim query = New List(Of SpectrumPeak) From {
            New SpectrumPeak() With {
        .Mass = nl_SN1,
        .Intensity = 1
    },
            New SpectrumPeak() With {
        .Mass = nl_SN2,
        .Intensity = 1
    }
        }
                    Dim foundCount = 0
                    Dim averageIntensity = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)
                    If foundCount = 2 Then
                        Dim molecule = LipidMsmsCharacterizationUtility.getDiacylglycerolMoleculeObjAsLevel2("DG", LbmClass.DG, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity)
                        candidates.Add(molecule)
                    End If
                    If candidates Is Nothing OrElse candidates.Count = 0 Then Return Nothing


                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("DG", LbmClass.DG, String.Empty, theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                    ' from here, acyl level annotation is executed.
                    Dim diagnosticMz = theoreticalMz
                    Dim candidates = New List(Of LipidMolecule)()
                    If sn2Double >= 7 Then Return Nothing
                    For sn1CarbonNum = 6 To totalCarbon
                        For sn1DoubleNum = 0 To totalDoubleBond
                            Dim sn2CarbonNum = totalCarbon - sn1CarbonNum
                            Dim sn2DoubleNum = totalDoubleBond - sn1DoubleNum
                            If sn2DoubleNum >= 7 Then Continue For

                            Dim nl_SN1 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1CarbonNum, sn1DoubleNum) - H2O + HydrogenMass * 2
                            Dim nl_SN2 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2CarbonNum, sn2DoubleNum) - H2O + HydrogenMass * 2

                            'Console.WriteLine(sn1Carbon + ":" + sn1Double + "-" + sn2Carbon + ":" + sn2Double + 
                            '    " " + nl_SN1 + " " + nl_SN2);

                            Dim query = New List(Of SpectrumPeak) From {
    New SpectrumPeak() With {
        .Mass = nl_SN1,
        .Intensity = 5
    },
    New SpectrumPeak() With {
        .Mass = nl_SN2,
        .Intensity = 5
    }
}
                            Dim foundCount = 0
                            Dim averageIntensity = 0.0
                            LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)
                            If foundCount = 2 Then
                                Dim molecule = LipidMsmsCharacterizationUtility.getDiacylglycerolMoleculeObjAsLevel2("DG", LbmClass.DG, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity)
                                candidates.Add(molecule)
                            End If
                        Next
                    Next
                    If candidates Is Nothing OrElse candidates.Count = 0 Then Return Nothing
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("DG", LbmClass.DG, String.Empty, theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)

                End If
            End If
            Return Nothing
        End Function
        Public Shared Function JudgeIfEtherpc(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sn1Carbon As Integer, sn2Carbon As Integer, sn1Double As Integer, sn2Double As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PC 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    Dim Gly_C As Double = {CarbonMass * 8, HydrogenMass * 18, NitrogenMass, OxygenMass * 4, PhosphorusMass, Proton}.Sum()
                    Dim Gly_O As Double = {CarbonMass * 7, HydrogenMass * 16, NitrogenMass, OxygenMass * 5, PhosphorusMass, Proton}.Sum()
                    Dim C5H14NO4P As Double = {CarbonMass * 5, HydrogenMass * 14, NitrogenMass, OxygenMass * 4, PhosphorusMass, Proton}.Sum()
                    ' 
                    Dim threshold = 3.0
                    Dim isClassIonFound1 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, C5H14NO4P, threshold)
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C, threshold)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O, threshold)
                    If Not isClassIonFound1 OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing
                    ' reject Na+ adduct
                    Dim diagnosticMz3 = theoreticalMz - 59.0735
                    Dim isNaTypicalFound1 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz3, 10.0)
                    If isNaTypicalFound1 Then
                        Return Nothing
                    End If


                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()
                    Dim AcylLoss = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2Carbon, sn2Double) - H2O
                    Dim EtherLoss = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1Carbon, sn1Double) - HydrogenMass * 2

                    Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                            .Mass = AcylLoss,
                            .Intensity = 0.1
                        },
                                New SpectrumPeak() With {
                            .Mass = EtherLoss,
                            .Intensity = 1.0
                        }
                             }

                    Dim foundCount = 0
                    Dim averageIntensity = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)
                    If foundCount = 2 Then ' 
                        Dim EtherString = "O-" & sn1Carbon.ToString() & ":" & sn1Double.ToString()
                        Dim AcylString = sn2Carbon.ToString() & ":" & sn2Double.ToString()

                        Dim nl_Ether_H2O_CH2 = EtherLoss - CH2
                        Dim nl_Acyl_H2O_CH2 = AcylLoss - CH2
                        Dim queryCh2 = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                .Mass = nl_Ether_H2O_CH2,
                                .Intensity = 0.1
                            },
                                New SpectrumPeak() With {
                                .Mass = nl_Acyl_H2O_CH2,
                                .Intensity = 0.1
                            }
                            }

                        Dim foundCountSN2 = 0
                        Dim averageIntensitySN2 = 0.0
                        LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, queryCh2, ms2Tolerance, foundCountSN2, averageIntensitySN2)
                        If foundCountSN2 > 0 Then
                            If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_Ether_H2O_CH2, nl_Acyl_H2O_CH2) Then
                                Dim molecule = getEtherPhospholipidMoleculeObjAsPositionChain("PC", LbmClass.EtherPC, sn1Carbon, sn1Double, EtherString, sn2Carbon, sn2Double, AcylString, averageIntensity, "e") ' use averageIntensity (not averageIntensitySN2)
                                candidates.Add(molecule)
                            Else
                                Dim molecule = getEtherPhospholipidMoleculeObjAsPositionChain("PC", LbmClass.EtherPC, sn2Carbon, sn2Double, AcylString, sn1Carbon, sn1Double, EtherString, averageIntensity, "e")
                                candidates.Add(molecule)
                            End If
                        Else
                            Dim molecule = LipidMsmsCharacterizationUtility.getEtherPhospholipidMoleculeObjAsLevel2("PC", LbmClass.EtherPC, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity, "e")
                            candidates.Add(molecule)
                        End If
                    Else
                        Return Nothing
                    End If
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PC", LbmClass.EtherPC, "e", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                    ' seek PreCursor - 59 (C3H9N)
                    Dim threshold = 10.0
                    Dim diagnosticMz = theoreticalMz - 59.072951
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    If isClassIonFound = False Then Return Nothing  ' must or not?

                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()
                    'var averageIntensity = 0.0;
                    'var molecule = LipidMsmsCharacterizationUtility.getSingleacylchainwithsuffixMoleculeObjAsLevel2("PC", LbmClass.EtherPC, totalCarbon,
                    '                totalDoubleBond, averageIntensity, "e");
                    'candidates.Add(molecule);

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PC", LbmClass.EtherPC, "e", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                End If
            End If
            Return Nothing
        End Function
        Public Shared Function JudgeIfCholesterylEster(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Single, totalCarbon As Integer, totalDoubleBond As Integer, adduct As AdductIon) As LipidMolecule
            Dim skelton As Double = {CarbonMass * 27, HydrogenMass * 46, OxygenMass * 1}.Sum()
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+NH4]+") Then
                    ' seek 369.3515778691 (C27H45+)+ MassDiffDictionary.HydrogenMass*7
                    Dim threshold = 20.0
                    Dim diagnosticMz = skelton - H2O + Proton
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    If isClassIonFound = False Then Return Nothing
                    If totalCarbon >= 41 AndAlso totalDoubleBond >= 4 Then Return Nothing

                    Dim candidates = New List(Of LipidMolecule)()

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("CE", LbmClass.CE, String.Empty, theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                    ' seek 368.3515778691 (C27H44)+ MassDiffDictionary.HydrogenMass*7
                    Dim threshold = 10.0
                    Dim diagnosticMz = skelton - H2O
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    If isClassIonFound = False Then Return Nothing

                    Dim candidates = New List(Of LipidMolecule)()

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("CE", LbmClass.CE, String.Empty, theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                End If
            End If
            Return Nothing
        End Function

        Public Shared Function JudgeIfPhosphatidylcholineD5(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sn1Carbon As Integer, sn2Carbon As Integer, sn1Double As Integer, sn2Double As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PC 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim Gly_C As Double = {CarbonMass * 8, HydrogenMass * 13, NitrogenMass, OxygenMass * 4, PhosphorusMass, Hydrogen2Mass * 5}.Sum()
            Dim Gly_O As Double = {CarbonMass * 7, HydrogenMass * 13, NitrogenMass, OxygenMass * 5, PhosphorusMass, Hydrogen2Mass * 3}.Sum()
            Dim C5H14NO4P As Double = {CarbonMass * 5, HydrogenMass * 14, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()
            Dim C3H9N As Double = {CarbonMass * 3, HydrogenMass * 9, NitrogenMass}.Sum()

            Dim spectrum = msScanProp.Spectrum
            Dim candidates = New List(Of LipidMolecule)()

            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    If sn1Carbon < 10 OrElse sn2Carbon < 10 Then Return Nothing
                    If sn1Double > 6 OrElse sn2Double > 6 Then Return Nothing
                    '  seek PC diabnostic
                    Dim threshold = 3.0
                    Dim isClassIonFound1 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, C5H14NO4P + Proton, threshold)
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C + Proton, threshold)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O + Proton, threshold)
                    If Not isClassIonFound1 OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing
                    ' reject Na+ adduct
                    Dim diagnosticMz3 = theoreticalMz - C3H9N
                    Dim isNaTypicalFound1 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz3, 10.0)
                    ' reject PE
                    Dim PEHeaderLoss = theoreticalMz - 141.019094261
                    Dim isClassIonFoundPe = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, PEHeaderLoss, 5.0)
                    If isNaTypicalFound1 OrElse isClassIonFoundPe Then
                        Return Nothing
                    End If

                    ' from here, acyl level annotation is executed.
                    Dim nl_SN1 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1Carbon, sn1Double) + HydrogenMass
                    Dim nl_SN1_H2O = nl_SN1 - H2O

                    Dim nl_SN2 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2Carbon, sn2Double) + HydrogenMass
                    Dim nl_SN2_H2O = nl_SN2 - H2O

                    Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                            .Mass = nl_SN1,
                            .Intensity = 0.1
                        },
                                New SpectrumPeak() With {
                            .Mass = nl_SN1_H2O,
                            .Intensity = 0.1
                        },
                                New SpectrumPeak() With {
                            .Mass = nl_SN2,
                            .Intensity = 0.1
                        },
                                New SpectrumPeak() With {
                            .Mass = nl_SN2_H2O,
                            .Intensity = 0.1
                        }
                            }

                    Dim foundCount = 0
                    Dim averageIntensity = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)
                    If foundCount >= 2 Then ' now I set 2 as the correct level
                        'check Sn2 position spectrum
                        Dim nl_SN1_H2O_CH2 = nl_SN1_H2O - CD2
                        Dim nl_SN2_H2O_CH2 = nl_SN2_H2O - CD2
                        Dim queryCh2 = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                .Mass = nl_SN1_H2O_CH2,
                                .Intensity = 1.0
                            },
                                New SpectrumPeak() With {
                                .Mass = nl_SN2_H2O_CH2,
                                .Intensity = 1.0
                            }
                            }

                        Dim foundCountSN2 = 0
                        Dim averageIntensitySN2 = 0.0
                        LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, queryCh2, ms2Tolerance, foundCountSN2, averageIntensitySN2)
                        If foundCountSN2 > 0 Then
                            If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_SN1_H2O_CH2, nl_SN2_H2O_CH2) Then
                                Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PC_d5", LbmClass.PC_d5, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity)
                                candidates.Add(molecule)
                            Else
                                Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PC_d5", LbmClass.PC_d5, sn2Carbon, sn2Double, sn1Carbon, sn1Double, averageIntensity)
                                candidates.Add(molecule)
                            End If
                        Else
                            Dim molecule = LipidMsmsCharacterizationUtility.getPhospholipidMoleculeObjAsLevel2("PC_d5", LbmClass.PC_d5, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity)
                            candidates.Add(molecule)
                        End If
                    Else
                        Return Nothing
                    End If
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PC_d5", LbmClass.PC_d5, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                    'var diagnosticMz = 184.07332;
                    ' seek [M+Na -C5H14NO4P]+
                    Dim diagnosticMz2 = theoreticalMz - C5H14NO4P
                    ' seek [M+Na -C3H9N]+
                    Dim diagnosticMz3 = theoreticalMz - C3H9N
                    Dim threshold = 3.0
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz2, threshold)
                    Dim isClassIon2Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz3, threshold)
                    If Not isClassIonFound OrElse Not isClassIon2Found Then Return Nothing
                    ' seek PC diabnostic
                    Dim isClassIonFound1 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, C5H14NO4P + Na, threshold)
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C + Na, threshold)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O + Na, threshold)
                    If Not isClassIonFound1 OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing

                    ' from here, acyl level annotation is executed.
                    For sn1CarbonNum = 6 To totalCarbon
                        For sn1DoubleNum = 0 To totalDoubleBond
                            Dim sn2CarbonNum = totalCarbon - sn1CarbonNum
                            Dim sn2DoubleNum = totalDoubleBond - sn1DoubleNum
                            If sn2DoubleNum >= 7 Then Continue For
                            Dim nl_SN1 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1CarbonNum, sn1DoubleNum) + HydrogenMass
                            Dim nl_SN1_H2O = nl_SN1 - H2O

                            Dim nl_SN2 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2CarbonNum, sn2DoubleNum) + HydrogenMass
                            Dim nl_SN2_H2O = nl_SN2 - H2O

                            Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                    .Mass = nl_SN1,
                                    .Intensity = 0.1
                                },
                                New SpectrumPeak() With {
                                    .Mass = nl_SN1_H2O,
                                    .Intensity = 0.1
                                },
                                New SpectrumPeak() With {
                                    .Mass = nl_SN2,
                                    .Intensity = 0.1
                                },
                                New SpectrumPeak() With {
                                    .Mass = nl_SN2_H2O,
                                    .Intensity = 0.1
                                }
                            }

                            Dim foundCount = 0
                            Dim averageIntensity = 0.0
                            LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                            If foundCount >= 2 Then ' now I set 2 as the correct level
                                'check Sn2 position spectrum
                                Dim nl_SN1_H2O_CH2 = nl_SN1_H2O + HydrogenMass - CD2
                                Dim nl_SN2_H2O_CH2 = nl_SN2_H2O + HydrogenMass - CD2
                                Dim queryCh2 = New List(Of SpectrumPeak) From {
                                    New SpectrumPeak() With {
                                        .Mass = nl_SN1_H2O_CH2,
                                        .Intensity = 1.0
                                    },
                                    New SpectrumPeak() With {
                                        .Mass = nl_SN2_H2O_CH2,
                                        .Intensity = 1.0
                                    }
                                }

                                Dim foundCountSN2 = 0
                                Dim averageIntensitySN2 = 0.0
                                LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, queryCh2, ms2Tolerance, foundCountSN2, averageIntensitySN2)
                                If foundCountSN2 > 0 Then
                                    If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_SN1_H2O_CH2, nl_SN2_H2O_CH2) Then
                                        Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PC_d5", LbmClass.PC_d5, sn2CarbonNum, sn2DoubleNum, sn1CarbonNum, sn1DoubleNum, averageIntensity)
                                        candidates.Add(molecule)
                                    Else
                                        Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PC_d5", LbmClass.PC_d5, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity)
                                        candidates.Add(molecule)
                                    End If
                                Else
                                    Dim molecule = LipidMsmsCharacterizationUtility.getPhospholipidMoleculeObjAsLevel2("PC_d5", LbmClass.PC_d5, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity)
                                    candidates.Add(molecule)
                                End If
                            End If
                        Next
                    Next
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PC_d5", LbmClass.PC_d5, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                End If
            End If
            Return Nothing
        End Function

        Public Shared Function JudgeIfPhosphatidylethanolamineD5(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sn1Carbon As Integer, sn2Carbon As Integer, sn1Double As Integer, sn2Double As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PE 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            Dim candidates = New List(Of LipidMolecule)()

            Dim Gly_C As Double = {CarbonMass * 5, HydrogenMass * 7, NitrogenMass, OxygenMass * 4, PhosphorusMass, Hydrogen2Mass * 5}.Sum()

            Dim Gly_O As Double = {CarbonMass * 4, HydrogenMass * 7, NitrogenMass, OxygenMass * 5, PhosphorusMass, Hydrogen2Mass * 3}.Sum()
            Dim C2H8NO4P As Double = {CarbonMass * 2, HydrogenMass * 8, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    ' seek -141.019094261 (C2H8NO4P)and PE diagnostic
                    Dim threshold = 2.5
                    Dim diagnosticMz = theoreticalMz - C2H8NO4P
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C + Proton, threshold)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O + Proton, threshold)
                    If Not isClassIonFound OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing

                    ' from here, acyl level annotation is executed.
                    Dim sn1 = LipidMsmsCharacterizationUtility.acylCainMass(sn1Carbon, sn1Double) - Electron
                    Dim sn2 = LipidMsmsCharacterizationUtility.acylCainMass(sn2Carbon, sn2Double) - Electron

                    Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                            .Mass = sn1,
                            .Intensity = 0.1
                        },
                                New SpectrumPeak() With {
                            .Mass = sn2,
                            .Intensity = 0.1
                        }
                            }

                    Dim foundCount = 0
                    Dim averageIntensity = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                    If foundCount = 2 Then ' now I set 2 as the correct level
                        'check Sn2 position spectrum
                        Dim nl_SN1_H2O_CH2 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1Carbon, sn1Double) + HydrogenMass - H2O - CH2
                        Dim nl_SN2_H2O_CH2 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2Carbon, sn2Double) + HydrogenMass - H2O - CH2
                        Dim queryCh2 = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                .Mass = nl_SN1_H2O_CH2,
                                .Intensity = 1.0
                            },
                                New SpectrumPeak() With {
                                .Mass = nl_SN2_H2O_CH2,
                                .Intensity = 1.0
                            }
                            }

                        Dim foundCountSN2 = 0
                        Dim averageIntensitySN2 = 0.0
                        LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, queryCh2, ms2Tolerance, foundCountSN2, averageIntensitySN2)
                        If foundCountSN2 > 0 Then
                            If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_SN1_H2O_CH2, nl_SN2_H2O_CH2) Then
                                Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PE_d5", LbmClass.PE_d5, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity)
                                candidates.Add(molecule)
                            Else
                                Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PE_d5", LbmClass.PE_d5, sn2Carbon, sn2Double, sn1Carbon, sn1Double, averageIntensity)
                                candidates.Add(molecule)
                            End If
                        Else
                            Dim molecule = LipidMsmsCharacterizationUtility.getPhospholipidMoleculeObjAsLevel2("PE_d5", LbmClass.PE_d5, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity)
                            candidates.Add(molecule)
                        End If
                    End If

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PE_d5", LbmClass.PE_d5, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                    ' seek -141.019094261 (C2H8NO4P)
                    Dim threshold = 3.0
                    Dim diagnosticMz = theoreticalMz - C2H8NO4P
                    ' seek - 43.042199 (C2H5N)
                    'var diagnosticMz2 = theoreticalMz - C2H5N;
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C + Na, threshold)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O + Na, threshold)
                    If Not isClassIonFound OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing

                    ' from here, acyl level annotation is executed.
                    For sn1CarbonNum = 6 To totalCarbon
                        For sn1DoubleNum = 0 To totalDoubleBond
                            Dim sn2CarbonNum = totalCarbon - sn1CarbonNum
                            Dim sn2DoubleNum = totalDoubleBond - sn1DoubleNum
                            If sn2DoubleNum >= 7 Then Continue For

                            Dim nl_SN1 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1CarbonNum, sn1DoubleNum) - HydrogenMass
                            Dim nl_SN1_H2O = nl_SN1 - OxygenMass
                            Dim nl_SN2 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2CarbonNum, sn2DoubleNum) - HydrogenMass
                            Dim nl_SN2_H2O = nl_SN2 - OxygenMass

                            Dim query = New List(Of SpectrumPeak) From {
    New SpectrumPeak() With {
        .Mass = nl_SN1,
        .Intensity = 0.1
    },
    New SpectrumPeak() With {
        .Mass = nl_SN1_H2O,
        .Intensity = 0.1
    },
    New SpectrumPeak() With {
        .Mass = nl_SN2,
        .Intensity = 0.1
    },
    New SpectrumPeak() With {
        .Mass = nl_SN2_H2O,
        .Intensity = 0.1
    }
}
                            Dim foundCount = 0
                            Dim averageIntensity = 0.0
                            LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)
                            If foundCount >= 2 Then
                                'check Sn2 position spectrum
                                Dim nl_SN1_H2O_CH2 = nl_SN1_H2O - CD2
                                Dim nl_SN2_H2O_CH2 = nl_SN2_H2O - CD2
                                Dim queryCh2 = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                        .Mass = nl_SN1_H2O_CH2,
                                        .Intensity = 1.0
                                    },
                                New SpectrumPeak() With {
                                        .Mass = nl_SN2_H2O_CH2,
                                        .Intensity = 1.0
                                    }
                                }

                                Dim foundCountSN2 = 0
                                Dim averageIntensitySN2 = 0.0
                                LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, queryCh2, ms2Tolerance, foundCountSN2, averageIntensitySN2)
                                If foundCountSN2 > 0 Then
                                    If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_SN1_H2O_CH2, nl_SN2_H2O_CH2) Then
                                        Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PE_d5", LbmClass.PE_d5, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity)
                                        candidates.Add(molecule)
                                    Else
                                        Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PE_d5", LbmClass.PE_d5, sn2CarbonNum, sn2DoubleNum, sn1CarbonNum, sn1DoubleNum, averageIntensity)
                                        candidates.Add(molecule)
                                    End If
                                Else
                                    Dim molecule = LipidMsmsCharacterizationUtility.getPhospholipidMoleculeObjAsLevel2("PE_d5", LbmClass.PE_d5, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity)
                                    candidates.Add(molecule)

                                End If
                            End If
                        Next
                    Next
                    If candidates Is Nothing OrElse candidates.Count = 0 Then Return Nothing

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PE_d5", LbmClass.PE_d5, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                End If
            End If
            Return Nothing
        End Function

        Public Shared Function JudgeIfPhosphatidylglycerolD5(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sn1Carbon As Integer, sn2Carbon As Integer, sn1Double As Integer, sn2Double As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PS 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            Dim Gly_C As Double = {CarbonMass * 6, HydrogenMass * 8, OxygenMass * 6, PhosphorusMass, Hydrogen2Mass * 5}.Sum()
            Dim Gly_O As Double = {CarbonMass * 5, HydrogenMass * 8, OxygenMass * 7, PhosphorusMass, Hydrogen2Mass * 3}.Sum()
            Dim C3H9O6P As Double = {CarbonMass * 3, HydrogenMass * 9, OxygenMass * 6, PhosphorusMass}.Sum()
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+NH4]+") Then
                    ' seek -189.040227 (C3H8O6P+NH4)
                    Dim threshold = 10.0
                    Dim diagnosticMz = theoreticalMz - C3H9O6P - NH3
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    ' PG diagnostic
                    Dim threshold2 = 1.0
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C + Proton, threshold2)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O + Proton, threshold2)
                    If Not isClassIonFound OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing

                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()
                    Dim nl_SN1 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1Carbon, sn1Double) + HydrogenMass
                    Dim nl_SN1_H2O = nl_SN1 - H2O
                    Dim nl_SN2 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2Carbon, sn2Double) + HydrogenMass
                    Dim nl_SN2_H2O = nl_SN2 - H2O

                    Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                            .Mass = nl_SN1,
                            .Intensity = 0.1
                        },
                                New SpectrumPeak() With {
                            .Mass = nl_SN1_H2O,
                            .Intensity = 0.1
                        },
                                New SpectrumPeak() With {
                            .Mass = nl_SN2,
                            .Intensity = 0.1
                        },
                                New SpectrumPeak() With {
                            .Mass = nl_SN2_H2O,
                            .Intensity = 0.1
                        }
                            }

                    Dim foundCount = 0
                    Dim averageIntensity = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                    If foundCount >= 2 AndAlso averageIntensity < 30 Then ' average intensity < 30 is nessesarry to distinguish it from BMP
                        'check Sn2 position spectrum
                        Dim nl_SN1_H2O_CH2 = nl_SN1_H2O - CD2
                        Dim nl_SN2_H2O_CH2 = nl_SN1_H2O - CD2
                        Dim queryCh2 = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                .Mass = nl_SN1_H2O_CH2,
                                .Intensity = 1.0
                            },
                                New SpectrumPeak() With {
                                .Mass = nl_SN2_H2O_CH2,
                                .Intensity = 1.0
                            }
                            }

                        Dim foundCountSN2 = 0
                        Dim averageIntensitySN2 = 0.0
                        LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, queryCh2, ms2Tolerance, foundCountSN2, averageIntensitySN2)
                        If foundCountSN2 > 0 Then
                            If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_SN1_H2O_CH2, nl_SN2_H2O_CH2) Then
                                Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PG_d5", LbmClass.PG_d5, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity)
                                candidates.Add(molecule)
                            Else
                                Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PG_d5", LbmClass.PG_d5, sn2Carbon, sn2Double, sn1Carbon, sn1Double, averageIntensity)
                                candidates.Add(molecule)
                            End If
                        Else
                            Dim molecule = LipidMsmsCharacterizationUtility.getPhospholipidMoleculeObjAsLevel2("PG_d5", LbmClass.PG_d5, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity)
                            candidates.Add(molecule)
                        End If
                    End If
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PG_d5", LbmClass.PG_d5, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                    ' seek header loss
                    Dim threshold = 10.0
                    Dim diagnosticMz = theoreticalMz - C3H9O6P
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    ' PG diagnostic
                    Dim threshold2 = 5.0
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C + Na, threshold2)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O + Na, threshold2)
                    If Not isClassIonFound OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing

                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PG_d5", LbmClass.PG_d5, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                End If
            End If
            Return Nothing
        End Function

        Public Shared Function JudgeIfPhosphatidylserineD5(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sn1Carbon As Integer, sn2Carbon As Integer, sn1Double As Integer, sn2Double As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PS 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            Dim Gly_C As Double = {CarbonMass * 6, HydrogenMass * 7, NitrogenMass, OxygenMass * 6, PhosphorusMass, Hydrogen2Mass * 5}.Sum()
            Dim Gly_O As Double = {CarbonMass * 5, HydrogenMass * 7, NitrogenMass, OxygenMass * 7, PhosphorusMass, Hydrogen2Mass * 3}.Sum()
            Dim C3H8NO6P As Double = {CarbonMass * 3, HydrogenMass * 8, NitrogenMass, OxygenMass * 6, PhosphorusMass}.Sum()
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    ' seek -185.008927 (C3H8NO6P)
                    Dim threshold = 10.0
                    Dim diagnosticMz = theoreticalMz - C3H8NO6P
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    ' PS diagnostic
                    Dim threshold2 = 5.0
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C + Proton, threshold2)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O + Proton, threshold2)
                    If Not isClassIonFound OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing

                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()
                    Dim nl_SN1 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1Carbon, sn1Double) + HydrogenMass
                    Dim nl_SN1_H2O = nl_SN1 - H2O

                    Dim nl_SN2 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2Carbon, sn2Double) + HydrogenMass
                    Dim nl_NS2_H2O = nl_SN2 - H2O

                    Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                            .Mass = nl_SN1,
                            .Intensity = 0.1
                        },
                                New SpectrumPeak() With {
                            .Mass = nl_SN1_H2O,
                            .Intensity = 0.1
                        },
                                New SpectrumPeak() With {
                            .Mass = nl_SN2,
                            .Intensity = 0.1
                        },
                                New SpectrumPeak() With {
                            .Mass = nl_NS2_H2O,
                            .Intensity = 0.1
                        }
                            }

                    Dim foundCount = 0
                    Dim averageIntensity = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                    If foundCount >= 2 Then
                        'check Sn2 position spectrum
                        Dim nl_SN1_H2O_CH2 = nl_SN1 - H2O - CD2
                        Dim nl_SN2_H2O_CH2 = nl_SN2 - H2O - CD2
                        Dim queryCh2 = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                .Mass = nl_SN1_H2O_CH2,
                                .Intensity = 1.0
                            },
                                New SpectrumPeak() With {
                                .Mass = nl_SN2_H2O_CH2,
                                .Intensity = 1.0
                            }
                            }

                        Dim foundCountSN2 = 0
                        Dim averageIntensitySN2 = 0.0
                        LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, queryCh2, ms2Tolerance, foundCountSN2, averageIntensitySN2)
                        If foundCountSN2 > 0 Then
                            If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_SN1_H2O_CH2, nl_SN2_H2O_CH2) Then
                                Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PS_d5", LbmClass.PS_d5, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity)
                                candidates.Add(molecule)
                            Else
                                Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PS_d5", LbmClass.PS_d5, sn2Carbon, sn2Double, sn1Carbon, sn1Double, averageIntensity)
                                candidates.Add(molecule)
                            End If
                        Else
                            Dim molecule = LipidMsmsCharacterizationUtility.getPhospholipidMoleculeObjAsLevel2("PS_d5", LbmClass.PS_d5, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity)
                            candidates.Add(molecule)
                        End If
                    End If

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PS_d5", LbmClass.PS_d5, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                    ' seek -185.008927 (C3H8NO6P)
                    Dim threshold = 3.0
                    Dim diagnosticMz = theoreticalMz - C3H8NO6P
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    ' PS diagnostic
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C + Na, threshold)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O + Na, threshold)
                    If Not isClassIonFound OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing

                    Dim candidates = New List(Of LipidMolecule)()
                    ' from here, acyl level annotation is executed.
                    For sn1CarbonNum = 6 To totalCarbon
                        For sn1DoubleNum = 0 To totalDoubleBond
                            Dim sn2CarbonNum = totalCarbon - sn1CarbonNum
                            Dim sn2DoubleNum = totalDoubleBond - sn1DoubleNum
                            If sn2DoubleNum >= 7 Then Continue For

                            Dim nl_SN1 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1CarbonNum, sn1DoubleNum) + HydrogenMass
                            Dim nl_SN1_H2O = nl_SN1 - OxygenMass - HydrogenMass
                            Dim nl_SN2 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2CarbonNum, sn2DoubleNum) + HydrogenMass
                            Dim nl_SN2_H2O = nl_SN2 - OxygenMass - HydrogenMass


                            Dim query = New List(Of SpectrumPeak) From {
    New SpectrumPeak() With {
        .Mass = nl_SN1,
        .Intensity = 0.1
    },
    New SpectrumPeak() With {
        .Mass = nl_SN1_H2O,
        .Intensity = 0.1
    },
    New SpectrumPeak() With {
        .Mass = nl_SN2,
        .Intensity = 0.1
    },
    New SpectrumPeak() With {
        .Mass = nl_SN2_H2O,
        .Intensity = 0.1
    }
}
                            Dim foundCount = 0
                            Dim averageIntensity = 0.0
                            LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)
                            If foundCount >= 2 Then
                                'check Sn2 position spectrum
                                Dim nl_SN1_H2O_CH2 = nl_SN1_H2O - CD2
                                Dim nl_SN2_H2O_CH2 = nl_SN2_H2O - CD2
                                Dim queryCh2 = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                        .Mass = nl_SN1_H2O_CH2,
                                        .Intensity = 1.0
                                    },
                                New SpectrumPeak() With {
                                        .Mass = nl_SN2_H2O_CH2,
                                        .Intensity = 1.0
                                    }
                            }

                                Dim foundCountSN2 = 0
                                Dim averageIntensitySN2 = 0.0
                                LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, queryCh2, ms2Tolerance, foundCountSN2, averageIntensitySN2)
                                If foundCountSN2 > 0 Then
                                    If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_SN1_H2O_CH2, nl_SN2_H2O_CH2) Then
                                        Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PS_d5", LbmClass.PS_d5, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity)
                                        candidates.Add(molecule)
                                    Else
                                        Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PS_d5", LbmClass.PS_d5, sn2CarbonNum, sn2DoubleNum, sn1CarbonNum, sn1DoubleNum, averageIntensity)
                                        candidates.Add(molecule)
                                    End If
                                Else
                                    Dim molecule = LipidMsmsCharacterizationUtility.getPhospholipidMoleculeObjAsLevel2("PS_d5", LbmClass.PS_d5, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity)
                                    candidates.Add(molecule)

                                End If
                            End If
                        Next
                    Next
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PS_d5", LbmClass.PS_d5, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                End If
            End If
            Return Nothing
        End Function

        Public Shared Function JudgeIfPhosphatidylinositolD5(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sn1Carbon As Integer, sn2Carbon As Integer, sn1Double As Integer, sn2Double As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PS 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim Gly_C As Double = {CarbonMass * 9, HydrogenMass * 12, OxygenMass * 9, PhosphorusMass, Hydrogen2Mass * 5}.Sum()
            Dim Gly_O As Double = {CarbonMass * 8, HydrogenMass * 12, OxygenMass * 10, PhosphorusMass, Hydrogen2Mass * 3}.Sum()
            Dim C6H13O9P As Double = {CarbonMass * 6, HydrogenMass * 13, OxygenMass * 9, PhosphorusMass}.Sum()
            Dim C6H10O5 As Double = {CarbonMass * 6, HydrogenMass * 10, OxygenMass * 5}.Sum()
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+NH4]+") Then
                    ' seek -277.056272 (C6H12O9P+NH4)
                    Dim threshold = 10.0
                    Dim diagnosticMz = theoreticalMz - C6H13O9P - NH3
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    ' PI diagnostic
                    Dim threshold2 = 5.0
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C + Proton, threshold2)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O + Proton, threshold2)
                    If Not isClassIonFound OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing

                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()
                    For sn1CarbonNum = 6 To totalCarbon
                        For sn1DoubleNum = 0 To totalDoubleBond
                            Dim sn2CarbonNum = totalCarbon - sn1CarbonNum
                            Dim sn2DoubleNum = totalDoubleBond - sn1DoubleNum

                            Dim nl_SN1 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1CarbonNum, sn1DoubleNum) + HydrogenMass
                            Dim nl_SN1_H2O = nl_SN1 - H2O
                            Dim nl_SN2 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2CarbonNum, sn2DoubleNum) + HydrogenMass
                            Dim nl_SN2_H2O = nl_SN2 - H2O

                            Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                    .Mass = nl_SN1,
                                    .Intensity = 0.1
                                },
                                New SpectrumPeak() With {
                                    .Mass = nl_SN1_H2O,
                                    .Intensity = 0.1
                                },
                                New SpectrumPeak() With {
                                    .Mass = nl_SN2,
                                    .Intensity = 0.1
                                },
                                New SpectrumPeak() With {
                                    .Mass = nl_SN2_H2O,
                                    .Intensity = 0.1
                                }
                            }

                            Dim foundCount = 0
                            Dim averageIntensity = 0.0
                            LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                            If foundCount >= 2 Then
                                'check Sn2 position spectrum
                                Dim nl_SN1_H2O_CH2 = nl_SN1_H2O - CD2
                                Dim nl_SN2_H2O_CH2 = nl_SN2_H2O - CD2
                                Dim queryCh2 = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                        .Mass = nl_SN1_H2O_CH2,
                                        .Intensity = 0.5
                                    },
                                New SpectrumPeak() With {
                                        .Mass = nl_SN2_H2O_CH2,
                                        .Intensity = 0.5
                                    }
                            }

                                Dim foundCountSN2 = 0
                                Dim averageIntensitySN2 = 0.0
                                LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, queryCh2, ms2Tolerance, foundCountSN2, averageIntensitySN2)
                                If foundCountSN2 > 0 Then
                                    If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_SN1_H2O_CH2, nl_SN2_H2O_CH2) Then
                                        Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PI_d5", LbmClass.PI_d5, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity)
                                        candidates.Add(molecule)
                                    Else
                                        Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PI_d5", LbmClass.PI_d5, sn2CarbonNum, sn2DoubleNum, sn1CarbonNum, sn1DoubleNum, averageIntensity)
                                        candidates.Add(molecule)
                                    End If
                                Else
                                    Dim molecule = LipidMsmsCharacterizationUtility.getPhospholipidMoleculeObjAsLevel2("PI_d5", LbmClass.PI_d5, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity)
                                    candidates.Add(molecule)
                                End If
                            End If
                        Next
                    Next

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PI_d5", LbmClass.PI_d5, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                    ' seek -C6H10O5
                    Dim threshold = 5.0
                    Dim diagnosticMz = theoreticalMz - C6H10O5
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    ' PI diagnostic
                    Dim threshold2 = 5.0
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C + Na, threshold2)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O + Na, threshold2)
                    If Not isClassIonFound OrElse Not isClassIonFound2 OrElse Not isClassIonFound3 Then Return Nothing

                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()
                    For sn1CarbonNum = 6 To totalCarbon
                        For sn1DoubleNum = 0 To totalDoubleBond
                            Dim sn2CarbonNum = totalCarbon - sn1CarbonNum
                            Dim sn2DoubleNum = totalDoubleBond - sn1DoubleNum

                            Dim nl_SN1 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1CarbonNum, sn1DoubleNum) + HydrogenMass
                            Dim nl_SN1_H2O = nl_SN1 - H2O
                            Dim nl_SN2 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2CarbonNum, sn2DoubleNum) + HydrogenMass
                            Dim nl_SN2_H2O = nl_SN2 - H2O

                            Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                    .Mass = nl_SN1,
                                    .Intensity = 0.1
                                },
                                New SpectrumPeak() With {
                                    .Mass = nl_SN1_H2O,
                                    .Intensity = 0.1
                                },
                                New SpectrumPeak() With {
                                    .Mass = nl_SN2,
                                    .Intensity = 0.1
                                },
                                New SpectrumPeak() With {
                                    .Mass = nl_SN2_H2O,
                                    .Intensity = 0.1
                                }
                            }

                            Dim foundCount = 0
                            Dim averageIntensity = 0.0
                            LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                            If foundCount >= 2 Then
                                'check Sn2 position spectrum
                                Dim nl_SN1_H2O_CH2 = nl_SN1_H2O - CD2 + HydrogenMass
                                Dim nl_SN2_H2O_CH2 = nl_SN2_H2O - CD2 + HydrogenMass
                                Dim queryCh2 = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                        .Mass = nl_SN1_H2O_CH2,
                                        .Intensity = 1.0
                                    },
                                New SpectrumPeak() With {
                                        .Mass = nl_SN2_H2O_CH2,
                                        .Intensity = 1.0
                                    }
                            }

                                Dim foundCountSN2 = 0
                                Dim averageIntensitySN2 = 0.0
                                LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, queryCh2, ms2Tolerance, foundCountSN2, averageIntensitySN2)
                                If foundCountSN2 > 0 Then
                                    If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, nl_SN1_H2O_CH2, nl_SN2_H2O_CH2) Then
                                        Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PI_d5", LbmClass.PI_d5, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity)
                                        candidates.Add(molecule)
                                    Else
                                        Dim molecule = getTwoChainLipidMoleculeObjAsPositionChain("PI_d5", LbmClass.PI_d5, sn2CarbonNum, sn2DoubleNum, sn1CarbonNum, sn1DoubleNum, averageIntensity)
                                        candidates.Add(molecule)
                                    End If
                                Else
                                    Dim molecule = LipidMsmsCharacterizationUtility.getPhospholipidMoleculeObjAsLevel2("PI_d5", LbmClass.PI_d5, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity)
                                    candidates.Add(molecule)
                                End If
                            End If
                        Next
                    Next

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("PI_d5", LbmClass.PI_d5, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                End If
            End If
            Return Nothing
        End Function
        Public Shared Function JudgeIfLysopcD5(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, snCarbon As Integer, snDoubleBond As Integer, adduct As AdductIon) As LipidMolecule ' 
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    If totalCarbon > 28 Then Return Nothing '  currently carbon > 28 is recognized as EtherPC
                    ' seek 184.07332 (C5H15NO4P)
                    Dim threshold = 3.0
                    Dim diagnosticMz = 184.07332
                    Dim diagnosticMz2 = 104.106990
                    Dim isClassIon1Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    If isClassIon1Found <> True Then Return Nothing
                    ' reject Na+ adduct
                    Dim diagnosticMz3 = theoreticalMz - 59.0735
                    Dim isNaTypicalFound1 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz3, 10.0)
                    If isNaTypicalFound1 Then
                        Return Nothing
                    End If

                    ' for eieio
                    Dim PEHeaderLoss = theoreticalMz - 141.019094261 + ProtonMass
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, PEHeaderLoss, 3.0)
                    If isClassIonFound2 AndAlso LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, PEHeaderLoss, diagnosticMz) Then
                        Return Nothing
                    End If
                    '
                    Dim candidates = New List(Of LipidMolecule)()
                    Dim chainSuffix = ""
                    Dim diagnosticMzExist = 0.0
                    Dim diagnosticMzIntensity = 0.0
                    Dim diagnosticMzExist2 = 0.0
                    Dim diagnosticMzIntensity2 = 0.0

                    For i = 0 To spectrum.Count - 1
                        Dim mz = spectrum(i).Mass
                        Dim intensity = spectrum(i).Intensity

                        If intensity > threshold AndAlso Math.Abs(mz - diagnosticMz) < ms2Tolerance Then
                            diagnosticMzExist = mz
                            diagnosticMzIntensity = intensity
                        ElseIf intensity > threshold AndAlso Math.Abs(mz - diagnosticMz2) < ms2Tolerance Then
                            diagnosticMzExist2 = mz
                            diagnosticMzIntensity2 = intensity
                        End If
                    Next

                    If diagnosticMzIntensity2 / diagnosticMzIntensity > 0.3 Then '
                        chainSuffix = "/0:0"
                    End If

                    Dim score = 0.0
                    If totalCarbon < 30 Then score = score + 1.0
                    Dim molecule = LipidMsmsCharacterizationUtility.getSingleacylchainwithsuffixMoleculeObjAsLevel2("LPC_d5", LbmClass.LPC_d5, totalCarbon, totalDoubleBond, score, chainSuffix)
                    candidates.Add(molecule)

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LPC_d5", LbmClass.LPC_d5, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                    If totalCarbon > 28 Then Return Nothing '  currently carbon > 28 is recognized as EtherPC
                    ' seek PreCursor - 59 (C3H9N)
                    Dim threshold = 10.0
                    Dim diagnosticMz = theoreticalMz - 59.072951
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    If isClassIonFound = False Then Return Nothing
                    ' seek 104.1070 (C5H14NO) maybe not found
                    'var threshold2 = 1.0;
                    'var diagnosticMz2 = 104.1070;

                    '
                    Dim candidates = New List(Of LipidMolecule)()
                    Dim score = 0.0
                    If totalCarbon < 30 Then score = score + 1.0
                    Dim molecule = LipidMsmsCharacterizationUtility.getSingleacylchainMoleculeObjAsLevel2("LPC_d5", LbmClass.LPC_d5, totalCarbon, totalDoubleBond, score)
                    candidates.Add(molecule)

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LPC_d5", LbmClass.LPC_d5, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                End If
            End If
            Return Nothing
        End Function

        Public Shared Function JudgeIfLysopeD5(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, snCarbon As Integer, snDoubleBond As Integer, adduct As AdductIon) As LipidMolecule ' 
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    If totalCarbon > 28 Then Return Nothing '  currently carbon > 28 is recognized as EtherPE
                    Dim Gly_C As Double = {CarbonMass * 5, HydrogenMass * 7, NitrogenMass, OxygenMass * 4, PhosphorusMass, Hydrogen2Mass * 5, Proton}.Sum()

                    Dim Gly_O As Double = {CarbonMass * 4, HydrogenMass * 7, NitrogenMass, OxygenMass * 5, PhosphorusMass, Hydrogen2Mass * 3, Proton}.Sum()
                    Dim C2H8NO4P As Double = {CarbonMass * 2, HydrogenMass * 8, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()
                    ' seek -141.019094261 (C2H8NO4P)
                    Dim threshold = 2.5
                    Dim diagnosticMz = theoreticalMz - C2H8NO4P
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C, threshold)
                    Dim isClassIonFound3 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O, threshold)
                    If If(isClassIonFound, 1, 0) + If(isClassIonFound2, 1, 0) + If(isClassIonFound3, 1, 0) < 2 Then
                        Return Nothing
                    End If
                    'if (!isClassIonFound || !isClassIonFound2 || !isClassIonFound3) return null;

                    ' reject EtherPE 
                    Dim sn1alkyl = CarbonMass * snCarbon + HydrogenMass * (snCarbon * 2 - snDoubleBond * 2 + 1) 'sn1(ether)

                    Dim NL_sn1 = diagnosticMz - sn1alkyl + Proton
                    Dim sn1_rearrange = sn1alkyl + HydrogenMass * 2 + 139.00290 'sn1(ether) + C2H5NO4P + proton 

                    Dim isClassIon2Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, NL_sn1, threshold)
                    Dim isClassIon3Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, sn1_rearrange, threshold)
                    If isClassIon2Found = True OrElse isClassIon3Found = True Then Return Nothing


                    Dim candidates = New List(Of LipidMolecule)()
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LPE_d5", LbmClass.LPE_d5, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                    ' seek PreCursor -141(C2H8NO4P)
                    Dim threshold = 10.0
                    Dim diagnosticMz = theoreticalMz - 141.019094
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    If isClassIonFound = False Then Return Nothing

                    ' reject EtherPE 
                    Dim sn1alkyl = CarbonMass * snCarbon + HydrogenMass * (snCarbon * 2 - snDoubleBond * 2 + 1) 'sn1(ether)

                    Dim NL_sn1 = diagnosticMz - sn1alkyl + Proton
                    Dim sn1_rearrange = sn1alkyl + 139.00290 + HydrogenMass * 2 'sn1(ether) + C2H5NO4P + proton 

                    Dim isClassIon2Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, NL_sn1, threshold)
                    Dim isClassIon3Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, sn1_rearrange, threshold)
                    If isClassIon2Found = True OrElse isClassIon3Found = True Then Return Nothing

                    '
                    Dim candidates = New List(Of LipidMolecule)()
                    'var score = 0.0;
                    'if (totalCarbon < 30) score = score + 1.0;
                    'var molecule = getSingleacylchainMoleculeObjAsLevel2("LPE_d5", LbmClass.LPE_d5, totalCarbon, totalDoubleBond,
                    'score);
                    'candidates.Add(molecule);
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LPE_d5", LbmClass.LPE_d5, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                End If
            End If
            Return Nothing
        End Function
        Public Shared Function JudgeIfLysopgD5(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, snCarbon As Integer, snDoubleBond As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PE 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Negative Then '
                If Equals(adduct.AdductIonName, "[M-H]-") Then
                    ' seek C3H6O5P-

                    Dim diagnosticMz1 = 152.99583  ' seek C3H6O5P-
                    Dim threshold1 = 1.0
                    Dim diagnosticMz2 = LipidMsmsCharacterizationUtility.fattyacidProductIon(totalCarbon, totalDoubleBond) ' seek [FA-H]-
                    Dim threshold2 = 10.0
                    Dim isClassIon1Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz1, threshold1)
                    Dim isClassIon2Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz2, threshold2)
                    If isClassIon1Found <> True OrElse isClassIon2Found <> True Then Return Nothing

                    '
                    Dim candidates = New List(Of LipidMolecule)()
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LPG_d5", LbmClass.LPG_d5, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                End If
            ElseIf adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    ' seek Header loss (MG+ + chain Acyl) 
                    Dim threshold = 5.0
                    Dim diagnosticMz = LipidMsmsCharacterizationUtility.acylCainMass(snCarbon, snDoubleBond) + (12 * 3 + Hydrogen2Mass * 5 + OxygenMass * 2) + ProtonMass
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    If isClassIonFound = False Then Return Nothing

                    Dim candidates = New List(Of LipidMolecule)()
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LPG_d5", LbmClass.LPG_d5, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                End If
            End If

            Return Nothing
        End Function

        Public Shared Function JudgeIfLysopiD5(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, snCarbon As Integer, snDoubleBond As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PE 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Negative Then 'negative ion mode only
                If Equals(adduct.AdductIonName, "[M-H]-") Then
                    ' seek C3H6O5P-

                    Dim diagnosticMz1 = 241.0118806 + Electron  ' seek C3H6O5P-
                    Dim threshold1 = 1.0
                    Dim diagnosticMz2 = 315.048656 ' seek C9H16O10P-
                    Dim threshold2 = 1.0
                    Dim diagnosticMz3 = LipidMsmsCharacterizationUtility.fattyacidProductIon(totalCarbon, totalDoubleBond) ' seek [FA-H]-
                    Dim threshold3 = 10.0
                    Dim isClassIon1Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz1, threshold1)
                    Dim isClassIon2Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz2, threshold2)
                    Dim isClassIon3Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz3, threshold3)
                    If isClassIon1Found <> True OrElse isClassIon2Found <> True OrElse isClassIon3Found <> True Then Return Nothing

                    '
                    Dim candidates = New List(Of LipidMolecule)()
                    'var averageIntensity = 0.0;

                    'var molecule = getSingleacylchainMoleculeObjAsLevel2("LPI_d5", LbmClass.LPI_d5, totalCarbon, totalDoubleBond,
                    'averageIntensity);
                    'candidates.Add(molecule);
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LPI_d5", LbmClass.LPI_d5, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                End If
            ElseIf adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    ' seek Header loss (MG+ + chain Acyl) 
                    Dim threshold = 5.0
                    Dim diagnosticMz = LipidMsmsCharacterizationUtility.acylCainMass(snCarbon, snDoubleBond) + (12 * 3 + Hydrogen2Mass * 5 + OxygenMass * 2) + ProtonMass
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    If isClassIonFound = False Then Return Nothing

                    Dim candidates = New List(Of LipidMolecule)()
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LPI_d5", LbmClass.LPI_d5, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                End If
            End If
            Return Nothing
        End Function
        Public Shared Function JudgeIfLysopsD5(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, snCarbon As Integer, snDoubleBond As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PE 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Negative Then 'negative ion mode only
                If Equals(adduct.AdductIonName, "[M-H]-") Then
                    ' seek C3H6O5P-

                    Dim diagnosticMz1 = 152.99583  ' seek C3H6O5P-
                    Dim threshold1 = 10.0
                    Dim diagnosticMz2 = theoreticalMz - 87.032029 ' seek -C3H6NO2-H
                    Dim threshold2 = 5.0
                    Dim isClassIon1Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz1, threshold1)
                    Dim isClassIon2Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz2, threshold2)
                    If isClassIon1Found <> True OrElse isClassIon2Found <> True Then Return Nothing

                    '
                    Dim candidates = New List(Of LipidMolecule)()
                    'var averageIntensity = 0.0;

                    'var molecule = getSingleacylchainMoleculeObjAsLevel2("LPS_d5", LbmClass.LPS_d5, totalCarbon, totalDoubleBond,
                    'averageIntensity);
                    'candidates.Add(molecule);
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LPS_d5", LbmClass.LPS_d5, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                End If
            ElseIf adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    ' seek Header loss (MG+ + chain Acyl) 
                    Dim threshold = 5.0
                    Dim diagnosticMz = LipidMsmsCharacterizationUtility.acylCainMass(snCarbon, snDoubleBond) + (12 * 3 + Hydrogen2Mass * 5 + OxygenMass * 2) + ProtonMass
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    If isClassIonFound = False Then Return Nothing

                    Dim candidates = New List(Of LipidMolecule)()
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LPS_d5", LbmClass.LPS_d5, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                End If
            End If
            Return Nothing
        End Function
        Public Shared Function JudgeIfDagD5(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sn1Carbon As Integer, sn2Carbon As Integer, sn1Double As Integer, sn2Double As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PS 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If totalCarbon > 52 Then Return Nothing ' currently, very large DAG is excluded.
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+NH4]+") Then
                    ' seek -17.026549 (NH3)
                    Dim threshold = 1.0
                    Dim diagnosticMz = theoreticalMz - 17.026549
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)

                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()
                    If sn2Double >= 7 Then Return Nothing

                    Dim nl_SN1 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1Carbon, sn1Double) - H2O + HydrogenMass
                    Dim nl_SN2 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2Carbon, sn2Double) - H2O + HydrogenMass

                    'Console.WriteLine(sn1Carbon + ":" + sn1Double + "-" + sn2Carbon + ":" + sn2Double + 
                    '    " " + nl_SN1 + " " + nl_SN2);

                    Dim query = New List(Of SpectrumPeak) From {
            New SpectrumPeak() With {
        .Mass = nl_SN1,
        .Intensity = 5
    },
            New SpectrumPeak() With {
        .Mass = nl_SN2,
        .Intensity = 5
    }
        }
                    Dim foundCount = 0
                    Dim averageIntensity = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)
                    If foundCount = 2 Then
                        Dim molecule = LipidMsmsCharacterizationUtility.getDiacylglycerolMoleculeObjAsLevel2("DG_d5", LbmClass.DG_d5, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity)
                        candidates.Add(molecule)
                    End If
                    If candidates Is Nothing OrElse candidates.Count = 0 Then Return Nothing


                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("DG_d5", LbmClass.DG_d5, String.Empty, theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                    ' from here, acyl level annotation is executed.
                    Dim diagnosticMz = theoreticalMz
                    Dim candidates = New List(Of LipidMolecule)()
                    For sn1CarbonNum = 6 To totalCarbon
                        For sn1DoubleNum = 0 To totalDoubleBond
                            Dim sn2CarbonNum = totalCarbon - sn1CarbonNum
                            Dim sn2DoubleNum = totalDoubleBond - sn1DoubleNum
                            If sn2DoubleNum >= 7 Then Continue For

                            Dim nl_SN1 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1CarbonNum, sn1DoubleNum) - H2O + HydrogenMass
                            Dim nl_SN2 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2CarbonNum, sn2DoubleNum) - H2O + HydrogenMass

                            'Console.WriteLine(sn1Carbon + ":" + sn1Double + "-" + sn2Carbon + ":" + sn2Double + 
                            '    " " + nl_SN1 + " " + nl_SN2);

                            Dim query = New List(Of SpectrumPeak) From {
    New SpectrumPeak() With {
        .Mass = nl_SN1,
        .Intensity = 1
    },
    New SpectrumPeak() With {
        .Mass = nl_SN2,
        .Intensity = 1
    }
}
                            Dim foundCount = 0
                            Dim averageIntensity = 0.0
                            LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)
                            If foundCount = 2 Then
                                Dim molecule = LipidMsmsCharacterizationUtility.getDiacylglycerolMoleculeObjAsLevel2("DG", LbmClass.DG, sn1CarbonNum, sn1DoubleNum, sn2CarbonNum, sn2DoubleNum, averageIntensity)
                                candidates.Add(molecule)
                            End If
                        Next
                    Next
                    If candidates Is Nothing OrElse candidates.Count = 0 Then Return Nothing


                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("DG_d5", LbmClass.DG_d5, String.Empty, theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)

                End If
            End If
            Return Nothing
        End Function
        Public Shared Function JudgeIfTriacylglycerolD5(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sn1Carbon As Integer, sn2Carbon As Integer, sn3Carbon As Integer, sn1Double As Integer, sn2Double As Integer, sn3Double As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PS 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+NH4]+") Then
                    ' seek -17.026549 (NH3)
                    Dim threshold = 1.0
                    Dim diagnosticMz = theoreticalMz - 17.026549
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)

                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()
                    If sn1Carbon = 18 AndAlso sn1Double = 5 OrElse sn2Carbon = 18 AndAlso sn2Double = 5 OrElse sn3Carbon = 18 AndAlso sn3Double = 5 Then Return Nothing

                    Dim nl_SN1 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1Carbon, sn1Double) - H2O + HydrogenMass
                    Dim nl_SN2 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2Carbon, sn2Double) - H2O + HydrogenMass
                    Dim nl_SN3 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn3Carbon, sn3Double) - H2O + HydrogenMass
                    Dim query = New List(Of SpectrumPeak) From {
                                        New SpectrumPeak() With {
                            .Mass = nl_SN1,
                            .Intensity = 5
                        },
                                        New SpectrumPeak() With {
                            .Mass = nl_SN2,
                            .Intensity = 5
                        },
                                        New SpectrumPeak() With {
                            .Mass = nl_SN3,
                            .Intensity = 5
                        }
                                    }

                    Dim foundCount = 0
                    Dim averageIntensity = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                    If foundCount = 3 Then ' these three chains must be observed.
                        Dim C2H3O As Double = {CarbonMass * 2, HydrogenMass * 3, OxygenMass, Proton}.Sum()

                        Dim sn1Sn2Diagnostics = LipidMsmsCharacterizationUtility.acylCainMass(sn1Carbon, sn1Double) + C2H3O
                        Dim sn2Sn2Diagnostics = LipidMsmsCharacterizationUtility.acylCainMass(sn2Carbon, sn2Double) + C2H3O
                        Dim sn3Sn2Diagnostics = LipidMsmsCharacterizationUtility.acylCainMass(sn3Carbon, sn3Double) + C2H3O
                        Dim querySn2 = New List(Of SpectrumPeak) From {
                                        New SpectrumPeak() With {
                                .Mass = sn1Sn2Diagnostics,
                                .Intensity = 3.0
                            },
                                        New SpectrumPeak() With {
                                .Mass = sn2Sn2Diagnostics,
                                .Intensity = 3.0
                            },
                                        New SpectrumPeak() With {
                                .Mass = sn3Sn2Diagnostics,
                                .Intensity = 3.0
                            }
                                    }

                        Dim foundCountSn2 = 0
                        Dim averageIntensitySn2 = 0.0
                        LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, querySn2, ms2Tolerance, foundCountSn2, averageIntensitySn2)
                        If foundCountSn2 > 0 Then

                        End If

                        Dim molecule = LipidMsmsCharacterizationUtility.getTriacylglycerolMoleculeObjAsLevel2("TG_d5", LbmClass.TG_d5, sn1Carbon, sn1Double, sn2Carbon, sn2Double, sn3Carbon, sn3Double, averageIntensity)
                        candidates.Add(molecule)
                    End If
                    If candidates Is Nothing OrElse candidates.Count = 0 Then Return Nothing
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("TG_d5", LbmClass.TG_d5, String.Empty, theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 3)
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then   'add MT
                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()
                    Dim diagnosticMz = theoreticalMz ' - 22.9892207 + MassDiffDictionary.HydrogenMass; //if want to choose [M+H]+
                    Dim nl_SN1 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1Carbon, sn1Double) - H2O + HydrogenMass
                    Dim nl_SN2 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2Carbon, sn2Double) - H2O + HydrogenMass
                    Dim nl_SN3 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(sn3Carbon, sn3Double) - H2O + HydrogenMass
                    Dim query = New List(Of SpectrumPeak) From {
                                        New SpectrumPeak() With {
                            .Mass = nl_SN1,
                            .Intensity = 0.1
                        },
                                        New SpectrumPeak() With {
                            .Mass = nl_SN2,
                            .Intensity = 0.1
                        },
                                        New SpectrumPeak() With {
                            .Mass = nl_SN3,
                            .Intensity = 0.1
                        }
                                    }

                    Dim foundCount = 0
                    Dim averageIntensity = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                    If foundCount < 3 Then
                        Dim diagnosticMzH = theoreticalMz - 22.9892207 + HydrogenMass
                        Dim nl_SN1_H = diagnosticMzH - LipidMsmsCharacterizationUtility.acylCainMass(sn1Carbon, sn1Double) - H2O + HydrogenMass
                        Dim nl_SN2_H = diagnosticMzH - LipidMsmsCharacterizationUtility.acylCainMass(sn2Carbon, sn2Double) - H2O + HydrogenMass
                        Dim nl_SN3_H = diagnosticMzH - LipidMsmsCharacterizationUtility.acylCainMass(sn3Carbon, sn3Double) - H2O + HydrogenMass
                        Dim query2 = New List(Of SpectrumPeak) From {
                                        New SpectrumPeak() With {
                                .Mass = nl_SN1_H,
                                .Intensity = 0.1
                            },
                                        New SpectrumPeak() With {
                                .Mass = nl_SN2_H,
                                .Intensity = 0.1
                            },
                                        New SpectrumPeak() With {
                                .Mass = nl_SN3_H,
                                .Intensity = 0.1
                            }
                                        }

                        Dim foundCount2 = 0
                        Dim averageIntensity2 = 0.0
                        LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query2, ms2Tolerance, foundCount2, averageIntensity2)

                        If foundCount2 = 3 Then
                            Dim molecule = LipidMsmsCharacterizationUtility.getTriacylglycerolMoleculeObjAsLevel2("TG_d5", LbmClass.TG_d5, sn1Carbon, sn1Double, sn2Carbon, sn2Double, sn3Carbon, sn3Double, averageIntensity2)
                            candidates.Add(molecule)
                        End If
                    ElseIf foundCount = 3 Then ' these three chains must be observed.
                        Dim molecule = LipidMsmsCharacterizationUtility.getTriacylglycerolMoleculeObjAsLevel2("TG_d5", LbmClass.TG_d5, sn1Carbon, sn1Double, sn2Carbon, sn2Double, sn3Carbon, sn3Double, averageIntensity)
                        candidates.Add(molecule)
                    End If
                    If candidates Is Nothing OrElse candidates.Count = 0 Then Return Nothing
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("TG_d5", LbmClass.TG_d5, String.Empty, theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 3)
                End If
            End If
            Return Nothing
        End Function
        Public Shared Function JudgeIfSphingomyelinD9(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sphCarbon As Integer, acylCarbon As Integer, sphDouble As Integer, acylDouble As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PC 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            Dim C5H5D9NO4P = {CarbonMass * 5, HydrogenMass * 5, NitrogenMass, OxygenMass * 4, PhosphorusMass, Hydrogen2Mass * 9}.Sum()

            Dim C2H2N = 12 * 2 + HydrogenMass * 2 + NitrogenMass
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then

                    ' seek 184.07332 (C5H15NO4P) D9
                    Dim threshold = 10.0
                    Dim diagnosticMz = C5H5D9NO4P + ProtonMass
                    Dim diagnosticMz1 = C5H5D9NO4P + 12 * 2 + HydrogenMass * 3 + NitrogenMass + Proton
                    Dim diagnosticMz2 = C5H5D9NO4P + 12 * 3 + HydrogenMass * 3 + NitrogenMass + OxygenMass + Proton
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    Dim isClassIonFound1 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz1, 1.0)
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz2, 1.0)
                    If Not isClassIonFound OrElse Not isClassIonFound1 OrElse Not isClassIonFound2 Then Return Nothing

                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()
                    If sphCarbon <= 13 Then Return Nothing
                    If sphCarbon = 16 AndAlso sphDouble >= 3 Then Return Nothing
                    If acylCarbon < 8 Then Return Nothing

                    Dim diagnosChain1 = LipidMsmsCharacterizationUtility.acylCainMass(acylCarbon, acylDouble) + C2H2N + HydrogenMass + Proton
                    Dim diagnosChain2 = diagnosChain1 + C5H5D9NO4P - HydrogenMass

                    Dim query = New List(Of SpectrumPeak) From {
                        New SpectrumPeak() With {
                            .Mass = diagnosChain1,
                            .Intensity = 0.5
                        },
                        New SpectrumPeak() With {
                            .Mass = diagnosChain2,
                            .Intensity = 1.0
                        }
                    }

                    Dim foundCount = 0
                    Dim averageIntensity = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                    If foundCount = 2 Then ' the diagnostic acyl ion must be observed for level 2 annotation
                        Dim molecule = LipidMsmsCharacterizationUtility.getCeramideMoleculeObjAsLevel2("SM_d9", LbmClass.SM_d9, "d", sphCarbon, sphDouble, acylCarbon, acylDouble, averageIntensity)
                        candidates.Add(molecule)
                    Else
                        Return Nothing
                    End If
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("SM_d9", LbmClass.SM_d9, "d", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                    ' seek -59.0735 [M-C3H9N+Na]+
                    Dim threshold = 20.0
                    Dim diagnosticMz = theoreticalMz - 59.0735
                    ' seek C5H15NO4P + Na+
                    Dim threshold2 = 30.0
                    Dim diagnosticMz2 = C5H5D9NO4P + Na

                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    Dim isClassIon2Found = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz2, threshold2)
                    If isClassIonFound = False OrElse isClassIon2Found = False Then Return Nothing

                    Dim candidates = New List(Of LipidMolecule)()
                    ' from here, acyl level annotation is executed.
                    For sphCarbonNum = 6 To totalCarbon
                        For sphDoubleNum = 0 To totalDoubleBond
                            Dim acylCarbonNum = totalCarbon - sphCarbonNum
                            Dim acylDoubleNum = totalDoubleBond - sphDoubleNum
                            If acylDoubleNum >= 7 Then Continue For

                            Dim diagnosChain1 = LipidMsmsCharacterizationUtility.acylCainMass(acylCarbonNum, acylDoubleNum) + C2H2N + HydrogenMass + ProtonMass
                            Dim diagnosChain2 = diagnosChain1 + C5H5D9NO4P - HydrogenMass

                            Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                    .Mass = diagnosChain1,
                                    .Intensity = 20.0
                                },
                                New SpectrumPeak() With {
                                    .Mass = diagnosChain2,
                                    .Intensity = 20.0
                                }
                            }
                            Dim foundCount = 0
                            Dim averageIntensity = 0.0
                            LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)
                            If foundCount = 2 Then
                                Dim molecule = LipidMsmsCharacterizationUtility.getCeramideMoleculeObjAsLevel2("SM_d9", LbmClass.SM_d9, "d", sphCarbon, sphDouble, acylCarbon, acylDouble, averageIntensity)
                                candidates.Add(molecule)
                            End If
                        Next
                    Next
                    If candidates Is Nothing OrElse candidates.Count = 0 Then Return Nothing

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("SM_d9", LbmClass.SM_d9, "d", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                End If
            End If

            Return Nothing
        End Function
        Public Shared Function JudgeIfCeramidensD7(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, sphCarbon As Integer, acylCarbon As Integer, sphDouble As Integer, acylDouble As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PC 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                Dim adductform = adduct.AdductIonName
                If Equals(adductform, "[M+H]+") OrElse Equals(adductform, "[M+H-H2O]+") Then
                    ' seek -H2O
                    Dim threshold = 5.0
                    Dim diagnosticMz = If(Equals(adductform, "[M+H]+"), theoreticalMz - H2O, theoreticalMz)
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)

                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()
                    If acylDouble >= 7 Then Return Nothing
                    Dim sph1 = diagnosticMz - LipidMsmsCharacterizationUtility.acylCainMass(acylCarbon, acylDouble) + HydrogenMass
                    Dim sph2 = sph1 - H2O
                    Dim sph3 = sph2 - 12 '[Sph-CH4O2+H]+
                    Dim acylamide = acylCarbon * 12 + (2 * acylCarbon - 2 * acylDouble + 2) * HydrogenMass + OxygenMass + NitrogenMass

                    ' must query
                    Dim queryMust = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                            .Mass = sph2,
                            .Intensity = 5
                        }
                            }
                    Dim foundCountMust = 0
                    Dim averageIntensityMust = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, queryMust, ms2Tolerance, foundCountMust, averageIntensityMust)
                    If foundCountMust = 0 Then Return Nothing

                    Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                            .Mass = sph1,
                            .Intensity = 1
                        },
                                New SpectrumPeak() With {
                            .Mass = sph3,
                            .Intensity = 1
                        }
                            }

                    Dim foundCount = 0
                    Dim averageIntensity = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)
                    Dim foundCountThresh = If(acylCarbon < 12, 2, 1) ' to exclude strange annotation

                    If foundCount >= foundCountThresh Then ' 
                        Dim molecule = LipidMsmsCharacterizationUtility.getCeramideMoleculeObjAsLevel2("Cer_d7", LbmClass.Cer_NS_d7, "d", sphCarbon, sphDouble, acylCarbon, acylDouble, averageIntensity)
                        candidates.Add(molecule)
                    End If
                    If candidates.Count = 0 Then Return Nothing

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("Cer_d7", LbmClass.Cer_NS_d7, "d", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                ElseIf Equals(adductform, "[M+Na]+") Then
                    ' reject HexCer
                    Dim threshold = 1.0
                    Dim diagnosticMz = theoreticalMz - 162.052833 - H2O
                    If LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold) Then
                        Return Nothing
                    End If
                    Dim candidates = New List(Of LipidMolecule)()
                    Dim sph1 = LipidMsmsCharacterizationUtility.SphingoChainMass(sphCarbon, sphDouble) + HydrogenMass - OxygenMass + HydrogenMass * 7
                    Dim sph3 = sph1 - H2O + Proton

                    Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                            .Mass = sph3,
                            .Intensity = 1
                        }
                            }

                    Dim foundCount = 0
                    Dim averageIntensity = 0.0
                    LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                    If foundCount = 1 Then ' 
                        Dim molecule = LipidMsmsCharacterizationUtility.getCeramideMoleculeObjAsLevel2("Cer_d7", LbmClass.Cer_NS_d7, "d", sphCarbon, sphDouble, acylCarbon, acylDouble, averageIntensity)
                        candidates.Add(molecule)
                    End If
                    If candidates.Count = 0 Then Return Nothing
                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("Cer_d7", LbmClass.Cer_NS_d7, "d", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                End If
            End If

            Return Nothing
        End Function
        Public Shared Function JudgeIfCholesterylEsterD7(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Single, totalCarbon As Integer, totalDoubleBond As Integer, adduct As AdductIon) As LipidMolecule
            Dim skelton As Double = {CarbonMass * 27, HydrogenMass * 46, OxygenMass * 1, Hydrogen2Mass * 7, -HydrogenMass * 7}.Sum()
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+NH4]+") Then
                    ' seek 369.3515778691 (C27H45+)+ MassDiffDictionary.HydrogenMass*7
                    Dim threshold = 20.0
                    Dim diagnosticMz = skelton - H2O + Proton
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    If isClassIonFound = False Then Return Nothing
                    If totalCarbon >= 41 AndAlso totalDoubleBond >= 4 Then Return Nothing

                    Dim candidates = New List(Of LipidMolecule)()

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("CE_d7", LbmClass.CE_d7, String.Empty, theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                ElseIf Equals(adduct.AdductIonName, "[M+Na]+") Then
                    ' seek 368.3515778691 (C27H44)+ MassDiffDictionary.HydrogenMass*7
                    Dim threshold = 10.0
                    Dim diagnosticMz = skelton - H2O
                    Dim isClassIonFound = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, diagnosticMz, threshold)
                    If isClassIonFound = False Then Return Nothing

                    Dim candidates = New List(Of LipidMolecule)()

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("CE_d7", LbmClass.CE_d7, String.Empty, theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                End If

            End If
            Return Nothing
        End Function
        Public Shared Function JudgeIfDgts(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, minSnCarbon As Integer, maxSnCarbon As Integer, minSnDoubleBond As Integer, maxSnDoubleBond As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PC 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If maxSnCarbon > totalCarbon Then maxSnCarbon = totalCarbon
            If maxSnDoubleBond > totalDoubleBond Then maxSnDoubleBond = totalDoubleBond
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    Dim Gly_C As Double = {CarbonMass * 10, HydrogenMass * 19, NitrogenMass, OxygenMass * 3, Proton}.Sum()

                    Dim Gly_O As Double = {CarbonMass * 9, HydrogenMass * 17, NitrogenMass, OxygenMass * 4, Proton}.Sum()

                    Dim threshold = 1.0
                    Dim isClassIonFound1 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C, threshold)
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O, threshold)
                    If Not isClassIonFound1 AndAlso Not isClassIonFound2 Then Return Nothing

                    ' from here, acyl level annotation is executed.
                    Dim candidates = New List(Of LipidMolecule)()
                    For sn1Carbon = minSnCarbon To maxSnCarbon
                        For sn1Double = minSnDoubleBond To maxSnDoubleBond

                            Dim sn2Carbon = totalCarbon - sn1Carbon
                            Dim sn2Double = totalDoubleBond - sn1Double

                            Dim nl_SN1 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn1Carbon, sn1Double) + HydrogenMass
                            Dim nl_SN1_H2O = nl_SN1 - H2O

                            Dim nl_SN2 = theoreticalMz - LipidMsmsCharacterizationUtility.acylCainMass(sn2Carbon, sn2Double) + HydrogenMass
                            Dim nl_NS2_H2O = nl_SN2 - H2O

                            Dim query = New List(Of SpectrumPeak) From {
                                New SpectrumPeak() With {
                                    .Mass = nl_SN1,
                                    .Intensity = 0.01
                                },
                                New SpectrumPeak() With {
                                    .Mass = nl_SN1_H2O,
                                    .Intensity = 0.01
                                },
                                New SpectrumPeak() With {
                                    .Mass = nl_SN2,
                                    .Intensity = 0.01
                                },
                                New SpectrumPeak() With {
                                    .Mass = nl_NS2_H2O,
                                    .Intensity = 0.01
                                }
                            }

                            Dim foundCount = 0
                            Dim averageIntensity = 0.0
                            LipidMsmsCharacterizationUtility.countFragmentExistence(spectrum, query, ms2Tolerance, foundCount, averageIntensity)

                            If foundCount >= 2 Then ' now I set 2 as the correct level
                                Dim DgtsFrag = 130.0862 '130.0862 (C6H12NO2+)DGTS
                                Dim DgtaFrag = 144.10191 '144.10191 (C7H14NO2+) DGTA

                                Dim molecule = New LipidMolecule()
                                If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, DgtsFrag, DgtaFrag) Then
                                    molecule = LipidMsmsCharacterizationUtility.getPhospholipidMoleculeObjAsLevel2("DGTS", LbmClass.DGTS, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity)
                                Else
                                    molecule = LipidMsmsCharacterizationUtility.getPhospholipidMoleculeObjAsLevel2("DGTA", LbmClass.DGTA, sn1Carbon, sn1Double, sn2Carbon, sn2Double, averageIntensity)
                                End If
                                candidates.Add(molecule)
                            End If
                        Next
                    Next
                    'var score = 0;
                    'var molecule0 = getLipidAnnotaionAsLevel1("DGTS", LbmClass.DGTS, totalCarbon, totalDoubleBond, score, "");
                    'candidates.Add(molecule0);

                    Return LipidMsmsCharacterizationUtility.returnAnnotationResult("DGTS", LbmClass.DGTS, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 2)
                End If
            End If
            Return Nothing
        End Function

        Public Shared Function JudgeIfLdgts(msScanProp As IMSScanProperty, ms2Tolerance As Double, theoreticalMz As Double, totalCarbon As Integer, totalDoubleBond As Integer, minSnCarbon As Integer, maxSnCarbon As Integer, minSnDoubleBond As Integer, maxSnDoubleBond As Integer, adduct As AdductIon) As LipidMolecule ' If the candidate PC 46:6, totalCarbon = 46 and totalDoubleBond = 6
            Dim spectrum = msScanProp.Spectrum
            If spectrum Is Nothing OrElse spectrum.Count = 0 Then Return Nothing
            If maxSnCarbon > totalCarbon Then maxSnCarbon = totalCarbon
            If maxSnDoubleBond > totalDoubleBond Then maxSnDoubleBond = totalDoubleBond
            If adduct.IonMode = IonMode.Positive Then ' positive ion mode 
                If Equals(adduct.AdductIonName, "[M+H]+") Then
                    Dim Gly_C As Double = {CarbonMass * 10, HydrogenMass * 19, NitrogenMass, OxygenMass * 3, Proton}.Sum()

                    Dim Gly_O As Double = {CarbonMass * 9, HydrogenMass * 17, NitrogenMass, OxygenMass * 4, Proton}.Sum()

                    Dim threshold = 1.0
                    Dim isClassIonFound1 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_C, threshold)
                    Dim isClassIonFound2 = LipidMsmsCharacterizationUtility.isDiagnosticFragmentExist(spectrum, ms2Tolerance, Gly_O, threshold)
                    If Not isClassIonFound1 AndAlso Not isClassIonFound2 Then Return Nothing

                    Dim candidates = New List(Of LipidMolecule)()
                    Dim DgtsFrag = 130.0862 '130.0862 (C6H12NO2+)DGTS
                    Dim DgtaFrag = 144.10191 '144.10191 (C7H14NO2+) DGTA

                    If LipidMsmsCharacterizationUtility.isFragment1GreaterThanFragment2(spectrum, ms2Tolerance, DgtsFrag, DgtaFrag) Then
                        Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LDGTS", LbmClass.LDGTS, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                    Else
                        Return LipidMsmsCharacterizationUtility.returnAnnotationResult("LDGTA", LbmClass.LDGTA, "", theoreticalMz, adduct, totalCarbon, totalDoubleBond, 0, candidates, 1)
                    End If

                End If
            End If
            Return Nothing
        End Function

    End Class

