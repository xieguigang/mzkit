#Region "Microsoft.VisualBasic::d86351931470853fb79dbe7d1ef3d557, E:/mzkit/src/mzmath/Oligonucleotide_MS//Mapping_UV_Annotation/Oligonucleotide_Mapping_UV_Annotation_Tool_v11.vb"

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

    '   Total Lines: 1889
    '    Code Lines: 1611
    ' Comment Lines: 144
    '   Blank Lines: 134
    '     File Size: 84.33 KB


    ' Class Mapping_UV_Annotation
    ' 
    '     Sub: Macro2, Macro3, Macro4
    ' 
    ' /********************************************************************************/

#End Region

Public Class Mapping_UV_Annotation

    Sub Macro2()
        '
        ' Macro2 Macro
        '

        '
        Columns("K:L").Select
        Selection.Insert Shift:=xlToRight, CopyOrigin:=xlFormatFromLeftOrAbove
    Range("K1").Select
    End Sub
    Sub Macro3()
        '
        ' Macro3 Macro
        '

        '

    End Sub
    Sub Macro4()
        '
        ' Macro4 Macro
        '

        '
        Columns("K:K").Select
        Selection.Delete Shift:=xlToLeft
End Sub


    'v11 reconfigures Filing Table output to allow dynamic calculation of sequence coverage
    'v10 allow for disabling un-ID'd peak annotation
    'v8 written to address UV peak start and end times
    'v7 written to allow for "Find all the ions in a run" rather than "all the masses" in BPF. This simply means removing duplicate labels


    Option Explicit On
    Option OnBase 0

