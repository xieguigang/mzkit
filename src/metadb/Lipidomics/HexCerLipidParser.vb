Imports CompMs.Common.Enum
Imports CompMs.Common.FormulaGenerator.DataObj
Imports System.Linq
Imports System.Text.RegularExpressions

Namespace CompMs.Common.Lipidomics
    Public Class HexCerLipidParser
        Implements ILipidParser
        Public ReadOnly Property Target As String = "HexCer" Implements ILipidParser.Target

        Private Shared ReadOnly Skelton As Double = {CarbonMass * 6, HydrogenMass * 10, OxygenMass * 5}.Sum()

        Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildCeramideParser(2)
        Public Shared ReadOnly Pattern As String = $"^HexCer\s*(?<sn>{chainsParser.Pattern})$"
        Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)
        Public Shared ReadOnly CeramideClassPattern As String = "\d+:(?<d>\d+).*?\(?((?<sp>\d+)OH,?)+\)?/\d+:\d+.*?(;?(?<h>\(?((?<ab>\d+)OH,?)+\)?|((?<oxnum>\d+)?O)))?"
        Private Shared ReadOnly ceramideClassPatternField As Regex = New Regex(CeramideClassPattern, RegexOptions.Compiled)

        Public Function Parse(ByVal lipidStr As String) As ILipid Implements ILipidParser.Parse
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
End Namespace
