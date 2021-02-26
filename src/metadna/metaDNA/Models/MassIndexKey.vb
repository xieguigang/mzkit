''' <summary>
''' Indexed of target compound by m/z
''' </summary>
Public Structure MassIndexKey

    Dim mz As Double
    Dim precursorType As String

    Public Overrides Function ToString() As String
        Return $"{precursorType} {mz}"
    End Function

End Structure