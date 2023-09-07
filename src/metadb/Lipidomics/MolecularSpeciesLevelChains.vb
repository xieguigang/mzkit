Imports Microsoft.VisualBasic.ComponentModel.Algorithm


Public Class MolecularSpeciesLevelChains
    Inherits SeparatedChains
    Implements ITotalChain
    Private Shared ReadOnly CHAIN_COMPARER As ChainComparer = New ChainComparer()

    Public Sub New(ParamArray chains As IChain())
        MyBase.New(chains.OrderBy(Function(c) c, CHAIN_COMPARER).ToArray(), LipidDescription.Class Or LipidDescription.Chain)

    End Sub

    Private Function GetChainByPosition(position As Integer) As IChain Implements ITotalChain.GetChainByPosition
        Return Nothing
    End Function

    Private Function GetCandidateSets(totalChainGenerator As ITotalChainVariationGenerator) As IEnumerable(Of ITotalChain) Implements ITotalChain.GetCandidateSets
        Return totalChainGenerator.Permutate(Me)
    End Function

    Public Overrides Function ToString() As String
        If GetDeterminedChains().Count(Function(c) c.CarbonCount > 0) = 1 Then
            Return Enumerable.First(GetDeterminedChains(), Function(c) c.CarbonCount > 0).ToString() ' for LPC...
        End If
        Return String.Join("_", GetDeterminedChains().[Select](Function(c) c.ToString()))
    End Function

    Private Function Includes(chains As ITotalChain) As Boolean Implements ITotalChain.Includes
        Dim sChains As SeparatedChains = Nothing

        If chains.ChainCount <> ChainCount OrElse Not (CSharpImpl.__Assign(sChains, TryCast(chains, SeparatedChains)) IsNot Nothing) Then
            Return False
        End If

        Dim matching = New BipartiteMatching(ChainCount + chains.ChainCount)
        For i = 0 To GetDeterminedChains().Length - 1
            For j = 0 To sChains.GetDeterminedChains().Length - 1
                If GetDeterminedChains()(i).Includes(sChains.GetDeterminedChains()(j)) Then
                    matching.AddEdge(i, j + ChainCount)
                End If
            Next
        Next
        Return matching.Match() = ChainCount
    End Function

    Public Overrides Function Equals(other As ITotalChain) As Boolean Implements IEquatable(Of ITotalChain).Equals
        Dim mChains As MolecularSpeciesLevelChains = Nothing
        Return CSharpImpl.__Assign(mChains, TryCast(other, MolecularSpeciesLevelChains)) IsNot Nothing AndAlso ChainCount = other.ChainCount AndAlso CarbonCount = other.CarbonCount AndAlso DoubleBondCount = other.DoubleBondCount AndAlso OxidizedCount = other.OxidizedCount AndAlso Description = other.Description AndAlso GetDeterminedChains().Zip(mChains.GetDeterminedChains(), Function(a, b) a.Equals(b)).All(Function(p) p)
    End Function

    Public Overrides Function Accept(Of TResult)(visitor As IAcyclicVisitor, decomposer As IAcyclicDecomposer(Of TResult)) As TResult Implements IVisitableElement.Accept
        Dim decomposer_ As IDecomposer(Of TResult, MolecularSpeciesLevelChains) = Nothing

        If CSharpImpl.__Assign(decomposer_, TryCast(decomposer, IDecomposer(Of TResult, MolecularSpeciesLevelChains))) IsNot Nothing Then
            Return decomposer_.Decompose(visitor, Me)
        End If
        Return Nothing
    End Function

    Friend Class ChainComparer
        Implements IComparer(Of IChain)
        Public Function Compare(x As IChain, y As IChain) As Integer Implements IComparer(Of IChain).Compare
            Dim priorityx = (TypeToOrder(x), x.DoubleBondCount, x.CarbonCount, x.OxidizedCount)
            Dim priorityy = (TypeToOrder(y), y.DoubleBondCount, y.CarbonCount, y.OxidizedCount)
            Return priorityx.CompareTo(priorityy)
        End Function

        Private Function TypeToOrder(x As IChain) As Integer
            Select Case x.GetType
                Case GetType(SphingoChain)
                    Return 0
                Case GetType(AlkylChain)
                    Return 1
                Case GetType(AcylChain)
                    Return 2
                Case Else
                    Return 3
            End Select
        End Function
    End Class

End Class

