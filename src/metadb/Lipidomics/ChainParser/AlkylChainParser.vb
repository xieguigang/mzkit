Imports System.Text.RegularExpressions

Public Class AlkylChainParser
    Private Shared ReadOnly CarbonPattern As String = "(?<carbon>\d+)"
    Private Shared ReadOnly DoubleBondPattern As String = "(?<db>\d+)(\(((?<dbpos>\d+[EZ]?),?)+\))?"
    Private Shared ReadOnly OxidizedPattern As String = "[;\(]((?<ox>O(?<oxnum>\d+)?)|((?<oxpos>\d+)OH,?)+\)?)"
    'private static readonly string OxidizedPattern = @";(\(?((?<oxpos>\d+)OH,?)+\)?|(?<ox>(?<oxnum>\d+)?O))";

    Public Shared ReadOnly Pattern As String = $"(?<plasm>[OP])-{CarbonPattern}:{DoubleBondPattern}({OxidizedPattern})?"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Public Function Parse(chainStr As String) As IChain
        Dim match = patternField.Match(chainStr)
        If match.Success Then
            Dim groups = match.Groups
            Dim numCarbon = Integer.Parse(groups("carbon").Value)
            Dim ox = ParseOxidized(groups)
            If groups("plasm").Success Then
                Select Case groups("plasm").Value
                    Case "P"
                        Return New AlkylChain(numCarbon, ParseDoubleBond(groups, True), ox)
                    Case "O"
                        Return New AlkylChain(numCarbon, ParseDoubleBond(groups, False), ox)
                End Select
            End If
        End If
        Return Nothing
    End Function

    Private Function ParseDoubleBond(groups As GroupCollection, isPlasma As Boolean) As DoubleBond
        Dim dbnum = Integer.Parse(groups("db").Value) + If(isPlasma, 1, 0)
        If groups("dbpos").Success Then
            If isPlasma Then
                Return New DoubleBond(dbnum, groups("dbpos").Captures.Cast(Of Capture)().[Select](Function(c) ParseDoubleBondInfo(c.Value)).Prepend(DoubleBondInfo.Create(1)).ToArray())
            Else
                Return New DoubleBond(dbnum, groups("dbpos").Captures.Cast(Of Capture)().[Select](Function(c) ParseDoubleBondInfo(c.Value)).ToArray())
            End If
        Else
            If isPlasma Then
                Return New DoubleBond(dbnum, DoubleBondInfo.Create(1))
            Else
                Return New DoubleBond(dbnum)
            End If
        End If
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
            If groups("oxnum").Success Then
                Return New Oxidized(Integer.Parse(groups("oxnum").Value))
            End If
            Return New Oxidized(1)
        End If
        Return New Oxidized(0)
    End Function
End Class
