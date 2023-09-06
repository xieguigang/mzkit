Imports CompMs.Common.DataStructure
Imports System.Collections.Generic
Imports System.Linq


Public Class MolecularSpeciesLevelChains
        Inherits SeparatedChains
        Implements ITotalChain
        Private Shared ReadOnly CHAIN_COMPARER As ChainComparer = New ChainComparer()

        Public Sub New(ParamArray chains As IChain())
            MyBase.New(chains.OrderBy(Function(c) c, CHAIN_COMPARER).ToArray(), LipidDescription.Class Or LipidDescription.Chain)

        End Sub

        Private Function GetChainByPosition(ByVal position As Integer) As IChain Implements ITotalChain.GetChainByPosition
            Return Nothing
        End Function

        Private Function GetCandidateSets(ByVal totalChainGenerator As ITotalChainVariationGenerator) As IEnumerable(Of ITotalChain) Implements ITotalChain.GetCandidateSets
            Return totalChainGenerator.Permutate(Me)
        End Function

        Public Overrides Function ToString() As String
            If GetDeterminedChains().Count(Function(c) c.CarbonCount > 0) = 1 Then
                Return Enumerable.First(GetDeterminedChains(), Function(c) c.CarbonCount > 0).ToString() ' for LPC...
            End If
            Return String.Join("_", GetDeterminedChains().[Select](Function(c) c.ToString()))
        End Function

        Private Function Includes(ByVal chains As ITotalChain) As Boolean Implements ITotalChain.Includes
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

        Public Overrides Function Equals(ByVal other As ITotalChain) As Boolean Implements IEquatable(Of ITotalChain).Equals
            Dim mChains As MolecularSpeciesLevelChains = Nothing
            Return CSharpImpl.__Assign(mChains, TryCast(other, MolecularSpeciesLevelChains)) IsNot Nothing AndAlso ChainCount = other.ChainCount AndAlso CarbonCount = other.CarbonCount AndAlso DoubleBondCount = other.DoubleBondCount AndAlso OxidizedCount = other.OxidizedCount AndAlso Description = other.Description AndAlso GetDeterminedChains().Zip(mChains.GetDeterminedChains(), Function(a, b) a.Equals(b)).All(Function(p) p)
        End Function

        Public Overrides Function Accept(Of TResult)(ByVal visitor As IAcyclicVisitor, ByVal decomposer As IAcyclicDecomposer(Of TResult)) As TResult Implements IVisitableElement.Accept
            Dim decomposer_ As IDecomposer(Of TResult, MolecularSpeciesLevelChains) = Nothing

            If CSharpImpl.__Assign(decomposer_, TryCast(decomposer, IDecomposer(Of TResult, MolecularSpeciesLevelChains))) IsNot Nothing Then
                Return decomposer_.Decompose(visitor, Me)
            End If
            Return Nothing
        End Function

        Friend Class ChainComparer
            Implements IComparer(Of IChain)
            Public Function Compare(ByVal x As IChain, ByVal y As IChain) As Integer Implements IComparer(Of IChain).Compare
                Dim priorityx = (TypeToOrder(x), x.DoubleBondCount, x.CarbonCount, x.OxidizedCount)
                Dim priorityy = (TypeToOrder(y), y.DoubleBondCount, y.CarbonCount, y.OxidizedCount)
                Return priorityx.CompareTo(priorityy)
            End Function

            Private Function TypeToOrder(ByVal x As IChain) As Integer
                                ''' Cannot convert SwitchStatementSyntax, System.InvalidCastException: Unable to cast object of type 'Microsoft.CodeAnalysis.VisualBasic.Syntax.EmptyStatementSyntax' to type 'Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseClauseSyntax'.
'''    在 System.Linq.Enumerable.<CastIterator>d__97`1.MoveNext()
'''    在 Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.SeparatedList[TNode](IEnumerable`1 nodes)
'''    在 ICSharpCode.CodeConverter.VB.MethodBodyExecutableStatementVisitor.ConvertSwitchSection(SwitchSectionSyntax section)
'''    在 System.Linq.Enumerable.WhereSelectEnumerableIterator`2.MoveNext()
'''    在 System.Linq.Buffer`1..ctor(IEnumerable`1 source)
'''    在 System.Linq.Enumerable.ToArray[TSource](IEnumerable`1 source)
'''    在 ICSharpCode.CodeConverter.VB.MethodBodyExecutableStatementVisitor.VisitSwitchStatement(SwitchStatementSyntax node)
'''    在 Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor`1.Visit(SyntaxNode node)
'''    在 ICSharpCode.CodeConverter.VB.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)
''' 
''' Input:
'''                 switch (x) {
                    case CompMs.Common.Lipidomics.SphingoChain _:
                        return 0;
                    case CompMs.Common.Lipidomics.AlkylChain _:
                        return 1;
                    case CompMs.Common.Lipidomics.AcylChain _:
                        return 2;
                    default:
                        return 3;
                }

''' 
            End Function
        End Class

End Class

