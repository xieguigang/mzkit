Imports Microsoft.VisualBasic.DataMining.BinaryTree

''' <summary>
''' a term vocabulary mapper based on the <see cref="ClusterTree"/> clustering result.
''' </summary>
Public Class SpectrumVocabulary

    ReadOnly mapping As New Dictionary(Of String, String)
    ReadOnly clusters As New Dictionary(Of String, String())

    Sub New(taxonomy As ClusterTree)
        TreeCluster.GetTree(taxonomy, pull:=clusters)

        For Each cluster_id As String In clusters.Keys
            For Each id As String In clusters(cluster_id)
                mapping(id) = cluster_id
            Next
        Next
    End Sub

    Public Function GetClusters() As Dictionary(Of String, String())
        Return New Dictionary(Of String, String())(clusters)
    End Function

    Public Function ToTerm(id As String) As String
        If mapping.ContainsKey(id) Then
            Return mapping(id)
        Else
            Return "__None__"
        End If
    End Function

    Public Overrides Function ToString() As String
        Return $"{mapping.Count} objects in {clusters.Count} terms"
    End Function

End Class
