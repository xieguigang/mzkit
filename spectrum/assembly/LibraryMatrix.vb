Imports Microsoft.VisualBasic.Serialization.JSON

Public Class LibraryMatrix

    Public Property ID As String
    Public Property PrecursorMz As String
    Public Property ProductMz As String
    Public Property LibraryIntensity As String
    Public Property Name As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
