Imports Microsoft.VisualBasic.Serialization.JSON
Imports MetabolomeXchange.Json
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

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
        Return GetAllDataSetJson(provider) _
            .LoadObject(Of response) _
            .datasets _
            .Values _
            .ToArray
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function ToTable(data As IEnumerable(Of DataSet)) As DataTable()
        Return data.Select(AddressOf ToTable).ToArray
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function ToTable(data As DataSet) As DataTable
        Return New DataTable With {
            .analysis = data.meta.analysis,
            .date = Date.Parse(data.date),           ' .description = data.description,
            .ID = data.accession,            ' .metabolites = data.meta.metabolites,
            .organism = ensureStringArray(data.meta.organism),
            .organism_parts = ensureStringArray(data.meta.organism_parts),
            .platform = data.meta.platform,
            .publications = data.publications _
                .SafeQuery _
                .Select(Function(pub) pub.ToString) _
                .ToArray,
            .submitter = data.submitter,
            .title = data.title            ' .url = data.url
        }
    End Function

    Private Function ensureStringArray(data) As String()
        Return If(TypeOf data Is String, {CStr(data)}, DirectCast(data, IEnumerable(Of String)).ToArray)
    End Function
End Module