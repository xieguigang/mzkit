Imports Microsoft.VisualBasic.Serialization.JSON
Imports MetabolomeXchange.Json

Public Module Provider

    Const api$ = "http://api.metabolomexchange.org"

    ''' <summary>
    ''' Get all info about a single provider, including a list of the datasets of that provider.
    ''' 
    ''' > ``/provider/{shortname}``
    ''' </summary>
    ''' <param name="provider">The provider shortname, default is using ``metabolights`` repostiory</param>
    ''' <returns></returns>
    Public Function GetAllDataSetJson(Optional provider$ = "mtbls") As String
        Dim url$ = $"{api}/provider/{provider}"
        Dim json$ = url.GET
        Return json
    End Function

    Public Function GetAllDataSet(Optional provider$ = "mtbls") As DataSet()
        Return GetAllDataSetJson(provider).LoadObject(Of response).datasets
    End Function
End Module