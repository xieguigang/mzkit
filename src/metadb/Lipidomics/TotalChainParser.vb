Imports System.Linq
Imports System.Text.RegularExpressions

Public Class TotalChainParser
    Private Shared ReadOnly CarbonPattern As String = "(?<carbon>\d+)"
    Private Shared ReadOnly DoubleBondPattern As String = "(?<db>\d+)"
    Private Shared ReadOnly OxidizedPattern As String = ";(?<ox>O(?<oxnum>\d+)?)"
    'private static readonly string OxidizedPattern = @";(?<ox>(?<oxnum>\d+)?O)"; //


    Private Shared ReadOnly ChainsPattern As String = $"(?<Chain>{AlkylChainParser.Pattern}|{AcylChainParser.Pattern})"
    Private Shared ReadOnly AcylChainsPattern As String = $"(?<Chain>{AcylChainParser.Pattern})"

    Private Shared ReadOnly AlkylParser As AlkylChainParser = New AlkylChainParser()
    Private Shared ReadOnly AcylParser As AcylChainParser = New AcylChainParser()
    Private Shared ReadOnly SphingoParser As SphingoChainParser = New SphingoChainParser()

    Private ReadOnly HasSphingosine As Boolean

    Public Shared Function BuildParser(chainCount As Integer) As TotalChainParser
        Return New TotalChainParser(chainCount, chainCount, False, False, False)
    End Function

    Public Shared Function BuildSpeciesLevelParser(chainCount As Integer, capacity As Integer) As TotalChainParser
        Return New TotalChainParser(chainCount, capacity, False, False, True)
    End Function

    Public Shared Function BuildEtherParser(chainCount As Integer) As TotalChainParser
        Return New TotalChainParser(chainCount, chainCount, False, True, False)
    End Function
    Public Shared Function BuildLysoEtherParser(chainCount As Integer, capacity As Integer) As TotalChainParser
        Return New TotalChainParser(chainCount, capacity, False, True, True)
    End Function

    Public Shared Function BuildCeramideParser(chainCount As Integer) As TotalChainParser
        Return New TotalChainParser(chainCount, chainCount, True, False, False)
    End Function

    Private Sub New(chainCount As Integer, capacity As Integer, hasSphingosine As Boolean, hasEther As Boolean, atLeastSpeciesLevel As Boolean)
        Me.ChainCount = chainCount
        Me.Capacity = capacity
        Dim submolecularLevelPattern = If(hasEther, $"(?<TotalChain>(?<plasm>[de]?[OP]-)?{CarbonPattern}:{DoubleBondPattern}({OxidizedPattern})?)", $"(?<TotalChain>{CarbonPattern}:{DoubleBondPattern}({OxidizedPattern})?)")
        Dim molecularSpeciesLevelPattern = If(hasSphingosine, $"(?<MolecularSpeciesLevel>(?<Chain>{SphingoChainParser.Pattern})_({ChainsPattern}_?){{{Me.ChainCount - 1}}})", If(hasEther, $"(?<MolecularSpeciesLevel>({ChainsPattern}_?){{{Me.ChainCount}}})", $"(?<MolecularSpeciesLevel>({AcylChainsPattern}_?){{{Me.ChainCount}}})"))
        Dim positionLevelPattern = If(hasSphingosine, $"(?<PositionLevel>(?<Chain>{SphingoChainParser.Pattern})/({ChainsPattern}/?){{{Me.Capacity - 1}}})", If(hasEther, $"(?<PositionLevel>({ChainsPattern}/?){{{Me.Capacity}}})", $"(?<PositionLevel>({AcylChainsPattern}/?){{{Me.Capacity}}})"))
        If Me.Capacity = 1 Then
            Dim postionLevelExpression = New Regex(positionLevelPattern, RegexOptions.Compiled)
            Pattern = positionLevelPattern
            Expression = postionLevelExpression
        Else
            Dim patterns = {positionLevelPattern, molecularSpeciesLevelPattern}
            Dim totalPattern = String.Join("|", If(atLeastSpeciesLevel, patterns, patterns.Append(submolecularLevelPattern)))
            Dim totalExpression = New Regex(totalPattern, RegexOptions.Compiled)
            Pattern = totalPattern
            Expression = totalExpression
        End If
        Me.HasSphingosine = hasSphingosine
    End Sub

    Public ReadOnly Property ChainCount As Integer
    Public ReadOnly Property Capacity As Integer
    Public ReadOnly Property Pattern As String

    Private ReadOnly Expression As Regex

    Public Function Parse(lipidStr As String) As ITotalChain
        Dim match = Expression.Match(lipidStr)
        If match.Success Then
            Dim groups = match.Groups
            If groups("PositionLevel").Success Then
                Dim chains = ParsePositionLevelChains(groups)
                If chains.ChainCount = Capacity Then
                    Return chains
                End If
            ElseIf groups("MolecularSpeciesLevel").Success Then
                Dim chains = ParseMolecularSpeciesLevelChains(groups)
                If chains.ChainCount = Capacity Then
                    Return chains
                ElseIf chains.ChainCount < Capacity Then
                    Return New MolecularSpeciesLevelChains(chains.GetDeterminedChains().Concat(Enumerable.Range(0, Capacity - chains.ChainCount).[Select](Function(__) New AcylChain(0, DoubleBond.CreateFromPosition(), Oxidized.CreateFromPosition()))).ToArray())
                End If
            ElseIf ChainCount > 1 AndAlso groups("TotalChain").Success Then
                Return ParseTotalChains(groups, ChainCount)
            End If
        End If
        Return Nothing
    End Function

    Private Function ParsePositionLevelChains(groups As GroupCollection) As PositionLevelChains
        Dim matches = groups("Chain").Captures.Cast(Of Capture)().ToArray()
        Dim sphingo As IChain = Nothing
        If HasSphingosine Then
            If CSharpImpl.__Assign(sphingo, TryCast(SphingoParser.Parse(matches(0).Value), IChain)) IsNot Nothing Then
                Return New PositionLevelChains(matches.Skip(1).[Select](Function(c) If(AlkylParser.Parse(c.Value), AcylParser.Parse(c.Value))).Prepend(sphingo).ToArray())
            End If
            Return Nothing
        End If
        Return New PositionLevelChains(matches.[Select](Function(c) If(AlkylParser.Parse(c.Value), AcylParser.Parse(c.Value))).ToArray())

    End Function

    Private Function ParseMolecularSpeciesLevelChains(groups As GroupCollection) As MolecularSpeciesLevelChains
        Return New MolecularSpeciesLevelChains(groups("Chain").Captures.Cast(Of Capture)().[Select](Function(c) If(AlkylParser.Parse(c.Value), AcylParser.Parse(c.Value))).ToArray())
    End Function

    Private Function ParseTotalChains(groups As GroupCollection, chainCount As Integer) As TotalChain
        Dim carbon = Integer.Parse(groups("carbon").Value)
        Dim db = Integer.Parse(groups("db").Value)
        Dim ox = If(Not groups("ox").Success, 0, If(Not groups("oxnum").Success, 1, Integer.Parse(groups("oxnum").Value)))

        Select Case groups("plasm").Value
            Case "O-"
                Return New TotalChain(carbon, db, ox, chainCount - 1, 1, 0)
            Case "dO-"
                Return New TotalChain(carbon, db, ox, chainCount - 2, 2, 0)
            Case "eO-"
                Return New TotalChain(carbon, db, ox, chainCount - 4, 4, 0)
            Case "P-"
                Return New TotalChain(carbon, db + 1, ox, chainCount - 1, 1, 0)
            Case "dP-"
                Return New TotalChain(carbon, db + 2, ox, chainCount - 2, 2, 0)
            Case "eP-"
                Return New TotalChain(carbon, db + 4, ox, chainCount - 4, 4, 0)
            Case ""
                If HasSphingosine Then
                    Return New TotalChain(carbon, db, ox, chainCount - 1, 0, 1)
                Else
                    Return New TotalChain(carbon, db, ox, chainCount, 0, 0)
                End If
        End Select
        Return Nothing
    End Function

End Class
