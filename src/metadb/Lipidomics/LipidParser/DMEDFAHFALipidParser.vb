Imports System.Text.RegularExpressions


Public Class DMEDFAHFALipidParser
        Implements ILipidParser
        Private Shared ReadOnly C4N2H10 As Double = {CarbonMass * 4, NitrogenMass * 2, HydrogenMass * 10}.Sum()
        Public ReadOnly Property Target As String = "DMEDFAHFA" Implements ILipidParser.Target

        Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(2)
        Public Shared ReadOnly Pattern As String = $"^DMEDFAHFA\s*(?<sn>{chainsParser.Pattern})$"
        Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

        Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
            Dim match = patternField.Match(lipidStr)
            If match.Success Then
                Dim group = match.Groups
                Dim chains = chainsParser.Parse(group("sn").Value)
                Return New Lipid(LbmClass.DMEDFAHFA, chains.Mass + C4N2H10, chains)
            End If
            Return Nothing
        End Function
    End Class

