Public Class SeedsProvider

    ReadOnly unknowns As UnknownSet

    Sub New(unknowns As UnknownSet)
        Me.unknowns = unknowns
    End Sub

    Public Iterator Function Seeding(infer As IEnumerable(Of InferLink)) As IEnumerable(Of AnnotatedSeed)

    End Function
End Class
