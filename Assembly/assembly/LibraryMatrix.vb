Imports Microsoft.VisualBasic.Serialization.JSON

Public Class Library

    Public Property ID As String
    Public Property PrecursorMz As String
    Public Property ProductMz As String
    Public Property LibraryIntensity As String
    Public Property Name As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class

Public Class ms2
    Public Property mz As Double
    Public Property intensity As Double
End Class

Public Class LibraryMatrix
    Public Property ms2 As ms2()
End Class