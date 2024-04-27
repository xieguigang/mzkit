#Region "Microsoft.VisualBasic::2e2bc7a0f1672060bb414aa82e15115f, G:/mzkit/src/metadb/Lipidomics//SpectrumPeakGenerator.vb"

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

    '   Total Lines: 249
    '    Code Lines: 167
    ' Comment Lines: 56
    '   Blank Lines: 26
    '     File Size: 12.82 KB


    ' Class SpectrumPeakGenerator
    ' 
    '     Function: GetAcylDoubleBondSpectrum, GetAlkylDoubleBondSpectrum, GetDoubleBondSpectrum, GetSphingoDoubleBondSpectrum
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

Public Class SpectrumPeakGenerator
    Implements ISpectrumPeakGenerator
    Private Shared ReadOnly CH2 As Double = {HydrogenMass * 2, CarbonMass}.Sum()

    Private Function GetDoubleBondSpectrum(lipid As ILipid, chain As IChain, adduct As AdductIon, nlMass As Double, abundance As Double) As IEnumerable(Of SpectrumPeak)
        If chain.DoubleBond.UnDecidedCount <> 0 OrElse chain.CarbonCount = 0 OrElse chain.Oxidized.UnDecidedCount <> 0 Then
            Return Enumerable.Empty(Of SpectrumPeak)()
        End If
        Dim chainLoss = lipid.Mass - chain.Mass - nlMass
        Dim diffs = New Double(chain.CarbonCount - 1) {}
        For i = 0 To chain.CarbonCount - 1 ' numbering from COOH. 18:2(9,12) -> 9 is 8 and 12 is 11 
            diffs(i) = CH2
        Next

        If chain.Oxidized IsNot Nothing Then
            For Each ox In chain.Oxidized.Oxidises
                diffs(ox - 1) = diffs(ox - 1) + OxygenMass
            Next
        End If

        Dim bondPositions = New List(Of Integer)()
        For Each bond In chain.DoubleBond.Bonds ' double bond 18:2(9,12) -> 9 is 9 and 12 is 12 
            'if (bond.Position > diffs.Length - 1) return Enumerable.Empty<SpectrumPeak>();
            diffs(bond.Position - 1) -= HydrogenMass
            diffs(bond.Position) -= HydrogenMass
            bondPositions.Add(bond.Position)

            'Console.WriteLine(bond.Position);
        Next
        For i = 1 To chain.CarbonCount - 1
            diffs(i) += diffs(i - 1)
        Next

        Dim peaks = New List(Of SpectrumPeak)()
        For i = 0 To chain.CarbonCount - 1 - 1
            Dim factor = 1.0
            Dim factorHLoss = 0.5
            Dim factorHGain = 0.05
            Dim speccomment_hloss = SpectrumComment.doublebond
            Dim speccomment_radical = SpectrumComment.doublebond
            Dim speccomment_hgain = SpectrumComment.doublebond

            ' in the case of 18:2(9,12)
            ' i=10 means i-1=9=C9 and C11 in chain obj and C7 from omega terminal
            ' i=13 means i-1=12=C12 in bondPositions and C14 in chain obj and C4 from omega terminal
            ' in the case of 20:4(5,8,11,14)
            ' i=6 means i-1=5=C5 in bondPositions and C7 in chain obj and C13 from omega terminal
            ' i=9 means i-1=8=C8 in bondPositions and C10 in chain obj and C10 from omega terminal
            ' i=12 means i-1=11=C11 in bondPositions and C13 in chain obj and C7 from omega terminal
            ' i=15 means i-1=14=C14 in bondPositions and C16 in chain obj and C4 from omega terminal
            If bondPositions.Contains(i - 1) Then ' in the case of 18:2(9,12), Radical is big, and H loss is next
                If nlMass < 0.001 Then
                    factor = 4.0
                    factorHLoss = 2.0
                    factorHGain = 0.05
                    speccomment_radical = speccomment_radical Or SpectrumComment.doublebond_high
                End If

                ' in the case of 18:2(9,12)
                ' i=8 means i+1=9=C9 and C9 in chain obj and C9 from omega terminal
                ' i=11 means i+1=12=C12 in bondPositions and C12 in chain obj and C6 from omega terminal
                ' in the case of 20:4(5,8,11,14)
                ' i=4 means i+1=5=C5 in bondPositions and C5 in chain obj and C15 from omega terminal
                ' i=7 means i+1=8=C8 in bondPositions and C8 in chain obj and C12 from omega terminal
                ' i=10 means i+1=11=C11 in bondPositions and C11 in chain obj and C9 from omega terminal
                ' i=13 means i+1=14=C14 in bondPositions and C14 in chain obj and C6 from omega terminal
            ElseIf bondPositions.Contains(i + 1) Then
                factor = 0.5
                factorHLoss = 0.25
                factorHGain = 0.05
                speccomment_radical = speccomment_radical Or SpectrumComment.doublebond_low
                ' now no modification
            ElseIf bondPositions.Contains(i + 2) Then
                ' in the case of 18:2(9,12)
                ' i=6 means i+3=9=C9 and C7 in chain obj and C11 from omega terminal
                ' i=9 means i+3=12=C12 in bondPositions and C10 in chain obj and C8 from omega terminal
                ' in the case of 20:4(5,8,11,14)
                ' i=2 means i+3=5=C5 in bondPositions and C3 in chain obj and C17 from omega terminal
                ' i=5 means i+3=8=C8 in bondPositions and C6 in chain obj and C14 from omega terminal
                ' i=8 means i+3=11=C11 in bondPositions and C9 in chain obj and C11 from omega terminal
                ' i=11 means i+3=14=C14 in bondPositions and C12 in chain obj and C8 from omega terminal
            ElseIf bondPositions.Contains(i + 3) Then
                factorHLoss = 4.0
                factor = 2.0
                factorHGain = 0.05
                speccomment_hloss = speccomment_hloss Or SpectrumComment.doublebond_high

                'if (bondPositions.Contains(i))
                '{
                '    factor = 3.0;
                '    factorHLoss = 0.5;
                '    factorHGain = 2.0;
                '}
                'else
                '{
                '    factorHLoss = 4.0;
                '    speccomment_hloss |= SpectrumComment.doublebond_high;

                '}
            End If

            If bondPositions.Contains(i) AndAlso bondPositions.Contains(i + 3) AndAlso bondPositions.Contains(i + 6) Then
                factorHGain = 4.0
                speccomment_hgain = speccomment_hgain Or SpectrumComment.doublebond_high
                ' now no modification
            End If

            If i = 2 Then
                If bondPositions.Contains(1) Then
                    factor = 2.5
                    factorHLoss = 0.5
                    factorHGain = 0.0
                Else
                    factor = 0.75
                    factorHLoss = 2.0
                    factorHGain = 0.5
                End If
            End If
            If i = 1 Then
                factor = 1.5
                factorHLoss = 0.5
                factorHGain = 2.0
            End If

            If factorHGain = 4.0 Then
                speccomment_hgain = speccomment_hgain Or SpectrumComment.doublebond_high
            End If

            peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(i) - HydrogenMass), factorHLoss * abundance, $"{chain} C{i + 1}-H") With {
                    .SpectrumComment = speccomment_hloss
                })
            peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(i)), factor * abundance, $"{chain} C{i + 1}") With {
                    .SpectrumComment = speccomment_radical
                })
            peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(i) + HydrogenMass), factorHGain * abundance, $"{chain} C{i + 1}+H") With {
                    .SpectrumComment = speccomment_hgain
                })
        Next

        Return peaks
    End Function

    Public Function GetAcylDoubleBondSpectrum(lipid As ILipid, acylChain As AcylChain, adduct As AdductIon, nlMass As Double, abundance As Double) As IEnumerable(Of SpectrumPeak) Implements ISpectrumPeakGenerator.GetAcylDoubleBondSpectrum
        Return GetDoubleBondSpectrum(lipid, acylChain, adduct, nlMass - OxygenMass + HydrogenMass * 2, abundance)
    End Function

    Public Function GetAlkylDoubleBondSpectrum(lipid As ILipid, acylChain As AlkylChain, adduct As AdductIon, nlMass As Double, abundance As Double) As IEnumerable(Of SpectrumPeak) Implements ISpectrumPeakGenerator.GetAlkylDoubleBondSpectrum
        Return GetDoubleBondSpectrum(lipid, acylChain, adduct, nlMass, abundance)
    End Function

    Public Function GetSphingoDoubleBondSpectrum(lipid As ILipid, sphingo As SphingoChain, adduct As AdductIon, nlMass As Double, abundance As Double) As IEnumerable(Of SpectrumPeak) Implements ISpectrumPeakGenerator.GetSphingoDoubleBondSpectrum
        If sphingo.DoubleBond.UnDecidedCount <> 0 OrElse sphingo.CarbonCount = 0 OrElse sphingo.Oxidized.UnDecidedCount <> 0 Then
            Return Enumerable.Empty(Of SpectrumPeak)()
        End If

        Dim chainLoss = lipid.Mass - sphingo.Mass - nlMass + NitrogenMass + 12 * 2 + OxygenMass * 1 + HydrogenMass * 5
        Dim diffs = New Double(sphingo.CarbonCount - 1) {}
        For i = 0 To sphingo.CarbonCount - 1
            diffs(i) = CH2
        Next

        If sphingo.Oxidized IsNot Nothing Then
            For Each ox In sphingo.Oxidized.Oxidises
                If ox > 1 Then
                    diffs(ox - 1) = diffs(ox - 1) + OxygenMass
                End If
            Next
        End If

        Dim bondPositions = New List(Of Integer)()
        For Each bond In sphingo.DoubleBond.Bonds
            diffs(bond.Position - 1) -= HydrogenMass
            diffs(bond.Position) -= HydrogenMass
            bondPositions.Add(bond.Position)
        Next
        For i = 3 To sphingo.CarbonCount - 1
            diffs(i) += diffs(i - 1)
        Next

        Dim peaks = New List(Of SpectrumPeak)()
        For i = 2 To sphingo.CarbonCount - 1 - 1
            Dim speccomment = SpectrumComment.doublebond
            Dim factor = 1.0
            Dim factorHLoss = 0.5
            Dim factorHGain = 0.2
            Dim speccomment_hloss = SpectrumComment.doublebond
            Dim speccomment_radical = SpectrumComment.doublebond
            Dim speccomment_hgain = SpectrumComment.doublebond

            If bondPositions.Contains(i - 1) Then ' in the case of 18:2(9,12), Radical is big, and H loss is next
                'if (nlMass < 0.001)
                '{
                factor = 4.0
                factorHLoss = 2.0
                factorHGain = 0.05
                '}
                speccomment_radical = speccomment_radical Or SpectrumComment.doublebond_high
                'if (bondPositions.Contains(i - 1))
                '{ // in the case of 18:2(9,12), Radical is big, and H loss is next
                '    factor = 4.0;
                '    factorHLoss = 2.0;
                '    speccomment |= SpectrumComment.doublebond_high;
                '}
                ' now no modification
            ElseIf bondPositions.Contains(i) Then
            ElseIf bondPositions.Contains(i + 1) Then
                factor = 0.25
                factorHLoss = 0.5
                factorHGain = 0.05
                speccomment = speccomment Or SpectrumComment.doublebond_low
                ' now no modification
            ElseIf bondPositions.Contains(i + 2) Then
            ElseIf bondPositions.Contains(i + 3) Then
                If bondPositions.Contains(i) Then
                    factor = 4.0
                    factorHLoss = 0.5
                    factorHGain = 2.0
                Else
                    factorHLoss = 4.0
                    speccomment = speccomment Or SpectrumComment.doublebond_high
                End If
                speccomment = speccomment Or SpectrumComment.doublebond_high
            End If

            peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(i) - HydrogenMass), factorHLoss * abundance, $"{sphingo} C{i + 1}-H") With {
                    .SpectrumComment = speccomment
                })
            peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(i)), factor * abundance, $"{sphingo} C{i + 1}") With {
                    .SpectrumComment = speccomment
                })
            peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(i) + HydrogenMass), factorHGain * abundance, $"{sphingo} C{i + 1}+H") With {
                    .SpectrumComment = speccomment
                })
        Next

        'for (int i = 2; i < sphingo.CarbonCount - 1; i++)
        '{
        '    peaks.Add(new SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs[i] - MassDiffDictionary.HydrogenMass), abundance * 0.5, $"{sphingo} C{i + 1}-H") { SpectrumComment = SpectrumComment.doublebond });
        '    peaks.Add(new SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs[i]), abundance, $"{sphingo} C{i + 1}") { SpectrumComment = SpectrumComment.doublebond });
        '    peaks.Add(new SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs[i] + MassDiffDictionary.HydrogenMass), abundance * 0.5, $"{sphingo} C{i + 1}+H") { SpectrumComment = SpectrumComment.doublebond });
        '}

        Return peaks
    End Function
End Class

