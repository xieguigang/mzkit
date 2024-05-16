#Region "Microsoft.VisualBasic::45ebd450ebebf486860fc0c5a59eb921, mzmath\Oligonucleotide_MS\MS_Peak\Oligonucleotide_MS_Peak_ID_Given_Sequence_v8.vb"

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

    '   Total Lines: 529
    '    Code Lines: 414
    ' Comment Lines: 50
    '   Blank Lines: 65
    '     File Size: 19.43 KB


    ' Class MS_Peak_ID
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetCoverage, GetMatches, MakeTheoryList, MatchMassesToOligoSequence
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Public Class MS_Peak_ID

    ''' <summary>
    ''' ppm threshold
    ''' </summary>
    ReadOnly ppm As Double = 5
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
            Optional Base_cutsite As String = "G|A|T|C",
            Optional which_side As String = "3'|5'",
            Optional phosphate_site As String = "3' of previous base|5' of next base")

        cutsite1side3 = which_side.Split("|"c).First = "3'"
        cutsite1 = Base_cutsite.Split("|"c).First
        cutsite1Pwith3 = phosphate_site.Split("|"c).First = "3' of previous base"

        Me.ppm = ppm
        Me.Nmiss = miss_sites
        Me.Monoisotopic = Monoisotopic
        Me.bases = MassDefault.GetBases(Monoisotopic).ToArray

        Dim groups As GroupMass() = MassDefault.GetGroupMass(Monoisotopic).ToArray

        end5 = groups.Where(Function(a) a.end = 5).ToArray
        end3 = groups.Where(Function(a) a.end = 3).ToArray
        Nend5 = end5.Length
        Nend3 = end3.Length
    End Sub

    Const OHstng = "HO-"
    Const Hstng = "-H"

    Public Iterator Function MakeTheoryList(seq As FastaSeq, Optional NameString As String = "R") As IEnumerable(Of TheoreticalDigestMass)
        Dim OHthing As Double, Hthing As Double
        Dim lng1 As Integer = 0
        Dim phosphate As Double

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

        For i As Integer = 0 To Nbases - 1
            Dim thing1 = bases(i).isotopic
            If thing1 > highbasemass Then highbasemass = thing1
            If thing1 < lowbasemass Then lowbasemass = thing1
        Next i

        Dim Construct As String = seq.SequenceData
        Dim ConstructName As String = seq.Title
        Dim ConstructLength As Long = seq.Length


        'Create theoretical list of nomisses

        Dim nomisses() As SimpleSegment
        Dim Nnomisses = ConstructLength - Len(Construct.Replace(cutsite1, "")) + 1

        ' sequence, start, stop, length
        ReDim nomisses(Nnomisses)

        For i As Integer = 0 To nomisses.Length - 1
            nomisses(i) = New SimpleSegment
        Next

        If cutsite1side3 Then
            Dim stng1 = Construct
            Dim lng2 = 0
            For i = 1 To Nnomisses - 1
                lng1 = InStr(stng1, cutsite1)
                nomisses(i).SequenceData = Left(stng1, lng1)
                stng1 = Right(stng1, Len(stng1) - lng1)
                nomisses(i).Start = lng2 + 1
                nomisses(i).Ends = lng2 + lng1
                ' nomisses(i)(4) = nomisses(i)(3) - nomisses(i)(2) + 1
                lng2 = lng2 + nomisses(i).Length
            Next i
            nomisses(Nnomisses).SequenceData = stng1
            nomisses(Nnomisses).Start = lng2 + 1
            nomisses(Nnomisses).Ends = ConstructLength
            ' nomisses(Nnomisses)(4) = nomisses(i)(3) - nomisses(i)(2) + 1
        Else
            Dim stng1 = Construct
            Dim lng2 = 0
            lng1 = InStr(stng1, cutsite1)
            nomisses(1).SequenceData = Left(stng1, lng1 - 1)
            nomisses(1).Start = 1
            nomisses(1).Ends = lng1 - 1
            ' nomisses(1)(4) = nomisses(i)(3) - nomisses(i)(2) + 1
            stng1 = Right(stng1, Len(stng1) - lng1)
            lng2 = lng2 + nomisses(1).Length
            For i = 2 To Nnomisses - 1
                lng1 = InStr(stng1, cutsite1)
                nomisses(i).SequenceData = cutsite1 & Left(stng1, lng1 - 1)
                stng1 = Right(stng1, Len(stng1) - lng1)
                nomisses(i).Start = lng2 + 1
                nomisses(i).Ends = lng2 + lng1
                ' nomisses(i)(4) = nomisses(i)(3) - nomisses(i)(2) + 1
                lng2 = lng2 + nomisses(i).Length
            Next i
            nomisses(Nnomisses).SequenceData = cutsite1 & stng1
            nomisses(Nnomisses).Start = lng2 + 1
            nomisses(Nnomisses).Ends = ConstructLength
            ' nomisses(Nnomisses)(4) = nomisses(i)(3) - nomisses(i)(2) + 1
        End If

        Dim misses() As Dim6, missends() As Dim2
        Dim Nmisses = 0

        For i As Integer = 1 To Nmiss
            For j As Integer = 1 To Nnomisses - i
                Nmisses = Nmisses + 1
            Next j
        Next i

        ReDim misses(Nmisses)
        ReDim missends(Nmiss)

        Dim k = 0
        Dim q = 0
        Dim p = 0
        Dim m As Integer = 0

        For i As Integer = 1 To Nmiss
            For j = 1 To Nnomisses - i
                k = k + 1
                Dim stng1 = nomisses(j).SequenceData
                For m = j + 1 To j + i
                    stng1 = stng1 & nomisses(m).SequenceData
                Next m
                misses(k)(1) = stng1
                misses(k)(2) = nomisses(j).Start
                misses(k)(3) = nomisses(m - 1).Ends
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

        Dim digest() As Dim8
        Dim Ndigest = Nnomisses + Nmisses + (Nend5 - 1) * (1 + Nmiss) + (Nend3 - 1) * (1 + Nmiss)
        Dim n = 0

        ReDim digest(Ndigest)

        For i As Integer = 1 To Nnomisses
            n = n + 1

            digest(n) = New Dim8
            digest(n)(1) = nomisses(i).SequenceData
            digest(n)(2) = nomisses(i).Start
            digest(n)(3) = nomisses(i).Ends
            digest(n)(4) = nomisses(i).Length

            If digest(n)(2) = 1 Then
                digest(n)(5) = end5(0).name
                digest(n)(6) = Hstng
                digest(n)(7) = end5(0).mass + Hthing
            Else
                If digest(n)(3) = ConstructLength Then
                    digest(n)(5) = OHstng
                    digest(n)(6) = end3(0).name
                    digest(n)(7) = OHthing + end3(0).mass
                Else
                    digest(n)(5) = OHstng
                    digest(n)(6) = Hstng
                    digest(n)(7) = OHthing + Hthing
                End If
            End If
            digest(n)(8) = NameString & n
        Next i

        For i As Integer = 1 To Nmisses
            n = n + 1
            For j = 1 To 4
                digest(n)(j) = misses(i)(j)
            Next j
            If digest(n)(2) = 1 Then
                digest(n)(5) = end5(0).name
                digest(n)(6) = Hstng
                digest(n)(7) = end5(0).mass + Hthing
            Else
                If digest(n)(3) = ConstructLength Then
                    digest(n)(5) = OHstng
                    digest(n)(6) = end3(0).name
                    digest(n)(7) = OHthing + end3(0).mass
                Else
                    digest(n)(5) = OHstng
                    digest(n)(6) = Hstng
                    digest(n)(7) = OHthing + Hthing
                End If
            End If
            digest(n)(8) = NameString & misses(i)(5) & "-" & misses(i)(6)
        Next i

        For j As Integer = 1 To Nend5 - 1
            n = n + 1

            If digest(n) Is Nothing Then
                digest(n) = New Dim8
            End If

            digest(n)(1) = nomisses(1).SequenceData
            digest(n)(2) = nomisses(1).Start
            digest(n)(3) = nomisses(1).Ends
            digest(n)(4) = nomisses(1).Length

            digest(n)(5) = end5(j).name
            digest(n)(6) = Hstng
            digest(n)(7) = end5(j).mass + Hthing
            digest(n)(8) = NameString & 1
        Next j
        For j = 2 To Nend3
            n = n + 1

            digest(n)(1) = nomisses(Nnomisses).SequenceData
            digest(n)(2) = nomisses(Nnomisses).Start
            digest(n)(3) = nomisses(Nnomisses).Ends
            digest(n)(4) = nomisses(Nnomisses).Length

            digest(n)(5) = OHstng
            digest(n)(6) = end3(j).name
            digest(n)(7) = OHthing + end3(j).mass
            digest(n)(8) = NameString & Nnomisses
        Next j
        lng1 = 0
        For i = 1 To Nmiss
            m = missends(i)(1)
            For j = 2 To Nend5
                n = n + 1
                For k = 1 To 4
                    digest(n)(k) = misses(m)(k)
                Next k
                digest(n)(5) = end5(j).name
                digest(n)(6) = Hstng
                digest(n)(7) = end5(j).mass + Hthing
                digest(n)(8) = NameString & misses(m)(5) & "-" & misses(m)(6)
            Next j
            m = missends(i)(2)
            For j = 2 To Nend3
                n = n + 1
                For k = 1 To 4
                    digest(n)(k) = misses(m)(k)
                Next k
                digest(n)(5) = OHstng
                digest(n)(6) = end3(j).name
                digest(n)(7) = OHthing + end3(j).mass
                digest(n)(8) = NameString & misses(m)(5) & "-" & misses(m)(6)
            Next j
        Next i
        For i = 1 To Ndigest
            For k = 0 To Nbases - 1
                ' sequence
                lng1 = digest(i)(4) - Len(CStr(digest(i)(1)).Replace(bases(k).name, ""))
                digest(i)(7) = digest(i)(7) + lng1 * bases(k).isotopic
            Next k
        Next i

        'Make end corrections. The algorithm above has presumed that phosphate remains on the 5' side of the right fragment.
        'For internal fragments, it doesn't matter if this isn't correct. For fragments starting at the 5' terminus or
        'ending at the 3' terminus, it matters. If instead the phosphate remains on the 3' side of the left fragment,
        'then for 5' terminus fragments the theoretical masses are 80 Da too small. Similarly, for 3' terminus fragments,
        'the theoretical masses are 80 Da too big.

        If cutsite1Pwith3 Then
            For i As Integer = 1 To Ndigest
                If digest(i)(2) = 1 Then
                    digest(i)(7) = digest(i)(7) + phosphate
                End If
                If digest(i)(3) = ConstructLength Then
                    digest(i)(7) = digest(i)(7) - phosphate
                End If
            Next i
        End If

        ' output and export the digest result
        For i As Integer = 1 To Ndigest
            Yield New TheoreticalDigestMass With {
                .Sequence = digest(i)(1),
                .Start = digest(i)(2),
                .Ends = digest(i)(3),
                .Length = digest(i)(4),
                .End5 = digest(i)(5),
                .End3 = digest(i)(6),
                .TheoreticalMass = digest(i)(7),
                .Name = digest(i)(8)
            }
        Next
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="seq"></param>
    ''' <remarks>
    ''' (matrix, "Sequence Coverage (%)")
    ''' </remarks>
    Private Function GetCoverage(matches As Match(), seq As FastaSeq) As (hit As Boolean(), coverage As Double)
        ' set sequence inputs
        Dim Construct As String = seq.SequenceData
        Dim ConstructName As String = seq.Title
        Dim ConstructLength As Long = seq.Length

        ' output matches
        Dim Nmatches As Long = matches.Length

        ' Determine coverage
        ' Map coverage
        Dim Ncovered As Long, PercentCovered As Double, SequenceCoverage() As Boolean
        ReDim SequenceCoverage(ConstructLength)
        Ncovered = 0
        For i = 0 To Nmatches - 1
            For j = matches(i).Start To matches(i).Ends
                If Not SequenceCoverage(j) Then
                    SequenceCoverage(j) = True
                    Ncovered = Ncovered + 1
                End If
            Next j
        Next i
        PercentCovered = Ncovered / ConstructLength * 100

        Return (SequenceCoverage, PercentCovered)
    End Function

    Public Function MatchMassesToOligoSequence(obs As Double(), seq As FastaSeq) As DigestResult
        Dim digest As TheoreticalDigestMass() = MakeTheoryList(seq).ToArray
        Dim matches = GetMatches(obs, digest)

        ' print debug view
        'Call Match.Print(matches.Item1, App.StdOut)

        'For Each mass In matches.Item2
        '    Call Console.WriteLine(mass.ToString)
        'Next

        Dim coverage = GetCoverage(matches.Item1, seq)

        Return New DigestResult With {
            .coverage = coverage.coverage,
            .digest = digest,
            .hit_sites = coverage.hit,
            .mass = obs,
            .mass_hits = matches.Item2,
            .matches = matches.Item1,
            .nt = seq
        }
    End Function

    Private Function GetMatches(obs As Double(), digest As TheoreticalDigestMass()) As (Match(), MatchedInput())
        'Match theoertical masses to observed masses

        Dim Nadducts As Long, adducts() As Element
        Nadducts = 1 ' 4
        ReDim adducts(Nadducts)
        If Nadducts > 1 Then
            adducts(2).name = "Na+"
            adducts(3).name = "K+"
            adducts(4).name = "NH4+"
            If Monoisotopic Then
                adducts(2).isotopic = 21.98194425
                adducts(3).isotopic = 37.95588165
                adducts(4).isotopic = 17.0265491
            Else
                adducts(2).isotopic = 21.981799
                adducts(3).isotopic = 38.09033
                adducts(4).isotopic = 17.03061
            End If
        End If
        adducts(1) = New Element("", 0)

        Dim Nmassin = obs.Length
        Dim Massin As MassWindow() = obs.Select(Function(m) New MassWindow(m, ppm)).ToArray
        Dim Ndigest = digest.Length
        Dim matches() As Match, Nmatches As Long
        Nmatches = 0
        For i As Integer = 0 To Nmassin - 1
            For j As Integer = 0 To Ndigest - 1
                For k = 1 To Nadducts
                    Dim thing1 = digest(j)(7) + adducts(k).isotopic
                    If thing1 >= Massin(i).mzmin Then
                        If thing1 <= Massin(i).mzmax Then
                            Nmatches = Nmatches + 1
                        End If
                    End If
                Next k
            Next j
        Next i

        ' no matches
        If Nmatches = 0 Then
            Return Nothing
        End If

        ReDim matches(Nmatches)
        Dim massinmatch() As String
        ReDim massinmatch(Nmassin)
        Dim n = 0
        For i As Integer = 0 To Nmassin - 1
            Dim stng1 = ""
            For j As Integer = 0 To Ndigest - 1
                For k = 1 To Nadducts
                    Dim thing1 = digest(j)(7) + adducts(k).isotopic
                    If thing1 >= Massin(i).mzmin Then
                        If thing1 <= Massin(i).mzmax Then
                            n = n + 1
                            matches(n) = New Match
                            matches(n).ObservedMass = Massin(i).mass
                            matches(n).Sequence = digest(j)(1)
                            matches(n).Start = digest(j)(2)
                            matches(n).Ends = digest(j)(3)
                            matches(n).Length = digest(j)(4)
                            matches(n).End5 = digest(j)(5)
                            matches(n).End3 = digest(j)(6)
                            matches(n).Adduct = adducts(k).name
                            matches(n).TheoreticalMass = thing1
                            matches(n).ErrorPpm = (Massin(i).mass - thing1) / thing1 * 1000000.0#
                            Dim stng2 = digest(j).Start & digest(j).Sequence & digest(j).Ends
                            If k > 1 Then stng2 = stng2 & "(" & adducts(k).name & ")"
                            stng1 = stng1 & stng2 & ", "
                            matches(n).Name = digest(j).Name
                        End If
                    End If
                Next k
            Next j

            If stng1 <> "" Then
                massinmatch(i + 1) = Left(stng1, Len(stng1) - 2)
            End If
        Next i

        Dim lng1 = Nmatches
        Dim lng2 = 11

        Dim temp As Boolean() = New Boolean(lng1) {}
        Dim outputwrite As Match() = matches

        For i As Integer = 1 To lng1
            If Not temp(i) Then
                Dim stng1 = outputwrite(i).Sequence
                Dim lng3 = 1

                For j = i + 1 To lng1
                    If Not temp(j) Then
                        If outputwrite(j).Sequence = stng1 Then
                            lng3 = lng3 + 1
                            temp(j) = True
                        End If
                    End If
                Next j
                outputwrite(i).f1StOccurance = "x"
                outputwrite(i).Frequency = lng3
                For j = i + 1 To lng1
                    If outputwrite(j).Sequence = stng1 Then
                        outputwrite(j).Frequency = lng3
                    End If
                Next j
            End If
        Next i

        Dim MatchedInputList As New List(Of MatchedInput)

        lng1 = Nmassin
        lng2 = 2

        For i As Integer = 0 To lng1 - 1
            MatchedInputList.Add(New MatchedInput With {
                .ObservedMass = Massin(i).mass,
                .Match = massinmatch(i + 1).StringSplit("\s*,\s*")
            })
        Next i

        ' skip first null element due to the reason of
        ' vb6 array is start from base 1
        outputwrite = outputwrite _
            .Skip(1) _
            .ToArray

        Return (outputwrite, MatchedInputList.ToArray)
    End Function
End Class
