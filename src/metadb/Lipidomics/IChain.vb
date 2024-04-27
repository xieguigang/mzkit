#Region "Microsoft.VisualBasic::bf74a98f2f646b5ae625b9a8091013d7, G:/mzkit/src/metadb/Lipidomics//IChain.vb"

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

    '   Total Lines: 13
    '    Code Lines: 11
    ' Comment Lines: 0
    '   Blank Lines: 2
    '     File Size: 510 B


    ' Interface IChain
    ' 
    '     Properties: CarbonCount, DoubleBond, DoubleBondCount, Mass, Oxidized
    '                 OxidizedCount
    ' 
    '     Function: GetCandidates, Includes
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Public Interface IChain
    Inherits IVisitableElement, IEquatable(Of IChain)
    ReadOnly Property CarbonCount As Integer
    ReadOnly Property DoubleBond As IDoubleBond
    ReadOnly Property Oxidized As IOxidized
    ReadOnly Property DoubleBondCount As Integer
    ReadOnly Property OxidizedCount As Integer
    ReadOnly Property Mass As Double

    Function Includes(chain As IChain) As Boolean
    Function GetCandidates(generator As IChainGenerator) As IEnumerable(Of IChain)
End Interface


