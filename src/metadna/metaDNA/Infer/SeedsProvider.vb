Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports stdnum = System.Math

Namespace Infer

    Public Class SeedsProvider

        ReadOnly unknowns As UnknownSet

        Sub New(unknowns As UnknownSet)
            Me.unknowns = unknowns
        End Sub

        Public Iterator Function CandidateInfers(infer As IEnumerable(Of InferLink)) As IEnumerable(Of CandidateInfer)
            Dim kegg = infer.GroupBy(Function(i) i.kegg.kegg_id).ToArray

            For Each compound As IGrouping(Of String, InferLink) In kegg
                Dim bestOrder = compound _
                    .Select(Function(i)
                                Return (score:=Score(i), i)
                            End Function) _
                    .OrderByDescending(Function(i) i.score) _
                    .ToArray

                Yield New CandidateInfer
            Next
        End Function

        Public Iterator Function Seeding(infers As IEnumerable(Of CandidateInfer)) As IEnumerable(Of AnnotatedSeed)
            For Each compound As CandidateInfer In infers
                Dim products As New Dictionary(Of String, LibraryMatrix)

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
End Namespace