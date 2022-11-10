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

    Public Overrides Function ToString() As String
        Return $"{mz.ToString("F4")}: {query.Select(Function(i) i.name).JoinBy("; ")}"
    End Function

End Class
