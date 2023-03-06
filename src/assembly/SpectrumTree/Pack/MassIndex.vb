Imports Microsoft.VisualBasic.Serialization.JSON

Public Class MassIndex

    ''' <summary>
    ''' the unique reference of current metabolite spectrum cluster
    ''' </summary>
    ''' <returns></returns>
    Public Property name As String
    Public Property exactMass As Double
    Public Property spectrum As Integer()

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

End Class
