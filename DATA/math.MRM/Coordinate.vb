Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Models

    Public Class Coordinate

        Public Property HMDB As String
        Public Property Name As String
        Public Property C As Dictionary(Of String, Double)
        Public Property ISTD As String
        ''' <summary>
        ''' 内标的编号，需要使用这个编号来分别找到离子对和浓度信息
        ''' </summary>
        ''' <returns></returns>
        Public Property [IS] As String

        Public Overrides Function ToString() As String
            Return $"[{[IS]}] {HMDB}: {Name}, {C.GetJson}"
        End Function
    End Class
End Namespace