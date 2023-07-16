
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.MoleculeNetworking
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math.Distributions

Namespace PoolData

    Public Class DIAInfer

        ReadOnly pool As HttpTreeFs
        ReadOnly da As Tolerance
        ReadOnly ms2diff As Tolerance
        ReadOnly intocutoff As LowAbundanceTrimming
        ReadOnly ionMode As IonModes = IonModes.Positive

        Sub New(pool As HttpTreeFs,
                Optional ms1diff As String = "da:0.3",
                Optional ms2diff As String = "da:0.3",
                Optional intocutoff As Double = 0.05)

            Me.pool = pool
            Me.da = Tolerance.ParseScript(ms1diff)
            Me.ms2diff = Tolerance.ParseScript(ms2diff)
            Me.intocutoff = New RelativeIntensityCutoff(intocutoff)
        End Sub

        Public Function InferCluster(cluster_id As String, reference As NamedValue(Of String)()) As IEnumerable(Of PeakMs2)
            Dim candidates = reference.GroupBy(Function(i) i.Name).ToArray
            Dim cache As New Dictionary(Of String, PeakMs2)
            Dim ions_all = GetAllClusterMetadata(cluster_id).ToArray

            Return InferCluster(ions_all, candidates,
                                getFormula:=Function(f) f.First.Value,
                                getBiodeepID:=Function(f) f.Key,
                                getName:=Function(f) f.First.Description)
        End Function

        Private Iterator Function InferCluster(Of T)(ions_all As Metadata(),
                                                     reference As IEnumerable(Of T),
                                                     getFormula As Func(Of T, String),
                                                     getBiodeepID As Func(Of T, String),
                                                     getName As Func(Of T, String)) As IEnumerable(Of PeakMs2)

            Dim cache As New Dictionary(Of String, PeakMs2)

            ions_all = ions_all _
                .Where(Function(a) a.project <> "Reference Annotation") _
                .ToArray

            For Each refer As T In reference
                Dim formula As String = getFormula(refer)
                Dim exact_mass As Double = FormulaScanner.EvaluateExactMass(formula)
                Dim adducts_mz As Double() = GetAdducts(exact_mass).ToArray
                Dim selects = ions_all _
                    .Where(Function(i) adducts_mz.Any(Function(a) da(i.mz, a))) _
                    .ToArray

                If selects.IsNullOrEmpty Then
                    Continue For
                End If

                Dim rt_bin = selects.Select(Function(a) a.rt).TabulateBin
                Dim rt As Double = rt_bin.Average

                Yield New PeakMs2 With {
                    .lib_guid = getBiodeepID(refer),
                    .file = selects _
                        .Select(Function(a) a.source_file.BaseName) _
                        .Distinct _
                        .JoinBy(", "),
                    .scan = getName(refer),
                    .rt = rt,
                    .mz = exact_mass,
                    .mzInto = GetUnionSpectra(selects, cache).ToArray,
                    .precursor_type = "[M]",
                    .intensity = selects.Length,
                    .meta = New Dictionary(Of String, String) From {
                        {"organism", selects.Select(Function(a) a.organism).Distinct.JoinBy("; ")}
                    }
                }
            Next
        End Function

        Public Function InferCluster(cluster_id As String) As IEnumerable(Of PeakMs2)
            Dim ions_all As Metadata() = GetAllClusterMetadata(cluster_id).ToArray
            Dim reference = ions_all _
                .Where(Function(a) a.project = "Reference Annotation") _
                .GroupBy(Function(a) a.biodeep_id) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.ToArray
                              End Function)
            Dim cache As New Dictionary(Of String, PeakMs2)

            If reference.IsNullOrEmpty Then
                Return {}
            Else
                Return InferCluster(ions_all, reference,
                                    getFormula:=Function(f) f.Value(Scan0).formula,
                                    getBiodeepID:=Function(f) f.Key,
                                    getName:=Function(f) f.Value(Scan0).name)
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns>A set of the precursor m/z</returns>
        Private Iterator Function GetAdducts(exact_mass As Double) As IEnumerable(Of Double)
            For Each adduct As MzCalculator In Provider.GetCalculator(ionMode).Values
                Yield adduct.CalcMZ(exact_mass)
            Next
        End Function

        Private Function GetUnionSpectra(ions As IEnumerable(Of Metadata), cache As Dictionary(Of String, PeakMs2)) As IEnumerable(Of ms2)
            Dim all_spectrums As PeakMs2() = ions _
                .Select(Function(a)
                            Dim key As String = a.block.position

                            If Not cache.ContainsKey(key) Then
                                Call cache.Add(key, pool.ReadSpectrum(a))
                            End If

                            Return cache(key)
                        End Function) _
                .ToArray
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