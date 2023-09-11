Public Interface IChainGenerator
    Function Generate(chain As AcylChain) As IEnumerable(Of IChain)

    Function Generate(chain As AlkylChain) As IEnumerable(Of IChain)

    Function Generate(chain As SphingoChain) As IEnumerable(Of IChain)

    Function CarbonIsValid(carbon As Integer) As Boolean

    Function DoubleBondIsValid(carbon As Integer, doubleBond As Integer) As Boolean
End Interface

