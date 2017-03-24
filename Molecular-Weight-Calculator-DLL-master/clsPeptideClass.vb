Option Strict On

Imports System.Collections.Generic

Public Class MWPeptideClass

    ' Molecular Weight Calculator routines with ActiveX Class interfaces: MWPeptideClass

    ' -------------------------------------------------------------------------------
    ' Written by Matthew Monroe for the Department of Energy (PNNL, Richland, WA) in 2004
    ' E-mail: matthew.monroe@pnnl.gov or matt@alchemistmatt.com
    ' Website: http://ncrr.pnnl.gov/ or http://www.sysbio.org/resources/staff/
    ' -------------------------------------------------------------------------------
    ' 
    ' Licensed under the Apache License, Version 2.0; you may not use this file except
    ' in compliance with the License.  You may obtain a copy of the License at 
    ' http://www.apache.org/licenses/LICENSE-2.0
    '
    ' Notice: This computer software was prepared by Battelle Memorial Institute, 
    ' hereinafter the Contractor, under Contract No. DE-AC05-76RL0 1830 with the 
    ' Department of Energy (DOE).  All rights in the computer software are reserved 
    ' by DOE on behalf of the United States Government and the Contractor as 
    ' provided in the Contract.  NEITHER THE GOVERNMENT NOR THE CONTRACTOR MAKES ANY 
    ' WARRANTY, EXPRESS OR IMPLIED, OR ASSUMES ANY LIABILITY FOR THE USE OF THIS 
    ' SOFTWARE.  This notice including this sentence must appear on any copies of 
    ' this computer software.

    Public Sub New()
        MyBase.New()
        ElementAndMassRoutines = New MWElementAndMassRoutines
        InitializeClass()
    End Sub

    Public Sub New(ByVal objMWElementAndMassRoutines As MWElementAndMassRoutines)
        MyBase.New()
        ElementAndMassRoutines = objMWElementAndMassRoutines
        InitializeClass()
    End Sub


    Public Const DEFAULT_B_Y_ION_SHOULDER_INTENSITY As Single = 50
    Public Const DEFAULT_BYCZ_ION_INTENSITY As Single = 100
    Public Const DEFAULT_A_ION_INTENSITY As Single = 20
    Public Const DEFAULT_NEUTRAL_LOSS_ION_INTENSITY As Single = 20

    Public Const DEFAULT_DOUBLE_CHARGE_MZ_THRESHOLD As Single = 800
    Public Const DEFAULT_TRIPLE_CHARGE_MZ_THRESHOLD As Single = 900

    Private Const RESIDUE_DIM_CHUNK As Short = 50
    Private Const MAX_MODIFICATIONS As Short = 6 ' Maximum number of modifications for a single residue
    Private Const UNKNOWN_SYMBOL As String = "Xxx"
    Private Const UNKNOWN_SYMBOL_ONE_LETTER As String = "X"

    Private Const TERMINII_SYMBOL As String = "-"
    Private Const TRYPTIC_RULE_RESIDUES As String = "KR"
    Private Const TRYPTIC_EXCEPTION_RESIDUES As String = "P"

    Private Const SHOULDER_ION_PREFIX As String = "Shoulder-"

    Private ReadOnly ElementAndMassRoutines As MWElementAndMassRoutines

    Public Enum ctgCTerminusGroupConstants
        ctgHydroxyl = 0
        ctgAmide = 1
        ctgNone = 2
    End Enum

    Public Enum ntgNTerminusGroupConstants
        ntgHydrogen = 0
        ntgHydrogenPlusProton = 1
        ntgAcetyl = 2
        ntgPyroGlu = 3
        ntgCarbamyl = 4
        ntgPTC = 5
        ntgNone = 6
    End Enum

    Private Const ION_TYPE_MAX As itIonTypeConstants = itIonTypeConstants.itZIon
    Public Enum itIonTypeConstants
        itAIon = 0
        itBIon = 1
        itYIon = 2
        itCIon = 3
        itZIon = 4
    End Enum

    Private Structure udtModificationSymbolType
        Public Symbol As String ' Symbol used for modification in formula; may be 1 or more characters; for example: + ++ * ** etc.
        Public ModificationMass As Double ' Normally positive, but could be negative
        Public IndicatesPhosphorylation As Boolean ' When true, then this symbol means a residue is phosphorylated
        Public Comment As String
    End Structure

    Private Structure udtResidueType
        Public Symbol As String ' 3 letter symbol
        Public Mass As Double ' The mass of the residue alone (excluding any modification)
        Public MassWithMods As Double ' The mass of the residue, including phosphorylation or any modification
        Public IonMass() As Double ' 0-based array; the masses that the a, b, and y ions ending/starting with this residue will produce in the mass spectrum (includes H+)
        Public Phosphorylated As Boolean ' Technically, only Ser, Thr, or Tyr residues can be phosphorylated (H3PO4), but if the user phosphorylates other residues, we'll allow that
        Public ModificationIDCount As Short
        Public ModificationIDs() As Integer ' 1-based array

        ' Note: "Initialize" must be called to initialize instances of this structure
        Public Sub Initialize()
            Me.Initialize(False)
        End Sub
        Public Sub Initialize(ByVal blnForceInit As Boolean)
            If blnForceInit OrElse IonMass Is Nothing Then
                ReDim IonMass(ION_TYPE_MAX)
                ReDim ModificationIDs(MAX_MODIFICATIONS)
            End If
        End Sub
    End Structure

    Private Structure udtTerminusType
        Public Formula As String
        Public Mass As Double
        Public PrecedingResidue As udtResidueType ' If the peptide sequence is part of a protein, the user can record the final residue of the previous peptide sequence here
        Public FollowingResidue As udtResidueType ' If the peptide sequence is part of a protein, the user can record the first residue of the next peptide sequence here

        ' Note: "Initialize" must be called to initialize instances of this structure
        Public Sub Initialize()
            PrecedingResidue.Initialize()
            FollowingResidue.Initialize()
        End Sub
    End Structure

    Public Structure udtFragmentionSpectrumIntensitiesType
        Public IonType() As Double ' 0-based array
        Public BYIonShoulder As Double ' If > 0 then shoulder ions will be created by B and Y ions
        Public NeutralLoss As Double

        ' Note: "Initialize" must be called to initialize instances of this structure
        Public Sub Initialize()
            ReDim IonType(ION_TYPE_MAX)
        End Sub
    End Structure

    ' Note: A ions can have ammonia and phosphate loss, but not water loss, so this is set to false by default
    '       The graphical version of MwtWin does not allow this to be overridden, but a programmer could do so via a call to this Dll
    Public Structure udtIonTypeOptionsType
        Public ShowIon As Boolean
        Public NeutralLossWater As Boolean
        Public NeutralLossAmmonia As Boolean
        Public NeutralLossPhosphate As Boolean
    End Structure

    Public Structure udtFragmentationSpectrumOptionsType
        Public IntensityOptions As udtFragmentionSpectrumIntensitiesType
        Public IonTypeOptions() As udtIonTypeOptionsType
        Public DoubleChargeIonsShow As Boolean
        Public DoubleChargeIonsThreshold As Single
        Public TripleChargeIonsShow As Boolean
        Public TripleChargeIonsThreshold As Single

        ' Note: "Initialize" must be called to initialize instances of this structure
        Public Sub Initialize()
            IntensityOptions.Initialize()
            ReDim IonTypeOptions(ION_TYPE_MAX)
        End Sub
    End Structure

    Public Structure udtFragmentationSpectrumDataType
        Public Mass As Double
        Public Intensity As Double
        Public Symbol As String ' The symbol, with the residue number (e.g. y1, y2, b3-H2O, Shoulder-y1, etc.)
        Public SymbolGeneric As String ' The symbol, without the residue number (e.g. a, b, y, b++, Shoulder-y, etc.)
        Public SourceResidueNumber As Integer ' The residue number that resulted in this mass
        Public SourceResidueSymbol3Letter As String ' The residue symbol that resulted in this mass
        Public Charge As Short
        Public IonType As itIonTypeConstants
        Public IsShoulderIon As Boolean ' B and Y ions can have Shoulder ions at +-1

        Public Overloads Function ToString() As String
            Return Symbol & ", " & Mass.ToString("0.00")
        End Function
    End Structure

    ' Note: A peptide goes from N to C, eg. HGlyLeuTyrOH has N-Terminus = H and C-Terminus = OH
    ' Residue 1 would be Gly, Residue 2 would be Leu, Residue 3 would be Tyr
    Private Residues() As udtResidueType ' 1-based array
    Private ResidueCount As Integer
    Private ResidueCountDimmed As Integer

    ' ModificationSymbols() holds a list of the potential modification symbols and the mass of each modification
    ' Modification symbols can be 1 or more letters long
    Private ModificationSymbols() As udtModificationSymbolType ' 1-based array
    Private ModificationSymbolCount As Integer
    Private ModificationSymbolCountDimmed As Integer

    ' ReSharper disable once UnassignedField.Local - initialized in InitializeClass() when it calls InitializeArrays()
    Private mNTerminus As udtTerminusType ' Formula on the N-Terminus

    ' ReSharper disable once UnassignedField.Local - initialized in InitializeClass() when it calls InitializeArrays()
    Private mCTerminus As udtTerminusType ' Formula on the C-Terminus
    Private mTotalMass As Double

    Private mWaterLossSymbol As String ' -H2O
    Private mAmmoniaLossSymbol As String ' -NH3
    Private mPhosphoLossSymbol As String ' -H3PO4

    Private mFragSpectrumOptions As udtFragmentationSpectrumOptionsType

    Private dblHOHMass As Double
    Private dblNH3Mass As Double
    Private dblH3PO4Mass As Double
    Private dblPhosphorylationMass As Double ' H3PO4 minus HOH = 79.9663326
    Private dblHydrogenMass As Double ' Mass of hydrogen
    Private dblChargeCarrierMass As Double ' H minus one electron

    Private dblImmoniumMassDifference As Double ' CO minus H = 26.9871

    Private dblHistidineFW As Double ' 110
    Private dblPhenylalanineFW As Double ' 120
    Private dblTyrosineFW As Double ' 136

    Private mDelayUpdateResidueMass As Boolean
    '

    Private Sub AppendDataToFragSpectrum(ByRef lngIonCount As Integer, ByRef FragSpectrumWork() As udtFragmentationSpectrumDataType, ByVal sngMass As Single, ByVal sngIntensity As Single, ByVal strIonSymbol As String, ByVal strIonSymbolGeneric As String, ByVal lngSourceResidue As Integer, ByVal strSourceResidueSymbol3Letter As String, ByVal intCharge As Short, ByVal eIonType As itIonTypeConstants, ByVal blnIsShoulderIon As Boolean)

        Try
            If lngIonCount > UBound(FragSpectrumWork) Then
                ' This shouldn't happen
                Console.WriteLine("In AppendDataToFragSpectrum, lngIonCount is greater than UBound(FragSpectrumWork); this is unexpected")
                ReDim Preserve FragSpectrumWork(UBound(FragSpectrumWork) + 10)
            End If

            With FragSpectrumWork(lngIonCount)
                .Mass = sngMass
                .Intensity = sngIntensity
                .Symbol = strIonSymbol
                .SymbolGeneric = strIonSymbolGeneric
                .SourceResidueNumber = lngSourceResidue
                .SourceResidueSymbol3Letter = strSourceResidueSymbol3Letter
                .Charge = intCharge
                .IonType = eIonType
                .IsShoulderIon = blnIsShoulderIon
            End With
            lngIonCount += 1
        Catch ex As Exception
            Console.WriteLine(Err.Description)
        End Try

    End Sub

    Public Function AssureNonZero(ByVal lngNumber As Integer) As Integer
        ' Returns a non-zero number, either -1 if lngNumber = 0 or lngNumber if it's nonzero
        If lngNumber = 0 Then
            Return -1
        Else
            Return lngNumber
        End If
    End Function

    Private Function CheckForModifications(ByVal strPartialSequence As String, ByVal intResidueNumber As Integer, Optional ByVal blnAddMissingModificationSymbols As Boolean = False) As Integer
        ' Looks at strPartialSequence to see if it contains 1 or more modifications
        ' If any modification symbols are found, the modification is recorded in .ModificationIDs()
        ' If all or part of the modification symbol is not found in ModificationSymbols(), then a new entry
        '  is added to ModificationSymbols()
        ' Returns the total length of all modifications found

        Dim intCompareIndex, intSequenceStrLength As Integer
        Dim strModSymbolGroup As String
        Dim intModificationID, intModSymbolLengthTotal As Integer
        Dim strTestChar As String
        Dim intSubpartLength As Integer
        Dim blnMatchFound As Boolean

        intSequenceStrLength = strPartialSequence.Length

        ' Find the entire group of potential modification symbols
        strModSymbolGroup = String.Empty
        intCompareIndex = 0
        Do While intCompareIndex < intSequenceStrLength
            strTestChar = strPartialSequence.Substring(intCompareIndex, 1)
            If ElementAndMassRoutines.IsModSymbolInternal(strTestChar) Then
                strModSymbolGroup &= strTestChar
            Else
                Exit Do
            End If
            intCompareIndex += 1
        Loop

        intModSymbolLengthTotal = strModSymbolGroup.Length
        Do While strModSymbolGroup.Length > 0
            ' Step through strModSymbolGroup to see if all of it or parts of it match any of the defined
            '  modification symbols

            blnMatchFound = False
            For intSubpartLength = strModSymbolGroup.Length To 1 Step -1
                ' See if the modification is already defined
                intModificationID = GetModificationSymbolID(strModSymbolGroup.Substring(0, intSubpartLength))
                If intModificationID > 0 Then
                    blnMatchFound = True
                    Exit For
                End If
            Next

            If Not blnMatchFound Then
                If blnAddMissingModificationSymbols Then
                    ' Add strModSymbolGroup as a new modification, using a mass of 0 since we don't know the modification mass
                    SetModificationSymbol(strModSymbolGroup, 0)
                    blnMatchFound = True
                Else
                    ' Ignore the modification
                    strModSymbolGroup = "0"
                End If
                strModSymbolGroup = String.Empty
            End If

            If blnMatchFound Then
                ' Record the modification for this residue
                With Residues(intResidueNumber)
                    If .ModificationIDCount < MAX_MODIFICATIONS Then
                        .ModificationIDCount += 1S
                        .ModificationIDs(.ModificationIDCount) = intModificationID
                        If ModificationSymbols(intModificationID).IndicatesPhosphorylation Then
                            .Phosphorylated = True
                        End If
                    End If
                End With

                If intSubpartLength < strModSymbolGroup.Length Then
                    ' Remove the matched portion from strModSymbolGroup and test again
                    strModSymbolGroup = strModSymbolGroup.Substring(intSubpartLength)
                Else
                    strModSymbolGroup = String.Empty
                End If
            End If
        Loop

        Return intModSymbolLengthTotal

    End Function

    Private Function ComputeMaxIonsPerResidue() As Short
        ' Estimate the total ions per residue that will be created
        ' This number will nearly always be much higher than the number of ions that will actually
        '  be stored for a given sequence, since not all will be doubly charged, and not all will show
        '  all of the potential neutral losses

        Dim eIonIndex As itIonTypeConstants
        Dim intIonCount As Short

        intIonCount = 0
        With mFragSpectrumOptions
            For eIonIndex = 0 To ION_TYPE_MAX
                If .IonTypeOptions(eIonIndex).ShowIon Then
                    intIonCount += 1S

                    If Math.Abs(.IntensityOptions.BYIonShoulder) > 0 Then
                        If eIonIndex = itIonTypeConstants.itBIon OrElse eIonIndex = itIonTypeConstants.itYIon OrElse _
                           eIonIndex = itIonTypeConstants.itCIon Or eIonIndex = itIonTypeConstants.itZIon Then
                            intIonCount += 2S
                        End If
                    End If

                    If .IonTypeOptions(eIonIndex).NeutralLossAmmonia Then intIonCount += 1S
                    If .IonTypeOptions(eIonIndex).NeutralLossPhosphate Then intIonCount += 1S
                    If .IonTypeOptions(eIonIndex).NeutralLossWater Then intIonCount += 1S
                End If
            Next eIonIndex

            ' Double Charge ions could be created for all ions, so simply double intIonCount
            If .DoubleChargeIonsShow Then
                intIonCount *= 2S
            End If

            If .TripleChargeIonsShow Then
                intIonCount *= 2S
            End If
        End With

        ComputeMaxIonsPerResidue = intIonCount

    End Function

    Private Function FillResidueStructureUsingSymbol(ByVal strSymbol As String, Optional ByVal blnUse3LetterCode As Boolean = True) As udtResidueType
        ' Returns a variable of type udtResidueType containing strSymbol as the residue symbol
        ' If strSymbol is a valid amino acid type, then also updates udtResidue with the default information

        Dim strSymbol3Letter As String
        Dim lngAbbrevID As Integer
        Dim udtResidue As udtResidueType = New udtResidueType

        ' Initialize the UDTs
        udtResidue.Initialize()
        strSymbol3Letter = String.Empty

        If strSymbol.Length > 0 Then
            If blnUse3LetterCode Then
                strSymbol3Letter = strSymbol
            Else
                strSymbol3Letter = ElementAndMassRoutines.GetAminoAcidSymbolConversionInternal(strSymbol, True)
                If strSymbol3Letter.Length = 0 Then
                    strSymbol3Letter = strSymbol
                End If
            End If

            lngAbbrevID = ElementAndMassRoutines.GetAbbreviationIDInternal(strSymbol3Letter, True)
        Else
            lngAbbrevID = 0
        End If

        With udtResidue
            .Symbol = strSymbol3Letter
            .ModificationIDCount = 0
            .Phosphorylated = False
            If lngAbbrevID > 0 Then
                .Mass = ElementAndMassRoutines.GetAbbreviationMass(lngAbbrevID)
            Else
                .Mass = 0
            End If
            .MassWithMods = .Mass
        End With

        Return udtResidue

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="udtFragSpectrum"></param>
    ''' <returns>The number of ions in udtFragSpectrum()</returns>
    ''' <remarks></remarks>
    Public Function GetFragmentationMasses(ByRef udtFragSpectrum() As udtFragmentationSpectrumDataType) As Integer
        ' Old: Func GetFragmentationMasses(ByVal lngMaxIonCount As Long, ByRef sngIonMassesZeroBased() As Single, ByRef sngIonIntensitiesZeroBased() As Single, ByRef strIonSymbolsZeroBased() As String) As Long

        Dim lstFragSpectraData As List(Of udtFragmentationSpectrumDataType)

        lstFragSpectraData = GetFragmentationMasses()

        If lstFragSpectraData.Count = 0 Then
            ReDim udtFragSpectrum(0)
            Return 0
        End If

        ReDim udtFragSpectrum(lstFragSpectraData.Count)

        For intIndex As Integer = 0 To lstFragSpectraData.Count - 1
            udtFragSpectrum(intIndex) = lstFragSpectraData(intIndex)
        Next

        Return lstFragSpectraData.Count

    End Function

    Public Function GetFragmentationMasses() As List(Of udtFragmentationSpectrumDataType)

        Const MAX_CHARGE As Integer = 3

        Dim lngResidueIndex As Integer
        Dim intChargeIndex, intShoulderIndex As Short
        Dim eIonType As itIonTypeConstants
        Dim lngIndex As Integer
        Dim lngPredictedIonCount, lngIonCount As Integer
        Dim sngIonIntensities() As Single
        ReDim sngIonIntensities(ION_TYPE_MAX)
        Dim sngIonShoulderIntensity, sngNeutralLossIntensity As Single

        Dim blnShowCharge() As Boolean
        Dim sngChargeThreshold() As Single

        Dim sngConvolutedMass, sngBaseMass, sngObservedMass As Single
        Dim strResidues As String
        Dim blnPhosphorylated As Boolean
        Dim sngIntensity As Single
        Dim strIonSymbol, strIonSymbolGeneric As String
        Dim FragSpectrumWork() As udtFragmentationSpectrumDataType

        Dim PointerArray() As Integer

        If ResidueCount = 0 Then
            ' No residues
            Return New List(Of udtFragmentationSpectrumDataType)
        End If

        ReDim blnShowCharge(MAX_CHARGE)
        ReDim sngChargeThreshold(MAX_CHARGE)

        ' Copy some of the values from mFragSpectrumOptions to local variables to make things easier to read
        With mFragSpectrumOptions
            For eIonType = 0 To ION_TYPE_MAX
                sngIonIntensities(eIonType) = CSng(.IntensityOptions.IonType(eIonType))
            Next eIonType
            sngIonShoulderIntensity = CSng(.IntensityOptions.BYIonShoulder)
            sngNeutralLossIntensity = CSng(.IntensityOptions.NeutralLoss)

            If MAX_CHARGE >= 2 Then
                blnShowCharge(2) = .DoubleChargeIonsShow
                sngChargeThreshold(2) = .DoubleChargeIonsThreshold
            End If

            If MAX_CHARGE >= 3 Then
                blnShowCharge(3) = .TripleChargeIonsShow
                sngChargeThreshold(3) = .TripleChargeIonsThreshold
            End If
        End With

        ' Populate sngIonMassesZeroBased() and sngIonIntensitiesZeroBased()
        ' Put ion descriptions in strIonSymbolsZeroBased
        lngPredictedIonCount = GetFragmentationSpectrumRequiredDataPoints()

        If lngPredictedIonCount = 0 Then lngPredictedIonCount = ResidueCount
        ReDim FragSpectrumWork(lngPredictedIonCount)

        ' Need to update the residue masses in case the modifications have changed
        UpdateResidueMasses()

        lngIonCount = 0
        For lngResidueIndex = 1 To ResidueCount
            With Residues(lngResidueIndex)

                For eIonType = 0 To ION_TYPE_MAX
                    If mFragSpectrumOptions.IonTypeOptions(eIonType).ShowIon Then
                        If (lngResidueIndex = 1 OrElse lngResidueIndex = ResidueCount) AndAlso (eIonType = itIonTypeConstants.itAIon OrElse eIonType = itIonTypeConstants.itBIon OrElse eIonType = itIonTypeConstants.itCIon) Then
                            ' Don't include a, b, or c ions in the output masses for this residue
                        Else

                            ' Ion is used
                            sngBaseMass = CSng(.IonMass(eIonType)) ' Already in the H+ state
                            sngIntensity = sngIonIntensities(eIonType)

                            ' Get the list of residues preceding or following this residue
                            ' Note that the residue symbols are separated by a space to avoid accidental matching by the InStr() functions below
                            strResidues = GetInternalResidues(lngResidueIndex, eIonType, blnPhosphorylated)

                            For intChargeIndex = 1 To MAX_CHARGE
                                If intChargeIndex = 1 Or (intChargeIndex > 1 And blnShowCharge(intChargeIndex)) Then
                                    If intChargeIndex = 1 Then
                                        sngConvolutedMass = sngBaseMass
                                    Else
                                        ' Compute mass at higher charge
                                        sngConvolutedMass = CSng(ElementAndMassRoutines.ConvoluteMassInternal(sngBaseMass, 1, intChargeIndex, dblChargeCarrierMass))
                                    End If

                                    If intChargeIndex > 1 And sngBaseMass < sngChargeThreshold(intChargeIndex) Then
                                        ' BaseMass is below threshold, do not add to Predicted Spectrum
                                    Else
                                        ' Add ion to Predicted Spectrum

                                        ' Y and Z Ions are numbered in decreasing order: y5, y4, y3, y2, y1
                                        ' A, B, and C ions are numbered in increasing order: a1, a2, etc.  or b1, b2, etc.
                                        strIonSymbolGeneric = LookupIonTypeString(eIonType)
                                        If eIonType = itIonTypeConstants.itYIon OrElse eIonType = itIonTypeConstants.itZIon Then
                                            strIonSymbol = strIonSymbolGeneric & Trim(Str(ResidueCount - lngResidueIndex + 1))
                                        Else
                                            strIonSymbol = strIonSymbolGeneric & Trim(Str(lngResidueIndex))
                                        End If

                                        If intChargeIndex > 1 Then
                                            strIonSymbol = strIonSymbol & New String("+"c, intChargeIndex)
                                            strIonSymbolGeneric = strIonSymbolGeneric & New String("+"c, intChargeIndex)
                                        End If


                                        AppendDataToFragSpectrum(lngIonCount, FragSpectrumWork, sngConvolutedMass, sngIntensity, strIonSymbol, strIonSymbolGeneric, lngResidueIndex, .Symbol, intChargeIndex, eIonType, False)

                                        ' Add shoulder ions to PredictedSpectrum() if a B, Y, C, or Z ion and the shoulder intensity is > 0
                                        ' Need to use Abs() here since user can define negative theoretical intensities (which allows for plotting a spectrum inverted)
                                        If Math.Abs(sngIonShoulderIntensity) > 0 AndAlso (eIonType = itIonTypeConstants.itBIon OrElse eIonType = itIonTypeConstants.itYIon OrElse eIonType = itIonTypeConstants.itCIon OrElse eIonType = itIonTypeConstants.itZIon) Then
                                            For intShoulderIndex = -1 To 1 Step 2
                                                sngObservedMass = CSng(sngConvolutedMass + intShoulderIndex * (1 / intChargeIndex))
                                                AppendDataToFragSpectrum(lngIonCount, FragSpectrumWork, sngObservedMass, sngIonShoulderIntensity, SHOULDER_ION_PREFIX & strIonSymbol, SHOULDER_ION_PREFIX & strIonSymbolGeneric, lngResidueIndex, .Symbol, intChargeIndex, eIonType, True)
                                            Next intShoulderIndex
                                        End If

                                        ' Apply neutral loss modifications
                                        If mFragSpectrumOptions.IonTypeOptions(eIonType).NeutralLossWater Then
                                            ' Loss of water only affects Ser, Thr, Asp, or Glu (S, T, E, or D)
                                            ' See if the residues up to this point contain any of these residues
                                            If strResidues.Contains("Ser") Or strResidues.Contains("Thr") Or strResidues.Contains("Glue") Or strResidues.Contains("Asp") Then
                                                sngObservedMass = CSng(sngConvolutedMass - (dblHOHMass / intChargeIndex))
                                                AppendDataToFragSpectrum(lngIonCount, FragSpectrumWork, sngObservedMass, sngNeutralLossIntensity, strIonSymbol & mWaterLossSymbol, strIonSymbolGeneric & mWaterLossSymbol, lngResidueIndex, .Symbol, intChargeIndex, eIonType, False)
                                            End If
                                        End If

                                        If mFragSpectrumOptions.IonTypeOptions(eIonType).NeutralLossAmmonia Then
                                            ' Loss of Ammonia only affects Arg, Lys, Gln, or Asn (R, K, Q, or N)
                                            ' See if the residues up to this point contain any of these residues
                                            If strResidues.Contains("Arg") Or strResidues.Contains("Lys") Or strResidues.Contains("Gln") Or strResidues.Contains("Asn") Then
                                                sngObservedMass = CSng(sngConvolutedMass - (dblNH3Mass / intChargeIndex))
                                                AppendDataToFragSpectrum(lngIonCount, FragSpectrumWork, sngObservedMass, sngNeutralLossIntensity, strIonSymbol & mAmmoniaLossSymbol, strIonSymbolGeneric & mAmmoniaLossSymbol, lngResidueIndex, .Symbol, intChargeIndex, eIonType, False)
                                            End If
                                        End If

                                        If mFragSpectrumOptions.IonTypeOptions(eIonType).NeutralLossPhosphate Then
                                            ' Loss of phosphate only affects phosphorylated residues
                                            ' Technically, only Ser, Thr, or Tyr (S, T, or Y) can be phosphorylated, but if the user marks other residues as phosphorylated, we'll allow that
                                            ' See if the residues up to this point contain phosphorylated residues
                                            If blnPhosphorylated Then
                                                sngObservedMass = CSng(sngConvolutedMass - (dblH3PO4Mass / intChargeIndex))
                                                AppendDataToFragSpectrum(lngIonCount, FragSpectrumWork, sngObservedMass, sngNeutralLossIntensity, strIonSymbol & mPhosphoLossSymbol, strIonSymbolGeneric & mPhosphoLossSymbol, lngResidueIndex, .Symbol, intChargeIndex, eIonType, False)
                                            End If
                                        End If

                                    End If
                                End If
                            Next intChargeIndex
                        End If
                    End If
                Next eIonType
            End With
        Next lngResidueIndex

        ' Sort arrays by mass (using a pointer array to synchronize the arrays)
        ReDim PointerArray(lngIonCount)

        For lngIndex = 0 To lngIonCount - 1
            PointerArray(lngIndex) = lngIndex
        Next lngIndex

        ShellSortFragSpectrum(FragSpectrumWork, PointerArray, 0, lngIonCount - 1)

        ' Copy the data from FragSpectrumWork() to lstFragSpectraData
        Dim lstFragSpectraData = New List(Of udtFragmentationSpectrumDataType)(lngIonCount)

        For lngIndex = 0 To lngIonCount - 1
            lstFragSpectraData.Add(FragSpectrumWork(PointerArray(lngIndex)))
        Next lngIndex

        Return lstFragSpectraData

    End Function

    Public Function GetFragmentationSpectrumRequiredDataPoints() As Integer
        ' Determines the total number of data points that will be required for a theoretical fragmentation spectrum

        Return ResidueCount * ComputeMaxIonsPerResidue()

    End Function

    Public Function GetFragmentationSpectrumOptions() As udtFragmentationSpectrumOptionsType

        Try
            Return mFragSpectrumOptions
        Catch ex As Exception
            ElementAndMassRoutines.GeneralErrorHandler("MWPeptideClass.GetFragmentationSpectrumOptions", Err.Number)
        End Try

        Dim udtDefaultOptions As udtFragmentationSpectrumOptionsType = New udtFragmentationSpectrumOptionsType
        udtDefaultOptions.Initialize()

        Return udtDefaultOptions

    End Function

    Public Function GetPeptideMass() As Double
        ' Returns the mass of the entire peptide

        ' Update the residue masses in order to update mTotalMass
        UpdateResidueMasses()

        Return mTotalMass
    End Function

    Private Function GetInternalResidues(ByVal lngCurrentResidueIndex As Integer, ByVal eIonType As itIonTypeConstants) As String
        Dim blnPhosphorylated As Boolean = False
        Return GetInternalResidues(lngCurrentResidueIndex, eIonType, blnPhosphorylated)
    End Function

    Private Function GetInternalResidues(ByVal lngCurrentResidueIndex As Integer, ByVal eIonType As itIonTypeConstants, ByRef blnPhosphorylated As Boolean) As String
        ' Determines the residues preceding or following the given residue (up to and including the current residue)
        ' If eIonType is a, b, or c ions, then returns residues from the N terminus
        ' If eIonType is y or ions, then returns residues from the C terminus
        ' Also, set blnPhosphorylated to true if any of the residues is Ser, Thr, or Tyr and is phosphorylated
        '
        ' Note that the residue symbols are separated by a space to avoid accidental matching by the InStr() function

        Dim strInternalResidues As String
        Dim lngResidueIndex As Integer

        strInternalResidues = String.Empty
        blnPhosphorylated = False
        If eIonType = itIonTypeConstants.itYIon OrElse eIonType = itIonTypeConstants.itZIon Then
            For lngResidueIndex = lngCurrentResidueIndex To ResidueCount
                With Residues(lngResidueIndex)
                    strInternalResidues = strInternalResidues & .Symbol & " "
                    If .Phosphorylated Then blnPhosphorylated = True
                End With
            Next lngResidueIndex
        Else
            For lngResidueIndex = 1 To lngCurrentResidueIndex
                With Residues(lngResidueIndex)
                    strInternalResidues = strInternalResidues & .Symbol & " "
                    If .Phosphorylated Then blnPhosphorylated = True
                End With
            Next lngResidueIndex
        End If

        Return strInternalResidues

    End Function

    Public Function GetModificationSymbol(ByVal lngModificationID As Integer, ByRef strModSymbol As String, ByRef dblModificationMass As Double, ByRef blnIndicatesPhosphorylation As Boolean, ByRef strComment As String) As Integer
        ' Returns information on the modification with lngModificationID
        ' Returns 0 if success, 1 if failure

        If lngModificationID >= 1 And lngModificationID <= ModificationSymbolCount Then
            With ModificationSymbols(lngModificationID)
                strModSymbol = .Symbol
                dblModificationMass = .ModificationMass
                blnIndicatesPhosphorylation = .IndicatesPhosphorylation
                strComment = .Comment
            End With
            Return 0
        Else
            strModSymbol = String.Empty
            dblModificationMass = 0
            blnIndicatesPhosphorylation = False
            strComment = String.Empty
            Return 1
        End If

    End Function

    Public Function GetModificationSymbolCount() As Integer
        ' Returns the number of modifications defined

        Return ModificationSymbolCount
    End Function

    Public Function GetModificationSymbolID(ByRef strModSymbol As String) As Integer
        ' Returns the ID for a given modification
        ' Returns 0 if not found, the ID if found

        Dim intIndex As Integer
        Dim lngModificationIDMatch As Integer

        For intIndex = 1 To ModificationSymbolCount
            If ModificationSymbols(intIndex).Symbol = strModSymbol Then
                lngModificationIDMatch = intIndex
                Exit For
            End If
        Next intIndex

        Return lngModificationIDMatch

    End Function

    Public Function GetResidue(ByVal lngResidueNumber As Integer, ByRef strSymbol As String, ByRef dblMass As Double, ByRef blnIsModified As Boolean, ByRef intModificationCount As Short) As Integer
        ' Returns 0 if success, 1 if failure
        If lngResidueNumber >= 1 And lngResidueNumber <= ResidueCount Then
            With Residues(lngResidueNumber)
                strSymbol = .Symbol
                dblMass = .Mass
                blnIsModified = (.ModificationIDCount > 0)
                intModificationCount = .ModificationIDCount
            End With
            Return 0
        Else
            Return 1
        End If
    End Function

    Public Function GetResidueCount() As Integer
        Return ResidueCount
    End Function

    Public Function GetResidueCountSpecificResidue(ByVal strResidueSymbol As String, ByVal blnUse3LetterCode As Boolean) As Integer
        ' Returns the number of occurrences of the given residue in the loaded sequence

        Dim strSearchResidue3Letter As String
        Dim lngResidueCount As Integer
        Dim lngResidueIndex As Integer

        If blnUse3LetterCode Then
            strSearchResidue3Letter = strResidueSymbol
        Else
            strSearchResidue3Letter = ElementAndMassRoutines.GetAminoAcidSymbolConversionInternal(strResidueSymbol, True)
        End If

        lngResidueCount = 0
        For lngResidueIndex = 0 To ResidueCount - 1
            If Residues(lngResidueIndex).Symbol = strSearchResidue3Letter Then
                lngResidueCount = lngResidueCount + 1
            End If

        Next lngResidueIndex

        Return lngResidueCount
    End Function

    Public Function GetResidueModificationIDs(ByVal lngResidueNumber As Integer, ByRef lngModificationIDsOneBased() As Integer) As Integer
        ' Returns the number of Modifications
        ' ReDims lngModificationIDsOneBased() to hold the values

        Dim intIndex As Short

        If lngResidueNumber >= 1 And lngResidueNumber <= ResidueCount Then

            With Residues(lngResidueNumber)

                ' Need to use this in case the calling program is sending an array with fixed dimensions
                Try
                    ReDim lngModificationIDsOneBased(.ModificationIDCount)
                Catch ex As Exception
                    ' Ignore errors
                End Try

                For intIndex = 1 To .ModificationIDCount
                    lngModificationIDsOneBased(intIndex) = .ModificationIDs(intIndex)
                Next intIndex

                Return .ModificationIDCount
            End With
        Else
            Return 0
        End If

    End Function

    Public Function GetResidueSymbolOnly(ByVal lngResidueNumber As Integer, ByVal blnUse3LetterCode As Boolean) As String
        ' Returns the symbol at the given residue number, or string.empty if an invalid residue number

        Dim strSymbol As String

        If lngResidueNumber >= 1 And lngResidueNumber <= ResidueCount Then
            With Residues(lngResidueNumber)
                strSymbol = .Symbol
            End With
            If Not blnUse3LetterCode Then strSymbol = ElementAndMassRoutines.GetAminoAcidSymbolConversionInternal(strSymbol, False)
        Else
            strSymbol = String.Empty
        End If

        Return strSymbol

    End Function

    Public Function GetSequence() As String
        Return GetSequence(blnUse3LetterCode:=True, blnAddSpaceEvery10Residues:=False, blnSeparateResiduesWithDash:=False, blnIncludeNandCTerminii:=False, blnIncludeModificationSymbols:=True)
    End Function

    Public Function GetSequence1LetterCode() As String
        Return GetSequence(blnUse3LetterCode:=False, blnAddSpaceEvery10Residues:=False, blnSeparateResiduesWithDash:=False, blnIncludeNandCTerminii:=False, blnIncludeModificationSymbols:=True)
    End Function

    Public Function GetSequence(ByVal blnUse3LetterCode As Boolean) As String
        Return GetSequence(blnUse3LetterCode, blnAddSpaceEvery10Residues:=False, blnSeparateResiduesWithDash:=False, blnIncludeNandCTerminii:=False, blnIncludeModificationSymbols:=True)
    End Function

    Public Function GetSequence(ByVal blnUse3LetterCode As Boolean, _
                                ByVal blnAddSpaceEvery10Residues As Boolean) As String
        Return GetSequence(blnUse3LetterCode, blnAddSpaceEvery10Residues, blnSeparateResiduesWithDash:=False, blnIncludeNandCTerminii:=False, blnIncludeModificationSymbols:=True)
    End Function

    Public Function GetSequence(ByVal blnUse3LetterCode As Boolean, _
                                ByVal blnAddSpaceEvery10Residues As Boolean, _
                                ByVal blnSeparateResiduesWithDash As Boolean) As String
        Return GetSequence(blnUse3LetterCode, blnAddSpaceEvery10Residues, blnSeparateResiduesWithDash, blnIncludeNandCTerminii:=False, blnIncludeModificationSymbols:=True)
    End Function

    Public Function GetSequence(ByVal blnUse3LetterCode As Boolean, _
                                ByVal blnAddSpaceEvery10Residues As Boolean, _
                                ByVal blnSeparateResiduesWithDash As Boolean, _
                                ByVal blnIncludeNandCTerminii As Boolean) As String
        Return GetSequence(blnUse3LetterCode, blnAddSpaceEvery10Residues, blnSeparateResiduesWithDash, blnIncludeNandCTerminii, blnIncludeModificationSymbols:=True)
    End Function

    Public Function GetSequence(ByVal blnUse3LetterCode As Boolean, _
                                ByVal blnAddSpaceEvery10Residues As Boolean, _
                                ByVal blnSeparateResiduesWithDash As Boolean, _
                                ByVal blnIncludeNandCTerminii As Boolean, _
                                ByVal blnIncludeModificationSymbols As Boolean) As String

        ' Construct a text sequence using Residues() and the N and C Terminus info

        Dim strSymbol3Letter, strSequence, strSymbol1Letter As String
        Dim strDashAdd As String
        Dim strModSymbol As String = String.Empty
        Dim strModSymbolComment As String = String.Empty
        Dim blnIndicatesPhosphorylation As Boolean
        Dim dblModMass As Double
        Dim lngIndex As Integer
        Dim intModIndex As Short
        Dim lngError As Integer

        If blnSeparateResiduesWithDash Then strDashAdd = "-" Else strDashAdd = String.Empty

        strSequence = String.Empty
        For lngIndex = 1 To ResidueCount
            With Residues(lngIndex)
                strSymbol3Letter = .Symbol
                If blnUse3LetterCode Then
                    strSequence = strSequence & strSymbol3Letter
                Else
                    strSymbol1Letter = ElementAndMassRoutines.GetAminoAcidSymbolConversionInternal(strSymbol3Letter, False)
                    If strSymbol1Letter = String.Empty Then strSymbol1Letter = UNKNOWN_SYMBOL_ONE_LETTER
                    strSequence = strSequence & strSymbol1Letter
                End If

                If blnIncludeModificationSymbols Then
                    For intModIndex = 1 To .ModificationIDCount
                        lngError = GetModificationSymbol(.ModificationIDs(intModIndex), strModSymbol, dblModMass, blnIndicatesPhosphorylation, strModSymbolComment)
                        If lngError = 0 Then
                            strSequence = strSequence & strModSymbol
                        Else
                            Console.WriteLine("GetModificationSymbol returned error code " & lngError & " in GetSequence")
                        End If
                    Next intModIndex
                End If

            End With

            If lngIndex <> ResidueCount Then
                If blnAddSpaceEvery10Residues Then
                    If lngIndex Mod 10 = 0 Then
                        strSequence = strSequence & " "
                    Else
                        strSequence = strSequence & strDashAdd
                    End If
                Else
                    strSequence = strSequence & strDashAdd
                End If
            End If

        Next lngIndex

        If blnIncludeNandCTerminii Then
            strSequence = mNTerminus.Formula & strDashAdd & strSequence & strDashAdd & mCTerminus.Formula
        End If

        Return strSequence
    End Function

    Public Function GetSymbolWaterLoss() As String
        Return mWaterLossSymbol
    End Function

    Public Function GetSymbolPhosphoLoss() As String
        Return mPhosphoLossSymbol
    End Function

    Public Function GetSymbolAmmoniaLoss() As String
        Return mAmmoniaLossSymbol
    End Function

    Public Function GetTrypticName(ByVal strProteinResidues As String, ByVal strPeptideResidues As String) As String

        Return GetTrypticName(strProteinResidues, strPeptideResidues, 0, 0, False, _
                              TRYPTIC_RULE_RESIDUES, TRYPTIC_EXCEPTION_RESIDUES, TERMINII_SYMBOL, True, 1)

    End Function

    Public Function GetTrypticName(ByVal strProteinResidues As String, ByVal strPeptideResidues As String, _
                                   ByVal lngProteinSearchStartLoc As Integer) As String

        Return GetTrypticName(strProteinResidues, strPeptideResidues, 0, 0, False, _
                              TRYPTIC_RULE_RESIDUES, TRYPTIC_EXCEPTION_RESIDUES, TERMINII_SYMBOL, True, lngProteinSearchStartLoc)

    End Function

    Public Function GetTrypticName(ByVal strProteinResidues As String, ByVal strPeptideResidues As String, _
                               ByRef lngReturnResidueStart As Integer, _
                               ByRef lngReturnResidueEnd As Integer) As String

        Return GetTrypticName(strProteinResidues, strPeptideResidues, lngReturnResidueStart, lngReturnResidueEnd, False, _
                              TRYPTIC_RULE_RESIDUES, TRYPTIC_EXCEPTION_RESIDUES, TERMINII_SYMBOL, True, 1)

    End Function

    Public Function GetTrypticName(ByVal strProteinResidues As String, ByVal strPeptideResidues As String, _
                                  ByRef lngReturnResidueStart As Integer, _
                                  ByRef lngReturnResidueEnd As Integer, _
                                  ByVal blnICR2LSCompatible As Boolean) As String

        Return GetTrypticName(strProteinResidues, strPeptideResidues, lngReturnResidueStart, lngReturnResidueEnd, blnICR2LSCompatible, _
                              TRYPTIC_RULE_RESIDUES, TRYPTIC_EXCEPTION_RESIDUES, TERMINII_SYMBOL, True, 1)

    End Function


    Public Function GetTrypticName(ByVal strProteinResidues As String, ByVal strPeptideResidues As String, _
                                  ByRef lngReturnResidueStart As Integer, _
                                  ByRef lngReturnResidueEnd As Integer, _
                                  ByVal blnICR2LSCompatible As Boolean, _
                                  ByVal strRuleResidues As String, _
                                  ByVal strExceptionResidues As String, _
                                  ByVal strTerminiiSymbol As String) As String

        Return GetTrypticName(strProteinResidues, strPeptideResidues, lngReturnResidueStart, lngReturnResidueEnd, blnICR2LSCompatible, _
                              strRuleResidues, strExceptionResidues, strTerminiiSymbol, True, 1)

    End Function


    ''' <summary>
    ''' Examines strPeptideResidues to see where they exist in strProteinResidues
    ''' Constructs a name string based on their position and based on whether the fragment is truly tryptic
    ''' In addition, returns the position of the first and last residue in lngReturnResidueStart and lngReturnResidueEnd
    ''' </summary>
    ''' <param name="strProteinResidues"></param>
    ''' <param name="strPeptideResidues"></param>
    ''' <param name="lngReturnResidueStart">Output: start peptides of the peptide residues in the protein</param>
    ''' <param name="lngReturnResidueEnd">Output: end peptides of the peptide residues in the protein</param>
    ''' <param name="blnICR2LSCompatible"></param>
    ''' <param name="strRuleResidues"></param>
    ''' <param name="strExceptionResidues"></param>
    ''' <param name="strTerminiiSymbol"></param>
    ''' <param name="blnIgnoreCase"></param>
    ''' <param name="lngProteinSearchStartLoc"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTrypticName(ByVal strProteinResidues As String, ByVal strPeptideResidues As String, _
                                   ByRef lngReturnResidueStart As Integer, _
                                   ByRef lngReturnResidueEnd As Integer, _
                                   ByVal blnICR2LSCompatible As Boolean, _
                                   ByVal strRuleResidues As String, _
                                   ByVal strExceptionResidues As String, _
                                   ByVal strTerminiiSymbol As String, _
                                   ByVal blnIgnoreCase As Boolean, _
                                   ByVal lngProteinSearchStartLoc As Integer) As String

        ' The tryptic name in the following format
        ' t1  indicates tryptic peptide 1
        ' t2 represents tryptic peptide 2, etc.
        ' t1.2  indicates tryptic peptide 1, plus one more tryptic peptide, i.e. t1 and t2
        ' t5.2  indicates tryptic peptide 5, plus one more tryptic peptide, i.e. t5 and t6
        ' t5.3  indicates tryptic peptide 5, plus two more tryptic peptides, i.e. t5, t6, and t7
        ' 40.52  means that the residues are not tryptic, and simply range from residue 40 to 52
        ' If the peptide residues are not present in strProteinResidues, then returns ""
        ' Since a peptide can occur multiple times in a protein, one can set lngProteinSearchStartLoc to a value larger than 1 to ignore previous hits

        ' If blnICR2LSCompatible is True, then the values returned when a peptide is not tryptic are modified to
        ' range from the starting residue, to the ending residue +1
        ' lngReturnResidueEnd is always equal to the position of the final residue, regardless of blnICR2LSCompatible

        ' For example, if strProteinResidues = "IGKANR"
        ' Then when strPeptideResidues = "IGK", the TrypticName is t1
        ' Then when strPeptideResidues = "ANR", the TrypticName is t2
        ' Then when strPeptideResidues = "IGKANR", the TrypticName is t1.2
        ' Then when strPeptideResidues = "IG", the TrypticName is 1.2
        ' Then when strPeptideResidues = "KANR", the TrypticName is 3.6
        ' Then when strPeptideResidues = "NR", the TrypticName is 5.6

        ' However, if blnICR2LSCompatible = True, then the last three are changed to:
        ' Then when strPeptideResidues = "IG", the TrypticName is 1.3
        ' Then when strPeptideResidues = "KANR", the TrypticName is 3.7
        ' Then when strPeptideResidues = "NR", the TrypticName is 5.7

        Dim intStartLoc, intEndLoc As Integer
        Dim strTrypticName As String
        Dim strPrefix, strSuffix As String
        Dim strResidueFollowingSearchResidues As String
        Dim blnMatchesCleavageRule As Boolean

        Dim intTrypticResidueNumber As Short
        Dim intRuleResidueMatchCount As Short
        Dim lngRuleResidueLoc As Integer
        Dim strProteinResiduesBeforeStartLoc As String
        Dim lngPeptideResiduesLength As Integer

        If blnIgnoreCase Then
            strProteinResidues = UCase(strProteinResidues)
            strPeptideResidues = UCase(strPeptideResidues)
        End If

        If lngProteinSearchStartLoc <= 1 Then
            intStartLoc = InStr(strProteinResidues, strPeptideResidues)
        Else
            intStartLoc = InStr(Mid(strProteinResidues, lngProteinSearchStartLoc), strPeptideResidues)
            If intStartLoc > 0 Then
                intStartLoc = intStartLoc + lngProteinSearchStartLoc - 1
            End If
        End If

        lngPeptideResiduesLength = Len(strPeptideResidues)

        If intStartLoc > 0 And Len(strProteinResidues) > 0 And lngPeptideResiduesLength > 0 Then
            intEndLoc = intStartLoc + lngPeptideResiduesLength - 1

            ' Determine if the residue is tryptic
            ' Use CheckSequenceAgainstCleavageRule() for this
            If intStartLoc > 1 Then
                strPrefix = Mid(strProteinResidues, intStartLoc - 1, 1)
            Else
                strPrefix = strTerminiiSymbol
            End If

            If intEndLoc = Len(strProteinResidues) Then
                strSuffix = strTerminiiSymbol
            Else
                strSuffix = Mid(strProteinResidues, intEndLoc + 1, 1)
            End If

            blnMatchesCleavageRule = CheckSequenceAgainstCleavageRule(strPrefix & "." & strPeptideResidues & "." & strSuffix, _
                                                                      strRuleResidues, _
                                                                      strExceptionResidues, _
                                                                      False, _
                                                                      ".", _
                                                                      strTerminiiSymbol, _
                                                                      blnIgnoreCase)

            If blnMatchesCleavageRule Then
                ' Construct strTrypticName

                ' Determine which tryptic residue strPeptideResidues is
                If intStartLoc = 1 Then
                    intTrypticResidueNumber = 1
                Else
                    strProteinResiduesBeforeStartLoc = Left(strProteinResidues, intStartLoc - 1)
                    strResidueFollowingSearchResidues = Left(strPeptideResidues, 1)
                    intTrypticResidueNumber = 0
                    lngRuleResidueLoc = 0
                    Do
                        lngRuleResidueLoc = GetTrypticNameFindNextCleavageLoc(strProteinResiduesBeforeStartLoc, strResidueFollowingSearchResidues, lngRuleResidueLoc + 1, strRuleResidues, strExceptionResidues, strTerminiiSymbol)
                        If lngRuleResidueLoc > 0 Then
                            intTrypticResidueNumber = intTrypticResidueNumber + 1S
                        End If
                    Loop While lngRuleResidueLoc > 0 And lngRuleResidueLoc + 1 < intStartLoc
                    intTrypticResidueNumber = intTrypticResidueNumber + 1S
                End If

                ' Determine number of K or R residues in strPeptideResidues
                ' Ignore K or R residues followed by Proline
                intRuleResidueMatchCount = 0
                lngRuleResidueLoc = 0
                Do
                    lngRuleResidueLoc = GetTrypticNameFindNextCleavageLoc(strPeptideResidues, strSuffix, lngRuleResidueLoc + 1, strRuleResidues, strExceptionResidues, strTerminiiSymbol)
                    If lngRuleResidueLoc > 0 Then
                        intRuleResidueMatchCount = intRuleResidueMatchCount + 1S
                    End If
                Loop While lngRuleResidueLoc > 0 And lngRuleResidueLoc < lngPeptideResiduesLength

                strTrypticName = "t" & Trim(Str(intTrypticResidueNumber))
                If intRuleResidueMatchCount > 1 Then
                    strTrypticName = strTrypticName & "." & Trim(Str(intRuleResidueMatchCount))
                End If
            Else
                If blnICR2LSCompatible Then
                    strTrypticName = Trim(Str(intStartLoc)) & "." & Trim(Str(intEndLoc + 1))
                Else
                    strTrypticName = Trim(Str(intStartLoc)) & "." & Trim(Str(intEndLoc))
                End If
            End If

            lngReturnResidueStart = intStartLoc
            lngReturnResidueEnd = intEndLoc
            Return strTrypticName
        Else
            ' Residues not found
            lngReturnResidueStart = 0
            lngReturnResidueEnd = 0
            Return String.Empty
        End If

    End Function

    Public Function GetTrypticNameMultipleMatches(ByVal strProteinResidues As String, _
                                                 ByVal strPeptideResidues As String) As String

        Return GetTrypticNameMultipleMatches(strProteinResidues, strPeptideResidues, _
                                             0, 0, 0, False, _
                                             TRYPTIC_RULE_RESIDUES, TRYPTIC_EXCEPTION_RESIDUES, TERMINII_SYMBOL, True, 1, ", ")
    End Function

    Public Function GetTrypticNameMultipleMatches(ByVal strProteinResidues As String, _
                                                  ByVal strPeptideResidues As String, _
                                                  ByVal lngProteinSearchStartLoc As Integer) As String

        Return GetTrypticNameMultipleMatches(strProteinResidues, strPeptideResidues, _
                                             0, 0, 0, False, _
                                             TRYPTIC_RULE_RESIDUES, TRYPTIC_EXCEPTION_RESIDUES, TERMINII_SYMBOL, True, _
                                             lngProteinSearchStartLoc, ", ")
    End Function

    Public Function GetTrypticNameMultipleMatches(ByVal strProteinResidues As String, _
                                              ByVal strPeptideResidues As String, _
                                              ByVal lngProteinSearchStartLoc As Integer, _
                                              ByVal strListDelimeter As String) As String

        Return GetTrypticNameMultipleMatches(strProteinResidues, strPeptideResidues, _
                                             0, 0, 0, False, _
                                             TRYPTIC_RULE_RESIDUES, TRYPTIC_EXCEPTION_RESIDUES, TERMINII_SYMBOL, True, _
                                             lngProteinSearchStartLoc, strListDelimeter)
    End Function


    Public Function GetTrypticNameMultipleMatches(ByVal strProteinResidues As String, _
                                                  ByVal strPeptideResidues As String, _
                                                  ByRef lngReturnMatchCount As Integer, _
                                                  ByRef lngReturnResidueStart As Integer, _
                                                  ByRef lngReturnResidueEnd As Integer) As String

        Return GetTrypticNameMultipleMatches(strProteinResidues, strPeptideResidues, _
                                            lngReturnMatchCount, lngReturnResidueStart, lngReturnResidueEnd, False, _
                                            TRYPTIC_RULE_RESIDUES, TRYPTIC_EXCEPTION_RESIDUES, TERMINII_SYMBOL, True, 1, ", ")

    End Function

    Public Function GetTrypticNameMultipleMatches(ByVal strProteinResidues As String, _
                                                ByVal strPeptideResidues As String, _
                                                ByRef lngReturnMatchCount As Integer, _
                                                ByRef lngReturnResidueStart As Integer, _
                                                ByRef lngReturnResidueEnd As Integer, _
                                                ByVal blnICR2LSCompatible As Boolean) As String

        Return GetTrypticNameMultipleMatches(strProteinResidues, strPeptideResidues, _
                                            lngReturnMatchCount, lngReturnResidueStart, lngReturnResidueEnd, blnICR2LSCompatible, _
                                            TRYPTIC_RULE_RESIDUES, TRYPTIC_EXCEPTION_RESIDUES, TERMINII_SYMBOL, True, 1, ", ")

    End Function

    Public Function GetTrypticNameMultipleMatches(ByVal strProteinResidues As String, _
                                              ByVal strPeptideResidues As String, _
                                              ByRef lngReturnMatchCount As Integer, _
                                              ByRef lngReturnResidueStart As Integer, _
                                              ByRef lngReturnResidueEnd As Integer, _
                                              ByVal blnICR2LSCompatible As Boolean, _
                                              ByVal strRuleResidues As String, _
                                              ByVal strExceptionResidues As String, _
                                              ByVal strTerminiiSymbol As String) As String


        Return GetTrypticNameMultipleMatches(strProteinResidues, strPeptideResidues, _
                                            lngReturnMatchCount, lngReturnResidueStart, lngReturnResidueEnd, blnICR2LSCompatible, _
                                            strRuleResidues, strExceptionResidues, strTerminiiSymbol, True, 1, ", ")

    End Function

    ''' <summary>
    ''' Examines strPeptideResidues to see where they exist in strProteinResidues
    ''' Looks for all possible matches, returning them as a comma separated list
    ''' </summary>
    ''' <param name="strProteinResidues"></param>
    ''' <param name="strPeptideResidues"></param>
    ''' <param name="lngReturnMatchCount"></param>
    ''' <param name="lngReturnResidueStart"></param>
    ''' <param name="lngReturnResidueEnd"></param>
    ''' <param name="blnICR2LSCompatible"></param>
    ''' <param name="strRuleResidues"></param>
    ''' <param name="strExceptionResidues"></param>
    ''' <param name="strTerminiiSymbol"></param>
    ''' <param name="blnIgnoreCase"></param>
    ''' <param name="lngProteinSearchStartLoc"></param>
    ''' <param name="strListDelimeter"></param>
    ''' <returns>The number of matches</returns>
    ''' <remarks></remarks>
    Public Function GetTrypticNameMultipleMatches(ByVal strProteinResidues As String, _
                                                  ByVal strPeptideResidues As String, _
                                                  ByRef lngReturnMatchCount As Integer, _
                                                  ByRef lngReturnResidueStart As Integer, _
                                                  ByRef lngReturnResidueEnd As Integer, _
                                                  ByVal blnICR2LSCompatible As Boolean, _
                                                  ByVal strRuleResidues As String, _
                                                  ByVal strExceptionResidues As String, _
                                                  ByVal strTerminiiSymbol As String, _
                                                  ByVal blnIgnoreCase As Boolean, _
                                                  ByVal lngProteinSearchStartLoc As Integer, _
                                                  ByVal strListDelimeter As String) As String

        ' Returns the number of matches in lngReturnMatchCount
        ' lngReturnResidueStart contains the residue number of the start of the first match
        ' lngReturnResidueEnd contains the residue number of the end of the last match

        ' See GetTrypticName for additional information

        Dim strNameList, strCurrentName As String
        Dim lngCurrentSearchLoc As Integer
        Dim lngCurrentResidueStart, lngCurrentResidueEnd As Integer

        lngCurrentSearchLoc = lngProteinSearchStartLoc
        lngReturnMatchCount = 0
        strNameList = String.Empty

        Do
            strCurrentName = GetTrypticName(strProteinResidues, strPeptideResidues, lngCurrentResidueStart, lngCurrentResidueEnd, blnICR2LSCompatible, strRuleResidues, strExceptionResidues, strTerminiiSymbol, blnIgnoreCase, lngCurrentSearchLoc)

            If Len(strCurrentName) > 0 Then
                If strNameList.Length > 0 Then
                    strNameList &= strListDelimeter
                End If
                strNameList &= strCurrentName
                lngCurrentSearchLoc = lngCurrentResidueEnd + 1
                lngReturnMatchCount = lngReturnMatchCount + 1

                If lngReturnMatchCount = 1 Then
                    lngReturnResidueStart = lngCurrentResidueStart
                End If
                lngReturnResidueEnd = lngCurrentResidueEnd

                If lngCurrentSearchLoc > Len(strProteinResidues) Then Exit Do
            Else
                Exit Do
            End If
        Loop

        Return strNameList

    End Function

    Private Function GetTrypticNameFindNextCleavageLoc(ByVal strSearchResidues As String, ByVal strResidueFollowingSearchResidues As String, _
                ByVal lngStartChar As Integer, _
                Optional ByVal strSearchChars As String = TRYPTIC_RULE_RESIDUES, _
                Optional ByVal strExceptionSuffixResidues As String = TRYPTIC_EXCEPTION_RESIDUES, _
                Optional ByVal strTerminiiSymbol As String = TERMINII_SYMBOL) As Integer
        ' Finds the location of the next strSearchChar in strSearchResidues (K or R by default)
        ' Assumes strSearchResidues are already upper case
        ' Examines the residue following the matched residue
        '   If it matches one of the characters in strExceptionSuffixResidues, then the match is not counted
        ' Note that strResidueFollowingSearchResidues is necessary in case the potential cleavage residue is the final residue in strSearchResidues
        ' We need to know the next residue to determine if it matches an exception residue
        ' For example, if strSearchResidues =      "IGASGEHIFIIGVDKPNR"
        '  and the protein it is part of is: TNSANFRIGASGEHIFIIGVDKPNRQPDS
        '  and strSearchChars = "KR while strExceptionSuffixResidues  = "P"
        ' Then the K in IGASGEHIFIIGVDKPNR is ignored because the following residue is P,
        '  while the R in IGASGEHIFIIGVDKPNR is OK because strResidueFollowingSearchResidues is Q
        ' It is the calling function's responsibility to assign the correct residue to strResidueFollowingSearchResidues
        ' If no match is found, but strResidueFollowingSearchResidues is "-", then the cleavage location returned is Len(strSearchResidues) + 1

        Dim intCharLocInSearchChars As Short
        Dim lngCharLoc, lngMinCharLoc As Integer
        Dim intExceptionSuffixResidueCount As Short
        Dim intCharLocInExceptionChars As Short
        Dim strResidueFollowingCleavageResidue As String
        Dim lngExceptionCharLocInSearchResidues, lngCharLocViaRecursiveSearch As Integer

        intExceptionSuffixResidueCount = CShort(Len(strExceptionSuffixResidues))

        lngMinCharLoc = -1
        For intCharLocInSearchChars = 1 To CShort(Len(strSearchChars))
            lngCharLoc = InStr(Mid(strSearchResidues, lngStartChar), Mid(strSearchChars, intCharLocInSearchChars, 1))

            If lngCharLoc > 0 Then
                lngCharLoc = lngCharLoc + lngStartChar - 1

                If intExceptionSuffixResidueCount > 0 Then
                    ' Make sure strSuffixResidue does not match strExceptionSuffixResidues
                    If lngCharLoc < Len(strSearchResidues) Then
                        lngExceptionCharLocInSearchResidues = lngCharLoc + 1
                        strResidueFollowingCleavageResidue = Mid(strSearchResidues, lngExceptionCharLocInSearchResidues, 1)
                    Else
                        ' Matched the last residue in strSearchResidues
                        lngExceptionCharLocInSearchResidues = Len(strSearchResidues) + 1
                        strResidueFollowingCleavageResidue = strResidueFollowingSearchResidues
                    End If

                    For intCharLocInExceptionChars = 1 To intExceptionSuffixResidueCount
                        If strResidueFollowingCleavageResidue = Mid(strExceptionSuffixResidues, intCharLocInExceptionChars, 1) Then
                            ' Exception char is the following character; can't count this as the cleavage point

                            If lngExceptionCharLocInSearchResidues < Len(strSearchResidues) Then
                                ' Recursively call this function to find the next cleavage position, using an updated lngStartChar position
                                lngCharLocViaRecursiveSearch = GetTrypticNameFindNextCleavageLoc(strSearchResidues, strResidueFollowingSearchResidues, lngExceptionCharLocInSearchResidues, strSearchChars, strExceptionSuffixResidues, strTerminiiSymbol)

                                If lngCharLocViaRecursiveSearch > 0 Then
                                    ' Found a residue further along that is a valid cleavage point
                                    lngCharLoc = lngCharLocViaRecursiveSearch
                                Else
                                    lngCharLoc = 0
                                End If
                            Else
                                lngCharLoc = 0
                            End If
                            Exit For
                        End If
                    Next intCharLocInExceptionChars
                End If
            End If

            If lngCharLoc > 0 Then
                If lngMinCharLoc < 0 Then
                    lngMinCharLoc = lngCharLoc
                Else
                    If lngCharLoc < lngMinCharLoc Then
                        lngMinCharLoc = lngCharLoc
                    End If
                End If
            End If
        Next intCharLocInSearchChars

        If lngMinCharLoc < 0 And strResidueFollowingSearchResidues = strTerminiiSymbol Then
            lngMinCharLoc = strSearchResidues.Length + 1
        End If

        If lngMinCharLoc < 0 Then
            Return 0
        Else
            Return (lngMinCharLoc)
        End If

    End Function

    Public Function GetTrypticPeptideNext(ByVal strProteinResidues As String, _
                                          ByVal lngSearchStartLoc As Integer) As String

        Return GetTrypticPeptideNext(strProteinResidues, lngSearchStartLoc, 0, 0, TRYPTIC_RULE_RESIDUES, TRYPTIC_EXCEPTION_RESIDUES, TERMINII_SYMBOL)
    End Function

    Public Function GetTrypticPeptideNext(ByVal strProteinResidues As String, _
                                          ByVal lngSearchStartLoc As Integer, _
                                          ByRef lngReturnResidueStart As Integer, _
                                          ByRef lngReturnResidueEnd As Integer, _
                                          ByVal strRuleResidues As String, _
                                          ByVal strExceptionResidues As String, _
                                          ByVal strTerminiiSymbol As String) As String

        ' Returns the next tryptic peptide in strProteinResidues, starting the search as lngSearchStartLoc
        ' Useful when obtaining all of the tryptic peptides for a protein, since this function will operate
        '  much faster than repeatedly calling GetTrypticPeptideByFragmentNumber()

        ' Returns the position of the start and end residues using lngReturnResidueStart and lngReturnResidueEnd

        Dim lngRuleResidueLoc As Integer
        Dim lngProteinResiduesLength As Integer

        If lngSearchStartLoc < 1 Then lngSearchStartLoc = 1

        lngProteinResiduesLength = Len(strProteinResidues)
        If lngSearchStartLoc > lngProteinResiduesLength Then
            Return String.Empty
        End If

        lngRuleResidueLoc = GetTrypticNameFindNextCleavageLoc(strProteinResidues, strTerminiiSymbol, lngSearchStartLoc, strRuleResidues, strExceptionResidues, strTerminiiSymbol)
        If lngRuleResidueLoc > 0 Then
            lngReturnResidueStart = lngSearchStartLoc
            If lngRuleResidueLoc > lngProteinResiduesLength Then
                lngReturnResidueEnd = lngProteinResiduesLength
            Else
                lngReturnResidueEnd = lngRuleResidueLoc
            End If
            Return Mid(strProteinResidues, lngReturnResidueStart, lngReturnResidueEnd - lngReturnResidueStart + 1)
        Else
            lngReturnResidueStart = 1
            lngReturnResidueEnd = lngProteinResiduesLength
            Return strProteinResidues
        End If

    End Function

    Public Function GetTrypticPeptideByFragmentNumber(ByVal strProteinResidues As String, _
                                                      ByVal intDesiredPeptideNumber As Short) As String

        Return GetTrypticPeptideByFragmentNumber(strProteinResidues, intDesiredPeptideNumber, 0, 0, _
                                                 TRYPTIC_RULE_RESIDUES, TRYPTIC_EXCEPTION_RESIDUES, TERMINII_SYMBOL, True)

    End Function

    Public Function GetTrypticPeptideByFragmentNumber(ByVal strProteinResidues As String, _
                                                      ByVal intDesiredPeptideNumber As Short, _
                                                      ByRef lngReturnResidueStart As Integer, _
                                                      ByRef lngReturnResidueEnd As Integer) As String

        Return GetTrypticPeptideByFragmentNumber(strProteinResidues, intDesiredPeptideNumber, _
                                                 lngReturnResidueStart, lngReturnResidueEnd, _
                                                 TRYPTIC_RULE_RESIDUES, TRYPTIC_EXCEPTION_RESIDUES, TERMINII_SYMBOL, True)

    End Function


    Public Function GetTrypticPeptideByFragmentNumber(ByVal strProteinResidues As String, _
                                                      ByVal intDesiredPeptideNumber As Short, _
                                                      ByRef lngReturnResidueStart As Integer, _
                                                      ByRef lngReturnResidueEnd As Integer, _
                                                      ByVal strRuleResidues As String, _
                                                      ByVal strExceptionResidues As String, _
                                                      ByVal strTerminiiSymbol As String, _
                                                      ByVal blnIgnoreCase As Boolean) As String

        ' Returns the desired tryptic peptide from strProteinResidues
        ' For example, if strProteinResidues = "IGKANRMTFGL" then
        '  when intDesiredPeptideNumber = 1, returns "IGK"
        '  when intDesiredPeptideNumber = 2, returns "ANR"
        '  when intDesiredPeptideNumber = 3, returns "MTFGL"

        ' Optionally, returns the position of the start and end residues
        '  using lngReturnResidueStart and lngReturnResidueEnd


        Dim lngStartLoc, lngRuleResidueLoc As Integer
        Dim lngPrevStartLoc As Integer
        Dim lngProteinResiduesLength As Integer
        Dim intCurrentTrypticPeptideNumber As Short

        Dim strMatchingFragment As String

        If intDesiredPeptideNumber < 1 Then
            Return String.Empty
        End If

        If blnIgnoreCase Then
            strProteinResidues = UCase(strProteinResidues)
        End If
        lngProteinResiduesLength = Len(strProteinResidues)

        lngStartLoc = 1
        intCurrentTrypticPeptideNumber = 0
        Do
            lngRuleResidueLoc = GetTrypticNameFindNextCleavageLoc(strProteinResidues, strTerminiiSymbol, lngStartLoc, strRuleResidues, strExceptionResidues, strTerminiiSymbol)
            If lngRuleResidueLoc > 0 Then
                intCurrentTrypticPeptideNumber = intCurrentTrypticPeptideNumber + 1S
                lngPrevStartLoc = lngStartLoc
                lngStartLoc = lngRuleResidueLoc + 1

                If lngPrevStartLoc > lngProteinResiduesLength Then
                    ' User requested a peptide number that doesn't exist
                    Return String.Empty
                End If
            Else
                ' I don't think I'll ever reach this code
                Exit Do
            End If
        Loop While intCurrentTrypticPeptideNumber < intDesiredPeptideNumber

        If intCurrentTrypticPeptideNumber > 0 And lngPrevStartLoc > 0 Then
            If lngPrevStartLoc > Len(strProteinResidues) Then
                ' User requested a peptide number that is too high
                lngReturnResidueStart = 0
                lngReturnResidueEnd = 0
                strMatchingFragment = String.Empty
            Else
                ' Match found, find the extent of this peptide
                lngReturnResidueStart = lngPrevStartLoc
                If lngRuleResidueLoc > lngProteinResiduesLength Then
                    lngReturnResidueEnd = lngProteinResiduesLength
                Else
                    lngReturnResidueEnd = lngRuleResidueLoc
                End If
                strMatchingFragment = Mid(strProteinResidues, lngPrevStartLoc, lngRuleResidueLoc - lngPrevStartLoc + 1)
            End If
        Else
            lngReturnResidueStart = 1
            lngReturnResidueEnd = lngProteinResiduesLength
            strMatchingFragment = strProteinResidues
        End If

        Return strMatchingFragment

    End Function

    Public Function CheckSequenceAgainstCleavageRule(ByVal strSequence As String, _
                                                     ByVal strRuleResidues As String, _
                                                     ByVal strExceptionSuffixResidues As String, _
                                                     ByVal blnAllowPartialCleavage As Boolean) As Boolean

        Return CheckSequenceAgainstCleavageRule(strSequence, strRuleResidues, strExceptionSuffixResidues, _
                                                blnAllowPartialCleavage, ".", TERMINII_SYMBOL, True, 0)

    End Function

    Public Function CheckSequenceAgainstCleavageRule(ByVal strSequence As String, _
                                                     ByVal strRuleResidues As String, _
                                                     ByVal strExceptionSuffixResidues As String, _
                                                     ByVal blnAllowPartialCleavage As Boolean, _
                                                     ByRef intRuleMatchCount As Short) As Boolean

        Return CheckSequenceAgainstCleavageRule(strSequence, strRuleResidues, strExceptionSuffixResidues, _
                                                blnAllowPartialCleavage, ".", TERMINII_SYMBOL, True, intRuleMatchCount)

    End Function


    Public Function CheckSequenceAgainstCleavageRule(ByVal strSequence As String, _
                                                     ByVal strRuleResidues As String, _
                                                     ByVal strExceptionSuffixResidues As String, _
                                                     ByVal blnAllowPartialCleavage As Boolean, _
                                                     ByVal strSeparationChar As String, _
                                                     ByVal strTerminiiSymbol As String, _
                                                     ByVal blnIgnoreCase As Boolean) As Boolean

        Return CheckSequenceAgainstCleavageRule(strSequence, strRuleResidues, strExceptionSuffixResidues, _
                                                blnAllowPartialCleavage, strSeparationChar, strTerminiiSymbol, blnIgnoreCase, 0)

    End Function


    Public Function CheckSequenceAgainstCleavageRule(ByVal strSequence As String, _
                                                     ByVal strRuleResidues As String, _
                                                     ByVal strExceptionSuffixResidues As String, _
                                                     ByVal blnAllowPartialCleavage As Boolean, _
                                                     ByVal strSeparationChar As String, _
                                                     ByVal strTerminiiSymbol As String, _
                                                     ByVal blnIgnoreCase As Boolean, _
                                                     ByRef intRuleMatchCount As Short) As Boolean

        ' Checks strSequence to see if it matches the cleavage rule
        ' Returns True if valid, False if invalid
        ' Returns True if doesn't contain any periods, and thus, can't be examined
        ' The ByRef variable intRuleMatchCount can be used to retrieve the number of ends that matched the rule (0, 1, or 2); terminii are counted as rule matches

        ' The residues in strRuleResidues specify the cleavage rule
        ' The peptide must end in one of the residues, or in -
        ' The preceding residue must be one of the residues or be -
        ' EXCEPTION: if blnAllowPartialCleavage = True then the rules need only apply to one end
        ' Finally, the suffix residue cannot match any of the residues in strExceptionSuffixResidues

        ' For example, if strRuleResidues = "KR" and strExceptionSuffixResidues = "P"
        ' Then if strSequence = "R.AEQDDLANYGPGNGVLPSAGSSISMEK.L" then blnMatchesCleavageRule = True
        ' However, if strSequence = "R.IGASGEHIFIIGVDK.P" then blnMatchesCleavageRule = False since strSuffix = "P"
        ' Finally, if strSequence = "R.IGASGEHIFIIGVDKPNR.Q" then blnMatchesCleavageRule = True since K is ignored, but the final R.Q is valid

        Dim strSequenceStart, strSequenceEnd As String
        Dim strPrefix, strSuffix As String
        Dim blnMatchesCleavageRule, blnSkipThisEnd As Boolean
        Dim strTestResidue As String
        Dim intEndToCheck As Short

        ' Need to reset this to zero since passed ByRef
        intRuleMatchCount = 0
        strPrefix = String.Empty
        strSuffix = String.Empty

        ' First, make sure the sequence is in the form A.BCDEFG.H or A.BCDEFG or BCDEFG.H
        ' If it isn't, then we can't check it (we'll return true)

        If strRuleResidues Is Nothing OrElse strRuleResidues.Length = 0 Then
            ' No rule residues
            Return True
        End If

        If strSeparationChar Is Nothing Then strSeparationChar = "."

        If Not strSequence.Contains(strSeparationChar) Then
            ' No periods, can't check
            Console.WriteLine("Warning: strSequence does not contain " & strSeparationChar & "; unable to determine cleavage state")
            Return True
        End If

        If blnIgnoreCase Then
            strSequence = UCase(strSequence)
        End If

        ' Find the prefix residue and starting residue
        If Mid(strSequence, 2, 1) = strSeparationChar Then
            strPrefix = Left(strSequence, 1)
            strSequenceStart = Mid(strSequence, 3, 1)
        Else
            strSequenceStart = Left(strSequence, 1)
        End If

        ' Find the suffix residue and the ending residue
        If Mid(strSequence, Len(strSequence) - 1, 1) = strSeparationChar Then
            strSuffix = Right(strSequence, 1)
            strSequenceEnd = Mid(strSequence, Len(strSequence) - 2, 1)
        Else
            strSequenceEnd = Right(strSequence, 1)
        End If

        If strRuleResidues = strTerminiiSymbol Then
            ' Peptide database rules
            ' See if prefix and suffix are "" or are strTerminiiSymbol
            If (strPrefix = strTerminiiSymbol And strSuffix = strTerminiiSymbol) OrElse _
               (strPrefix = String.Empty And strSuffix = String.Empty) Then
                intRuleMatchCount = 2
                blnMatchesCleavageRule = True
            Else
                blnMatchesCleavageRule = False
            End If
        Else
            If blnIgnoreCase Then
                strRuleResidues = UCase(strRuleResidues)
            End If

            ' Test each character in strRuleResidues against both strPrefix and strSequenceEnd
            ' Make sure strSuffix does not match strExceptionSuffixResidues
            For intEndToCheck = 0 To 1
                blnSkipThisEnd = False
                If intEndToCheck = 0 Then
                    strTestResidue = strPrefix
                    If strPrefix = strTerminiiSymbol Then
                        intRuleMatchCount = intRuleMatchCount + 1S
                        blnSkipThisEnd = True
                    Else
                        ' See if strSequenceStart matches one of the exception residues
                        ' If it does, make sure strPrefix does not match one of the rule residues
                        If CheckSequenceAgainstCleavageRuleMatchTestResidue(strSequenceStart, strExceptionSuffixResidues) Then
                            ' Match found
                            ' Make sure strPrefix does not match one of the rule residues
                            If CheckSequenceAgainstCleavageRuleMatchTestResidue(strPrefix, strRuleResidues) Then
                                ' Match found; thus does not match cleavage rule
                                blnSkipThisEnd = True
                            End If
                        End If
                    End If
                Else
                    strTestResidue = strSequenceEnd
                    If strSuffix = strTerminiiSymbol Then
                        intRuleMatchCount = intRuleMatchCount + 1S
                        blnSkipThisEnd = True
                    Else
                        ' Make sure strSuffix does not match strExceptionSuffixResidues
                        If CheckSequenceAgainstCleavageRuleMatchTestResidue(strSuffix, strExceptionSuffixResidues) Then
                            ' Match found; thus does not match cleavage rule
                            blnSkipThisEnd = True
                        End If
                    End If
                End If

                If Not blnSkipThisEnd Then
                    If CheckSequenceAgainstCleavageRuleMatchTestResidue(strTestResidue, strRuleResidues) Then
                        intRuleMatchCount = intRuleMatchCount + 1S
                    End If
                End If
            Next intEndToCheck

            If intRuleMatchCount = 2 Then
                blnMatchesCleavageRule = True
            ElseIf intRuleMatchCount >= 1 And blnAllowPartialCleavage Then
                blnMatchesCleavageRule = True
            End If
        End If

        Return blnMatchesCleavageRule

    End Function

    Private Function CheckSequenceAgainstCleavageRuleMatchTestResidue(ByVal strTestResidue As String, ByVal strRuleResidues As String) As Boolean
        ' Checks to see if strTestResidue matches one of the residues in strRuleResidues
        ' Used to test by Rule Residues and Exception Residues

        Dim strCompareResidue As String

        For intCharIndex As Integer = 0 To strRuleResidues.Length - 1
            strCompareResidue = strRuleResidues.Substring(intCharIndex, 1).Trim()
            If strCompareResidue.Length > 0 Then
                If strTestResidue = strCompareResidue Then
                    ' Match found
                    Return True
                End If
            End If
        Next

        Return False

    End Function

    Public Function ComputeImmoniumMass(ByVal dblResidueMass As Double) As Double
        Return dblResidueMass - dblImmoniumMassDifference
    End Function

    Private Sub InitializeArrays()
        mNTerminus.Initialize()
        mCTerminus.Initialize()
        mFragSpectrumOptions.Initialize()
    End Sub

    Public Function LookupIonTypeString(ByVal eIonType As itIonTypeConstants) As String

        Select Case eIonType
            Case itIonTypeConstants.itAIon : Return "a"
            Case itIonTypeConstants.itBIon : Return "b"
            Case itIonTypeConstants.itYIon : Return "y"
            Case itIonTypeConstants.itCIon : Return "c"
            Case itIonTypeConstants.itZIon : Return "z"
            Case Else
                Return String.Empty
        End Select

    End Function

    Public Function RemoveAllResidues() As Integer
        ' Removes all the residues
        ' Returns 0 on success, 1 on failure

        ReserveMemoryForResidues(50, False)
        ResidueCount = 0
        mTotalMass = 0

        RemoveAllResidues = 0
    End Function

    Public Function RemoveAllModificationSymbols() As Integer
        ' Removes all possible Modification Symbols
        ' Returns 0 on success, 1 on failure
        ' Removing all modifications will invalidate any modifications present in a sequence

        ReserveMemoryForModifications(10, False)
        ModificationSymbolCount = 0

        RemoveAllModificationSymbols = 0
    End Function

    Private Sub RemoveLeadingH(ByRef strWorkingSequence As String)
        ' Returns True if a leading H is removed
        Dim lngAbbrevID As Integer

        If strWorkingSequence.Length >= 4 AndAlso strWorkingSequence.ToUpper.StartsWith("H") Then
            ' If next character is not a character, then remove the H and the non-letter character
            If Not Char.IsLetter(strWorkingSequence.Chars(1)) Then
                ' Remove the leading H
                strWorkingSequence = strWorkingSequence.Substring(2)
            Else
                ' Otherwise, see if next three characters are letters
                If Char.IsLetter(strWorkingSequence.Chars(1)) AndAlso Char.IsLetter(strWorkingSequence.Chars(2)) AndAlso Char.IsLetter(strWorkingSequence.Chars(3)) Then
                    ' Formula starts with 4 characters and the first is H, see if the first 3 characters are a valid amino acid code
                    lngAbbrevID = ElementAndMassRoutines.GetAbbreviationIDInternal(strWorkingSequence.Substring(0, 3), True)

                    If lngAbbrevID <= 0 Then
                        ' Doesn't start with a valid amino acid 3 letter abbreviation, so remove the initial H
                        strWorkingSequence = strWorkingSequence.Substring(1)
                    End If
                End If
            End If
        End If

        Return
    End Sub

    Private Function RemoveTrailingOH(ByRef strWorkingSequence As String) As Boolean
        ' Returns True if a trailing OH is removed
        Dim lngAbbrevID As Integer
        Dim blnOHRemoved As Boolean
        Dim lngStringLength As Integer

        blnOHRemoved = False
        lngStringLength = Len(strWorkingSequence)
        If strWorkingSequence.Length >= 5 AndAlso strWorkingSequence.ToUpper.EndsWith("OH") Then
            ' If previous character is not a character, then remove the OH
            If Not Char.IsLetter(CChar(Mid(strWorkingSequence, lngStringLength - 2, 1))) Then
                strWorkingSequence = Left(strWorkingSequence, lngStringLength - 3)
                blnOHRemoved = True
            Else
                ' Otherwise, see if previous three characters are letters
                If Char.IsLetter(CChar(Mid(strWorkingSequence, lngStringLength - 2, 1))) Then
                    ' Formula ends with 3 characters and the last two are OH, see if the last 3 characters are a valid amino acid code
                    lngAbbrevID = ElementAndMassRoutines.GetAbbreviationIDInternal(Mid(strWorkingSequence, lngStringLength - 2, 3), True)

                    If lngAbbrevID <= 0 Then
                        ' Doesn't end with a valid amino acid 3 letter abbreviation, so remove the trailing OH
                        strWorkingSequence = Left(strWorkingSequence, lngStringLength - 2)
                        blnOHRemoved = True
                    End If
                End If
            End If
        End If

        RemoveTrailingOH = blnOHRemoved

    End Function

    Public Function RemoveModification(ByRef strModSymbol As String) As Integer
        ' Returns 0 if found and removed; 1 if error

        Dim lngIndex As Integer
        Dim blnRemoved As Boolean

        For lngIndex = 1 To ModificationSymbolCount
            If ModificationSymbols(lngIndex).Symbol = strModSymbol Then
                RemoveModificationByID(lngIndex)
                blnRemoved = True
            End If
        Next lngIndex

        If blnRemoved Then
            RemoveModification = 0
        Else
            RemoveModification = 1
        End If
    End Function

    Public Function RemoveModificationByID(ByVal lngModificationID As Integer) As Integer
        ' Returns 0 if found and removed; 1 if error

        Dim lngIndex As Integer
        Dim blnRemoved As Boolean

        If lngModificationID >= 1 And lngModificationID <= ModificationSymbolCount Then
            For lngIndex = lngModificationID To ModificationSymbolCount - 1
                ModificationSymbols(lngIndex) = ModificationSymbols(lngIndex + 1)
            Next lngIndex
            ModificationSymbolCount = ModificationSymbolCount - 1
            blnRemoved = True
        Else
            blnRemoved = False
        End If

        If blnRemoved Then
            Return 0
        Else
            Return 1
        End If

    End Function

    Public Function RemoveResidue(ByVal lngResidueNumber As Integer) As Integer
        ' Returns 0 if found and removed; 1 if error

        Dim lngIndex As Integer

        If lngResidueNumber >= 1 And lngResidueNumber <= ResidueCount Then
            For lngIndex = lngResidueNumber To ResidueCount - 1
                Residues(lngIndex) = Residues(lngIndex + 1)
            Next lngIndex
            ResidueCount -= 1
            Return 0
        Else
            Return 1
        End If

    End Function

    Private Sub ReserveMemoryForResidues(ByVal lngNewResidueCount As Integer, ByVal blnPreserveContents As Boolean)
        ' Only reserves the memory if necessary
        ' Thus, do not use this sub to clear Residues()

        Dim intIndex As Integer
        Dim intOldIndexEnd As Integer

        If lngNewResidueCount > ResidueCountDimmed Then
            ResidueCountDimmed = lngNewResidueCount + RESIDUE_DIM_CHUNK
            If blnPreserveContents And Not Residues Is Nothing Then
                intOldIndexEnd = Residues.Length - 1
                ReDim Preserve Residues(ResidueCountDimmed)
                For intIndex = intOldIndexEnd + 1 To ResidueCountDimmed
                    Residues(intIndex).Initialize(True)
                Next intIndex
            Else
                ReDim Residues(ResidueCountDimmed)
                For intIndex = 0 To ResidueCountDimmed
                    Residues(intIndex).Initialize(True)
                Next intIndex
            End If
        End If
    End Sub

    Private Sub ReserveMemoryForModifications(ByVal lngNewModificationCount As Integer, ByVal blnPreserveContents As Boolean)

        If lngNewModificationCount > ModificationSymbolCountDimmed Then
            ModificationSymbolCountDimmed = lngNewModificationCount + 10
            If blnPreserveContents Then
                ReDim Preserve ModificationSymbols(ModificationSymbolCountDimmed)
            Else
                ReDim ModificationSymbols(ModificationSymbolCountDimmed)
            End If
        End If
    End Sub

    Public Function SetCTerminus(ByVal strFormula As String) As Integer
        Return SetCTerminus(strFormula, "", True)
    End Function

    Public Function SetCTerminus(ByVal strFormula As String, ByVal strFollowingResidue As String) As Integer
        Return SetCTerminus(strFormula, strFollowingResidue, True)
    End Function

    Public Function SetCTerminus(ByVal strFormula As String, _
                                  ByVal strFollowingResidue As String, _
                                  ByVal blnUse3LetterCode As Boolean) As Integer

        ' Returns 0 if success; 1 if error

        ' Typical N terminus mods
        ' Free Acid = OH
        ' Amide = NH2

        With mCTerminus
            .Formula = strFormula
            .Mass = ElementAndMassRoutines.ComputeFormulaWeight(.Formula)
            If .Mass < 0 Then
                .Mass = 0
                SetCTerminus = 1
            Else
                SetCTerminus = 0
            End If
            .PrecedingResidue = FillResidueStructureUsingSymbol(String.Empty)
            .FollowingResidue = FillResidueStructureUsingSymbol(strFollowingResidue, blnUse3LetterCode)
        End With

        UpdateResidueMasses()
    End Function

    Public Function SetCTerminusGroup(ByVal eCTerminusGroup As ctgCTerminusGroupConstants) As Integer
        Return SetCTerminusGroup(eCTerminusGroup, "", True)
    End Function

    Public Function SetCTerminusGroup(ByVal eCTerminusGroup As ctgCTerminusGroupConstants, ByVal strFollowingResidue As String) As Integer
        Return SetCTerminusGroup(eCTerminusGroup, strFollowingResidue, True)
    End Function

    Public Function SetCTerminusGroup(ByVal eCTerminusGroup As ctgCTerminusGroupConstants, _
                                      ByVal strFollowingResidue As String, _
                                      ByVal blnUse3LetterCode As Boolean) As Integer

        ' Returns 0 if success; 1 if error
        Dim lngError As Integer

        Select Case eCTerminusGroup
            Case ctgCTerminusGroupConstants.ctgHydroxyl : lngError = SetCTerminus("OH", strFollowingResidue, blnUse3LetterCode)
            Case ctgCTerminusGroupConstants.ctgAmide : lngError = SetCTerminus("NH2", strFollowingResidue, blnUse3LetterCode)
            Case ctgCTerminusGroupConstants.ctgNone : lngError = SetCTerminus(String.Empty, strFollowingResidue, blnUse3LetterCode)
            Case Else : lngError = 1
        End Select

        Return lngError

    End Function

    Public Sub SetDefaultModificationSymbols()

        Try
            RemoveAllModificationSymbols()

            ' Add the symbol for phosphorylation
            SetModificationSymbol("*", dblPhosphorylationMass, True, "Phosphorylation [HPO3]")

            ' Define the other default modifications
            ' Valid Mod Symbols are ! # $ % & ' * + ? ^ _ ` ~

            SetModificationSymbol("+", 14.01565, False, "Methylation [CH2]")
            SetModificationSymbol("@", 15.99492, False, "Oxidation [O]")
            SetModificationSymbol("!", 57.02146, False, "Carbamidomethylation [C2H3NO]")
            SetModificationSymbol("&", 58.00548, False, "Carboxymethylation [CH2CO2]")
            SetModificationSymbol("#", 71.03711, False, "Acrylamide [CHCH2CONH2]")
            SetModificationSymbol("$", 227.127, False, "Cleavable ICAT [(^12C10)H17N3O3]")
            SetModificationSymbol("%", 236.127, False, "Cleavable ICAT [(^13C9)(^12C)H17N3O3]")
            SetModificationSymbol("~", 442.225, False, "ICAT D0 [C20H34N4O5S]")
            SetModificationSymbol("`", 450.274, False, "ICAT D8 [C20H26D8N4O5S]")
        Catch ex As Exception
            ElementAndMassRoutines.GeneralErrorHandler("MWPeptideClass.SetDefaultModificationSymbols", Err.Number)
        End Try

    End Sub

    Public Sub SetDefaultOptions()
        Dim eIonIndex As itIonTypeConstants

        Try
            With mFragSpectrumOptions
                With .IntensityOptions
                    .IonType(itIonTypeConstants.itAIon) = DEFAULT_A_ION_INTENSITY
                    .IonType(itIonTypeConstants.itBIon) = DEFAULT_BYCZ_ION_INTENSITY
                    .IonType(itIonTypeConstants.itYIon) = DEFAULT_BYCZ_ION_INTENSITY
                    .IonType(itIonTypeConstants.itCIon) = DEFAULT_BYCZ_ION_INTENSITY
                    .IonType(itIonTypeConstants.itZIon) = DEFAULT_BYCZ_ION_INTENSITY
                    .BYIonShoulder = DEFAULT_B_Y_ION_SHOULDER_INTENSITY
                    .NeutralLoss = DEFAULT_NEUTRAL_LOSS_ION_INTENSITY
                End With

                ' A ions can have ammonia and phosphate loss, but not water loss
                With .IonTypeOptions(itIonTypeConstants.itAIon)
                    .ShowIon = True
                    .NeutralLossAmmonia = True
                    .NeutralLossPhosphate = True
                    .NeutralLossWater = False
                End With

                For eIonIndex = itIonTypeConstants.itBIon To itIonTypeConstants.itZIon
                    With .IonTypeOptions(eIonIndex)
                        .ShowIon = True
                        .NeutralLossAmmonia = True
                        .NeutralLossPhosphate = True
                        .NeutralLossWater = True
                    End With
                Next eIonIndex

                .DoubleChargeIonsShow = True
                .DoubleChargeIonsThreshold = DEFAULT_DOUBLE_CHARGE_MZ_THRESHOLD

                .TripleChargeIonsShow = False
                .TripleChargeIonsThreshold = DEFAULT_TRIPLE_CHARGE_MZ_THRESHOLD
            End With

            SetSymbolWaterLoss("-H2O")
            SetSymbolAmmoniaLoss("-NH3")
            SetSymbolPhosphoLoss("-H3PO4")

            UpdateStandardMasses()

            SetDefaultModificationSymbols()
        Catch ex As Exception
            ElementAndMassRoutines.GeneralErrorHandler("MWPeptideClass.SetDefaultOptions", Err.Number)
        End Try

    End Sub

    Public Sub SetFragmentationSpectrumOptions(ByVal udtNewFragSpectrumOptions As udtFragmentationSpectrumOptionsType)
        mFragSpectrumOptions = udtNewFragSpectrumOptions
    End Sub

    Public Sub SetModificationSymbol(ByVal strModSymbol As String, ByVal dblModificationMass As Double)
        SetModificationSymbol(strModSymbol, dblModificationMass, blnIndicatesPhosphorylation:=False, strComment:=String.Empty)
    End Sub

    Public Sub SetModificationSymbol(ByVal strModSymbol As String, ByVal dblModificationMass As Double, ByVal strComment As String)
        SetModificationSymbol(strModSymbol, dblModificationMass, blnIndicatesPhosphorylation:=False, strComment:=strComment)
    End Sub

    Public Function SetModificationSymbol(ByVal strModSymbol As String, ByVal dblModificationMass As Double, ByVal blnIndicatesPhosphorylation As Boolean, ByVal strComment As String) As Integer
        ' Adds a new modification or updates an existing one (based on strModSymbol)
        ' Returns 0 if successful, otherwise, returns -1

        Dim strTestChar As String
        Dim lngIndexToUse, lngIndex, lngErrorID As Integer

        lngErrorID = 0
        If Len(strModSymbol) < 1 Then
            lngErrorID = -1
        Else
            ' Make sure strModSymbol contains no letters, numbers, spaces, dashes, or periods
            For lngIndex = 1 To Len(strModSymbol)
                strTestChar = Mid(strModSymbol, lngIndex, 1)
                If Not ElementAndMassRoutines.IsModSymbolInternal(strTestChar) Then
                    lngErrorID = -1
                End If
            Next lngIndex

            If lngErrorID = 0 Then
                ' See if the modification is alrady present
                lngIndexToUse = GetModificationSymbolID(strModSymbol)

                If lngIndexToUse = 0 Then
                    ' Need to add the modification
                    ModificationSymbolCount = ModificationSymbolCount + 1
                    lngIndexToUse = ModificationSymbolCount
                    ReserveMemoryForModifications(ModificationSymbolCount, True)
                End If

                With ModificationSymbols(lngIndexToUse)
                    .Symbol = strModSymbol
                    .ModificationMass = dblModificationMass
                    .IndicatesPhosphorylation = blnIndicatesPhosphorylation
                    .Comment = strComment
                End With
            End If
        End If

        SetModificationSymbol = lngErrorID

    End Function

    Public Function SetNTerminus(ByVal strFormula As String) As Integer
        Return SetNTerminus(strFormula, "", True)
    End Function

    Public Function SetNTerminus(ByVal strFormula As String, ByVal strPrecedingResidue As String) As Integer
        Return SetNTerminus(strFormula, strPrecedingResidue, True)
    End Function

    Public Function SetNTerminus(ByVal strFormula As String, _
                                 ByVal strPrecedingResidue As String, _
                                 ByVal blnUse3LetterCode As Boolean) As Integer
        ' Returns 0 if success; 1 if error

        ' Typical N terminus mods
        ' Hydrogen = H
        ' Acetyl = C2OH3
        ' PyroGlu = C5O2NH6
        ' Carbamyl = CONH2
        ' PTC = C7H6NS

        With mNTerminus
            .Formula = strFormula
            .Mass = ElementAndMassRoutines.ComputeFormulaWeight(.Formula)
            If .Mass < 0 Then
                .Mass = 0
                SetNTerminus = 1
            Else
                SetNTerminus = 0
            End If
            .PrecedingResidue = FillResidueStructureUsingSymbol(strPrecedingResidue, blnUse3LetterCode)
            .FollowingResidue = FillResidueStructureUsingSymbol(String.Empty)
        End With

        UpdateResidueMasses()
    End Function

    Public Function SetNTerminusGroup(ByVal eNTerminusGroup As ntgNTerminusGroupConstants) As Integer
        Return SetNTerminusGroup(eNTerminusGroup, "", True)
    End Function

    Public Function SetNTerminusGroup(ByVal eNTerminusGroup As ntgNTerminusGroupConstants, ByVal strPrecedingResidue As String) As Integer
        Return SetNTerminusGroup(eNTerminusGroup, strPrecedingResidue, True)
    End Function


    Public Function SetNTerminusGroup(ByVal eNTerminusGroup As ntgNTerminusGroupConstants, _
                                      ByVal strPrecedingResidue As String, _
                                      ByVal blnUse3LetterCode As Boolean) As Integer

        ' Returns 0 if success; 1 if error
        Dim lngError As Integer

        Select Case eNTerminusGroup
            Case ntgNTerminusGroupConstants.ntgHydrogen : lngError = SetNTerminus("H", strPrecedingResidue, blnUse3LetterCode)
            Case ntgNTerminusGroupConstants.ntgHydrogenPlusProton : lngError = SetNTerminus("HH", strPrecedingResidue, blnUse3LetterCode)
            Case ntgNTerminusGroupConstants.ntgAcetyl : lngError = SetNTerminus("C2OH3", strPrecedingResidue, blnUse3LetterCode)
            Case ntgNTerminusGroupConstants.ntgPyroGlu : lngError = SetNTerminus("C5O2NH6", strPrecedingResidue, blnUse3LetterCode)
            Case ntgNTerminusGroupConstants.ntgCarbamyl : lngError = SetNTerminus("CONH2", strPrecedingResidue, blnUse3LetterCode)
            Case ntgNTerminusGroupConstants.ntgPTC : lngError = SetNTerminus("C7H6NS", strPrecedingResidue, blnUse3LetterCode)
            Case ntgNTerminusGroupConstants.ntgNone : lngError = SetNTerminus(String.Empty, strPrecedingResidue, blnUse3LetterCode)
            Case Else : lngError = 1
        End Select

        Return lngError

    End Function

    Public Function SetResidue(ByVal lngResidueNumber As Integer, _
                               ByVal strSymbol As String) As Integer
        Return SetResidue(lngResidueNumber, strSymbol, True, False)
    End Function

    Public Function SetResidue(ByVal lngResidueNumber As Integer, _
                               ByVal strSymbol As String, _
                               ByVal blnIs3LetterCode As Boolean) As Integer
        Return SetResidue(lngResidueNumber, strSymbol, blnIs3LetterCode, False)
    End Function


    Public Function SetResidue(ByVal lngResidueNumber As Integer, _
                               ByVal strSymbol As String, _
                               ByVal blnIs3LetterCode As Boolean, _
                               ByVal blnPhosphorylated As Boolean) As Integer

        ' Sets or adds a residue (must add residues in order)
        ' Returns the index of the modified residue, or the new index if added
        ' Returns -1 if a problem

        Dim lngIndexToUse As Integer
        Dim str3LetterSymbol As String

        If strSymbol Is Nothing OrElse strSymbol.Length = 0 Then
            Return -1
        End If

        If lngResidueNumber > ResidueCount Then
            ResidueCount = ResidueCount + 1
            ReserveMemoryForResidues(ResidueCount, True)
            lngIndexToUse = ResidueCount
        Else
            lngIndexToUse = lngResidueNumber
        End If

        With Residues(lngIndexToUse)
            If blnIs3LetterCode Then
                str3LetterSymbol = strSymbol
            Else
                str3LetterSymbol = ElementAndMassRoutines.GetAminoAcidSymbolConversionInternal(strSymbol, True)
            End If

            If Len(str3LetterSymbol) = 0 Then
                .Symbol = UNKNOWN_SYMBOL
            Else
                .Symbol = str3LetterSymbol
            End If

            .Phosphorylated = blnPhosphorylated
            If blnPhosphorylated Then
                ' Only Ser, Thr, or Tyr should be phosphorylated
                ' However, if the user sets other residues as phosphorylated, we'll allow that
                If Not (.Symbol = "Ser" OrElse .Symbol = "Thr" OrElse .Symbol = "Tyr") Then
                    Console.WriteLine("Residue '" & .Symbol & "' is marked as being phosphorylated; this is unexpected")
                End If
            End If

            .ModificationIDCount = 0
        End With

        UpdateResidueMasses()

        SetResidue = lngIndexToUse
    End Function

    Public Function SetResidueModifications(ByVal lngResidueNumber As Integer, ByVal intModificationCount As Short, ByVal lngModificationIDsOneBased() As Integer) As Integer
        ' Sets the modifications for a specific residue
        ' Modification Symbols are defined using successive calls to SetModificationSymbol()

        ' Returns 0 if modifications set; returns 1 if an error

        Dim intIndex As Short
        Dim lngNewModID As Integer

        If lngResidueNumber >= 1 And lngResidueNumber <= ResidueCount And intModificationCount >= 0 Then
            With Residues(lngResidueNumber)
                If intModificationCount > MAX_MODIFICATIONS Then
                    intModificationCount = MAX_MODIFICATIONS
                End If

                .ModificationIDCount = 0
                .Phosphorylated = False
                For intIndex = 1 To intModificationCount
                    lngNewModID = lngModificationIDsOneBased(intIndex)
                    If lngNewModID >= 1 And lngNewModID <= ModificationSymbolCount Then
                        .ModificationIDs(.ModificationIDCount) = lngNewModID

                        ' Check for phosphorylation
                        If ModificationSymbols(lngNewModID).IndicatesPhosphorylation Then
                            .Phosphorylated = True
                        End If

                        .ModificationIDCount = .ModificationIDCount + 1S
                    End If
                Next intIndex

            End With

            SetResidueModifications = 0
        Else
            SetResidueModifications = 1
        End If

    End Function

    ''' <summary>
    ''' Defines the peptide sequence
    ''' </summary>
    ''' <param name="strSequence">Peptide sequence using 3-letter amino acid symbols</param>
    ''' <returns>0 if success or 1 if an error</returns>
    ''' <remarks>If strSequence is blank or contains no valid residues, then will still return 0</remarks>
    Public Function SetSequence(ByVal strSequence As String) As Integer
        Return SetSequence(strSequence, _
               ntgNTerminusGroupConstants.ntgHydrogen, _
               ctgCTerminusGroupConstants.ctgHydroxyl, _
               blnIs3LetterCode:=True, bln1LetterCheckForPrefixAndSuffixResidues:=True, bln3LetterCheckForPrefixHandSuffixOH:=True, blnAddMissingModificationSymbols:=False)

    End Function

    ''' <summary>
    ''' Defines the peptide sequence
    ''' </summary>
    ''' <param name="strSequence">Peptide sequence using 1-letter amino acid symbols</param>
    ''' <returns>0 if success or 1 if an error</returns>
    ''' <remarks>If strSequence is blank or contains no valid residues, then will still return 0</remarks>
    Public Function SetSequence1LetterSymbol(ByVal strSequence As String) As Integer
        Return SetSequence(strSequence, _
         ntgNTerminusGroupConstants.ntgHydrogen, _
         ctgCTerminusGroupConstants.ctgHydroxyl, _
         blnIs3LetterCode:=False, bln1LetterCheckForPrefixAndSuffixResidues:=True, bln3LetterCheckForPrefixHandSuffixOH:=True, blnAddMissingModificationSymbols:=False)

    End Function

    ''' <summary>
    ''' Defines the peptide sequence
    ''' </summary>
    ''' <param name="strSequence">Peptide sequence</param>
    ''' <param name="blnIs3LetterCode">Set to True for 3-letter amino acid symbols, False for 1-letter symbols (for example, R.ABCDEF.R)</param>
    ''' <param name="bln1LetterCheckForPrefixAndSuffixResidues"></param>
    ''' <returns>0 if success or 1 if an error</returns>
    ''' <remarks>If strSequence is blank or contains no valid residues, then will still return 0</remarks>
    Public Function SetSequence(ByVal strSequence As String, _
     ByVal blnIs3LetterCode As Boolean, _
     ByVal bln1LetterCheckForPrefixAndSuffixResidues As Boolean) As Integer

        Return SetSequence(strSequence, ntgNTerminusGroupConstants.ntgHydrogen, ctgCTerminusGroupConstants.ctgHydroxyl, _
         blnIs3LetterCode, bln1LetterCheckForPrefixAndSuffixResidues, bln3LetterCheckForPrefixHandSuffixOH:=True, blnAddMissingModificationSymbols:=False)

    End Function

    ''' <summary>
    ''' Defines the peptide sequence
    ''' </summary>
    ''' <param name="strSequence">Peptide sequence using 3-letter amino acid symbols</param>
    ''' <param name="eNTerminus">N-terminus group</param>
    ''' <param name="eCTerminus">C-terminus group</param>
    ''' <returns>0 if success or 1 if an error</returns>
    ''' <remarks>If strSequence is blank or contains no valid residues, then will still return 0</remarks>
    Public Function SetSequence(ByVal strSequence As String, _
                                ByVal eNTerminus As ntgNTerminusGroupConstants, _
                                ByVal eCTerminus As ctgCTerminusGroupConstants) As Integer

        Return SetSequence(strSequence, eNTerminus, eCTerminus, _
            blnIs3LetterCode:=True, bln1LetterCheckForPrefixAndSuffixResidues:=True, bln3LetterCheckForPrefixHandSuffixOH:=True, blnAddMissingModificationSymbols:=False)
    End Function

    ''' <summary>
    ''' Defines the peptide sequence
    ''' </summary>
    ''' <param name="strSequence">Peptide sequence</param>
    ''' <param name="eNTerminus">N-terminus group</param>
    ''' <param name="eCTerminus">C-terminus group</param>
    ''' <param name="blnIs3LetterCode">Set to True for 3-letter amino acid symbols, False for 1-letter symbols (for example, R.ABCDEF.R)</param>
    ''' <returns>0 if success or 1 if an error</returns>
    ''' <remarks>If strSequence is blank or contains no valid residues, then will still return 0</remarks>
    Public Function SetSequence(ByVal strSequence As String, _
                                ByVal eNTerminus As ntgNTerminusGroupConstants, _
                                ByVal eCTerminus As ctgCTerminusGroupConstants, _
                                ByVal blnIs3LetterCode As Boolean) As Integer

        Return SetSequence(strSequence, eNTerminus, eCTerminus, _
            blnIs3LetterCode, bln1LetterCheckForPrefixAndSuffixResidues:=True, bln3LetterCheckForPrefixHandSuffixOH:=True, blnAddMissingModificationSymbols:=False)

    End Function

    ''' <summary>
    ''' Defines the peptide sequence
    ''' </summary>
    ''' <param name="strSequence">Peptide sequence</param>
    ''' <param name="eNTerminus">N-terminus group</param>
    ''' <param name="eCTerminus">C-terminus group</param>
    ''' <param name="blnIs3LetterCode">Set to True for 3-letter amino acid symbols, False for 1-letter symbols (for example, R.ABCDEF.R)</param>
    ''' <param name="bln1LetterCheckForPrefixAndSuffixResidues">Set to True to check for and remove prefix and suffix residues when blnIs3LetterCode = False</param>
    ''' <returns>0 if success or 1 if an error</returns>
    ''' <remarks>If strSequence is blank or contains no valid residues, then will still return 0</remarks>
    Public Function SetSequence(ByVal strSequence As String, _
             ByVal eNTerminus As ntgNTerminusGroupConstants, _
             ByVal eCTerminus As ctgCTerminusGroupConstants, _
             ByVal blnIs3LetterCode As Boolean, _
             ByVal bln1LetterCheckForPrefixAndSuffixResidues As Boolean) As Integer

        Return SetSequence(strSequence, eNTerminus, eCTerminus, _
            blnIs3LetterCode, bln1LetterCheckForPrefixAndSuffixResidues, bln3LetterCheckForPrefixHandSuffixOH:=True, blnAddMissingModificationSymbols:=False)

    End Function

    ''' <summary>
    ''' Defines the peptide sequence
    ''' </summary>
    ''' <param name="strSequence">Peptide sequence</param>
    ''' <param name="eNTerminus">N-terminus group</param>
    ''' <param name="eCTerminus">C-terminus group</param>
    ''' <param name="blnIs3LetterCode">Set to True for 3-letter amino acid symbols, False for 1-letter symbols (for example, R.ABCDEF.R)</param>
    ''' <param name="bln1LetterCheckForPrefixAndSuffixResidues">Set to True to check for and remove prefix and suffix residues when blnIs3LetterCode = False</param>
    ''' <param name="bln3LetterCheckForPrefixHandSuffixOH">Set to True to check for and remove prefix H and OH when blnIs3LetterCode = True</param>
    ''' <returns>0 if success or 1 if an error</returns>
    ''' <remarks>If strSequence is blank or contains no valid residues, then will still return 0</remarks>
    Public Function SetSequence(ByVal strSequence As String, _
                                ByVal eNTerminus As ntgNTerminusGroupConstants, _
                                ByVal eCTerminus As ctgCTerminusGroupConstants, _
                                ByVal blnIs3LetterCode As Boolean, _
                                ByVal bln1LetterCheckForPrefixAndSuffixResidues As Boolean, _
                                ByVal bln3LetterCheckForPrefixHandSuffixOH As Boolean) As Integer

        Return SetSequence(strSequence, eNTerminus, eCTerminus, _
          blnIs3LetterCode, _
          bln1LetterCheckForPrefixAndSuffixResidues, _
          bln3LetterCheckForPrefixHandSuffixOH, blnAddMissingModificationSymbols:=False)

    End Function

    ''' <summary>
    ''' Defines the peptide sequence
    ''' </summary>
    ''' <param name="strSequence">Peptide sequence</param>
    ''' <param name="eNTerminus">N-terminus group</param>
    ''' <param name="eCTerminus">C-terminus group</param>
    ''' <param name="blnIs3LetterCode">Set to True for 3-letter amino acid symbols, False for 1-letter symbols (for example, R.ABCDEF.R)</param>
    ''' <param name="bln1LetterCheckForPrefixAndSuffixResidues">Set to True to check for and remove prefix and suffix residues when blnIs3LetterCode = False</param>
    ''' <param name="bln3LetterCheckForPrefixHandSuffixOH">Set to True to check for and remove prefix H and OH when blnIs3LetterCode = True</param>
    ''' <param name="blnAddMissingModificationSymbols">Set to True to automatically add missing modification symbols (though the mod masses will be 0)</param>
    ''' <returns>0 if success or 1 if an error</returns>
    ''' <remarks>If strSequence is blank or contains no valid residues, then will still return 0</remarks>
    Public Function SetSequence(ByVal strSequence As String, _
                            ByVal eNTerminus As ntgNTerminusGroupConstants, _
                            ByVal eCTerminus As ctgCTerminusGroupConstants, _
                            ByVal blnIs3LetterCode As Boolean, _
                            ByVal bln1LetterCheckForPrefixAndSuffixResidues As Boolean, _
                            ByVal bln3LetterCheckForPrefixHandSuffixOH As Boolean, _
                            ByVal blnAddMissingModificationSymbols As Boolean) As Integer

        Dim lngIndex, lngSequenceStrLength, lngModSymbolLength As Integer
        Dim str3LetterSymbol, str1LetterSymbol, strFirstChar As String

        Try
            strSequence = Trim(strSequence)

            lngSequenceStrLength = Len(strSequence)
            If lngSequenceStrLength = 0 Then
                Return AssureNonZero(0)
            End If

            ' Clear any old residue information
            ResidueCount = 0
            ReserveMemoryForResidues(ResidueCount, False)

            If Not blnIs3LetterCode Then
                ' Sequence is 1 letter codes

                If bln1LetterCheckForPrefixAndSuffixResidues Then
                    ' First look if sequence is in the form A.BCDEFG.Z or -.BCDEFG.Z or A.BCDEFG.-
                    ' If so, then need to strip out the preceding A and Z residues since they aren't really part of the sequence
                    If lngSequenceStrLength > 1 And strSequence.Contains(".") Then
                        If Mid(strSequence, 2, 1) = "." Then
                            strSequence = Mid(strSequence, 3)
                            lngSequenceStrLength = Len(strSequence)
                        End If

                        If Mid(strSequence, lngSequenceStrLength - 1, 1) = "." Then
                            strSequence = Left(strSequence, lngSequenceStrLength - 2)
                            lngSequenceStrLength = Len(strSequence)
                        End If

                        ' Also check for starting with a . or ending with a .
                        If Left(strSequence, 1) = "." Then
                            strSequence = Mid(strSequence, 2)
                        End If

                        If Right(strSequence, 1) = "." Then
                            strSequence = Left(strSequence, Len(strSequence) - 1)
                        End If

                        lngSequenceStrLength = Len(strSequence)
                    End If

                End If

                For lngIndex = 1 To lngSequenceStrLength
                    str1LetterSymbol = Mid(strSequence, lngIndex, 1)
                    If Char.IsLetter(CChar(str1LetterSymbol)) Then
                        ' Character found
                        ' Look up 3 letter symbol
                        ' If none is found, this will return an empty string
                        str3LetterSymbol = ElementAndMassRoutines.GetAminoAcidSymbolConversionInternal(str1LetterSymbol, True)

                        If Len(str3LetterSymbol) = 0 Then str3LetterSymbol = UNKNOWN_SYMBOL

                        SetSequenceAddResidue(str3LetterSymbol)

                        ' Look at following character(s), and record any modification symbols present
                        lngModSymbolLength = CheckForModifications(Mid(strSequence, lngIndex + 1), ResidueCount, blnAddMissingModificationSymbols)

                        lngIndex += lngModSymbolLength
                    Else
                        ' If . or - or space, then ignore it
                        ' If a number, ignore it
                        ' If anything else, then should have been skipped, or should be skipped
                        If str1LetterSymbol = "." Or str1LetterSymbol = "-" Or str1LetterSymbol = " " Then
                            ' All is fine; we can skip this
                        Else
                            ' Ignore it
                        End If
                    End If
                Next lngIndex

            Else
                ' Sequence is 3 letter codes
                lngIndex = 1

                If bln3LetterCheckForPrefixHandSuffixOH Then
                    ' Look for a leading H or trailing OH, provided those don't match any of the amino acids
                    RemoveLeadingH(strSequence)
                    RemoveTrailingOH(strSequence)

                    ' Recompute sequence length
                    lngSequenceStrLength = Len(strSequence)
                End If

                Do While lngIndex <= lngSequenceStrLength - 2
                    strFirstChar = Mid(strSequence, lngIndex, 1)
                    If Char.IsLetter(CChar(strFirstChar)) Then
                        If Char.IsLetter(CChar(Mid(strSequence, lngIndex + 1, 1))) And Char.IsLetter(CChar(Mid(strSequence, lngIndex + 2, 1))) Then

                            str3LetterSymbol = UCase(strFirstChar) & LCase(Mid(strSequence, lngIndex + 1, 2))

                            If ElementAndMassRoutines.GetAbbreviationIDInternal(str3LetterSymbol, True) = 0 Then
                                ' 3 letter symbol not found
                                ' Add anyway, but mark as Xxx
                                str3LetterSymbol = UNKNOWN_SYMBOL
                            End If

                            SetSequenceAddResidue(str3LetterSymbol)

                            ' Look at following character(s), and record any modification symbols present
                            lngModSymbolLength = CheckForModifications(Mid(strSequence, lngIndex + 3), ResidueCount, blnAddMissingModificationSymbols)

                            lngIndex = lngIndex + 3
                            lngIndex = lngIndex + lngModSymbolLength

                        Else
                            ' First letter is a character, but next two are not; ignore it
                            lngIndex = lngIndex + 1
                        End If
                    Else
                        ' If . or - or space, then ignore it
                        ' If a number, ignore it
                        ' If anything else, then should have been skipped or should be skipped
                        If strFirstChar = "." Or strFirstChar = "-" Or strFirstChar = " " Then
                            ' All is fine; we can skip this
                        Else
                            ' Ignore it
                        End If
                        lngIndex = lngIndex + 1
                    End If
                Loop
            End If

            ' By calling SetNTerminus and SetCTerminus, the UpdateResidueMasses() Sub will also be called
            mDelayUpdateResidueMass = True
            SetNTerminusGroup(eNTerminus)
            SetCTerminusGroup(eCTerminus)

            mDelayUpdateResidueMass = False
            UpdateResidueMasses()

            Return 0
        Catch ex As Exception
            Return AssureNonZero(Err.Number)
        End Try

    End Function

    Private Sub SetSequenceAddResidue(ByVal str3LetterSymbol As String)

        If String.IsNullOrWhiteSpace(str3LetterSymbol) Then
            str3LetterSymbol = UNKNOWN_SYMBOL
        End If

        ResidueCount += 1
        ReserveMemoryForResidues(ResidueCount, True)

        With Residues(ResidueCount)
            .Symbol = str3LetterSymbol
            .Phosphorylated = False
            .ModificationIDCount = 0
        End With

    End Sub

    Public Sub SetSymbolAmmoniaLoss(ByVal strNewSymbol As String)
        If Not String.IsNullOrWhiteSpace(strNewSymbol) Then
            mAmmoniaLossSymbol = strNewSymbol
        End If
    End Sub

    Public Sub SetSymbolPhosphoLoss(ByVal strNewSymbol As String)
        If Not String.IsNullOrWhiteSpace(strNewSymbol) Then
            mPhosphoLossSymbol = strNewSymbol
        End If
    End Sub

    Public Sub SetSymbolWaterLoss(ByVal strNewSymbol As String)
        If Not String.IsNullOrWhiteSpace(strNewSymbol) Then
            mWaterLossSymbol = strNewSymbol
        End If
    End Sub

    Private Sub ShellSortFragSpectrum(ByRef FragSpectrumWork() As udtFragmentationSpectrumDataType, ByRef PointerArray() As Integer, ByVal lngLowIndex As Integer, ByVal lngHighIndex As Integer)
        ' Sort the list using a shell sort
        Dim lngCount As Integer
        Dim lngIncrement As Integer
        Dim lngIndex As Integer
        Dim lngIndexCompare As Integer
        Dim lngPointerSwap As Integer

        ' Sort PointerArray[lngLowIndex..lngHighIndex] by comparing FragSpectrumWork(PointerArray(x)).Mass

        ' Compute largest increment
        lngCount = lngHighIndex - lngLowIndex + 1
        lngIncrement = 1
        If (lngCount < 14) Then
            lngIncrement = 1
        Else
            Do While lngIncrement < lngCount
                lngIncrement = 3 * lngIncrement + 1
            Loop
            lngIncrement = lngIncrement \ 3
            lngIncrement = lngIncrement \ 3
        End If

        Do While lngIncrement > 0
            ' Sort by insertion in increments of lngIncrement
            For lngIndex = lngLowIndex + lngIncrement To lngHighIndex
                lngPointerSwap = PointerArray(lngIndex)
                For lngIndexCompare = lngIndex - lngIncrement To lngLowIndex Step -lngIncrement
                    ' Use <= to sort ascending; Use > to sort descending
                    If FragSpectrumWork(PointerArray(lngIndexCompare)).Mass <= FragSpectrumWork(lngPointerSwap).Mass Then Exit For
                    PointerArray(lngIndexCompare + lngIncrement) = PointerArray(lngIndexCompare)
                Next lngIndexCompare
                PointerArray(lngIndexCompare + lngIncrement) = lngPointerSwap
            Next lngIndex
            lngIncrement = lngIncrement \ 3
        Loop

    End Sub

    Private Sub UpdateResidueMasses()
        Dim lngIndex, lngAbbrevID As Integer
        Dim lngValidResidueCount As Integer
        Dim intModIndex As Short
        Dim dblRunningTotal As Double
        Dim blnPhosphorylationMassAdded As Boolean
        Dim blnProtonatedNTerminus As Boolean

        If mDelayUpdateResidueMass Then Exit Sub

        ' The N-terminus ions are the basis for the running total
        dblRunningTotal = mNTerminus.Mass
        If UCase(mNTerminus.Formula) = "HH" Then
            ' ntgHydrogenPlusProton; since we add back in the proton below when computing the fragment masses,
            '  we need to subtract it out here
            ' However, we need to subtract out dblHydrogenMass, and not dblChargeCarrierMass since the current
            '  formula's mass was computed using two hydrogens, and not one hydrogen and one charge carrier
            blnProtonatedNTerminus = True
            dblRunningTotal = dblRunningTotal - dblHydrogenMass
        End If

        For lngIndex = 1 To ResidueCount
            With Residues(lngIndex)
                .Initialize()

                lngAbbrevID = ElementAndMassRoutines.GetAbbreviationIDInternal(.Symbol, True)

                If lngAbbrevID > 0 Then
                    lngValidResidueCount = lngValidResidueCount + 1
                    .Mass = ElementAndMassRoutines.GetAbbreviationMass(lngAbbrevID)

                    blnPhosphorylationMassAdded = False

                    ' Compute the mass, including the modifications
                    .MassWithMods = .Mass
                    For intModIndex = 1 To .ModificationIDCount
                        If .ModificationIDs(intModIndex) <= ModificationSymbolCount Then
                            .MassWithMods = .MassWithMods + ModificationSymbols(.ModificationIDs(intModIndex)).ModificationMass
                            If ModificationSymbols(.ModificationIDs(intModIndex)).IndicatesPhosphorylation Then
                                blnPhosphorylationMassAdded = True
                            End If
                        Else
                            ' Invalid ModificationID
                            Console.WriteLine("Invalid ModificationID: " & .ModificationIDs(intModIndex))
                        End If
                    Next intModIndex

                    If .Phosphorylated Then
                        ' Only add a mass if none of the .ModificationIDs has .IndicatesPhosphorylation = True
                        If Not blnPhosphorylationMassAdded Then
                            .MassWithMods = .MassWithMods + dblPhosphorylationMass
                        End If
                    End If

                    dblRunningTotal = dblRunningTotal + .MassWithMods

                    .IonMass(itIonTypeConstants.itAIon) = dblRunningTotal - dblImmoniumMassDifference - dblChargeCarrierMass
                    .IonMass(itIonTypeConstants.itBIon) = dblRunningTotal

                    ' Add NH3 (ammonia) to the B ion mass to get the C ion mass
                    .IonMass(itIonTypeConstants.itCIon) = .IonMass(itIonTypeConstants.itBIon) + dblNH3Mass
                Else
                    .Mass = 0
                    .MassWithMods = 0
                    System.Array.Clear(.IonMass, 0, .IonMass.Length)
                End If
            End With
        Next lngIndex

        dblRunningTotal = dblRunningTotal + mCTerminus.Mass
        If blnProtonatedNTerminus Then
            dblRunningTotal = dblRunningTotal + dblChargeCarrierMass
        End If

        If lngValidResidueCount > 0 Then
            mTotalMass = dblRunningTotal
        Else
            mTotalMass = 0
        End If

        ' Now compute the y-ion and z-ion masses
        dblRunningTotal = mCTerminus.Mass + dblChargeCarrierMass

        For lngIndex = ResidueCount To 1 Step -1
            With Residues(lngIndex)
                If .IonMass(itIonTypeConstants.itAIon) > 0 Then
                    dblRunningTotal = dblRunningTotal + .MassWithMods
                    .IonMass(itIonTypeConstants.itYIon) = dblRunningTotal + dblChargeCarrierMass
                    If lngIndex = 1 Then
                        ' Add the N-terminus mass to highest y ion
                        .IonMass(itIonTypeConstants.itYIon) = .IonMass(itIonTypeConstants.itYIon) + mNTerminus.Mass - dblChargeCarrierMass
                        If blnProtonatedNTerminus Then
                            ' ntgHydrogenPlusProton; since we add back in the proton below when computing the fragment masses,
                            '  we need to subtract it out here
                            ' However, we need to subtract out dblHydrogenMass, and not dblChargeCarrierMass since the current
                            '  formula's mass was computed using two hydrogens, and not one hydrogen and one charge carrier
                            .IonMass(itIonTypeConstants.itYIon) = .IonMass(itIonTypeConstants.itYIon) - dblHydrogenMass
                        End If
                    End If

                    ' Subtract NH2 (amide) from the Y ion mass to get the Z ion mass
                    .IonMass(itIonTypeConstants.itZIon) = .IonMass(itIonTypeConstants.itYIon) - (dblNH3Mass - dblHydrogenMass)
                End If
            End With
        Next lngIndex

    End Sub

    Public Sub UpdateStandardMasses()
        Dim eElementModeSaved As MWElementAndMassRoutines.emElementModeConstants

        Try
            eElementModeSaved = ElementAndMassRoutines.GetElementModeInternal()

            ElementAndMassRoutines.SetElementModeInternal(MWElementAndMassRoutines.emElementModeConstants.emIsotopicMass)

            dblChargeCarrierMass = ElementAndMassRoutines.GetChargeCarrierMassInternal()

            ' Update standard mass values
            dblHOHMass = ElementAndMassRoutines.ComputeFormulaWeight("HOH")
            dblNH3Mass = ElementAndMassRoutines.ComputeFormulaWeight("NH3")
            dblH3PO4Mass = ElementAndMassRoutines.ComputeFormulaWeight("H3PO4")
            dblHydrogenMass = ElementAndMassRoutines.ComputeFormulaWeight("H")

            ' Phosphorylation is the loss of OH and the addition of H2PO4, for a net change of HPO3
            dblPhosphorylationMass = ElementAndMassRoutines.ComputeFormulaWeight("HPO3")

            ' The immonium mass is equal to the mass of CO minus the mass of H, thus typically 26.9871
            dblImmoniumMassDifference = ElementAndMassRoutines.ComputeFormulaWeight("CO") - dblHydrogenMass

            dblHistidineFW = ElementAndMassRoutines.ComputeFormulaWeight("His")
            dblPhenylalanineFW = ElementAndMassRoutines.ComputeFormulaWeight("Phe")
            dblTyrosineFW = ElementAndMassRoutines.ComputeFormulaWeight("Tyr")

            ElementAndMassRoutines.SetElementModeInternal(eElementModeSaved)
        Catch ex As Exception
            ElementAndMassRoutines.GeneralErrorHandler("MWPeptideClass.UpdateStandardMasses", Err.Number)
        End Try

    End Sub

    Private Sub InitializeClass()

        Try
            InitializeArrays()

            ResidueCountDimmed = 0
            ResidueCount = 0
            ReserveMemoryForResidues(50, False)

            ModificationSymbolCountDimmed = 0
            ModificationSymbolCount = 0
            ReserveMemoryForModifications(10, False)

            SetDefaultOptions()
        Catch ex As Exception
            ElementAndMassRoutines.GeneralErrorHandler("MWPeptideClass.Class_Initialize", Err.Number)
        End Try

    End Sub


End Class