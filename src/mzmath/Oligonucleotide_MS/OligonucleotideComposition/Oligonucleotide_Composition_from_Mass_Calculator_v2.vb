Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.Math.Statistics
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' Oligonucleotide_Composition_from_Mass_Calculator_v2
''' </summary>
Public Class Composition

    ReadOnly ppmthresh As Double
    ReadOnly water As Double = GetMass("Water", True)
    ReadOnly mods() As Element
    ReadOnly bases() As Element

    Sub New(ppm As Double, Optional modifications As IEnumerable(Of Element) = Nothing, Optional Monoisotopic As Boolean = True)
        Me.bases = MassDefault.GetBases(Monoisotopic).ToArray
        Me.ppmthresh = ppm
        Me.water = GetMass("Water", Monoisotopic)

        If modifications Is Nothing Then
            mods = MassDefault.GetModifications(Monoisotopic).ToArray
        Else
            mods = New Element() {
                MassDefault.zero
            } _
                .JoinIterates(modifications) _
                .ToArray
        End If
    End Sub

    Private Shared Function GetMass(id As String, monoisotopic As Boolean) As Double
        Static _monoisotopic As Dictionary(Of String, Element) = GetElements(True).ToDictionary(Function(a) a.name)
        Static _average_mass As Dictionary(Of String, Element) = GetElements(False).ToDictionary(Function(a) a.name)

        If monoisotopic Then
            Return _monoisotopic(id).isotopic
        Else
            Return _average_mass(id).isotopic
        End If
    End Function

    Private Class Dim4

        Public Cells As Long()

        Sub New()
            Cells = New Long(4) {}
        End Sub

        Public Overrides Function ToString() As String
            Return Cells.GetJson
        End Function

    End Class

    Public Iterator Function FindCompositions(Mass() As Double) As IEnumerable(Of OligonucleotideCompositionOutput)

        Dim j As Long, k As Long, m As Long, n As Long, q As Long, p As Long
        Dim ii As Long, jj As Long, kk As Long, mm As Long, nn As Long, qq As Long, pp As Long
        Dim lng1 As Long, lng2 As Long, lng3 As Long, lng4 As Long, lng5 As Long, lng6 As Long, lng7 As Long
        Dim lngg1 As Long, lngg2 As Long, lngg3 As Long, lngg4 As Long, lngg5 As Long, lngg6 As Long, lngg7 As Long
        Dim thing1 As Double, thing2 As Double, thing3 As Double, thing4 As Double
        Dim stng1 As String, stng2 As String
        Dim var1 As Object
        Dim rng1 As Range
        Dim tempcheck() As Boolean
        Dim columnover As Long, columnover2 As Long
        Dim Massin As MassWindow() = Mass.Select(Function(mz) New MassWindow(mz, ppmthresh)).ToArray
        Dim Nmassin As Long = Massin.Length


        Dim Nbases = 4


        Dim Nmods As Integer = mods.Length
        Dim lowbasemass As Double, highbasemass As Double
        lowbasemass = 1000000000.0#
        highbasemass = 0
        For i = 0 To Nbases - 1
            thing1 = bases(i).isotopic
            If thing1 > highbasemass Then highbasemass = thing1
            If thing1 < lowbasemass Then lowbasemass = thing1
        Next i

        Dim combins() As Dim4
        Dim topways As Long, bottomways As Long
        Dim nextindex() As Dim4, sumaccross() As Dim4
        Dim Nmatch As Long, matches() As Object, Ncats As Long

        ' outputs
        columnover2 = 0
        For k = 0 To Nmassin - 1

            columnover = 0
            Ncats = 0
            For j = 0 To Nmods - 1
                thing1 = mods(j).isotopic
                thing2 = Massin(k).mzmin - thing1
                lng1 = Int(thing2 / highbasemass) + 1
                thing2 = Massin(k).mzmax - thing1
                lng2 = Int(thing2 / lowbasemass) + 1
                For m = lng1 To lng2
                    topways = m + Nbases - 1
                    bottomways = m
                    lng3 = SpecialFunctions.Combination(topways, bottomways)
                    ReDim combins(lng3 - 1)
                    ReDim nextindex(lng3 - 1)
                    ReDim sumaccross(lng3 - 1)
                    lng4 = 0
                    For n = 0 To lng3 - 1
                        lng5 = bottomways - lng4     'value
                        lng6 = bottomways - lng5     'remainder
                        lng7 = SpecialFunctions.Combination(lng6 + Nbases - 2, lng6)
                        For p = 1 To lng7
                            combins(n) = New Dim4
                            nextindex(n) = New Dim4
                            sumaccross(n) = New Dim4

                            combins(n).Cells(1) = lng5
                            nextindex(n).Cells(1) = lng7
                            sumaccross(n).Cells(1) = combins(n).Cells(1)

                            n = n + 1
                        Next p
                        lng4 = lng4 + 1
                        n = n - 1
                    Next n
                    For q = 1 To Nbases - 1
                        For n = 0 To lng3 - 1
                            lngg2 = sumaccross(n).Cells(q)
                            lngg3 = m - lngg2
                            lngg4 = 0
                            For ii = 1 To nextindex(n).Cells(q)
                                lngg5 = lngg3 - lngg4   'value
                                lngg6 = lngg3 - lngg5   'remainder
                                If lngg6 <= 0 Then
                                    combins(n).Cells(q + 1) = lngg5
                                    nextindex(n).Cells(q + 1) = 1
                                    sumaccross(n).Cells(q + 1) = sumaccross(n).Cells(q) + combins(n).Cells(q + 1)
                                    n = n + 1
                                Else
                                    topways = lngg6 + Nbases - q - 1 - 1
                                    If topways >= 0 Then
                                        lngg7 = SpecialFunctions.Combination(topways, lngg6)
                                        For jj = 1 To lngg7
                                            ii = ii + 1
                                            combins(n).Cells(q + 1) = lngg5
                                            nextindex(n).Cells(q + 1) = lngg7
                                            sumaccross(n).Cells(q + 1) = sumaccross(n).Cells(q) + combins(n).Cells(q + 1)
                                            n = n + 1
                                        Next jj
                                        ii = ii - 1
                                    Else
                                        combins(n).Cells(q + 1) = lngg5
                                        nextindex(n).Cells(q + 1) = 1
                                        sumaccross(n).Cells(q + 1) = sumaccross(n).Cells(q) + combins(n).Cells(q + 1)
                                        n = n + 1
                                    End If
                                End If
                                lngg4 = lngg4 + 1
                            Next ii
                            n = n - 1
                        Next n
                    Next q
                    ReDim tempcheck(lng3)
                    Nmatch = 0
                    For n = 0 To lng3 - 1
                        thing3 = thing1 + water
                        For jj = 0 To Nbases - 1
                            thing3 = thing3 + bases(jj).isotopic * combins(n).Cells(jj + 1)
                        Next jj
                        If thing3 <= Massin(k).mzmax Then
                            If thing3 >= Massin(k).mzmin Then
                                tempcheck(n) = True
                                Nmatch = Nmatch + 1
                            End If
                        End If
                    Next n
                    If Nmatch > 0 Then
                        Ncats = Ncats + 1
                        ' ReDim outputwrite(0 To Nmatch - 1, 0 To Nbases + 5)
                        Dim outputwrite As New OligonucleotideCompositionOutput
                        nn = 0
                        For ii = 0 To lng3 - 1
                            If tempcheck(ii) Then
                                outputwrite.ObservedMass = Massin(k).mass
                                thing3 = thing1 + water
                                For jj = 0 To Nbases - 1
                                    thing3 = thing3 + bases(jj).isotopic * combins(ii).Cells(jj + 1)
                                Next jj
                                outputwrite.TheoreticalMass = thing3
                                outputwrite.Errorppm = (Massin(k).mass - thing3) / thing3 * 1000000.0#
                                lngg7 = 0
                                For kk = 1 To Nbases
                                    outputwrite.SetBaseNumber(kk, combins(ii).Cells(kk))
                                    lngg7 = lngg7 + combins(ii).Cells(kk)
                                Next kk
                                outputwrite.Modification = mods(j).name
                                outputwrite.OfBases = lngg7
                                nn = nn + 1
                            End If
                        Next ii

                        Yield outputwrite
                        columnover = columnover + Nbases + 6
                    End If
                Next m
            Next j
        Next k
    End Function
End Class