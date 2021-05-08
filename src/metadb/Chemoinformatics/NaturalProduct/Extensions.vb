Namespace NaturalProduct

    <HideModuleName>
    Public Module Extensions

        Public Function GetQuantityPrefix(n As Integer) As String
            If n <= 12 Then
                Return CType(n, QuantityPrefix).Description
            Else
                Return n.ToString
            End If
        End Function
    End Module
End Namespace