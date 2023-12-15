Public Class Composition

    Sub FindCompositions()

        Dim i As Long, j As Long, k As Long, m As Long, n As Long, q As Long, p As Long
        Dim ii As Long, jj As Long, kk As Long, mm As Long, nn As Long, qq As Long, pp As Long
        Dim lng1 As Long, lng2 As Long, lng3 As Long, lng4 As Long, lng5 As Long, lng6 As Long, lng7 As Long
        Dim lngg1 As Long, lngg2 As Long, lngg3 As Long, lngg4 As Long, lngg5 As Long, lngg6 As Long, lngg7 As Long
        Dim thing1 As Double, thing2 As Double, thing3 As Double, thing4 As Double
        Dim stng1 As String, stng2 As String
        Dim var1 As Object
        Dim rng1 As Range
        Dim outputwrite() As Object
        Dim tempcheck() As Boolean
        Dim columnover As Long, columnover2 As Long

        ThisWorkbook.Worksheets(1).Activate

        Dim Nmassin As Long, Massin() As Double
        Nmassin = 0
        For i = 1 To 10000
            If Len(Cells(i + 1, 1)) > 0 Then
                Nmassin = Nmassin + 1
            Else
                i = 10000
            End If
        Next i
        ReDim Massin(Nmassin, 3)
        For i = 1 To Nmassin
            Massin(i, 1) = Cells(i + 1, 1)
        Next i

        Dim ppmthresh As Double
        ppmthresh = Cells(2, 4)
        For i = 1 To Nmassin
            thing1 = Massin(i, 1)
            thing2 = thing1 / 1000000.0# * ppmthresh
            Massin(i, 2) = thing1 - thing2
            Massin(i, 3) = thing1 + thing2
        Next i

        Dim Monoisotopic As Boolean
        If Cells(2, 6) = "Monoisotopic" Then
            Monoisotopic = True
        Else
            Monoisotopic = False
        End If

        Dim water As Double
        If Monoisotopic Then
            water = Cells(8, 20)
        Else
            water = Cells(8, 21)
        End If

        Dim bases() As Object, Nbases As Long
        Nbases = 4
        ReDim bases(Nbases, 3)
        If Monoisotopic Then
            For i = 1 To Nbases
                bases(i, 1) = Cells(i + 2, 9)
                bases(i, 2) = Cells(i + 2, 10)
                bases(i, 3) = Cells(i + 2, 11)
            Next i
        Else
            For i = 1 To Nbases
                bases(i, 1) = Cells(i + 2, 9)
                bases(i, 2) = Cells(i + 2, 10)
                bases(i, 3) = Cells(i + 2, 12)
            Next i
        End If

        Dim mods() As Object, Nmods As Long
        Nmods = 1
        For i = 1 To 10000
            If Len(Cells(i + 1, 15)) > 0 Then
                Nmods = Nmods + 1
            Else
                i = 10000
            End If
        Next i
        k = 1
        ReDim mods(Nmods, 2)
        mods(1, 1) = ""
        mods(1, 2) = 0
        For i = 1 To 10000
            If Len(Cells(i + 1, 15)) > 0 Then
                k = k + 1
                mods(k, 1) = Cells(i + 1, 15)
                If Monoisotopic Then
                    mods(k, 2) = Cells(i + 1, 16)
                Else
                    mods(k, 2) = Cells(i + 1, 17)
                End If
            Else
                i = 10000
            End If
        Next i

        Dim lowbasemass As Double, highbasemass As Double
        lowbasemass = 1000000000.0#
        highbasemass = 0
        For i = 1 To Nbases
            thing1 = bases(i, 3)
            If thing1 > highbasemass Then highbasemass = thing1
            If thing1 < lowbasemass Then lowbasemass = thing1
        Next i

        Dim combins() As Long
        Dim topways As Long, bottomways As Long
        Dim nextindex() As Long, sumaccross() As Long
        Dim Nmatch As Long, matches() As Object, Ncats As Long

        ThisWorkbook.Worksheets(3).Activate
        Cells.Select
        Selection.ClearContents
        Selection.Font.Bold = False
        Selection.NumberFormat = "General"
        With Selection.Interior
            .Pattern = xlNone
            .TintAndShade = 0
            .PatternTintAndShade = 0
        End With
        With Selection.Borders
            .LineStyle = xlNone
        End With

        columnover2 = 0
        For k = 1 To Nmassin
            ThisWorkbook.Worksheets(2).Activate
            Cells.Select
            Selection.ClearContents
            Selection.Font.Bold = False
            Selection.NumberFormat = "General"
            With Selection.Interior
                .Pattern = xlNone
                .TintAndShade = 0
                .PatternTintAndShade = 0
            End With
            With Selection.Borders
                .LineStyle = xlNone
            End With
            columnover = 0
            Ncats = 0
            For j = 1 To Nmods
                thing1 = mods(j, 2)
                thing2 = Massin(k, 2) - thing1
                lng1 = Int(thing2 / highbasemass) + 1
                thing2 = Massin(k, 3) - thing1
                lng2 = Int(thing2 / lowbasemass) + 1
                For m = lng1 To lng2
                    topways = m + Nbases - 1
                    bottomways = m
                    lng3 = WorksheetFunction.Combin(topways, bottomways)
                    ReDim combins(lng3, 4)
                    ReDim nextindex(lng3, 4)
                    ReDim sumaccross(lng3, 4)
                    lng4 = 0
                    For n = 0 To lng3 - 1
                        lng5 = bottomways - lng4     'value
                        lng6 = bottomways - lng5     'remainder
                        lng7 = WorksheetFunction.Combin(lng6 + Nbases - 2, lng6)
                        For p = 1 To lng7
                            n = n + 1
                            combins(n, 1) = lng5
                            nextindex(n, 1) = lng7
                            sumaccross(n, 1) = combins(n, 1)
                        Next p
                        lng4 = lng4 + 1
                        n = n - 1
                    Next n
                    For q = 2 To Nbases
                        For n = 0 To lng3 - 1
                            lngg2 = sumaccross(n + 1, q - 1)
                            lngg3 = m - lngg2
                            lngg4 = 0
                            For ii = 1 To nextindex(n + 1, q - 1)
                                lngg5 = lngg3 - lngg4   'value
                                lngg6 = lngg3 - lngg5   'remainder
                                If lngg6 <= 0 Then
                                    n = n + 1
                                    combins(n, q) = lngg5
                                    nextindex(n, q) = 1
                                    sumaccross(n, q) = sumaccross(n, q - 1) + combins(n, q)
                                Else
                                    topways = lngg6 + Nbases - q - 1
                                    If topways >= 0 Then
                                        lngg7 = WorksheetFunction.Combin(topways, lngg6)
                                        For jj = 1 To lngg7
                                            n = n + 1
                                            ii = ii + 1
                                            combins(n, q) = lngg5
                                            nextindex(n, q) = lngg7
                                            sumaccross(n, q) = sumaccross(n, q - 1) + combins(n, q)
                                        Next jj
                                        ii = ii - 1
                                    Else
                                        n = n + 1
                                        combins(n, q) = lngg5
                                        nextindex(n, q) = 1
                                        sumaccross(n, q) = sumaccross(n, q - 1) + combins(n, q)
                                    End If
                                End If
                                lngg4 = lngg4 + 1
                            Next ii
                            n = n - 1
                        Next n
                    Next q
                    ReDim tempcheck(lng3)
                    Nmatch = 0
                    For n = 1 To lng3
                        thing3 = thing1 + water
                        For jj = 1 To Nbases
                            thing3 = thing3 + bases(jj, 3) * combins(n, jj)
                        Next jj
                        If thing3 <= Massin(k, 3) Then
                            If thing3 >= Massin(k, 2) Then
                                tempcheck(n) = True
                                Nmatch = Nmatch + 1
                            End If
                        End If
                    Next n
                    If Nmatch > 0 Then
                        Ncats = Ncats + 1
                        ReDim outputwrite(0 To Nmatch - 1, 0 To Nbases + 5)
                        nn = 0
                        For ii = 1 To lng3
                            If tempcheck(ii) Then
                                outputwrite(nn, 0) = Massin(k, 1)
                                thing3 = thing1 + water
                                For jj = 1 To Nbases
                                    thing3 = thing3 + bases(jj, 3) * combins(ii, jj)
                                Next jj
                                outputwrite(nn, 1) = thing3
                                outputwrite(nn, 2) = (Massin(k, 1) - thing3) / thing3 * 1000000.0#
                                lngg7 = 0
                                For kk = 1 To Nbases
                                    outputwrite(nn, kk + 2) = combins(ii, kk)
                                    lngg7 = lngg7 + combins(ii, kk)
                                Next kk
                                outputwrite(nn, Nbases + 3) = mods(j, 1)
                                outputwrite(nn, Nbases + 5) = lngg7
                                nn = nn + 1
                            End If
                        Next ii
                    Set rng1 = Range(Cells(1, columnover + 1), Cells(Nmatch, columnover + Nbases + 6))
                    rng1 = outputwrite
                        columnover = columnover + Nbases + 6
                    End If
                Next m
            Next j
            lngg3 = Nbases + 6
            For i = 1 To Ncats - 1
                lngg2 = (i - 1) * lngg3 + lngg3 + 1
                lngg1 = WorksheetFunction.Count(Columns(lngg2))
                Range(Cells(1, lngg2), Cells(lngg1, lngg2 + lngg3 - 1)).Select
                Selection.Cut
                lngg4 = WorksheetFunction.Count(Columns(1))
                Cells(lngg4 + 1, 1).Select
                ActiveSheet.Paste
            Next i
            lngg1 = WorksheetFunction.Count(Columns(1))
            If lngg1 > 0 Then
                Range(Cells(1, 1), Cells(lngg1, lngg3)).Select
                Selection.Cut
                ThisWorkbook.Worksheets(3).Activate
                Cells(2, 1 + columnover2).Select
                ActiveSheet.Paste
                ReDim outputwrite(0 To 0, 0 To lngg3 - 1)
                outputwrite(0, 0) = "Observed Mass"
                outputwrite(0, 1) = "Theoretical Mass"
                outputwrite(0, 2) = "Error (ppm)"
                For kk = 1 To Nbases
                    outputwrite(0, kk + 2) = "# of " & bases(kk, 1)
                Next kk
                outputwrite(0, Nbases + 3) = "Modification"
                outputwrite(0, Nbases + 5) = "# of Bases"
            Set rng1 = Range(Cells(1, 1 + columnover2), Cells(1, lngg3 + columnover2))
            rng1 = outputwrite
                Columns(1 + columnover2).NumberFormat = "0.000"
                Columns(2 + columnover2).NumberFormat = "0.000"
                Columns(3 + columnover2).NumberFormat = "0.0"
            
            Set rng1 = Range(Cells(2, 1 + columnover2), Cells(lngg1 + 1, lngg3 + columnover2))
            rng1.Select
                Columns(3 + columnover2).Activate
                ActiveWorkbook.Worksheets(3).Sort.SortFields.Clear
                ActiveWorkbook.Worksheets(3).Sort.SortFields.Add2 Key:=Columns(3 + columnover2) _
                , SortOn:=xlSortOnValues, Order:=xlAscending, DataOption:=xlSortNormal
            With ActiveWorkbook.Worksheets(3).Sort
                    .SetRange rng1
                .Header = xlNo
                    .MatchCase = False
                    .Orientation = xlTopToBottom
                    .SortMethod = xlPinYin
                    .Apply
                End With
            End If

            columnover2 = columnover2 + lngg3 + 1
        Next k


    End Sub
End Class