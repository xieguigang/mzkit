Imports CompMs.Common.Enum
Imports CompMs.Common.FormulaGenerator.DataObj
Imports System.Linq
Imports System.Text.RegularExpressions

Namespace CompMs.Common.Lipidomics
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

        Public Function Parse(ByVal lipidStr As String) As ILipid Implements ILipidParser.Parse
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
End Namespace
