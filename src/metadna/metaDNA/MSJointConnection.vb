Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Public Class MSJointConnection

    ReadOnly kegg As KEGGHandler
    ReadOnly jointSet As Background

    Sub New(kegg As KEGGHandler, peakSet As Background)
        Me.kegg = kegg
        Me.jointSet = peakSet
    End Sub

    ''' <summary>
    ''' MS1 peak list annotation
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="topN"></param>
    ''' <returns></returns>
    Public Function SetAnnotation(mz As Double(), Optional topN As Integer = 3) As KEGGQuery()
        Dim allId As Dictionary(Of String, KEGGQuery()) = mz _
            .Select(AddressOf kegg.QueryByMz) _
            .IteratesALL _
            .GroupBy(Function(cid) cid.kegg_id) _
            .ToDictionary(Function(cid) cid.Key,
                          Function(cid)
                              Return cid.ToArray
                          End Function)
        Dim enrichment As EnrichmentResult() = jointSet _
            .Enrichment(allId.Keys, showProgress:=False) _
            .OrderBy(Function(d) d.pvalue) _
            .Take(topN) _
            .ToArray
        Dim mzSet = enrichment _
            .Select(Function(list)
                        Dim score As Double = -Math.Log10(list.pvalue)
                        Dim result = list.geneIDs _
                            .Select(Function(id) allId(id)) _
                            .IteratesALL _
                            .ToArray
                        Dim copy = result _
                            .Select(Function(q)
                                        Return New KEGGQuery With {
                                            .score = score,
                                            .precursorType = q.precursorType,
                                            .kegg_id = q.kegg_id,
                                            .mz = q.mz,
                                            .ppm = q.ppm
                                        }
                                    End Function) _
                            .ToArray

                        Return copy
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(c)
                         Return c.mz.ToString("F4")
                     End Function) _
            .ToArray
        Dim annotation = mzSet _
            .Select(Function(mzi)
                        Return mzi _
                            .GroupBy(Function(i) i.kegg_id) _
                            .Select(Function(m)
                                        Return New KEGGQuery With {
                                            .kegg_id = m.Key,
                                            .mz = Double.Parse(mzi.Key),
                                            .ppm = m.First.ppm,
                                            .precursorType = m.First.precursorType,
                                            .score = Aggregate hit In m Into Sum(hit.score)
                                        }
                                    End Function) _
                            .OrderByDescending(Function(d) d.score) _
                            .First
                    End Function) _
            .ToArray
        Dim unique = annotation _
            .GroupBy(Function(a) a.kegg_id) _
            .Select(Function(cid)
                        Return cid.OrderByDescending(Function(d) d.score).First
                    End Function) _
            .ToArray

        Return unique
    End Function

    Public Shared Function ImportsBackground(maps As IEnumerable(Of Map)) As Background
        Return New Background With {
           .clusters = toClusters(maps).ToArray,
           .size = .clusters _
              .Select(Function(c) c.members) _
              .IteratesALL _
              .Select(Function(c) c.accessionID) _
              .Distinct _
              .Count
        }
    End Function

    Private Shared Iterator Function toClusters(maps As IEnumerable(Of Map)) As IEnumerable(Of Cluster)
        For Each map As Map In maps
            Yield New Cluster With {
                .description = map.description,
                .ID = map.id,
                .names = map.Name,
                .members = map _
                    .GetMembers _
                    .Where(Function(id)
                               Return id.IsPattern("C\d+")
                           End Function) _
                    .Distinct _
                    .Select(Function(id)
                                Return New BackgroundGene With {
                                    .accessionID = id,
                                    .[alias] = {id},
                                    .locus_tag = New NamedValue With {
                                        .name = id,
                                        .text = id
                                    },
                                    .name = id,
                                    .term_id = {id}
                                }
                            End Function) _
                    .ToArray
            }
        Next
    End Function


End Class
