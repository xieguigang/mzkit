Imports CompMs.Common.DataStructure
Imports System

Friend NotInheritable Class ChainsVisitor
    Implements IVisitor(Of ITotalChain, ITotalChain)
    Private ReadOnly _chainVisitor As IAcyclicVisitor
    Private ReadOnly _chainsVisitor As IVisitor(Of ITotalChain, ITotalChain)

    Public Sub New(chainVisitor As IAcyclicVisitor, chainsVisitor As IVisitor(Of ITotalChain, ITotalChain))
        _chainVisitor = If(chainVisitor, CSharpImpl.__Throw(Of IAcyclicVisitor)(New ArgumentNullException(NameOf(chainVisitor))))
        _chainsVisitor = If(chainsVisitor, CSharpImpl.__Throw(Of IVisitor(Of ITotalChain, ITotalChain))(New ArgumentNullException(NameOf(chainsVisitor))))
    End Sub

    Private Function Visit(item As ITotalChain) As ITotalChain Implements IVisitor(Of ITotalChain, ITotalChain).Visit
        Dim converted = item.Accept(_chainsVisitor, IdentityDecomposer(Of ITotalChain, ITotalChain).Instance)
        Return converted.Accept(_chainVisitor, New ChainsDecomposer())
    End Function
End Class
