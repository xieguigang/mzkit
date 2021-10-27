
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("mzDeco")>
Module mzDeco

    ''' <summary>
    ''' Chromatogram data deconvolution
    ''' </summary>
    ''' <param name="ms1"></param>
    ''' <param name="tolerance"></param>
    ''' <returns></returns>
    <ExportAPI("mz.deco")>
    <RApiReturn(GetType(PeakFeature()))>
    Public Function mz_deco(<RRawVectorArgument>
                            ms1 As Object,
                            Optional tolerance As Object = "ppm:20",
                            Optional baseline# = 0.65,
                            <RRawVectorArgument>
                            Optional peakwidth As Object = "3,20",
                            Optional env As Environment = Nothing) As Object

        Dim ms1_scans As IEnumerable(Of IMs1Scan) = ms1Scans(ms1)
        Dim errors As [Variant](Of Tolerance, Message) = Math.getTolerance(tolerance, env)
        Dim rtRange = ApiArgumentHelpers.GetDoubleRange(peakwidth, env, [default]:="3,20")

        If errors Like GetType(Message) Then
            Return errors.TryCast(Of Message)
        ElseIf rtRange Like GetType(Message) Then
            Return rtRange.TryCast(Of Message)
        End If

        Return ms1_scans _
            .GetMzGroups(errors) _
            .DecoMzGroups(rtRange.TryCast(Of DoubleRange), quantile:=baseline) _
            .ToArray
    End Function

    ''' <summary>
    ''' do ``m/z`` grouping under the given tolerance
    ''' </summary>
    ''' <param name="ms1"></param>
    ''' <param name="mzdiff"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("mz.groups")>
    <RApiReturn(GetType(MzGroup()))>
    Public Function mz_groups(<RRawVectorArgument>
                              ms1 As Object,
                              Optional mzdiff As Object = "ppm:20",
                              Optional env As Environment = Nothing) As Object

        Return ms1Scans(ms1).GetMzGroups(Math.getTolerance(mzdiff, env)).ToArray
    End Function

    Private Function ms1Scans(ms1 As Object) As IEnumerable(Of IMs1Scan)
        If ms1 Is Nothing Then
            Return {}
        ElseIf ms1.GetType Is GetType(ms1_scan()) Then
            Return DirectCast(ms1, ms1_scan()).Select(Function(t) DirectCast(t, IMs1Scan))
        Else
            Throw New NotImplementedException
        End If
    End Function
End Module
