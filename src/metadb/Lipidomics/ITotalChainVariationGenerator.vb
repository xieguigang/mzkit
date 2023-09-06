Imports System.Collections.Generic


Public Interface ITotalChainVariationGenerator
        Function Separate(ByVal chain As TotalChain) As IEnumerable(Of ITotalChain)

        Function Permutate(ByVal chains As MolecularSpeciesLevelChains) As IEnumerable(Of ITotalChain)

        Function Product(ByVal chains As PositionLevelChains) As IEnumerable(Of ITotalChain)
    End Interface

