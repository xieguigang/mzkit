Imports std = System.Math

Public Class SeparatedChains
    Implements ITotalChain
    Private ReadOnly _chains, _decided, _undecided As ChainInformation()

    ''' <summary>
    ''' chains should contains at least 1 chain.
    ''' </summary>
    ''' <param name="chains"></param>
    ''' <param name="description"></param>
    ''' <exceptioncref="ArgumentException"></exception>
    Public Sub New(chains As IChain(), description As LipidDescription)
        If chains.Length = 0 Then
            Throw New ArgumentException(NameOf(chains))
        End If
        _chains = chains.[Select](Function(c) New ChainInformation(c)).ToArray()
        _decided = Array.Empty(Of ChainInformation)()
        _undecided = _chains
        If chains.All(Function(c) c.DoubleBond.UnDecidedCount = 0) Then
            description = description Or LipidDescription.DoubleBondPosition
        End If
        Me.Description = description
    End Sub

    ''' <summary>
    ''' chains should contains at least 1 chain.
    ''' </summary>
    ''' <param name="chains"></param>
    ''' <param name="description"></param>
    ''' <exceptioncref="ArgumentException"></exception>
    Public Sub New(chains As (IChain, Integer)(), description As LipidDescription)
        If chains.Length = 0 Then
            Throw New ArgumentException(NameOf(chains))
        End If
        _chains = chains.[Select](Function(c) New ChainInformation(c.Item1, c.Item2)).ToArray()
        _decided = _chains.Where(Function(c) c.Position >= 0).ToArray()
        _undecided = _chains.Where(Function(c) c.Position < 0).ToArray()
        If chains.All(Function(c) c.Item1.DoubleBond.UnDecidedCount = 0) Then
            description = description Or LipidDescription.DoubleBondPosition
        End If
        Me.Description = description
    End Sub

    Private Function GetChainByPosition(position As Integer) As IChain Implements ITotalChain.GetChainByPosition
        Return _chains.FirstOrDefault(Function(c) c.Position = position)?.Chain
    End Function

    Public Function GetDeterminedChains() As IChain() Implements ITotalChain.GetDeterminedChains
        Return _chains.[Select](Function(c) c.Chain).ToArray()
    End Function

    Public ReadOnly Property ChainCount As Integer Implements ITotalChain.ChainCount
        Get
            Return _chains.Length
        End Get
    End Property
    Public ReadOnly Property AcylChainCount As Integer Implements ITotalChain.AcylChainCount
        Get
            Return _chains.Count(Function(c) TypeOf c.Chain Is AcylChain)
        End Get
    End Property
    Public ReadOnly Property AlkylChainCount As Integer Implements ITotalChain.AlkylChainCount
        Get
            Return _chains.Count(Function(c) TypeOf c.Chain Is AlkylChain)
        End Get
    End Property
    Public ReadOnly Property SphingoChainCount As Integer Implements ITotalChain.SphingoChainCount
        Get
            Return _chains.Count(Function(c) TypeOf c.Chain Is SphingoChain)
        End Get
    End Property
    Public ReadOnly Property CarbonCount As Integer Implements ITotalChain.CarbonCount
        Get
            Return _chains.Sum(Function(c) c.Chain.CarbonCount)
        End Get
    End Property
    Public ReadOnly Property DoubleBondCount As Integer Implements ITotalChain.DoubleBondCount
        Get
            Return _chains.Sum(Function(c) c.Chain.DoubleBondCount)
        End Get
    End Property
    Public ReadOnly Property OxidizedCount As Integer Implements ITotalChain.OxidizedCount
        Get
            Return _chains.Sum(Function(c) c.Chain.OxidizedCount)
        End Get
    End Property
    Public ReadOnly Property Description As LipidDescription Implements ITotalChain.Description
    Public ReadOnly Property Mass As Double Implements ITotalChain.Mass
        Get
            Return _chains.Sum(Function(c) c.Chain.Mass)
        End Get
    End Property

    Private Function GetCandidateSets(totalChainGenerator As ITotalChainVariationGenerator) As IEnumerable(Of ITotalChain) Implements ITotalChain.GetCandidateSets
        Dim gc = New GenerateChain(_chains.Length, _decided)
        Dim indetermined = _undecided.[Select](Function(c) c.Chain).ToArray()
        If indetermined.Length > 0 Then
            Return Permutations(indetermined).[Select](Function(cs) New PositionLevelChains(gc.Apply(cs)))
        End If
        Dim pc As ITotalChain = New PositionLevelChains(_decided.[Select](Function(c) c.Chain).ToArray())
        Return pc.GetCandidateSets(totalChainGenerator)
    End Function

    Public Overrides Function ToString() As String
        Dim box = New ChainInformation(_chains.Length - 1) {}
        For Each c In _decided
            box(c.Position - 1) = c
        Next
        Dim idx = 0
        For Each c In _undecided
            While idx < box.Length AndAlso box(idx) IsNot Nothing
                Threading.Interlocked.Increment(idx)
            End While
            If idx < box.Length Then
                box(std.Min(Threading.Interlocked.Increment(idx), idx - 1)) = c
            End If
        Next
        Return String.Concat(CType(Enumerable.Select(box, CType(Function(c) CStr(c.Chain.ToString() & If(c.Position < 0, "_", "/")), Func(Of ChainInformation, String))), IEnumerable(Of String))).TrimEnd("_"c, "/"c)
    End Function

    Private Function Includes(chains As ITotalChain) As Boolean Implements ITotalChain.Includes
        If chains.ChainCount <> ChainCount Then
            Return False
        End If
        Dim sChains As SeparatedChains = TryCast(chains, SeparatedChains)

        If sChains Is Nothing Then
            Return False
        End If
        If _decided.Length > sChains._decided.Length Then
            Return False
        End If
        Dim used = New HashSet(Of ChainInformation)()
        Dim ci As ChainInformation = Nothing
        For Each d In _decided
            ci = TryCast(sChains._decided.FirstOrDefault(Function(d2) d2.Position = d.Position), ChainInformation)
            If ci IsNot Nothing AndAlso d.Chain.Includes(ci.Chain) Then
                used.Add(ci)
            Else
                Return False
            End If
        Next
        Dim canUse = New HashSet(Of ChainInformation)(sChains._chains.Except(used))
        For Each d In _undecided
            If canUse.All(Function(d2) Not d.Chain.Includes(d2.Chain)) Then
                Return False
            End If
        Next
        Return True
    End Function

    Public Overridable Overloads Function Equals(other As ITotalChain) As Boolean Implements IEquatable(Of ITotalChain).Equals
        If other.ChainCount <> ChainCount Then
            Return False
        End If
        Dim sChains As SeparatedChains = TryCast(other, SeparatedChains)

        If sChains Is Nothing Then
            Return False
        End If
        If _decided.Length > sChains._decided.Length Then
            Return False
        End If
        Dim used = New HashSet(Of ChainInformation)()
        Dim ci As ChainInformation = Nothing
        For Each d In _decided
            ci = TryCast(sChains._decided.FirstOrDefault(Function(d2) d2.Position = d.Position), ChainInformation)
            If ci IsNot Nothing AndAlso d.Chain.Equals(ci.Chain) Then
                used.Add(ci)
            Else
                Return False
            End If
        Next
        Dim canUse = New HashSet(Of ChainInformation)(sChains._chains.Except(used))
        For Each d In _undecided
            If canUse.All(Function(d2) Not d.Chain.Equals(d2.Chain)) Then
                Return False
            End If
        Next
        Return True
    End Function

    Public Overridable Function Accept(Of TResult)(visitor As IAcyclicVisitor, decomposer As IAcyclicDecomposer(Of TResult)) As TResult Implements IVisitableElement.Accept
        Dim decomposer_ As IDecomposer(Of TResult, SeparatedChains) = TryCast(decomposer, IDecomposer(Of TResult, SeparatedChains))

        If decomposer_ IsNot Nothing Then
            Return decomposer_.Decompose(visitor, Me)
        End If
        Return Nothing
    End Function

    Friend Class ChainInformation
        Public Sub New(chain As IChain, position As Integer)
            Me.Chain = chain
            Me.Position = position
        End Sub

        Public Sub New(chain As IChain)
            Me.New(chain, -1)

        End Sub

        Public ReadOnly Property Chain As IChain

        ''' <summary>
        ''' position 1-indexed.
        ''' If position is indetermined, -1 assigned.
        ''' </summary>
        Public ReadOnly Property Position As Integer
    End Class

    Friend Class GenerateChain
        Private ReadOnly _box As IChain()
        Private ReadOnly _remains As List(Of Integer)

        Public Sub New(length As Integer, determined As IEnumerable(Of ChainInformation))
            _box = New IChain(length + 1 - 1) {}
            _remains = Enumerable.Range(1, length).ToList()
            For Each d In determined
                _box(d.Position) = d.Chain
                _remains.Remove(d.Position)
            Next
        End Sub

        Public Function Apply(chains As IEnumerable(Of IChain)) As IChain()
            Dim result = _box.ToArray()
            For Each iC In _remains.Zip(chains, Function(i, c) (i, c))
                Dim i = iC.Item1
                Dim c = iC.Item2
                result(i) = c
            Next
            Return result
        End Function
    End Class

End Class
