#Region "Microsoft.VisualBasic::99ab4141f45aa564f05bd7205d427022, G:/mzkit/src/metadb/Lipidomics//IChainGenerator.vb"

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

    '   Total Lines: 12
    '    Code Lines: 7
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 413 B


    ' Interface IChainGenerator
    ' 
    '     Function: CarbonIsValid, DoubleBondIsValid, (+3 Overloads) Generate
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Public Interface IChainGenerator
    Function Generate(chain As AcylChain) As IEnumerable(Of IChain)

    Function Generate(chain As AlkylChain) As IEnumerable(Of IChain)

    Function Generate(chain As SphingoChain) As IEnumerable(Of IChain)

    Function CarbonIsValid(carbon As Integer) As Boolean

    Function DoubleBondIsValid(carbon As Integer, doubleBond As Integer) As Boolean
End Interface


