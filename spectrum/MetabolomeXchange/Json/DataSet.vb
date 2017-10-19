Imports System.Runtime.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Json

    Public Class DataSet

        <DataMember(Name:="@context")>
        Public Property context As String
        Public Property description As String()
        Public Property accession As String
        Public Property url As String
        Public Property title As String
        Public Property [date] As String
        Public Property timestamp As Long
        Public Property submitter As String()
        Public Property publications As publication()
        Public Property meta As meta

        Public Overrides Function ToString() As String
            Return title
        End Function
    End Class

    Public Class meta

        Public Property analysis As String
        Public Property platform As String
        Public Property organism As String()
        Public Property organism_parts As String()
        Public Property metabolites As String()

        Public Overrides Function ToString() As String
            Return metabolites.GetJson
        End Function
    End Class

    Public Class publication

        Public Property title As String
        Public Property doi As String
        Public Property pubmed As String

        Public Overrides Function ToString() As String
            If doi.StringEmpty And pubmed.StringEmpty Then
                Return title
            ElseIf doi.StringEmpty Then
                Return $"{title} ({pubmed})"
            ElseIf pubmed.StringEmpty Then
                Return $"{title} (doi:{doi})"
            Else
                Return $"{title} (doi:{doi}, {pubmed})"
            End If
        End Function
    End Class

    Public Class response

        Public Property name As String
        Public Property url As String
        Public Property description As String
        Public Property datasets As Dictionary(Of String, DataSet)

        Public Overrides Function ToString() As String
            Return url
        End Function
    End Class
End Namespace