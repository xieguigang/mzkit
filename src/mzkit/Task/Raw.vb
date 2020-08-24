Public Class Raw

    Public Property source As String
    Public Property cache As String

    Public Property rtmin As Double
    Public Property rtmax As Double

    Public ReadOnly Property numOfScans As Integer
        Get
            If scans.IsNullOrEmpty Then
                Return 0
            Else
                Return scans.Length
            End If
        End Get
    End Property

    Public Property scans As ScanEntry()

End Class

Public Class ScanEntry

    Public Property id As String
    Public Property mz As Double
    Public Property rt As Double
    Public Property intensity As Double

    Public Overrides Function ToString() As String
        Return id
    End Function
End Class