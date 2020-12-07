Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("data")>
Module data

    Friend Sub Main()

    End Sub

    Private Function XICTable(XIC As ms1_scan(), args As list, env As Environment) As dataframe

    End Function

    <ExportAPI("XIC")>
    <RApiReturn(GetType(ms1_scan))>
    Public Function XIC(<RRawVectorArgument> ms1 As Object, mz#, Optional ppm# = 20, Optional env As Environment = Nothing) As Object
        Dim ms1_scans As pipeline = pipeline.TryCreatePipeline(Of ms1_scan)(ms1, env)

        If ms1_scans.isError Then
            Return ms1_scans.getError
        End If

        Dim xicFilter As ms1_scan() = ms1_scans _
            .populates(Of ms1_scan)(env) _
            .Where(Function(pt) PPMmethod.PPM(pt.mz, mz) <= ppm) _
            .OrderBy(Function(a) a.scan_time) _
            .ToArray

        Return xicFilter
    End Function

    <ExportAPI("rt_slice")>
    <RApiReturn(GetType(ms1_scan))>
    Public Function RtSlice(<RRawVectorArgument> ms1 As Object, rtmin#, rtmax#, Optional env As Environment = Nothing) As Object
        Dim ms1_scans As pipeline = pipeline.TryCreatePipeline(Of ms1_scan)(ms1, env)

        If ms1_scans.isError Then
            Return ms1_scans.getError
        End If

        Dim xicFilter As ms1_scan() = ms1_scans _
            .populates(Of ms1_scan)(env) _
            .Where(Function(pt) pt.scan_time >= rtmin AndAlso pt.scan_time <= rtmax) _
            .OrderBy(Function(a) a.scan_time) _
            .ToArray

        Return xicFilter
    End Function

    ''' <summary>
    ''' get intensity value from the ion scan points
    ''' </summary>
    ''' <param name="ticks"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("intensity")>
    <RApiReturn(GetType(Double))>
    Public Function getIntensity(<RRawVectorArgument> ticks As Object, Optional env As Environment = Nothing) As Object
        Dim scans As pipeline = pipeline.TryCreatePipeline(Of ms1_scan)(ticks, env)

        If scans.isError Then
            Return scans.getError
        End If

        Return scans.populates(Of ms1_scan)(env) _
            .Select(Function(x) x.intensity) _
            .DoCall(AddressOf vector.asVector)
    End Function

    <ExportAPI("scan_time")>
    <RApiReturn(GetType(Double))>
    Public Function getScantime(<RRawVectorArgument> ticks As Object, Optional env As Environment = Nothing) As Object
        Dim scans As pipeline = pipeline.TryCreatePipeline(Of ms1_scan)(ticks, env)

        If scans.isError Then
            Return scans.getError
        End If

        Return scans.populates(Of ms1_scan)(env) _
            .Select(Function(x) x.scan_time) _
            .DoCall(AddressOf vector.asVector)
    End Function
End Module
