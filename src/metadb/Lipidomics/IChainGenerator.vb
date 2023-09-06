Imports System.Collections.Generic

Namespace CompMs.Common.Lipidomics
    Public Interface IChainGenerator
        Function Generate(ByVal chain As AcylChain) As IEnumerable(Of IChain)

        Function Generate(ByVal chain As AlkylChain) As IEnumerable(Of IChain)

        Function Generate(ByVal chain As SphingoChain) As IEnumerable(Of IChain)

        Function CarbonIsValid(ByVal carbon As Integer) As Boolean

        Function DoubleBondIsValid(ByVal carbon As Integer, ByVal doubleBond As Integer) As Boolean
    End Interface
End Namespace
