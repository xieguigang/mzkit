Public Class ShortHandGenerator
    Implements ITotalChainVariationGenerator
    Public Iterator Function Permutate(chains As MolecularSpeciesLevelChains) As IEnumerable(Of ITotalChain) Implements ITotalChainVariationGenerator.Permutate
        Yield [Short](chains)
    End Function

    Public Iterator Function Product(chains As PositionLevelChains) As IEnumerable(Of ITotalChain) Implements ITotalChainVariationGenerator.Product
        Yield [Short](chains)
    End Function

    Private Function [Short](chains As ITotalChain) As ITotalChain
        Return ChainsIndeterminateState.SpeciesLevel.Indeterminate(chains)
    End Function

    Public Iterator Function Separate(chain As TotalChain) As IEnumerable(Of ITotalChain) Implements ITotalChainVariationGenerator.Separate
        Yield chain
    End Function
End Class

