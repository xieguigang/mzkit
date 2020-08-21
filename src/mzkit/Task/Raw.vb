Public Class Raw

    Public Property source As String
    Public Property cache As String
    Public ReadOnly Property scans As Integer
        Get
            Return scanIDList.Length
        End Get
    End Property

    Public Property scanIDList As String()

End Class
