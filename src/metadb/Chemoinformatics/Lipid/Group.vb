Namespace Lipidomics

    Public Class Group : Inherits BondPosition

        Public Property groupName As String

        Public Overrides Function ToString() As String
            Return $"{groupName}({index}{[structure]})"
        End Function

    End Class
End Namespace