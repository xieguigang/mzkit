Public Class MzSet

    Public Property mz As Double
    Public Property query As MzQuery()

    Public ReadOnly Property size As Integer
        Get
            Return query.Length
        End Get
    End Property

    Default Public ReadOnly Property Item(i As Integer) As MzQuery
        Get
            Return query(i)
        End Get
    End Property

End Class
