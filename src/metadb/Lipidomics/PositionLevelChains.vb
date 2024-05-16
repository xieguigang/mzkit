#Region "Microsoft.VisualBasic::75691394c62cf75c18a9daec79277d48, metadb\Lipidomics\PositionLevelChains.vb"

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

    '   Total Lines: 53
    '    Code Lines: 43
    ' Comment Lines: 0
    '   Blank Lines: 10
    '     File Size: 2.60 KB


    ' Class PositionLevelChains
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Accept, Equals, GetCandidateSets, GetChainByPosition, Includes
    '               ToString
    ' 
    ' /********************************************************************************/

#End Region

Public Class PositionLevelChains : Inherits SeparatedChains
    Implements ITotalChain

    Private ReadOnly _chains As IChain()

    Public Sub New(ParamArray chains As IChain())
        MyBase.New(chains.[Select](Function(c, i) (c, i)).ToArray(), LipidDescription.Class Or LipidDescription.Chain Or LipidDescription.SnPosition)
        _chains = chains
    End Sub

    Private Function GetChainByPosition(position As Integer) As IChain Implements ITotalChain.GetChainByPosition
        If position > _chains.Length Then
            Return Nothing
        End If
        Return _chains(position - 1)
    End Function

    Private Function GetCandidateSets(totalChainGenerator As ITotalChainVariationGenerator) As IEnumerable(Of ITotalChain) Implements ITotalChain.GetCandidateSets
        Return totalChainGenerator.Product(Me)
    End Function

    Public Overrides Function ToString() As String
        Return String.Join("/", GetDeterminedChains().[Select](Function(c) c.ToString()))
    End Function

    Private Function Includes(chains As ITotalChain) As Boolean Implements ITotalChain.Includes
        Dim pChains As PositionLevelChains = TryCast(chains, PositionLevelChains)
        Return chains.ChainCount = ChainCount AndAlso
            pChains IsNot Nothing AndAlso
            Enumerable.Range(0, ChainCount).All(Function(i) GetDeterminedChains()(i).Includes(pChains.GetDeterminedChains()(i)))
    End Function

    Public Overrides Function Equals(other As ITotalChain) As Boolean Implements IEquatable(Of ITotalChain).Equals
        Dim pChains As PositionLevelChains = TryCast(other, PositionLevelChains)
        Return pChains IsNot Nothing AndAlso
            ChainCount = other.ChainCount AndAlso
            CarbonCount = other.CarbonCount AndAlso
            DoubleBondCount = other.DoubleBondCount AndAlso
            OxidizedCount = other.OxidizedCount AndAlso
            Description = other.Description AndAlso
            GetDeterminedChains().Zip(pChains.GetDeterminedChains(), Function(a, b) a.Equals(b)).All(Function(p) p)
    End Function

    Public Overrides Function Accept(Of TResult)(visitor As IAcyclicVisitor, decomposer As IAcyclicDecomposer(Of TResult)) As TResult Implements IVisitableElement.Accept
        Dim decomposer_ As IDecomposer(Of TResult, PositionLevelChains) = TryCast(decomposer, IDecomposer(Of TResult, PositionLevelChains))

        If decomposer_ IsNot Nothing Then
            Return decomposer_.Decompose(visitor, Me)
        End If
        Return Nothing
    End Function

End Class
