Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace NCBI.PubChem

    Public Module Query

        ''' <summary>
        ''' Search pubchem by CAS
        ''' </summary>
        Const queryCAS_URL As String = "https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/name/%s/cids/JSON"

        Public Function QueryCAS(CAS As String) As IdentifierList
            Dim url As String = sprintf(queryCAS_URL, CAS)
            Dim list As IdentifierList = url.GET.LoadJSON(Of QueryResponse)?.IdentifierList

            Return list
        End Function

        Public Function PugView(cid As String)

        End Function
    End Module

    Public Class QueryResponse

        Public Property IdentifierList As IdentifierList

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class IdentifierList

        Public Property CID As String()

        Public Overrides Function ToString() As String
            Return CID.GetJson
        End Function
    End Class
End Namespace