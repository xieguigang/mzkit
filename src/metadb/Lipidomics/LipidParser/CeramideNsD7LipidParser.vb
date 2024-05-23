#Region "Microsoft.VisualBasic::e2b02fb9312eb8420edbb8eb0fe09f75, metadb\Lipidomics\LipidParser\CeramideNsD7LipidParser.vb"

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

    '   Total Lines: 24
    '    Code Lines: 19 (79.17%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (20.83%)
    '     File Size: 1.19 KB


    ' Class CeramideNsD7LipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions

Public Class CeramideNsD7LipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "Cer_d7" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildCeramideParser(2)
    Public Shared ReadOnly Pattern As String = $"^Cer_d7\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Public Shared ReadOnly CeramideClassPattern As String = "\d+:(?<d>\d+).*?\(?((?<sp>\d+)OH,?)+\)?/\d+:\d+.*?(;?(?<h>\(?((?<ab>\d+)OH,?)+\)?|(O(?<oxnum>\d+)?)))?"

    Private Shared ReadOnly ceramideClassPatternField As Regex = New Regex(CeramideClassPattern, RegexOptions.Compiled)

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.Cer_NS_d7, chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class
