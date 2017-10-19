Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Json

    Public Structure DataSet

        Public Property description As String()
        Public Property accession As String
        Public Property url As String
        Public Property title As String
        Public Property [date] As String
        Public Property timestamp As String
        Public Property submitter As String()
        Public Property publications As publication()
        Public Property meta As meta

        Public Overrides Function ToString() As String
            Return title
        End Function
    End Structure

    Public Structure meta

        Public Property analysis As String
        Public Property platform As String
        Public Property organism As Object
        Public Property organism_parts As Object
        Public Property metabolites As String()

        Public Overrides Function ToString() As String
            Return metabolites.GetJson
        End Function
    End Structure

    Public Structure publication

        Public Property title As String
        Public Property doi As String
        Public Property pubmed As String

        Public Overrides Function ToString() As String
            Return title
        End Function
    End Structure

    Public Structure response

        Public Property name As String
        Public Property url As String
        Public Property description As String
        Public Property datasets As DataSet()

        Public Overrides Function ToString() As String
            Return url
        End Function
    End Structure
End Namespace