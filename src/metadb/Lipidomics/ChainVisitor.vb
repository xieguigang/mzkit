#Region "Microsoft.VisualBasic::b8d7b74371719b763380f66cfec84526, metadb\Lipidomics\ChainVisitor.vb"

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

    '   Total Lines: 85
    '    Code Lines: 71 (83.53%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 14 (16.47%)
    '     File Size: 4.10 KB


    ' Class ChainVisitor
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Visit1, Visit2, Visit3
    ' 
    ' Class AcylChainVisitor
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Visit
    ' 
    ' Class AlkylChainVisitor
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Visit
    ' 
    ' Class SphingosineChainVisitor
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Visit
    ' 
    ' /********************************************************************************/

#End Region

Friend NotInheritable Class ChainVisitor
    Implements IVisitor(Of AcylChain, AcylChain), IVisitor(Of AlkylChain, AlkylChain), IVisitor(Of SphingoChain, SphingoChain)
    Private ReadOnly _acylVisitor As IVisitor(Of AcylChain, AcylChain)
    Private ReadOnly _alkylVisitor As IVisitor(Of AlkylChain, AlkylChain)
    Private ReadOnly _sphingosineVisitor As IVisitor(Of SphingoChain, SphingoChain)

    Public Sub New(acylVisitor As IVisitor(Of AcylChain, AcylChain), alkylVisitor As IVisitor(Of AlkylChain, AlkylChain), sphingosineVisitor As IVisitor(Of SphingoChain, SphingoChain))
        _acylVisitor = acylVisitor
        _alkylVisitor = alkylVisitor
        _sphingosineVisitor = sphingosineVisitor
    End Sub

    Private Function Visit1(item As AcylChain) As AcylChain Implements IVisitor(Of AcylChain, AcylChain).Visit
        Return _acylVisitor.Visit(item)
    End Function
    Private Function Visit2(item As AlkylChain) As AlkylChain Implements IVisitor(Of AlkylChain, AlkylChain).Visit
        Return _alkylVisitor.Visit(item)
    End Function
    Private Function Visit3(item As SphingoChain) As SphingoChain Implements IVisitor(Of SphingoChain, SphingoChain).Visit
        Return _sphingosineVisitor.Visit(item)
    End Function
End Class

Friend NotInheritable Class AcylChainVisitor
    Implements IVisitor(Of AcylChain, AcylChain)
    Private ReadOnly _doubleBondVisitor As IVisitor(Of IDoubleBond, IDoubleBond)
    Private ReadOnly _oxidizedVisitor As IVisitor(Of IOxidized, IOxidized)

    Public Sub New(doubleBondVisitor As IVisitor(Of IDoubleBond, IDoubleBond), oxidizedVisitor As IVisitor(Of IOxidized, IOxidized))
        _doubleBondVisitor = doubleBondVisitor
        _oxidizedVisitor = oxidizedVisitor
    End Sub

    Public Function Visit(item As AcylChain) As AcylChain Implements IVisitor(Of AcylChain, AcylChain).Visit
        Dim db = _doubleBondVisitor.Visit(item.DoubleBond)
        Dim ox = _oxidizedVisitor.Visit(item.Oxidized)
        If db.Equals(item.DoubleBond) AndAlso ox.Equals(item.Oxidized) Then
            Return item
        End If
        Return New AcylChain(item.CarbonCount, db, ox)
    End Function

End Class

Friend NotInheritable Class AlkylChainVisitor
    Implements IVisitor(Of AlkylChain, AlkylChain)
    Private ReadOnly _doubleBondVisitor As IVisitor(Of IDoubleBond, IDoubleBond)
    Private ReadOnly _oxidizedVisitor As IVisitor(Of IOxidized, IOxidized)

    Public Sub New(doubleBondVisitor As IVisitor(Of IDoubleBond, IDoubleBond), oxidizedVisitor As IVisitor(Of IOxidized, IOxidized))
        _doubleBondVisitor = doubleBondVisitor
        _oxidizedVisitor = oxidizedVisitor
    End Sub

    Public Function Visit(item As AlkylChain) As AlkylChain Implements IVisitor(Of AlkylChain, AlkylChain).Visit
        Dim db = _doubleBondVisitor.Visit(item.DoubleBond)
        Dim ox = _oxidizedVisitor.Visit(item.Oxidized)
        If db.Equals(item.DoubleBond) AndAlso ox.Equals(item.Oxidized) Then
            Return item
        End If
        Return New AlkylChain(item.CarbonCount, db, ox)
    End Function

End Class

Friend NotInheritable Class SphingosineChainVisitor
    Implements IVisitor(Of SphingoChain, SphingoChain)
    Private ReadOnly _doubleBondVisitor As IVisitor(Of IDoubleBond, IDoubleBond)
    Private ReadOnly _oxidizedVisitor As IVisitor(Of IOxidized, IOxidized)

    Public Sub New(doubleBondVisitor As IVisitor(Of IDoubleBond, IDoubleBond), oxidizedVisitor As IVisitor(Of IOxidized, IOxidized))
        _doubleBondVisitor = doubleBondVisitor
        _oxidizedVisitor = oxidizedVisitor
    End Sub

    Public Function Visit(item As SphingoChain) As SphingoChain Implements IVisitor(Of SphingoChain, SphingoChain).Visit
        Dim db = _doubleBondVisitor.Visit(item.DoubleBond)
        Dim ox = _oxidizedVisitor.Visit(item.Oxidized)
        If db.Equals(item.DoubleBond) AndAlso ox.Equals(item.Oxidized) Then
            Return item
        End If
        Return New SphingoChain(item.CarbonCount, db, ox)
    End Function

End Class
