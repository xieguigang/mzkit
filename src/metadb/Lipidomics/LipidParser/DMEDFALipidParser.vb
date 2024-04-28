#Region "Microsoft.VisualBasic::6b40bdc47d640954032e36ba8c7412b7, G:/mzkit/src/metadb/Lipidomics//LipidParser/DMEDFALipidParser.vb"

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

    '   Total Lines: 23
    '    Code Lines: 19
    ' Comment Lines: 0
    '   Blank Lines: 4
    '     File Size: 1.14 KB


    ' Class DMEDFALipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass

Public Class DMEDFALipidParser
        Implements ILipidParser
        Private Shared ReadOnly Skelton As Double = {CarbonMass * 4, NitrogenMass * 2, HydrogenMass * 11}.Sum()
        Public ReadOnly Property Target As String = "DMEDFA" Implements ILipidParser.Target

        Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(1)
        Public Shared ReadOnly Pattern As String = $"^DMEDFA\s*(?<sn>{chainsParser.Pattern})$"
        Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

        Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
            Dim match = patternField.Match(lipidStr)
            If match.Success Then
                Dim group = match.Groups
                Dim chains = chainsParser.Parse(group("sn").Value)
                Return New Lipid(LbmClass.DMEDFA, Skelton + chains.Mass, chains)
            End If
            Return Nothing
        End Function

    End Class
