Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions

Public Module FormulaStringParcer
    Public Function OrganicElementsReader(ByVal formulaString As String) As DerivatizationFormula
        If Equals(formulaString, Nothing) Then Return Nothing
        If Equals(formulaString, String.Empty) Then Return Nothing
        If Equals(formulaString, "Unknown") Then Return Nothing
        If Equals(formulaString, "null") Then Return Nothing

        Dim cnum = 0, hnum = 0, pnum = 0, snum = 0, onum = 0, nnum = 0, fnum = 0, clnum = 0, brnum = 0, inum = 0, sinum = 0, c13num = 0, h2num = 0, tmsCount = 0, meoxCount = 0

        Dim elements = formulaString

        'check the existense of TMS or MEOX information in the string
        If formulaString.Contains("_"c) Then
            Dim stringArray = formulaString.Split("_"c)
            elements = stringArray(0)
            For i = 1 To stringArray.Length - 1
                Dim numberChars = String.Empty
                For Each charValue In stringArray(i)
                    If Char.IsNumber(charValue) Then
                        numberChars += charValue
                    Else
                        Exit For
                    End If
                Next

                If stringArray(i).Contains("TMS") AndAlso Not Equals(numberChars, String.Empty) Then
                    tmsCount = Integer.Parse(numberChars)
                ElseIf stringArray(i).Contains("MEOX") AndAlso Not Equals(numberChars, String.Empty) Then
                    meoxCount = Integer.Parse(numberChars)
                End If
            Next
        End If

        setElementNumbers(elements, cnum, hnum, nnum, onum, pnum, snum, fnum, clnum, brnum, inum, sinum, c13num, h2num, tmsCount, meoxCount)
        Dim formula = New DerivatizationFormula(cnum, hnum, nnum, onum, pnum, snum, fnum, clnum, brnum, inum, sinum, tmsCount, meoxCount)
        If c13num > 0 OrElse h2num > 0 Then
            formula = New DerivatizationFormula(cnum, hnum, nnum, onum, pnum, snum, fnum, clnum, brnum, inum, sinum, c13num, h2num, tmsCount, meoxCount)
        End If

        Return formula
    End Function

    Public Function OrganicElementsReader(ByVal formulaString As String, ByVal cLabelMass As Double, ByVal hLabelMass As Double, ByVal nLabelMass As Double, ByVal oLabelMass As Double, ByVal pLabelMass As Double, ByVal sLabelMass As Double, ByVal fLabelMass As Double, ByVal clLabelMass As Double, ByVal brLabelMass As Double, ByVal iLabelMass As Double, ByVal siLabelMass As Double) As DerivatizationFormula
        Dim cnum = 0, hnum = 0, pnum = 0, snum = 0, onum = 0, nnum = 0, fnum = 0, clnum = 0, brnum = 0, inum = 0, sinum = 0

        setElementNumbers(formulaString, cnum, hnum, nnum, onum, pnum, snum, fnum, clnum, brnum, inum, sinum)

        Dim formula = New DerivatizationFormula(cnum, hnum, nnum, onum, pnum, snum, fnum, clnum, brnum, inum, sinum, cLabelMass, hLabelMass, nLabelMass, oLabelMass, pLabelMass, sLabelMass, fLabelMass, clLabelMass, brLabelMass, iLabelMass, siLabelMass)

        Return formula
    End Function

    Public Function IsOrganicFormula(ByVal formulaString As String) As Boolean
        Dim mc As MatchCollection

        Dim cnum = 0
        Dim hnum = 0
        Dim pnum = 0
        Dim snum = 0
        Dim onum = 0
        Dim nnum = 0
        Dim fnum = 0
        Dim clnum = 0
        Dim brnum = 0
        Dim inum = 0
        Dim sinum = 0

