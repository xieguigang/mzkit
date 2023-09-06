Imports CompMs.Common.DataStructure


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

        Public Function Visit(ByVal item As AcylChain) As AcylChain Implements IVisitor(Of AcylChain, AcylChain).Visit
            Return CType(_chainVisitor, IVisitor(Of AcylChain, AcylChain)).Visit(item)
        End Function

        Public Function Visit(ByVal item As AlkylChain) As AlkylChain Implements IVisitor(Of AlkylChain, AlkylChain).Visit
            Return CType(_chainVisitor, IVisitor(Of AlkylChain, AlkylChain)).Visit(item)
        End Function

        Public Function Visit(ByVal item As SphingoChain) As SphingoChain Implements IVisitor(Of SphingoChain, SphingoChain).Visit
            Return CType(_chainVisitor, IVisitor(Of SphingoChain, SphingoChain)).Visit(item)
        End Function
    End Class

