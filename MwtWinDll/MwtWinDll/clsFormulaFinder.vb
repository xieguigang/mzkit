Option Strict On

Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports System.Text
Imports MwtWinDll.MolecularWeightCalculator

Public Class MWFormulaFinder

#Region "Constants"
    Private Const MAX_MATCHINGELEMENTS = 10
    Public Const DEFAULT_RESULTS_TO_FIND = 1000
    Public Const MAXIMUM_ALLOWED_RESULTS_TO_FIND = 1000000

    Public Const MAX_BOUNDED_SEARCH_COUNT = 65565
#End Region

#Region "Strutures and Enums"

    ''' <summary>
    ''' Search tolerances for each element
    ''' </summary>
    ''' <remarks>
    ''' Target percent composition values must be between 0 and 100; they are only used when calling FindMatchesByPercentComposition
    ''' MinimumCount and MaximumCount are only used when the search mode is Bounded; they are ignored for Thorough search
    ''' </remarks>
    Public Structure udtCandidateElementTolerances
        Public TargetPercentComposition As Double
        Public MinimumCount As Integer
        Public MaximumCount As Integer
    End Structure

    Private Structure udtElementNumType
        Public H As Integer
        Public C As Integer
        Public Si As Integer
        Public N As Integer
        Public P As Integer
        Public O As Integer
        Public S As Integer
        Public Cl As Integer
        Public I As Integer
        Public F As Integer
        Public Br As Integer
        Public Other As Integer
    End Structure

    Private Structure udtBoundedSearchRangeType
        Public Min As Integer
        Public Max As Integer
    End Structure

    Private Enum eCalculationMode
        MatchMolecularWeight = 0
        MatchPercentComposition = 1
    End Enum

#End Region

#Region "Member Variables"

    Private mAbortProcessing As Boolean
    Private mCalculating As Boolean
    Private mErrorMessage As String

    ''' <summary>
    ''' Keys are element symbols, abbreviations, or even simply a mass value
    ''' Values are target percent composition values, between 0 and 100
    ''' </summary>
    ''' <remarks>The target percent composition values are only used when FindMatchesByPercentComposition is called</remarks>
    Private mCandidateElements As Dictionary(Of String, udtCandidateElementTolerances)

    Private ReadOnly mElementAndMassRoutines As MWElementAndMassRoutines

    Private mMaximumHits As Integer

    Private mRecursiveCount As Integer
    Private mRecursiveFunctionCallCount As Integer
    Private mMaxRecursiveCount As Integer

    ''' <summary>
    ''' Percent complete, between 0 and 100
    ''' </summary>
    ''' <remarks></remarks>
    Private mPercentComplete As Double

#End Region

#Region "Properties"

    ''' <summary>
    ''' Element symbols to consider when finding empirical formulas
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>The values in the dictionary are target percent composition values; only used if you call FindMatchesByPercentComposition</remarks>
    Public Property CandidateElements As Dictionary(Of String, udtCandidateElementTolerances)
        Get
            Return mCandidateElements
        End Get
        Set(value As Dictionary(Of String, udtCandidateElementTolerances))
            If Not value Is Nothing Then
                mCandidateElements = value

                ValidateBoundedSearchValues()
                ValidatePercentCompositionValues()
            End If
        End Set
    End Property

    Public Property EchoMessagesToConsole As Boolean

    Public Property MaximumHits As Integer
        Get
            Return mMaximumHits
        End Get
        Set(value As Integer)
            If value < 1 Then
                value = 1
            End If

            If value > MAXIMUM_ALLOWED_RESULTS_TO_FIND Then
                value = MAXIMUM_ALLOWED_RESULTS_TO_FIND
            End If

            mMaximumHits = value
        End Set
    End Property

    ''' <summary>
    ''' Percent complete, between 0 and 100
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly Property PercentComplete As Double
        Get
            Return mPercentComplete
        End Get
    End Property

#End Region

#Region "Events"
    Public Event MessageEvent(strMessage As String)
    Public Event ErrorEvent(strErrorMessage As String)
    Public Event WarningEvent(strWarningMessage As String)
#End Region

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(oMWElementAndMassRoutines As MWElementAndMassRoutines)

        mElementAndMassRoutines = oMWElementAndMassRoutines
        mCandidateElements = New Dictionary(Of String, udtCandidateElementTolerances)

        EchoMessagesToConsole = True

        Reset()

    End Sub

#Region "Public Methods"

    ''' <summary>
    ''' Abort processing
    ''' </summary>
    ''' <remarks>Only useful if the formula finder is running on a separate thread from the calling program</remarks>
    Public Sub AbortProcessingNow()
        mAbortProcessing = True
    End Sub

    ''' <summary>
    ''' Add a candidate element, abbreviation, or monoisotopic mass
    ''' </summary>
    ''' <param name="elementSymbolAbbrevOrMass">Element symbol, abbreviation symbol, or monoisotopic mass</param>
    ''' <remarks></remarks>
    Public Sub AddCandidateElement(elementSymbolAbbrevOrMass As String)

        Dim udtElementTolerances = GetDefaultCandidateElementTolerance()

        AddCandidateElement(elementSymbolAbbrevOrMass, udtElementTolerances)
    End Sub

    ''' <summary>
    ''' Add a candidate element, abbreviation, or monoisotopic mass
    ''' </summary>
    ''' <param name="elementSymbolAbbrevOrMass">Element symbol, abbreviation symbol, or monoisotopic mass</param>
    ''' <param name="targetPercentComposition">Target percent composition</param>
    ''' <remarks></remarks>
    Public Sub AddCandidateElement(elementSymbolAbbrevOrMass As String, targetPercentComposition As Double)
        Dim udtElementTolerances = GetDefaultCandidateElementTolerance(targetPercentComposition)
        AddCandidateElement(elementSymbolAbbrevOrMass, udtElementTolerances)
    End Sub

    ''' <summary>
    ''' Add a candidate element, abbreviation, or monoisotopic mass
    ''' </summary>
    ''' <param name="elementSymbolAbbrevOrMass">Element symbol, abbreviation symbol, or monoisotopic mass</param>
    ''' <param name="minimumCount">Minimum occurrence count</param>
    ''' <param name="maximumCount">Maximum occurrence count</param>
    ''' <remarks>This method should be used when defining elements for a bounded search</remarks>
    Public Sub AddCandidateElement(elementSymbolAbbrevOrMass As String, minimumCount As Integer, maximumCount As Integer)
        Dim udtElementTolerances = GetDefaultCandidateElementTolerance(minimumCount, maximumCount)
        AddCandidateElement(elementSymbolAbbrevOrMass, udtElementTolerances)
    End Sub

    ''' <summary>
    ''' Add a candidate element, abbreviation, or monoisotopic mass
    ''' </summary>
    ''' <param name="elementSymbolAbbrevOrMass">Element symbol, abbreviation symbol, or monoisotopic mass</param>
    ''' <param name="udtElementTolerances">Search tolerances, including % composition range and Min/Max count when using a bounded search</param>
    ''' <remarks></remarks>
    Public Sub AddCandidateElement(elementSymbolAbbrevOrMass As String, udtElementTolerances As udtCandidateElementTolerances)

        If mCandidateElements.ContainsKey(elementSymbolAbbrevOrMass) Then
            mCandidateElements(elementSymbolAbbrevOrMass) = udtElementTolerances
        Else
            mCandidateElements.Add(elementSymbolAbbrevOrMass, udtElementTolerances)
        End If
    End Sub

    ''' <summary>
    ''' Find empirical formulas that match the given target mass, with the given ppm tolerance
    ''' </summary>
    ''' <param name="targetMass"></param>
    ''' <param name="massTolerancePPM"></param>
    ''' <returns></returns>
    ''' <remarks>Uses default search options</remarks>
    Public Function FindMatchesByMassPPM(targetMass As Double, massTolerancePPM As Double) As List(Of clsFormulaFinderResult)

        Dim lstResults = FindMatchesByMassPPM(targetMass, massTolerancePPM, Nothing)

        ' No need to sort because FindMatchesByMassPPM has already done so
        Return lstResults

    End Function

    ''' <summary>
    ''' Find empirical formulas that match the given target mass, with the given ppm tolerance
    ''' </summary>
    ''' <param name="targetMass"></param>
    ''' <param name="massTolerancePPM"></param>
    ''' <param name="searchOptions"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FindMatchesByMassPPM(targetMass As Double, massTolerancePPM As Double, searchOptions As clsFormulaFinderOptions) As List(Of clsFormulaFinderResult)
        Dim massToleranceDa = massTolerancePPM * targetMass / 1000000.0
        If searchOptions Is Nothing Then searchOptions = New clsFormulaFinderOptions()

        Dim lstResults = FindMatchesByMass(targetMass, massToleranceDa, searchOptions, True)

        Dim sortedResults = (From item In lstResults Order By item.SortKey Select item).ToList()
        Return sortedResults

    End Function

    ''' <summary>
    ''' Find empirical formulas that match the given target mass, with the given tolerance
    ''' </summary>
    ''' <param name="targetMass"></param>
    ''' <param name="massToleranceDa"></param>
    ''' <returns></returns>
    ''' <remarks>Uses default search options</remarks>
    Public Function FindMatchesByMass(targetMass As Double, massToleranceDa As Double) As List(Of clsFormulaFinderResult)
        Dim lstResults = FindMatchesByMass(targetMass, massToleranceDa, Nothing)

        ' No need to sort because FindMatchesByMassPPM has already done so
        Return lstResults

    End Function

    ''' <summary>
    ''' Find empirical formulas that match the given target mass, with the given tolerance
    ''' </summary>
    ''' <param name="targetMass"></param>
    ''' <param name="massToleranceDa"></param>
    ''' <param name="searchOptions"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FindMatchesByMass(targetMass As Double, massToleranceDa As Double, searchOptions As clsFormulaFinderOptions) As List(Of clsFormulaFinderResult)
        If searchOptions Is Nothing Then searchOptions = New clsFormulaFinderOptions()

        Dim lstResults = FindMatchesByMass(targetMass, massToleranceDa, searchOptions, False)

        Dim sortedResults = (From item In lstResults Order By item.SortKey Select item).ToList()
        Return sortedResults

    End Function

    Public Function FindMatchesByPercentComposition(
     maximumFormulaMass As Double,
     percentTolerance As Double,
     searchOptions As clsFormulaFinderOptions) As List(Of clsFormulaFinderResult)

        If searchOptions Is Nothing Then searchOptions = New clsFormulaFinderOptions()

        Dim lstResults = FindMatchesByPercentCompositionWork(maximumFormulaMass, percentTolerance, searchOptions)

        Dim sortedResults = (From item In lstResults Order By item.SortKey Select item).ToList()
        Return sortedResults

    End Function

    ''' <summary>
    ''' Reset to defaults
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Reset()

        mCandidateElements.Clear()
        mCandidateElements.Add("C", GetDefaultCandidateElementTolerance(70))
        mCandidateElements.Add("H", GetDefaultCandidateElementTolerance(10))
        mCandidateElements.Add("N", GetDefaultCandidateElementTolerance(10))
        mCandidateElements.Add("O", GetDefaultCandidateElementTolerance(10))

        mErrorMessage = String.Empty
        mAbortProcessing = False

        MaximumHits = DEFAULT_RESULTS_TO_FIND
        
    End Sub