#Region ""

        mc = Regex.Matches(formulaString, "C(?!a|d|e|l|o|r|s|u)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                cnum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, cnum)
            End If
        End If

        mc = Regex.Matches(formulaString, "H(?!e|f|g|o)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                hnum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, hnum)
            End If
        End If

        mc = Regex.Matches(formulaString, "N(?!a|b|d|e|i)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                nnum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, nnum)
            End If
        End If

        mc = Regex.Matches(formulaString, "O(?!s)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                onum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, onum)
            End If
        End If

        mc = Regex.Matches(formulaString, "S(?!b|c|e|i|m|n|r)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                snum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, snum)
            End If
        End If

        mc = Regex.Matches(formulaString, "P(?!d|t|b|r)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                pnum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, pnum)
            End If
        End If

        mc = Regex.Matches(formulaString, "Br([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                brnum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, brnum)
            End If
        End If

        mc = Regex.Matches(formulaString, "Cl([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                clnum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, clnum)
            End If
        End If

        mc = Regex.Matches(formulaString, "F(?!e)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                fnum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, fnum)
            End If
        End If

        mc = Regex.Matches(formulaString, "I(?!n|r)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                inum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, inum)
            End If
        End If

        mc = Regex.Matches(formulaString, "Si([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                sinum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, sinum)
            End If
        End If
#End Region

        Dim formula = New DerivatizationFormula(cnum, hnum, nnum, onum, pnum, snum, fnum, clnum, brnum, inum, sinum, 0, 0)

        If formula.EmpiricalFormula.Length = formulaString.Length Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function MQComposition2FormulaObj(ByVal composition As String) As DerivatizationFormula
        Dim elements = composition.Split(" "c)
        Dim dict = New Dictionary(Of String, Integer)()
        Dim elemCount As Integer = Nothing
        For Each elem In elements
            If Not elem.Contains("(") Then
                dict(elem) = 1
            Else
                Dim elemName = elem.Split("("c)(0)
                Dim elemCountString = elem.Split("("c)(1).Split(")"c)(0)

                If Integer.TryParse(elemCountString, elemCount) Then
                    dict(elemName) = elemCount
                End If
            End If
        Next

        Return New DerivatizationFormula(dict)
    End Function

    Friend Function Convert2FormulaObjV3(ByVal formulaString As String) As DerivatizationFormula
        Return ToFormula(TokenizeFormula(formulaString))
    End Function

    Private Function ToFormula(ByVal elements As List(Of (String, Integer))) As DerivatizationFormula
        Dim result = New Dictionary(Of String, Integer)()
        Dim element As String = Nothing, number As Integer = Nothing

        For Each __ As (element$, number%) In elements
            element = __.element
            number = __.number

            If result.ContainsKey(element) Then
                result(element) += number
            Else
                result(element) = number
            End If
        Next
        Return New DerivatizationFormula(result)
    End Function

    Public Function TokenizeFormula(ByVal formulaString As String) As List(Of (String, Integer))
        Return TokenizeFormulaCharacter(formulaString).[Select](New Func(Of String, (String, Integer))(AddressOf ParseToken)).ToList()
    End Function

    Private Function TokenizeFormulaCharacter(ByVal formulaString As String) As List(Of String)
        ' C12H24O12 -> C12, H24, O12
        Dim result = New List(Of String)()
        Dim i = 0
        While i < formulaString.Length
            While i < formulaString.Length AndAlso Not Char.IsUpper(formulaString(i)) AndAlso formulaString(i) <> "["c
                i += 1
            End While

            If i >= formulaString.Length Then
                Exit While
            End If

            If Char.IsUpper(formulaString(i)) Then
                Dim j = i
                j += 1
                While j < formulaString.Length AndAlso Not Char.IsUpper(formulaString(j)) AndAlso formulaString(j) <> "["c
                    j += 1
                End While
                result.Add(formulaString.Substring(i, j - i))
                i = j
            ElseIf formulaString(i) = "["c Then
                Dim j = i
                j += 1
                While j < formulaString.Length AndAlso formulaString(j) <> "]"c
                    j += 1
                End While
                While j < formulaString.Length AndAlso Not Char.IsUpper(formulaString(j)) AndAlso formulaString(j) <> "["c
                    j += 1
                End While
                result.Add(formulaString.Substring(i, j - i))
                i = j
            Else
                Throw New ArgumentException(NameOf(formulaString))
            End If
        End While
        Return result
    End Function

    Private Function ParseToken(ByVal token As String) As (String, Integer)
        ' C2 -> C, 2 , [13C]2 -> [13C], 2
        Dim cleaned = New String(token.Where(Function(c) Not Char.IsWhiteSpace(c)).ToArray())
        Dim i = 0, j = 0
        Dim result As Integer = Nothing
        If Char.IsUpper(cleaned(i)) Then
            While j < cleaned.Length AndAlso Not Char.IsNumber(cleaned(j))
                j += 1
            End While
            Dim element = cleaned.Substring(i, j - i)
            Dim number = If(Integer.TryParse(cleaned.Substring(j, cleaned.Length - j), result), result, 1)
            Return (element, number)
        ElseIf cleaned(i) = "["c Then
            While j < cleaned.Length AndAlso cleaned(j) <> "]"c
                j += 1
            End While
            j += 1
            Dim element = cleaned.Substring(i, j - i)
            Dim number = If(Integer.TryParse(cleaned.Substring(j, cleaned.Length - j), result), result, 1)
            Return (element, number)
        Else
            Throw New ArgumentException(NameOf(token))
        End If
    End Function

    Private Sub setElementNumbers(ByVal formulaString As String, <Out> ByRef cnum As Integer, <Out> ByRef hnum As Integer, <Out> ByRef nnum As Integer, <Out> ByRef onum As Integer, <Out> ByRef pnum As Integer, <Out> ByRef snum As Integer, <Out> ByRef fnum As Integer, <Out> ByRef clnum As Integer, <Out> ByRef brnum As Integer, <Out> ByRef inum As Integer, <Out> ByRef sinum As Integer, ByVal Optional tmsCount As Integer = 0, ByVal Optional meoxCount As Integer = 0)
        Dim mc As MatchCollection

        cnum = 0
        hnum = 0
        pnum = 0
        snum = 0
        onum = 0
        nnum = 0
        fnum = 0
        clnum = 0
        brnum = 0
        inum = 0
        sinum = 0

#Region ""

        mc = Regex.Matches(formulaString, "C(?!a|d|e|l|o|r|s|u)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                cnum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, cnum)
            End If
        End If

        mc = Regex.Matches(formulaString, "H(?!e|f|g|o)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                hnum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, hnum)
            End If
        End If

        mc = Regex.Matches(formulaString, "N(?!a|b|d|e|i)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                nnum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, nnum)
            End If
        End If

        mc = Regex.Matches(formulaString, "O(?!s)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                onum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, onum)
            End If
        End If

        mc = Regex.Matches(formulaString, "S(?!b|c|e|i|m|n|r)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                snum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, snum)
            End If
        End If

        mc = Regex.Matches(formulaString, "P(?!d|t|b|r)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                pnum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, pnum)
            End If
        End If

        mc = Regex.Matches(formulaString, "Br([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                brnum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, brnum)
            End If
        End If

        mc = Regex.Matches(formulaString, "Cl([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                clnum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, clnum)
            End If
        End If

        mc = Regex.Matches(formulaString, "F(?!e)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                fnum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, fnum)
            End If
        End If

        mc = Regex.Matches(formulaString, "I(?!n|r)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                inum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, inum)
            End If
        End If

        mc = Regex.Matches(formulaString, "Si([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                sinum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, sinum)
            End If
        End If
#End Region

        cnum += tmsCount * 3 + meoxCount * 1
        hnum += tmsCount * 8 + meoxCount * 3
        nnum += meoxCount
        sinum += tmsCount
    End Sub

    Private Sub setElementNumbers(ByVal formulaString As String, <Out> ByRef cnum As Integer, <Out> ByRef hnum As Integer, <Out> ByRef nnum As Integer, <Out> ByRef onum As Integer, <Out> ByRef pnum As Integer, <Out> ByRef snum As Integer, <Out> ByRef fnum As Integer, <Out> ByRef clnum As Integer, <Out> ByRef brnum As Integer, <Out> ByRef inum As Integer, <Out> ByRef sinum As Integer, <Out> ByRef c13num As Integer, <Out> ByRef h2num As Integer, ByVal Optional tmsCount As Integer = 0, ByVal Optional meoxCount As Integer = 0)
        Dim mc As MatchCollection

        cnum = 0
        hnum = 0
        pnum = 0
        snum = 0
        onum = 0
        nnum = 0
        fnum = 0
        clnum = 0
        brnum = 0
        inum = 0
        sinum = 0
        c13num = 0
        h2num = 0

        If formulaString.Contains("D") Then
            formulaString = formulaString.Replace("D", "[2H]")
        End If

#Region ""

        mc = Regex.Matches(formulaString, "C(?!a|d|e|l|o|r|s|u)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                cnum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, cnum)
            End If
        End If

        mc = Regex.Matches(formulaString, "H(?!e|f|g|o)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                hnum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, hnum)
            End If
        End If

        mc = Regex.Matches(formulaString, "N(?!a|b|d|e|i)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                nnum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, nnum)
            End If
        End If

        mc = Regex.Matches(formulaString, "O(?!s)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                onum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, onum)
            End If
        End If

        mc = Regex.Matches(formulaString, "S(?!b|c|e|i|m|n|r)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                snum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, snum)
            End If
        End If

        mc = Regex.Matches(formulaString, "P(?!d|t|b|r)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                pnum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, pnum)
            End If
        End If

        mc = Regex.Matches(formulaString, "Br([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                brnum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, brnum)
            End If
        End If

        mc = Regex.Matches(formulaString, "Cl([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                clnum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, clnum)
            End If
        End If

        mc = Regex.Matches(formulaString, "F(?!e)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                fnum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, fnum)
            End If
        End If

        mc = Regex.Matches(formulaString, "I(?!n|r)([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                inum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, inum)
            End If
        End If

        mc = Regex.Matches(formulaString, "Si([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                sinum = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, sinum)
            End If
        End If
        mc = Regex.Matches(formulaString, "\[2H\]([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                h2num = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, h2num)
            End If
        End If
        mc = Regex.Matches(formulaString, "\[13C\]([0-9]*)", RegexOptions.None)
        If mc.Count > 0 Then
            If Equals(mc(0).Groups(1).Value, String.Empty) Then
                c13num = 1
            Else
                Integer.TryParse(mc(0).Groups(1).Value, c13num)
            End If
        End If

#End Region

        cnum += tmsCount * 3 + meoxCount * 1
        hnum += tmsCount * 8 + meoxCount * 3
        nnum += meoxCount
        sinum += tmsCount
    End Sub

End Module
