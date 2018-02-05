Namespace MRM

    Public Class [IS]

        Public Property ID As String
        Public Property name As String
        Public Property CIS As Double

        Public Overrides Function ToString() As String
            Return $"Dim {name} As {ID} = {CIS}"
        End Function
    End Class
End Namespace