Imports CompMs.Common.DataStructure
Imports System

Namespace CompMs.Common.Lipidomics
    Friend NotInheritable Class ChainsVisitor
        Implements IVisitor(Of ITotalChain, ITotalChain)
        Private ReadOnly _chainVisitor As IAcyclicVisitor
        Private ReadOnly _chainsVisitor As IVisitor(Of ITotalChain, ITotalChain)

        Public Sub New(ByVal chainVisitor As IAcyclicVisitor, ByVal chainsVisitor As IVisitor(Of ITotalChain, ITotalChain))
            _chainVisitor = If(chainVisitor, CSharpImpl.__Throw(Of IAcyclicVisitor)(New ArgumentNullException(NameOf(chainVisitor))))
            _chainsVisitor = If(chainsVisitor, CSharpImpl.__Throw(Of IVisitor(Of ITotalChain, ITotalChain))(New ArgumentNullException(NameOf(chainsVisitor))))
        End Sub

        Private Function Visit(ByVal item As ITotalChain) As ITotalChain Implements IVisitor(Of ITotalChain, ITotalChain).Visit
            Dim converted = item.Accept(_chainsVisitor, IdentityDecomposer(Of ITotalChain, ITotalChain).Instance)
            Return converted.Accept(_chainVisitor, New ChainsDecomposer())
        End Function

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal throw statements")>
            Shared Function __Throw(Of T)(ByVal e As Exception) As T
                Throw e
            End Function
        End Class
    End Class
End Namespace
