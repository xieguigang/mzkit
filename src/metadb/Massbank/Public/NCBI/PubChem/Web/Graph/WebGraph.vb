Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace NCBI.PubChem.Graph

    Public Enum Types
        ChemicalGeneSymbolNeighbor
        ChemicalDiseaseNeighbor
        ChemicalNeighbor
    End Enum

    Public Class LinkDataSet

        Public Property LinkData As MeshGraph()

    End Class

    Public Class WebGraph : Inherits WebQuery(Of (cid As String, type As Types))

        Public Sub New(<CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False
        )
            MyBase.New(
                url:=AddressOf getJSONUrl,
                contextGuid:=Function(q) $"{q.type}_{q.cid}",
                parser:=AddressOf parseJSON,
                prefix:=Function(q) q.Split("_"c).First & "/" & MD5(q).Substring(1, 2),
                cache:=cache,
                interval:=interval,
                offline:=offline
            )
        End Sub

        Private Shared Function parseJSON(data As String, schema As Type) As Object
            Return data.LoadJSON(Of GraphJSON)
        End Function

        Private Shared Function getJSONUrl(q As (cid As String, type As Types)) As String
            Return $"https://pubchem.ncbi.nlm.nih.gov/link_db/link_db_server.cgi?format=JSON&type={q.type.Description}&operation=GetAllLinks&id_1={q.cid}&response_type=display"
        End Function

        Public Overloads Shared Function Query(cid As String,
                                               Optional type As Types = Types.ChemicalDiseaseNeighbor,
                                               Optional cache As String = "./graph",
                                               Optional interval As Integer = -1,
                                               Optional offline As Boolean = False) As MeshGraph()

            Static web As New Dictionary(Of String, WebGraph)

            Dim json As GraphJSON = web.ComputeIfAbsent(
                key:=cache,
                lazyValue:=Function() New WebGraph(cache, interval, offline)
            ) _
                .Query(Of GraphJSON)(
                    context:=(cid, type),
                    cacheType:=".json"
                )

            If json Is Nothing OrElse json.LinkDataSet Is Nothing OrElse json.LinkDataSet.LinkData Is Nothing Then
                Return Nothing
            Else
                Return json.LinkDataSet.LinkData
            End If
        End Function
    End Class

    Public Class GraphJSON

        Public Property LinkDataSet As LinkDataSet

    End Class
End Namespace