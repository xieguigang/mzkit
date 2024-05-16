#Region "Microsoft.VisualBasic::b046d25acdd424f4d1e2d6e6f04e9849, mzmath\MoleculeNetworking\SpectrumPool\DIAInfer.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 236
    '    Code Lines: 154
    ' Comment Lines: 56
    '   Blank Lines: 26
    '     File Size: 10.77 KB


    '     Class DIAInfer
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetAdducts, GetAllClusterMetadata, GetUnionSpectra, (+3 Overloads) InferCluster
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.MoleculeNetworking
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math.Distributions

Namespace PoolData

    ''' <summary>
    ''' A helper module for run DIA infer of the unknow spectrum in the cluster
    ''' </summary>
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

            Me.ms2diff = Tolerance.ParseScript(ms2diff)
            Me.pool = pool
            Me.pool.SetScore(0.3, intocutoff)
            Me.da = Tolerance.ParseScript(ms1diff)
            Me.intocutoff = New RelativeIntensityCutoff(intocutoff)
        End Sub

        ''' <summary>
        ''' Infer the cluster spectrum result based on the alignment result
        ''' </summary>
        ''' <param name="cluster_id">An integer cluster id in the biodeep database</param>
        ''' <param name="reference">
        ''' the alignment candidates
        ''' </param>
        ''' <returns></returns>
        Public Iterator Function InferCluster(cluster_id As String,
                                              reference As NamedValue(Of String)(),
                                              Optional push_cluster As Boolean = True) As IEnumerable(Of PeakMs2)

            Dim candidates = reference.GroupBy(Function(i) i.Name).ToArray
            Dim cache As New Dictionary(Of String, PeakMs2)
            Dim ions_all = GetAllClusterMetadata(cluster_id).ToArray
            Dim result As IEnumerable(Of PeakMs2) = InferCluster(
                ions_all, candidates,
                getFormula:=Function(f) f.First.Value,
                getBiodeepID:=Function(f) f.Key,
                getName:=Function(f)
                             Return f.First.Description
                         End Function)

            If push_cluster Then
                Dim pool As MetadataProxy = Me.pool.LoadMetadata(id:=Integer.Parse(cluster_id))
                Dim root = Me.pool.ReadSpectrum(pool(pool.RootId))
                Dim biodeep_spectral As PeakMs2

                For Each inferDIA As PeakMs2 In result
                    ' the unique lib guid is required in the database
                    ' due to the reason of lib guid at here is biodeep id
                    ' so we needs change it to lib guid temporary
                    ' and then changed back at last after post to database pool
                    biodeep_spectral = Protocols.MakeCopy(inferDIA)
                    biodeep_spectral.meta("organism") = ReferenceProjectId
                    biodeep_spectral.file = ReferenceProjectId
                    biodeep_spectral.lib_guid = Utils.ConservedGuid(biodeep_spectral)
#If DEBUG Then
                    SpectrumPool.DirectPush(biodeep_spectral, Me.pool, pool, root)
#Else
                    Try
                        SpectrumPool.DirectPush(biodeep_spectral, Me.pool, pool, root)
                    Catch ex As Exception
                        Call ex.Message.Warning
                    End Try
#End If
                    Yield inferDIA
                Next
            Else
                For Each inferDIA As PeakMs2 In result
                    Yield inferDIA
                Next
            End If
        End Function

        ''' <summary>
        ''' A common method for create the reference inference method 
        ''' based on a given set of the reference candidates
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="ions_all">
        ''' All of the spectrum ion information that exists in current
        ''' spectrum cluster node
        ''' </param>
        ''' <param name="reference"></param>
        ''' <param name="getFormula"></param>
        ''' <param name="getBiodeepID"></param>
        ''' <param name="getName"></param>
        ''' <returns>
        ''' + the <see cref="PeakMs2.lib_guid"/> should be the metabolite biodeep id
        ''' + the metadata is generated via function <see cref="TreeFs.GetMetadata(PeakMs2)"/>
        ''' </returns>
        Private Iterator Function InferCluster(Of T)(ions_all As Metadata(),
                                                     reference As IEnumerable(Of T),
                                                     getFormula As Func(Of T, String),
                                                     getBiodeepID As Func(Of T, String),
                                                     getName As Func(Of T, String)) As IEnumerable(Of PeakMs2)

            Dim cache As New Dictionary(Of String, PeakMs2)

            ' all of the reference spectrum that exists in current 
            ' cluster node should be filter out at first
            ' or it may affects the generated spectrum data
            ions_all = ions_all _
                .Where(Function(a) a.project <> ReferenceProjectId) _
                .ToArray

            ' loops for each metabolite reference data
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
                        {"name", getName(refer)},
                        {"organism", selects.Select(Function(a) a.organism).Distinct.JoinBy("; ")},
                        {"biosample", "DIA"},
                        {"biodeep_id", getBiodeepID(refer)},
                        {"formula", formula},
                        {"instrument", "MZKit DIA"},
                        {"project", ReferenceProjectId}
                    }
                }
            Next
        End Function

        Public Const ReferenceProjectId As String = "Reference Annotation"

        ''' <summary>
        ''' infer the spectrum based on the reference annotation of the cluster hits
        ''' </summary>
        ''' <param name="cluster_id"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' this method required of reference spectrum has already 
        ''' been push into current cluster node
        ''' </remarks>
        Public Function InferCluster(cluster_id As String) As IEnumerable(Of PeakMs2)
            Dim ions_all As Metadata() = GetAllClusterMetadata(cluster_id).ToArray
            Dim reference = ions_all _
                .Where(Function(a) a.project = ReferenceProjectId) _
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

        ''' <summary>
        ''' Get the union representive spectrum based on a given collection
        ''' of the spectrum in current cluster
        ''' </summary>
        ''' <param name="ions"></param>
        ''' <param name="cache"></param>
        ''' <returns></returns>
        Private Function GetUnionSpectra(ions As IEnumerable(Of Metadata), cache As Dictionary(Of String, PeakMs2)) As IEnumerable(Of ms2)
            Dim all_spectrums As PeakMs2() = ions _
                .Select(Function(a)
                            Dim key As String = a.block.position

                            If Not cache.ContainsKey(key) Then
                                Call cache.Add(key, pool.ReadSpectrum(a))
                            End If

                            Return cache(key)
                        End Function) _
                .Where(Function(a) Not a Is Nothing) _
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
