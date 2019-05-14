
Public Class PolarityData(Of T)

    Public Property positive As T
    Public Property negative As T

    Public Overrides Function ToString() As String
        Return $"(+) {positive} / (-) {negative}"
    End Function
End Class