
Imports SMRUCC.MassSpectrum.Math.Ms1.PrecursorType

Public Class PolarityData(Of T)

    Public Property positive As T
    Public Property negative As T

    Default Public Property Item(ionMode As String) As T
        Get
            If Provider.ParseIonMode(ionMode) = 1 Then
                Return positive
            Else
                Return negative
            End If
        End Get
        Set
            If Provider.ParseIonMode(ionMode) = 1 Then
                positive = Value
            Else
                negative = Value
            End If
        End Set
    End Property

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