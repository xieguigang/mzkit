Imports CompMs.Common.Enum
Imports System.Text.RegularExpressions

Namespace CompMs.Common.Lipidomics
    Public Class CeramideNsD7LipidParser
        Implements ILipidParser
        Public ReadOnly Property Target As String = "Cer_d7" Implements ILipidParser.Target

        Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildCeramideParser(2)
        Public Shared ReadOnly Pattern As String = $"^Cer_d7\s*(?<sn>{chainsParser.Pattern})$"
        Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

        Public Shared ReadOnly CeramideClassPattern As String = "\d+:(?<d>\d+).*?\(?((?<sp>\d+)OH,?)+\)?/\d+:\d+.*?(;?(?<h>\(?((?<ab>\d+)OH,?)+\)?|(O(?<oxnum>\d+)?)))?"

        Private Shared ReadOnly ceramideClassPatternField As Regex = New Regex(CeramideClassPattern, RegexOptions.Compiled)

        Public Function Parse(ByVal lipidStr As String) As ILipid Implements ILipidParser.Parse
            Dim match = patternField.Match(lipidStr)
            If match.Success Then
                Dim group = match.Groups
                Dim chains = chainsParser.Parse(group("sn").Value)
                Return New Lipid(LbmClass.Cer_NS_d7, chains.Mass, chains)
            End If
            Return Nothing
        End Function
    End Class
End Namespace
