Imports Microsoft.VisualBasic.Serialization.JSON

Public Module Provider

    Const api$ = "http://api.metabolomexchange.org"

    Public Function GetAllDataSet(provider As String)
        Dim url$ = $"{api}/provider/{provider}"
        Dim json$ = url.GET
        Return json.LoadObject(Of DataSet)
    End Function

End Module