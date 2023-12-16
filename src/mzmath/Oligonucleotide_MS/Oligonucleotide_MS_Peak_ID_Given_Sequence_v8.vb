Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class MS_Peak_ID

    ''' <summary>
    ''' ppm threshold
    ''' </summary>
    ReadOnly ppmthresh As Double = 5
    ''' <summary>
    ''' Base cutsite
    ''' </summary>
    ReadOnly cutsite1 As String
    ''' <summary>
    ''' Which side? true for 3'
    ''' </summary>
    ReadOnly cutsite1side3 As Boolean
    ''' <summary>
    ''' Where does phosphate go at cutsite? true for ``3' of previous base``
    ''' </summary>
    ReadOnly cutsite1Pwith3 As Boolean
    ''' <summary>
    ''' # Missed Cleavage Sites
    ''' </summary>
    ReadOnly Nmiss As Long
    ReadOnly Monoisotopic As Boolean
    ReadOnly bases() As Element

    Const Nbases As Long = 4

    ReadOnly end5() As GroupMass, Nend5 As Long
    ReadOnly end3() As GroupMass, Nend3 As Long

    Sub New(Optional ppm As Double = 5,
            Optional miss_sites As Long = 0,
            Optional Monoisotopic As Boolean = True,
            Optional Base_cutsite As String = "A|T|G|C",
            Optional which_side As String = "3'|5'",
            Optional phosphate_site As String = "3' of previous base|5' of next base")

        cutsite1side3 = which_side.Split("|"c).First = "3'"
        cutsite1 = Base_cutsite.Split("|"c).First
        cutsite1Pwith3 = phosphate_site.Split("|"c).First = "3' of previous base"
        ppmthresh = ppm
        Nmiss = miss_sites
        Me.Monoisotopic = Monoisotopic
        Me.bases = MassDefault.GetBases(Monoisotopic).ToArray

        Dim groups = MassDefault.GetGroupMass(Monoisotopic).ToArray

        end5 = groups.Where(Function(a) a.end = 5).ToArray
        end3 = groups.Where(Function(a) a.end = 3).ToArray
        Nend5 = end5.Length
        Nend3 = end3.Length
    End Sub

    Sub maketheorylist(seq As FastaSeq)

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
        Dim OHthing As Double, Hthing As Double
        Dim OHstng As String, Hstng As String

        Dim phosphate As Double
        Dim Namestring As String

        Namestring = "R"

        OHstng = "HO-"
        Hstng = "-H"

        ' set up inputs and parameters

        If Monoisotopic Then
            OHthing = 17.00273965
            Hthing = 1.007825032
            phosphate = 79.96633089
        Else
            OHthing = 17.00734
            Hthing = 1.00794
            phosphate = 79.979902
        End If


        Dim lowbasemass As Double, highbasemass As Double
        lowbasemass = 1000000000.0#
        highbasemass = 0
        For i = 0 To Nbases - 1
            thing1 = bases(i).isotopic
            If thing1 > highbasemass Then highbasemass = thing1
            If thing1 < lowbasemass Then lowbasemass = thing1
        Next i

        Dim Construct As String = seq.SequenceData
        Dim ConstructName As String = seq.Title
        Dim ConstructLength As Long = seq.Length


        'Create theoretical list of nomisses

        Dim nomisses() As Dim4, Nnomisses As Long
        Nnomisses = ConstructLength - Len(Construct.Replace(cutsite1, "")) + 1
        ReDim nomisses(Nnomisses)    ' sequence, start, stop, length
        If cutsite1side3 Then
            stng1 = Construct
            lng2 = 0
            For i = 1 To Nnomisses - 1
                lng1 = InStr(stng1, cutsite1)
                nomisses(i)(1) = Left(stng1, lng1)
                stng1 = Right(stng1, Len(stng1) - lng1)
                nomisses(i)(2) = lng2 + 1
                nomisses(i)(3) = lng2 + lng1
                nomisses(i)(4) = nomisses(i)(3) - nomisses(i)(2) + 1
                lng2 = lng2 + nomisses(i)(4)
            Next i
            nomisses(Nnomisses)(1) = stng1
            nomisses(Nnomisses)(2) = lng2 + 1
            nomisses(Nnomisses)(3) = ConstructLength
            nomisses(Nnomisses)(4) = nomisses(i)(3) - nomisses(i)(2) + 1
        Else
            stng1 = Construct
            lng2 = 0
            lng1 = InStr(stng1, cutsite1)
            nomisses(1)(1) = Left(stng1, lng1 - 1)
            nomisses(1)(2) = 1
            nomisses(1)(3) = lng1 - 1
            nomisses(1)(4) = nomisses(i)(3) - nomisses(i)(2) + 1
            stng1 = Right(stng1, Len(stng1) - lng1)
            lng2 = lng2 + nomisses(1)(4)
            For i = 2 To Nnomisses - 1
                lng1 = InStr(stng1, cutsite1)
                nomisses(i)(1) = cutsite1 & Left(stng1, lng1 - 1)
                stng1 = Right(stng1, Len(stng1) - lng1)
                nomisses(i)(2) = lng2 + 1
                nomisses(i)(3) = lng2 + lng1
                nomisses(i)(4) = nomisses(i)(3) - nomisses(i)(2) + 1
                lng2 = lng2 + nomisses(i)(4)
            Next i
            nomisses(Nnomisses)(1) = cutsite1 & stng1
            nomisses(Nnomisses)(2) = lng2 + 1
            nomisses(Nnomisses)(3) = ConstructLength
            nomisses(Nnomisses)(4) = nomisses(i)(3) - nomisses(i)(2) + 1
        End If

        Dim misses() As Dim6, Nmisses As Long, missends() As Dim2
        Nmisses = 0
        For i = 1 To Nmiss
            For j = 1 To Nnomisses - i
                Nmisses = Nmisses + 1
            Next j
        Next i
        ReDim misses(Nmisses)
        ReDim missends(Nmiss)
        k = 0
        q = 0
        p = 0
        For i = 1 To Nmiss
            For j = 1 To Nnomisses - i
                k = k + 1
                stng1 = nomisses(j)(1)
                For m = j + 1 To j + i
                    stng1 = stng1 & nomisses(m)(1)
                Next m
                misses(k)(1) = stng1
                misses(k)(2) = nomisses(j)(2)
                misses(k)(3) = nomisses(m - 1)(3)
                misses(k)(4) = misses(k)(3) - misses(k)(2) + 1
                misses(k)(5) = j
                misses(k)(6) = m - 1
                If misses(k)(2) = 1 Then
                    p = p + 1
                    missends(p)(1) = k
                End If
                If misses(k)(3) = ConstructLength Then
                    q = q + 1
                    missends(q)(2) = k
                End If
            Next j
        Next i

        Dim digest() As Dim8, Ndigest As Long
        Ndigest = Nnomisses + Nmisses + (Nend5 - 1) * (1 + Nmiss) + (Nend3 - 1) * (1 + Nmiss)
        ReDim digest(Ndigest)
        n = 0
        For i = 1 To Nnomisses
            n = n + 1
            For j = 1 To 4
                digest(n)(j) = nomisses(i)(j)
            Next j
            If digest(n)(2) = 1 Then
                digest(n)(5) = end5(1, 1)
                digest(n)(6) = Hstng
                digest(n)( 7) = end5(1, 2) + Hthing
            Else
                If digest(n)(3) = ConstructLength Then
                    digest(n)(5) = OHstng
                    digest(n)(6) = end3(1, 1)
                    digest(n)(7) = OHthing + end3(1, 2)
                Else
                    digest(n)(5) = OHstng
                    digest(n)(6) = Hstng
                    digest(n)(7) = OHthing + Hthing
                End If
            End If
            digest(n)(8) = Namestring & n
        Next i
        For i = 1 To Nmisses
            n = n + 1
            For j = 1 To 4
                digest(n)(j) = misses(i)(j)
            Next j
            If digest(n)(2) = 1 Then
                digest(n)(5) = end5(1, 1)
                digest(n)(6) = Hstng
                digest(n)(7) = end5(1, 2) + Hthing
            Else
                If digest(n)(3) = ConstructLength Then
                    digest(n)(5) = OHstng
                    digest(n)(6) = end3(1, 1)
                    digest(n)(7) = OHthing + end3(1, 2)
                Else
                    digest(n)(5) = OHstng
                    digest(n)(6) = Hstng
                    digest(n)(7) = OHthing + Hthing
                End If
            End If
            digest(n)(8) = Namestring & misses(i)(5) & "-" & misses(i)(6)
        Next i
        For j = 2 To Nend5
            n = n + 1
            For k = 1 To 4
                digest(n)(k) = nomisses(1)(k)
            Next k
            digest(n)(5) = end5(j, 1)
            digest(n)(6) = Hstng
            digest(n)(7) = end5(j, 2) + Hthing
            digest(n)(8) = Namestring & 1
        Next j
        For j = 2 To Nend3
            n = n + 1
            For k = 1 To 4
                digest(n)(k) = nomisses(Nnomisses)(k)
            Next k
            digest(n)(5) = OHstng
            digest(n)(6) = end3(j, 1)
            digest(n)(7) = OHthing + end3(j, 2)
            digest(n)(8) = Namestring & Nnomisses
        Next j
        lng1 = 0
        For i = 1 To Nmiss
            m = missends(i)(1)
            For j = 2 To Nend5
                n = n + 1
                For k = 1 To 4
                    digest(n)(k) = misses(m)(k)
                Next k
                digest(n)(5) = end5(j, 1)
                digest(n)(6) = Hstng
                digest(n)(7) = end5(j, 2) + Hthing
                digest(n)(8) = Namestring & misses(m)(5) & "-" & misses(m)(6)
            Next j
            m = missends(i)(2)
            For j = 2 To Nend3
                n = n + 1
                For k = 1 To 4
                    digest(n)(k) = misses(m)(k)
                Next k
                digest(n)(5) = OHstng
                digest(n)(6) = end3(j, 1)
                digest(n)(7) = OHthing + end3(j, 2)
                digest(n)(8) = Namestring & misses(m)(5) & "-" & misses(m)(6)
            Next j
        Next i
        For i = 1 To Ndigest
            For k = 1 To Nbases
                lng1 = digest(i)(4) - Len(digest(i)(1).replace(bases(k, 1), ""))
                digest(i)(7) = digest(i)(7) + lng1 * bases(k, 3)
            Next k
        Next i

        'Make end corrections. The algorithm above has presumed that phosphate remains on the 5' side of the right fragment.
        'For internal fragments, it doesn't matter if this isn't correct. For fragments starting at the 5' terminus or
        'ending at the 3' terminus, it matters. If instead the phosphate remains on the 3' side of the left fragment,
        'then for 5' terminus fragments the theoretical masses are 80 Da too small. Similarly, for 3' terminus fragments,
        'the theoretical masses are 80 Da too big.

        If cutsite1Pwith3 Then
            For i = 1 To Ndigest
                If digest(i, 2) = 1 Then
                    digest(i, 7) = digest(i, 7) + phosphate
                End If
                If digest(i, 3) = ConstructLength Then
                    digest(i, 7) = digest(i, 7) - phosphate
                End If
            Next i
        End If

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
        lng1 = Ndigest
        lng2 = 8
        ReDim outputwrite(0 To lng1, 0 To lng2 - 1)
        outputwrite(0, 0) = "Sequence"
        outputwrite(0, 1) = "Start"
        outputwrite(0, 2) = "End"
        outputwrite(0, 3) = "Length"
        outputwrite(0, 4) = "5' End"
        outputwrite(0, 5) = "3' End"
        outputwrite(0, 6) = "Theoretical Mass"
        outputwrite(0, 7) = "Name"
        For i = 1 To lng1
            For j = 1 To lng2
                outputwrite(i, j - 1) = digest(i, j)
            Next j
        Next i
    Set rng1 = Range(Cells(1, 1), Cells(lng1 + 1, lng2))
    rng1 = outputwrite
        Columns(7).NumberFormat = "0.0000"

    End Sub
    Sub getcoverage()

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

        Dim coveragecolor As Long

        ThisWorkbook.Worksheets(1).Activate
        coveragecolor = Cells(23, 4).Interior.Color

        ThisWorkbook.Worksheets(2).Activate
        Dim Construct As String, ConstructName As String, ConstructLength As Long
        For i = 1 To 100
            If Len(Cells(i + 1, 5)) > 0 Then
                Construct = Cells(i + 1, 4)
                ConstructName = Cells(i + 1, 1)
                ConstructLength = Len(Construct)
                i = 100
            End If
        Next i

        ThisWorkbook.Worksheets(4).Activate
        lng1 = Intersect(ActiveSheet.UsedRange, Columns(1)).Count

        Dim Nmatches As Long, matches() As Object
        For i = 1 To lng1
            If Len(Cells(i + 1, 1)) > 0 Then Nmatches = Nmatches + 1
        Next i
        ReDim matches(Nmatches, 2)
        n = 0
        For i = 1 To lng1
            If Len(Cells(i + 1, 1)) > 0 Then
                n = n + 1
                matches(n, 1) = Cells(i + 1, 3)
                matches(n, 2) = Cells(i + 1, 4)
            End If
        Next i

        ThisWorkbook.Worksheets(6).Activate
        Cells.Select
        Selection.ClearContents
        Selection.Font.Bold = False
        Selection.Font.Color = RGB(0, 0, 0)
        Selection.Font.Strikethrough = False
        Cells.Font.Size = 12
        With Selection.Interior
            .Pattern = xlNone
            .TintAndShade = 0
            .PatternTintAndShade = 0
            .Color = RGB(255, 255, 255)
        End With
        With Selection.Borders
            .LineStyle = xlNone
        End With

        Dim Ncolumns As Long, Nrows As Long
        Dim colorme() As Long

        'Write sequence in fasta format

        Ncolumns = 50
        Nrows = Int(ConstructLength / Ncolumns) + 1
        ReDim outputwrite(0 To Nrows - 1, 0 To Ncolumns + 1)
        ReDim colorme(ConstructLength, 2)
        For i = 1 To ConstructLength
            stng1 = Mid(Construct, i, 1)
            j = (i - 1) Mod Ncolumns + 1
            k = Int((i - 1) / Ncolumns)
            outputwrite(k, j) = stng1
            colorme(i, 1) = k + 1    'row
            colorme(i, 2) = j + 1    'column
        Next i
        For i = 1 To Nrows
            outputwrite(i - 1, 0) = (i - 1) * Ncolumns + 1
            outputwrite(i - 1, Ncolumns + 1) = outputwrite(i - 1, 0) + Ncolumns - 1
        Next i
        outputwrite(Nrows - 1, Ncolumns + 1) = ConstructLength
    Set rng1 = Range(Cells(1, 1), Cells(Nrows, Ncolumns + 2))
    rng1 = outputwrite

        'Determine coverage

        Dim Ncovered As Long, PercentCovered As Double, SequenceCoverage() As Boolean
        ReDim SequenceCoverage(ConstructLength)
        Ncovered = 0
        For i = 1 To Nmatches
            For j = matches(i, 1) To matches(i, 2)
                If Not SequenceCoverage(j) Then
                    SequenceCoverage(j) = True
                    Ncovered = Ncovered + 1
                End If
            Next j
        Next i
        PercentCovered = Ncovered / ConstructLength * 100

        'Map coverage

        For i = 1 To ConstructLength
            If SequenceCoverage(i) Then
                m = colorme(i, 1)   'row for residue j
                n = colorme(i, 2)   'column for residue j
                Cells(m, n).Interior.Color = coveragecolor
                Cells(m, n).Font.TintAndShade = 0
            End If
        Next i

        Cells(1, Ncolumns + 4) = "Sequence Coverage (%)"
        Cells(2, Ncolumns + 4) = PercentCovered


    End Sub
    Sub MatchMassesToOligoSequence()

        getmatches()

        getcoverage()

    End Sub

    Private Sub getmatches()

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
        Dim OHthing As Double, Hthing As Double
        Dim OHstng As String, Hstng As String
        Dim cutsite1side3 As Boolean, cutsite1Pwith3 As Boolean
        Dim phosphate As Double
        Dim Namestring As String

        Namestring = "R"
        OHstng = "HO-"
        Hstng = "-H"

        Dim cutsite1 As String, Nmiss As Long
        cutsite1 = Cells(4, 4)
        If Cells(6, 4) = "3'" Then
            cutsite1side3 = True
        Else
            cutsite1side3 = False
        End If
        If Cells(8, 4) = "3' of previous base" Then
            cutsite1Pwith3 = True
        Else
            cutsite1Pwith3 = False
        End If

        Nmiss = Cells(10, 4)

        ThisWorkbook.Worksheets(1).Activate

        Dim Nmassin As Long, Massin() As Double
        Nmassin = 0
        For i = 1 To 500000
            If Len(Cells(i + 1, 1)) > 0 Then
                Nmassin = Nmassin + 1
            Else
                i = 500000
            End If
        Next i
        ReDim Massin(Nmassin, 3)
        For i = 1 To Nmassin
            Massin(i, 1) = Cells(i + 1, 1)
        Next i


        For i = 1 To Nmassin
            thing1 = Massin(i, 1)
            thing2 = thing1 / 1000000.0# * ppmthresh
            Massin(i, 2) = thing1 - thing2
            Massin(i, 3) = thing1 + thing2
        Next i

        Dim Monoisotopic As Boolean
        If Cells(2, 6) = "Monoisotopic" Then
            Monoisotopic = True
            OHthing = 17.00273965
            Hthing = 1.007825032
            phosphate = 79.96633089
        Else
            Monoisotopic = False
            OHthing = 17.00734
            Hthing = 1.00794
            phosphate = 79.979902
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

        Dim end5() As Object, Nend5 As Long
        Dim end3() As Object, Nend3 As Long
        Nend5 = 0
        Nend3 = 0
        For i = 1 To 10000
            If Len(Cells(i + 1, 14)) > 0 Then
                If Cells(i + 1, 14) = "5'" Then
                    Nend5 = Nend5 + 1
                Else
                    If Cells(i + 1, 14) = "3'" Then
                        Nend3 = Nend3 + 1
                    End If
                End If
            Else
                i = 10000
            End If
        Next i
        j = 0
        k = 0
        If Nend5 > 0 Then ReDim end5(Nend5, 2)
        If Nend3 > 0 Then ReDim end3(Nend3, 2)
        For i = 1 To 10000
            If Len(Cells(i + 1, 14)) > 0 Then
                If Cells(i + 1, 14) = "5'" Then
                    j = j + 1
                    end5(j, 1) = Cells(i + 1, 15)
                    If Monoisotopic Then
                        end5(j, 2) = Cells(i + 1, 16)
                    Else
                        end5(j, 2) = Cells(i + 1, 17)
                    End If
                Else
                    If Cells(i + 1, 14) = "3'" Then
                        k = k + 1
                        end3(k, 1) = Cells(i + 1, 15)
                        If Monoisotopic Then
                            end3(k, 2) = Cells(i + 1, 16)
                        Else
                            end3(k, 2) = Cells(i + 1, 17)
                        End If
                    End If
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

        ThisWorkbook.Worksheets(2).Activate
        Dim Construct As String, ConstructName As String, ConstructLength As Long
        For i = 1 To 100
            If Len(Cells(i + 1, 5)) > 0 Then
                Construct = Cells(i + 1, 4)
                ConstructName = Cells(i + 1, 1)
                ConstructLength = Len(Construct)
                i = 100
            End If
        Next i


        'Create theoretical list of nomisses

        Dim nomisses() As Object, Nnomisses As Long
        Nnomisses = ConstructLength - Len(WorksheetFunction.Substitute(Construct, cutsite1, "")) + 1
        ReDim nomisses(Nnomisses, 4)    'sequence, start, stop, length
        If cutsite1side3 Then
            stng1 = Construct
            lng2 = 0
            For i = 1 To Nnomisses - 1
                lng1 = InStr(stng1, cutsite1)
                nomisses(i, 1) = Left(stng1, lng1)
                stng1 = Right(stng1, Len(stng1) - lng1)
                nomisses(i, 2) = lng2 + 1
                nomisses(i, 3) = lng2 + lng1
                nomisses(i, 4) = nomisses(i, 3) - nomisses(i, 2) + 1
                lng2 = lng2 + nomisses(i, 4)
            Next i
            nomisses(Nnomisses, 1) = stng1
            nomisses(Nnomisses, 2) = lng2 + 1
            nomisses(Nnomisses, 3) = ConstructLength
            nomisses(Nnomisses, 4) = nomisses(i, 3) - nomisses(i, 2) + 1
        Else
            stng1 = Construct
            lng2 = 0
            lng1 = InStr(stng1, cutsite1)
            nomisses(1, 1) = Left(stng1, lng1 - 1)
            nomisses(1, 2) = 1
            nomisses(1, 3) = lng1 - 1
            nomisses(1, 4) = nomisses(i, 3) - nomisses(i, 2) + 1
            stng1 = Right(stng1, Len(stng1) - lng1)
            lng2 = lng2 + nomisses(1, 4)
            For i = 2 To Nnomisses - 1
                lng1 = InStr(stng1, cutsite1)
                nomisses(i, 1) = cutsite1 & Left(stng1, lng1 - 1)
                stng1 = Right(stng1, Len(stng1) - lng1)
                nomisses(i, 2) = lng2 + 1
                nomisses(i, 3) = lng2 + lng1
                nomisses(i, 4) = nomisses(i, 3) - nomisses(i, 2) + 1
                lng2 = lng2 + nomisses(i, 4)
            Next i
            nomisses(Nnomisses, 1) = cutsite1 & stng1
            nomisses(Nnomisses, 2) = lng2 + 1
            nomisses(Nnomisses, 3) = ConstructLength
            nomisses(Nnomisses, 4) = nomisses(i, 3) - nomisses(i, 2) + 1
        End If

        Dim misses() As Object, Nmisses As Long, missends() As Long
        Nmisses = 0
        For i = 1 To Nmiss
            For j = 1 To Nnomisses - i
                Nmisses = Nmisses + 1
            Next j
        Next i
        ReDim misses(Nmisses, 6)
        ReDim missends(Nmiss, 2)
        k = 0
        q = 0
        p = 0
        For i = 1 To Nmiss
            For j = 1 To Nnomisses - i
                k = k + 1
                stng1 = nomisses(j, 1)
                For m = j + 1 To j + i
                    stng1 = stng1 & nomisses(m, 1)
                Next m
                misses(k, 1) = stng1
                misses(k, 2) = nomisses(j, 2)
                misses(k, 3) = nomisses(m - 1, 3)
                misses(k, 4) = misses(k, 3) - misses(k, 2) + 1
                misses(k, 5) = j
                misses(k, 6) = m - 1
                If misses(k, 2) = 1 Then
                    p = p + 1
                    missends(p, 1) = k
                End If
                If misses(k, 3) = ConstructLength Then
                    q = q + 1
                    missends(q, 2) = k
                End If
            Next j
        Next i

        Dim digest() As Object, Ndigest As Long
        Ndigest = Nnomisses + Nmisses + (Nend5 - 1) * (1 + Nmiss) + (Nend3 - 1) * (1 + Nmiss)
        ReDim digest(Ndigest, 8)
        n = 0
        For i = 1 To Nnomisses
            n = n + 1
            For j = 1 To 4
                digest(n, j) = nomisses(i, j)
            Next j
            If digest(n, 2) = 1 Then
                digest(n, 5) = end5(1, 1)
                digest(n, 6) = Hstng
                digest(n, 7) = end5(1, 2) + Hthing
            Else
                If digest(n, 3) = ConstructLength Then
                    digest(n, 5) = OHstng
                    digest(n, 6) = end3(1, 1)
                    digest(n, 7) = OHthing + end3(1, 2)
                Else
                    digest(n, 5) = OHstng
                    digest(n, 6) = Hstng
                    digest(n, 7) = OHthing + Hthing
                End If
            End If
            digest(n, 8) = Namestring & n
        Next i
        For i = 1 To Nmisses
            n = n + 1
            For j = 1 To 4
                digest(n, j) = misses(i, j)
            Next j
            If digest(n, 2) = 1 Then
                digest(n, 5) = end5(1, 1)
                digest(n, 6) = Hstng
                digest(n, 7) = end5(1, 2) + Hthing
            Else
                If digest(n, 3) = ConstructLength Then
                    digest(n, 5) = OHstng
                    digest(n, 6) = end3(1, 1)
                    digest(n, 7) = OHthing + end3(1, 2)
                Else
                    digest(n, 5) = OHstng
                    digest(n, 6) = Hstng
                    digest(n, 7) = OHthing + Hthing
                End If
            End If
            digest(n, 8) = Namestring & misses(i, 5) & "-" & misses(i, 6)
        Next i
        For j = 2 To Nend5
            n = n + 1
            For k = 1 To 4
                digest(n, k) = nomisses(1, k)
            Next k
            digest(n, 5) = end5(j, 1)
            digest(n, 6) = Hstng
            digest(n, 7) = end5(j, 2) + Hthing
            digest(n, 8) = Namestring & 1
        Next j
        For j = 2 To Nend3
            n = n + 1
            For k = 1 To 4
                digest(n, k) = nomisses(Nnomisses, k)
            Next k
            digest(n, 5) = OHstng
            digest(n, 6) = end3(j, 1)
            digest(n, 7) = OHthing + end3(j, 2)
            digest(n, 8) = Namestring & Nnomisses
        Next j
        lng1 = 0
        For i = 1 To Nmiss
            m = missends(i, 1)
            For j = 2 To Nend5
                n = n + 1
                For k = 1 To 4
                    digest(n, k) = misses(m, k)
                Next k
                digest(n, 5) = end5(j, 1)
                digest(n, 6) = Hstng
                digest(n, 7) = end5(j, 2) + Hthing
                digest(n, 8) = Namestring & misses(m, 5) & "-" & misses(m, 6)
            Next j
            m = missends(i, 2)
            For j = 2 To Nend3
                n = n + 1
                For k = 1 To 4
                    digest(n, k) = misses(m, k)
                Next k
                digest(n, 5) = OHstng
                digest(n, 6) = end3(j, 1)
                digest(n, 7) = OHthing + end3(j, 2)
                digest(n, 8) = Namestring & misses(m, 5) & "-" & misses(m, 6)
            Next j
        Next i
        For i = 1 To Ndigest
            For k = 1 To Nbases
                lng1 = digest(i, 4) - Len(WorksheetFunction.Substitute(digest(i, 1), bases(k, 1), ""))
                digest(i, 7) = digest(i, 7) + lng1 * bases(k, 3)
            Next k
        Next i

        'Make end corrections. The algorithm above has presumed that phosphate remains on the 5' side of the right fragment.
        'For internal fragments, it doesn't matter if this isn't correct. For fragments starting at the 5' terminus or
        'ending at the 3' terminus, it matters. If instead the phosphate remains on the 3' side of the left fragment,
        'then for 5' terminus fragments the theoretical masses are 80 Da too small. Similarly, for 3' terminus fragments,
        'the theoretical masses are 80 Da too big.

        If cutsite1Pwith3 Then
            For i = 1 To Ndigest
                If digest(i, 2) = 1 Then
                    digest(i, 7) = digest(i, 7) + phosphate
                End If
                If digest(i, 3) = ConstructLength Then
                    digest(i, 7) = digest(i, 7) - phosphate
                End If
            Next i
        End If

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
        lng1 = Ndigest
        lng2 = 8
        ReDim outputwrite(0 To lng1, 0 To lng2 - 1)
        outputwrite(0, 0) = "Sequence"
        outputwrite(0, 1) = "Start"
        outputwrite(0, 2) = "End"
        outputwrite(0, 3) = "Length"
        outputwrite(0, 4) = "5' End"
        outputwrite(0, 5) = "3' End"
        outputwrite(0, 6) = "Theoretical Mass"
        outputwrite(0, 7) = "Name"
        For i = 1 To lng1
            For j = 1 To lng2
                outputwrite(i, j - 1) = digest(i, j)
            Next j
        Next i
    Set rng1 = Range(Cells(1, 1), Cells(lng1 + 1, lng2))
    rng1 = outputwrite
        Columns(7).NumberFormat = "0.0000"

        'Match theoertical masses to observed masses

        Dim Nadducts As Long, adducts() As Object
        Nadducts = 1 ' 4
        ReDim adducts(Nadducts, 2)
        If Nadducts > 1 Then
            adducts(2, 1) = "Na+"
            adducts(3, 1) = "K+"
            adducts(4, 1) = "NH4+"
            If Monoisotopic Then
                adducts(2, 2) = 21.98194425
                adducts(3, 2) = 37.95588165
                adducts(4, 2) = 17.0265491
            Else
                adducts(2, 2) = 21.981799
                adducts(3, 2) = 38.09033
                adducts(4, 2) = 17.03061
            End If
        End If
        adducts(1, 1) = ""
        adducts(1, 2) = 0

        Dim matches() As Object, Nmatches As Long
        Nmatches = 0
        For i = 1 To Nmassin
            For j = 1 To Ndigest
                For k = 1 To Nadducts
                    thing1 = digest(j, 7) + adducts(k, 2)
                    If thing1 >= Massin(i, 2) Then
                        If thing1 <= Massin(i, 3) Then
                            Nmatches = Nmatches + 1
                        End If
                    End If
                Next k
            Next j
        Next i


        If Nmatches = 0 Then
            MsgBox("No Matches")
            End
        End If

        ReDim matches(Nmatches, 11)
        Dim massinmatch() As String
        ReDim massinmatch(Nmassin)
        n = 0
        For i = 1 To Nmassin
            stng1 = ""
            For j = 1 To Ndigest
                For k = 1 To Nadducts
                    thing1 = digest(j, 7) + adducts(k, 2)
                    If thing1 >= Massin(i, 2) Then
                        If thing1 <= Massin(i, 3) Then
                            n = n + 1
                            matches(n, 1) = Massin(i, 1)
                            For m = 1 To 6
                                matches(n, m + 1) = digest(j, m)
                            Next m
                            matches(n, 8) = adducts(k, 1)
                            matches(n, 9) = thing1
                            matches(n, 10) = (Massin(i, 1) - thing1) / thing1 * 1000000.0#
                            stng2 = digest(j, 2) & digest(j, 1) & digest(j, 3)
                            If k > 1 Then stng2 = stng2 & "(" & adducts(k, 1) & ")"
                            stng1 = stng1 & stng2 & ", "
                            matches(n, 11) = digest(j, 8)
                        End If
                    End If
                Next k
            Next j
            If stng1 <> "" Then massinmatch(i) = Left(stng1, Len(stng1) - 2)
        Next i

        ThisWorkbook.Worksheets(4).Activate
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
        lng1 = Nmatches
        lng2 = 11
        ReDim outputwrite(0 To lng1, 0 To lng2 + 1)
        outputwrite(0, 0) = "Observed Mass"
        outputwrite(0, 1) = "Sequence"
        outputwrite(0, 2) = "Start"
        outputwrite(0, 3) = "End"
        outputwrite(0, 4) = "Length"
        outputwrite(0, 5) = "5' End"
        outputwrite(0, 6) = "3' End"
        outputwrite(0, 7) = "Adduct"
        outputwrite(0, 8) = "Theoretical Mass"
        outputwrite(0, 9) = "Error (ppm)"
        outputwrite(0, 10) = "Name"
        outputwrite(0, 11) = "Frequency"
        outputwrite(0, 12) = "1st Occurance"
        For i = 1 To lng1
            For j = 1 To lng2
                outputwrite(i, j - 1) = matches(i, j)
            Next j
        Next i
        ReDim temp(lng1)
        For i = 1 To lng1
            If Not temp(i) Then
                stng1 = outputwrite(i, 1)
                lng3 = 1
                For j = i + 1 To lng1
                    If Not temp(j) Then
                        If outputwrite(j, 1) = stng1 Then
                            lng3 = lng3 + 1
                            temp(j) = True
                        End If
                    End If
                Next j
                outputwrite(i, 12) = "x"
                outputwrite(i, 11) = lng3
                For j = i + 1 To lng1
                    If outputwrite(j, 1) = stng1 Then
                        outputwrite(j, 11) = lng3
                    End If
                Next j
            End If
        Next i
    Set rng1 = Range(Cells(1, 1), Cells(lng1 + 1, lng2 + 2))
    rng1 = outputwrite
        Columns(1).NumberFormat = "0.0000"
        Columns(9).NumberFormat = "0.0000"
        Columns(10).NumberFormat = "0.0"
        Columns(11).NumberFormat = "0"

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
        lng1 = Nmassin
        lng2 = 2
        ReDim outputwrite(0 To lng1, 0 To lng2 - 1)
        outputwrite(0, 0) = "Observed Mass"
        outputwrite(0, 1) = "Match"
        For i = 1 To lng1
            outputwrite(i, 0) = Massin(i, 1)
            outputwrite(i, 1) = massinmatch(i)
        Next i
    Set rng1 = Range(Cells(1, 1), Cells(lng1 + 1, lng2))
    rng1 = outputwrite
        Columns(1).NumberFormat = "0.0000"

    End Sub
End Class