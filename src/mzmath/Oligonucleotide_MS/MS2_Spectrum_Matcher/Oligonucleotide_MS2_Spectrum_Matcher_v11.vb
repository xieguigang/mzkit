'v11 added back in precursor minus base fragment ions
Imports System.Runtime.Intrinsics
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports System.Math

Public Class MS2_Spectrum_Matcher

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="spectrum">the spectrum input</param>
    Sub test_MS2match(spectrum As PeakMs2)

        Dim i As Long, j As Long, k As Long, m As Long, n As Long, p As Long, q As Long
        Dim lng1 As Long, lng2 As Long, lng3 As Long, lng4 As Long
        Dim stng1 As String, stng2 As String, stng3 As String, stng4 As String, stng5 As String
        Dim thing1 As Double, thing2 As Double, thing3 As Double, thing4 As Double, thing5 As Double, thing6 As Double
        Dim var1 As Object
        Dim rng1 As Range, rng2 As Range, rng3 As Range, rng4 As Range, rng5 As Range, rng6 As Range, rng7 As Range, rng8 As Range
        Dim check As Boolean
        Dim outputwrite() As Object, temp() As Object, tempcheck() As Boolean

        Dim fragments() As Object, Nfragments As Long
        Dim ladder3() As Object, Nladder3 As Long
        Dim ladder5() As Object, Nladder5 As Long
        Dim fragmentnames() As String
        Dim fragmentmeta() As Long
        Dim fragmentmass() As Double
        Dim fragmentcol() As String, Nfragmentcol As Long
        Dim startres As Long, endres As Long
        Dim startxy As Long, isprofile As Boolean, filetext As String, scantext As String, scannum As String
        Dim Npeaks As Long, maxint As Long, beforecheck As Boolean
        Dim carbon As Double, hydrogen As Double, nitrogen As Double, oxygen As Double, phosphorus As Double, sulfur As Double, proton As Double, water As Double
        Dim Nbases As Long, oligo() As Dim9
        Dim MolecularMass As Double
        Dim composition(6) As Double
        Dim precursorCS As Long
        Dim basenames() As String, Nbasenames As Long
        Dim wxyz() As Object, Nwxyz As Long
        Dim abcd() As Object, Nabcd As Long
        Dim aB() As Object, NaB As Long
        Dim neutralnames() As String
        Dim neutralmeta() As Long
        Dim neutralmass() As Double
        Dim Nneutrals As Long
        Dim Nneutralcols As Long
        Dim neutralcols() As String
        Dim Nmasses As Long, masses() As Object
        Dim Nuniquemasses As Long, uniquemasses() As Object
        Dim peakthresh As Double, Aplus1 As Double
        Dim basepeakint As Double, aplus1tol As Double, aplus1frac As Double, CS As Long
        Dim matchtol As Double
        Dim plotpeaks() As Object, gapx As Double, gapy As Double
        Dim Nkinds As Long, kinds() As String, labelthresh As Double, labelbin As Double
        Dim basesonly As String
        Dim sequence_name As String
        Dim NbasesInSequence As Long, BasesInSequence() As Object
        Dim sequence_mass As Double, sequence_longname As String
        Dim ignoreinternals As Boolean
        Dim RT As String

        Aplus1 = 1.002736   'based on the theoretical isotopic distribution of C268 H342 N98 O194 P28

        ' Input Oligonucleotide
        ' ThisWorkbook.Worksheets(1).Activate
        ' Precursor Charge State (assumed to be negative)
        precursorCS = 3 ' Abs(Cells(13, 1))
        sequence_mass = 4474.6441        ' Cells(9, 1)
        stng1 = "5-Arp-Arp-Crp-Vrp-Vrp-Crp-Arp-Arp-Crp-Vrp-Vrp-Crp-Arp-Arp-3" ' Cells(7, 1)
        sequence_longname = stng1.Replace("-", "")
        ignoreinternals = True

        Nkinds = 3
        ReDim kinds(3)
        kinds(1) = "5' Ladder"
        kinds(2) = "3' Ladder"
        kinds(3) = "Internal"
        labelbin = 1   'Da

        'Get spectrum
        ' spectrum input
        ' ThisWorkbook.Worksheets(2).Activate

        ' COLUMNS A-B ARE FOR PASTING AN INDIVIDUAL MS2 SPECTRUM.  ALL OTHER CELLS ON THIS WORKSHEET CAN BE MODIFIED/DELETED AS DESIRED, EXCEPT CELL F2.
        ' <--STARTING ROW OF XY DATA.  FOR XCALIBUR-COPIED SPECTRA, START ROW IS 8 IF YOU PASTE IN CELL A1.
        startxy = 9 ' Cells(2, 6)                   'must be entered if not 1 in cell F2 of 2nd worksheet
        isprofile = False

        If startxy < 1 Then
            startxy = 1
            filetext = ""
            scantext = ""
            scannum = ""
        Else
            ' file name
            filetext = "20220525_WRMnew_B.raw"
            ' spectrum scan id
            scantext = "FTMS - c ESI d Full ms2 1490.8749@hcd30.00 [156.0000-2000.0000]"
            ' scan number
            scannum = "Scan #: 39256-39398"
            RT = "RT: 191.98-192.64"
        End If
        lng1 = spectrum.fragments
        maxint = spectrum.mzInto.Select(Function(f) f.intensity).Max
        ReDim tempcheck(lng1)
        Dim minint As Double = maxint * 0.0001
        Dim peaks() As ms2 = spectrum.mzInto _
            .Where(Function(f) f.intensity >= minint) _
            .ToArray

        If isprofile Then
            ' make spectrum data centroid
            peaks = peaks _
                .Centroid(Tolerance.DeltaMass(0.01), New RelativeIntensityCutoff(0)) _
                .ToArray
        End If

        Npeaks = peaks.Length

        'Read in oligonucleotide
        ' ThisWorkbook.Worksheets(1).Activate

        Dim colors() As ColorMap = {
            New ColorMap("0", 0),
            New ColorMap("0.25", 1),
            New ColorMap("0.5", 2),
            New ColorMap("1", 3),
            New ColorMap("2", 4),
            New ColorMap("4", 5),
            New ColorMap("8", 6),
            New ColorMap("16", 7),
            New ColorMap("32", 8),
            New ColorMap("100", 9)
        }
        Dim Ncolors As Long = 10


        carbon = 12
        hydrogen = 1.007825
        nitrogen = 14.003074
        oxygen = 15.994915
        phosphorus = 30.973762
        sulfur = 31.97207117
        proton = 1.007276467
        water = oxygen + hydrogen * 2
        ' Peak Annotation Threshold (Minimum % Base Peak)
        peakthresh = 0.2
        ' Match Tolerance (ppm)
        matchtol = 20
        labelthresh = peakthresh
        aplus1tol = 10 'matchtol * 1.5 'ppm
        ' 1-Letter Sequence
        sequence_name = "AACVVCAACVVCAA"
        basesonly = sequence_name

        stng1 = ""
        Dim oligoends() As Dim9
        ReDim oligoends(2)
        Nbases = sequence_name.Length - 1
        ReDim oligo(Nbases)

        Dim seq53 = {"Arp", "Arp", "Crp", "Vrp", "Vrp", "Crp", "Arp", "Arp", "Crp", "Vrp", "Vrp", "Crp", "Arp", "Arp"}

        ' 5
        ' Arp
        ' Arp
        ' Crp
        ' Vrp
        ' Vrp
        ' Crp
        ' Arp
        ' Arp
        ' Crp
        ' Vrp
        ' Vrp
        ' Crp
        ' Arp
        ' Arp
        ' 3

        Dim list = Dim9.List

        For i = 1 To Nbases
            oligo(i)(1) = seq53(i - 1)   'base name
            stng1 = stng1 & Mid(sequence_name, i, 1)
            oligo(i)(8) = 0   'modification mass
            If oligo(i)(8) <> 0 Then
                If oligo(i)(8) < 0 Then
                    stng1 = stng1 & "(-" & Abs(Round(oligo(i)(8), 0)) & ")"
                Else
                    stng1 = stng1 & "(+" & Round(oligo(i)(8), 0) & ")"
                End If
            End If

            Dim cells = list(seq53(i - 1))

            oligo(i)(2) = cells(0)  '# carbons
            oligo(i)(3) = cells(1)  '# hydrogens
            oligo(i)(4) = cells(2)  '# nitrogens
            oligo(i)(5) = cells(3)  '# oxygens
            oligo(i)(6) = cells(4)  '# phosphorus atoms
            oligo(i)(7) = cells(5)  '# sulfur atoms
            oligo(i)(9) = oligo(i)(8) + oligo(i)(2) * carbon + oligo(i)(3) * hydrogen + oligo(i)(4) * nitrogen + oligo(i)(5) * oxygen + oligo(i)(6) * phosphorus + oligo(i)(7) * sulfur
        Next i
        sequence_name = stng1
        oligoends(1)(1) = Cells(6, 3)
        oligoends(1)(1) = Cells(6, 3)   'base name
        oligoends(1)(8) = Cells(6, 4)   'modification mass
        oligoends(1)(2) = Cells(6, 13)  '# carbons
        oligoends(1)(3) = Cells(6, 14)  '# hydrogens
        oligoends(1)(4) = Cells(6, 15)  '# nitrogens
        oligoends(1)(5) = Cells(6, 16)  '# oxygens
        oligoends(1)(6) = Cells(6, 17)  '# phosphorus atoms
        oligoends(1)(7) = Cells(6, 18)  '# sulfur atoms
        oligoends(1)(9) = oligoends(1)(8) + oligoends(1)(2) * carbon + oligoends(1)(3) * hydrogen + oligoends(1)(4) * nitrogen + oligoends(1)(5) * oxygen + oligoends(1)(6) * phosphorus + oligoends(1)(7) * sulfur
        oligoends(2)(1) = Cells(7 + Nbases, 3)   'base name
        oligoends(2)(8) = Cells(7 + Nbases, 4)   'modification mass
        oligoends(2)(2) = Cells(7 + Nbases, 13)  '# carbons
        oligoends(2)(3) = Cells(7 + Nbases, 14)  '# hydrogens
        oligoends(2)(4) = Cells(7 + Nbases, 15)  '# nitrogens
        oligoends(2)(5) = Cells(7 + Nbases, 16)  '# oxygens
        oligoends(2)(6) = Cells(7 + Nbases, 17)  '# phosphorus atoms
        oligoends(2)(7) = Cells(7 + Nbases, 18)  '# sulfur atoms
        oligoends(2)(9) = oligoends(2)(8) + oligoends(2)(2) * carbon + oligoends(2)(3) * hydrogen + oligoends(2)(4) * nitrogen + oligoends(2)(5) * oxygen + oligoends(2)(6) * phosphorus + oligoends(2)(7) * sulfur
        thing1 = oligoends(1)(8) + oligoends(2)(8)
        For i = 1 To Nbases
            thing1 = thing1 + oligo(i)(8)
            For j = 1 To 6
                composition(j) = composition(j) + oligo(i)(j + 1)
            Next j
        Next i
        For i = 1 To 2
            For j = 1 To 6
                composition(j) = composition(j) + oligoends(i)(j + 1)
            Next j
        Next i
        MolecularMass = thing1 + composition(1) * carbon + composition(2) * hydrogen + composition(3) * nitrogen + composition(4) * oxygen + composition(5) * phosphorus + composition(6) * sulfur

        Nbasenames = 0
        For i = 1 To 5
            If Len(Cells(i + 2, 24)) > 0 Then
                Nbasenames = Nbasenames + 1
            End If
        Next i
        ReDim basenames(Nbasenames)
        j = 0
        For i = 1 To 5
            If Len(Cells(i + 2, 24)) > 0 Then
                j = j + 1
                basenames(j) = Cells(i + 2, 24)
            End If
        Next i

        'Readin fragments

        Nwxyz = 0
        Nabcd = 0
        NaB = 0
        For i = 1 To 20
            If Cells(38 + i, 26) = "5'" And Cells(38 + i, 33) Then Nwxyz = Nwxyz + 1
            If Cells(38 + i, 26) = "3'" And InStr(Cells(38 + i, 25), "-") = 0 And Cells(38 + i, 33) Then Nabcd = Nabcd + 1
            If Cells(38 + i, 26) = "3'" And InStr(Cells(38 + i, 25), "-") > 0 And Cells(38 + i, 33) Then NaB = Nbasenames
        Next i
        Dim wxyz_s() As Object
        Dim abcd_s() As Object

        If NaB > 0 Then ReDim aB(NaB, 8)
        If Nwxyz > 0 Then
            ReDim wxyz(Nwxyz, 8)
            ReDim wxyz_s(Nwxyz, 8)
        End If
        If Nabcd > 0 Then
            ReDim abcd(Nabcd, 8)
            ReDim abcd_s(Nabcd, 8)
        End If
        j = 0
        k = 0
        n = 0
        For i = 1 To 20
            If Cells(38 + i, 26) = "5'" And Cells(38 + i, 33) Then
                j = j + 1
                wxyz(j, 1) = Cells(38 + i, 25)
                wxyz_s(j, 1) = Cells(38 + i, 25)
                For m = 1 To 6
                    wxyz(j, m + 1) = Cells(38 + i, 26 + m)
                    wxyz_s(j, m + 1) = Cells(38 + i, 26 + m)
                Next m
                If Cells(38 + i, 25) = "y" Or Cells(38 + i, 25) = "z" Then
                    wxyz_s(j, 5) = wxyz_s(j, 5) + 1
                    wxyz_s(j, 7) = wxyz_s(j, 7) - 1
                End If
            End If
            If Cells(38 + i, 26) = "3'" And InStr(Cells(38 + i, 25), "-") = 0 And Cells(38 + i, 33) Then
                k = k + 1
                abcd(k, 1) = Cells(38 + i, 25)
                abcd_s(k, 1) = Cells(38 + i, 25)
                For m = 1 To 6
                    abcd(k, m + 1) = Cells(38 + i, 26 + m)
                    abcd_s(k, m + 1) = Cells(38 + i, 26 + m)
                Next m
                If Cells(38 + i, 25) = "c" Or Cells(38 + i, 25) = "d" Then
                    abcd_s(k, 5) = abcd_s(k, 5) - 1
                    abcd_s(k, 7) = abcd_s(k, 7) + 1
                End If
            End If
        Next i
        If NaB > 0 Then
            For i = 1 To Nbasenames
                aB(i, 1) = "a-" & Cells(i + 2, 24)
                aB(i, 2) = 0 - Cells(i + 2, 27)         'C
                aB(i, 3) = 0 - Cells(i + 2, 28) - 1     'H
                aB(i, 4) = 0 - Cells(i + 2, 29)         'N
                aB(i, 5) = 0 - Cells(i + 2, 30)         'O
                aB(i, 6) = 0                            'P
                aB(i, 7) = 0                            'S
            Next i
        End If
        For i = 1 To Nwxyz
            wxyz(i, 8) = wxyz(i, 2) * carbon + wxyz(i, 3) * hydrogen + wxyz(i, 4) * nitrogen + wxyz(i, 5) * oxygen + wxyz(i, 6) * phosphorus + wxyz(i, 7) * sulfur
            wxyz_s(i, 8) = wxyz_s(i, 2) * carbon + wxyz_s(i, 3) * hydrogen + wxyz_s(i, 4) * nitrogen + wxyz_s(i, 5) * oxygen + wxyz_s(i, 6) * phosphorus + wxyz_s(i, 7) * sulfur
        Next i
        For i = 1 To Nabcd
            abcd(i, 8) = abcd(i, 2) * carbon + abcd(i, 3) * hydrogen + abcd(i, 4) * nitrogen + abcd(i, 5) * oxygen + abcd(i, 6) * phosphorus + abcd(i, 7) * sulfur
            abcd_s(i, 8) = abcd_s(i, 2) * carbon + abcd_s(i, 3) * hydrogen + abcd_s(i, 4) * nitrogen + abcd_s(i, 5) * oxygen + abcd_s(i, 6) * phosphorus + abcd_s(i, 7) * sulfur
        Next i
        For i = 1 To NaB
            aB(i, 8) = aB(i, 2) * carbon + aB(i, 3) * hydrogen + aB(i, 4) * nitrogen + aB(i, 5) * oxygen + aB(i, 6) * phosphorus + aB(i, 7) * sulfur
        Next i
        NbasesInSequence = 0
        For i = 1 To Nbasenames
            For j = 1 To Len(sequence_name)
                If Mid(sequence_name, j, 1) = basenames(i) Then
                    NbasesInSequence = NbasesInSequence + 1
                    j = Len(sequence_name)
                End If
            Next j
        Next i
        ReDim BasesInSequence(NbasesInSequence, 2)
        n = 0
        For i = 1 To Nbasenames
            For j = 1 To Len(sequence_name)
                If Mid(sequence_name, j, 1) = basenames(i) Then
                    n = n + 1
                    BasesInSequence(n, 1) = basenames(i)
                    BasesInSequence(n, 2) = aB(i, 8)        'this is a negative value
                    j = Len(sequence_name)
                End If
            Next j
        Next i

        'Create list of all fragments, neutral mass

        '5' ladder

        Nladder5 = Nbases
        ReDim ladder5(Nladder5, Nabcd + 1 + 4)
        ladder5(1, 1) = 1           'start residue
        ladder5(1, 2) = 1           'end residue
        ladder5(1, 3) = oligoends(1, 1) & oligo(1, 1) 'sequence
        ladder5(1, 4) = oligoends(1, 9) + oligo(1, 9) 'seed mass
        If Left(oligo(2, 1), 1) = "s" Then
            For j = 1 To Nabcd
                ladder5(1, 4 + j) = ladder5(1, 4) + abcd_s(j, 8)
            Next j
        Else
            For j = 1 To Nabcd
                ladder5(1, 4 + j) = ladder5(1, 4) + abcd(j, 8)
            Next j
        End If
        For j = 1 To Nbasenames
            If Mid(ladder5(1, 3), Len(ladder5(1, 3)) - 2, 1) = basenames(j) Then
                ladder5(1, 4 + Nabcd + 1) = ladder5(1, 4) + abcd(1, 8) + aB(j, 8)
                j = Nbasenames
            End If
        Next j
        For i = 2 To Nladder5
            ladder5(i, 1) = 1           'start residue
            ladder5(i, 2) = i           'end residue
            ladder5(i, 3) = ladder5(i - 1, 3) & oligo(i, 1)  'sequence
            ladder5(i, 4) = ladder5(i - 1, 4) + oligo(i, 9) 'seed mass
            If i < Nladder5 Then
                If Left(oligo(i + 1, 1), 1) = "s" Then
                    For j = 1 To Nabcd
                        ladder5(i, 4 + j) = ladder5(i, 4) + abcd_s(j, 8)
                    Next j
                Else
                    For j = 1 To Nabcd
                        ladder5(i, 4 + j) = ladder5(i, 4) + abcd(j, 8)
                    Next j
                End If
            Else
                For j = 1 To Nabcd
                    ladder5(i, 4 + j) = ladder5(i, 4) + abcd(j, 8)
                Next j
            End If
            For j = 1 To Nbasenames
                stng1 = ladder5(i, 3)
                If Mid(stng1, Len(stng1) - 2, 1) = basenames(j) Then
                    ladder5(i, 4 + Nabcd + 1) = ladder5(i, 4) + abcd(1, 8) + aB(j, 8)
                    j = Nbasenames
                End If
            Next j
        Next i

        '3' ladder

        'rewrite this with 2 arrays, one for bases, one for ends

        Nladder3 = Nbases
        ReDim ladder3(Nladder3, Nwxyz + 4)
        ladder3(1, 1) = Nbases                             'start residue
        ladder3(1, 2) = Nbases                             'end residue
        ladder3(1, 3) = oligo(Nbases, 1) & oligoends(2, 1) 'sequence
        ladder3(1, 4) = oligo(Nbases, 9) + oligoends(2, 9) 'seed mass
        If Left(ladder3(1, 3), 1) = "s" Then
            For j = 1 To Nwxyz
                ladder3(1, 4 + j) = ladder3(1, 4) + wxyz_s(j, 8)
            Next j
        Else
            For j = 1 To Nwxyz
                ladder3(1, 4 + j) = ladder3(1, 4) + wxyz(j, 8)
            Next j
        End If
        For i = 2 To Nladder3
            k = Nbases - i + 1
            ladder3(i, 1) = k                               'start residue
            ladder3(i, 2) = Nbases                          'end residue
            ladder3(i, 3) = oligo(k, 1) & ladder3(i - 1, 3) 'sequence
            ladder3(i, 4) = ladder3(i - 1, 4) + oligo(k, 9) 'seed mass
            If k = 1 Then
                For j = 3 To 3
                    ladder3(i, 4 + j) = ladder3(i, 4) + wxyz(j, 8)  'this mass is the same as the oligoends(1,9)
                Next j
            Else
                If Left(ladder3(i, 3), 1) = "s" Then
                    For j = 1 To Nwxyz
                        ladder3(i, 4 + j) = ladder3(i, 4) + wxyz_s(j, 8)
                    Next j
                Else
                    For j = 1 To Nwxyz
                        ladder3(i, 4 + j) = ladder3(i, 4) + wxyz(j, 8)
                    Next j
                End If
            End If
        Next i

        'internal fragments

        lng1 = Nbases
        Nfragments = 0
        For i = 1 To lng1
            Nfragments = Nfragments + i
        Next i
        ReDim fragmentnames(Nfragments)
        ReDim fragmentmeta(Nfragments, 2)
        Nfragmentcol = 1 + Nwxyz * (Nabcd + 1)
        ReDim fragmentmass(Nfragments, Nfragmentcol)
        n = 0
        For i = 1 To Nbases
            For j = i To Nbases
                n = n + 1
                startres = i
                endres = j
                fragmentmeta(n, 1) = startres
                fragmentmeta(n, 2) = endres
                For k = startres To endres
                    fragmentnames(n) = fragmentnames(n) & oligo(k, 1)
                    fragmentmass(n, 1) = fragmentmass(n, 1) + oligo(k, 9)
                Next k
            Next j
        Next i
        n = 0
        For p = 1 To Nbases
            For q = p To Nbases
                n = n + 1
                If p > 1 Or p = 1 And q = Nbases Then   'the phosphate is never on the 5' when RNase T1-digested;
                    'therefore, no internal fragments slightly smaller than 5'ladder w/ missing 5' end phosphate
                    If p = 1 Then
                        For i = 3 To 3 'Nwxyz
                            For j = 1 To Nabcd
                                k = (i - 1) * (Nabcd + 1) + 1 + j
                                m = fragmentmeta(n, 2) + 1
                                If m <= Nbases Then
                                    If Left(oligo(m, 1), 1) = "s" Then
                                        If Left(fragmentnames(n), 1) = "s" Then
                                            fragmentmass(n, k) = fragmentmass(n, 1) + wxyz_s(i, 8) + abcd_s(j, 8)
                                        Else
                                            fragmentmass(n, k) = fragmentmass(n, 1) + wxyz(i, 8) + abcd_s(j, 8)
                                        End If
                                    Else
                                        If Left(fragmentnames(n), 1) = "s" Then
                                            fragmentmass(n, k) = fragmentmass(n, 1) + wxyz_s(i, 8) + abcd(j, 8)
                                        Else
                                            fragmentmass(n, k) = fragmentmass(n, 1) + wxyz(i, 8) + abcd(j, 8)
                                        End If
                                    End If
                                Else
                                    If Left(fragmentnames(n), 1) = "s" Then
                                        fragmentmass(n, k) = fragmentmass(n, 1) + wxyz_s(i, 8) + abcd(j, 8)
                                    Else
                                        fragmentmass(n, k) = fragmentmass(n, 1) + wxyz(i, 8) + abcd(j, 8)
                                    End If
                                End If
                            Next j
                            k = (i - 1) * (Nabcd + 1) + 1 + Nabcd + 1
                            stng1 = fragmentnames(n)
                            For j = 1 To Nbasenames
                                If Mid(stng1, Len(stng1) - 2, 1) = basenames(j) Then
                                    If Left(fragmentnames(n), 1) = "s" Then
                                        fragmentmass(n, k) = fragmentmass(n, 1) + wxyz_s(i, 8) + abcd(1, 8) + aB(j, 8)
                                    Else
                                        fragmentmass(n, k) = fragmentmass(n, 1) + wxyz(i, 8) + abcd(1, 8) + aB(j, 8)
                                    End If
                                    j = Nbasenames
                                End If
                            Next j
                        Next i
                    Else
                        For i = 1 To Nwxyz
                            For j = 1 To Nabcd
                                k = (i - 1) * (Nabcd + 1) + 1 + j
                                m = fragmentmeta(n, 2) + 1
                                If m <= Nbases Then
                                    If Left(oligo(m, 1), 1) = "s" Then
                                        If Left(fragmentnames(n), 1) = "s" Then
                                            fragmentmass(n, k) = fragmentmass(n, 1) + wxyz_s(i, 8) + abcd_s(j, 8)
                                        Else
                                            fragmentmass(n, k) = fragmentmass(n, 1) + wxyz(i, 8) + abcd_s(j, 8)
                                        End If
                                    Else
                                        If Left(fragmentnames(n), 1) = "s" Then
                                            fragmentmass(n, k) = fragmentmass(n, 1) + wxyz_s(i, 8) + abcd(j, 8)
                                        Else
                                            fragmentmass(n, k) = fragmentmass(n, 1) + wxyz(i, 8) + abcd(j, 8)
                                        End If
                                    End If
                                Else
                                    If Left(fragmentnames(n), 1) = "s" Then
                                        fragmentmass(n, k) = fragmentmass(n, 1) + wxyz_s(i, 8) + abcd(j, 8)
                                    Else
                                        fragmentmass(n, k) = fragmentmass(n, 1) + wxyz(i, 8) + abcd(j, 8)
                                    End If
                                End If
                            Next j
                            k = (i - 1) * (Nabcd + 1) + 1 + Nabcd + 1
                            stng1 = fragmentnames(n)
                            For j = 1 To Nbasenames
                                If Mid(stng1, Len(stng1) - 2, 1) = basenames(j) Then
                                    If Left(fragmentnames(n), 1) = "s" Then
                                        fragmentmass(n, k) = fragmentmass(n, 1) + wxyz_s(i, 8) + abcd(1, 8) + aB(j, 8)
                                    Else
                                        fragmentmass(n, k) = fragmentmass(n, 1) + wxyz(i, 8) + abcd(1, 8) + aB(j, 8)
                                    End If
                                    j = Nbasenames
                                End If
                            Next j
                        Next i
                    End If
                End If
            Next q
        Next p

        'combine lists

        Nneutralcols = Nwxyz * (Nabcd + 1) + Nabcd + 1 + Nwxyz
        Nneutrals = Nladder5 + Nladder3 + Nfragments
        ReDim neutralnames(Nneutrals)
        ReDim neutralmeta(Nneutrals, 2)
        ReDim neutralcols(Nneutralcols)
        ReDim neutralmass(Nneutrals, Nneutralcols)
        n = 0
        For i = 1 To Nabcd
            n = n + 1
            neutralcols(n) = "5'end " & abcd(i, 1)
        Next i
        n = n + 1
        neutralcols(n) = "5'end " & "a-B"
        For i = 1 To Nwxyz
            n = n + 1
            neutralcols(n) = wxyz(i, 1) & " 3'end"
        Next i
        For i = 1 To Nwxyz
            For j = 1 To Nabcd
                n = n + 1
                neutralcols(n) = wxyz(i, 1) & " " & abcd(j, 1)
            Next j
            n = n + 1
            neutralcols(n) = wxyz(i, 1) & " a-B"
        Next i
        n = 0
        For i = 1 To Nladder5
            n = n + 1
            neutralnames(n) = ladder5(i, 3)
            neutralmeta(n, 1) = ladder5(i, 1)
            neutralmeta(n, 2) = ladder5(i, 2)
            m = 0
            For j = 1 To Nabcd + 1
                m = m + 1
                neutralmass(n, m) = ladder5(i, 4 + j)
            Next j
        Next i
        lng2 = m
        For i = 1 To Nladder3
            n = n + 1
            neutralnames(n) = ladder3(i, 3)
            neutralmeta(n, 1) = ladder3(i, 1)
            neutralmeta(n, 2) = ladder3(i, 2)
            m = lng2
            For j = 1 To Nwxyz
                m = m + 1
                neutralmass(n, m) = ladder3(i, 4 + j)
            Next j
        Next i
        lng2 = m
        For i = 1 To Nfragments
            n = n + 1
            neutralnames(n) = fragmentnames(i)
            neutralmeta(n, 1) = fragmentmeta(i, 1)
            neutralmeta(n, 2) = fragmentmeta(i, 2)
            m = lng2
            For j = 1 To Nwxyz
                For k = 1 To Nabcd + 1
                    m = m + 1
                    p = (j - 1) * (Nabcd + 1) + 1 + k
                    neutralmass(n, m) = fragmentmass(i, p)
                Next k
            Next j
        Next i

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
        ReDim outputwrite(0 To Nneutrals, 0 To Nneutralcols + 2)
        outputwrite(0, 0) = "Sequence"
        outputwrite(0, 1) = "Start"
        outputwrite(0, 2) = "End"
        For i = 1 To Nneutralcols
            outputwrite(0, i + 2) = neutralcols(i)
        Next i
        For i = 1 To Nneutrals
            outputwrite(i, 0) = neutralnames(i)
            outputwrite(i, 1) = neutralmeta(i, 1)
            outputwrite(i, 2) = neutralmeta(i, 2)
            For j = 1 To Nneutralcols
                thing1 = neutralmass(i, j)
                If thing1 <> 0 Then outputwrite(i, j + 2) = neutralmass(i, j)
            Next j
        Next i
    Set rng1 = Range(Cells(1, 1), Cells(Nneutrals + 1, Nneutralcols + 3))
    rng1 = outputwrite
        For i = 1 To Nneutralcols
            Columns(i + 3).NumberFormat = "0.000"
        Next i

        'Build single column mass list

        Dim lng99 As Long
        Nmasses = 0
        For i = 1 To Nneutralcols
            For j = 1 To Nneutrals
                If Len(Cells(j + 1, i + 3)) > 0 Then
                    Nmasses = Nmasses + 1
                End If
            Next j
        Next i
        lng99 = Nmasses
        Nmasses = Nmasses + NbasesInSequence * 2
        ReDim masses(Nmasses, 6)
        n = 0
        For i = 1 To Nneutralcols
            stng1 = Cells(1, i + 3)
            For j = 1 To Nneutrals
                If Len(Cells(j + 1, i + 3)) > 0 Then
                    n = n + 1
                    lng1 = Cells(j + 1, 2)
                    lng2 = Cells(j + 1, 3)
                    If InStr(stng1, "end") > 0 Then
                        If lng1 = 1 And lng2 = Nbases Then 'doesn't matter if we call the precursor 5'end or 3'end; go with "5'end d"
                            stng2 = Right(stng1, Len(stng1) - InStr(stng1, " ")) & lng2
                            masses(n, 4) = kinds(1)
                            stng3 = "d"
                        Else
                            If lng1 = 1 Then
                                stng2 = Right(stng1, Len(stng1) - InStr(stng1, " ")) & lng2
                                masses(n, 4) = kinds(1)
                                stng3 = Cells(1, i + 3)
                                stng3 = WorksheetFunction.Substitute(stng3, "5'end ", "")
                            Else    'lng2 must be = Nbases
                                stng2 = Left(stng1, InStr(stng1, " ") - 1) & (Nbases - lng1 + 1)
                                masses(n, 4) = kinds(2)
                                stng3 = Cells(1, i + 3)
                                stng3 = WorksheetFunction.Substitute(stng3, " 3'end", "")
                            End If
                        End If
                    Else
                        stng2 = Mid(basesonly, lng1, 1) & lng1 & "-" & Mid(basesonly, lng2, 1) & lng2
                        masses(n, 4) = kinds(3)
                        If InStr(stng1, "-B") > 0 Then
                            stng2 = stng2 & "-B"
                        End If
                        stng3 = Cells(1, i + 3)
                    End If
                    masses(n, 1) = stng2
                    masses(n, 2) = Cells(j + 1, i + 3)
                    masses(n, 3) = Cells(j + 1, 1)
                    masses(n, 5) = stng3
                    masses(n, 6) = "[" & lng1 & "-" & lng2 & "]"
                End If
            Next j
        Next i

        ' Add extra losses of bases from 5'ladder d (the precursor molecule)

        For i = 1 To NbasesInSequence
            masses(lng99 + i, 1) = "M" & " minus " & BasesInSequence(i, 1)
            masses(lng99 + i, 2) = sequence_mass + BasesInSequence(i, 2)
            masses(lng99 + i, 3) = sequence_longname
            masses(lng99 + i, 4) = kinds(1)
            masses(lng99 + i, 5) = "base loss"
            masses(lng99 + i, 6) = "[" & 1 & "-" & Len(sequence_name) & "]"
        Next i
        For i = 1 To lng99
            If Round(masses(i, 2), 5) = Round(sequence_mass, 5) Then
                masses(i, 1) = "M"
                masses(i, 3) = sequence_longname
                masses(i, 5) = "molecular ion"
            End If
        Next i
        lng99 = lng99 + NbasesInSequence
        For i = 1 To NbasesInSequence
            masses(lng99 + i, 1) = BasesInSequence(i, 1) & "(base)"
            masses(lng99 + i, 2) = -1 * BasesInSequence(i, 2)
            masses(lng99 + i, 3) = ""
            masses(lng99 + i, 4) = kinds(3)
            masses(lng99 + i, 5) = "base loss"
            masses(lng99 + i, 6) = ""
        Next i

        ThisWorkbook.Worksheets(4).Activate
        Cells.Select
        Selection.ClearContents
        Selection.Font.Bold = "false"
        Selection.NumberFormat = "General"
        With Selection.Interior
            .Pattern = xlNone
            .TintAndShade = 0
            .PatternTintAndShade = 0
        End With
        With Selection.Borders
            .LineStyle = xlNone
        End With
        ReDim outputwrite(0 To Nmasses, 0 To 5)
        outputwrite(0, 0) = "Fragment"
        outputwrite(0, 1) = "Mass"
        outputwrite(0, 2) = "Sequence"
        outputwrite(0, 3) = "Type"
        outputwrite(0, 4) = "Cut Type"
        outputwrite(0, 5) = "Start-End"
        For i = 1 To Nmasses
            For j = 1 To 6
                outputwrite(i, j - 1) = masses(i, j)
            Next j
        Next i
    Set rng1 = Range(Cells(1, 1), Cells(Nmasses + 1, 6))
    rng1 = outputwrite
        Columns(2).NumberFormat = "0.000"

        'Sort the list, then condense for like masses

        Columns("A:F").Select
        ActiveWorkbook.Worksheets(4).Sort.SortFields.Clear
        ActiveWorkbook.Worksheets(4).Sort.SortFields.Add2 _
        Key:=Range(Cells(2, 2), Cells(Nmasses + 1, 2)), SortOn:=xlSortOnValues, Order:=xlAscending,
        DataOption:=xlSortNormal
    With ActiveWorkbook.Worksheets(4).Sort
            .SetRange rng1
        .Header = xlYes
            .MatchCase = False
            .Orientation = xlTopToBottom
            .SortMethod = xlPinYin
            .Apply
        End With
        Nuniquemasses = 0
        For i = 1 To Nmasses
            Nuniquemasses = Nuniquemasses + 1
            thing1 = Round(Cells(i + 1, 2), 5)
            For j = i + 1 To Nmasses
                If Round(Cells(j + 1, 2), 5) = thing1 Then
                    i = i + 1
                Else
                    j = Nmasses
                End If
            Next j
        Next i
        ReDim uniquemasses(Nuniquemasses, 6)
        n = 0
        For i = 1 To Nmasses
            n = n + 1
            thing1 = Round(Cells(i + 1, 2), 5)
            stng1 = Cells(i + 1, 1)
            stng2 = Cells(i + 1, 3)
            stng3 = Cells(i + 1, 4)
            stng4 = Cells(i + 1, 5)
            stng5 = Cells(i + 1, 6)
            For j = i + 1 To Nmasses
                If Round(Cells(j + 1, 2), 5) = thing1 Then
                    i = i + 1
                    If Right(stng3, 6) <> "Ladder" Then
                        If Right(Cells(j + 1, 4), 6) = "Ladder" Then
                            stng1 = Cells(j + 1, 1)
                            stng2 = Cells(j + 1, 3)
                            stng3 = Cells(j + 1, 4)
                            stng4 = Cells(j + 1, 5)
                            stng5 = Cells(j + 1, 6)
                        Else                                            'only combine internal fragment info
                            stng1 = stng1 & ", " & Cells(j + 1, 1)
                            stng2 = stng2 & ", " & Cells(j + 1, 3)
                        End If
                    End If
                Else
                    j = Nmasses
                    uniquemasses(n, 1) = stng1
                    uniquemasses(n, 2) = thing1
                    uniquemasses(n, 3) = stng2
                    uniquemasses(n, 4) = stng3
                    uniquemasses(n, 5) = stng4
                    uniquemasses(n, 6) = stng5
                    '               If InStr(stng1, "/") > 0 Then
                    '                   uniquemasses(n, 4) = kinds(3)
                    '               Else
                    '                   check = False
                    '                   For k = 1 To Nwxyz
                    '                       If Left(stng1, 1) = wxyz(k, 1) Then
                    '                           k = Nwxyz
                    '                           uniquemasses(n, 4) = kinds(2)
                    '                           check = True
                    '                       End If
                    '                   Next k
                    '                   If Not check Then
                    '                       uniquemasses(n, 4) = kinds(1)
                    '                   End If
                    '               End If
                End If
            Next j
        Next i
        uniquemasses(Nuniquemasses, 1) = stng1
        uniquemasses(Nuniquemasses, 2) = thing1
        uniquemasses(Nuniquemasses, 3) = stng2
        uniquemasses(Nuniquemasses, 4) = stng3
        uniquemasses(Nuniquemasses, 5) = stng4
        uniquemasses(Nuniquemasses, 6) = stng5
        ThisWorkbook.Worksheets(4).Activate
        Cells.Select
        Selection.ClearContents
        Selection.Font.Bold = "false"
        Selection.NumberFormat = "General"
        With Selection.Interior
            .Pattern = xlNone
            .TintAndShade = 0
            .PatternTintAndShade = 0
        End With
        With Selection.Borders
            .LineStyle = xlNone
        End With
        ReDim outputwrite(0 To Nuniquemasses, 0 To 5)
        outputwrite(0, 0) = "Fragment(s)"
        outputwrite(0, 1) = "Mass"
        outputwrite(0, 2) = "Sequence"
        outputwrite(0, 3) = "Type"
        outputwrite(0, 4) = "Cut Type"
        outputwrite(0, 5) = "Start-End"
        For i = 1 To Nuniquemasses
            For j = 1 To 6
                outputwrite(i, j - 1) = uniquemasses(i, j)
            Next j
        Next i
    Set rng1 = Range(Cells(1, 1), Cells(Nuniquemasses + 1, 6))
    rng1 = outputwrite
        Columns(2).NumberFormat = "0.0000"

        'Consider each mass in the mass list. Determine its possible charge state(s).
        'Deconvolute the mass and compare to the theoretical list. Move to the next mass.
        'If no CS is defined, assume a -1 CS.

        aplus1frac = 0.1

        'Find base peak
        basepeakint = 0
        For i = 1 To Npeaks
            If basepeakint < peaks(i, 2) Then basepeakint = peaks(i, 2)
        Next i
        'Normalize to base peak
        For i = 1 To Npeaks
            peaks(i, 2) = peaks(i, 2) / basepeakint * 100
        Next i
        'Proceed with deconvolution.
        For i = 1 To Npeaks 'presume the peaks are sorted low to high
            If peaks(i, 2) >= peakthresh Then
                thing1 = peaks(i, 1)
                check = False
                For CS = precursorCS To 2 Step -1
                    thing2 = thing1 + Aplus1 / CS
                    thing3 = thing2 / 1000000.0# * aplus1tol
                    thing4 = thing2 - thing3
                    thing5 = thing2 + thing3
                    For j = i + 1 To Npeaks
                        If peaks(j, 1) >= thing4 Then
                            If peaks(j, 1) <= thing5 Then
                                If peaks(j, 2) >= aplus1frac * peaks(i, 2) Then
                                    peaks(i, 3) = 0 - CS
                                    peaks(i, 4) = thing1 * CS + CS * proton
                                    check = True
                                    GoTo outCSloop
                                End If
                            Else
                                j = Npeaks
                            End If
                        End If
                    Next j
                Next CS
