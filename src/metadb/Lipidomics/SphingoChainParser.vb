Imports System.Linq
Imports System.Text.RegularExpressions

Public Class SphingoChainParser
    Private Shared ReadOnly CarbonPattern As String = "(?<carbon>\d+)"
    Private Shared ReadOnly DoubleBondPattern As String = "(?<db>\d+)(\(((?<dbpos>\d+[EZ]?),?)+\))?"
    Private Shared ReadOnly OxidizedPattern As String = "[;\(]((?<ox>O(?<oxnum>\d+)?)|((?<oxpos>\d+)OH,?)+\)?)"

    'private static readonly string OxidizedPattern = @";(((?<oxpos>\d+)OH,?)+|(?<ox>(?<oxnum>\d+)?O))";

    Public Shared ReadOnly Pattern As String = $"{CarbonPattern}:{DoubleBondPattern}{OxidizedPattern}"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Public Function Parse(chainStr As String) As IChain
        Dim match = patternField.Match(chainStr)
        If match.Success Then
            Dim groups = match.Groups
            Dim numCarbon = Integer.Parse(groups("carbon").Value)
            Dim db = ParseDoubleBond(groups)
            Dim ox = ParseOxidized(groups)
            Return New SphingoChain(numCarbon, db, ox)
        End If
        Return Nothing
    End Function

    Private Function ParseDoubleBond(groups As GroupCollection) As DoubleBond
        Dim dbnum = Integer.Parse(groups("db").Value)
        If groups("dbpos").Success Then
            Return New DoubleBond(dbnum, groups("dbpos").Captures.Cast(Of Capture)().[Select](Function(c) ParseDoubleBondInfo(c.Value)).ToArray())
        ElseIf dbnum >= 1 Then ' sphingo doublebond >=1 one of them position "4"
            Return New DoubleBond(dbnum, DoubleBondInfo.Create(4))
        End If
        Return New DoubleBond(dbnum)
    End Function

    Private Function ParseDoubleBondInfo(bond As String) As DoubleBondInfo
        Select Case bond(bond.Length - 1)
            Case "E"c
                Return DoubleBondInfo.E(Integer.Parse(bond.TrimEnd("E"c)))
            Case "Z"c
                Return DoubleBondInfo.Z(Integer.Parse(bond.TrimEnd("Z"c)))
            Case Else
                Return DoubleBondInfo.Create(Integer.Parse(bond))
        End Select
    End Function

    Private Function ParseOxidized(groups As GroupCollection) As Oxidized
        If groups("oxpos").Success Then
            Dim oxpos = groups("oxpos").Captures.Cast(Of Capture)().[Select](Function(c) Integer.Parse(c.Value)).ToArray()
            Return New Oxidized(oxpos.Length, oxpos)
        End If
        If groups("ox").Success Then
            Dim ox = If(Not groups("oxnum").Success, 1, Integer.Parse(groups("oxnum").Value))
            Select Case ox 'TBC
                Case 1
                    Return New Oxidized(ox, 1)
                Case 2
                    Return New Oxidized(ox, 1, 3)
                Case 3
                    Return New Oxidized(ox, 1, 3, 4)
            End Select
            Return New Oxidized(Integer.Parse(groups("oxnum").Value), 1, 3)
        End If
        Return New Oxidized(0)
    End Function
End Class
