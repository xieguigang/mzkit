Imports CompMs.Common.Enum
Imports System.Text.RegularExpressions

Public Class CeramideLipidParser
    Implements ILipidParser
    Public ReadOnly Property Target As String = "Cer" Implements ILipidParser.Target

    Private Shared ReadOnly chainsParser As TotalChainParser = TotalChainParser.BuildCeramideParser(2)
    Public Shared ReadOnly Pattern As String = $"^Cer\s*(?<sn>{chainsParser.Pattern})$"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    'public static readonly string CeramideClassPattern = @"\d+:(?<d>\d+).*?\(?((?<sp>\d+)OH,?)+\)?/\d+:\d+.*?(;?(?<h>\(?((?<ab>\d+)OH,?)+\)?|(O(?<oxnum>\d+)?)))?";
    Public Shared ReadOnly CeramideClassPattern As String = "\d+:(?<d>\d+).*?\)?;?\(?((?<oxSph>O(?<oxnumSph>\d+)?)|((?<sp>\d+)OH,?)+\)?)/\d+:\d+.*?(;?(?<h>\(?((?<ab>\d+)OH,?)+\)?|(O(?<oxnum>\d+)?)))?"

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

                If classGroup("sp").Success Then
                    Select Case classGroup("sp").Value
                        Case "3"
                            classString = classString & "S"
                        Case "4"
                            classString = classString & "P"
                    End Select
                Else
                    Select Case classGroup("oxnumSph").Value
                        Case "2"
                            classString = classString & "S"
                        Case "3"
                            classString = classString & "P"
                    End Select
                End If
            End If
            Dim lipidClass = New LbmClass()
            Select Case classString
                Case "ADS"
                    lipidClass = LbmClass.Cer_ADS
                Case "AS"
                    lipidClass = LbmClass.Cer_AS
                Case "BDS"
                    lipidClass = LbmClass.Cer_BDS
                Case "BS"
                    lipidClass = LbmClass.Cer_BS
                Case "NDS"
                    lipidClass = LbmClass.Cer_NDS
                Case "NS"
                    lipidClass = LbmClass.Cer_NS
                Case "AP", "ADP"
                    lipidClass = LbmClass.Cer_AP
                Case "NP", "NDP"
                    lipidClass = LbmClass.Cer_NP
                Case "HDS"
                    lipidClass = LbmClass.Cer_HDS
                Case "HS"
                    lipidClass = LbmClass.Cer_HS

            End Select
            Return New Lipid(lipidClass, chains.Mass, chains)
        End If
        Return Nothing
    End Function
End Class
