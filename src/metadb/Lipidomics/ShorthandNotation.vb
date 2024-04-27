#Region "Microsoft.VisualBasic::285fad43d25602b1b0eada3f0266115a, G:/mzkit/src/metadb/Lipidomics//ShorthandNotation.vb"

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

    '   Total Lines: 26
    '    Code Lines: 20
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 1.22 KB


    ' Class ChainShorthandNotation
    ' 
    '     Properties: [Default]
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: (+3 Overloads) Visit
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Friend Class ChainShorthandNotation
    Implements IVisitor(Of AcylChain, AcylChain), IVisitor(Of AlkylChain, AlkylChain), IVisitor(Of SphingoChain, SphingoChain)
    Private ReadOnly _chainVisitor As ChainVisitor

    Public Shared ReadOnly Property [Default] As ChainShorthandNotation = New ChainShorthandNotation()

    Private Sub New()
        Dim builder = New ChainVisitorBuilder()
        Dim director = New ShorthandNotationDirector(builder)
        director.Construct()
        _chainVisitor = builder.Create()
    End Sub

    Public Function Visit(item As AcylChain) As AcylChain Implements IVisitor(Of AcylChain, AcylChain).Visit
        Return CType(_chainVisitor, IVisitor(Of AcylChain, AcylChain)).Visit(item)
    End Function

    Public Function Visit(item As AlkylChain) As AlkylChain Implements IVisitor(Of AlkylChain, AlkylChain).Visit
        Return CType(_chainVisitor, IVisitor(Of AlkylChain, AlkylChain)).Visit(item)
    End Function

    Public Function Visit(item As SphingoChain) As SphingoChain Implements IVisitor(Of SphingoChain, SphingoChain).Visit
        Return CType(_chainVisitor, IVisitor(Of SphingoChain, SphingoChain)).Visit(item)
    End Function
End Class