Sub IDUVPeaksMakePeptideTable()

        Dim i As Long, j As Long, k As Long, m As Long, n As Long, p As Long, q As Long
        Dim lng1 As Long, lng2 As Long, lng3 As Long, lng4 As Long
        Dim lng5 As Long, lng6 As Long, lng7 As Long, lng8 As Long, lng9 As Long
        Dim stng1 As String, stng2 As String, stng3 As String, stng4 As String, stng5 As String, stng6 As String
        Dim thing1 As Double, thing2 As Double, thing3 As Double
        Dim thing4 As Double, thing5 As Double
        Dim var1 As Object
        Dim rng1 As Range, rng2 As Range, rng3 As Range, rng4 As Range
        Dim rng5 As Range, rng6 As Range
        Dim outputwrite() As Object
        Dim tempcheck() As Boolean
        Dim check As Boolean
        Dim stngarray() As String, lngarray() As Long

        Dim tighten As Double
        Dim NBPFrows As Long, NBPFcols As Long, MaxPeak As Double, dex As Long, dex2 As Long
        Dim BPFcomps() As Object
        Dim IsExpected As Boolean, MoreThanOne As Boolean
        Dim coverage() As Boolean
        Dim UseNons As Boolean
        Dim rawfile As String, TICthresh As Double
        Dim NallowedMods As Long, AllowedMods() As Object
        Dim Noligothry As Long, Oligothry() As Object
        Dim construct() As Object
        Dim ppmthresh As Double, Confthresh As Double, Structurethresh As Double, NeedMS2 As Boolean
        Dim starttime As Double, endtime As Double, UVarea_thresh As Double
        Dim UVpeakBasePeakThresh As Double
        Dim axisfactor1 As Double, Nysegments As Long, timeD As Double
        Dim PlotH As Double, PlotB As Double, MajorUy As Double, MinorUy As Double
        Dim PPApp As Object 'As PowerPoint.Application
        Dim PPPres As Object 'As PowerPoint.Presentation
        Dim PPSlide As Object 'As PowerPoint.Slide
        Dim Shp As Object
        Dim topleftxppt As Double, topleftyppt As Double
        Dim axisfontsize As Double
        Dim xT As Double, yT As Double, offsetleft As Double, offsettop As Double
        Dim fromleft As Double, fromtop As Double
        Dim originalwidth As Double, originalheight As Double
        Dim plotwidth As Double, plotheight As Double
        Dim textboxwidth As Double, textboxheight As Double, fontsz As Double
        Dim MajorU As Double, MinorU As Double
        Dim baselineshift As Double, heightppt As Double, widthppt As Double
        Dim Npoints As Long, chromatogram() As Object
        Dim localmaxwindow As Double
        Dim Nannots As Long, annots() As Object, calltemp() As String
        Dim NtableUVrows As Long, tableUV() As Object
        Dim table2() As Object
        Dim alignment() As Double
        Dim maxUVpeak As Double
        Dim Nuvpeaks As Long, Nuvcols As Long, uvpeaks() As Object
        Dim startshift As Double, endshift As Double, shiftincrement As Double, timeshift As Double
        Dim scorebefore As Double, bestshift As Double, currentscore As Double
        Dim Ntable2cols As Long, table2header() As String
        Dim Ntable2rows As Long
        Dim morethanonestng As String
        Dim startaxistime As Long
        Dim coverageonly As Boolean
        Dim smallthing As Double, smallreport As Double
        Dim Nmissedcl As Long
        Dim NoIDannotate As Boolean

        morethanonestng = "*"
        fontsz = 10
        topleftxppt = 0.5
        topleftyppt = 1.43
        axisfontsize = 10
        textboxwidth = 12
        textboxheight = 50
        tighten = 0.9 '1.1

        'Get the construct, oligo names, and allowed modifications

        ThisWorkbook.Worksheets(3).Activate

        ReDim construct(1, 3)
        stng1 = Cells(1, 10)
        construct(1, 1) = Right(stng1, Len(stng1) - 1)
        lng1 = WorksheetFunction.CountA(Columns(10)) - 1
        stng1 = ""
        For i = 1 To lng1
            stng1 = stng1 & Cells(i + 1, 10)
        Next i
        construct(1, 2) = stng1
        construct(1, 3) = Len(stng1)


        'Previous versions:
        '    Noligothry = WorksheetFunction.CountA(Columns(1)) - 1
        '    ReDim Oligothry(Noligothry, 4)
        '    For i = 1 To Noligothry
        '        For j = 1 To 4
        '            Oligothry(i, j) = Cells(i + 1, j)
        '        Next j
        '    Next i

        Dim ConstructLetter As String
        ConstructLetter = "R"
        'cut at G
        If Right(construct(1, 2), 1) = "G" Then
            Noligothry = construct(1, 3) - Len(WorksheetFunction.Substitute(construct(1, 2), "G", ""))
            stng1 = construct(1, 2)
            ReDim Oligothry(Noligothry, 4)
            lng3 = 1
            For i = 1 To Noligothry
                lng1 = InStr(stng1, "G")
                stng2 = Left(stng1, lng1)
                stng1 = Right(stng1, Len(stng1) - lng1)
                Oligothry(i, 1) = ConstructLetter & i
                Oligothry(i, 2) = lng3
                Oligothry(i, 3) = lng3 + lng1 - 1
                Oligothry(i, 4) = stng2
                lng3 = lng3 + lng1
            Next i
        Else
            Noligothry = construct(1, 3) - Len(WorksheetFunction.Substitute(construct(1, 2), "G", "")) + 1
            stng1 = construct(1, 2) & "G"
            ReDim Oligothry(Noligothry, 4)
            lng3 = 1
            For i = 1 To Noligothry
                lng1 = InStr(stng1, "G")
                stng2 = Left(stng1, lng1)
                stng1 = Right(stng1, Len(stng1) - lng1)
                Oligothry(i, 1) = ConstructLetter & i
                Oligothry(i, 2) = lng3
                Oligothry(i, 3) = lng3 + lng1 - 1
                Oligothry(i, 4) = stng2
                lng3 = lng3 + lng1
            Next i
            Oligothry(Noligothry, 3) = construct(1, 3)
            Oligothry(Noligothry, 4) = Left(Oligothry(Noligothry, 4), Len(Oligothry(Noligothry, 4)) - 1)
        End If

        NallowedMods = WorksheetFunction.CountA(Columns(6)) - 1
        ReDim AllowedMods(NallowedMods, 3)
        For i = 1 To NallowedMods
            AllowedMods(i, 1) = Cells(i + 1, 6)
            AllowedMods(i, 2) = Cells(i + 1, 7)
            AllowedMods(i, 3) = Cells(i + 1, 8)
        Next i

        'Get the file name & thresholds

        ThisWorkbook.Worksheets(1).Activate
        smallthing = Cells(22, 12)
        smallreport = Cells(20, 12)
        baselineshift = Cells(38, 12)
        heightppt = Cells(40, 12)
        widthppt = Cells(42, 12)
        bestshift = Cells(44, 12)
        If Cells(36, 12) = "yes" Then
            coverageonly = True
        Else
            coverageonly = False
        End If
        MajorU = Cells(32, 12)
        MinorU = Cells(34, 12)
        starttime = Cells(2, 12)
        endtime = Cells(4, 12)
        thing1 = MajorU - starttime
        lng1 = Abs(Int(thing1 / MinorU))
        If thing1 > 0 Then
            startaxistime = MajorU - lng1 * MinorU
        Else
            startaxistime = MajorU + lng1 * MinorU
        End If

        ppmthresh = Cells(12, 9)
        Confthresh = Cells(12, 10)
        Structurethresh = Cells(12, 11)
        UVpeakBasePeakThresh = Cells(18, 12)
        If InStr(Cells(12, 12), "MS2") > 0 Then
            NeedMS2 = True
        Else
            NeedMS2 = False
        End If

        rawfile = Cells(2, 1)
        TICthresh = Cells(8, 12)

        If Cells(16, 12) = "yes" Then
            UseNons = True
        Else
            UseNons = False
        End If

        If Cells(24, 12) = "yes" Then
            NoIDannotate = False
        Else
            NoIDannotate = True
        End If


        'Read in BPF components

        ThisWorkbook.Worksheets(2).Activate
        lng1 = WorksheetFunction.CountA(Columns(1)) - 1
        lng2 = WorksheetFunction.CountA(Rows(1))

        'Sort the worksheet based on retention time (which is usually the default)

        Range(Cells(1, 1), Cells(lng1, lng2)).Select
        ActiveWorkbook.Worksheets(2).Sort.SortFields.Clear
        ActiveWorkbook.Worksheets(2).Sort.SortFields.Add2 Key:=
        Columns(13), SortOn:=xlSortOnValues, Order:=xlAscending, DataOption _
        :=xlSortNormal
    With ActiveWorkbook.Worksheets(2).Sort
            .SetRange Range(Cells(1, 1), Cells(lng1, lng2))
        .Header = xlYes
            .MatchCase = False
            .Orientation = xlTopToBottom
            .SortMethod = xlPinYin
            .Apply
        End With


        For i = 1 To lng2
            stng1 = Cells(1, i)
            stng2 = "MS Area:" & rawfile
            If stng1 = "MS Area" Or stng1 = stng2 Then
                dex = i
                i = lng2
            End If
        Next i

        'Determine reference chromatogram, number of samples, and their area dex

        Dim Nsamples As Long, sampledex() As Long, samplenames() As String

        Nsamples = 0
        For i = 1 To lng2
            If InStr(Cells(1, i), "MS Area:") > 0 And InStr(Cells(1, i), ".raw") > 0 Then Nsamples = Nsamples + 1
        Next i
        If Nsamples = 0 Then Nsamples = 1
        ReDim sampledex(Nsamples)
        ReDim samplenames(Nsamples)
        If Nsamples = 1 Then
            sampledex(1) = dex
            samplenames(1) = Left(rawfile, Len(rawfile) - 4)
        Else
            n = 0
            For i = 1 To lng2
                If InStr(Cells(1, i), "MS Area:") > 0 And InStr(Cells(1, i), ".raw") > 0 Then
                    n = n + 1
                    sampledex(n) = i
                    samplenames(n) = Mid(Cells(1, i), 9, Len(Cells(1, i)) - 12)
                End If
            Next i
        End If
        NBPFcols = 18 + 5 + 2 * Nsamples
        For i = 1 To lng2
            stng1 = Cells(1, i)
            'If InStr(stng1, "Oligo") > 0 Then
            If stng1 = "Oligo" Then
                dex2 = i
                i = lng2
            End If
        Next i
        MaxPeak = 0
        For i = 1 To lng1
            thing2 = Cells(i + 1, 13)
            If thing2 >= starttime - 0.5 Then
                If thing2 <= endtime + 0.5 Then
                    thing1 = Cells(i + 1, dex)
                    If MaxPeak < thing1 Then MaxPeak = thing1
                End If
            End If
        Next i
        ReDim tempcheck(lng1)
        NBPFrows = 0
        For i = 1 To lng1
            thing1 = Cells(i + 1, dex) / MaxPeak * 100
            If thing1 >= TICthresh Then
                thing2 = Cells(i + 1, 13)
                If thing2 >= starttime - 0.3 Then
                    If thing2 <= endtime + 0.3 Then
                        NBPFrows = NBPFrows + 1
                        tempcheck(i) = True
                    End If
                End If
            End If
        Next i
        ReDim BPFcomps(NBPFrows, NBPFcols)
        '1 Level | 2 No. | 3 Identification | 4 Positions | 5 Oligo Sequence | 6 Modification |
        '7 Site | 8 Delta (ppm) | 9 Confidence Score | 10 Overall Best Average Structural Resolution |
        '11 ID Type | 12 Integration Type | 13 RT | 14 M/Z | 15 Charge State | 16 Mono Mass Exp. |
        '17 Avg Mass Exp. | 18 Mono Mass Theo. | 19 Oligonucleotide | 20 Reference Peak Area | 21 Sequence |
        '22 Label | 23 % TIC Base Peak | 24 1st Sample Mass | 25 1st Sample Area | 26 2nd Sample Mass ...
        n = 0
        If Nsamples = 1 Then
            For i = 1 To lng1
                If tempcheck(i) Then
                    n = n + 1
                    For j = 1 To 18
                        BPFcomps(n, j) = Cells(i + 1, j)    'BPF annotation
                    Next j
                    BPFcomps(n, 19) = Cells(i + 1, dex2)    'BPF oligonucleotide
                    BPFcomps(n, 20) = Cells(i + 1, dex)     'Peak Area for chromatogram to be annotated
                    For j = 1 To Nsamples   'the reference data will be listed twice
                        k = (j - 1) * 2 + 24
                        BPFcomps(n, k) = Cells(i + 1, sampledex(j) - 3) 'Mono mass
                        BPFcomps(n, k + 1) = Cells(i + 1, sampledex(j)) 'Area
                    Next j
                End If
            Next i
        Else
            For i = 1 To lng1
                If tempcheck(i) Then
                    n = n + 1
                    For j = 1 To 18
                        BPFcomps(n, j) = Cells(i + 1, j)    'BPF annotation
                    Next j
                    BPFcomps(n, 19) = Cells(i + 1, dex2)    'BPF oligonucleotide
                    BPFcomps(n, 20) = Cells(i + 1, dex)     'Peak Area for chromatogram to be annotated
                    For j = 1 To Nsamples   'the reference data will be listed twice
                        k = (j - 1) * 2 + 24
                        BPFcomps(n, k) = Cells(i + 1, sampledex(j) + 5) 'Mono mass
                        BPFcomps(n, k + 1) = Cells(i + 1, sampledex(j)) 'Area
                    Next j
                End If
            Next i
        End If

        'Append to BPFcomps Oligo Sequence, All matching R#s, & Annotation field

        Nmissedcl = 20  'Version 6: changed from 3 to 20

        ReDim coverage(construct(1, 3))
        For i = 1 To NBPFrows
            If BPFcomps(i, 19) = construct(1, 1) Then
                stng1 = BPFcomps(i, 6)
                check = False

                '1st check if the call agrees w/ AllowedMods

                If Len(stng1) > 0 Then
                    For j = 1 To NallowedMods
                        If InStr(stng1, AllowedMods(j, 1)) > 0 Then
                            If AllowedMods(j, 2) = "any" Or BPFcomps(i, 7) = AllowedMods(j, 2) Then
                                stng1 = WorksheetFunction.Substitute(stng1, AllowedMods(j, 1), "")
                            End If
                        End If
                    Next j
                    stng1 = WorksheetFunction.Substitute(stng1, ",", "")
                    If Len(stng1) = 0 Then check = True
                Else
                    check = True
                End If
                If check Then
                    check = False

                    '2nd check if the call meets ppm and MS2 criteria

                    If Abs(BPFcomps(i, 8)) <= ppmthresh Then
                        If BPFcomps(i, 9) >= Confthresh Then
                            If NeedMS2 Then
                                If InStr(BPFcomps(i, 11), "Both") > 0 Or InStr(BPFcomps(i, 11), "MS2") > 0 Then
                                    If BPFcomps(i, 10) <= Structurethresh Then
                                        check = True
                                    End If
                                End If
                            Else
                                check = True
                            End If
                        End If
                    End If
                End If
                If Not UseNons Then
                    If check Then
                        check = False

                        '3rd check if non-specific

                        If InStr(BPFcomps(i, 6), "nonspecific") = 0 Then check = True
                    End If
                End If
                If check Then   'if it checks, it's worth reporting; otherwise, it's an unknown mass
                    IsExpected = False
                    stng1 = BPFcomps(i, 4)
                    lng3 = InStr(stng1, "-")
                    'lng1 = Mid(stng1, 2, lng3 - 2)
                    lng1 = Left(stng1, lng3 - 1)
                    If InStr(stng1, "|") = 0 Then
                        lng2 = Right(stng1, Len(stng1) - lng3)
                    Else
                        lng2 = Mid(stng1, lng3 + 1, InStr(stng1, "|") - lng3 - 1)
                    End If
                    'lng2 = Mid(stng1, lng3 + 1, Len(stng1) - lng3 - 1)
                    stng1 = Mid(construct(1, 2), lng1, lng2 - lng1 + 1)
                    BPFcomps(i, 21) = stng1
                    'find its R#
                    For j = 1 To Noligothry
                        If stng1 = Oligothry(j, 4) Then
                            IsExpected = True
                            stng2 = Oligothry(j, 1)
                            For p = Oligothry(j, 2) To Oligothry(j, 3)
                                coverage(p) = True
                            Next p
                            lng1 = Oligothry(j, 2)
                            lng2 = Oligothry(j, 3)
                            lng4 = j
                            GoTo foundlabel1
                        End If
                    Next j
                    'if here it's either non-specific or a missed cleavage
                    'if it's missed cleavage it must have an internal G
                    If Len(stng1) > 1 Then
                        stng3 = Left(stng1, Len(stng1) - 1)
                        'Version 5: If InStr(stng3, "G") > 1 Then   'could be missed cleavage
                        If InStr(stng3, "G") > 0 Then   'could be missed cleavage
                            For k = 1 To Nmissedcl
                                For j = 1 To Noligothry - k
                                    stng4 = Oligothry(j, 4)
                                    For m = 1 To k
                                        stng4 = stng4 & Oligothry(j + m, 4)
                                    Next m
                                    If stng1 = stng4 Then
                                        IsExpected = True
                                        stng2 = Oligothry(j, 1) & "-" & Right(Oligothry(j + k, 1), Len(Oligothry(j + k, 1)) - 1)
                                        For p = Oligothry(j, 2) To Oligothry(j + k, 3)
                                            coverage(p) = True
                                        Next p
                                        lng1 = Oligothry(j, 2)
                                        lng2 = Oligothry(j + k, 3)
                                        lng4 = j
                                        GoTo foundlabel1
                                    End If
                                Next j
                            Next k
                        End If
                    End If
                    'if it's here it's definitely a non-specific
                    For j = 1 To Noligothry
                        If lng1 = Oligothry(j, 2) Then
                            'if here, then it's a nonspecific 3' side
                            If lng2 < Oligothry(j, 3) Then
                                stng2 = Oligothry(j, 1) & "(" & lng1 & "-" & lng2 & ")"
                            Else
                                k = j + 1
                                n = 1
                                Do While lng2 > Oligothry(k, 3)
                                    n = n + 1
                                    k = k + 1
                                    If n = Noligothry Then Stop
                                Loop
                                stng2 = Oligothry(j, 1) & "-" & k - 1 & "(" & lng1 & "-" & lng2 & ")"
                            End If
                            If UseNons Then
                                For p = lng1 To lng2
                                    coverage(p) = True
                                Next p
                            End If
                            GoTo foundlabel1
                        End If
                    Next j
                    'if here, then it's a nonspecific 5' side
                    For j = 1 To Noligothry
                        If lng2 = Oligothry(j, 3) Then
                            If lng1 > Oligothry(j, 2) Then
                                stng2 = Oligothry(j, 1) & "(" & lng1 & "-" & lng2 & ")"
                            Else
                                k = j - 1
                                n = 1
                                Do While lng1 < Oligothry(k, 2)
                                    n = n + 1
                                    k = k - 1
                                    If n = Noligothry Then Stop
                                Loop
                                stng2 = Oligothry(k, 1) & "-" & j & "(" & lng1 & "-" & lng2 & ")"
                            End If
                            If UseNons Then
                                For p = lng1 To lng2
                                    coverage(p) = True
                                Next p
                            End If
                            GoTo foundlabel1
                        End If
                    Next j
                    'if here, then it's a nonspecific both sides
                    stng2 = "(" & lng1 & "-" & lng2 & ")"
                    If UseNons Then
                        For p = lng1 To lng2
                            coverage(p) = True
                        Next p
                    End If
