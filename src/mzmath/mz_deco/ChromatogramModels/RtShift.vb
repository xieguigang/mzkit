
''' <summary>
''' the rt shift result
''' </summary>
Public Class RtShift

    ''' <summary>
    ''' the sample name
    ''' </summary>
    ''' <returns></returns>
    Public Property sample As String
    Public Property xcms_id As String
    ''' <summary>
    ''' the reference rt
    ''' </summary>
    ''' <returns></returns>
    Public Property refer_rt As Double
    Public Property sample_rt As Double
    Public Property RI As Double

    Public ReadOnly Property shift As Double
        Get
            Return sample_rt - refer_rt
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

End Class