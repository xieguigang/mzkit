
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MassSpectrometry.MoleculeNetworking.PoolData
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

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
    Public Function createPool(link As String,
                               Optional level As Double = 0.9,
                               Optional split As Integer = 9,
                               Optional name As String = "no_named",
                               Optional desc As String = "no_information") As SpectrumPool

        Return SpectrumPool.Create(link, level, split:=split, name:=name, desc:=desc)
    End Function

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
    ''' open the spectrum pool from a given resource link
    ''' </summary>
    ''' <param name="link">
    ''' the resource string to the spectrum pool
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
    ''' <returns></returns>
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
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("conservedGuid")>
    <RApiReturn(GetType(String))>
    Public Function conservedGuid(<RRawVectorArgument> spectral As Object, Optional env As Environment = Nothing) As Object
        Return env.EvaluateFramework(Of PeakMs2, String)(spectral, AddressOf Utils.ConservedGuid)
    End Function

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
    <ExportAPI("commit")>
    Public Function commit(pool As SpectrumPool) As Object
        Call pool.Commit()
        Return pool
    End Function
End Module
