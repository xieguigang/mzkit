Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace NCBI.PubChem

    Public Class CIDExport : Inherits WebQuery(Of String)

        Const queryCAS_Api As String = "https://pubchem.ncbi.nlm.nih.gov/sdq/sdqagent.cgi?infmt=json&outfmt=jsonp"

        Public Sub New(<CallerMemberName> Optional cache As String = Nothing, Optional interval As Integer = -1)
            MyBase.New(AddressOf queryApi, AddressOf normalizeFileName, AddressOf parseExportTable, , cache, interval)
        End Sub

        Private Shared Function parseExportTable(text As String, schema As Type) As Object
            Return text _
                .LineTokens _
                .AsDataSource(Of QueryTableExport) _
                .ToArray
        End Function

        Private Shared Function queryApi(CAS As String) As String
            Dim query As New JsonQuery With {
                .where = New QueryWhere With {
                    .ands = {New Dictionary(Of String, String) From {
                        {"*", CAS}
                    }}
                }
            }
            Dim json$ = query.GetJson.UrlEncode
            Dim url$ = $"{queryCAS_Api}&query={json}"

            Return url
        End Function

        Private Shared Function normalizeFileName(text As String) As String
            Return text.NormalizePathString(False)
        End Function
    End Class
End Namespace