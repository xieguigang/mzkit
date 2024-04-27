#Region "Microsoft.VisualBasic::9203c4c62e3ab076c5513c6e8bae711b, G:/mzkit/src/metadb/Lipidomics//SpectrumGenerator/EidSpecificSpectrumGenerator.vb"

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

    '   Total Lines: 82
    '    Code Lines: 79
    ' Comment Lines: 0
    '   Blank Lines: 3
    '     File Size: 5.20 KB


    ' Class EidSpecificSpectrumGenerator
    ' 
    '     Function: EidSpecificSpectrumGen
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports std = System.Math

Public Class EidSpecificSpectrumGenerator
        Private Shared ReadOnly CH2 As Double = {HydrogenMass * 2, CarbonMass}.Sum()
        Public Shared Function EidSpecificSpectrumGen(lipid As ILipid, chain As IChain, adduct As AdductIon, nlMass As Double, intensity As Double) As SpectrumPeak()
            Dim peaks = New List(Of SpectrumPeak)()
            If chain.GetType() Is GetType(AcylChain) Then
                nlMass = nlMass - OxygenMass + HydrogenMass * 2
            End If
            Dim chainLoss = lipid.Mass - chain.Mass - nlMass
            Dim diffs = New Double(chain.CarbonCount - 1) {}
            Dim bondPositions = New List(Of Integer)()
            For i = 0 To chain.CarbonCount - 1 ' numbering from COOH. 18:2(9,12) -> 9 is 8 and 12 is 11 
                diffs(i) = CH2
            Next
            For Each bond In chain.DoubleBond.Bonds ' double bond 18:2(9,12) -> 9 is 9 and 12 is 12 
                diffs(bond.Position - 1) -= HydrogenMass
                diffs(bond.Position) -= HydrogenMass
                bondPositions.Add(bond.Position)
            Next
            For i = 1 To chain.CarbonCount - 1
                diffs(i) += diffs(i - 1)
            Next

            If chain.DoubleBond.Count < 3 Then
                For Each dbPosition In bondPositions
                    If dbPosition = 1 Then Continue For
                    Dim intArray = {0.5, 0.5, 0.5, 0.7, 1, 0.7, 0.5}

                    For i = dbPosition - 1 - 2 To std.Min(diffs.Length, dbPosition - 1 + 5) - 1
                        If i <= 0 Then Continue For
                        Dim n = i - (dbPosition - 1 - 2)
                        Dim diffMass = If(i = dbPosition - 1, diffs(i), If(i >= dbPosition - 1, diffs(i) + HydrogenMass, diffs(i) - HydrogenMass))
                        peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffMass), intensity * intArray(n), $"{chain} db{dbPosition} EID specific(c{i + 1})") With {
                            .SpectrumComment = SpectrumComment.doublebond
                        })
                    Next
                Next
            ElseIf chain.DoubleBond.Count >= 3 AndAlso bondPositions.Contains(bondPositions.Max() - 3) AndAlso bondPositions.Contains(bondPositions.Max() - 6) Then
                Dim dbPosition = bondPositions.Max() - 6
                If bondPositions.Count = 4 Then
                    Dim spectrum = New List(Of SpectrumPeak) From {
                        New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(dbPosition - 1 - 2) + HydrogenMass), intensity * 0.5, $"{chain} EID specific") With {
                            .SpectrumComment = SpectrumComment.doublebond
                        },
                        New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(dbPosition - 1 - 1)), intensity * 0.5, $"{chain} EID specific") With {
                            .SpectrumComment = SpectrumComment.doublebond
                        },
                        New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(dbPosition - 1)), intensity * 0.75, $"{chain} EID specific") With {
                            .SpectrumComment = SpectrumComment.doublebond
                        },
                        New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(dbPosition - 1 + 1)) + HydrogenMass, intensity * 1, $"{chain} EID specific") With {
                            .SpectrumComment = SpectrumComment.doublebond
                        },
                        New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(dbPosition - 1 + 2)) + HydrogenMass, intensity * 0.5, $"{chain} EID specific") With {
                            .SpectrumComment = SpectrumComment.doublebond
                        }
                    }
                    peaks.AddRange(spectrum)
                Else
                    Dim spectrum = New List(Of SpectrumPeak) From {
                        New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(dbPosition - 1)), intensity * 0.25, $"{chain} EID specific") With {
                            .SpectrumComment = SpectrumComment.doublebond
                        },
                        New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(dbPosition - 1 + 1) + HydrogenMass), intensity * 1, $"{chain} EID specific") With {
                            .SpectrumComment = SpectrumComment.doublebond
                        }
                    }
                    peaks.AddRange(spectrum)
                End If
            End If
            If bondPositions.Contains(5) AndAlso bondPositions.Contains(8) AndAlso bondPositions.Contains(11) Then
                peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(2) - HydrogenMass), intensity * 0.5, $"{chain} C3 specific") With {
.SpectrumComment = SpectrumComment.doublebond
})
            End If
            Return peaks.ToArray()
        End Function
    End Class
