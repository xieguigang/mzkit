Imports System.Threading
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
            Dim jsonText = url.GET

            If jsonText.StringEmpty Then
                ' 404 代码之下得到的content text是空字符串
                Return Nothing
            Else
                Dim list As IdentifierList = jsonText _
                    .LoadJSON(Of QueryResponse) _
                   ?.IdentifierList
                Return list
            End If
        End Function

        Public Function QueryPugViews(CAS As String) As Dictionary(Of String, PugViewRecord)
            Dim cache = $"./pubchem_cache/{CAS}.Xml"

            If cache.FileLength > 0 Then
                Return cache _
                    .LoadXml(Of List(Of PugViewRecord)) _
                    .ToDictionary(Function(info) info.RecordNumber)
            Else
                Call Thread.Sleep(1000)
            End If

            Dim list As IdentifierList = QueryCAS(CAS)
            Dim table As New Dictionary(Of String, PugViewRecord)

            If list Is Nothing OrElse list.CID.IsNullOrEmpty Then
                Return New Dictionary(Of String, PugViewRecord)
            Else
                For Each cid As String In list.CID
                    table(cid) = PugView(cid)
                    Call Thread.Sleep(500)
                Next

                Call Thread.Sleep(1000)
            End If

            Call table.Values _
                .ToList _
                .GetXml _
                .SaveTo(cache)

            Return table
        End Function

        Public Function PugView(cid As String) As PugViewRecord
            Dim url As String = sprintf(fetchPugView, cid)
            Dim view As PugViewRecord = url.GET.LoadFromXml(Of PugViewRecord)

            Return view
        End Function
    End Module

    Public Class QueryResponse

        Public Property IdentifierList As IdentifierList
        ''' <summary>
        ''' 当这个属性为空值的时候说明请求成功,反之不为空的时候说明出现了错误
        ''' </summary>
        ''' <returns></returns>
        Public Property Fault As Fault

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class Fault
        Public Property Code As String
        Public Property Message As String
        Public Property Details As String()
    End Class

    Public Class IdentifierList

        Public Property CID As String()

        Public Overrides Function ToString() As String
            Return CID.GetJson
        End Function
    End Class
End Namespace