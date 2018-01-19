Namespace MarkupData

    Public Interface IBase64Container
        Property BinaryArray As String

        Function GetPrecision() As Integer
        Function GetCompressionType() As String
    End Interface
End Namespace