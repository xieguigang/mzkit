
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.MoleculeNetworking
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.Math.Distributions

Namespace PoolData

    Public Class DIAInfer

        ReadOnly pool As HttpTreeFs
        ReadOnly da As Tolerance
        ReadOnly ms2diff As Tolerance
        ReadOnly intocutoff As LowAbundanceTrimming

        Sub New(pool As HttpTreeFs,
                Optional ms1diff As String = "da:0.5",
                Optional ms2diff As String = "da:0.3",
                Optional intocutoff As Double = 0.05)

            Me.pool = pool
            Me.da = Tolerance.ParseScript(ms1diff)
            Me.ms2diff = Tolerance.ParseScript(ms2diff)
            Me.intocutoff = New RelativeIntensityCutoff(intocutoff)
        End Sub

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
            Else
                ions_all = ions_all _
                    .Where(Function(a) a.project <> "Reference Annotation") _
                    .ToArray
            End If

            For Each refer As Metadata() In reference.Values
                Dim exact_mass As Double = FormulaScanner.EvaluateExactMass(refer(Scan0).formula)
                Dim adducts_mz As Double() = GetAdducts(refer, exact_mass).ToArray
                Dim selects = ions_all _
                    .Where(Function(i) adducts_mz.Any(Function(a) da(i.mz, a))) _
                    .ToArray

                If selects.IsNullOrEmpty Then
                    Continue For
                End If

                Dim rt_bin = selects.Select(Function(a) a.rt).TabulateBin
                Dim rt As Double = rt_bin.Average

                Yield New PeakMs2 With {
                    .lib_guid = refer(0).biodeep_id,
                    .file = selects _
                        .Select(Function(a) a.source_file.BaseName) _
                        .Distinct _
                        .JoinBy(", "),
                    .scan = refer(0).name,
                    .rt = rt,
                    .mz = exact_mass,
                    .mzInto = GetUnionSpectra(selects).ToArray,
                    .precursor_type = "[M]",
                    .intensity = selects.Length,
                    .meta = New Dictionary(Of String, String) From {
                        {"organism", selects.Select(Function(a) a.organism).Distinct.JoinBy("; ")}
                    }
                }
            Next
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="refer">
        ''' this metadata collection should be reference to the same metabolite
        ''' </param>
        ''' <returns>A set of the precursor m/z</returns>
        Private Iterator Function GetAdducts(refer As Metadata(), exact_mass As Double) As IEnumerable(Of Double)
            Dim polarity = refer _
                .Where(Function(a) Not a.adducts.StringEmpty) _
                .Select(Function(a) Provider.ParseIonMode(a.adducts.Last)) _
                .ToArray

            If Not polarity.All(Function(a) a = polarity(Scan0)) Then
                Return
            End If

            For Each adduct As MzCalculator In Provider.GetCalculator(polarity(Scan0)).Values
                Yield adduct.CalcMZ(exact_mass)
            Next
        End Function

        Private Function GetUnionSpectra(ions As IEnumerable(Of Metadata)) As IEnumerable(Of ms2)
            Dim all_spectrums = ions.Select(Function(a) pool.ReadSpectrum(a)).ToArray
            Dim union = NetworkingNode.UnionRepresentative(all_spectrums, ms2diff, intocutoff)

            Return union.ms2
        End Function

        Private Function GetAllClusterMetadata(cluster_id As String) As IEnumerable(Of Metadata)
            Dim url As String = $"{pool.HttpServices}/get/metadata/"
            Dim gets = HttpRESTMetadataPool.FetchClusterData(url, cluster_id, model_id:=pool.model_id)

            Return gets
        End Function
    End Class
End Namespace