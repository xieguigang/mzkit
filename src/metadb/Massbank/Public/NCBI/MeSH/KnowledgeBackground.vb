﻿Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS.GSEA

Namespace NCBI.MeSH

    Public Module KnowledgeBackground

        <Extension>
        Private Function MeshTree(mesh As Tree(Of Term)) As Background
            Return mesh _
                .ImportsTree(Function(term)
                                 Return New BackgroundGene With {
                                    .accessionID = term.term,
                                    .[alias] = {term.term},
                                    .locus_tag = New NamedValue With {
                                        .name = term.term,
                                        .text = term.term
                                    },
                                    .name = term.term,
                                    .term_id = {term.term}
                                 }
                             End Function)
        End Function

        <Extension>
        Private Function clusterJoin(c As IGrouping(Of String, Cluster)) As Cluster
            If c.Count = 1 Then
                Return c.First
            Else
                Return New Cluster With {
                    .description = c.Select(Function(i) i.description).Distinct.JoinBy("; "),
                    .ID = c.Key,
                    .names = c.Select(Function(i) i.names).Distinct.JoinBy("; "),
                    .members = c.Select(Function(i) i.members) _
                        .IteratesALL _
                        .GroupBy(Function(g) g.accessionID) _
                        .Select(Function(a) a.First) _
                        .ToArray
                }
            End If
        End Function

        <Extension>
        Public Function MeshTermOntologyBackground(mesh As Tree(Of Term)) As Background
            Dim bg = mesh.MeshTree

            bg = New Background With {
                .build = Now,
                .clusters = bg.clusters _
                    .GroupBy(Function(c) c.ID) _
                    .Select(Function(c)
                                Return c.clusterJoin
                            End Function) _
                    .ToArray
            }

            Return bg
        End Function

        <Extension>
        Public Function MeshTopicBackground(mesh As Tree(Of Term), cluster As Background) As Background
            Dim tree = mesh.MeshTermOntologyBackground
            Dim list = cluster.clusters.ToDictionary(Function(c) c.ID)

            Return New Background With {
                .build = tree.build,
                .comments = tree.comments,
                .id = tree.id,
                .name = tree.name,
                .clusters = tree.clusters _
                    .Where(Function(v) list.ContainsKey(v.ID)) _
                    .Select(Function(cl)
                                Return joinClusterTopic(cl, list)
                            End Function) _
                    .Where(Function(cl) Not cl Is Nothing) _
                    .ToArray
            }
        End Function

        Private Function joinClusterTopic(node As Cluster, list As Dictionary(Of String, Cluster)) As Cluster
            Dim topics = {node.ID} _
                .JoinIterates(node.members.Select(Function(a) a.accessionID)) _
                .Distinct _
                .Where(Function(v) list.ContainsKey(v)) _
                .Select(Function(id) list(id)) _
                .ToArray
            Dim data As BackgroundGene() = topics _
                .Select(Function(c) c.members) _
                .IteratesALL _
                .ToArray
            Dim metainfo = list(node.ID)

            If data.Length = 0 Then
                Return Nothing
            End If

            Return New Cluster With {
                .description = metainfo.description,
                .ID = metainfo.names,
                .members = data,
                .names = metainfo.ID
            }
        End Function
    End Module
End Namespace