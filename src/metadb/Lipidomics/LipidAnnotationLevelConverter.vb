Imports CompMs.Common.DataStructure

Namespace CompMs.Common.Lipidomics
    Friend NotInheritable Class LipidAnnotationLevelConverter
        Implements IVisitor(Of Lipid, ILipid)
        Private ReadOnly _chainsVisitor As IVisitor(Of ITotalChain, ITotalChain)

        Public Sub New(ByVal chainsVisitor As IVisitor(Of ITotalChain, ITotalChain))
            _chainsVisitor = If(chainsVisitor, CSharpImpl.__Throw(Of IVisitor(Of ITotalChain, ITotalChain))(New ArgumentNullException(NameOf(chainsVisitor))))
        End Sub

        Private Function Visit(ByVal item As ILipid) As Lipid Implements IVisitor(Of Lipid, ILipid).Visit
            Dim converted = item.Chains.Accept(_chainsVisitor, IdentityDecomposer(Of ITotalChain, ITotalChain).Instance)
            Dim lipid As Lipid = Nothing

            If item.Chains Is converted AndAlso CSharpImpl.__Assign(lipid, TryCast(item, Lipid)) IsNot Nothing Then
                Return lipid
            End If
            Return New Lipid(item.LipidClass, item.Mass, converted)
        End Function

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function <Obsolete("Please refactor calling code to use normal throw statements")>
            Shared Function __Throw(Of T)(ByVal e As Exception) As T
                Throw e
            End Function
        End Class
    End Class
End Namespace
