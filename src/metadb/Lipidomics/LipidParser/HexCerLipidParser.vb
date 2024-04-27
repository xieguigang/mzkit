#Region "Microsoft.VisualBasic::cf589b74d2392d404734c9f35ff9a126, G:/mzkit/src/metadb/Lipidomics//LipidParser/HexCerLipidParser.vb"

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

    '   Total Lines: 70
    '    Code Lines: 64
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 3.26 KB


    ' Class HexCerLipidParser
    ' 
    '     Properties: Target
    ' 
    '     Function: Parse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Public Class HexCerLipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "HexCer" Implements ILipidParser.Target

    Private Shared ReadOnly Skelton As Double = {CarbonMass * 6, HydrogenMass * 10, OxygenMass * 5}.Sum()

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildCeramideParser(2)
    Public Shared ReadOnly Pattern As String = $"^HexCer\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)
    Public Shared ReadOnly CeramideClassPattern As String = "\d+:(?<d>\d+).*?\(?((?<sp>\d+)OH,?)+\)?/\d+:\d+.*?(;?(?<h>\(?((?<ab>\d+)OH,?)+\)?|((?<oxnum>\d+)?O)))?"
    Private Shared ReadOnly ceramideClassPatternField As Regex = New Regex(CeramideClassPattern, RegexOptions.Compiled)

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim match = patternField.Match(lipidStr)
        If match.Success Then
            Dim group = match.Groups
            Dim chains = chainsParser.Parse(group("sn").Value)
            Dim classPattern = ceramideClassPatternField.Match(chains.ToString())
            Dim classString = ""
            If classPattern.Success Then
                Dim classGroup = classPattern.Groups
                If classGroup("h").Success Then
                    If classGroup("ab").Success Then
                        Select Case classGroup("ab").Value
                            Case "2"
                                classString = classString & "A"
                            Case "3"
                                classString = classString & "B"
                            Case Else
                                classString = classString & "H"
                        End Select
                    Else
                        classString = classString & "H"
                    End If
                Else
                    classString = classString & "N"
                End If

                If Equals(classGroup("d").Value, "0") Then
                    classString = classString & "D"
                End If

                Select Case classGroup("sp").Value
                    Case "3"
                        classString = classString & "S"
                    Case "4"
                        classString = classString & "P"
                End Select
            End If
            Dim lipidClass = New LbmClass()
            Select Case classString
                Case "AS", "BS", "HS"
                    lipidClass = LbmClass.HexCer_HS
                Case "HDS", "BDS", "ADS"
                    lipidClass = LbmClass.HexCer_HDS
                Case "NDS"
                    lipidClass = LbmClass.HexCer_NDS
                Case "NS"
                    lipidClass = LbmClass.HexCer_NS
                Case "AP", "ADP"
                    lipidClass = LbmClass.HexCer_AP
            End Select
            Return New Lipid(lipidClass, chains.Mass + Skelton, chains)
        End If
        Return Nothing
    End Function
End Class


