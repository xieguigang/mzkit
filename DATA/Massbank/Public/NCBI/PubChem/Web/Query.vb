Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace NCBI.PubChem

    Public Module Query

        ''' <summary>
        ''' Search pubchem by CAS
        ''' </summary>
        Const queryCAS_URL As String = "https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/name/%s/cids/JSON"
        Const fetchPugView As String = "https://pubchem.ncbi.nlm.nih.gov/rest/pug_view/data/compound/%s/XML/"

        Public Function QueryCAS(CAS As String) As IdentifierList
            Dim url As String = sprintf(queryCAS_URL, CAS)
            Dim list As IdentifierList = url.GET.LoadJSON(Of QueryResponse)?.IdentifierList

            Return list
        End Function

        Public Function QueryPugViews(CAS As String) As Dictionary(Of String, PugView)
            Dim list As IdentifierList = QueryCAS(CAS)
            Dim table As New Dictionary(Of String, PugView)

            For Each cid As String In list.CID
                table(cid) = PugView(cid)
            Next

            Return table
        End Function

        Public Function PugView(cid As String) As PugView
            Dim url As String = sprintf(fetchPugView, cid)
            Dim view As PugView = url.GET.LoadXml(Of PugView)

            Return view
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