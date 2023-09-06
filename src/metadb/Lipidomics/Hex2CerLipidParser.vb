Imports CompMs.Common.Enum
Imports CompMs.Common.FormulaGenerator.DataObj
Imports System.Linq
Imports System.Text.RegularExpressions


Public Class Hex2CerLipidParser
        Implements ILipidParser
        Public ReadOnly Property Target As String = "Hex2Cer" Implements ILipidParser.Target

        Private Shared ReadOnly Skelton As Double = {CarbonMass * 12, HydrogenMass * 20, OxygenMass * 10}.Sum()

        Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildCeramideParser(2)
        Public Shared ReadOnly Pattern As String = $"^Hex2Cer\s*(?<sn>{chainsParser.Pattern})$"
        Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)
        'public static readonly string CeramideClassPattern = @"\d+:(?<d>\d+).*?\(?((?<sp>\d+)OH,?)+\)?/\d+:\d+.*?(;?(?<h>\(?((?<ab>\d+)OH,?)+\)?|((?<oxnum>\d+)?O)))?";
        'private static readonly Regex ceramideClassPattern = new Regex(CeramideClassPattern, RegexOptions.Compiled);

        Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
            Dim match = patternField.Match(lipidStr)
            If match.Success Then
                Dim group = match.Groups
                Dim chains = chainsParser.Parse(group("sn").Value)
                Return New Lipid(LbmClass.Hex2Cer, chains.Mass + Skelton, chains)
            End If
            Return Nothing
        End Function
    End Class
