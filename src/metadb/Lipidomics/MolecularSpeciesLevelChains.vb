#Region "Microsoft.VisualBasic::af28b83f56ffde37916405be8b41f257, metadb\Lipidomics\MolecularSpeciesLevelChains.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 90
    '    Code Lines: 73
    ' Comment Lines: 0
    '   Blank Lines: 17
    '     File Size: 4.03 KB


    ' Class MolecularSpeciesLevelChains
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Accept, Equals, GetCandidateSets, GetChainByPosition, Includes
    '               ToString
    '     Class ChainComparer
    ' 
    '         Function: Compare, TypeToOrder
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
        Dim sChains As SeparatedChains = TryCast(chains, SeparatedChains)

        If chains.ChainCount <> ChainCount OrElse sChains IsNot Nothing Then
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
        Dim mChains As MolecularSpeciesLevelChains = TryCast(other, MolecularSpeciesLevelChains)

        Return mChains IsNot Nothing AndAlso
            ChainCount = other.ChainCount AndAlso
            CarbonCount = other.CarbonCount AndAlso
            DoubleBondCount = other.DoubleBondCount AndAlso
            OxidizedCount = other.OxidizedCount AndAlso
            Description = other.Description AndAlso
            GetDeterminedChains().Zip(mChains.GetDeterminedChains(), Function(a, b) a.Equals(b)).All(Function(p) p)
    End Function

    Public Overrides Function Accept(Of TResult)(visitor As IAcyclicVisitor, decomposer As IAcyclicDecomposer(Of TResult)) As TResult Implements IVisitableElement.Accept
        Dim decomposer_ As IDecomposer(Of TResult, MolecularSpeciesLevelChains) = TryCast(decomposer, IDecomposer(Of TResult, MolecularSpeciesLevelChains))

        If decomposer_ IsNot Nothing Then
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
