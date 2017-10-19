Imports Microsoft.VisualBasic.Data.csv.IO

Public Class DataTable : Inherits EntityObject

    Public Property description As String()
    Public Property url As String
    Public Property title As String
    Public Property [date] As Date
    Public Property submitter As String()
    Public Property publications As String()
    Public Property analysis As String
    Public Property platform As String
    Public Property organism As String()
    Public Property organism_parts As String()
    Public Property metabolites As String()

    Public Overrides Function ToString() As String
        Return title
    End Function
End Class