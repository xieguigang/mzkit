Imports System.Runtime.CompilerServices

Public Interface ITotalChain
    Inherits IEquatable(Of ITotalChain), IVisitableElement
    ReadOnly Property CarbonCount As Integer
    ReadOnly Property DoubleBondCount As Integer
    ReadOnly Property OxidizedCount As Integer
    ReadOnly Property ChainCount As Integer
    ReadOnly Property AcylChainCount As Integer
    ReadOnly Property AlkylChainCount As Integer
    ReadOnly Property SphingoChainCount As Integer
    ReadOnly Property Mass As Double
    ReadOnly Property Description As LipidDescription

    ''' <summary>
    ''' Retrieve the determined chain by position.
    ''' The position here refers to a specific order defined for each lipid class.
    ''' It may not necessarily match the commonly used sn-position for lipids.
    ''' </summary>
    ''' <paramname="position">1-indexed position</param>
    ''' <returns>IChain if the specified position chain is determined; otherwise, null.</returns>
    Function GetChainByPosition(position As Integer) As IChain
    ''' <summary>
    ''' This method returns an array of lipid chains with confirmed structures.
    ''' It only includes the chains that have their structures determined, and there is no guarantee that the position and index will match.
    ''' </summary>
    ''' <returns>IChain[]</returns>
    Function GetDeterminedChains() As IChain()
    Function Includes(chains As ITotalChain) As Boolean
    Function GetCandidateSets(totalChainGenerator As ITotalChainVariationGenerator) As IEnumerable(Of ITotalChain)
End Interface

Public Module TotalChainExtension
        <Extension()>
        Public Function GetChains(lipid As LipidMolecule) As ITotalChain
            Dim prop = LipidClassDictionary.Default.LbmItems(lipid.LipidClass)
            Select Case lipid.AnnotationLevel
                Case 1
                    Return New TotalChain(lipid.TotalCarbonCount, lipid.TotalDoubleBondCount, lipid.TotalOxidizedCount, acylChainCount:=prop.AcylChain, alkylChainCount:=prop.AlkylChain, sphingoChainCount:=prop.SphingoChain)
                Case 2, 3
                    Return New MolecularSpeciesLevelChains(GetEachChains(lipid, prop))
                Case Else
            End Select
            Return Nothing
        End Function

        Private Function GetEachChains(lipid As LipidMolecule, prop As LipidClassProperty) As IChain()
            Dim chains = New IChain(prop.TotalChain - 1) {}
            If prop.TotalChain >= 1 Then
                If prop.SphingoChain >= 1 Then
                    ' chains[0] = SphingoParser.Parse(lipid.Sn1AcylChainString);
                    chains(0) = New SphingoChain(lipid.Sn1CarbonCount, New DoubleBond(lipid.Sn1DoubleBondCount), New Oxidized(lipid.Sn1Oxidizedount))
                ElseIf prop.AlkylChain >= 1 Then
                    chains(0) = New AlkylChain(lipid.Sn1CarbonCount, New DoubleBond(lipid.Sn1DoubleBondCount), New Oxidized(lipid.Sn1Oxidizedount))
                Else
                    chains(0) = New AcylChain(lipid.Sn1CarbonCount, New DoubleBond(lipid.Sn1DoubleBondCount), New Oxidized(lipid.Sn1Oxidizedount))
                End If
            End If
            If prop.TotalChain >= 2 Then
                chains(1) = New AcylChain(lipid.Sn2CarbonCount, New DoubleBond(lipid.Sn2DoubleBondCount), New Oxidized(lipid.Sn2Oxidizedount))
            End If
            If prop.TotalChain >= 3 Then
                chains(2) = New AcylChain(lipid.Sn3CarbonCount, New DoubleBond(lipid.Sn3DoubleBondCount), New Oxidized(lipid.Sn3Oxidizedount))
            End If
            If prop.TotalChain >= 4 Then
                chains(3) = New AcylChain(lipid.Sn4CarbonCount, New DoubleBond(lipid.Sn4DoubleBondCount), New Oxidized(lipid.Sn4Oxidizedount))
            End If
            Return chains
        End Function

        <Extension()>
        Public Function GetTypedChains(Of T As IChain)(chain As ITotalChain) As IEnumerable(Of T)
            Return chain.GetDeterminedChains().OfType(Of T)()
        End Function

        <Extension()>
        Public Function Deconstruct(Of tT As IChain, tU As IChain)(chain As ITotalChain) As (tT, tU)
            If chain.ChainCount <> 2 OrElse GetType(tT) Is GetType(tU) Then
                Return Nothing
            End If
            Dim t = chain.GetTypedChains(Of tT)().SingleOrDefault()
            Dim u = chain.GetTypedChains(Of tU)().SingleOrDefault()
            If TypeOf t Is tT AndAlso TypeOf u Is tU Then
                Return (t, u)
            End If
            Return Nothing
        End Function

        <Extension()>
        Public Sub ApplyToChain(chains As ITotalChain, position As Integer, action As Action(Of IChain))
            Dim chain = chains.GetChainByPosition(position)
            If chain IsNot Nothing Then
                action?.Invoke(chain)
            End If
        End Sub

    <Extension()>
    Public Sub ApplyToChain(Of T As IChain)(chains As ITotalChain, position As Integer, action As Action(Of T))
        Dim chain = chains.GetChainByPosition(position)
        Dim c As T = Nothing

        If CSharpImpl.__Assign(c, TryCast(chain, T)) IsNot Nothing Then
            action?.Invoke(c)
        End If
    End Sub
End Module

