Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Public Module NetworkHandler

    <Extension>
    Public Function ExportNetwork(table As IEnumerable(Of MetaDNAResult)) As NetworkGraph
        Dim g As New NetworkGraph
        Dim edgeData As EdgeData
        Dim names As New Dictionary(Of String, String)

        For Each inferLink As MetaDNAResult In table
            If g.GetElementByID(inferLink.KEGGId) Is Nothing Then
                g.CreateNode(inferLink.KEGGId)
            End If
            If g.GetElementByID(inferLink.partnerKEGGId) Is Nothing Then
                g.CreateNode(inferLink.partnerKEGGId)
            End If

            edgeData = New EdgeData With {
                .Properties = New Dictionary(Of String, String) From {
                    {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, inferLink.inferLevel}
                }
            }
            g.CreateEdge(inferLink.partnerKEGGId, inferLink.KEGGId, inferLink.parentTrace, edgeData)
            names(inferLink.KEGGId) = inferLink.name
        Next

        For Each vertex In g.vertex
            vertex.data.label = names.TryGetValue(vertex.label, [default]:=vertex.label)
        Next

        Return g
    End Function
End Module
