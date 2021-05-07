Namespace Data

    Public Structure QueryInput

        Dim word As String
        Dim type As Types

        Public Overrides Function ToString() As String
            Return $"{type.Description}+{word.UrlEncode}"
        End Function
    End Structure
End Namespace