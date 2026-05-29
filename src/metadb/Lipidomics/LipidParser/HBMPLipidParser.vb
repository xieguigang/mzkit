#Region "Microsoft.VisualBasic::003f650b0949050f347e5f892ef40134, metadb\Lipidomics\LipidParser\HBMPLipidParser.vb"

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
    '    Code Lines: 26 (72.22%)
    ' Comment Lines: 5 (13.89%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (13.89%)
    '     File Size: 1.87 KB


    ' Class HBMPLipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Public Class HBMPLipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "HBMP" Implements ILipidParser.Target

    'HBMP explain rule -> HBMP (1 chain(sn1))/(2 chain(sn2,sn3))
    'HBMP sn1_sn2_sn3 (follow the rules of alignment) -- MolecularSpeciesLevelChains
    'HBMP sn1/(sn2+sn3) (follow the rules of alignment) -- MolecularSpeciesLevelChains <- cannot parsing now. maybe don't need(?)
    'HBMP sn1/sn2_sn3 -- MolecularSpeciesLevelChains <- now same as sn1_sn2_sn3
    'HBMP sn1/sn4(or sn4/sn1)/sn2/sn3 (sn4= 0:0)  -- PositionLevelChains <- !?

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(3)
    Public Shared ReadOnly Pattern As String = $"^HBMP\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 6, HydrogenMass * 12, OxygenMass * 8, PhosphorusMass}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.HBMP, Skelton + chains.Mass, chains)
        Else
            Dim matchSub2 = patternField.Match(lipidStr.Replace("_", "/"))
            If matchSub2.Success Then
                Dim group = matchSub2.Groups
                Dim chains = chainsParser.Parse(group("sn").Value)
                Return New Lipid(LbmClass.HBMP, Skelton + chains.Mass, chains)
            End If
        End If

        Return Nothing
    End Function
End Class
