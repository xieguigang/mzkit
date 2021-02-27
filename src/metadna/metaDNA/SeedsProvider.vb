Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports stdnum = System.Math

Public Class SeedsProvider

    ReadOnly unknowns As UnknownSet

    Sub New(unknowns As UnknownSet)
        Me.unknowns = unknowns
    End Sub

    Public Iterator Function Seeding(infer As IEnumerable(Of InferLink)) As IEnumerable(Of AnnotatedSeed)
        Dim kegg = infer.GroupBy(Function(i) i.kegg.kegg_id).ToArray

        For Each compound As IGrouping(Of String, InferLink) In kegg
            Dim bestOrder = compound _
                .Select(Function(i)
                            Return (score:=Score(i), i)
                        End Function) _
                .OrderByDescending(Function(i) i.score) _
                .ToArray
            Dim best = bestOrder.First
            Dim bestLib As New LibraryMatrix With {
                .ms2 = unknowns.QueryByKey(best.Item2.query.id).mzInto,
                .name = best.Item2.query.id
            }

            Yield New AnnotatedSeed With {
                .id = best.Item2.query.id,
                .kegg_id = compound.Key,
                .parent = New ms1_scan With {
                    .mz = best.Item2.query.mz,
                    .scan_time = best.Item2.query.rt,
                    .intensity = 0
                },
                .products = New Dictionary(Of String, LibraryMatrix) From {
                    {best.Item2.query.id, bestLib}
                }
            }
        Next
    End Function

    Public Function Score(infer As InferLink) As Double
        Return stdnum.Min(infer.forward, infer.reverse)
    End Function
End Class
