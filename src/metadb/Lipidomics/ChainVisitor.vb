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
