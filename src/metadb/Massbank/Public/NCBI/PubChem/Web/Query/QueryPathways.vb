Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Net.Http

Namespace NCBI.PubChem

    ' {%22download%22:%22*%22,%22collection%22:%22lotus%22,%22where%22:{%22ands%22:[{%22cid%22:%22128861%22}]},%22order%22:[%22relevancescore,desc%22],%22start%22:1,%22limit%22:10000000,%22downloadfilename%22:%22CID_128861_lotus%22}

    Public Class QueryPathways : Inherits WebQuery(Of NamedValue(Of Types))

        Public Sub New(<CallerMemberName> Optional cache As String = Nothing, Optional interval As Integer = -1, Optional offline As Boolean = False)
            MyBase.New(
                url:=AddressOf url,
                contextGuid:=Function(str) $"{str.Value.ToString}_{str.Name}",
                parser:=Function(str, type) str,
                prefix:=Function(str) str.Last,
                cache:=cache,
                interval:=interval,
                offline:=offline
            )
        End Sub

        Const queryOriginJSON As String = "{""download"":""*"",""collection"":""lotus"",""where"":{""ands"":[{""cid"":""%cid""}]},""order"":[""relevancescore,desc""],""start"":1,""limit"":10000000,""downloadfilename"":""cID_%cid_lotus""}"
        Const queryPathwayJSON As String = "{""download"":""*"",""collection"":""pathway"",""where"":{""ands"":[{""cid"":""%cid""},{""core"":""1""}]},""order"":[""taxname,asc""],""start"":1,""limit"":10000000,""downloadfilename"":""cID_%cid_pathway""}"
        Const queryReactionJSON As String = "{""download"":""*"",""collection"":""pathwayreaction"",""where"":{""ands"":[{""cid"":""%cid""}]},""order"":[""relevancescore,desc""],""start"":1,""limit"":10000000,""downloadfilename"":""cID_%cid_pathwayreaction""}"

        Private Shared Function url(cid As NamedValue(Of Types)) As String
            Dim template As String

            Select Case cid.Value
                Case Types.pathways : template = queryPathwayJSON
                Case Types.reaction : template = queryReactionJSON
                Case Types.taxonomy : template = queryOriginJSON
                Case Else
                    Throw New NotImplementedException(cid.ToString)
            End Select

            Dim query As String = template.Replace("%cid", cid.Name)
            Dim urlArg As String = query.UrlEncode

            Return $"https://pubchem.ncbi.nlm.nih.gov/sdq/sdqagent.cgi?infmt=json&outfmt=json&query={urlArg}"
        End Function
    End Class

    Public Enum Types
        pathways
        taxonomy
        reaction
    End Enum
End Namespace