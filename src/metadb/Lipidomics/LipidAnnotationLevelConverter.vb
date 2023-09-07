Public NotInheritable Class IdentityDecomposer(Of TResult, TElement)
    Implements IDecomposer(Of TResult, TElement)
    Private Shared _instance As IdentityDecomposer(Of TResult, TElement)

    Public Shared ReadOnly Property Instance As IdentityDecomposer(Of TResult, TElement)
        Get
            Return If(_instance, Function()
                                     _instance = New IdentityDecomposer(Of TResult, TElement)()
                                     Return _instance
                                 End Function())
        End Get
    End Property

    Private Function Decompose(Of T As TElement)(ByVal visitor As IAcyclicVisitor, ByVal element As T) As TResult Implements IDecomposer(Of TResult, TElement).Decompose
        Dim vis As IVisitor(Of TResult, T) = TryCast(visitor, IVisitor(Of TResult, T))

        If vis IsNot Nothing Then
            Return vis.Visit(element)
        End If
        Return Nothing
    End Function
End Class

Friend NotInheritable Class LipidAnnotationLevelConverter
    Implements IVisitor(Of Lipid, ILipid)
    Private ReadOnly _chainsVisitor As IVisitor(Of ITotalChain, ITotalChain)

    Public Sub New(chainsVisitor As IVisitor(Of ITotalChain, ITotalChain))
        _chainsVisitor = chainsVisitor
    End Sub

    Private Function Visit(item As ILipid) As Lipid Implements IVisitor(Of Lipid, ILipid).Visit
        Dim converted = item.Chains.Accept(_chainsVisitor, IdentityDecomposer(Of ITotalChain, ITotalChain).Instance)
        Dim lipid As Lipid = TryCast(item, Lipid)

        If item.Chains Is converted AndAlso lipid IsNot Nothing Then
            Return lipid
        End If
        Return New Lipid(item.LipidClass, item.Mass, converted)
    End Function
End Class
