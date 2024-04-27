#Region "Microsoft.VisualBasic::16c7ea230869dde0070285aed7f04714, G:/mzkit/src/metadb/Lipidomics//ChainsDecomposer.vb"

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

    '   Total Lines: 16
    '    Code Lines: 14
    ' Comment Lines: 0
    '   Blank Lines: 2
    '     File Size: 1.27 KB


    ' Class ChainsDecomposer
    ' 
    '     Function: Decompose1, Decompose2, Decompose3
    ' 
    ' /********************************************************************************/

#End Region

Friend NotInheritable Class ChainsDecomposer
    Implements IDecomposer(Of ITotalChain, TotalChain), IDecomposer(Of ITotalChain, MolecularSpeciesLevelChains), IDecomposer(Of ITotalChain, PositionLevelChains)
    Private Function Decompose1(Of T As TotalChain)(visitor As IAcyclicVisitor, element As T) As ITotalChain Implements IDecomposer(Of ITotalChain, TotalChain).Decompose
        Return element
    End Function

    Private Function Decompose2(Of T As MolecularSpeciesLevelChains)(visitor As IAcyclicVisitor, element As T) As ITotalChain Implements IDecomposer(Of ITotalChain, MolecularSpeciesLevelChains).Decompose
        Dim chains = element.GetDeterminedChains().[Select](Function(c) c.Accept(visitor, IdentityDecomposer(Of IChain, IChain).Instance)).ToArray()
        Return New MolecularSpeciesLevelChains(chains)
    End Function

    Private Function Decompose3(Of T As PositionLevelChains)(visitor As IAcyclicVisitor, element As T) As ITotalChain Implements IDecomposer(Of ITotalChain, PositionLevelChains).Decompose
        Dim chains = element.GetDeterminedChains().[Select](Function(c) c.Accept(visitor, IdentityDecomposer(Of IChain, IChain).Instance)).ToArray()
        Return New PositionLevelChains(chains)
    End Function
End Class
