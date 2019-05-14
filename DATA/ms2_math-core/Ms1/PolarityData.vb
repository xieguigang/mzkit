
Public Class PolarityData(Of T)

    Public Property positive As T
    Public Property negative As T

    Public Overrides Function ToString() As String
        Return $"(+) {positive} / (-) {negative}"
    End Function

    Public Shared Widening Operator CType(tuple As (pos As T, neg As T)) As PolarityData(Of T)
        Return New PolarityData(Of T) With {
            .positive = tuple.pos,
            .negative = tuple.neg
        }
    End Operator
End Class