outCSloop:
                If Not check Then
                    peaks(i, 3) = -1
                    peaks(i, 4) = thing1 + proton
                End If
            End If
        Next i

        'match peaks

        Dim peakmatch() As Object
        ReDim peakmatch(Npeaks, 6)

        ThisWorkbook.Worksheets(4).Activate
    Set rng1 = Range(Cells(2, 2), Cells(Nuniquemasses + 1, 2))
    For i = 1 To Npeaks
            If Len(peaks(i, 4)) > 0 Then
                check = False
                stng1 = ""
                stng2 = ""
                thing1 = peaks(i, 4)
                thing2 = thing1 / 1000000.0# * matchtol
                thing3 = thing1 - thing2
                thing4 = thing1 + thing2
                If thing3 >= Cells(2, 2) And thing3 <= Cells(Nuniquemasses + 1, 2) Then
                    lng1 = WorksheetFunction.Match(thing3, rng1, 1) + 1
                    For k = lng1 To Nuniquemasses + 1
                        thing5 = Cells(k, 2)
                        If thing5 > thing4 Then
                            k = Nuniquemasses + 1
                        Else
                            If thing5 >= thing3 Then
                                If check = True Then
                                    If Abs(thing1 - thing5) < Abs(thing1 - peakmatch(i, 2)) Then    'closest match wins
                                        peakmatch(i, 1) = Cells(k, 1)
                                        peakmatch(i, 2) = Cells(k, 2)
                                        peakmatch(i, 3) = Cells(k, 3)
                                        peakmatch(i, 4) = Cells(k, 4)
                                        peakmatch(i, 5) = Cells(k, 5)
                                        peakmatch(i, 6) = Cells(k, 6)
                                    End If
                                Else
                                    check = True
                                    peakmatch(i, 1) = Cells(k, 1)
                                    peakmatch(i, 2) = Cells(k, 2)
                                    peakmatch(i, 3) = Cells(k, 3)
                                    peakmatch(i, 4) = Cells(k, 4)
                                    peakmatch(i, 5) = Cells(k, 5)
                                    peakmatch(i, 6) = Cells(k, 6)
                                End If
                            End If
                        End If
                    Next k
                End If
            End If
        Next i

        ThisWorkbook.Worksheets(5).Activate
        Cells.Select
        Selection.ClearContents
        Selection.Font.Bold = "false"
        Selection.NumberFormat = "General"
        With Selection.Interior
            .Pattern = xlNone
            .TintAndShade = 0
            .PatternTintAndShade = 0
        End With
        With Selection.Borders
            .LineStyle = xlNone
        End With
        ReDim outputwrite(0 To Npeaks, 0 To 3)
        outputwrite(0, 0) = "m/z"
        outputwrite(0, 1) = "Normalized Intensity"
        outputwrite(0, 2) = "Charge State"
        outputwrite(0, 3) = "Mass"
        For i = 1 To Npeaks
            For j = 1 To 4
                outputwrite(i, j - 1) = peaks(i, j)
            Next j
        Next i
    Set rng1 = Range(Cells(1, 1), Cells(Npeaks + 1, 4))
    rng1 = outputwrite
        Columns(2).NumberFormat = "0.000"
        Columns(4).NumberFormat = "0.000"

        ReDim outputwrite(0 To Npeaks, 0 To 8)
        outputwrite(0, 0) = "Match"
        outputwrite(0, 1) = "m/z"
        outputwrite(0, 2) = "Sequence"
        outputwrite(0, 3) = "Type"
        outputwrite(0, 4) = "Cut Type"
        outputwrite(0, 5) = "Observed Mass"
        outputwrite(0, 6) = "Theoretical Mass"
        outputwrite(0, 7) = "Error (ppm)"
        outputwrite(0, 8) = "Start-End"
        For i = 1 To Npeaks
            If Len(peakmatch(i, 1)) > 0 Then
                thing1 = peakmatch(i, 2)
                outputwrite(i, 5) = Cells(i + 1, 1) * Abs(Cells(i + 1, 3)) + Abs(Cells(i + 1, 3)) * proton
                outputwrite(i, 6) = thing1
                outputwrite(i, 7) = (outputwrite(i, 5) - outputwrite(i, 6)) / outputwrite(i, 6) * 1000000.0#
                CS = Abs(peaks(i, 3))
                peakmatch(i, 2) = (thing1 - CS * proton) / CS
                stng1 = peakmatch(i, 1)
                peakmatch(i, 1) = stng1 & " " & CS & "-"
            End If
            For j = 1 To 5
                outputwrite(i, j - 1) = peakmatch(i, j)
            Next j
            outputwrite(i, 8) = peakmatch(i, 6)
        Next i
    Set rng1 = Range(Cells(1, 5), Cells(Npeaks + 1, 13))
    rng1 = outputwrite
        Columns(10).NumberFormat = "0.0000"
        Columns(11).NumberFormat = "0.0000"
        Columns(12).NumberFormat = "0.0"

        'Make plot

        ThisWorkbook.Worksheets(6).Activate
        Cells.Select
        Selection.ClearContents
        Selection.Font.Bold = "false"
        Selection.NumberFormat = "General"
        With Selection.Interior
            .Pattern = xlNone
            .TintAndShade = 0
            .PatternTintAndShade = 0
        End With
        With Selection.Borders
            .LineStyle = xlNone
        End With

        gapx = 0.001
        gapy = -10
        ReDim plotpeaks(3 * Npeaks, 2)
        For i = 1 To Npeaks
            j = (i - 1) * 3 + 1
            thing1 = peaks(i, 1)
            plotpeaks(j, 1) = thing1 - gapx
            plotpeaks(j, 2) = gapy
            plotpeaks(j + 1, 1) = thing1
            plotpeaks(j + 1, 2) = peaks(i, 2)
            plotpeaks(j + 2, 1) = thing1 + gapx
            plotpeaks(j + 2, 2) = gapy
        Next i
        ReDim outputwrite(0 To 3 * Npeaks, 0 To 1)
        outputwrite(0, 0) = "m/z"
        outputwrite(0, 1) = "Observed"
        For i = 1 To Npeaks * 3
            For j = 1 To 2
                outputwrite(i, j - 1) = plotpeaks(i, j)
            Next j
        Next i
    Set rng1 = Range(Cells(1, 1), Cells(Npeaks * 3 + 1, 2))
    rng1 = outputwrite
        Columns(1).NumberFormat = "0.000"
        Columns(2).NumberFormat = "0.0"
        lng2 = 2
        If ignoreinternals Then Nkinds = 2
        For k = 1 To Nkinds
            ReDim tempcheck(Npeaks)
            lng1 = 0
            For i = 1 To Npeaks
                If peakmatch(i, 4) = kinds(k) Then
                    lng1 = lng1 + 1
                    tempcheck(i) = True
                End If
            Next i
            If lng1 > 0 Then
                ReDim plotpeaks(3 * lng1, 4)
                n = 0
                For i = 1 To Npeaks
                    If tempcheck(i) Then
                        n = n + 1
                        j = (n - 1) * 3 + 1
                        thing1 = peakmatch(i, 2)
                        plotpeaks(j, 1) = thing1 - gapx
                        plotpeaks(j, 2) = gapy
                        plotpeaks(j + 1, 1) = thing1
                        plotpeaks(j + 1, 2) = peaks(i, 2)
                        stng1 = peakmatch(i, 1)
                        stng2 = Right(stng1, 3)
                        stng2 = WorksheetFunction.Substitute(stng2, " ", "")
                        lng3 = InStr(stng1, ",")
                        If lng3 > 0 Then
                            stng1 = "[" & Left(stng1, lng3 - 1) & "...]" & stng2
                        End If
                        plotpeaks(j + 1, 3) = stng1
                        plotpeaks(j + 1, 4) = peakmatch(i, 3)
                        plotpeaks(j + 2, 1) = thing1 + gapx
                        plotpeaks(j + 2, 2) = gapy
                    End If
                Next i
                ReDim outputwrite(0 To 3 * Npeaks, 0 To 3)
                outputwrite(0, 0) = "m/z"
                outputwrite(0, 1) = kinds(k)
                outputwrite(0, 2) = "Ion"
                outputwrite(0, 3) = "Sequence"
                For i = 1 To 3 * lng1
                    For j = 1 To 4
                        outputwrite(i, j - 1) = plotpeaks(i, j)
                    Next j
                Next i
            Set rng1 = Range(Cells(1, lng2 + 1), Cells(Npeaks * 3 + 1, lng2 + 4))
            rng1 = outputwrite
            Else
                Cells(1, lng2 + 1) = "m/z"
                Cells(1, lng2 + 2) = kinds(k)
                Cells(2, lng2 + 1) = gapy
                Cells(2, lng2 + 2) = gapy
            End If
            Columns(lng2 + 1).NumberFormat = "0.000"
            Columns(lng2 + 2).NumberFormat = "0.0"
            lng2 = lng2 + 4
        Next k

        lng1 = WorksheetFunction.Count(Columns(1))
        lng2 = WorksheetFunction.Count(Columns(3))
        lng3 = WorksheetFunction.Count(Columns(7))
        lng4 = WorksheetFunction.Count(Columns(11))

    Set rng1 = Range(Cells(2, 1), Cells(lng1, 1))
    Set rng2 = Range(Cells(2, 2), Cells(lng1, 2))
    Set rng3 = Range(Cells(2, 3), Cells(lng2, 3))
    Set rng4 = Range(Cells(2, 4), Cells(lng2, 4))
    Set rng5 = Range(Cells(2, 7), Cells(lng3, 7))
    Set rng6 = Range(Cells(2, 8), Cells(lng3, 8))
    If Not ignoreinternals Then
        Set rng7 = Range(Cells(2, 11), Cells(lng4, 11))
        Set rng8 = Range(Cells(2, 12), Cells(lng4, 12))
    End If

        Cells(1, 1).Select
        Application.ScreenUpdating = False
        Application.DisplayAlerts = False
        ActiveWorkbook.Charts.Delete
        Application.DisplayAlerts = True
        ActiveWorkbook.Charts.Add
        With ActiveChart
            .ChartType = xlXYScatterLinesNoMarkers
            ActiveChart.SetSourceData Source:=rng1
        ActiveChart.SeriesCollection(1).XValues = rng1
            ActiveChart.SeriesCollection(1).Values = rng2
            n = 1
            For i = 1 To ActiveChart.SeriesCollection(1).Points.Count
                ActiveChart.SeriesCollection(1).Points(i).HasDataLabel = False
            Next i
            ActiveChart.SeriesCollection(1).Format.Line.Visible = msoTrue
            ActiveChart.SeriesCollection(1).Format.Line.ForeColor.RGB = RGB(0, 0, 0)
            ActiveChart.SeriesCollection(1).Format.Line.Weight = 1
            ActiveChart.SeriesCollection(1).Format.Line.Transparency = 0
            ActiveChart.SeriesCollection(1).Name = "Observed"
            If lng2 > 1 Then
                n = n + 1
                ActiveChart.SeriesCollection.Add Source:=rng3
            ActiveChart.SeriesCollection(n).XValues = rng3
                ActiveChart.SeriesCollection(n).Values = rng4
                For i = 1 To ActiveChart.SeriesCollection(n).Points.Count
                    ActiveChart.SeriesCollection(n).Points(i).HasDataLabel = False
                Next i
                ActiveChart.SeriesCollection(n).Format.Line.Visible = msoTrue
                ActiveChart.SeriesCollection(n).Format.Line.ForeColor.RGB = RGB(0, 0, 255)
                ActiveChart.SeriesCollection(n).Format.Line.Weight = 1
                ActiveChart.SeriesCollection(n).Format.Line.Transparency = 0
                ActiveChart.SeriesCollection(n).Name = kinds(1)
                thing2 = 0
                For i = 1 To lng2 / 3
                    j = (i - 1) * 3 + 3
                    If ThisWorkbook.Worksheets(6).Cells(j, 4) > labelthresh Then
                        thing1 = ThisWorkbook.Worksheets(6).Cells(j, 3)    'm/z
                        If thing1 >= thing2 Then
                            thing2 = thing1 + labelbin
                            thing3 = 0
                            For k = i To lng2 / 3
                                m = (k - 1) * 3 + 3
                                thing4 = ThisWorkbook.Worksheets(6).Cells(m, 3)
                                If thing4 < thing2 Then
                                    thing5 = ThisWorkbook.Worksheets(6).Cells(m, 4)
                                    If thing5 > thing3 Then
                                        thing3 = thing5
                                        p = m
                                    End If
                                Else
                                    k = lng2
                                End If
                            Next k
                            stng1 = ThisWorkbook.Worksheets(6).Cells(p, 5)
                            p = p - 1
                            ActiveChart.SeriesCollection(n).Points(p).HasDataLabel = True
                            ActiveChart.SeriesCollection(n).Points(p).DataLabel.Text = stng1
                            ActiveChart.SeriesCollection(n).Points(p).DataLabel.Position = xlLabelPositionAbove
                            ActiveChart.SeriesCollection(n).Points(p).DataLabel.Font.Size = 6
                        End If
                    End If
                Next i
                ActiveChart.ChartArea.Select
                ActiveChart.FullSeriesCollection(n).DataLabels.Select
                With Selection.Format.TextFrame2.TextRange.Font.Fill
                    .Visible = msoTrue
                    .ForeColor.RGB = RGB(0, 0, 255)
                    .Transparency = 0
                    .Solid
                End With

            End If
            If lng3 > 1 Then
                n = n + 1
                ActiveChart.SeriesCollection.Add Source:=rng5
            ActiveChart.SeriesCollection(n).XValues = rng5
                ActiveChart.SeriesCollection(n).Values = rng6
                For i = 1 To ActiveChart.SeriesCollection(n).Points.Count
                    ActiveChart.SeriesCollection(n).Points(i).HasDataLabel = False
                Next i
                ActiveChart.SeriesCollection(n).Format.Line.Visible = msoTrue
                ActiveChart.SeriesCollection(n).Format.Line.ForeColor.RGB = RGB(0, 150, 0)
                ActiveChart.SeriesCollection(n).Format.Line.Weight = 1
                ActiveChart.SeriesCollection(n).Format.Line.Transparency = 0
                ActiveChart.SeriesCollection(n).Name = kinds(2)
                thing2 = 0
                For i = 1 To lng3 / 3
                    j = (i - 1) * 3 + 3
                    If ThisWorkbook.Worksheets(6).Cells(j, 8) > labelthresh Then
                        thing1 = ThisWorkbook.Worksheets(6).Cells(j, 7)    'm/z
                        If thing1 >= thing2 Then
                            thing2 = thing1 + labelbin
                            thing3 = 0
                            For k = i To lng3 / 3
                                m = (k - 1) * 3 + 3
                                thing4 = ThisWorkbook.Worksheets(6).Cells(m, 7)
                                If thing4 < thing2 Then
                                    thing5 = ThisWorkbook.Worksheets(6).Cells(m, 8)
                                    If thing5 > thing3 Then
                                        thing3 = thing5
                                        p = m
                                    End If
                                Else
                                    k = lng3
                                End If
                            Next k
                            stng1 = ThisWorkbook.Worksheets(6).Cells(p, 9)
                            p = p - 1
                            ActiveChart.SeriesCollection(n).Points(p).HasDataLabel = True
                            ActiveChart.SeriesCollection(n).Points(p).DataLabel.Text = stng1
                            ActiveChart.SeriesCollection(n).Points(p).DataLabel.Position = xlLabelPositionAbove
                            ActiveChart.SeriesCollection(n).Points(p).DataLabel.Font.Size = 6
                        End If
                    End If
                Next i
                ActiveChart.ChartArea.Select
                ActiveChart.FullSeriesCollection(n).DataLabels.Select
                With Selection.Format.TextFrame2.TextRange.Font.Fill
                    .Visible = msoTrue
                    .ForeColor.RGB = RGB(0, 150, 0)
                    .Transparency = 0
                    .Solid
                End With
            End If
            If lng4 > 1 Then
                n = n + 1
                ActiveChart.SeriesCollection.Add Source:=rng7
            ActiveChart.SeriesCollection(n).XValues = rng7
                ActiveChart.SeriesCollection(n).Values = rng8
                For i = 1 To ActiveChart.SeriesCollection(n).Points.Count
                    ActiveChart.SeriesCollection(n).Points(i).HasDataLabel = False
                Next i
                ActiveChart.SeriesCollection(n).Format.Line.Visible = msoTrue
                ActiveChart.SeriesCollection(n).Format.Line.ForeColor.RGB = RGB(255, 0, 0)
                ActiveChart.SeriesCollection(n).Format.Line.Weight = 1
                ActiveChart.SeriesCollection(n).Format.Line.Transparency = 0
                ActiveChart.SeriesCollection(n).Name = kinds(3)
                thing2 = 0
                For i = 1 To lng4 / 3
                    j = (i - 1) * 3 + 3
                    If ThisWorkbook.Worksheets(6).Cells(j, 12) > labelthresh Then
                        thing1 = ThisWorkbook.Worksheets(6).Cells(j, 11)    'm/z
                        If thing1 >= thing2 Then
                            thing2 = thing1 + labelbin
                            thing3 = 0
                            For k = i To lng4 / 3
                                m = (k - 1) * 3 + 3
                                thing4 = ThisWorkbook.Worksheets(6).Cells(m, 11)
                                If thing4 < thing2 Then
                                    thing5 = ThisWorkbook.Worksheets(6).Cells(m, 12)
                                    If thing5 > thing3 Then
                                        thing3 = thing5
                                        p = m
                                    End If
                                Else
                                    k = lng4
                                End If
                            Next k
                            stng1 = ThisWorkbook.Worksheets(6).Cells(p, 13)
                            p = p - 1
                            ActiveChart.SeriesCollection(n).Points(p).HasDataLabel = True
                            ActiveChart.SeriesCollection(n).Points(p).DataLabel.Text = stng1
                            ActiveChart.SeriesCollection(n).Points(p).DataLabel.Position = xlLabelPositionAbove
                            ActiveChart.SeriesCollection(n).Points(p).DataLabel.Font.Size = 6
                        End If
                    End If
                Next i
                ActiveChart.ChartArea.Select
                ActiveChart.FullSeriesCollection(n).DataLabels.Select
                With Selection.Format.TextFrame2.TextRange.Font.Fill
                    .Visible = msoTrue
                    .ForeColor.RGB = RGB(255, 0, 0)
                    .Transparency = 0
                    .Solid
                End With
            End If
            ActiveChart.ChartArea.Select
            ActiveChart.SetElement(msoElementLegendNone)
            ActiveChart.SetElement(msoElementPrimaryValueGridLinesNone)
            ActiveChart.SetElement(msoElementPrimaryCategoryAxisTitleAdjacentToAxis)
            Selection.Caption = "m/z"
            ActiveChart.SetElement(msoElementPrimaryValueAxisTitleRotated)
            Selection.Caption = "Relative Intensity (%)"
            ActiveChart.Axes(xlValue).Select
            ActiveChart.Axes(xlValue).MinimumScale = 0
            ActiveChart.Axes(xlValue).MaximumScale = 120
            ActiveChart.Axes(xlValue).MaximumScale = 100
            With Selection.Format.Line
                .Visible = msoTrue
                .ForeColor.ObjectThemeColor = msoThemeColorText1
                .ForeColor.TintAndShade = 0
                .ForeColor.Brightness = 0
                .Transparency = 0
            End With
            ActiveChart.Axes(xlValue).Select
            Selection.TickLabels.NumberFormat = "General"
            ActiveChart.Axes(xlCategory).Select
            ActiveChart.Axes(xlCategory).MinimumScale = 0
            ActiveChart.Axes(xlCategory).MinimumScale = 100
            With Selection.Format.Line
                .Visible = msoTrue
                .ForeColor.ObjectThemeColor = msoThemeColorText1
                .ForeColor.TintAndShade = 0
                .ForeColor.Brightness = 0
                .Transparency = 0
            End With
            ActiveChart.Axes(xlCategory).Select
            Selection.TickLabels.NumberFormat = "General"

            ActiveChart.Name = "Spectrum Match Plot"
            ActiveChart.ChartArea.Select
            Selection.Border.LineStyle = xlNone
            Selection.Fill.Visible = msoFalse
            lng1 = Selection.Width
            lng2 = Int(Log(maxint) / Log(10))
            thing1 = 10 ^ lng2
            thing1 = Round(maxint / thing1, 2)
            ActiveChart.Shapes.AddLabel(msoTextOrientationHorizontal, lng1 - 100, 0, 100, 120).Select
            Selection.ShapeRange(1).TextFrame2.TextRange.Characters.Text = filetext & " " & scantext & " " & vbCr & scannum _
         & vbCr & "NL: " & thing1 & "E" & lng2 & vbCr & RT
            Selection.ShapeRange(1).TextFrame2.TextRange.Characters(1, 4).ParagraphFormat.FirstLineIndent = 0
            Selection.Font.Size = 8
            ActiveChart.SetElement(msoElementLegendRightOverlay)
        End With
        ActiveChart.SetElement(msoElementChartTitleCenteredOverlay)
        ActiveChart.ChartTitle.Text = "Chart Title"
        Selection.Format.TextFrame2.TextRange.Characters.Text = "Chart Title"
        With Selection.Format.TextFrame2.TextRange.Characters(1, 11).ParagraphFormat
            .TextDirection = msoTextDirectionLeftToRight
            .Alignment = msoAlignCenter
        End With
        With Selection.Format.TextFrame2.TextRange.Characters(1, 11).Font
            .BaselineOffset = 0
            .Bold = msoTrue
            .NameComplexScript = "+mn-cs"
            .NameFarEast = "+mn-ea"
            .Fill.Visible = msoTrue
            .Fill.ForeColor.RGB = RGB(0, 0, 0)
            .Fill.Transparency = 0
            .Fill.Solid
            .Size = 18
            .Italic = msoFalse
            .Kerning = 12
            .Name = "+mn-lt"
            .UnderlineStyle = msoNoUnderline
            .Strike = msoNoStrike
        End With
        Selection.Caption = sequence_name
        Application.ScreenUpdating = True

        ThisWorkbook.Worksheets(5).Activate
        lng1 = WorksheetFunction.CountIf(Columns(8), kinds(1))
        lng2 = WorksheetFunction.CountIf(Columns(8), kinds(2))
        lng3 = WorksheetFunction.Count(Columns(1))
        lng4 = WorksheetFunction.CountIf(Columns(9), "base loss")
        Dim observedLadder() As Object, NobservedLadder As Long
        'NobservedLadder = lng1 + lng2 - lng4
        NobservedLadder = 0
        For i = 1 To lng3
            stng1 = Cells(i + 1, 8)
            If stng1 = kinds(1) Or stng1 = kinds(2) Then
                stng2 = Cells(i + 1, 9)
                If stng2 <> "base loss" Then
                    NobservedLadder = NobservedLadder + 1
                End If
            End If
        Next i
        ReDim observedLadder(NobservedLadder, 15)   '14th row is fragment column, 15th row is fragment row
        n = 0
        For i = 1 To lng3
            stng1 = Cells(i + 1, 8)
            If stng1 = kinds(1) Or stng1 = kinds(2) Then
                stng2 = Cells(i + 1, 9)
                If stng2 <> "base loss" Then
                    n = n + 1
                    For j = 1 To 13
                        observedLadder(n, j) = Cells(i + 1, j)
                    Next j
                End If
            End If
        Next i

        For i = 1 To NobservedLadder
            stng1 = observedLadder(i, 9)
            If stng1 = "a" Then
                observedLadder(i, 14) = 1
            Else
                If stng1 = "a-B" Then
                    observedLadder(i, 14) = 2
                Else
                    If stng1 = "b" Then
                        observedLadder(i, 14) = 3
                    Else
                        If stng1 = "c" Then
                            observedLadder(i, 14) = 4
                        Else
                            If stng1 = "d" Then
                                observedLadder(i, 14) = 5
                            Else
                                If stng1 = "w" Then
                                    observedLadder(i, 14) = 9
                                Else
                                    If stng1 = "x" Then
                                        observedLadder(i, 14) = 10
                                    Else
                                        If stng1 = "y" Then
                                            observedLadder(i, 14) = 11
                                        Else
                                            If stng1 = "z" Then
                                                observedLadder(i, 14) = 12
                                            Else    'stng1 = "M"
                                                observedLadder(i, 14) = 5
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
            stng2 = observedLadder(i, 8)
            stng3 = observedLadder(i, 13)
            lng1 = InStr(stng3, "-")
            If stng2 = kinds(1) Then    '5'Ladder
                observedLadder(i, 15) = Mid(stng3, 4, Len(stng3) - lng1 - 1)
            Else    '3'Ladder
                observedLadder(i, 15) = Mid(stng3, 2, lng1 - 2)
            End If
        Next i
        Dim chartions() As String, chartions2() As Object
        ReDim chartions(Nbases, 12)
        ReDim chartions2(Nbases, 12)
        For i = 1 To NobservedLadder
            j = observedLadder(i, 15)
            k = observedLadder(i, 14)
            thing1 = observedLadder(i, 2) ' / 100 * basepeakint
            chartions2(j, k) = chartions2(j, k) + thing1
            If Len(chartions(j, k)) = 0 Then
                chartions(j, k) = "(" & Round(observedLadder(i, 1), 1) & ")" & observedLadder(i, 3)
            Else
                chartions(j, k) = chartions(j, k) & ", (" & Round(observedLadder(i, 1), 1) & ")" & observedLadder(i, 3)
            End If
        Next i


        ThisWorkbook.Worksheets(7).Activate
        Cells.Select
        Selection.ClearContents
        Selection.Font.Bold = False
        Selection.Font.Superscript = False
        Selection.NumberFormat = "General"
        With Selection.Interior
            .Pattern = xlNone
            .TintAndShade = 0
            .PatternTintAndShade = 0
        End With
        With Selection.Borders
            .LineStyle = xlNone
        End With
        lng1 = Nbases + 2
        lng2 = 12
        ReDim outputwrite(0 To lng1 - 1, 0 To lng2 - 1)
        For i = 1 To Nbases
            For j = 1 To lng2
                outputwrite(i + 1, j - 1) = chartions(i, j)
            Next j
        Next i
        outputwrite(0, 0) = "5'Fragments"
        outputwrite(1, 0) = "a"
        outputwrite(1, 1) = "a-B"
        outputwrite(1, 2) = "b"
        outputwrite(1, 3) = "c"
        outputwrite(1, 4) = "d"
        outputwrite(1, 5) = "5'-3' Index"
        outputwrite(1, 6) = "Nucleotide"
        outputwrite(1, 7) = "3'-5' Index"
        outputwrite(0, 8) = "3'Fragments"
        outputwrite(1, 8) = "w"
        outputwrite(1, 9) = "x"
        outputwrite(1, 10) = "y"
        outputwrite(1, 11) = "z"
        For i = 1 To Nbases
            outputwrite(i + 1, 5) = i
            outputwrite(i + 1, 6) = Mid(basesonly, i, 1)
            outputwrite(i + 1, 7) = Nbases - i + 1
        Next i
    Set rng1 = Range(Cells(2, 1), Cells(lng1 + 1, lng2))
    rng1 = outputwrite
        For i = 1 To Nbases
            For j = 1 To 5
                stng1 = Cells(i + 3, j) & ", "
                lng1 = Len(stng1) - Len(WorksheetFunction.Substitute(stng1, ",", ""))
                lng4 = 0
                For k = 1 To lng1
                    lng2 = InStr(stng1, "-")
                    lng3 = InStr(stng1, ",")
                    stng1 = Right(stng1, Len(stng1) - lng3 - 1)
                    Cells(i + 3, j).Characters(Start:=lng2 + lng4, Length:=lng3 - lng2).Font.Superscript = True
                    lng4 = lng3 + 1
                Next k
            Next j
            For j = 9 To 12
                stng1 = Cells(i + 3, j) & ", "
                lng1 = Len(stng1) - Len(WorksheetFunction.Substitute(stng1, ",", ""))
                lng4 = 0
                For k = 1 To lng1
                    lng2 = InStr(stng1, "-")
                    lng3 = InStr(stng1, ",")
                    stng1 = Right(stng1, Len(stng1) - lng3 - 1)
                    Cells(i + 3, j).Characters(Start:=lng2 + lng4, Length:=lng3 - lng2).Font.Superscript = True
                    lng4 = lng3 + 1
                Next k
            Next j
        Next i
        Cells.WrapText = True

        lng1 = Nbases + 2
        lng2 = 12
        ReDim outputwrite(0 To lng1 - 1, 0 To lng2 - 1)
        For i = 1 To Nbases
            For j = 1 To lng2
                outputwrite(i + 1, j - 1) = chartions2(i, j)
            Next j
        Next i
        outputwrite(0, 0) = "5'Fragments"
        outputwrite(1, 0) = "a"
        outputwrite(1, 1) = "a-B"
        outputwrite(1, 2) = "b"
        outputwrite(1, 3) = "c"
        outputwrite(1, 4) = "d"
        outputwrite(1, 5) = "5'-3' Index"
        outputwrite(1, 6) = "Nucleotide"
        outputwrite(1, 7) = "3'-5' Index"
        outputwrite(0, 8) = "3'Fragments"
        outputwrite(1, 8) = "w"
        outputwrite(1, 9) = "x"
        outputwrite(1, 10) = "y"
        outputwrite(1, 11) = "z"
        For i = 1 To Nbases
            outputwrite(i + 1, 5) = i
            outputwrite(i + 1, 6) = Mid(basesonly, i, 1)
            outputwrite(i + 1, 7) = Nbases - i + 1
        Next i
    Set rng1 = Range(Cells(2, 1 + 15), Cells(lng1 + 1, lng2 + 15))
    rng1 = outputwrite
        For i = 1 To 5
            Columns(i + 15).NumberFormat = "0.0"
        Next i
        For i = 1 To 4
            Columns(i + 23).NumberFormat = "0.0"
        Next i
        For i = 1 To Nbases
            For j = 1 To 5
                thing1 = chartions2(i, j)
                If thing1 > 0 Then
                    For k = 2 To Ncolors
                        If thing1 > colors(k - 1, 1) And thing1 <= colors(k, 1) Then
                            Cells(i + 3, j).Interior.Color = colors(k, 2)
                            k = Ncolors
                        End If
                    Next k
                End If
            Next j
            For j = 9 To 12
                thing1 = chartions2(i, j)
                If thing1 > 0 Then
                    For k = 2 To Ncolors
                        If thing1 > colors(k - 1, 1) And thing1 <= colors(k, 1) Then
                            Cells(i + 3, j).Interior.Color = colors(k, 2)
                            k = Ncolors
                        End If
                    Next k
                End If
            Next j
        Next i
        Rows(1).Font.Bold = True
        Rows(2).Font.Bold = True
        Rows(3).Font.Bold = True
        Rows(1).WrapText = False
        Rows(1).HorizontalAlignment = xlLeft
        Cells(1, 1) = "Observed Fragment Ions"
        Cells(1, 16) = "Fragment Ion Intensities (% Base Peak)"

    End Sub
End Class