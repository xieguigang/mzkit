Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Public Module Extensions

    ''' <summary>
    ''' Build hmdb metabolite classify table.
    ''' </summary>
    ''' <param name="path$"></param>
    ''' <returns></returns>
    <Extension> Public Function LoadHMDBTaxonomy(path$) As Dictionary(Of String, taxonomy)
        Return metabolite.Load(path) _
            .Where(Function(x) Not x.taxonomy Is Nothing) _
            .Select(Function(metabolite)
                        Return metabolite.secondary_accessions _
                            .accession _
                            .SafeQuery _
                            .JoinIterates({metabolite.accession}) _
                            .Select(Function(id)
                                        Return (id, metabolite.taxonomy)
                                    End Function)
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(x) x.Item1) _
            .ToDictionary(Function(g) g.Key,
                          Function(taxonomy)
                              Return taxonomy.First.Item2
                          End Function)
    End Function
End Module
