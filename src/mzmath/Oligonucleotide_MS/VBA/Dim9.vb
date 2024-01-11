Friend Class Dim9 : Inherits VB6DimensionVector(Of Object)

    Public Sub New()
        MyBase.New(9)
    End Sub

    Public Shared Function Arp() As Integer()
        Return {10, 12, 5, 6, 1, 0}
    End Function

    Public Shared Function Crp() As Integer()
        Return {9, 12, 3, 7, 1, 0}
    End Function

    Public Shared Function Vrp() As Integer()
        Return {10, 13, 2, 8, 1, 0}
    End Function

    Public Shared Function List() As Dictionary(Of String, Integer())
        Return New Dictionary(Of String, Integer()) From {
            {NameOf(Arp), Arp()},
            {NameOf(Crp), Crp()},
            {NameOf(Vrp), Vrp()}
        }
    End Function

End Class
