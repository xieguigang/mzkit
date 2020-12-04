Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("data")>
Module data

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
End Module
