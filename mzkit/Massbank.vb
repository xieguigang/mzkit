Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports ChEBIRepo = SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.DATA

<Package("mzkit.massbank")>
Module Massbank

    <ExportAPI("chebi.secondary2main.mapping")>
    Public Function chebiSecondary2Main(repository As String) As Dictionary(Of String, String)
        Return ChEBIRepo.ScanEntities(repository).GroupBy(Function(c) c.chebiId).Select(Function(c) c.First).Select(Function(c)
                                                                                                                        If c.SecondaryChEBIIds.IsNullOrEmpty Then
                                                                                                                            Return {(c.chebiId, c.chebiId)}
                                                                                                                        Else
                                                                                                                            Return c.SecondaryChEBIIds.Select(Function(sid) (sid, c.chebiId))
                                                                                                                        End If
                                                                                                                    End Function).IteratesALL.GroupBy(Function(c) c.Item1).ToDictionary(Function(c) c.Key, Function(g) g.Select(Function(t) t.Item2).First)
    End Function
End Module
