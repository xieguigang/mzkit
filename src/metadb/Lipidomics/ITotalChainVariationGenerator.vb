Imports System.Collections.Generic


Public Interface ITotalChainVariationGenerator
        Function Separate(chain As TotalChain) As IEnumerable(Of ITotalChain)

        Function Permutate(chains As MolecularSpeciesLevelChains) As IEnumerable(Of ITotalChain)

        Function Product(chains As PositionLevelChains) As IEnumerable(Of ITotalChain)
    End Interface

