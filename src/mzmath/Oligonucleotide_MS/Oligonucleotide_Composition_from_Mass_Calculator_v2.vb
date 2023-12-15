Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Math.Statistics

Public Class Composition

    ReadOnly ppmthresh As Double
    ReadOnly baseTable As Dictionary(Of String, Element)

    Private Shared Iterator Function MonoisotopicBases() As IEnumerable(Of Element)
        Yield New Element("A", 329.0525)
        Yield New Element("G", 345.0474)
        Yield New Element("C", 305.0413)
        Yield New Element("V", 320.041)
    End Function

    Private Shared Iterator Function AverageMassBases() As IEnumerable(Of Element)
        Yield New Element("A", 329.2091)
        Yield New Element("G", 345.2085)
        Yield New Element("C", 305.1841)
        Yield New Element("V", 320.1957)
    End Function

    ' Cells(row_id, column_id)

    Private Class Mass

        Public ReadOnly Property mass As Double
        Public Property range As DoubleRange

        Sub New(mass As Double)
            Me.mass = mass
        End Sub

    End Class

    Private Shared Iterator Function MonoisotopicElements() As IEnumerable(Of Element)
        Yield New Element("C", 12)
        Yield New Element("H", 1.007825)
        Yield New Element("N", 14.003074)
        Yield New Element("O", 15.994915)
        Yield New Element("P", 30.973762)
        Yield New Element("S", 31.972071)
        Yield New Element("Water", 18.010565)
        Yield New Element("Proton", 1.0072765)
    End Function

    Private Shared Iterator Function AverageMassElements() As IEnumerable(Of Element)
        Yield New Element("C", 12.011)
        Yield New Element("H", 1.00794)
        Yield New Element("N", 14.00674)
        Yield New Element("O", 15.9994)
        Yield New Element("P", 30.973762)
        Yield New Element("S", 32.066)
        Yield New Element("Water", 18.01528)
        Yield New Element("Proton", 1)
    End Function

    Private Shared Iterator Function MonoisotopicModifications() As IEnumerable(Of Element)
        Yield New Element("minus p", -79.9663)
        Yield New Element("plus p", 79.9663)
        Yield New Element("cp", -18.0106)
    End Function

    Private Shared Iterator Function AverageMassModifications() As IEnumerable(Of Element)
        Yield New Element("minus p", -79.9799)
        Yield New Element("plus p", 79.9799)
        Yield New Element("cp", -18.0153)
    End Function

    Private Shared Function GetMass(id As String, monoisotopic As Boolean) As Double
        Static _monoisotopic As Dictionary(Of String, Element) = MonoisotopicElements.ToDictionary(Function(a) a.name)
        Static _average_mass As Dictionary(Of String, Element) = AverageMassElements.ToDictionary(Function(a) a.name)

        If monoisotopic Then
            Return _monoisotopic(id).isotopic
        Else
            Return _average_mass(id).isotopic
        End If
    End Function

    Private Class Dim4

        Public Cells As Long()

    End Class

    Public Class Output

        ''' <summary>
        ''' 1
        ''' </summary>
        Public ObservedMass As Double
        ''' <summary>
        ''' 2
        ''' </summary>
        Public TheoreticalMass As Double
        ''' <summary>
        ''' 3
        ''' </summary>
        Public Errorppm As Double
        ''' <summary>
        ''' 4
        ''' </summary>
        Public OfpA As Integer
        ''' <summary>
        ''' 5
        ''' </summary>
        Public OfpG As Integer
        ''' <summary>
        ''' 6
        ''' </summary>
        Public OfpC As Integer
        ''' <summary>
        ''' 7
        ''' </summary>
        Public OfpV As Integer
        ''' <summary>
        ''' 8
        ''' </summary>
        Public Modification As String
        ''' <summary>
        ''' 10
        ''' </summary>
        Public OfBases As Integer

        Public Sub SetBaseNumber(i As Integer, n As Integer)
            Select Case i
                Case 1 : OfpA = n
                Case 2 : OfpG = n
                Case 3 : OfpC = n
                Case 4 : OfpV = n

                Case Else
                    Throw New OutOfMemoryException(i)
            End Select
        End Sub

    End Class

    Public Iterator Function FindCompositions(Mass() As Double, Optional Monoisotopic As Boolean = True) As IEnumerable(Of Output)

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
        Dim Massin As Mass() = Mass.Select(Function(mz) New Mass(mz)).ToArray
        Dim Nmassin As Long = Massin.Length

        For i As Integer = 0 To Nmassin - 1
            thing1 = Massin(i).mass
            thing2 = thing1 / 1000000.0# * ppmthresh
            Massin(i).range = New DoubleRange(thing1 - thing2, thing1 + thing2)
        Next i

        Dim water As Double = GetMass("Water", Monoisotopic)

        Dim bases() As Element = If(Monoisotopic, MonoisotopicBases(), AverageMassBases()).ToArray
        Dim Nbases = 4

        Dim mods() As Element = If(Monoisotopic, MonoisotopicModifications(), AverageMassModifications()).ToArray
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
        For k = 1 To Nmassin

            columnover = 0
            Ncats = 0
            For j = 0 To Nmods - 1
                thing1 = mods(j).isotopic
                thing2 = Massin(k).range.Min - thing1
                lng1 = Int(thing2 / highbasemass) + 1
                thing2 = Massin(k).range.Max - thing1
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
                            n = n + 1
                            combins(n).Cells(1) = lng5
                            nextindex(n).Cells(1) = lng7
                            sumaccross(n).Cells(1) = combins(n).Cells(1)
                        Next p
                        lng4 = lng4 + 1
                        n = n - 1
                    Next n
                    For q = 2 To Nbases
                        For n = 0 To lng3 - 1
                            lngg2 = sumaccross(n + 1).Cells(q - 1)
                            lngg3 = m - lngg2
                            lngg4 = 0
                            For ii = 1 To nextindex(n + 1).Cells(q - 1)
                                lngg5 = lngg3 - lngg4   'value
                                lngg6 = lngg3 - lngg5   'remainder
                                If lngg6 <= 0 Then
                                    n = n + 1
                                    combins(n).Cells(q) = lngg5
                                    nextindex(n).Cells(q) = 1
                                    sumaccross(n).Cells(q) = sumaccross(n).Cells(q - 1) + combins(n).Cells(q)
                                Else
                                    topways = lngg6 + Nbases - q - 1
                                    If topways >= 0 Then
                                        lngg7 = SpecialFunctions.Combination(topways, lngg6)
                                        For jj = 1 To lngg7
                                            n = n + 1
                                            ii = ii + 1
                                            combins(n).Cells(q) = lngg5
                                            nextindex(n).Cells(q) = lngg7
                                            sumaccross(n).Cells(q) = sumaccross(n).Cells(q - 1) + combins(n).Cells(q)
                                        Next jj
                                        ii = ii - 1
                                    Else
                                        n = n + 1
                                        combins(n).Cells(q) = lngg5
                                        nextindex(n).Cells(q) = 1
                                        sumaccross(n).Cells(q) = sumaccross(n).Cells(q - 1) + combins(n).Cells(q)
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
                            thing3 = thing3 + bases(jj).isotopic * combins(n).Cells(jj)
                        Next jj
                        If thing3 <= Massin(k).range.Max Then
                            If thing3 >= Massin(k).range.Min Then
                                tempcheck(n) = True
                                Nmatch = Nmatch + 1
                            End If
                        End If
                    Next n
                    If Nmatch > 0 Then
                        Ncats = Ncats + 1
                        ' ReDim outputwrite(0 To Nmatch - 1, 0 To Nbases + 5)
                        Dim outputwrite As New Output
                        nn = 0
                        For ii = 1 To lng3
                            If tempcheck(ii) Then
                                outputwrite.ObservedMass = Massin(k).mass
                                thing3 = thing1 + water
                                For jj = 1 To Nbases
                                    thing3 = thing3 + bases(jj).isotopic * combins(ii).Cells(jj)
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