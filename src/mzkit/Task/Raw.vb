Public Class Raw

    Public Property source As String
    Public Property cache As String
    Public ReadOnly Property numOfScans As Integer
        Get
            Return scans.Length
        End Get
    End Property

    Public Property scans As ScanEntry()

End Class

Public Class ScanEntry

    Public Property id As String
    Public Property mz As Double

End Class