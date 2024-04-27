#Region "Microsoft.VisualBasic::7dd2dd2f64464823aa4b69a9983a95ed, G:/mzkit/src/metadb/Lipidomics//ChainsVisitor.vb"

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

    '   Total Lines: 15
    '    Code Lines: 13
    ' Comment Lines: 0
    '   Blank Lines: 2
    '     File Size: 765 B


    ' Class ChainsVisitor
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Visit
    ' 
    ' /********************************************************************************/

#End Region

Friend NotInheritable Class ChainsVisitor
    Implements IVisitor(Of ITotalChain, ITotalChain)
    Private ReadOnly _chainVisitor As IAcyclicVisitor
    Private ReadOnly _chainsVisitor As IVisitor(Of ITotalChain, ITotalChain)

    Public Sub New(chainVisitor As IAcyclicVisitor, chainsVisitor As IVisitor(Of ITotalChain, ITotalChain))
        _chainVisitor = chainVisitor
        _chainsVisitor = chainsVisitor
    End Sub

    Private Function Visit(item As ITotalChain) As ITotalChain Implements IVisitor(Of ITotalChain, ITotalChain).Visit
        Dim converted = item.Accept(_chainsVisitor, IdentityDecomposer(Of ITotalChain, ITotalChain).Instance)
        Return converted.Accept(_chainVisitor, New ChainsDecomposer())
    End Function
End Class

