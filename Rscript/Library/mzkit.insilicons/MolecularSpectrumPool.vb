
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MoleculeNetworking.PoolData
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("spectrumPool")>
Public Module MolecularSpectrumPool

    Const unknown As String = NameOf(unknown)

    ''' <summary>
    ''' open the spectrum pool from a given resource link
    ''' </summary>
    ''' <param name="link"></param>
    ''' <param name="level"></param>
    ''' <returns></returns>
    <ExportAPI("openPool")>
    Public Function openPool(link As String, Optional level As Double = 0.8) As SpectrumPool
        Return SpectrumPool.Open(link, level, split:=6)
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
    Public Function getClusterInfo(pool As SpectrumPool, path As String) As Object
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
    <ExportAPI("conservedGuid")>
    Public Function conservedGuid(spectral As PeakMs2) As String
        Dim peaks As String() = spectral.mzInto _
            .OrderByDescending(Function(mzi) mzi.intensity) _
            .Take(6) _
            .Select(Function(m) m.mz.ToString("F1")) _
            .ToArray
        Dim mz1 As String = spectral.mz.ToString("F1")
        Dim meta As String() = {
            spectral.meta.TryGetValue("biosample", unknown),
            spectral.meta.TryGetValue("organism", unknown)
        }
        Dim hashcode As String = peaks _
            .JoinIterates(mz1) _
            .JoinIterates(meta) _
            .JoinBy(spectral.mzInto.Length) _
            .MD5

        Return hashcode
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
