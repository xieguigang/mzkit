Namespace Models

    ''' <summary>
    ''' 内标
    ''' </summary>
    Public Class [IS]

        Public Property ID As String
        Public Property name As String
        ''' <summary>
        ''' 内标的浓度
        ''' </summary>
        ''' <returns></returns>
        Public Property CIS As Double

        Public Overrides Function ToString() As String
            Return $"Dim {name} As {ID} = {CIS}"
        End Function
    End Class
End Namespace