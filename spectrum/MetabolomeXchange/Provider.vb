Public Module Provider

 const api$ = "http://api.metabolomexchange.org"

Public Function GetAllDataSet(provider As String)
Dim url$ = $"{api}/provider/{provider}"
Dim json$ = url.Get
Return json.LoadObject(Of )
End Function

End Module