foundlabel1:
                    'append mod label
                    stng6 = BPFcomps(i, 6)
                    If Len(stng6) > 0 Then
                        For j = 3 To NallowedMods
                            If InStr(stng6, AllowedMods(j, 1)) > 0 Then
                                stng2 = stng2 & " " & AllowedMods(j, 3)
                            End If
                        Next j
                    End If
                    If IsExpected Then  'look for repeats further down the sequence
                        MoreThanOne = False
                        stng5 = ""
                        For j = 1 To Len(stng1) - 1
                            stng5 = stng5 & "X"
                        Next j
                        lng5 = construct(1, 3) - Len(WorksheetFunction.Substitute(construct(1, 2), stng1, stng5))
                        lng6 = lng1
                        For j = 2 To lng5
                            stng3 = Right(construct(1, 2), Len(construct(1, 2)) - lng6)
                            lng1 = InStr(stng3, stng1) + lng6
                            lng2 = lng1 + Len(stng1) - 1
                            For k = lng4 + 1 To Noligothry
                                If lng1 = Oligothry(k, 2) Then
                                    For m = k To Noligothry
                                        If lng2 = Oligothry(m, 3) Then
                                            'If m = k Then                                              'Comment out b/c too many entries for small oligos
                                            '    stng2 = stng2 & ", " & Oligothry(k, 1)                 'Comment out b/c too many entries for small oligos
                                            'Else                                                       'Comment out b/c too many entries for small oligos
                                            '    stng2 = stng2 & ", " & Oligothry(k, 1) & "-" & m       'Comment out b/c too many entries for small oligos
                                            'End If                                                     'Comment out b/c too many entries for small oligos
                                            For p = lng1 To lng2
                                                coverage(p) = True
                                            Next p
                                            MoreThanOne = True
                                            lng4 = k
                                        End If
                                    Next m
                                End If
                            Next k
                            lng6 = lng1
                        Next j
                        If MoreThanOne Then
                            stng2 = stng2 & morethanonestng
                        End If
                    End If
                Else
                    stng2 = "m" & Round(BPFcomps(i, 16), 4)
                End If
            Else
                If BPFcomps(i, 16) = 0 Then
                    stng2 = "a" & Round(BPFcomps(i, 17), 3)
                Else
                    stng2 = "m" & Round(BPFcomps(i, 16), 4)
                End If
            End If
            BPFcomps(i, 22) = stng2
            BPFcomps(i, 23) = BPFcomps(i, 20) / MaxPeak * 100
        Next i

        'Report BPFcomps table

        lng1 = NBPFrows
        lng2 = NBPFcols
        ReDim outputwrite(0 To lng1, 0 To lng2 - 1)
        For i = 1 To 18
            outputwrite(0, i - 1) = Cells(1, i)
        Next i
        outputwrite(0, 18) = "Construct"
        outputwrite(0, 19) = "Peak Area"
        outputwrite(0, 20) = "Sequence"
        outputwrite(0, 21) = "Label"
        outputwrite(0, 22) = "% TIC Base Peak Intensity"
        For i = 1 To Nsamples
            j = (i - 1) * 2 + 23
            outputwrite(0, j) = samplenames(i) & " Mono Mass"
            outputwrite(0, j + 1) = samplenames(i) & " Peak Area"
        Next i
        For i = 1 To lng1
            For j = 1 To lng2
                outputwrite(i, j - 1) = BPFcomps(i, j)
            Next j
        Next i
        ThisWorkbook.Worksheets(6).Activate
        Cells.Select
        Selection.ClearContents
        Selection.Font.Bold = False
        With Selection.Interior
            .Pattern = xlNone
            .TintAndShade = 0
            .PatternTintAndShade = 0
        End With
        With Selection.Borders
            .LineStyle = xlNone
        End With
    Set rng1 = Range(Cells(1, 1), Cells(lng1 + 1, lng2))
    rng1 = outputwrite

        Ntable2cols = 7 + NBPFcols
        ReDim table2header(Ntable2cols)
        table2header(1) = "UV Apex RT"
        table2header(2) = "UV Start RT"
        table2header(3) = "UV End RT"
        table2header(4) = "UV Area"
        table2header(5) = "Match?"
        table2header(6) = "UV Peak Index"
        table2header(7) = "% Base Peak in UV peak"
        For i = 1 To NBPFcols
            table2header(i + 7) = outputwrite(0, i - 1)
        Next i

        'Sequence Coverage

        stng1 = construct(1, 2)
        Dim covwidth As Long
        Dim coverage2() As Long
        covwidth = 70
        lng1 = construct(1, 3)
        ReDim coverage2(lng1, 2)
        lng2 = Int(lng1 / covwidth - 0.01) + 1
        ReDim outputwrite(0 To lng2 - 1, 0 To covwidth + 1)
        n = 0
        For i = 1 To lng2
            outputwrite(i - 1, 0) = (i - 1) * covwidth + 1
            outputwrite(i - 1, covwidth + 1) = outputwrite(i - 1, 0) + covwidth - 1
            For j = 1 To covwidth
                n = n + 1
                If n <= lng1 Then
                    outputwrite(i - 1, j) = Mid(stng1, n, 1)
                    coverage2(n, 1) = i
                    coverage2(n, 2) = j + 1
                End If
            Next j
        Next i
        outputwrite(lng2 - 1, covwidth + 1) = lng1
        ThisWorkbook.Worksheets(9).Activate
        Cells.Select
        Selection.ClearContents
        Selection.Font.Bold = False
        With Selection.Interior
            .Pattern = xlNone
            .TintAndShade = 0
            .PatternTintAndShade = 0
        End With
        With Selection.Borders
            .LineStyle = xlNone
        End With
    Set rng1 = Range(Cells(1, 1), Cells(lng2, covwidth + 2))
    rng1 = outputwrite
        thing1 = 0
        For i = 1 To lng1
            If coverage(i) Then
                thing1 = thing1 + 1
                Cells(coverage2(i, 1), coverage2(i, 2)).Interior.Color = RGB(140, 140, 140)
            End If
        Next i
        thing1 = thing1 / lng1 * 100
        Cells(1, covwidth + 5) = "% coverage by features between " & starttime & " and " & endtime & " min"
        Cells(2, covwidth + 5) = thing1

        Dim NFilingMassTableRows As Long, FilingMassTable() As Object
        NFilingMassTableRows = Noligothry
        For i = 1 To NBPFrows
            stng1 = BPFcomps(i, 22)
            If Left(stng1, 1) = ConstructLetter Then
                lng3 = 0
                stng1 = "G" & BPFcomps(i, 21)
                stng2 = "G" & construct(1, 2)
                lng1 = InStr(stng2, stng1)
                lng2 = lng1 + lng3
                lng4 = lng2 + Len(stng1) - 2
                Do While lng1 > 0
                    For j = 1 To Noligothry
                        If lng2 = Oligothry(j, 2) Then
                            If lng4 = Oligothry(j, 3) Then
                                If BPFcomps(i, 6) <> "3'p" Then
                                    NFilingMassTableRows = NFilingMassTableRows + 1
                                End If
                            Else
                                NFilingMassTableRows = NFilingMassTableRows + 1
                            End If
                            j = Noligothry
                        Else
                            If lng2 < Oligothry(j, 2) Then
                                NFilingMassTableRows = NFilingMassTableRows + 1
                                j = Noligothry
                            End If
                        End If
                    Next j
                    lng3 = lng2
                    stng2 = Right(stng2, Len(stng2) - lng1)
                    lng1 = InStr(stng2, stng1)
                    lng2 = lng1 + lng3
                    lng4 = lng2 + Len(stng1) - 2
                Loop
            End If
        Next i

        Dim FilingMassTableColumns As Long
        FilingMassTableColumns = 12 + Nsamples
        ReDim FilingMassTable(NFilingMassTableRows, FilingMassTableColumns)

        '1              Oligonucleotide
        '2              Residues
        '3              Sequence
        '4              Theoretical Mass
        '5              RT (min)
        '6              Annotated in UV?
        '7              1st Occurance Start Position
        '8              Length
        '9              # of Occurances
        '10             1st sample mass
        '10+1           2nd sample mass..
        '10+Nsamples    Sort
        '11+Nsamples    BPF index
        '12+Nsamples    Peak area of 1st sample

        ThisWorkbook.Worksheets(3).Activate
        Dim bases() As Object
        ReDim bases(4, 2)
        For i = 1 To 4
            For j = 1 To 2
                bases(i, j) = Cells(i + 2, j + 12)
            Next j
        Next i
        Dim water As Double, fivecap As Double
        fivecap = Cells(7, 14)
        water = Cells(8, 14)

        For i = 1 To Noligothry
            FilingMassTable(i, 1) = Oligothry(i, 1)                                         'R#
            FilingMassTable(i, 2) = "[" & Oligothry(i, 2) & "-" & Oligothry(i, 3) & "]"     '[Start-End]
            FilingMassTable(i, 3) = Oligothry(i, 4)                                         'Sequence
            FilingMassTable(i, 10 + Nsamples) = Oligothry(i, 2) * 10000 + (Oligothry(i, 3) - Oligothry(i, 2) + 1)            'Sort value
            stng1 = Oligothry(i, 4)
            lng1 = Len(stng1)
            thing1 = water
            For j = 1 To lng1
                stng2 = Mid(stng1, j, 1)
                For k = 1 To 4
                    If stng2 = bases(k, 1) Then
                        thing1 = thing1 + bases(k, 2)
                        k = 4
                    End If
                Next k
            Next j
            FilingMassTable(i, 4) = thing1
        Next i
        FilingMassTable(1, 4) = FilingMassTable(1, 4) + fivecap
        '1 Level | 2 No. | 3 Identification | 4 Positions | 5 Oligo Sequence | 6 Modification |
        '7 Site | 8 Delta (ppm) | 9 Confidence Score | 10 Overall Best Average Structural Resolution |
        '11 ID Type | 12 Integration Type | 13 RT | 14 M/Z | 15 Charge State | 16 Mono Mass Exp. |
        '17 Avg Mass Exp. | 18 Mono Mass Theo. | 19 Oligonucleotide | 20 Reference Peak Area | 21 Sequence |
        '22 Label | 23 % TIC Base Peak | 24 1st Sample Mass | 25 1st Sample Area | 26 2nd Sample Mass ...
        Dim thang1 As Double
        n = Noligothry
        For i = 1 To NBPFrows
            stng1 = BPFcomps(i, 22)
            If BPFcomps(i, 23) = 0 Then
                thang1 = 9000000000.0#
            Else
                thang1 = BPFcomps(i, 23)
            End If
            If Left(stng1, 1) = ConstructLetter Then
                lng3 = 0
                stng1 = "G" & BPFcomps(i, 21)
                stng2 = "G" & construct(1, 2)
                lng1 = InStr(stng2, stng1)
                lng2 = lng1 + lng3
                lng4 = lng2 + Len(stng1) - 2
                Do While lng1 > 0
                    For j = 1 To Noligothry
                        If lng2 = Oligothry(j, 2) Then
                            If lng4 = Oligothry(j, 3) Then
                                If BPFcomps(i, 6) <> "3'p" Then
                                    n = n + 1
                                    FilingMassTable(n, 1) = Oligothry(j, 1)                                         'R#
                                    For q = 3 To NallowedMods
                                        If InStr(BPFcomps(i, 6), AllowedMods(q, 1)) > 0 Then
                                            FilingMassTable(n, 1) = FilingMassTable(n, 1) & " " & AllowedMods(q, 3)
                                        End If
                                    Next q
                                    FilingMassTable(n, 2) = "[" & lng2 & "-" & lng4 & "]"                           '[Start-End]
                                    FilingMassTable(n, 3) = Oligothry(j, 4)                                         'Sequence
                                    FilingMassTable(n, 4) = BPFcomps(i, 18)                                         'Theoretical Mass
                                    FilingMassTable(n, 5) = BPFcomps(i, 13)                                         'RT
                                    'FilingMassTable(n, 6) = "Self"                                                  'Observed as
                                    For m = 1 To Nsamples
                                        p = (m - 1) * 2 + 24
                                        FilingMassTable(n, 9 + m) = BPFcomps(i, p)                                  'Observed Mass
                                    Next m
                                    FilingMassTable(n, 10 + Nsamples) = lng2 * 10000 + (lng4 - lng2 + 1) + 1 / thang1    'Sort value
                                    FilingMassTable(n, 11 + Nsamples) = BPFcomps(i, 2)                               'BPFcomps component #
                                Else
                                    If Len(FilingMassTable(j, 1)) > 0 Then
                                        If BPFcomps(i, 25) > FilingMassTable(j, 12 + Nsamples) Then
                                            FilingMassTable(j, 1) = Oligothry(j, 1)                                         'R#
                                            FilingMassTable(j, 2) = "[" & lng2 & "-" & lng4 & "]"                           '[Start-End]
                                            FilingMassTable(j, 3) = Oligothry(j, 4)                                         'Sequence
                                            'FilingMassTable(j, 4) = BPFcomps(i, 18)                                         'Theoretical Mass
                                            FilingMassTable(j, 5) = BPFcomps(i, 13)                                         'RT
                                            'FilingMassTable(j, 6) = "Self"                                                  'Observed as
                                            For m = 1 To Nsamples
                                                p = (m - 1) * 2 + 24
                                                FilingMassTable(j, 9 + m) = BPFcomps(i, p)                                  'Observed Mass
                                            Next m
                                            FilingMassTable(j, 10 + Nsamples) = lng2 * 10000 + (lng4 - lng2 + 1) + 1 / thang1    'Sort value
                                            FilingMassTable(j, 11 + Nsamples) = BPFcomps(i, 2)                               'BPFcomps component #
                                            FilingMassTable(j, 12 + Nsamples) = BPFcomps(i, 25)
                                        End If
                                    Else
                                        FilingMassTable(j, 1) = Oligothry(j, 1)                                         'R#
                                        FilingMassTable(j, 2) = "[" & lng2 & "-" & lng4 & "]"                           '[Start-End]
                                        FilingMassTable(j, 3) = Oligothry(j, 4)                                         'Sequence
                                        'FilingMassTable(j, 4) = BPFcomps(i, 18)                                         'Theoretical Mass
                                        FilingMassTable(j, 5) = BPFcomps(i, 13)                                         'RT
                                        'FilingMassTable(j, 6) = "Self"                                                  'Observed as
                                        For m = 1 To Nsamples
                                            p = (m - 1) * 2 + 24
                                            FilingMassTable(j, 7 + m) = BPFcomps(i, p)                                  'Observed Mass
                                        Next m
                                        FilingMassTable(j, 8 + Nsamples) = lng2 * 10000 + (lng4 - lng2 + 1) + 1 / thang1    'Sort value
                                        FilingMassTable(j, 9 + Nsamples) = BPFcomps(i, 2)                               'BPFcomps component #
                                        FilingMassTable(j, 12 + Nsamples) = BPFcomps(i, 25)
                                    End If

                                End If
                            Else
                                n = n + 1
                                For k = j + 1 To Noligothry
                                    If lng4 = Oligothry(k, 3) Then
                                        check = True
                                        FilingMassTable(n, 1) = Oligothry(j, 1) & "-" & Right(Oligothry(k, 1), Len(Oligothry(k, 1)) - 1)
                                        For q = 3 To NallowedMods
                                            If InStr(BPFcomps(i, 6), AllowedMods(q, 1)) > 0 Then
                                                FilingMassTable(n, 1) = FilingMassTable(n, 1) & " " & AllowedMods(q, 3)
                                            End If
                                        Next q
                                        FilingMassTable(n, 2) = "[" & lng2 & "-" & lng4 & "]"                           '[Start-End]
                                        FilingMassTable(n, 3) = Mid(construct(1, 2), Oligothry(j, 2), Oligothry(k, 3) - Oligothry(j, 2) + 1)
                                        FilingMassTable(n, 4) = BPFcomps(i, 18)                                         'Theoretical Mass
                                        FilingMassTable(n, 5) = BPFcomps(i, 13)                                         'RT
                                        'FilingMassTable(n, 6) = "Self"                                                  'Observed as
                                        For m = 1 To Nsamples
                                            p = (m - 1) * 2 + 24
                                            FilingMassTable(n, 9 + m) = BPFcomps(i, p)                                  'Observed Mass
                                        Next m
                                        FilingMassTable(n, 10 + Nsamples) = lng2 * 10000 + (lng4 - lng2 + 1) + 1 / thang1    'Sort value
                                        FilingMassTable(n, 11 + Nsamples) = BPFcomps(i, 2)                               'BPFcomps component #
                                        k = Noligothry
                                    Else
                                        If lng4 < Oligothry(k, 2) Then  '3' non-specific
                                            FilingMassTable(n, 1) = Oligothry(j, 1) & "-" & Right(Oligothry(k, 1), Len(Oligothry(k, 1)) - 1) & "(" & lng2 & "-" & lng4 & ")"
                                            For q = 3 To NallowedMods
                                                If InStr(BPFcomps(i, 6), AllowedMods(q, 1)) > 0 Then
                                                    FilingMassTable(n, 1) = FilingMassTable(n, 1) & " " & AllowedMods(q, 3)
                                                End If
                                            Next q
                                            FilingMassTable(n, 2) = "[" & lng2 & "-" & lng4 & "]"                           '[Start-End]
                                            FilingMassTable(n, 3) = Mid(construct(1, 2), lng2, lng4 - lng2 + 1)
                                            FilingMassTable(n, 4) = BPFcomps(i, 18)                                         'Theoretical Mass
                                            FilingMassTable(n, 5) = BPFcomps(i, 13)                                         'RT
                                            'FilingMassTable(n, 6) = "Self"                                                  'Observed as
                                            For m = 1 To Nsamples
                                                p = (m - 1) * 2 + 24
                                                FilingMassTable(n, 9 + m) = BPFcomps(i, p)                                  'Observed Mass
                                            Next m
                                            FilingMassTable(n, 10 + Nsamples) = lng2 * 10000 + (lng4 - lng2 + 1) + 1 / thang1   'Sort value
                                            FilingMassTable(n, 11 + Nsamples) = BPFcomps(i, 2)                               'BPFcomps component #
                                            k = Noligothry
                                        End If
                                    End If
                                Next k
                            End If
                            j = Noligothry
                        Else
                            If lng2 < Oligothry(j, 2) Then  '5' non-specific
                                n = n + 1
                                If lng4 = Oligothry(j, 3) Then
                                    FilingMassTable(n, 1) = Oligothry(j, 1) & "(" & lng2 & "-" & lng4 & ")"
                                    For q = 3 To NallowedMods
                                        If InStr(BPFcomps(i, 6), AllowedMods(q, 1)) > 0 Then
                                            FilingMassTable(n, 1) = FilingMassTable(n, 1) & " " & AllowedMods(q, 3)
                                        End If
                                    Next q
                                    FilingMassTable(n, 2) = "[" & lng2 & "-" & lng4 & "]"                           '[Start-End]
                                    FilingMassTable(n, 3) = Mid(construct(1, 2), lng2, lng4 - lng2 + 1)
                                    FilingMassTable(n, 4) = BPFcomps(i, 18)                                         'Theoretical Mass
                                    FilingMassTable(n, 5) = BPFcomps(i, 13)                                         'RT
                                    'FilingMassTable(n, 6) = "Self"                                                  'Observed as
                                    For m = 1 To Nsamples
                                        p = (m - 1) * 2 + 24
                                        FilingMassTable(n, 9 + m) = BPFcomps(i, p)                                  'Observed Mass
                                    Next m
                                    FilingMassTable(n, 10 + Nsamples) = lng2 * 10000 + (lng4 - lng2 + 1) + 1 / thang1    'Sort value
                                    FilingMassTable(n, 11 + Nsamples) = BPFcomps(i, 2)                               'BPFcomps component #
                                    k = Noligothry
                                Else
                                    For k = j To Noligothry
                                        If lng4 = Oligothry(k, 3) Then
                                            FilingMassTable(n, 1) = Oligothry(j, 1) & "-" & Right(Oligothry(k, 1), Len(Oligothry(k, 1)) - 1) & "(" & lng2 & "-" & lng4 & ")"
                                            For q = 3 To NallowedMods
                                                If InStr(BPFcomps(i, 6), AllowedMods(q, 1)) > 0 Then
                                                    FilingMassTable(n, 1) = FilingMassTable(n, 1) & " " & AllowedMods(q, 3)
                                                End If
                                            Next q
                                            FilingMassTable(n, 2) = "[" & lng2 & "-" & lng4 & "]"                           '[Start-End]
                                            FilingMassTable(n, 3) = Mid(construct(1, 2), lng2, lng4 - lng2 + 1)
                                            FilingMassTable(n, 4) = BPFcomps(i, 18)                                         'Theoretical Mass
                                            FilingMassTable(n, 5) = BPFcomps(i, 13)                                         'RT
                                            'FilingMassTable(n, 6) = "Self"                                                  'Observed as
                                            For m = 1 To Nsamples
                                                p = (m - 1) * 2 + 24
                                                FilingMassTable(n, 9 + m) = BPFcomps(i, p)                                  'Observed Mass
                                            Next m
                                            FilingMassTable(n, 10 + Nsamples) = lng2 * 10000 + (lng4 - lng2 + 1) + 1 / thang1  'Sort value
                                            FilingMassTable(n, 11 + Nsamples) = BPFcomps(i, 2)                               'BPFcomps component #
                                            k = Noligothry
                                        End If
                                    Next k
                                End If
                                j = Noligothry
                            End If
                        End If
                    Next j
                    lng3 = lng2
                    stng2 = Right(stng2, Len(stng2) - lng1)
                    lng1 = InStr(stng2, stng1)
                    lng2 = lng1 + lng3
                    lng4 = lng2 + Len(stng1) - 2
                Loop
            End If
        Next i

        ThisWorkbook.Worksheets(10).Activate
        Cells.Select
        Selection.ClearContents
        Selection.Font.Bold = False
        With Selection.Interior
            .Pattern = xlNone
            .TintAndShade = 0
            .PatternTintAndShade = 0
        End With
        With Selection.Borders
            .LineStyle = xlNone
        End With
        lng1 = NFilingMassTableRows
        lng2 = FilingMassTableColumns
        ReDim outputwrite(0 To lng1, 0 To lng2 - 1)
        For i = 1 To lng1
            For j = 1 To lng2
                outputwrite(i, j - 1) = FilingMassTable(i, j)
            Next j
        Next i
    Set rng1 = Range(Cells(1, 1), Cells(lng1 + 1, lng2))
    rng1 = outputwrite
        ThisWorkbook.Worksheets(10).Sort.SortFields.Clear
        ThisWorkbook.Worksheets(10).Sort.SortFields.Add Key:=Range(Cells(1, Nsamples + 10), Cells(lng1 + 1, Nsamples + 10)),
        SortOn:=xlSortOnValues, Order:=xlAscending, DataOption:=xlSortNormal
    With ThisWorkbook.Worksheets(10).Sort
            .SetRange Range(Cells(1, 1), Cells(lng1 + 1, lng2))
        .Header = xlYes
            .MatchCase = False
            .Orientation = xlTopToBottom
            .SortMethod = xlPinYin
            .Apply
        End With

        'Remove duplicates owing to multiple charge state entries and sugar phosphate isomers

        NFilingMassTableRows = 0
        ReDim tempcheck(lng1)
        For i = 1 To lng1
            If Not tempcheck(i) Then
                stng1 = Cells(i + 1, 1)
                NFilingMassTableRows = NFilingMassTableRows + 1
                For j = i + 1 To lng1
                    If Not tempcheck(j) Then
                        If Cells(j + 1, 1) = stng1 Then
                            tempcheck(j) = True
                        End If
                    End If
                Next j
            End If
        Next i
        'lng1 = NFilingMassTableRows
        lng2 = 11 + Nsamples
        ReDim outputwrite(0 To NFilingMassTableRows, 0 To lng2 - 1)
        outputwrite(0, 0) = "Oligonucleotide"
        outputwrite(0, 1) = "Residues"
        outputwrite(0, 2) = "Sequence"
        outputwrite(0, 3) = "Theoretical Mass"
        outputwrite(0, 4) = "RT (min)"
        outputwrite(0, 5) = "Annotated in UV?"
        outputwrite(0, 6) = "1st Occurance Start Position"
        outputwrite(0, 7) = "Length"
        outputwrite(0, 8) = "# of Occurances"
        outputwrite(0, Nsamples + 9) = "Sort"
        outputwrite(0, Nsamples + 10) = "BPF index"
        For i = 1 To Nsamples
            outputwrite(0, i + 8) = samplenames(i) & " Observed Mass"
        Next i
        n = 0
        For i = 1 To lng1
            If Not tempcheck(i) Then
                n = n + 1
                For j = 1 To lng2
                    outputwrite(n, j - 1) = Cells(i + 1, j)
                Next j
            End If
        Next i
        Cells.Select
        Selection.ClearContents
        Selection.Font.Bold = False
        With Selection.Interior
            .Pattern = xlNone
            .TintAndShade = 0
            .PatternTintAndShade = 0
        End With
        With Selection.Borders
            .LineStyle = xlNone
        End With
    Set rng1 = Range(Cells(1, 1), Cells(NFilingMassTableRows + 1, lng2))
    rng1 = outputwrite


        Cells(2, 7).Select
        ActiveCell.FormulaR1C1 = "=VALUE(MID(RC[-5],2,FIND(""-"",RC[-5])-2))"
        Cells(2, 8).Select
        ActiveCell.FormulaR1C1 = "=LEN(RC[-5])"
        Cells(2, 9).Select
        ActiveCell.FormulaR1C1 = "=COUNTIF(C[-6],RC[-6])"
        Range(Cells(2, 7), Cells(2, 9)).Select
    Set rng1 = Range(Cells(2, 7), Cells(NFilingMassTableRows + 1, 9))
    Selection.AutoFill Destination:=rng1, Type:=xlFillDefault

    Columns("K:N").Select
        Selection.Insert Shift:=xlToRight, CopyOrigin:=xlFormatFromLeftOrAbove

    Cells(1, 11) = construct(1, 2)
        Cells(2, 11).Select
        ActiveCell.FormulaR1C1 = "=IF(LEN(RC[-1])>0,CONCATENATE(LEFT(R[-1]C,RC[-4]-1),REPT(""X"",RC[-3]),RIGHT(R[-1]C,LEN(R[-1]C)-RC[-4]-RC[-3]+1)),R[-1]C)"
    Set rng1 = Range(Cells(2, 11), Cells(NFilingMassTableRows + 1, 11))
    Selection.AutoFill Destination:=rng1, Type:=xlFillDefault


    Cells(3, 13) = "# NT"
        Cells(4, 13) = "# Covered"
        Cells(5, 13) = "% Covered"
        Cells(3, 14).Select
        ActiveCell.FormulaR1C1 = "=LEN(R[-2]C[-3])"
        Cells(4, 14).Select
        ActiveCell.FormulaR1C1 = "=R[-1]C - LEN(SUBSTITUTE(INDIRECT(ADDRESS(COUNTA(C[-3]),COLUMN(RC[-3]))),""X"",""""))"
        Cells(5, 14).Select
        ActiveCell.FormulaR1C1 = "=R[-1]C/R[-2]C*100"


        Columns("K:N").Select
        Selection.Cut
        Cells(1, FilingMassTableColumns + 4).Select
        ActiveSheet.Paste
        Columns("K:N").Select
        Selection.Delete Shift:=xlToLeft

    Cells.HorizontalAlignment = xlCenter
        Cells.VerticalAlignment = xlCenter
        Cells.NumberFormat = "General"
        Cells.IndentLevel = 0
        Cells.ColumnWidth = 11
        Rows(1).Font.Bold = True
        Rows(1).WrapText = True
        Cells(1, FilingMassTableColumns).WrapText = False
        Cells(1, FilingMassTableColumns).Font.Bold = False
        Columns(3).HorizontalAlignment = xlLeft
        Columns(3).ColumnWidth = 14
        Columns(4).NumberFormat = "0.0000"
        Columns(4).HorizontalAlignment = xlRight
        Columns(4).IndentLevel = 1
        Columns(4).ColumnWidth = 12
        Columns(5).NumberFormat = "0.00"
        Columns(5).HorizontalAlignment = xlRight
        Columns(5).IndentLevel = 1
        Columns(5).ColumnWidth = 12
        For i = 1 To Nsamples
            Columns(i + 9).NumberFormat = "0.0000"
            Columns(i + 9).HorizontalAlignment = xlRight
            Columns(i + 9).IndentLevel = 1
        Next i
        Columns(FilingMassTableColumns).HorizontalAlignment = xlRight

        If coverageonly Then End

        ReDim lngarray(NFilingMassTableRows)
        For i = 1 To NFilingMassTableRows
            lngarray(i) = Cells(i + 1, lng2)
        Next i



        'Get UV Peaks----------------------------------------------------------------------------

        ThisWorkbook.Worksheets(1).Activate
        UVarea_thresh = Cells(6, 12)
        lng1 = Intersect(ActiveSheet.UsedRange, Columns(1)).Count - 5
        maxUVpeak = 0
        For i = 1 To lng1
            thing1 = Cells(i + 5, 1)
            If thing1 >= starttime Then
                If thing1 <= endtime Then
                    If Cells(i + 5, 4) > maxUVpeak Then maxUVpeak = Cells(i + 5, 4)
                End If
            End If
        Next i

        Nuvpeaks = 0
        Nuvcols = 8
        'Apex RT Start RT    End RT  Area StartRTadjusted EndRTadjusted RelArea SmallPeak?
        ReDim tempcheck(lng1)
        For i = 1 To lng1
            thing1 = Cells(i + 5, 1)
            If thing1 >= starttime Then
                If thing1 <= endtime Then
                    thing2 = Cells(i + 5, 4) / maxUVpeak * 100
                    If thing2 >= UVarea_thresh Then
                        Nuvpeaks = Nuvpeaks + 1
                        tempcheck(i) = True
                    End If
                End If
            End If
        Next i
        ReDim uvpeaks(Nuvpeaks, Nuvcols)
        n = 0

        'v8 change....
        tighten = 15    'Distance in sec from the apex of the UV peak an MS peak can be to be associated with it
        tighten = tighten / 60  'converted to min
        '....

        For i = 1 To lng1
            If tempcheck(i) Then
                n = n + 1
                For j = 1 To 4
                    uvpeaks(n, j) = Cells(i + 5, j)
                Next j
                'thing1 = uvpeaks(n, 1) - uvpeaks(n, 2)
                'uvpeaks(n, 5) = uvpeaks(n, 1) - thing1 * tighten
                'thing1 = uvpeaks(n, 3) - uvpeaks(n, 1)
                'uvpeaks(n, 6) = uvpeaks(n, 1) + thing1 * tighten
                uvpeaks(n, 5) = uvpeaks(n, 1) - tighten
                uvpeaks(n, 6) = uvpeaks(n, 1) + tighten
                uvpeaks(n, 7) = uvpeaks(n, 4) / maxUVpeak * 100
                If uvpeaks(n, 7) < smallthing Then
                    uvpeaks(n, 8) = True
                Else
                    uvpeaks(n, 8) = False
                End If
                n = n
            End If
        Next i

        'v8 change....
        'In case tighten > gap between two peaks (for example w/ a shoulder)
        For i = 2 To Nuvpeaks
            If uvpeaks(i - 1, 6) > uvpeaks(i, 5) Then
                thing1 = uvpeaks(i, 1) - uvpeaks(i - 1, 1)
                thing1 = thing1 / 2
                uvpeaks(i, 5) = uvpeaks(i, 1) - thing1
                uvpeaks(i - 1, 6) = uvpeaks(i - 1, 1) + thing1
            End If
        Next i
        '....

        ReDim tempcheck(1)

        'Determine the theoretical UV peak height as a product of oligo length and MS area-----

        thing1 = 0
        ReDim alignment(NBPFrows, 1)
        For i = 1 To NBPFrows
            If Len(BPFcomps(i, 21)) > 0 Then
                alignment(i, 1) = Len(BPFcomps(i, 21)) * BPFcomps(i, 20)
                If thing1 < alignment(i, 1) Then thing1 = alignment(i, 1)
            End If
        Next i
        For i = 1 To NBPFrows
            alignment(i, 1) = alignment(i, 1) / thing1 * 100
        Next i

        'Find the best peak alignment timeshift--------------------------------------------------

        startshift = -5 'minutes
        endshift = 5    'minutes
        shiftincrement = 0.01
        timeshift = startshift
        lng1 = Int((endshift - startshift) / shiftincrement) + 1
        scorebefore = 0
        '    bestshift = startshift
        '    For n = 1 To lng1
        '        currentscore = 0
        '        For i = 1 To NBPFrows
        '            If alignment(i, 1) > 0 Then
        '                thing1 = BPFcomps(i, 13) + timeshift    'RT
        '                thing2 = alignment(i, 1)                'Theoretical UV Peak Height
        '                For j = 1 To Nuvpeaks
        '                     If thing1 >= uvpeaks(j, 5) Then    'Start RT adjusted
        '                        If thing1 < uvpeaks(j, 6) Then  'End RT adjusted
        '                            currentscore = currentscore + thing2 * uvpeaks(j, 7)    'Area scaled
        '                            j = Nuvpeaks
        '                        End If
        '                    End If
        '                Next j
        '            End If
        '        Next i
        '        If currentscore > scorebefore Then
        '            scorebefore = currentscore
        '            bestshift = timeshift
        '        End If
        '        timeshift = timeshift + shiftincrement
        '    Next n


        'Match mass peaks to UV peaks------------------------------------------------------------

        Ntable2rows = 0
        For i = 1 To NBPFrows
            thing1 = BPFcomps(i, 13) + bestshift
            For j = 1 To Nuvpeaks
                If thing1 >= uvpeaks(j, 5) Then
                    If thing1 < uvpeaks(j, 6) Then
                        Ntable2rows = Ntable2rows + 1
                        j = Nuvpeaks
                    End If
                End If
            Next j
        Next i

        ReDim outputwrite(0 To NBPFrows, 0 To 0)
        ReDim table2(Ntable2rows, Ntable2cols)
        n = 0
        For i = 1 To NBPFrows
            thing1 = BPFcomps(i, 13) + bestshift
            For j = 1 To Nuvpeaks
                If thing1 >= uvpeaks(j, 5) Then
                    If thing1 < uvpeaks(j, 6) Then
                        n = n + 1
                        For k = 1 To 4
                            table2(n, k) = uvpeaks(j, k)
                        Next
                        If Len(BPFcomps(i, 21)) > 0 Then
                            table2(n, 5) = True
                            table2(n, 6) = j              'UV Peak Index
                            table2(n, 7) = table2(n, 1) * 10000 - BPFcomps(i, 23) 'Sort ID
                            For k = 1 To NBPFcols
                                table2(n, k + 7) = BPFcomps(i, k)
                            Next k
                            outputwrite(i, 0) = uvpeaks(j, 1)
                            j = Nuvpeaks
                        Else
                            table2(n, 5) = False
                            table2(n, 6) = j              'UV Peak Index
                            table2(n, 7) = table2(n, 1) * 10000 - BPFcomps(i, 23) 'Sort ID
                            For k = 1 To 2
                                table2(n, k + 7) = BPFcomps(i, k)
                            Next k
                            For k = 11 To 17
                                table2(n, k + 7) = BPFcomps(i, k)
                            Next k
                            table2(n, 20 + 7) = BPFcomps(i, 20)
                            table2(n, 22 + 7) = BPFcomps(i, 22)
                            outputwrite(i, 0) = uvpeaks(j, 1)
                            j = Nuvpeaks
                        End If
                    End If
                End If
            Next j
        Next i

        ThisWorkbook.Worksheets(6).Activate
        outputwrite(0, 0) = "UV Peak (min)"
    Set rng1 = Range(Cells(1, NBPFcols + 1), Cells(NBPFrows + 1, NBPFcols + 1))
    rng1 = outputwrite
        Columns(8).NumberFormat = "0.0"
        Columns(13).NumberFormat = "0.00"
        Columns(14).NumberFormat = "0.0000"
        Columns(15).NumberFormat = "0"
        Columns(16).NumberFormat = "0.0000"
        Columns(17).NumberFormat = "0.000"
        Columns(18).NumberFormat = "0.0000"
        Columns(20).NumberFormat = "0.00E+00"
        Columns(23).NumberFormat = "0.00"
        Columns(24).NumberFormat = "0.00"


        '1 Level | 2 No. | 3 Identification | 4 Positions | 5 Oligo Sequence | 6 Modification |
        '7 Site | 8 Delta (ppm) | 9 Confidence Score | 10 Overall Best Average Structural Resolution |
        '11 ID Type | 12 Integration Type | 13 RT | 14 M/Z | 15 Charge State | 16 Mono Mass Exp. |
        '17 Avg Mass Exp. | 18 Mono Mass Theo. | 19 Oligonucleotide | 20 Peak Area | 21 Sequence |
        '22 Label | 23 % TIC Base Peak

        'Sort Table2 by RT and intensity-----------------------------------------------------------------------

        ThisWorkbook.Worksheets(5).Activate
        Cells.Select
        Selection.ClearContents
        Selection.Font.Bold = False
        With Selection.Interior
            .Pattern = xlNone
            .TintAndShade = 0
            .PatternTintAndShade = 0
        End With
        With Selection.Borders
            .LineStyle = xlNone
        End With
        lng1 = Ntable2rows
        lng2 = Ntable2cols
        ReDim outputwrite(0 To lng1 - 1, 0 To lng2 - 1)
        For i = 1 To lng1
            For j = 1 To lng2
                outputwrite(i - 1, j - 1) = table2(i, j)
            Next j
        Next i
        ReDim table2(1)
    Set rng1 = Range(Cells(1, 1), Cells(lng1, lng2))
    rng1.Value = outputwrite

        ThisWorkbook.Worksheets(5).Sort.SortFields.Clear
        ThisWorkbook.Worksheets(5).Sort.SortFields.Add Key:=Range(Cells(1, 7), Cells(lng1, 7)),
        SortOn:=xlSortOnValues, Order:=xlAscending, DataOption:=xlSortNormal
    With ThisWorkbook.Worksheets(5).Sort
            .SetRange Range(Cells(1, 1), Cells(lng1, lng2))
        .Header = xlGuess
            .MatchCase = False
            .Orientation = xlTopToBottom
            .SortMethod = xlPinYin
            .Apply
        End With

        ReDim table2(Ntable2rows, Ntable2cols)
        For i = 1 To Ntable2rows
            For j = 1 To Ntable2cols
                table2(i, j) = Cells(i, j)
            Next j
        Next i

        'Determine top peaks in each UV peak-----------------------------------------------------

        For i = 1 To Ntable2rows
            lng1 = table2(i, 6)
            thing1 = table2(i, 27)
            table2(i, 7) = 100     '% base peak in UV peak
            For j = i + 1 To Ntable2rows
                If table2(j, 6) <> lng1 Then
                    i = j - 1
                    j = Ntable2rows
                Else
                    table2(j, 7) = table2(j, 27) / thing1 * 100
                End If
            Next j
        Next i

        'Rule1: IF is match and is > UVpeakBasePeakThresh then report for UV peak
        'Rule2: IF no match at all then report highest sorted mass (preferences cs > 1)

        NtableUVrows = 0
        ReDim tempcheck(Ntable2rows)
        For i = 1 To Ntable2rows
            k = i
            lng1 = table2(i, 6)
            check = table2(i, 5)
            If check Then
                tempcheck(i) = True
                NtableUVrows = NtableUVrows + 1
            End If
            For j = i + 1 To Ntable2rows
                If table2(j, 6) <> lng1 Then
                    GoTo outloop1
                Else
                    If table2(j, 5) Then
                        If table2(j, 7) >= UVpeakBasePeakThresh Then
                            If uvpeaks(lng1, 8) Then
                                If table2(j, 7) >= smallreport Then
                                    check = True
                                    tempcheck(j) = True
                                    NtableUVrows = NtableUVrows + 1
                                End If
                            Else
                                check = True
                                tempcheck(j) = True
                                NtableUVrows = NtableUVrows + 1
                            End If
                        End If
                    End If
                End If
            Next j
outloop1:
            If Not check Then
                tempcheck(k) = True
                NtableUVrows = NtableUVrows + 1
            End If
            i = j - 1
        Next i
        ReDim tableUV(NtableUVrows, Ntable2cols)
        n = 0
        For i = 1 To Ntable2rows
            If tempcheck(i) Then
                n = n + 1
                For j = 1 To Ntable2cols
                    tableUV(n, j) = table2(i, j)
                Next j
            End If
        Next i

        'Report UV match table-------------------------------------------------------------------

        ThisWorkbook.Worksheets(5).Activate
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
        lng1 = NtableUVrows
        lng2 = Ntable2cols
        ReDim outputwrite(0 To lng1, 0 To lng2 - 1)
        For i = 1 To lng1
            For j = 1 To lng2
                outputwrite(i, j - 1) = tableUV(i, j)
            Next j
        Next i
        For j = 1 To lng2
            outputwrite(0, j - 1) = table2header(j)
        Next j
    Set rng1 = Range(Cells(1, 1), Cells(lng1 + 1, lng2))
    rng1.Value = outputwrite

        Columns(1).NumberFormat = "0.00"
        Columns(2).NumberFormat = "0.00"
        Columns(3).NumberFormat = "0.00"
        Columns(4).NumberFormat = "0.00E+00"
        Columns(7).NumberFormat = "0.0"
        Columns(8).NumberFormat = "0.0000"
        Columns(15).NumberFormat = "0.0"
        Columns(20).NumberFormat = "0.00"
        Columns(21).NumberFormat = "0.0000"
        Columns(22).NumberFormat = "0"
        Columns(23).NumberFormat = "0.0000"
        Columns(24).NumberFormat = "0.000"
        Columns(25).NumberFormat = "0.0000"
        Columns(27).NumberFormat = "0.00E+00"
        Columns(30).NumberFormat = "0.00"

        ReDim stngarray(NFilingMassTableRows)
        For i = 1 To NFilingMassTableRows
            lng1 = lngarray(i)
            For j = 1 To NtableUVrows
                If tableUV(j, 9) = lng1 Then
                    stngarray(i) = "y"
                    j = NtableUVrows
                End If
            Next j
        Next i

        ThisWorkbook.Worksheets(10).Activate
        ReDim outputwrite(0 To NFilingMassTableRows, 0)
        outputwrite(0, 0) = "UV Peak Annotation"
        For i = 1 To NFilingMassTableRows
            outputwrite(i, 0) = stngarray(i)
        Next i
    Set rng1 = Range(Cells(1, 6), Cells(NFilingMassTableRows + 1, 6))
    rng1 = outputwrite

        'Generate Plot Data table----------------------------------------------------------------

        'Make Annotation matrix

        Nannots = Nuvpeaks
        ReDim annots(Nannots, 2) 'UV peak apex time, ID(s)
        n = 0
        For i = 1 To NtableUVrows
            lng1 = tableUV(i, 6)
            stng1 = tableUV(i, 29)
            k = 0
            For j = i + 1 To NtableUVrows
                If tableUV(j, 6) = lng1 Then
                    stng2 = tableUV(j, 29)
                    If InStr(stng1, stng2) = 0 Then stng1 = stng1 & ", " & stng2
                    k = k + 1
                Else
                    j = NtableUVrows
                End If
            Next j
            i = i + k
            n = n + 1
            annots(n, 1) = tableUV(i, 1)
            annots(n, 2) = stng1
        Next i

        'Readin chromatogram

        ThisWorkbook.Worksheets(4).Activate
        lng1 = Intersect(ActiveSheet.UsedRange, Columns(1)).Count - 4
        Npoints = 0
        ReDim tempcheck(lng1)
        thing2 = 0
        For i = 1 To lng1
            thing1 = Cells(i + 4, 1)
            If thing1 >= starttime Then
                If thing1 <= endtime Then
                    Npoints = Npoints + 1
                    tempcheck(i) = True
                    thing3 = Cells(i + 4, 2)
                    If thing2 < thing3 Then thing2 = thing3
                End If
            End If
        Next i
        thing3 = thing2 / 100 * baselineshift
        ReDim chromatogram(Npoints, 3)
        n = 0
        For i = 1 To lng1
            If tempcheck(i) Then
                n = n + 1
                chromatogram(n, 1) = Cells(i + 4, 1)
                chromatogram(n, 2) = Cells(i + 4, 2) + thing3
            End If
        Next i
        localmaxwindow = 0.1
        For i = 1 To Nannots
            thing1 = annots(i, 1)
            thing3 = 9000000.0#
            For j = 1 To Npoints
                thing2 = chromatogram(j, 1) - thing1
                thing2 = Abs(thing2)
                If thing2 < thing3 Then
                    thing3 = thing2
                    lng1 = j
                End If
            Next j
            thing1 = chromatogram(lng1, 1) - localmaxwindow / 2
            thing2 = chromatogram(lng1, 1) + localmaxwindow / 2
            thing4 = -10000
            For j = 1 To Npoints
                thing3 = chromatogram(j, 1)
                If thing3 >= thing1 Then
                    If thing3 <= thing2 Then
                        If thing4 < chromatogram(j, 2) Then
                            thing4 = chromatogram(j, 2)
                            lng2 = j
                        End If
                    Else
                        j = Npoints
                    End If
                End If
            Next j
            If NoIDannotate Then
                chromatogram(lng2, 3) = annots(i, 2)
            Else
                If Left(annots(i, 2), 1) <> "a" And Left(annots(i, 2), 1) <> "m" Then
                    chromatogram(lng2, 3) = annots(i, 2)
                End If
            End If
        Next i

        ThisWorkbook.Worksheets(7).Activate
        Cells.Select
        Selection.ClearContents
        Selection.Font.Bold = False
        With Selection.Interior
            .Pattern = xlNone
            .TintAndShade = 0
            .PatternTintAndShade = 0
        End With
        With Selection.Borders
            .LineStyle = xlNone
        End With
        lng1 = Npoints
        lng2 = 3
        ReDim outputwrite(0 To lng1 - 1, 0 To lng2 - 1)
        For i = 1 To lng1
            For j = 1 To lng2
                outputwrite(i - 1, j - 1) = chromatogram(i, j)
            Next j
        Next i
    Set rng1 = Range(Cells(1, 1), Cells(lng1, lng2))
    rng1.Value = outputwrite


        'Plot X axis

        axisfactor1 = 1.8
        Nysegments = 14
    Set rng1 = Range(Cells(1, 2), Cells(lng1, 2))
    timeD = 1 / 10000
        lng2 = Int((endtime - starttime) / MinorU) + 1
        lng1 = 3 * lng2

        PlotH = WorksheetFunction.Max(rng1) * 1.1
        thing1 = WorksheetFunction.Min(rng1, 0) * 1.1
        thing1 = -1 * WorksheetFunction.Max(rng1) / 100
        thing2 = 1.5 * thing1
        PlotB = thing2 * axisfactor1
        thing3 = PlotH / Nysegments
        thing4 = Log(thing3) / Log(10)
        lng3 = Int(thing4)
        thing4 = 10 ^ lng3
        lng3 = Round(thing3 / thing4, 0)
        If lng3 = 1 Then
            lng4 = 1
        Else
            If lng3 < 4 Then
                lng4 = 2
            Else
                If lng3 < 8 Then
                    lng4 = 5
                Else
                    lng4 = 10
                End If
            End If
        End If
        MajorUy = lng4 * thing4
        MinorUy = MajorUy / 5
        PlotB = -MajorUy

        ReDim outputwrite(0 To lng1 - 1, 0 To 1)
        outputwrite(1, 0) = startaxistime
        outputwrite(0, 0) = outputwrite(1, 0) - timeD
        outputwrite(2, 0) = outputwrite(1, 0) + timeD
        lng8 = Round(startaxistime, 2) * 100
        MajorU = MajorU * 100
        If lng8 Mod MajorU = 0 Then
            outputwrite(1, 1) = thing2
        Else
            outputwrite(1, 1) = thing1
        End If
        outputwrite(0, 1) = 0
        outputwrite(2, 1) = 0
        For i = 2 To lng2
            j = (i - 1) * 3 + 2
            outputwrite(j - 1, 0) = outputwrite(j - 4, 0) + MinorU
            outputwrite(j - 2, 0) = outputwrite(j - 1, 0) - timeD
            outputwrite(j, 0) = outputwrite(j - 1, 0) + timeD
            lng8 = Round(outputwrite(j - 1, 0), 2) * 100
            If lng8 Mod MajorU = 0 Then
                outputwrite(j - 1, 1) = thing2
            Else
                outputwrite(j - 1, 1) = thing1
            End If
            outputwrite(j - 2, 1) = 0
            outputwrite(j, 1) = 0
        Next i
        If outputwrite(1, 0) = starttime Then
        Set rng1 = Range(Cells(1, 10), Cells(lng1, 11))
        rng1.Value = outputwrite
        Else
        Set rng1 = Range(Cells(2, 10), Cells(lng1 + 1, 11))
        rng1.Value = outputwrite
            Cells(1, 10) = starttime
            Cells(1, 11) = 0
        End If
        ReDim outputwrite(0 To lng1 - 1, 0 To 1)
        outputwrite(1, 0) = startaxistime
        outputwrite(0, 0) = outputwrite(1, 0) - timeD
        outputwrite(2, 0) = outputwrite(1, 0) + timeD
        lng8 = Round(startaxistime, 2) * 100
        If lng8 Mod MajorU = 0 Then
            outputwrite(1, 1) = thing2 * axisfactor1
        Else
            outputwrite(1, 1) = 0
        End If
        outputwrite(0, 1) = 0
        outputwrite(2, 1) = 0
        For i = 2 To lng2
            j = (i - 1) * 3 + 2
            outputwrite(j - 1, 0) = outputwrite(j - 4, 0) + MinorU
            outputwrite(j - 2, 0) = outputwrite(j - 1, 0) - timeD
            outputwrite(j, 0) = outputwrite(j - 1, 0) + timeD
            lng8 = Round(outputwrite(j - 1, 0), 2) * 100
            If lng8 Mod MajorU = 0 Then
                outputwrite(j - 1, 1) = thing2 * axisfactor1
            Else
                outputwrite(j - 1, 1) = 0
            End If
            outputwrite(j - 2, 1) = 0
            outputwrite(j, 1) = 0
        Next i
        If outputwrite(1, 0) = starttime Then
        Set rng1 = Range(Cells(1, 12), Cells(lng1, 13))
        rng1.Value = outputwrite
            lng9 = 0
        Else
        Set rng1 = Range(Cells(2, 12), Cells(lng1 + 1, 13))
        rng1.Value = outputwrite
            Cells(1, 12) = starttime
            Cells(1, 13) = 0
            lng1 = lng1 + 1
            lng9 = 1
        End If
    
'Create plots----------------------------------------------------------------------------
    
    Set rng1 = Range(Cells(1, 1), Cells(Npoints, 1))
    Set rng2 = Range(Cells(1, 2), Cells(Npoints, 2))
    Set rng3 = Range(Cells(1, 10), Cells(lng1, 10))
    Set rng4 = Range(Cells(1, 11), Cells(lng1, 11))
    Set rng5 = Range(Cells(1, 12), Cells(lng1, 12))
    Set rng6 = Range(Cells(1, 13), Cells(lng1, 13))
    
    ThisWorkbook.Worksheets(8).Activate
        ActiveSheet.ChartObjects(1).Activate
        ActiveChart.Parent.Delete
        ActiveSheet.Shapes.AddChart2(240, xlXYScatter).Select


        'Uses Late Binding to the PowerPoint Object Model
        'No reference required to PowerPoint Object Library; NOT TRUE ANYMORE--to add open new PPt, need library

        '1 inch = 72 points
        topleftxppt = topleftxppt * 72
        topleftyppt = topleftyppt * 72
        heightppt = heightppt * 72
        widthppt = widthppt * 72
    Set PPApp = New PowerPoint.Application
    PPApp.presentations.Add
        PPApp.ActivePresentation.Slides.Add PPApp.ActivePresentation.Slides.Count + 1, ppLayoutBlank
    Set PPPres = PPApp.ActivePresentation
    Set PPSlide = PPPres.Slides(PPApp.ActiveWindow.Selection.SlideRange.SlideIndex)
    
    'ThisWorkbook.Charts(1).Activate
    ThisWorkbook.Worksheets(8).ChartObjects(1).Activate
        ThisWorkbook.Worksheets(8).Shapes(ActiveChart.Parent.Name).Line.Visible = msoFalse
        With ActiveChart
            .ChartArea.ClearContents
            .ChartType = xlXYScatterLinesNoMarkers
            .SeriesCollection.Add Source:=rng1
        .SeriesCollection(1).XValues = rng1
            .SeriesCollection(1).Values = rng2
            .SeriesCollection(1).Format.Line.Visible = msoTrue
            .SeriesCollection(1).Format.Line.ForeColor.RGB = RGB(0, 0, 0)
            .SeriesCollection(1).Format.Line.Weight = 0.5
            .SeriesCollection(1).Format.Line.Transparency = 0
            .Axes(xlValue).MinorTickMark = xlTickMarkOutside
            .Axes(xlValue).Format.Line.Weight = 0.5
            .Axes(xlValue).Format.Line.ForeColor.RGB = RGB(0, 0, 0)
            .Axes(xlValue).MajorUnit = MajorUy
            .Axes(xlValue).MinorUnit = MinorUy
            .SeriesCollection.Add Source:=rng3
        .SeriesCollection(2).XValues = rng3
            .SeriesCollection(2).Values = rng4
            .SeriesCollection(2).Format.Line.Visible = msoTrue
            .SeriesCollection(2).Format.Line.ForeColor.RGB = RGB(0, 0, 0)
            .SeriesCollection(2).Format.Line.Weight = 0.5
            .SeriesCollection(2).Format.Line.Transparency = 0
            .SeriesCollection.Add Source:=rng5
        .SeriesCollection(3).XValues = rng5
            .SeriesCollection(3).Values = rng6
            .SeriesCollection(3).Format.Line.Visible = msoFalse
            .Axes(xlCategory).MinimumScale = starttime
            .Axes(xlCategory).MaximumScale = endtime
            .HasAxis(xlCategory) = False
            For i = 1 To lng2
                j = (i - 1) * 3 + 2 + lng9
                thing1 = ThisWorkbook.Worksheets(7).Cells(j, 13)
                If thing1 <> 0 Then
                    .SeriesCollection(3).Points(j).HasDataLabel = True
                    .SeriesCollection(3).Points(j).DataLabel.ShowCategoryName = True
                    .SeriesCollection(3).Points(j).DataLabel.ShowValue = False
                    .SeriesCollection(3).Points(j).DataLabel.Position = xlLabelPositionCenter
                    .SeriesCollection(3).Points(j).DataLabel.Font.Size = axisfontsize
                    .SeriesCollection(3).Points(j).DataLabel.Font.Bold = True
                End If
            Next i
            .Axes(xlValue).MinimumScale = PlotB
            .Axes(xlValue).MaximumScale = PlotH
            .Axes(xlValue).TickLabels.Font.Size = axisfontsize
            .Axes(xlValue).TickLabels.Font.Bold = True
            .ChartArea.Select
            originalwidth = Selection.Width
            originalheight = Selection.Height
            .SetElement(msoElementLegendNone)
            .SetElement(msoElementPrimaryValueGridLinesNone)
            .SetElement(msoElementPrimaryCategoryAxisTitleBelowAxis)
            Selection.Caption = "Time (min)"
            Selection.Font.Size = axisfontsize
            .SetElement(msoElementPrimaryValueAxisTitleRotated)
            Selection.Caption = "uAU"
            Selection.Font.Size = axisfontsize
            .PlotArea.Format.Fill.Visible = msoFalse
            .ChartArea.Format.Fill.Visible = msoFalse

            .ChartArea.Height = heightppt
            .ChartArea.Width = widthppt

            plotheight = .PlotArea.InsideHeight
            offsettop = .Axes(xlValue).Top
            plotwidth = .PlotArea.InsideWidth
            plotwidth = plotwidth * 0.99            'Empirical factor for 8" wide by 5" high
            offsetleft = .Axes(xlCategory).Left

            ActiveChart.CopyPicture Appearance:=xlScreen, Format:=xlPrinter
        Set Shp = PPSlide.Shapes.Paste
        With Shp
                .Left = topleftxppt
                .Top = topleftyppt
                .Height = heightppt
                .Width = widthppt
                thing1 = .Height
                If thing1 > heightppt Then
                    .Top = Int((7.5 * 72 - thing1) / 2)
                End If
                fromleft = .Left
                fromtop = .Top
            End With
            n = 0
            fromleft = fromleft - textboxwidth * 0.2    'Empirical factor for 8" wide by 5" high
            fromtop = fromtop - textboxheight
            For i = 1 To Npoints
                stng1 = ThisWorkbook.Worksheets(7).Cells(i, 3)
                If Len(stng1) > 0 Then
                    .SeriesCollection(1).Points(i).HasDataLabel = True
                    .SeriesCollection(1).Points(i).DataLabel.Text = stng1
                    .SeriesCollection(1).Points(i).DataLabel.Position = xlLabelPositionAbove
                    .SeriesCollection(1).Points(i).DataLabel.Orientation = 90
                    n = n + 1
                    thing1 = offsetleft + plotwidth * (ThisWorkbook.Worksheets(7).Cells(i, 1) - starttime) / (endtime - starttime)
                    thing2 = offsettop + plotheight * (PlotH - ThisWorkbook.Worksheets(7).Cells(i, 2)) / (PlotH - PlotB)
                Set Shp = PPSlide.Shapes.AddTextbox(Orientation:=msoTextOrientationUpward, _
                    Left:=thing1 + fromleft, _
                    Top:=thing2 + fromtop, _
                    Width:=textboxwidth, Height:=textboxheight)
                Shp.TextFrame.TextRange.Text = stng1
                    If Left(ThisWorkbook.Worksheets(7).Cells(i, 3), 1) = "m" Or Left(ThisWorkbook.Worksheets(7).Cells(i, 3), 1) = "a" Then
                        Shp.TextFrame.TextRange.Font.Color = RGB(255, 0, 0)
                    End If
                    Shp.TextEffect.FontSize = fontsz
                    Shp.TextFrame.MarginLeft = 0
                    Shp.TextFrame.MarginRight = 0
                    Shp.TextFrame.MarginTop = 0
                    Shp.TextFrame.MarginBottom = 0
                    Shp.TextFrame.WordWrap = False
                    n = n
                End If
            Next i
        End With
    Set PPSlide = Nothing
    Set PPPres = Nothing
    Set PPApp = Nothing
    
 
End Sub








End Class
