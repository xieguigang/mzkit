Imports CompMs.Common.DataStructure
Imports System

Friend NotInheritable Class ChainVisitor
    Implements IVisitor(Of AcylChain, AcylChain), IVisitor(Of AlkylChain, AlkylChain), IVisitor(Of SphingoChain, SphingoChain)
    Private ReadOnly _acylVisitor As IVisitor(Of AcylChain, AcylChain)
    Private ReadOnly _alkylVisitor As IVisitor(Of AlkylChain, AlkylChain)
    Private ReadOnly _sphingosineVisitor As IVisitor(Of SphingoChain, SphingoChain)

    Public Sub New(ByVal acylVisitor As IVisitor(Of AcylChain, AcylChain), ByVal alkylVisitor As IVisitor(Of AlkylChain, AlkylChain), ByVal sphingosineVisitor As IVisitor(Of SphingoChain, SphingoChain))
        _acylVisitor = acylVisitor
        _alkylVisitor = alkylVisitor
        _sphingosineVisitor = sphingosineVisitor
    End Sub

    Private Function Visit1(ByVal item As AcylChain) As AcylChain Implements IVisitor(Of AcylChain, AcylChain).Visit
        Return _acylVisitor.Visit(item)
    End Function
    Private Function Visit2(ByVal item As AlkylChain) As AlkylChain Implements IVisitor(Of AlkylChain, AlkylChain).Visit
        Return _alkylVisitor.Visit(item)
    End Function
    Private Function Visit3(ByVal item As SphingoChain) As SphingoChain Implements IVisitor(Of SphingoChain, SphingoChain).Visit
        Return _sphingosineVisitor.Visit(item)
    End Function
End Class

Friend NotInheritable Class AcylChainVisitor
    Implements IVisitor(Of AcylChain, AcylChain)
    Private ReadOnly _doubleBondVisitor As IVisitor(Of IDoubleBond, IDoubleBond)
    Private ReadOnly _oxidizedVisitor As IVisitor(Of IOxidized, IOxidized)

    Public Sub New(ByVal doubleBondVisitor As IVisitor(Of IDoubleBond, IDoubleBond), ByVal oxidizedVisitor As IVisitor(Of IOxidized, IOxidized))
        _doubleBondVisitor = If(doubleBondVisitor, CSharpImpl.__Throw(Of IVisitor(Of IDoubleBond, IDoubleBond))(New ArgumentNullException(NameOf(doubleBondVisitor))))
        _oxidizedVisitor = If(oxidizedVisitor, CSharpImpl.__Throw(Of IVisitor(Of IOxidized, IOxidized))(New ArgumentNullException(NameOf(oxidizedVisitor))))
    End Sub

    Public Function Visit(ByVal item As AcylChain) As AcylChain Implements IVisitor(Of AcylChain, AcylChain).Visit
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

    Public Sub New(ByVal doubleBondVisitor As IVisitor(Of IDoubleBond, IDoubleBond), ByVal oxidizedVisitor As IVisitor(Of IOxidized, IOxidized))
        _doubleBondVisitor = If(doubleBondVisitor, CSharpImpl.__Throw(Of IVisitor(Of IDoubleBond, IDoubleBond))(New ArgumentNullException(NameOf(doubleBondVisitor))))
        _oxidizedVisitor = If(oxidizedVisitor, CSharpImpl.__Throw(Of IVisitor(Of IOxidized, IOxidized))(New ArgumentNullException(NameOf(oxidizedVisitor))))
    End Sub

    Public Function Visit(ByVal item As AlkylChain) As AlkylChain Implements IVisitor(Of AlkylChain, AlkylChain).Visit
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

    Public Sub New(ByVal doubleBondVisitor As IVisitor(Of IDoubleBond, IDoubleBond), ByVal oxidizedVisitor As IVisitor(Of IOxidized, IOxidized))
        _doubleBondVisitor = If(doubleBondVisitor, CSharpImpl.__Throw(Of IVisitor(Of IDoubleBond, IDoubleBond))(New ArgumentNullException(NameOf(doubleBondVisitor))))
        _oxidizedVisitor = If(oxidizedVisitor, CSharpImpl.__Throw(Of IVisitor(Of IOxidized, IOxidized))(New ArgumentNullException(NameOf(oxidizedVisitor))))
    End Sub

    Public Function Visit(ByVal item As SphingoChain) As SphingoChain Implements IVisitor(Of SphingoChain, SphingoChain).Visit
        Dim db = _doubleBondVisitor.Visit(item.DoubleBond)
        Dim ox = _oxidizedVisitor.Visit(item.Oxidized)
        If db.Equals(item.DoubleBond) AndAlso ox.Equals(item.Oxidized) Then
            Return item
        End If
        Return New SphingoChain(item.CarbonCount, db, ox)
    End Function

End Class
