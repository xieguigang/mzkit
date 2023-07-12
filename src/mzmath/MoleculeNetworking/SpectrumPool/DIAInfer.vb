
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.MoleculeNetworking
Imports Microsoft.VisualBasic.Math.Distributions

Namespace PoolData

    Public Class DIAInfer

        ReadOnly pool As HttpTreeFs
        ReadOnly da As Tolerance
        ReadOnly ms2diff As Tolerance
        ReadOnly intocutoff As LowAbundanceTrimming

        Public Iterator Function InferCluster(cluster_id As String) As IEnumerable(Of PeakMs2)
            Dim ions_all = GetAllClusterMetadata(cluster_id).ToArray
            Dim reference = ions_all _
                .Where(Function(a) a.project = "Reference Annotation") _
                .GroupBy(Function(a) a.biodeep_id) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.ToArray
                              End Function)

            If reference.IsNullOrEmpty Then
                Return
            End If

            For Each refer As Metadata() In reference.Values
                Dim selects = ions_all _
                    .Where(Function(i) refer.Any(Function(a) da(i.mz, a.mz))) _
                    .ToArray

                If selects.IsNullOrEmpty Then
                    Continue For
                End If

                Dim rt_bin = selects.Select(Function(a) a.rt).TabulateBin
                Dim rt As Double = rt_bin.Average

                Yield New PeakMs2 With {
                    .lib_guid = refer(0).biodeep_id,
                    .file = selects.Select(Function(a) a.source_file).Distinct.JoinBy(", "),
                    .scan = "NA",
                    .rt = rt,
                    .mz = 0,
                    .mzInto = GetUnionSpectra(selects).ToArray
                }
            Next
        End Function

        Private Function GetUnionSpectra(ions As IEnumerable(Of Metadata)) As IEnumerable(Of ms2)
            Dim all_spectrums = ions.Select(Function(a) pool.ReadSpectrum(a)).ToArray
            Dim union = NetworkingNode.UnionRepresentative(all_spectrums, ms2diff, intocutoff)

            Return union.ms2
        End Function

        Private Iterator Function GetAllClusterMetadata(cluster_id As String) As IEnumerable(Of Metadata)

        End Function
    End Class
End Namespace