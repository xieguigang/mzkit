#Region "Microsoft.VisualBasic::adc54be5efd001b9b996e82d7838f586, G:/mzkit/src/metadb/Lipidomics//ShortHandGenerator.vb"

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

    '   Total Lines: 19
    '    Code Lines: 15
    ' Comment Lines: 0
    '   Blank Lines: 4
    '     File Size: 848 B


    ' Class ShortHandGenerator
    ' 
    '     Function: [Short], Permutate, Product, Separate
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Public Class ShortHandGenerator
    Implements ITotalChainVariationGenerator
    Public Iterator Function Permutate(chains As MolecularSpeciesLevelChains) As IEnumerable(Of ITotalChain) Implements ITotalChainVariationGenerator.Permutate
        Yield [Short](chains)
    End Function

    Public Iterator Function Product(chains As PositionLevelChains) As IEnumerable(Of ITotalChain) Implements ITotalChainVariationGenerator.Product
        Yield [Short](chains)
    End Function

    Private Function [Short](chains As ITotalChain) As ITotalChain
        Return ChainsIndeterminateState.SpeciesLevel.Indeterminate(chains)
    End Function

    Public Iterator Function Separate(chain As TotalChain) As IEnumerable(Of ITotalChain) Implements ITotalChainVariationGenerator.Separate
        Yield chain
    End Function
End Class


