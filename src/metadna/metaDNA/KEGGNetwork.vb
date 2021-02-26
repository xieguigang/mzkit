Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Public Class KEGGNetwork

    Dim kegg_id As Dictionary(Of String, String())

    Private Sub New()
    End Sub

    Public Function FindPartners(kegg_id As String) As IEnumerable(Of String)
        If Me.kegg_id.ContainsKey(kegg_id) Then
            Return Me.kegg_id(kegg_id)
        Else
            Return {}
        End If
    End Function

    Public Shared Function CreateNetwork(network As IEnumerable(Of ReactionClass)) As KEGGNetwork
        Dim index As New Dictionary(Of String, List(Of String))

        For Each reaction As ReactionClass In network
            For Each link As ReactionCompoundTransform In reaction.reactantPairs
                If Not index.ContainsKey(link.from) Then
                    index(link.from) = New List(Of String)
                End If
                If Not index.ContainsKey(link.to) Then
                    index(link.to) = New List(Of String)
                End If

                index(link.from).Add(link.to)
                index(link.to).Add(link.from)
            Next
        Next

        Return New KEGGNetwork With {
            .kegg_id = index _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.Value.Distinct.ToArray
                              End Function)
        }
    End Function

End Class
