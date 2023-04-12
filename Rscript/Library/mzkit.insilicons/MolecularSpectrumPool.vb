
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MoleculeNetworking.PoolData
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
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
    ''' open the spectrum pool from a given resource link
    ''' </summary>
    ''' <param name="link"></param>
    ''' <param name="level"></param>
    ''' <param name="split">
    ''' hex, max=15
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("openPool")>
    Public Function openPool(link As String,
                             Optional level As Double = 0.9,
                             Optional split As Integer = 9) As SpectrumPool

        Return SpectrumPool.Open(link, level, split:=split)
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
                {"organism", info.Select(Function(a) a.organism).ToArray}
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
                        Optional organism As String = "unkown",
                        Optional env As Environment = Nothing) As Object

        Dim data As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(x, env)

        If data.isError Then
            Return data.getError
        End If

        For Each peak As PeakMs2 In data.populates(Of PeakMs2)(env)
            peak.meta.Add("biosample", biosample)
            peak.meta.Add("organism", organism)
            peak.lib_guid = conservedGuid(peak)

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