#End Region

    Private Sub AppendToEmpiricalFormula(sbEmpiricalFormula As StringBuilder, elementSymbol As String, elementCount As Integer)
        If elementCount <> 0 Then
            sbEmpiricalFormula.Append(elementSymbol)

            If elementCount > 1 Then
                sbEmpiricalFormula.Append(elementCount)
            End If
        End If

    End Sub

    Private Sub AppendPercentCompositionResult(
       searchResult As clsFormulaFinderResult,
       elementcount As Integer,
       sortedElementStats As IList(Of clsFormulaFinderCandidateElement),
       targetIndex As Integer,
       percentComposition As Double)

        If elementcount <> 0 AndAlso targetIndex < sortedElementStats.Count Then
            searchResult.PercentComposition.Add(sortedElementStats(targetIndex).Symbol, percentComposition)
        End If

    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="targetMass">Only used when calculationMode is MatchMolecularWeight</param>
    ''' <param name="massToleranceDa">Only used when calculationMode is MatchMolecularWeigh</param>
    ''' <param name="maximumFormulaMass">Only used when calculationMode is MatchPercentComposition</param>
    ''' <param name="searchOptions"></param>
    ''' <param name="ppmMode"></param>
    ''' <param name="calculationMode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function BoundedSearch(
       targetMass As Double,
       massToleranceDa As Double,
       maximumFormulaMass As Double,
       searchOptions As clsFormulaFinderOptions,
       ppmMode As Boolean,
       calculationMode As eCalculationMode,
       sortedElementStats As List(Of clsFormulaFinderCandidateElement)) As List(Of clsFormulaFinderResult)

        Dim lstResults As List(Of clsFormulaFinderResult)

        If searchOptions.FindTargetMZ Then
            ' Searching for target m/z rather than target mass

            Dim mzSearchChargeMin As Integer
            Dim mzSearchChargeMax As Integer

            MultipleSearchMath(sortedElementStats.Count, searchOptions, mzSearchChargeMin, mzSearchChargeMax)

            lstResults = OldFormulaFinder(searchOptions, ppmMode, calculationMode, sortedElementStats, targetMass, massToleranceDa, maximumFormulaMass)
        Else
            searchOptions.ChargeMin = 1
            searchOptions.ChargeMax = 1

            lstResults = OldFormulaFinder(searchOptions, ppmMode, calculationMode, sortedElementStats, targetMass, massToleranceDa, maximumFormulaMass)
        End If

        ComputeSortKeys(lstResults)

        Return lstResults

    End Function

    Private Sub ComputeSortKeys(lstResults As IEnumerable(Of clsFormulaFinderResult))

        ' Compute the sort key for each result
        Dim sbCodeString = New StringBuilder()

        For Each item In lstResults
            item.SortKey = ComputeSortKey(sbCodeString, item.EmpiricalFormula)
        Next

    End Sub

    Private Function ComputeSortKey(sbCodeString As StringBuilder, empiricalFormula As String) As String

        ' Precedence order for sbCodeString
        '  C1_ C2_ C3_ C4_ C5_ C6_ C7_ C8_ C9_  a   z    1,  2,  3...
        '   1   2   3   4   5   6   7   8   9   10  35   36  37  38
        '
        ' Custom elements are converted to Chr(1), Chr(2), etc.
        ' Letters are converted to Chr(10) through Chr(35)
        ' Number are converted to Chr(36) through Chr(255)
        '
        ' 220 = Chr(0) + Chr(220+35) = Chr(0) + Chr(255)

        ' 221 = Chr(CInt(Math.Floor(221+34/255))) + Chr((221 + 34) Mod 255 + 1)

        Dim charIndex = 0
        Dim formulaLength = empiricalFormula.Length
        Dim parsedValue As Integer

        sbCodeString.Clear()

        While charIndex < formulaLength
            Dim strCurrentLetter = Char.ToUpper(empiricalFormula(charIndex))

            If (Char.IsLetter(strCurrentLetter)) Then

                sbCodeString.Append(Chr(0))

                If charIndex + 2 < formulaLength AndAlso empiricalFormula.Substring(charIndex + 2, 1) = "_" Then
                    ' At a custom element, which are notated as "C1_", "C2_", etc.
                    ' Give it a value of Chr(1) through Chr(10)
                    ' Also, need to bump up charIndex by 2

                    Dim customElementNum = empiricalFormula.Substring(charIndex + 1, 1)

                    If Integer.TryParse(customElementNum, parsedValue) Then
                        sbCodeString.Append(Chr(parsedValue))
                    Else
                        sbCodeString.Append(Chr(1))
                    End If

                    charIndex += 2
                Else
                    ' 65 is the ascii code for the letter a
                    ' Thus, 65-55 = 10
                    Dim asciiValue = Asc(strCurrentLetter)
                    sbCodeString.Append(Chr(asciiValue - 55))

                End If
            ElseIf strCurrentLetter <> "_" Then
                ' At a number, since empirical formulas can only have letters or numbers or underscores

                Dim endIndex = charIndex
                While endIndex + 1 < formulaLength
                    Dim nextChar = empiricalFormula(endIndex + 1)
                    If Not Integer.TryParse(nextChar, parsedValue) Then
                        Exit While
                    End If
                    endIndex += 1
                End While

                If Integer.TryParse(empiricalFormula.Substring(charIndex, endIndex - charIndex + 1), parsedValue) Then
                    If parsedValue < 221 Then
                        sbCodeString.Append(Chr(0))
                        sbCodeString.Append(Chr(parsedValue + 35))
                    Else
                        sbCodeString.Append(Chr(CInt(Math.Floor(parsedValue + 34 / 255))))
                        sbCodeString.Append(Chr((parsedValue + 34) Mod 255 + 1))
                    End If
                End If

                charIndex = endIndex
            End If

            charIndex += 1

        End While

        Return sbCodeString.ToString()

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="totalMass"></param>
    ''' <param name="totalCharge"></param>
    ''' <param name="targetMass"></param>
    ''' <param name="massToleranceDa"></param>
    ''' <param name="intMultipleMtoZCharge"></param>
    ''' <remarks>True if the m/z is within tolerance of the target</remarks>
    Private Function CheckMtoZWithTarget(
       totalMass As Double,
       totalCharge As Double,
       targetMass As Double,
       massToleranceDa As Double,
       intMultipleMtoZCharge As Integer) As Boolean

        Dim dblMtoZ As Double, dblOriginalMtoZ As Double

        ' The original target is the target m/z; assure this compound has that m/z
        If Math.Abs(totalCharge) > 0.5 Then
            dblMtoZ = Math.Abs(totalMass / totalCharge)
        Else
            dblMtoZ = 0
        End If

        If intMultipleMtoZCharge = 0 Then
            Return False
        End If

        dblOriginalMtoZ = targetMass / intMultipleMtoZCharge
        If dblMtoZ < dblOriginalMtoZ - massToleranceDa Or dblMtoZ > dblOriginalMtoZ + massToleranceDa Then
            ' dblMtoZ is not within tolerance of dblOriginalMtoZ, so don't report the result
            Return False
        End If

        Return True

    End Function

    Private Function Combinatorial(a As Integer, b As Integer) As Double
        If a > 170 Or b > 170 Then
            Console.WriteLine("Cannot compute factorial of a number over 170.  Thus, cannot compute the combination.")
            Return -1
        ElseIf a < b Then
            Console.WriteLine("First number should be greater than or equal to the second number")
            Return -1
        Else
            Return Factorial(a) / (Factorial(b) * Factorial(a - b))
        End If
    End Function

    ''' <summary>
    ''' Construct the empirical formula and verify hydrogens
    ''' </summary>
    ''' <param name="searchOptions"></param>
    ''' <param name="sbEmpiricalFormula"></param>
    ''' <param name="count1"></param>
    ''' <param name="count2"></param>
    ''' <param name="count3"></param>
    ''' <param name="count4"></param>
    ''' <param name="count5"></param>
    ''' <param name="count6"></param>
    ''' <param name="count7"></param>
    ''' <param name="count8"></param>
    ''' <param name="count9"></param>
    ''' <param name="count10"></param>
    ''' <param name="totalMass"></param>
    ''' <param name="targetMass">Only used when searchOptions.FindTargetMZ is true, and that is only valid when matching a target mass, not when matching percent composition values</param>
    ''' <param name="massToleranceDa">Only used when searchOptions.FindTargetMZ is true</param>
    ''' <param name="totalCharge"></param>
    ''' <param name="intMultipleMtoZCharge">When searchOptions.FindTargetMZ is false, this will be 1; otherwise, the current charge being searched for</param>
    ''' <returns>False if compound has too many hydrogens AND hydrogen checking is on, otherwise returns true</returns>
    ''' <remarks>Common function to both molecular weight and percent composition matching</remarks>
    Private Function ConstructAndVerifyCompound(
       searchOptions As clsFormulaFinderOptions,
       sbEmpiricalFormula As StringBuilder,
       count1 As Integer, count2 As Integer, count3 As Integer, count4 As Integer, count5 As Integer, count6 As Integer, count7 As Integer, count8 As Integer, count9 As Integer, count10 As Integer,
       sortedElementStats As IList(Of clsFormulaFinderCandidateElement),
       totalMass As Double,
       targetMass As Double,
       massToleranceDa As Double,
       totalCharge As Double,
       intMultipleMtoZCharge As Integer,
       <Out> ByRef empiricalResultSymbols As Dictionary(Of String, Integer),
       <Out> ByRef correctedCharge As Double) As Boolean

        ' This dictionary tracks the elements and abbreviations of the found formula so that they can be properly ordered according to empirical formula conventions
        ' Key is the element or abbreviation symbol, value is the number of each element or abbreviation
        empiricalResultSymbols = New Dictionary(Of String, Integer)

        sbEmpiricalFormula.Clear()

        Try

            ' Convert to empirical formula and sort
            ConstructAndVerifyAddIfValid(sortedElementStats, empiricalResultSymbols, 0, count1)
            ConstructAndVerifyAddIfValid(sortedElementStats, empiricalResultSymbols, 1, count2)
            ConstructAndVerifyAddIfValid(sortedElementStats, empiricalResultSymbols, 2, count3)
            ConstructAndVerifyAddIfValid(sortedElementStats, empiricalResultSymbols, 3, count4)
            ConstructAndVerifyAddIfValid(sortedElementStats, empiricalResultSymbols, 4, count5)
            ConstructAndVerifyAddIfValid(sortedElementStats, empiricalResultSymbols, 5, count6)
            ConstructAndVerifyAddIfValid(sortedElementStats, empiricalResultSymbols, 6, count7)
            ConstructAndVerifyAddIfValid(sortedElementStats, empiricalResultSymbols, 7, count8)
            ConstructAndVerifyAddIfValid(sortedElementStats, empiricalResultSymbols, 8, count9)
            ConstructAndVerifyAddIfValid(sortedElementStats, empiricalResultSymbols, 9, count10)

            Dim valid = ConstructAndVerifyCompoundWork(searchOptions,
                                                       sbEmpiricalFormula,
                                                       totalMass, targetMass, massToleranceDa,
                                                       totalCharge, intMultipleMtoZCharge,
                                                       empiricalResultSymbols, correctedCharge)

            Return valid

        Catch ex As Exception
            mElementAndMassRoutines.GeneralErrorHandler("ConstructAndVerifyCompound", 0, ex.Message)
            correctedCharge = totalCharge
            Return False
        End Try

    End Function

    Private Sub ConstructAndVerifyAddIfValid(
       sortedElementStats As IList(Of clsFormulaFinderCandidateElement),
       empiricalResultSymbols As IDictionary(Of String, Integer),
       targetElementIndex As Integer,
       currentCount As Integer)

        If currentCount <> 0 AndAlso targetElementIndex < sortedElementStats.Count Then
            empiricalResultSymbols.Add(sortedElementStats(targetElementIndex).Symbol, currentCount)
        End If
    End Sub

    ''' <summary>
    ''' Construct the empirical formula and verify hydrogens
    ''' </summary>
    ''' <param name="searchOptions"></param>
    ''' <param name="sbEmpiricalFormula"></param>
    ''' <param name="lstPotentialElementPointers"></param>
    ''' <param name="totalMass"></param>
    ''' <param name="targetMass">Only used when searchOptions.FindTargetMZ is true, and that is only valid when matching a target mass, not when matching percent composition values</param>
    ''' <param name="massToleranceDa">Only used when searchOptions.FindTargetMZ is true</param>
    ''' <param name="totalCharge"></param>
    ''' <param name="intMultipleMtoZCharge">When searchOptions.FindTargetMZ is false, this will be 0; otherwise, the current charge being searched for</param>
    ''' <returns>False if compound has too many hydrogens AND hydrogen checking is on, otherwise returns true</returns>
    ''' <remarks>Common function to both molecular weight and percent composition matching</remarks>
    Private Function ConstructAndVerifyCompoundRecursive(
       searchOptions As clsFormulaFinderOptions,
       sbEmpiricalFormula As StringBuilder,
       sortedElementStats As IList(Of clsFormulaFinderCandidateElement),
       lstPotentialElementPointers As IEnumerable(Of Integer),
       totalMass As Double,
       targetMass As Double,
       massToleranceDa As Double,
       totalCharge As Double,
       intMultipleMtoZCharge As Integer,
       <Out> ByRef empiricalResultSymbols As Dictionary(Of String, Integer),
       <Out> ByRef correctedCharge As Double) As Boolean

        sbEmpiricalFormula.Clear()

        Try
            ' The empiricalResultSymbols dictionary tracks the elements and abbreviations of the found formula 
            ' so that they can be properly ordered according to empirical formula conventions
            ' Keys are the element or abbreviation symbol, value is the number of each element or abbreviation
            empiricalResultSymbols = ConvertElementPointersToElementStats(sortedElementStats, lstPotentialElementPointers)

            Dim valid = ConstructAndVerifyCompoundWork(searchOptions,
                                                       sbEmpiricalFormula,
                                                       totalMass, targetMass, massToleranceDa,
                                                       totalCharge, intMultipleMtoZCharge,
                                                       empiricalResultSymbols, correctedCharge)

            ' Uncomment to debug
            'Dim computedMass = mElementAndMassRoutines.ComputeFormulaWeight(sbEmpiricalFormula.ToString())
            'If Math.Abs(computedMass - totalMass) > massToleranceDa Then
            '    Console.WriteLine("Wrong result: " & sbEmpiricalFormula.ToString())
            'End If

            Return valid

        Catch ex As Exception
            mElementAndMassRoutines.GeneralErrorHandler("ConstructAndVerifyCompoundRecursive", 0, ex.Message)
            empiricalResultSymbols = New Dictionary(Of String, Integer)
            correctedCharge = totalCharge
            Return False
        End Try

    End Function

    Private Function GetElementCountArray(
       potentialElementCount As Integer,
       lstNewPotentialElementPointers As IEnumerable(Of Integer)) As Integer()

        ' Store the occurrence count of each element
        Dim elementCountArray(potentialElementCount - 1) As Integer

        For Each elementIndex In lstNewPotentialElementPointers
            elementCountArray(elementIndex) += 1
        Next

        Return elementCountArray

    End Function

    Private Function ConstructAndVerifyCompoundWork(
       searchOptions As clsFormulaFinderOptions,
       sbEmpiricalFormula As StringBuilder,
       totalMass As Double,
       targetMass As Double,
       massToleranceDa As Double,
       totalCharge As Double,
       intMultipleMtoZCharge As Integer,
       empiricalResultSymbols As Dictionary(Of String, Integer),
       <Out> ByRef correctedCharge As Double) As Boolean

        correctedCharge = totalCharge

        ' Convert to a formatted empirical formula (elements order by C, H, then alphabetical)

        Dim matchCount As Integer

        ' First find C
        If empiricalResultSymbols.TryGetValue("C", matchCount) Then
            sbEmpiricalFormula.Append("C")
            If matchCount > 1 Then sbEmpiricalFormula.Append(matchCount)
        End If

        ' Next find H
        If empiricalResultSymbols.TryGetValue("H", matchCount) Then
            sbEmpiricalFormula.Append("H")
            If matchCount > 1 Then sbEmpiricalFormula.Append(matchCount)
        End If

        Dim query = From item In empiricalResultSymbols Where item.Key <> "C" And item.Key <> "H" Order By item.Key Select item

        For Each result In query
            sbEmpiricalFormula.Append(result.Key)
            If result.Value > 1 Then sbEmpiricalFormula.Append(result.Value)
        Next

        If Not searchOptions.VerifyHydrogens And Not searchOptions.FindTargetMZ Then
            Return True
        End If

        ' Verify that the formula does not have too many hydrogens

        ' Counters for elements of interest (hydrogen, carbon, silicon, nitrogen, phosphorus, chlorine, iodine, flourine, bromine, and other)
        Dim udtElementNum As udtElementNumType

        ' Determine number of C, Si, N, P, O, S, Cl, I, F, Br and H atoms
        For Each item In empiricalResultSymbols
            Select Case item.Key
                Case "C" : udtElementNum.C = udtElementNum.C + item.Value
                Case "Si" : udtElementNum.Si = udtElementNum.Si + item.Value
                Case "N" : udtElementNum.N = udtElementNum.N + item.Value
                Case "P" : udtElementNum.P = udtElementNum.P + item.Value
                Case "O" : udtElementNum.O = udtElementNum.O + item.Value
                Case "S" : udtElementNum.S = udtElementNum.S + item.Value
                Case "Cl" : udtElementNum.Cl = udtElementNum.Cl + item.Value
                Case "I" : udtElementNum.I = udtElementNum.I + item.Value
                Case "F" : udtElementNum.F = udtElementNum.F + item.Value
                Case "Br" : udtElementNum.Br = udtElementNum.Br + item.Value
                Case "H" : udtElementNum.H = udtElementNum.H + item.Value
                Case Else : udtElementNum.Other = udtElementNum.Other + item.Value
            End Select
        Next

        Dim maxH As Integer = 0

        ' Compute maximum number of hydrogens
        If udtElementNum.Si = 0 AndAlso udtElementNum.C = 0 AndAlso udtElementNum.N = 0 AndAlso
           udtElementNum.P = 0 AndAlso udtElementNum.Other = 0 AndAlso
           (udtElementNum.O > 0 OrElse udtElementNum.S > 0) Then
            ' Only O and S
            maxH = 3
        Else
            ' Formula is: [#C*2 + 3 - (2 if N or P present)] + [#N + 3 - (1 if C or Si present)] + [#other elements * 4 + 3], where we assume other elements can have a coordination Number of up to 7
            If udtElementNum.C > 0 Or udtElementNum.Si > 0 Then
                maxH += (udtElementNum.C + udtElementNum.Si) * 2 + 3
                ' If udtElementNum.N > 0 Or udtElementNum.P > 0 Then maxh = maxh - 2
            End If

            If udtElementNum.N > 0 Or udtElementNum.P > 0 Then
                maxH += (udtElementNum.N + udtElementNum.P) + 3
                ' If udtElementNum.C > 0 Or udtElementNum.Si > 0 Then maxh = maxh - 1
            End If

            ' Correction for carbon contribution
            'If (udtElementNum.C > 0 Or udtElementNum.Si > 0) And (udtElementNum.N > 0 Or udtElementNum.P > 0) Then udtElementNum.H = udtElementNum.H - 2

            ' Correction for nitrogen contribution
            'If (udtElementNum.N > 0 Or udtElementNum.P > 0) And (udtElementNum.C > 0 Or udtElementNum.Si > 0) Then udtElementNum.H = udtElementNum.H - 1

            ' Combine the above two commented out if's to obtain:
            If (udtElementNum.N > 0 Or udtElementNum.P > 0) And (udtElementNum.C > 0 Or udtElementNum.Si > 0) Then
                maxH = maxH - 3
            End If

            If udtElementNum.Other > 0 Then maxH += udtElementNum.Other * 4 + 3

        End If

        ' correct for if H only
        If maxH < 3 Then maxH = 3

        ' correct for halogens
        maxH = maxH - udtElementNum.F - udtElementNum.Cl - udtElementNum.Br - udtElementNum.I

        ' correct for negative udtElementNum.H
        If maxH < 0 Then maxH = 0

        ' Verify H's
        Dim blnHOK = (udtElementNum.H <= maxH)

        ' Only proceed if hydrogens check out
        If Not blnHOK Then
            Return False
        End If

        Dim chargeOK As Boolean

        ' See if totalCharge is within charge limits (chargeOK will be set to True or False by CorrectChargeEmpirical)
        If searchOptions.FindCharge Then
            correctedCharge = CorrectChargeEmpirical(searchOptions, totalCharge, udtElementNum, chargeOK)
        Else
            chargeOK = True
        End If

        ' If charge is within range and checking for multiples, see if correct m/z too
        If chargeOK AndAlso searchOptions.FindTargetMZ Then
            chargeOK = CheckMtoZWithTarget(totalMass, correctedCharge, targetMass,
                                              massToleranceDa, intMultipleMtoZCharge)
        End If

        Return chargeOK

    End Function

    Private Function ConvertElementPointersToElementStats(
       sortedElementStats As IList(Of clsFormulaFinderCandidateElement),
       lstPotentialElementPointers As IEnumerable(Of Integer)) As Dictionary(Of String, Integer)

        ' This dictionary tracks the elements and abbreviations of the found formula so that they can be properly ordered according to empirical formula conventions
        ' Key is the element or abbreviation symbol, value is the number of each element or abbreviation
        Dim empiricalResultSymbols = New Dictionary(Of String, Integer)

        Dim elementCountArray = GetElementCountArray(sortedElementStats.Count, lstPotentialElementPointers)

        For intIndex = 0 To sortedElementStats.Count - 1
            If elementCountArray(intIndex) <> 0 Then
                empiricalResultSymbols.Add(sortedElementStats(intIndex).Symbol, elementCountArray(intIndex))
            End If
        Next

        Return empiricalResultSymbols

    End Function

    ''' <summary>
    ''' Correct charge using rules for an empirical formula
    ''' </summary>
    ''' <param name="searchOptions"></param>
    ''' <param name="totalCharge"></param>
    ''' <param name="udtElementNum"></param>
    ''' <param name="chargeOK"></param>
    ''' <returns>Corrected charge</returns>
    ''' <remarks></remarks>
    Private Function CorrectChargeEmpirical(
       searchOptions As clsFormulaFinderOptions,
       totalCharge As Double,
       udtElementNum As udtElementNumType,
       <Out> ByRef chargeOK As Boolean) As Double

        Dim correctedCharge = totalCharge

        If udtElementNum.C + udtElementNum.Si >= 1 Then
            If udtElementNum.H > 0 And Math.Abs(mElementAndMassRoutines.GetElementStatInternal(1, esElementStatsConstants.esCharge) - 1) < Single.Epsilon Then
                ' Since carbon or silicon are present, assume the hydrogens should be negative
                ' Subtract udtElementNum.H * 2 since hydrogen is assigned a +1 charge if ElementStats(1).Charge = 1
                correctedCharge -= udtElementNum.H * 2
            End If

            ' Correct for udtElementNumber of C and Si
            If udtElementNum.C + udtElementNum.Si > 1 Then
                correctedCharge -= (udtElementNum.C + udtElementNum.Si - 1) * 2
            End If
        End If

        If udtElementNum.N + udtElementNum.P > 0 And udtElementNum.C > 0 Then
            ' Assume 2 hydrogens around each Nitrogen or Phosphorus, thus add back +2 for each H
            ' First, decrease udtElementNumber of halogens by udtElementNumber of hydrogens & halogens taken up by the carbons
            ' Determine # of H taken up by all the carbons in a compound without N or P, then add back 1 H for each N and P
            Dim intNumHalogens = udtElementNum.H + udtElementNum.F + udtElementNum.Cl + udtElementNum.Br + udtElementNum.I
            intNumHalogens = intNumHalogens - (udtElementNum.C * 2 + 2) + udtElementNum.N + udtElementNum.P

            If intNumHalogens >= 0 Then
                For intIndex = 1 To udtElementNum.N + udtElementNum.P
                    correctedCharge += 2
                    intNumHalogens -= 1

                    If intNumHalogens <= 0 Then
                        Exit For
                    Else
                        correctedCharge += 2
                        intNumHalogens -= 1
                        If intNumHalogens <= 0 Then Exit For
                    End If

                Next
            End If
        End If

        If searchOptions.LimitChargeRange Then
            ' Make sure correctedCharge is within the specified range
            If correctedCharge >= searchOptions.ChargeMin AndAlso
               correctedCharge <= searchOptions.ChargeMax Then
                ' Charge is within range
                chargeOK = True
            Else
                chargeOK = False
            End If
        Else
            chargeOK = True
        End If

        Return correctedCharge

    End Function

    ''' <summary>
    ''' Search empiricalResultSymbols for the elements in targetCountStats 
    ''' </summary>
    ''' <param name="empiricalResultSymbols"></param>
    ''' <param name="targetCountStats"></param>
    ''' <returns>True if all of the elements are present in the given counts (extra elements may also be present), 
    ''' false one or more is not found or has the wrong occurrence count</returns>
    ''' <remarks></remarks>
    Private Function EmpiricalFormulaHasElementCounts(
      empiricalResultSymbols As IDictionary(Of String, Integer),
      targetCountStats As Dictionary(Of String, Integer)) As Boolean

        Dim empiricalElementCount As Integer

        For Each targetElement In targetCountStats
            If Not empiricalResultSymbols.TryGetValue(targetElement.Key, empiricalElementCount) Then
                Return False
            End If

            If Not empiricalElementCount = targetElement.Value Then
                Return False
            End If
        Next

        Return True

    End Function

    Private Sub EstimateNumberOfOperations(potentialElementCount As Integer, Optional multipleSearchMax As Integer = 0)

        ' Estimate the number of operations that will be performed
        mRecursiveCount = 0
        mRecursiveFunctionCallCount = 0

        If potentialElementCount = 1 Then
            mMaxRecursiveCount = 1
            Exit Sub
        End If

        Const NUM_POINTERS As Integer = 3

        ' Calculate lngMaxRecursiveCount based on a combination function
        Dim maxRecursiveCount = Combinatorial(NUM_POINTERS + potentialElementCount, potentialElementCount - 1) - Combinatorial(potentialElementCount + NUM_POINTERS - 2, NUM_POINTERS - 1)
        If maxRecursiveCount > Integer.MaxValue Then
            mMaxRecursiveCount = Integer.MaxValue
        Else
            mMaxRecursiveCount = CInt(maxRecursiveCount)
        End If

        If multipleSearchMax > 0 Then
            ' Correct lngMaxRecursiveCount for searching for m/z values
            mMaxRecursiveCount = mMaxRecursiveCount * multipleSearchMax
        End If

    End Sub

    ''' <summary>
    ''' Compute the factorial of a number; uses recursion
    ''' </summary>
    ''' <param name="value">Integer between 0 and 170</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function Factorial(value As Integer) As Double

        If value > 170 Then
            Throw New Exception("Cannot compute factorial of a number over 170")
        End If

        If value < 0 Then
            Throw New Exception("Cannot compute factorial of a negative number")
        End If

        If value = 0 Then
            Return 1
        Else
            Return value * Factorial(value - 1)
        End If

    End Function

    Private Function FindMatchesByMass(
       targetMass As Double,
       massToleranceDa As Double,
       searchOptions As clsFormulaFinderOptions,
       ppmMode As Boolean) As List(Of clsFormulaFinderResult)

        ' Validate the Inputs
        If Not ValidateSettings(eCalculationMode.MatchMolecularWeight) Then
            Return New List(Of clsFormulaFinderResult)
        End If

        If Val(targetMass) <= 0 Then
            ReportError("Target molecular weight must be greater than 0")
            Return New List(Of clsFormulaFinderResult)
        End If

        If massToleranceDa < 0 Then
            ReportError("Mass tolerance cannot be negative")
            Return New List(Of clsFormulaFinderResult)
        End If

        Dim candidateElementsStats = GetCandidateElements()

        If candidateElementsStats.Count = 0 Then
            Return New List(Of clsFormulaFinderResult)
        End If

        Dim sortedElementStats = (From item In candidateElementsStats Order By item.Mass Descending Select item).ToList()

        If searchOptions.SearchMode = clsFormulaFinderOptions.eSearchMode.Thorough Then
            ' Thorough search

            EstimateNumberOfOperations(sortedElementStats.Count)

            ' Pointers to the potential elements
            Dim lstPotentialElementPointers = New List(Of Integer)

            Dim lstResults = New List(Of clsFormulaFinderResult)

            If searchOptions.FindTargetMZ Then
                ' Searching for target m/z rather than target mass

                Dim mzSearchChargeMin As Integer
                Dim mzSearchChargeMax As Integer

                MultipleSearchMath(sortedElementStats.Count, searchOptions, mzSearchChargeMin, mzSearchChargeMax)

                For currentMzCharge = mzSearchChargeMin To mzSearchChargeMax
                    ' Call the RecursiveMWfinder repeatedly, sending dblTargetWeight * x each time to search for target, target*2, target*3, etc.
                    RecursiveMWFinder(lstResults, searchOptions, ppmMode, sortedElementStats, 0, lstPotentialElementPointers, 0, targetMass * currentMzCharge, massToleranceDa, 0, currentMzCharge)
                Next

            Else
                ' RecursiveMWFinder(lstResults, searchOptions, ppmMode, strPotentialElements, dblPotentialElementStats, 0, potentialElementCount, lstPotentialElementPointers, 0, targetMass, massToleranceDa, 0, 0)
                RecursiveMWFinder(lstResults, searchOptions, ppmMode, sortedElementStats, 0, lstPotentialElementPointers, 0, targetMass, massToleranceDa, 0, 0)
            End If

            ComputeSortKeys(lstResults)

            Return lstResults

        Else
            ' Bounded search
            Const maximumFormulaMass = 0

            Dim boundedSearchResults = BoundedSearch(targetMass, massToleranceDa, maximumFormulaMass,
                                                     searchOptions, ppmMode, eCalculationMode.MatchMolecularWeight,
                                                     sortedElementStats)

            ComputeSortKeys(boundedSearchResults)

            Return boundedSearchResults
        End If

    End Function

    Private Function FindMatchesByPercentCompositionWork(
       maximumFormulaMass As Double,
       percentTolerance As Double,
       searchOptions As clsFormulaFinderOptions) As List(Of clsFormulaFinderResult)

        ' Validate the Inputs
        If Not ValidateSettings(eCalculationMode.MatchPercentComposition) Then
            Return New List(Of clsFormulaFinderResult)
        End If

        If Val(maximumFormulaMass) <= 0 Then
            ReportError("Maximum molecular weight must be greater than 0")
            Return New List(Of clsFormulaFinderResult)
        End If

        If percentTolerance < 0 Then
            ReportError("Percent tolerance cannot be negative")
            Return New List(Of clsFormulaFinderResult)
        End If

        Dim candidateElementsStats = GetCandidateElements(percentTolerance)

        If candidateElementsStats.Count = 0 Then
            Return New List(Of clsFormulaFinderResult)
        End If

        Dim sortedElementStats = (From item In candidateElementsStats Order By item.Mass Descending Select item).ToList()

        If searchOptions.SearchMode = clsFormulaFinderOptions.eSearchMode.Thorough Then
            ' Thorough search

            EstimateNumberOfOperations(sortedElementStats.Count)

            ' Pointers to the potential elements
            Dim lstPotentialElementPointers = New List(Of Integer)

            Dim lstResults = New List(Of clsFormulaFinderResult)

            RecursivePCompFinder(lstResults, searchOptions, sortedElementStats, 0, lstPotentialElementPointers, 0, maximumFormulaMass, 9)

            ComputeSortKeys(lstResults)

            Return lstResults

        Else
            ' Bounded search

            Const targetMass = 0
            Const massToleranceDa = 0
            Const ppmMode As Boolean = False

            Dim boundedSearchResults = BoundedSearch(targetMass, massToleranceDa, maximumFormulaMass,
                                                     searchOptions, ppmMode, eCalculationMode.MatchPercentComposition,
                                                     sortedElementStats)

            ComputeSortKeys(boundedSearchResults)

            Return boundedSearchResults
        End If

    End Function

    Private Function GetCandidateElements(Optional ByVal percentTolerance As Double = 0) As List(Of clsFormulaFinderCandidateElement)

        Dim candidateElementsStats = New List(Of clsFormulaFinderCandidateElement)

        Dim customElementCounter = 0
        Dim dblMass As Double
        Dim sngCharge As Single

        For Each item In mCandidateElements

            Dim candidateElement = New clsFormulaFinderCandidateElement(item.Key)

            candidateElement.CountMinimum = item.Value.MinimumCount
            candidateElement.CountMaximum = item.Value.MaximumCount

            If mElementAndMassRoutines.IsValidElementSymbol(item.Key) Then
                Dim elementID = mElementAndMassRoutines.GetElementIDInternal(item.Key)

                mElementAndMassRoutines.GetElementInternal(elementID, item.Key, dblMass, 0, sngCharge, 0)

                candidateElement.Mass = dblMass
                candidateElement.Charge = sngCharge

            Else
                ' Working with an abbreviation or simply a mass

                Dim customMass As Double

                If Double.TryParse(item.Key, customMass) Then
                    ' Custom element, only weight given so charge is 0
                    candidateElement.Mass = customMass
                    candidateElement.Charge = 0

                    customElementCounter += 1

                    ' Custom elements are named C1_ or C2_ or C3_ etc.
                    candidateElement.Symbol = "C" & customElementCounter & "_"
                Else
                    ' A single element or abbreviation was entered

                    ' Convert input to default format of first letter capitalized and rest lowercase
                    Dim abbrevSymbol = item.Key.Substring(0, 1).ToUpper() & item.Key.Substring(1).ToLower()

                    For Each currentChar In abbrevSymbol
                        If Not (Char.IsLetter(currentChar) OrElse currentChar = "+" OrElse currentChar = "_") Then
                            ReportError("Custom elemental weights must contain only numbers or only letters; if letters are used, they must be for a single valid elemental symbol or abbreviation")
                            Return New List(Of clsFormulaFinderCandidateElement)
                        End If
                    Next

                    If String.IsNullOrWhiteSpace(abbrevSymbol) Then
                        ' Too short
                        ReportError("Custom elemental weight is empty; if letters are used, they must be for a single valid elemental symbol or abbreviation")
                        Return New List(Of clsFormulaFinderCandidateElement)
                    End If

                    ' See if this is an abbreviation
                    Dim intSymbolReference = mElementAndMassRoutines.GetAbbreviationIDInternal(abbrevSymbol)
                    If intSymbolReference < 1 Then
                        ReportError("Unknown element or abbreviation for custom elemental weight: " & abbrevSymbol)
                        Return New List(Of clsFormulaFinderCandidateElement)
                    End If

                    ' Found a normal abbreviation
                    Dim matchedAbbrevSymbol As String = String.Empty
                    Dim abbrevFormula As String = String.Empty
                    Dim blnIsAminoAcid As Boolean
                    mElementAndMassRoutines.GetAbbreviationInternal(intSymbolReference, matchedAbbrevSymbol, abbrevFormula, sngCharge, blnIsAminoAcid)

                    dblMass = mElementAndMassRoutines.ComputeFormulaWeight(abbrevFormula)

                    candidateElement.Mass = dblMass

                    candidateElement.Charge = sngCharge

                End If

            End If

            candidateElement.PercentCompMinimum = item.Value.TargetPercentComposition - percentTolerance  ' Lower bound of target percentage
            candidateElement.PercentCompMaximum = item.Value.TargetPercentComposition + percentTolerance  ' Upper bound of target percentage

            candidateElementsStats.Add(candidateElement)
        Next

        Return candidateElementsStats

    End Function

    <Obsolete("Deprecated")>
    Private Function GetCandidateElements(
       ByVal percentTolerance As Double,
       ByVal intRange(,) As Integer,
       ByVal dblPotentialElementStats(,) As Double,
       ByVal strPotentialElements() As String,
       ByVal dblTargetPercents(,) As Double) As Integer

        Dim potentialElementCount = 0
        Dim customElementCounter = 0
        Dim dblMass As Double
        Dim sngCharge As Single

        For Each item In mCandidateElements

            intRange(potentialElementCount, 0) = item.Value.MinimumCount
            intRange(potentialElementCount, 1) = item.Value.MaximumCount

            If mElementAndMassRoutines.IsValidElementSymbol(item.Key) Then
                Dim elementID = mElementAndMassRoutines.GetElementIDInternal(item.Key)

                mElementAndMassRoutines.GetElementInternal(elementID, item.Key, dblMass, 0, sngCharge, 0)

                dblPotentialElementStats(potentialElementCount, 0) = dblMass
                dblPotentialElementStats(potentialElementCount, 1) = sngCharge

                strPotentialElements(potentialElementCount) = item.Key
            Else
                ' Working with an abbreviation or simply a mass

                Dim customMass As Double

                If Double.TryParse(item.Key, customMass) Then
                    ' Custom element, only weight given so charge is 0
                    dblPotentialElementStats(potentialElementCount, 0) = customMass
                    dblPotentialElementStats(potentialElementCount, 1) = 0

                    customElementCounter += 1

                    ' Custom elements are named C1_ or C2_ or C3_ etc.
                    strPotentialElements(potentialElementCount) = "C" & customElementCounter & "_"
                Else
                    ' A single element or abbreviation was entered

                    ' Convert input to default format of first letter capitalized and rest lowercase
                    Dim abbrevSymbol = item.Key.Substring(0).ToUpper() & item.Key.Substring(1).ToLower()

                    For Each currentChar In abbrevSymbol
                        If Not (Char.IsLetter(currentChar) OrElse currentChar = "+" OrElse currentChar = "_") Then
                            ReportError("Custom elemental weights must contain only numbers or only letters; if letters are used, they must be for a single valid elemental symbol or abbreviation")
                            Return 0
                        End If
                    Next

                    If String.IsNullOrWhiteSpace(abbrevSymbol) Then
                        ' Too short
                        ReportError("Custom elemental weight is empty; if letters are used, they must be for a single valid elemental symbol or abbreviation")
                        Return 0
                    End If

                    Dim charge = 0

                    ' See if this is an abbreviation
                    Dim intSymbolReference = mElementAndMassRoutines.GetAbbreviationIDInternal(abbrevSymbol)
                    If intSymbolReference < 1 Then

                        ReportError("Unknown element or abbreviation for custom elemental weight: " & abbrevSymbol)
                        Return 0
                    End If

                    ' Found a normal abbreviation
                    Dim matchedAbbrevSymbol As String = String.Empty
                    Dim abbrevFormula As String = String.Empty
                    Dim blnIsAminoAcid As Boolean
                    mElementAndMassRoutines.GetAbbreviationInternal(intSymbolReference, matchedAbbrevSymbol, abbrevFormula, sngCharge, blnIsAminoAcid)

                    dblMass = mElementAndMassRoutines.ComputeFormulaWeight(abbrevFormula)

                    ' Returns weight of element/abbreviation, but also charge
                    dblPotentialElementStats(potentialElementCount, 0) = dblMass

                    dblPotentialElementStats(potentialElementCount, 1) = charge

                    ' No problems, store symbol
                    strPotentialElements(potentialElementCount) = matchedAbbrevSymbol

                End If

            End If

            dblTargetPercents(potentialElementCount, 0) = item.Value.TargetPercentComposition - percentTolerance  ' Lower bound of target percentage
            dblTargetPercents(potentialElementCount, 1) = item.Value.TargetPercentComposition + percentTolerance  ' Upper bound of target percentage

            potentialElementCount += 1
        Next

        Return potentialElementCount

    End Function

    Private Function GetDefaultCandidateElementTolerance() As udtCandidateElementTolerances
        Return GetDefaultCandidateElementTolerance(0)
    End Function

    Private Function GetDefaultCandidateElementTolerance(minimumCount As Integer, maximumCount As Integer) As udtCandidateElementTolerances

        Dim udtElementTolerances = New udtCandidateElementTolerances

        udtElementTolerances.MinimumCount = minimumCount    ' Only used with the Bounded search mode
        udtElementTolerances.MaximumCount = maximumCount    ' Only used with the Bounded search mode

        udtElementTolerances.TargetPercentComposition = 0   ' Only used when searching for percent compositions

        Return udtElementTolerances
    End Function

    Private Function GetDefaultCandidateElementTolerance(targetPercentComposition As Double) As udtCandidateElementTolerances

        Dim udtElementTolerances = New udtCandidateElementTolerances

        udtElementTolerances.MinimumCount = 0               ' Only used with the Bounded search mode
        udtElementTolerances.MaximumCount = 10              ' Only used with the Bounded search mode

        udtElementTolerances.TargetPercentComposition = targetPercentComposition   ' Only used when searching for percent compositions

        Return udtElementTolerances

    End Function

    ''' <summary>
    ''' Initializes a new search result
    ''' </summary>
    ''' <param name="searchOptions"></param>
    ''' <param name="ppmMode"></param>
    ''' <param name="sbEmpiricalFormula"></param>
    ''' <param name="totalMass">If 0 or negative, means matching percent compositions, so don't want to add dm= to line</param>
    ''' <param name="targetMass"></param>
    ''' <param name="totalCharge"></param>
    ''' <remarks></remarks>
    Private Function GetSearchResult(
       searchOptions As clsFormulaFinderOptions,
       ppmMode As Boolean,
       sbEmpiricalFormula As StringBuilder,
       totalMass As Double,
       targetMass As Double,
       totalCharge As Double,
       empiricalResultSymbols As Dictionary(Of String, Integer)) As clsFormulaFinderResult

        Try

            Dim searchResult = New clsFormulaFinderResult(sbEmpiricalFormula.ToString(), empiricalResultSymbols)

            If searchOptions.FindCharge Then
                searchResult.ChargeState = CInt(Math.Round(totalCharge))
            End If

            If targetMass > 0 Then

                If ppmMode Then
                    searchResult.Mass = totalMass
                    searchResult.DeltaMass = CDbl(((totalMass) / targetMass - 1) * 1000000.0#)
                    searchResult.DeltaMassIsPPM = True
                Else
                    searchResult.Mass = totalMass
                    searchResult.DeltaMass = totalMass - targetMass
                    searchResult.DeltaMassIsPPM = False
                End If
            Else
                searchResult.Mass = totalMass
            End If

            If searchOptions.FindCharge AndAlso Math.Abs(totalCharge) > 0.1 Then
                ' Compute m/z value
                searchResult.MZ = Math.Abs(totalMass / totalCharge)
            End If

            Return searchResult

        Catch ex As Exception
            mElementAndMassRoutines.GeneralErrorHandler("GetSearchResult", 0, ex.Message)
            Return New clsFormulaFinderResult(String.Empty, New Dictionary(Of String, Integer))
        End Try

    End Function

    Private Function GetTotalPercentComposition() As Double
        Dim totalTargetPercentComp = mCandidateElements.Sum(Function(item) item.Value.TargetPercentComposition)
        Return totalTargetPercentComp

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="potentialElementCount"></param>
    ''' <param name="searchOptions"></param>
    ''' <remarks>searchOptions is passed ByRef because it is a value type and .MzChargeMin and .MzChargeMax are updated</remarks>
    Private Sub MultipleSearchMath(
       potentialElementCount As Integer,
       searchOptions As clsFormulaFinderOptions,
       <Out()> ByRef mzSearchChargeMin As Integer,
       <Out()> ByRef mzSearchChargeMax As Integer)

        mzSearchChargeMin = searchOptions.ChargeMin
        mzSearchChargeMax = searchOptions.ChargeMax

        mzSearchChargeMax = Math.Max(Math.Abs(mzSearchChargeMin), Math.Abs(mzSearchChargeMax))
        mzSearchChargeMin = 1

        If mzSearchChargeMax < mzSearchChargeMin Then mzSearchChargeMax = mzSearchChargeMin

        EstimateNumberOfOperations(potentialElementCount, mzSearchChargeMax - mzSearchChargeMin + 1)

    End Sub

    ''' <summary>
    ''' Formula finder that uses a series of nested for loops and is thus slow when a large number of candidate elements 
    ''' or when elements have a large range of potential counts
    ''' </summary>
    ''' <param name="searchOptions"></param>
    ''' <param name="ppmMode"></param>
    ''' <param name="calculationMode"></param>
    ''' <param name="targetMass">Only used when calculationMode is MatchMolecularWeight</param>
    ''' <param name="massToleranceDa">Only used when calculationMode is MatchMolecularWeigh</param>
    ''' <param name="maximumFormulaMass">Only used when calculationMode is MatchPercentComposition</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function OldFormulaFinder(
       searchOptions As clsFormulaFinderOptions,
       ppmMode As Boolean,
       calculationMode As eCalculationMode,
       sortedElementStats As List(Of clsFormulaFinderCandidateElement),
       targetMass As Double,
       massToleranceDa As Double,
       maximumFormulaMass As Double) As List(Of clsFormulaFinderResult)

        ' The calculated percentages for the specific compound
        Dim Percent(MAX_MATCHINGELEMENTS) As Double

        Dim lstResults = New List(Of clsFormulaFinderResult)

        Try

            ' Only used when calculationMode is MatchMolecularWeight
            Dim dblMultipleSearchMaxWeight = targetMass * searchOptions.ChargeMax

            Dim sbEmpiricalFormula = New StringBuilder()

            Dim lstRanges = New List(Of udtBoundedSearchRangeType)

            For elementIndex = 0 To sortedElementStats.Count - 1
                Dim udtBoundedSearchRange = New udtBoundedSearchRangeType
                udtBoundedSearchRange.Min = sortedElementStats(elementIndex).CountMinimum
                udtBoundedSearchRange.Max = sortedElementStats(elementIndex).CountMaximum
                lstRanges.Add(udtBoundedSearchRange)
            Next

            While lstRanges.Count < MAX_MATCHINGELEMENTS
                Dim udtBoundedSearchRange = New udtBoundedSearchRangeType
                udtBoundedSearchRange.Min = 0
                udtBoundedSearchRange.Max = 0
                lstRanges.Add(udtBoundedSearchRange)
            End While

            Dim potentialElementCount = sortedElementStats.Count

            ' Determine the valid compounds
            For j = lstRanges(0).Min To lstRanges(0).Max
                For k = lstRanges(1).Min To lstRanges(1).Max
                    For l = lstRanges(2).Min To lstRanges(2).Max
                        For m = lstRanges(3).Min To lstRanges(3).Max
                            For N = lstRanges(4).Min To lstRanges(4).Max
                                For O = lstRanges(5).Min To lstRanges(5).Max
                                    For P = lstRanges(6).Min To lstRanges(6).Max
                                        For q = lstRanges(7).Min To lstRanges(7).Max
                                            For r = lstRanges(8).Min To lstRanges(8).Max
                                                For S = lstRanges(9).Min To lstRanges(9).Max

                                                    Dim totalMass = j * sortedElementStats(0).Mass
                                                    Dim totalCharge = j * sortedElementStats(0).Charge

                                                    If potentialElementCount > 1 Then
                                                        totalMass += k * sortedElementStats(1).Mass
                                                        totalCharge += k * sortedElementStats(1).Charge

                                                        If potentialElementCount > 2 Then
                                                            totalMass += l * sortedElementStats(2).Mass
                                                            totalCharge += l * sortedElementStats(2).Charge

                                                            If potentialElementCount > 3 Then
                                                                totalMass += m * sortedElementStats(3).Mass
                                                                totalCharge += m * sortedElementStats(3).Charge

                                                                If potentialElementCount > 4 Then
                                                                    totalMass += N * sortedElementStats(4).Mass
                                                                    totalCharge += N * sortedElementStats(4).Charge

                                                                    If potentialElementCount > 5 Then
                                                                        totalMass += O * sortedElementStats(5).Mass
                                                                        totalCharge += O * sortedElementStats(5).Charge

                                                                        If potentialElementCount > 6 Then
                                                                            totalMass += P * sortedElementStats(6).Mass
                                                                            totalCharge += P * sortedElementStats(6).Charge

                                                                            If potentialElementCount > 7 Then
                                                                                totalMass += q * sortedElementStats(7).Mass
                                                                                totalCharge += q * sortedElementStats(7).Charge

                                                                                If potentialElementCount > 8 Then
                                                                                    totalMass += r * sortedElementStats(8).Mass
                                                                                    totalCharge += r * sortedElementStats(8).Charge

                                                                                    If potentialElementCount > 9 Then
                                                                                        totalMass += S * sortedElementStats(9).Mass
                                                                                        totalCharge += S * sortedElementStats(9).Charge
                                                                                    End If

                                                                                End If
                                                                            End If
                                                                        End If
                                                                    End If
                                                                End If
                                                            End If
                                                        End If
                                                    End If
                                                    
                                                    If calculationMode = eCalculationMode.MatchPercentComposition Then
                                                        ' Matching Percent Compositions
                                                        If totalMass > 0 And totalMass <= maximumFormulaMass Then
                                                            Percent(0) = j * sortedElementStats(0).Mass / totalMass * 100
                                                            If potentialElementCount > 1 Then
                                                                Percent(1) = k * sortedElementStats(1).Mass / totalMass * 100

                                                                If potentialElementCount > 1 Then
                                                                    Percent(2) = l * sortedElementStats(2).Mass / totalMass * 100

                                                                    If potentialElementCount > 1 Then
                                                                        Percent(3) = m * sortedElementStats(3).Mass / totalMass * 100

                                                                        If potentialElementCount > 1 Then
                                                                            Percent(4) = N * sortedElementStats(4).Mass / totalMass * 100

                                                                            If potentialElementCount > 1 Then
                                                                                Percent(5) = O * sortedElementStats(5).Mass / totalMass * 100

                                                                                If potentialElementCount > 1 Then
                                                                                    Percent(6) = P * sortedElementStats(6).Mass / totalMass * 100

                                                                                    If potentialElementCount > 1 Then
                                                                                        Percent(7) = q * sortedElementStats(7).Mass / totalMass * 100

                                                                                        If potentialElementCount > 1 Then
                                                                                            Percent(8) = r * sortedElementStats(8).Mass / totalMass * 100

                                                                                            If potentialElementCount > 1 Then
                                                                                                Percent(9) = S * sortedElementStats(9).Mass / totalMass * 100
                                                                                            End If
                                                                                        End If
                                                                                    End If
                                                                                End If
                                                                            End If
                                                                        End If
                                                                    End If
                                                                End If

                                                            End If

                                                            Dim intSubTrack = 0
                                                            For intIndex = 0 To potentialElementCount - 1
                                                                If Percent(intIndex) >= sortedElementStats(intIndex).PercentCompMinimum AndAlso
                                                                   Percent(intIndex) <= sortedElementStats(intIndex).PercentCompMaximum Then
                                                                    intSubTrack += 1
                                                                End If
                                                            Next

                                                            If intSubTrack = potentialElementCount Then
                                                                ' All of the elements have percent compositions matching the target

                                                                Dim empiricalResultSymbols As Dictionary(Of String, Integer) = Nothing
                                                                Dim correctedCharge As Double

                                                                ' Construct the empirical formula and verify hydrogens
                                                                Dim blnHOK = ConstructAndVerifyCompound(searchOptions,
                                                                                                        sbEmpiricalFormula,
                                                                                                        j, k, l, m, N, O, P, q, r, S,
                                                                                                        sortedElementStats,
                                                                                                        totalMass, targetMass, massToleranceDa,
                                                                                                        totalCharge, 0,
                                                                                                        empiricalResultSymbols,
                                                                                                        correctedCharge)


                                                                If sbEmpiricalFormula.Length > 0 AndAlso blnHOK Then
                                                                    Dim searchResult = GetSearchResult(searchOptions, ppmMode, sbEmpiricalFormula, totalMass, -1, correctedCharge, empiricalResultSymbols)

                                                                    ' Add % composition info

                                                                    AppendPercentCompositionResult(searchResult, j, sortedElementStats, 0, Percent(0))
                                                                    AppendPercentCompositionResult(searchResult, k, sortedElementStats, 1, Percent(1))
                                                                    AppendPercentCompositionResult(searchResult, l, sortedElementStats, 2, Percent(2))
                                                                    AppendPercentCompositionResult(searchResult, m, sortedElementStats, 3, Percent(3))
                                                                    AppendPercentCompositionResult(searchResult, N, sortedElementStats, 4, Percent(4))
                                                                    AppendPercentCompositionResult(searchResult, O, sortedElementStats, 5, Percent(5))
                                                                    AppendPercentCompositionResult(searchResult, P, sortedElementStats, 6, Percent(6))
                                                                    AppendPercentCompositionResult(searchResult, q, sortedElementStats, 7, Percent(7))
                                                                    AppendPercentCompositionResult(searchResult, r, sortedElementStats, 8, Percent(8))
                                                                    AppendPercentCompositionResult(searchResult, S, sortedElementStats, 9, Percent(9))

                                                                    lstResults.Add(searchResult)
                                                                End If
                                                            End If
                                                        End If

                                                    Else
                                                        ' Matching Molecular Weights

                                                        If totalMass <= dblMultipleSearchMaxWeight + massToleranceDa Then

                                                            ' When searchOptions.FindTargetMZ is false, ChargeMin and ChargeMax will be 1
                                                            For intCurrentCharge = searchOptions.ChargeMin To searchOptions.ChargeMax

                                                                Dim dblMatchWeight = targetMass * intCurrentCharge
                                                                If totalMass <= dblMatchWeight + massToleranceDa AndAlso
                                                                   totalMass >= dblMatchWeight - massToleranceDa Then
                                                                    ' Within massToleranceDa

                                                                    Dim empiricalResultSymbols As Dictionary(Of String, Integer) = Nothing
                                                                    Dim correctedCharge As Double

                                                                    ' Construct the empirical formula and verify hydrogens
                                                                    Dim blnHOK = ConstructAndVerifyCompound(searchOptions,
                                                                                                            sbEmpiricalFormula,
                                                                                                            j, k, l, m, N, O, P, q, r, S,
                                                                                                            sortedElementStats,
                                                                                                            totalMass, targetMass * intCurrentCharge, massToleranceDa,
                                                                                                            totalCharge, intCurrentCharge,
                                                                                                            empiricalResultSymbols,
                                                                                                            correctedCharge)

                                                                    If sbEmpiricalFormula.Length > 0 AndAlso blnHOK Then
                                                                        Dim searchResult = GetSearchResult(searchOptions, ppmMode, sbEmpiricalFormula, totalMass, targetMass, correctedCharge, empiricalResultSymbols)

                                                                        lstResults.Add(searchResult)
                                                                    End If
                                                                    Exit For
                                                                End If
                                                            Next
                                                        Else

                                                            ' Jump out of loop since weight is too high
                                                            ' Determine which variable is causing the weight to be too high
                                                            ' Incrementing "s" would definitely make the weight too high, so set it to its max (so it will zero and increment "r")
                                                            S = lstRanges(9).Max
                                                            If (j * lstRanges(0).Min + k * lstRanges(1).Min + l * lstRanges(2).Min + m * lstRanges(3).Min + N * lstRanges(4).Min + O * lstRanges(5).Min + P * lstRanges(6).Min + q * lstRanges(7).Min + (r + 1) * lstRanges(8).Min) > (massToleranceDa + dblMultipleSearchMaxWeight) Then
                                                                ' Incrementing r would make the weight too high, so set it to its max (so it will zero and increment q)
                                                                r = lstRanges(8).Max
                                                                If (j * lstRanges(0).Min + k * lstRanges(1).Min + l * lstRanges(2).Min + m * lstRanges(3).Min + N * lstRanges(4).Min + O * lstRanges(5).Min + P * lstRanges(6).Min + (q + 1) * lstRanges(7).Min) > (massToleranceDa + dblMultipleSearchMaxWeight) Then
                                                                    q = lstRanges(7).Max
                                                                    If (j * lstRanges(0).Min + k * lstRanges(1).Min + l * lstRanges(2).Min + m * lstRanges(3).Min + N * lstRanges(4).Min + O * lstRanges(5).Min + (P + 1) * lstRanges(6).Min) > (massToleranceDa + dblMultipleSearchMaxWeight) Then
                                                                        P = lstRanges(6).Max
                                                                        If (j * lstRanges(0).Min + k * lstRanges(1).Min + l * lstRanges(2).Min + m * lstRanges(3).Min + N * lstRanges(4).Min + (O + 1) * lstRanges(5).Min) > (massToleranceDa + dblMultipleSearchMaxWeight) Then
                                                                            O = lstRanges(5).Max
                                                                            If (j * lstRanges(0).Min + k * lstRanges(1).Min + l * lstRanges(2).Min + m * lstRanges(3).Min + (N + 1) * lstRanges(4).Min) > (massToleranceDa + dblMultipleSearchMaxWeight) Then
                                                                                N = lstRanges(4).Max
                                                                                If (j * lstRanges(0).Min + k * lstRanges(1).Min + l * lstRanges(2).Min + (m + 1) * lstRanges(3).Min) > (massToleranceDa + dblMultipleSearchMaxWeight) Then
                                                                                    m = lstRanges(3).Max
                                                                                    If (j * lstRanges(0).Min + k * lstRanges(1).Min + (l + 1) * lstRanges(2).Min) > (massToleranceDa + dblMultipleSearchMaxWeight) Then
                                                                                        l = lstRanges(2).Max
                                                                                        If (j * lstRanges(0).Min + (k + 1) * lstRanges(1).Min) > (massToleranceDa + dblMultipleSearchMaxWeight) Then
                                                                                            k = lstRanges(1).Max
                                                                                            If ((j + 1) * lstRanges(0).Min) > (massToleranceDa + dblMultipleSearchMaxWeight) Then
                                                                                                j = lstRanges(0).Max
                                                                                            End If
                                                                                        End If
                                                                                    End If
                                                                                End If
                                                                            End If
                                                                        End If
                                                                    End If
                                                                End If
                                                            End If
                                                        End If
                                                    End If


                                                    If mAbortProcessing Then
                                                        Return lstResults
                                                    End If

                                                    If lstResults.Count >= mMaximumHits Then

                                                        ' Set variables to their maximum so all the loops will end
                                                        j = lstRanges(0).Max
                                                        k = lstRanges(1).Max
                                                        l = lstRanges(2).Max
                                                        m = lstRanges(3).Max
                                                        N = lstRanges(4).Max
                                                        O = lstRanges(5).Max
                                                        P = lstRanges(6).Max
                                                        q = lstRanges(7).Max
                                                        r = lstRanges(8).Max
                                                        S = lstRanges(9).Max
                                                    End If

                                                Next S
                                            Next r
                                        Next q
                                    Next P
                                Next O
                            Next N
                        Next m
                    Next l
                Next k

                If lstRanges(0).Max <> 0 Then

                    If searchOptions.ChargeMin = 0 Then
                        mPercentComplete = j / lstRanges(0).Max * 100
                    Else
                        mPercentComplete = j / lstRanges(0).Max * 100 * searchOptions.ChargeMax
                    End If
                    Console.WriteLine("Bounded search: " & mPercentComplete.ToString("0") & "% complete")
                End If

            Next j

        Catch ex As Exception
            mElementAndMassRoutines.GeneralErrorHandler("OldFormulaFinder", 0, ex.Message)
        End Try

        Return lstResults

    End Function

    <Obsolete("Deprecated")>
    Private Sub SortCandidateElements(
       calculationMode As eCalculationMode,
       potentialElementCount As Integer,
       dblPotentialElementStats(,) As Double,
       strPotentialElements() As String,
       dblTargetPercents(,) As Double)

        ' Reorder dblPotentialElementStats pointer array in order from heaviest to lightest element
        ' Greatly speeds up the recursive routine

        ' Bubble sort
        For y = potentialElementCount - 1 To 1 Step -1       ' Sort from end to start
            For x = 0 To y - 1
                If dblPotentialElementStats(x, 0) < dblPotentialElementStats(x + 1, 0) Then
                    ' Swap the element symbols
                    Dim strSwap = strPotentialElements(x)
                    strPotentialElements(x) = strPotentialElements(x + 1)
                    strPotentialElements(x + 1) = strSwap

                    ' and their weights
                    Dim dblSwapVal = dblPotentialElementStats(x, 0)
                    dblPotentialElementStats(x, 0) = dblPotentialElementStats(x + 1, 0)
                    dblPotentialElementStats(x + 1, 0) = dblSwapVal

                    ' and their charge
                    dblSwapVal = dblPotentialElementStats(x, 1)
                    dblPotentialElementStats(x, 1) = dblPotentialElementStats(x + 1, 1)
                    dblPotentialElementStats(x + 1, 1) = dblSwapVal

                    If calculationMode = eCalculationMode.MatchPercentComposition Then
                        ' and the dblTargetPercents array
                        dblSwapVal = dblTargetPercents(x, 0)
                        dblTargetPercents(x, 0) = dblTargetPercents(x + 1, 0)
                        dblTargetPercents(x + 1, 0) = dblSwapVal

                        dblSwapVal = dblTargetPercents(x, 1)
                        dblTargetPercents(x, 1) = dblTargetPercents(x + 1, 1)
                        dblTargetPercents(x + 1, 1) = dblSwapVal

                    End If
                End If
            Next x
        Next y

    End Sub

    ''' <summary>
    ''' Recursively serch for a target mass
    ''' </summary>
    ''' <param name="lstResults"></param>
    ''' <param name="searchOptions"></param>
    ''' <param name="sortedElementStats">Candidate elements, including mass and charge. Sorted by de</param>
    ''' <param name="intStartIndex">Index in candidateElementsStats to start at</param>
    ''' <param name="lstPotentialElementPointers">Pointers to the elements that have been added to the potential formula so far</param>
    ''' <param name="dblPotentialMassTotal">Weight of the potential formula</param>
    ''' <param name="targetMass"></param>
    ''' <param name="massToleranceDa"></param>
    ''' <param name="potentialChargeTotal"></param>
    ''' <param name="intMultipleMtoZCharge">When searchOptions.FindTargetMZ is false, this will be 0; otherwise, the current charge being searched for</param>
    ''' <remarks></remarks>
    Private Sub RecursiveMWFinder(
       lstResults As ICollection(Of clsFormulaFinderResult),
       searchOptions As clsFormulaFinderOptions,
       ppmMode As Boolean,
       sortedElementStats As List(Of clsFormulaFinderCandidateElement),
       intStartIndex As Integer,
       lstPotentialElementPointers As List(Of Integer),
       dblPotentialMassTotal As Double,
       targetMass As Double,
       massToleranceDa As Double,
       potentialChargeTotal As Double,
       intMultipleMtoZCharge As Integer)

        Try

            Dim lstNewPotentialElementPointers = New List(Of Integer)(lstPotentialElementPointers.Count + 1)

            If mAbortProcessing Or lstResults.Count >= mMaximumHits Then
                Exit Sub
            End If

            Dim sbEmpiricalFormula = New StringBuilder()

            For intCurrentIndex = intStartIndex To sortedElementStats.Count - 1
                Dim totalMass = dblPotentialMassTotal + sortedElementStats(intCurrentIndex).Mass
                Dim totalCharge = potentialChargeTotal + sortedElementStats(intCurrentIndex).Charge

                lstNewPotentialElementPointers.Clear()

                If totalMass <= targetMass + massToleranceDa Then
                    ' Below or within dblMassTolerance, add current element's pointer to pointer array
                    lstNewPotentialElementPointers.AddRange(lstPotentialElementPointers)

                    ' Append the current element's number
                    lstNewPotentialElementPointers.Add(intCurrentIndex)

                    ' Update status
                    UpdateStatus()

                    ' Uncomment to add a breakpoint when a certain empirical formula is encountered
                    If lstPotentialElementPointers.Count >= 3 Then
                        Dim empiricalResultSymbols = ConvertElementPointersToElementStats(sortedElementStats, lstPotentialElementPointers)
                        Dim debugCompound = New Dictionary(Of String, Integer)
                        debugCompound.Add("C", 7)
                        debugCompound.Add("H", 4)
                        debugCompound.Add("O", 7)

                        If EmpiricalFormulaHasElementCounts(empiricalResultSymbols, debugCompound) Then
                            Console.WriteLine("Debug: Check this formula")
                        End If

                    End If

                    If mAbortProcessing OrElse lstResults.Count >= mMaximumHits Then
                        Exit Sub
                    End If

                    If totalMass >= targetMass - massToleranceDa Then
                        ' Matching compound

                        Dim empiricalResultSymbols As Dictionary(Of String, Integer) = Nothing
                        Dim correctedCharge As Double

                        ' Construct the empirical formula and verify hydrogens
                        Dim blnHOK = ConstructAndVerifyCompoundRecursive(searchOptions,
                                                                         sbEmpiricalFormula, sortedElementStats,
                                                                         lstNewPotentialElementPointers,
                                                                         totalMass, targetMass, massToleranceDa,
                                                                         totalCharge, intMultipleMtoZCharge,
                                                                         empiricalResultSymbols,
                                                                         correctedCharge)

                        If sbEmpiricalFormula.Length > 0 AndAlso blnHOK Then
                            Dim searchResult = GetSearchResult(searchOptions, ppmMode, sbEmpiricalFormula, totalMass, targetMass, correctedCharge, empiricalResultSymbols)

                            lstResults.Add(searchResult)
                        End If

                    End If

                    ' Haven't reached targetMass - dblMassTolerance region, so call RecursiveFinder again

                    ' But first, if adding the lightest element (i.e. the last in the list),
                    ' add a bunch of it until the potential compound's weight is close to the target
                    If intCurrentIndex = sortedElementStats.Count - 1 Then

                        Dim intExtra = 0
                        Do While totalMass < targetMass - massToleranceDa - sortedElementStats(intCurrentIndex).Mass
                            intExtra += 1
                            totalMass += sortedElementStats(intCurrentIndex).Mass
                            totalCharge += sortedElementStats(intCurrentIndex).Charge
                        Loop

                        If intExtra > 0 Then

                            For intPointer = 1 To intExtra
                                lstNewPotentialElementPointers.Add(intCurrentIndex)
                            Next

                        End If
                    End If

                    ' Now recursively call this sub
                    RecursiveMWFinder(lstResults, searchOptions, ppmMode, sortedElementStats, intCurrentIndex, lstNewPotentialElementPointers, totalMass, targetMass, massToleranceDa, totalCharge, intMultipleMtoZCharge)
                End If
            Next intCurrentIndex

        Catch ex As Exception
            mElementAndMassRoutines.GeneralErrorHandler("RecursiveMWFinder", 0, ex.Message)
            mAbortProcessing = True
        End Try

    End Sub

    ''' <summary>
    ''' Recursively search for target percent composition values
    ''' </summary>
    ''' <param name="lstResults"></param>
    ''' <param name="intStartIndex"></param>
    ''' <param name="lstPotentialElementPointers">Pointers to the elements that have been added to the potential formula so far</param>
    ''' <param name="dblPotentialMassTotal">>Weight of the potential formula</param>
    ''' <param name="maximumFormulaMass"></param>
    ''' <param name="potentialChargeTotal"></param>
    ''' <remarks></remarks>
    Private Sub RecursivePCompFinder(
       lstResults As ICollection(Of clsFormulaFinderResult),
       searchOptions As clsFormulaFinderOptions,
       sortedElementStats As IList(Of clsFormulaFinderCandidateElement),
       intStartIndex As Integer,
       lstPotentialElementPointers As ICollection(Of Integer),
       dblPotentialMassTotal As Double,
       maximumFormulaMass As Double,
       potentialChargeTotal As Double)

        Try

            Dim lstNewPotentialElementPointers = New List(Of Integer)(lstPotentialElementPointers.Count + 1)

            Dim dblPotentialPercents(sortedElementStats.Count) As Double

            If mAbortProcessing Or lstResults.Count >= mMaximumHits Then
                Exit Sub
            End If

            Dim sbEmpiricalFormula = New StringBuilder()
            Const ppmMode = False

            For intCurrentIndex = intStartIndex To sortedElementStats.Count - 1  ' potentialElementCount >= 1, if 1, means just dblPotentialElementStats(0,0), etc.
                Dim totalMass = dblPotentialMassTotal + sortedElementStats(intCurrentIndex).Mass
                Dim totalCharge = potentialChargeTotal + sortedElementStats(intCurrentIndex).Charge

                lstNewPotentialElementPointers.Clear()

                If totalMass <= maximumFormulaMass Then
                    ' only proceed if weight is less than max weight

                    lstNewPotentialElementPointers.AddRange(lstPotentialElementPointers)

                    ' Append the current element's number
                    lstNewPotentialElementPointers.Add(intCurrentIndex)

                    ' Compute the number of each element
                    Dim elementCountArray = GetElementCountArray(sortedElementStats.Count, lstNewPotentialElementPointers)

                    Dim nonZeroCount = (From item In elementCountArray Where item > 0).Count

                    ' Only proceed if all elements are present
                    If nonZeroCount = sortedElementStats.Count Then

                        ' Compute % comp of each element
                        For intIndex = 0 To sortedElementStats.Count - 1
                            dblPotentialPercents(intIndex) = elementCountArray(intIndex) * sortedElementStats(intIndex).Mass / totalMass * 100
                        Next
                        'If intPointerCount = 0 Then dblPotentialPercents(0) = 100

                        Dim intPercentTrack = 0
                        For intIndex = 0 To sortedElementStats.Count - 1
                            If dblPotentialPercents(intIndex) >= sortedElementStats(intIndex).PercentCompMinimum AndAlso
                               dblPotentialPercents(intIndex) <= sortedElementStats(intIndex).PercentCompMaximum Then
                                intPercentTrack += 1
                            End If
                        Next

                        If intPercentTrack = sortedElementStats.Count Then
                            ' Matching compound

                            Dim empiricalResultSymbols As Dictionary(Of String, Integer) = Nothing
                            Dim correctedCharge As Double

                            ' Construct the empirical formula and verify hydrogens
                            Dim blnHOK = ConstructAndVerifyCompoundRecursive(searchOptions,
                                                                             sbEmpiricalFormula, sortedElementStats,
                                                                             lstNewPotentialElementPointers,
                                                                             totalMass, 0, 0,
                                                                             totalCharge, 0,
                                                                             empiricalResultSymbols,
                                                                             correctedCharge)

                            If sbEmpiricalFormula.Length > 0 AndAlso blnHOK Then
                                Dim searchResult = GetSearchResult(searchOptions, ppmMode, sbEmpiricalFormula, totalMass, -1, correctedCharge, empiricalResultSymbols)

                                ' Add % composition info
                                For intIndex = 0 To sortedElementStats.Count - 1
                                    If elementCountArray(intIndex) <> 0 Then
                                        Dim percentComposition = elementCountArray(intIndex) * sortedElementStats(intIndex).Mass / totalMass * 100

                                        AppendPercentCompositionResult(searchResult, elementCountArray(intIndex), sortedElementStats, intIndex, percentComposition)

                                    End If
                                Next

                                lstResults.Add(searchResult)
                            End If

                        End If

                    End If

                    ' Update status
                    UpdateStatus()

                    If mAbortProcessing OrElse lstResults.Count >= mMaximumHits Then
                        Exit Sub
                    End If

                    ' Haven't reached maximumFormulaMass
                    ' Now recursively call this sub
                    RecursivePCompFinder(lstResults, searchOptions, sortedElementStats, intCurrentIndex, lstNewPotentialElementPointers, totalMass, maximumFormulaMass, totalCharge)

                End If
            Next intCurrentIndex


        Catch ex As Exception
            mElementAndMassRoutines.GeneralErrorHandler("RecursivePCompFinder", 0, ex.Message)
            mAbortProcessing = True
        End Try

    End Sub

    Protected Sub ReportError(strErrorMessage As String)
        mErrorMessage = strErrorMessage
        If EchoMessagesToConsole Then Console.WriteLine(strErrorMessage)

        RaiseEvent ErrorEvent(strErrorMessage)
    End Sub

    Protected Sub ReportWarning(strWarningMessage As String)
        If EchoMessagesToConsole Then Console.WriteLine(strWarningMessage)

        RaiseEvent WarningEvent(strWarningMessage)
    End Sub

    Protected Sub ShowMessage(strMessage As String)
        If EchoMessagesToConsole Then Console.WriteLine(strMessage)
        RaiseEvent MessageEvent(strMessage)
    End Sub

    Private Sub UpdateStatus()
        mRecursiveCount += 1

        If mRecursiveCount <= mMaxRecursiveCount Then
            mPercentComplete = mRecursiveCount / CSng(mMaxRecursiveCount) * 100
        End If

    End Sub

    Private Sub ValidateBoundedSearchValues()
        For Each elementSymbol In mCandidateElements.Keys()
            Dim udtElementTolerances = mCandidateElements(elementSymbol)

            If udtElementTolerances.MinimumCount < 0 OrElse udtElementTolerances.MaximumCount > MAX_BOUNDED_SEARCH_COUNT Then
                If udtElementTolerances.MinimumCount < 0 Then udtElementTolerances.MinimumCount = 0
                If udtElementTolerances.MaximumCount > MAX_BOUNDED_SEARCH_COUNT Then udtElementTolerances.MaximumCount = MAX_BOUNDED_SEARCH_COUNT

                mCandidateElements(elementSymbol) = udtElementTolerances
            End If
        Next
    End Sub

    Private Sub ValidatePercentCompositionValues()
        For Each elementSymbol In mCandidateElements.Keys()
            Dim udtElementTolerances = mCandidateElements(elementSymbol)

            If udtElementTolerances.TargetPercentComposition < 0 Or udtElementTolerances.TargetPercentComposition > 100 Then
                If udtElementTolerances.TargetPercentComposition < 0 Then udtElementTolerances.TargetPercentComposition = 0
                If udtElementTolerances.TargetPercentComposition > 100 Then udtElementTolerances.TargetPercentComposition = 100

                mCandidateElements(elementSymbol) = udtElementTolerances
            End If
        Next
    End Sub

    Private Function ValidateSettings(calculationMode As eCalculationMode) As Boolean

        If mCandidateElements.Count = 0 Then
            ReportError("No candidate elements are defined; use method AddCandidateElement or property CandidateElements")
            Return False
        End If

        ValidateBoundedSearchValues()

        If calculationMode = eCalculationMode.MatchPercentComposition Then
            Dim totalTargetPercentComp = GetTotalPercentComposition()

            If Math.Abs(totalTargetPercentComp - 100) > Single.Epsilon Then
                ReportError("Sum of the target percentages must be 100%; it is currently " + totalTargetPercentComp.ToString("0.0") + "%")
                Return False
            End If

        End If

        Return True

    End Function
End Class
