Imports CompMs.Common.Enum
Imports CompMs.Common.FormulaGenerator.DataObj
Imports System.Linq
Imports System.Text.RegularExpressions

Namespace CompMs.Common.Lipidomics
    Public Class DMEDFALipidParser
        Implements ILipidParser
        Private Shared ReadOnly Skelton As Double = {CarbonMass * 4, NitrogenMass * 2, HydrogenMass * 11}.Sum()
        Public ReadOnly Property Target As String = "DMEDFA" Implements ILipidParser.Target

        Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildParser(1)
        Public Shared ReadOnly Pattern As String = $"^DMEDFA\s*(?<sn>{chainsParser.Pattern})$"
        Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

        Public Function Parse(ByVal lipidStr As String) As ILipid Implements ILipidParser.Parse
            Dim match = patternField.Match(lipidStr)
            If match.Success Then
                Dim group = match.Groups
                Dim chains = chainsParser.Parse(group("sn").Value)
                Return New Lipid(LbmClass.DMEDFA, Skelton + chains.Mass, chains)
            End If
            Return Nothing
        End Function

    End Class
End Namespace
