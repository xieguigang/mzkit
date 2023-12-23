
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MassSpectrometry.MoleculeNetworking.PoolData
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' Spectrum clustering/inference via molecule networking method, 
''' this api module is working with the biodeep public cloud service
''' </summary>
<Package("spectrumPool")>
Public Module MolecularSpectrumPool

    ''' <summary>
    ''' create a new spectrum clustering data pool
    ''' </summary>
    ''' <param name="link"></param>
    ''' <param name="level"></param>
    ''' <param name="split">hex, max=15</param>
    ''' <returns></returns>
    <ExportAPI("createPool")>
    <RApiReturn(GetType(SpectrumPool))>
    Public Function createPool(link As String,
                               Optional level As Double = 0.9,
                               Optional split As Integer = 9,
                               Optional name As String = "no_named",
                               Optional desc As String = "no_information") As Object

        Return SpectrumPool.Create(link, level, split:=split, name:=name, desc:=desc)
    End Function

    ''' <summary>
    ''' get model id from the spectrum cluster graph model
    ''' </summary>
    ''' <param name="pool"></param>
    ''' <returns></returns>
    <ExportAPI("model_id")>
    Public Function GetModelId(pool As SpectrumPool) As String
        Dim fs = pool.GetFileSystem

        If TypeOf fs Is HttpTreeFs Then
            Return DirectCast(fs, HttpTreeFs).model_id
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Create a spectrum inference protocol workflow
    ''' </summary>
    ''' <param name="url"></param>
    ''' <param name="model_id"></param>
    ''' <returns></returns>
    <ExportAPI("load_infer")>
    Public Function openInferTool(url As String, model_id As String,
                                  Optional ms1diff As String = "da:0.3",
                                  Optional ms2diff As String = "da:0.3",
                                  Optional intocutoff As Double = 0.05) As DIAInfer

        Dim tree As New HttpTreeFs(url, model_id)
        Dim dia As New DIAInfer(tree, ms1diff, ms2diff, intocutoff)

        Return dia
    End Function

    ''' <summary>
    ''' Infer and make annotation to a specific cluster
    ''' </summary>
    ''' <param name="dia"></param>
    ''' <param name="cluster_id"></param>
    ''' <param name="reference_id">
    ''' the spectrum reference id, if this parameter is missing, then 
    ''' the spectrum inference will be based on the reference cluster hits
    ''' annotation result
    ''' </param>
    ''' <returns></returns>
    ''' <remarks>
    ''' workflow for reference id inference: the alignment should be perfermance
    ''' at first for the cluster spectrum and the reference specturm, and then
    ''' get the reference id list as candidates, then finally use this function
    ''' for the inference analysis.
    ''' </remarks>
    <ExportAPI("infer")>
    <RApiReturn(GetType(PeakMs2))>
    Public Function inferReferenceSpectrum(dia As DIAInfer, cluster_id As String,
                                           Optional reference_id As String() = Nothing,
                                           Optional formula As String() = Nothing,
                                           Optional name As String() = Nothing) As Object

        If reference_id.IsNullOrEmpty OrElse formula.IsNullOrEmpty Then
            Return dia.InferCluster(cluster_id).ToArray
        Else
            If name.IsNullOrEmpty Then
                name = formula
            End If

            Dim tuples = reference_id _
                .Select(Function(biodeep_id, i)
                            Return New NamedValue(Of String)(biodeep_id, formula(i), name(i))
                        End Function) _
                .ToArray
            Dim result = dia.InferCluster(cluster_id, reference:=tuples).ToArray

            Return result
        End If
    End Function

    ''' <summary>
    ''' open the spectrum pool from a given resource link
    ''' </summary>
    ''' <param name="link">
    ''' the resource string to the spectrum pool, this resource string could be
    ''' a local file or a remote cloud services endpoint
    ''' </param>
    ''' <param name="model_id">
    ''' the model id, this parameter works for open the model in the cloud service
    ''' </param>
    ''' <param name="score_overrides">
    ''' WARNING: this optional parameter will overrides the mode score 
    ''' level when this parameter has a positive numeric value in 
    ''' range ``(0,1]``. it is dangers to overrides the score parameter
    ''' in the exists model.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("openPool")>
    Public Function openPool(link As String,
                             Optional model_id As String = Nothing,
                             Optional score_overrides As Double? = Nothing,
                             Optional env As Environment = Nothing) As SpectrumPool

        If score_overrides IsNot Nothing AndAlso
            score_overrides > 0 AndAlso
            score_overrides < 1 Then

            Call env.AddMessage($"NOTICE: the score level of the spectrum graph model has been overrides to {score_overrides}!", MSG_TYPES.WRN)
        End If

        Return SpectrumPool.Open(link, model_id:=model_id, score:=score_overrides)
    End Function

    ''' <summary>
    ''' close the connection to the spectrum pool
    ''' </summary>
    ''' <param name="pool"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' this function works for the spectrum clustering pool in local file,
    ''' do nothing when running upon based on a cloud service.
    ''' </remarks>
    <ExportAPI("closePool")>
    Public Function closePool(pool As SpectrumPool) As Object
        Call pool.Dispose()
        Return Nothing
    End Function

    ''' <summary>
    ''' get metadata dataframe in a given cluster tree
    ''' </summary>
    ''' <param name="pool"></param>
    ''' <param name="path"></param>
    ''' <returns>
    ''' A dataframe object that contains the metadata of each spectrum inside the given 
    ''' cluster tree, this includes:
    ''' 
    ''' 1. biodeep_id: metabolite unique reference id inside the biodeep database
    ''' 2. name: the metabolite common name
    ''' 3. formula: the chemical formula of the current metabolite
    ''' 4. adducts: the precursor adducts of the metabolite addociated with the spectrum precursor ion
    ''' 5. mz: the precursor ion m/z
    ''' 6. rt: the lcms rt in data unit of seconds
    ''' 7. intensity: the ion intensity value
    ''' 8. source: the rawdata file source of current spectrum ion comes from
    ''' 9. biosample: the biological sample source
    ''' 10. organism: the biological species source
    ''' 11. project: the public project id, example as the metabolights project id
    ''' 12. instrument: the instrument name of the spectrum, could be extract from the metabolights project metadata.
    ''' </returns>
    <ExportAPI("getClusterInfo")>
    Public Function getClusterInfo(pool As SpectrumPool, Optional path As String = Nothing) As Object
        Dim tokens = path.Trim("\"c, "/"c).StringSplit("[\\/]+")

        For Each t As String In tokens
            pool = pool(t)

            If pool Is Nothing Then
                Return Nothing
            End If
        Next

        Dim info As Metadata() = pool.ClusterInfo.ToArray
        Dim data As New dataframe With {
            .rownames = info.Select(Function(a) a.guid).ToArray,
            .columns = New Dictionary(Of String, Array) From {
                {"biodeep_id", info.Select(Function(a) a.biodeep_id).ToArray},
                {"name", info.Select(Function(a) a.name).ToArray},
                {"formula", info.Select(Function(a) a.formula).ToArray},
                {"adducts", info.Select(Function(a) a.adducts).ToArray},
                {"mz", info.Select(Function(a) a.mz).ToArray},
                {"rt", info.Select(Function(a) a.rt).ToArray},
                {"intensity", info.Select(Function(a) a.intensity).ToArray},
                {"source", info.Select(Function(a) a.source_file).ToArray},
                {"biosample", info.Select(Function(a) a.sample_source).ToArray},
                {"organism", info.Select(Function(a) a.organism).ToArray},
                {"project", info.Select(Function(a) a.project).ToArray},
                {"instrument", info.Select(Function(a) a.instrument).ToArray}
            }
        }

        Return data
    End Function

    ''' <summary>
    ''' generates the guid for the spectrum with unknown annotation
    ''' </summary>
    ''' <param name="spectral"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' the conserved guid is generated based on the md5 hashcode of contents:
    ''' 
    ''' 1. mz(F4):into
    ''' 2. mz1(F4)
    ''' 3. rt(F2)
    ''' 4. biosample
    ''' 5. organism
    ''' 6. instrument
    ''' 7. precursor_type
    ''' </remarks>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("conservedGuid")>
    <RApiReturn(GetType(String))>
    Public Function conservedGuid(<RRawVectorArgument> spectral As Object, Optional env As Environment = Nothing) As Object
        Return env.EvaluateFramework(Of PeakMs2, String)(spectral, AddressOf Utils.ConservedGuid)
    End Function

    ''' <summary>
    ''' generate and set conserved guid for each spectrum data
    ''' </summary>
    ''' <param name="spectral"></param>
    ''' <param name="prefix"></param>
    ''' <param name="env"></param>
    ''' <returns>A collection of the mzkit <see cref="PeakMs2"/> clr object
    ''' which has the lib guid data assigned.</returns>
    <ExportAPI("set_conservedGuid")>
    Public Function SetConservedGuid(<RRawVectorArgument>
                                     spectral As Object,
                                     Optional prefix As String = Nothing,
                                     Optional env As Environment = Nothing) As Object

        Dim msms = pipeline.TryCreatePipeline(Of PeakMs2)(spectral, env)

        If msms.isError Then
            Return msms.getError
        End If

        Dim allData = msms.populates(Of PeakMs2)(env).ToArray

        If prefix.StringEmpty Then
            For i As Integer = 0 To allData.Length - 1
                allData(i).lib_guid = Utils.ConservedGuid(allData(i))
            Next
        Else
            ' 20230412 handling of the invalid id reference for biodeepMSMS
            ' script package
            For i As Integer = 0 To allData.Length - 1
                allData(i).lib_guid = $"{prefix}|{Utils.ConservedGuid(allData(i))}"
            Next
        End If

        Return allData
    End Function

    ''' <summary>
    ''' add sample peaks data to spectrum pool
    ''' </summary>
    ''' <param name="pool"></param>
    ''' <param name="x">
    ''' the spectrum data collection
    ''' </param>
    ''' <param name="biosample"></param>
    ''' <param name="organism"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' the spectrum data for run clustering should be 
    ''' processed into centroid mode at first!
    ''' </remarks>
    <ExportAPI("addPool")>
    Public Function add(pool As SpectrumPool, <RRawVectorArgument> x As Object,
                        Optional biosample As String = "unknown",
                        Optional organism As String = "unknown",
                        Optional project As String = "unknown",
                        Optional instrument As String = "unknown",
                        Optional file As String = "unknown",
                        Optional filename_overrides As Boolean = False,
                        Optional env As Environment = Nothing) As Object

        Dim data As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(x, env)

        If data.isError Then
            Return data.getError
        Else
            file = file _
                .Replace(".mzPack", "") _
                .Replace(".mzXML", "") _
                .Replace(".mzML", "") _
                .Replace(".txt", "") _
                .Replace(".csv", "") _
                .Replace(".mgf", "")
        End If

        For Each peak As PeakMs2 In data.populates(Of PeakMs2)(env)
            If peak.meta Is Nothing Then
                peak.meta = New Dictionary(Of String, String)
            End If

            peak.meta("biosample") = biosample
            peak.meta("organism") = organism
            peak.meta("project") = project
            peak.meta("instrument") = instrument

            peak.lib_guid = conservedGuid(peak)

            If filename_overrides OrElse peak.file.StringEmpty Then
                peak.file = file
                peak.meta("file") = file
            End If

            Call pool.Add(peak)
        Next

        Return Nothing
    End Function

    ''' <summary>
    ''' commit data to the spectrum pool database
    ''' </summary>
    ''' <param name="pool"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' this function works for the spectrum molecular networking pool in a local file,
    ''' do nothing when running upon a cloud service
    ''' </remarks>
    <ExportAPI("commit")>
    <RApiReturn(GetType(SpectrumPool))>
    Public Function commit(pool As SpectrumPool) As Object
        Call pool.Commit()
        Return pool
    End Function
End Module
