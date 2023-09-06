Imports CompMs.Common.DataStructure

Friend NotInheritable Class ChainsDecomposer
    Implements IDecomposer(Of ITotalChain, TotalChain), IDecomposer(Of ITotalChain, MolecularSpeciesLevelChains), IDecomposer(Of ITotalChain, PositionLevelChains)
    Private Function Decompose1(Of T)(ByVal visitor As IAcyclicVisitor, ByVal element As T) As ITotalChain Implements IDecomposer(Of ITotalChain, TotalChain).Decompose
        Return element
    End Function

    Private Function Decompose2(Of T)(ByVal visitor As IAcyclicVisitor, ByVal element As T) As ITotalChain Implements IDecomposer(Of ITotalChain, MolecularSpeciesLevelChains).Decompose
        Dim chains = element.GetDeterminedChains().[Select](Function(c) c.Accept(visitor, IdentityDecomposer(Of IChain, IChain).Instance)).ToArray()
        Return New MolecularSpeciesLevelChains(chains)
    End Function

    Private Function Decompose3(Of T)(ByVal visitor As IAcyclicVisitor, ByVal element As T) As ITotalChain Implements IDecomposer(Of ITotalChain, PositionLevelChains).Decompose
        Dim chains = element.GetDeterminedChains().[Select](Function(c) c.Accept(visitor, IdentityDecomposer(Of IChain, IChain).Instance)).ToArray()
        Return New PositionLevelChains(chains)
    End Function
End Class
