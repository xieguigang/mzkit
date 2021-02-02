#Region "Microsoft.VisualBasic::35175a985291601ff553d5ae8a9e36e1, MwtWinDll\MwtWinDll\Compound.vb"

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

    ' Class MWCompoundClass
    ' 
    '     Properties: CautionDescription, Charge, ErrorDescription, ErrorID, Formula
    '                 FormulaCapitalized, FormulaRTF, (+2 Overloads) Mass, (+2 Overloads) MassAndStdDevString, StandardDeviation
    '                 ValueForX
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    '     Function: ConvertToEmpirical, ElementPresent, ExpandAbbreviations, GetAtomCountForElement, GetPercentCompositionForElement
    '               (+2 Overloads) GetPercentCompositionForElementAsString, GetUsedElementCount, SetFormula, XIsPresentAfterBracket
    ' 
    '     Sub: GetPercentCompositionForAllElements, InitializeClass, UpdateErrorAndCaution, UpdateMass
    ' 
    ' /********************************************************************************/

#End Region

Public Class MWCompoundClass

    ' Molecular Weight Calculator routines with ActiveX Class interfaces: MWCompoundClass

    ' The compound class can be used to represent a compound
    ' Use the Formula Property to enter the compound's formula
    ' Use ErrorDescription and CautionDescription to see if there are any problems with the formula
    ' Custom abbreviations can be defined using the SetAbbreviationInternal() function in ElementAndMassRoutines()
    ' Note that the standard amino acids and 16 other abbreviations are defined by default (see MemoryLoadAbbreviations())

    ' Use the Mass Property to get the mass of the compound

    ' -------------------------------------------------------------------------------
    ' Written by Matthew Monroe for the Department of Energy (PNNL, Richland, WA) in 2002
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

    Public Sub New(objMWElementAndMassRoutines As MWElementAndMassRoutines)
        MyBase.New()
        ElementAndMassRoutines = objMWElementAndMassRoutines
        InitializeClass()
    End Sub

    Private mStrFormula As String
    Private mStrFormattedFormula As String

    Private mValueForX As Double ' The value to assign to x when present after a square bracket.
    ' For example, in C6H6[xBr] if x = 1, then the formula is treated like C6H6Br
    ' If x = 2, then the formula is treated like C6H6Br2

    Private mCautionDescription As String
    Private mErrorDescription As String
    Private mErrorID As Integer

    Private mComputationStats As MWElementAndMassRoutines.udtComputationStatsType

    Private ElementAndMassRoutines As MWElementAndMassRoutines

    Public Function ConvertToEmpirical() As String
        ' Converts mStrFormula to its empirical formula and returns the result
        Dim strResult As String

        strResult = ElementAndMassRoutines.ConvertFormulaToEmpirical(mStrFormula)
        UpdateErrorAndCaution()

        If mErrorDescription = "" Then
            mStrFormula = strResult
            mStrFormattedFormula = strResult
            ConvertToEmpirical = strResult
        Else
            ConvertToEmpirical = ErrorDescription
        End If
    End Function

    Public Function ElementPresent(ByRef intElementID As Short) As Boolean
        ' Returns True if the element is present
        If intElementID >= 1 And intElementID <= MWElementAndMassRoutines.ELEMENT_COUNT Then
            ElementPresent = mComputationStats.Elements(intElementID).Used
        Else
            ElementPresent = False
        End If

    End Function

    Public Function ExpandAbbreviations() As String
        ' Expands abbreviations in mStrFormula and returns the result

        Dim strResult As String

        strResult = ElementAndMassRoutines.ExpandAbbreviationsInFormula(mStrFormula)
        UpdateErrorAndCaution()

        If mErrorDescription = "" Then
            mStrFormula = strResult
            mStrFormattedFormula = strResult
            ExpandAbbreviations = strResult
        Else
            ExpandAbbreviations = ErrorDescription
        End If

    End Function

    Public Function GetAtomCountForElement(intElementID As Short) As Double
        ' Return the number of atoms of a given element that are present in the formula
        ' Note that the number of atoms is not necessarily an integer (e.g. C5.5)

        If intElementID >= 1 And intElementID <= MWElementAndMassRoutines.ELEMENT_COUNT Then
            GetAtomCountForElement = mComputationStats.Elements(intElementID).Count
        Else
            GetAtomCountForElement = 0
        End If

    End Function

    Public Function GetPercentCompositionForElement(intElementID As Short) As Double
        ' Returns the percent composition for element
        ' Returns -1 if an invalid ID

        If intElementID >= 1 And intElementID <= MWElementAndMassRoutines.ELEMENT_COUNT Then
            GetPercentCompositionForElement = mComputationStats.PercentCompositions(intElementID).PercentComposition
        Else
            GetPercentCompositionForElement = -1
        End If

    End Function

    Public Function GetPercentCompositionForElementAsString(intElementID As Short) As String
        Return GetPercentCompositionForElementAsString(intElementID, True)

    End Function

    Public Function GetPercentCompositionForElementAsString(intElementID As Short, blnIncludeStandardDeviation As Boolean) As String
        ' Returns the percent composition and standard deviation for element
        ' Returns "" if an invalid ID
        Dim strElementSymbol As String
        Dim strPctComposition As String

        If intElementID >= 1 And intElementID <= MWElementAndMassRoutines.ELEMENT_COUNT Then

            With mComputationStats.PercentCompositions(intElementID)

                strElementSymbol = ElementAndMassRoutines.GetElementSymbolInternal(intElementID) & ":"
                strPctComposition = ElementAndMassRoutines.ReturnFormattedMassAndStdDev(.PercentComposition, .StdDeviation, blnIncludeStandardDeviation, True)
                If .PercentComposition < 10 Then
                    strPctComposition = " " & strPctComposition
                End If
                GetPercentCompositionForElementAsString = ElementAndMassRoutines.SpacePad(strElementSymbol, 4) & strPctComposition
            End With
        Else
            GetPercentCompositionForElementAsString = ""
        End If

    End Function

    Public Sub GetPercentCompositionForAllElements(ByRef strPctCompositionsOneBased() As String)
        ' Returns the percent composition for all elements in strPctCompositionsOneBased

        Dim intElementIndex, intMaxIndex As Short

        Try
            intMaxIndex = CShort(UBound(strPctCompositionsOneBased))

            If intMaxIndex < MWElementAndMassRoutines.ELEMENT_COUNT Then
                ' Try to reserve more space in strPctCompositionsOneBased()
                Try
                    ReDim strPctCompositionsOneBased(MWElementAndMassRoutines.ELEMENT_COUNT)
                    intMaxIndex = CShort(UBound(strPctCompositionsOneBased))
                Catch ex As Exception
                    ' Ignore errors; probably a fixed length array that cannot be resized
                End Try
            End If

            If intMaxIndex >= MWElementAndMassRoutines.ELEMENT_COUNT Then intMaxIndex = MWElementAndMassRoutines.ELEMENT_COUNT

            ElementAndMassRoutines.ComputePercentComposition(mComputationStats)

            For intElementIndex = 1 To intMaxIndex
                With mComputationStats.PercentCompositions(intElementIndex)
                    If .PercentComposition > 0 Then
                        strPctCompositionsOneBased(intElementIndex) = ElementAndMassRoutines.ReturnFormattedMassAndStdDev(.PercentComposition, .StdDeviation)
                    Else
                        strPctCompositionsOneBased(intElementIndex) = ""
                    End If
                End With
            Next intElementIndex
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error occurred while copying percent composition values.  Probably an uninitialized array.")
        End Try

    End Sub

    Public Function GetUsedElementCount() As Short
        ' Returns the number of unique elements present in mStrFormula

        Dim intTotalElements As Short
        Dim intElementIndex As Short

        ' Determine # of elements in formula
        intTotalElements = 0
        For intElementIndex = 1 To MWElementAndMassRoutines.ELEMENT_COUNT
            ' Increment .TotalElements if element is present
            If mComputationStats.Elements(intElementIndex).Used Then
                intTotalElements = intTotalElements + 1S
            End If
        Next intElementIndex

        GetUsedElementCount = intTotalElements
    End Function

    Private Sub InitializeClass()
        mStrFormula = ""
        ValueForX = 1.0#
    End Sub

    Public Function SetFormula(strNewFormula As String) As Integer
        ' Provides an alternate method for setting the formula
        ' Returns ErrorID (0 if no error)

        Me.Formula = strNewFormula

        SetFormula = Me.ErrorID
    End Function

    Private Sub UpdateErrorAndCaution()
        mCautionDescription = ElementAndMassRoutines.GetCautionDescription()
        mErrorDescription = ElementAndMassRoutines.GetErrorDescription()
        mErrorID = ElementAndMassRoutines.GetErrorID()
    End Sub

    Private Sub UpdateMass()

        mStrFormattedFormula = mStrFormula

        ' mStrFormattedFormula is passed ByRef
        ' If gComputationOptions.CaseConversion = ccConvertCaseUp then mStrFormattedFormula is properly capitalized
        ' The mass of the compound is stored in mComputationStats.TotalMass
        ElementAndMassRoutines.ParseFormulaPublic(mStrFormattedFormula, mComputationStats, False, mValueForX)

        ElementAndMassRoutines.ComputePercentComposition(mComputationStats)

        UpdateErrorAndCaution()
    End Sub

    Public Function XIsPresentAfterBracket() As Boolean
        Dim intCharLoc As Short

        If ElementAndMassRoutines.gComputationOptions.BracketsAsParentheses Then
            ' Treating brackets as parentheses, therefore an x after a bracket isn't allowed
            XIsPresentAfterBracket = False
        Else
            intCharLoc = CShort(InStr(LCase(mStrFormattedFormula), "[x"))
            If intCharLoc > 0 Then
                If Mid(mStrFormattedFormula, intCharLoc + 1, 1) <> "e" Then
                    XIsPresentAfterBracket = True
                Else
                    XIsPresentAfterBracket = False
                End If
            Else
                XIsPresentAfterBracket = False
            End If
        End If

    End Function

    Public ReadOnly Property CautionDescription() As String
        Get
            Return mCautionDescription
        End Get
    End Property


    Public Property Charge() As Single
        Get
            Return mComputationStats.Charge
        End Get
        Set(Value As Single)
            mComputationStats.Charge = Value
        End Set
    End Property

    Public ReadOnly Property ErrorDescription() As String
        Get
            Return mErrorDescription
        End Get
    End Property

    Public ReadOnly Property ErrorID() As Integer
        Get
            Return mErrorID
        End Get
    End Property
    
    Public Property Formula() As String
        Get
            Return mStrFormula
        End Get
        Set(Value As String)
            mStrFormula = Value

            ' Recompute the mass for this formula
            ' Updates Error and Caution statements if there is a problem
            UpdateMass()
        End Set
    End Property

    Public ReadOnly Property FormulaCapitalized() As String
        Get
            Return mStrFormattedFormula
        End Get
    End Property

    Public ReadOnly Property FormulaRTF() As String
        Get
            Return ElementAndMassRoutines.PlainTextToRtfInternal((Me.FormulaCapitalized), False)
        End Get
    End Property

    Public ReadOnly Property Mass() As Double
        Get
            Return Me.Mass(True)
        End Get
    End Property

    Public ReadOnly Property Mass(blnRecomputeMass As Boolean) As Double
        Get
            If blnRecomputeMass Then UpdateMass()

            Return mComputationStats.TotalMass
        End Get
    End Property

    Public ReadOnly Property MassAndStdDevString() As String
        Get
            Return Me.MassAndStdDevString(True)
        End Get
    End Property
    Public ReadOnly Property MassAndStdDevString(blnRecomputeMass As Boolean) As String
        Get
            If blnRecomputeMass Then UpdateMass()

            With mComputationStats
                MassAndStdDevString = ElementAndMassRoutines.ReturnFormattedMassAndStdDev(.TotalMass, .StandardDeviation)
            End With
        End Get
    End Property

    Public ReadOnly Property StandardDeviation() As Double
        Get
            Return mComputationStats.StandardDeviation
        End Get
    End Property


    Public Property ValueForX() As Double
        Get
            Return mValueForX
        End Get
        Set(Value As Double)
            If Value >= 0 Then mValueForX = Value
        End Set
    End Property


End Class
