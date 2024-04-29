#Region "Microsoft.VisualBasic::d62ba56e1bcf995c08f2d1f7e11caddd, E:/mzkit/src/metadb/Lipidomics//ITotalChainVariationGenerator.vb"

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

    '   Total Lines: 10
    '    Code Lines: 6
    ' Comment Lines: 0
    '   Blank Lines: 4
    '     File Size: 374 B


    ' Interface ITotalChainVariationGenerator
    ' 
    '     Function: Permutate, Product, Separate
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic


Public Interface ITotalChainVariationGenerator
        Function Separate(chain As TotalChain) As IEnumerable(Of ITotalChain)

        Function Permutate(chains As MolecularSpeciesLevelChains) As IEnumerable(Of ITotalChain)

        Function Product(chains As PositionLevelChains) As IEnumerable(Of ITotalChain)
    End Interface
