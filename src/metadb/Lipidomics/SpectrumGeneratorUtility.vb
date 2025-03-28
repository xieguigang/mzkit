﻿#Region "Microsoft.VisualBasic::441a3c18e70bfde5e9b1ca84d438338e, metadb\Lipidomics\SpectrumGeneratorUtility.vb"

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

    '   Total Lines: 65
    '    Code Lines: 54 (83.08%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 11 (16.92%)
    '     File Size: 3.10 KB


    ' Module SpectrumGeneratorUtility
    ' 
    '     Function: GetAcylDoubleBondSpectrum, GetAlkylDoubleBondSpectrum
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

Public Module SpectrumGeneratorUtility

    Private ReadOnly CH2 As Double = {HydrogenMass * 2, CarbonMass}.Sum()

    <Obsolete>
    Public Function GetAcylDoubleBondSpectrum(lipid As ILipid, acylChain As AcylChain, adduct As AdductIon, Optional NLMass As Double = 0.0, Optional abundance As Double = 50.0) As IEnumerable(Of SpectrumPeak)
        If acylChain.DoubleBond.UnDecidedCount <> 0 OrElse acylChain.CarbonCount = 0 Then
            Return Enumerable.Empty(Of SpectrumPeak)()
        End If
        Dim chainLoss = lipid.Mass - acylChain.Mass - NLMass
        Dim diffs = New Double(acylChain.CarbonCount - 1) {}
        For i = 0 To acylChain.CarbonCount - 1
            diffs(i) = CH2
        Next

        diffs(0) += OxygenMass - HydrogenMass * 2

        For Each bond In acylChain.DoubleBond.Bonds
            diffs(bond.Position - 1) -= HydrogenMass
            diffs(bond.Position) -= HydrogenMass
        Next
        For i = 1 To acylChain.CarbonCount - 1
            diffs(i) += diffs(i - 1)
        Next

        Dim peaks = New List(Of SpectrumPeak)()
        For i = 0 To acylChain.CarbonCount - 1 - 1
            peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(i)), abundance, $"{acylChain} C{i + 1}"))
            peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(i) - HydrogenMass), abundance * 0.5, $"{acylChain} C{i + 1}-H"))
            peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(i) + HydrogenMass), abundance * 0.5, $"{acylChain} C{i + 1}+H"))
        Next

        Return peaks
    End Function

    <Obsolete>
    Public Function GetAlkylDoubleBondSpectrum(lipid As ILipid, alkylChain As AlkylChain, adduct As AdductIon, Optional NLMass As Double = 0.0, Optional abundance As Double = 50.0) As IEnumerable(Of SpectrumPeak)
        Dim chainLoss = lipid.Mass - alkylChain.Mass - NLMass
        Dim diffs = New Double(alkylChain.CarbonCount - 1) {}
        For i = 0 To alkylChain.CarbonCount - 1
            diffs(i) = CH2
        Next

        For Each bond In alkylChain.DoubleBond.Bonds
            diffs(bond.Position - 1) -= HydrogenMass
            diffs(bond.Position) -= HydrogenMass
        Next
        For i = 1 To alkylChain.CarbonCount - 1
            diffs(i) += diffs(i - 1)
        Next

        Dim peaks = New List(Of SpectrumPeak)()
        For i = 0 To alkylChain.CarbonCount - 1 - 1
            peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(i)), abundance, $"{alkylChain} C{i + 1}"))
            peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(i) - HydrogenMass), abundance * 0.5, $"{alkylChain} C{i + 1}-H"))
            peaks.Add(New SpectrumPeak(adduct.ConvertToMz(chainLoss + diffs(i) + HydrogenMass), abundance * 0.5, $"{alkylChain} C{i + 1}+H"))
        Next

        Return peaks
    End Function
End Module
