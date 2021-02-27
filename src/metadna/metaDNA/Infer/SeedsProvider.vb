Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports stdnum = System.Math

Namespace Infer

    Public Class SeedsProvider

        ReadOnly unknowns As UnknownSet
        ReadOnly precursorTypes As MzCalculator()
        ReadOnly kegg As KEGGHandler
        ReadOnly da3 As Tolerance = Tolerance.DeltaMass(0.3)

        Sub New(unknowns As UnknownSet, ranges As MzCalculator(), kegg As KEGGHandler)
            Me.unknowns = unknowns
            Me.precursorTypes = ranges
            Me.kegg = kegg
        End Sub

        Public Iterator Function CandidateInfers(infer As IEnumerable(Of InferLink)) As IEnumerable(Of CandidateInfer)
            Dim kegg = infer.GroupBy(Function(i) i.kegg.kegg_id).ToArray

            For Each compound As IGrouping(Of String, InferLink) In kegg
                Yield New CandidateInfer With {
                    .kegg_id = compound.Key,
                    .infers = compound _
                        .DoCall(AddressOf MzGroupCandidates) _
                        .ToArray
                }
            Next
        End Function

        Private Iterator Function MzGroupCandidates(candidates As IEnumerable(Of InferLink)) As IEnumerable(Of Candidate)
            Dim all As Dictionary(Of String, InferLink()) = candidates _
                .GroupBy(Function(c) c.query.id) _
                .ToDictionary(Function(c) c.Key,
                              Function(c)
                                  Return c.ToArray
                              End Function)
            Dim kegg_id As String = all.Values.First()(Scan0).kegg.kegg_id
            Dim kegg As Compound = Me.kegg.GetCompound(kegg_id)
            Dim tree As New SpectrumTreeCluster(showReport:=False)

            Call all _
                .Select(Function(i) unknowns.QueryByKey(i.Key)) _
                .ToArray _
                .DoCall(AddressOf tree.doCluster)

            Dim best As SpectrumCluster = tree.Best(Function(c) c.GetClusterVector(da3).Sum)

            candidates = best.cluster _
                .Select(Function(peak) all(peak.lib_guid)) _
                .IteratesALL _
                .ToArray

            For Each type As MzCalculator In precursorTypes
                Dim mz As Double = type.CalcMZ(kegg.exactMass)
                Dim typeName As String = type.ToString
                Dim group As Candidate() = candidates _
                    .Where(Function(q) da3(q.query.mz, mz)) _
                    .Select(Function(q) Score(q, typeName, mz)) _
                    .OrderBy(Function(q) q.pvalue) _
                    .ToArray

                Yield group(Scan0)
            Next
        End Function

        Private Function Score(infer As InferLink, type As String, mz As Double) As Candidate
            Dim scoreVal As Double
            Dim pvalue As Double

            If infer.level = 1 Then
                scoreVal = 0.1
                pvalue = 0.5
            Else
                scoreVal = stdnum.Min(infer.forward, infer.reverse)
                pvalue =
            End If

            Return New Candidate With {
                .infer = infer,
                .precursorType = type,
                .ppm = PPMmethod.PPM(mz, infer.query.mz),
                .score = scoreVal,
                .pvalue = pvalue
            }
        End Function

        Public Iterator Function Seeding(infers As IEnumerable(Of CandidateInfer)) As IEnumerable(Of AnnotatedSeed)
            For Each compound As CandidateInfer In infers
                Dim products As New Dictionary(Of String, LibraryMatrix)
                Dim pid As String
                Dim best As Candidate = compound.infers.OrderByDescending(Function(a) a.score).First

                For Each candidate As Candidate In compound.infers
                    pid = candidate.infer.query.id
                    products(pid) = New LibraryMatrix With {
                        .name = pid,
                        .ms2 = unknowns.QueryByKey(pid).mzInto
                    }
                Next

                Yield New AnnotatedSeed With {
                    .id = best.infer.query.id,
                    .kegg_id = compound.kegg_id,
                    .parent = New ms1_scan With {
                        .mz = best.infer.query.mz,
                        .scan_time = best.infer.query.rt,
                        .intensity = 0
                    },
                    .products = products
                }
            Next
        End Function
    End Class
End Namespace