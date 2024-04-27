#Region "Microsoft.VisualBasic::58c84233ff897f0bdf3a0a2de6b71bd6, G:/mzkit/src/mzmath/Oligonucleotide_MS//OligonucleotideComposition/Oligonucleotide_Composition_from_Mass_Calculator_v2.vb"

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

    '   Total Lines: 194
    '    Code Lines: 165
    ' Comment Lines: 4
    '   Blank Lines: 25
    '     File Size: 8.40 KB


    ' Class Composition
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: (+2 Overloads) FindCompositions, GetMass
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Statistics

''' <summary>
''' Oligonucleotide_Composition_from_Mass_Calculator_v2
''' </summary>
Public Class Composition

    ReadOnly ppmthresh As Double
    ReadOnly water As Double = GetMass("Water", True)
    ReadOnly mods() As Element
    ReadOnly bases() As Element
    ReadOnly Nmods As Integer

    Const Nbases = 4

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

        Nmods = mods.Length
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

    Public Iterator Function FindCompositions(ParamArray Mass() As Double) As IEnumerable(Of OligonucleotideCompositionOutput)
        Dim Massin As MassWindow() = Mass.Select(Function(mz) New MassWindow(mz, ppmthresh)).ToArray
        Dim Nmassin As Long = Massin.Length
        Dim lowbasemass As Double = 1000000000.0#, highbasemass As Double = 0

        For i As Integer = 0 To Nbases - 1
            Dim thing1 As Double = bases(i).isotopic

            If thing1 > highbasemass Then highbasemass = thing1
            If thing1 < lowbasemass Then lowbasemass = thing1
        Next

        For k As Integer = 0 To Nmassin - 1
            ' outputs
            For Each hit As OligonucleotideCompositionOutput In FindCompositions(Massin(k), lowbasemass, highbasemass)
                Yield hit
            Next
        Next
    End Function

    Private Iterator Function FindCompositions(Massin As MassWindow, lowbasemass As Double, highbasemass As Double) As IEnumerable(Of OligonucleotideCompositionOutput)
        Dim thing1 As Double
        Dim thing2 As Double
        Dim lng1 As Integer
        Dim lng2 As Integer
        Dim combins() As Dim4
        Dim nextindex() As Dim4
        Dim sumaccross() As Dim4
        Dim lng5, lng6, lng7 As Integer
        Dim tempcheck() As Boolean

        For j As Integer = 0 To Nmods - 1
            thing1 = mods(j).isotopic
            thing2 = Massin.mzmin - thing1
            lng1 = Int(thing2 / highbasemass) + 1
            thing2 = Massin.mzmax - thing1
            lng2 = Int(thing2 / lowbasemass) + 1

            For m As Integer = lng1 To lng2
                Dim topways = m + Nbases - 1
                Dim bottomways = m
                Dim lng3 = SpecialFunctions.Combination(topways, bottomways)
                Dim lng4 = 0

                ReDim combins(lng3 - 1)
                ReDim nextindex(lng3 - 1)
                ReDim sumaccross(lng3 - 1)

                For n As Integer = 0 To lng3 - 1
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
                For q As Integer = 1 To Nbases - 1
                    For n As Integer = 0 To lng3 - 1
                        Dim lngg2 = sumaccross(n).Cells(q)
                        Dim lngg3 = m - lngg2
                        Dim lngg4 = 0

                        For ii = 1 To nextindex(n).Cells(q)
                            Dim lngg5 = lngg3 - lngg4   'value
                            Dim lngg6 = lngg3 - lngg5   'remainder
                            If lngg6 <= 0 Then
                                combins(n).Cells(q + 1) = lngg5
                                nextindex(n).Cells(q + 1) = 1
                                sumaccross(n).Cells(q + 1) = sumaccross(n).Cells(q) + combins(n).Cells(q + 1)
                                n = n + 1
                            Else
                                topways = lngg6 + Nbases - q - 1 - 1
                                If topways >= 0 Then
                                    Dim lngg7 = SpecialFunctions.Combination(topways, lngg6)
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

                For n As Integer = 0 To lng3 - 1
                    Dim thing3 = thing1 + water
                    For jj = 0 To Nbases - 1
                        thing3 = thing3 + bases(jj).isotopic * combins(n).Cells(jj + 1)
                    Next jj
                    If thing3 <= Massin.mzmax Then
                        If thing3 >= Massin.mzmin Then
                            tempcheck(n) = True
                        End If
                    End If
                Next n

                If tempcheck.Any(Function(t) t) Then
                    Dim outputwrite As New OligonucleotideCompositionOutput

                    For ii = 0 To lng3 - 1
                        If tempcheck(ii) Then
                            outputwrite.ObservedMass = Massin.mass
                            Dim thing3 = thing1 + water
                            For jj As Integer = 0 To Nbases - 1
                                thing3 = thing3 + bases(jj).isotopic * combins(ii).Cells(jj + 1)
                            Next jj
                            outputwrite.TheoreticalMass = thing3
                            outputwrite.ErrorPpm = (Massin.mass - thing3) / thing3 * 1000000.0#
                            Dim lngg7 As Integer = 0
                            For kk As Integer = 1 To Nbases
                                outputwrite.SetBaseNumber(kk, combins(ii).Cells(kk))
                                lngg7 = lngg7 + combins(ii).Cells(kk)
                            Next kk
                            outputwrite.Modification = mods(j).name
                            outputwrite.OfBases = lngg7
                        End If
                    Next ii

                    Yield outputwrite
                End If
            Next m
        Next j
    End Function
End Class
