#Region "Microsoft.VisualBasic::7096fe1c2ba63244c56529ded926fca2, metadb\Lipidomics\MsCharacterization\TGEadMsCharacterization.vb"

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

    '   Total Lines: 36
    '    Code Lines: 32 (88.89%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 4 (11.11%)
    '     File Size: 1.79 KB


    ' Module TGEadMsCharacterization
    ' 
    '     Function: ChainsEqual, Characterize
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MSEngine

Public Module TGEadMsCharacterization
    Public Function Characterize(scan As IMSScanProperty, molecule As ILipid, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As (ILipid, Double())
        Dim class_cutoff = 0
        Dim chain_cutoff = 2
        Dim position_cutoff = 1
        Dim double_cutoff = 0.5

        Dim chains = molecule.Chains.GetDeterminedChains()
        If chains.Length = 3 Then
            If ChainsEqual(chains(0), chains(1)) AndAlso ChainsEqual(chains(1), chains(2)) Then
                chain_cutoff = 1
            ElseIf ChainsEqual(chains(0), chains(1)) AndAlso Not ChainsEqual(chains(1), chains(2)) Then
                chain_cutoff = 2
            ElseIf Not ChainsEqual(chains(0), chains(1)) AndAlso ChainsEqual(chains(1), chains(2)) Then
                chain_cutoff = 2
            ElseIf ChainsEqual(chains(0), chains(2)) AndAlso Not ChainsEqual(chains(1), chains(2)) Then
                chain_cutoff = 2
            Else
                chain_cutoff = 3
            End If
        End If
        If Equals(reference.AdductType.AdductIonName, "[M+NH4]+") Then
            position_cutoff = 0
        End If

        Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, class_cutoff, chain_cutoff, position_cutoff, double_cutoff)
        Return GetDefaultCharacterizationResultForTriacylGlycerols(molecule, defaultResult)
    End Function

    Private Function ChainsEqual(a As IChain, b As IChain) As Boolean
        Return a.CarbonCount = b.CarbonCount AndAlso a.DoubleBond Is b.DoubleBond
    End Function
End Module
