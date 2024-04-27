#Region "Microsoft.VisualBasic::49045f0676ac860bcc882e0869bdc38f, G:/mzkit/src/metadb/Lipidomics//LipidParser/CLLipidParser.vb"

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

    '   Total Lines: 50
    '    Code Lines: 34
    ' Comment Lines: 8
    '   Blank Lines: 8
    '     File Size: 2.85 KB


    ' Class CLLipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass

Public Class CLLipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "CL" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(4)
    Public Shared ReadOnly Pattern As String = $"^CL\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Private Shared ReadOnly chainsParserSub As TotalChainParser = TotalChainParser.BuildParser(2)
    Public Shared ReadOnly PatternSub As String = $"^CL\s*(?<sn1sn2>{chainsParserSub.Pattern})[/_]*?(?<sn3sn4>{chainsParserSub.Pattern})*?$"
    Private Shared ReadOnly patternSubField As Regex = New Regex(PatternSub, RegexOptions.Compiled)
    Public Shared ReadOnly PatternSub2 As String = $"^CL\s*(?<sn>{chainsParserSub.Pattern})$"
    Private Shared ReadOnly patternSub2Field As Regex = New Regex(PatternSub2, RegexOptions.Compiled)


    Private Shared ReadOnly Skelton As Double = {CarbonMass * 9, HydrogenMass * 18, OxygenMass * 13, PhosphorusMass * 2}.Sum()

    Private Shared ReadOnly SkeltonSub As Double = {CarbonMass * 9, HydrogenMass * 14, OxygenMass * 15, PhosphorusMass * 2}.Sum()

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Return New Lipid(LbmClass.CL, Skelton + chains.Mass, chains)
        Else
            Dim matchSub = patternSubField.Match(lipidStr)
            If matchSub.Success Then
                Dim groupSub = matchSub.Groups
                If Not Equals(groupSub("sn3sn4").Value, "") Then
                    Dim chains = chainsParser.Parse(groupSub("sn1sn2").Value & "_" & groupSub("sn3sn4").Value)
                    Return New Lipid(LbmClass.CL, Skelton + chains.Mass, chains)
                End If
                'else
                '{
                '    var carbon = int.Parse(groupSub["carbon"].Captures[0].Value) + int.Parse(groupSub["carbon"].Captures[1].Value);
                '    var db = int.Parse(groupSub["db"].Captures[0].Value) + int.Parse(groupSub["db"].Captures[1].Value);
                '    var ox = !groupSub["ox"].Success ? 0 : !groupSub["oxnum"].Success ? 1 : int.Parse(groupSub["oxnum"].Value);
                '    var chains = chainsParser.Parse("CL " + carbon + ":" + db + ";" + ox);
                '    return new Lipid(LbmClass.CL, Skelton + chains.Mass, chains);

                '}
            End If
        End If
        Return Nothing
    End Function
End Class

