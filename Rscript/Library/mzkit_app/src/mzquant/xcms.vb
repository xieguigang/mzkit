
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' the xcms interop and data handler
''' </summary>
<Package("xcms")>
Module xcms

    <ExportAPI("parse_xcms_samples")>
    <RApiReturn(GetType(XcmsSamplePeak), GetType(PeakFeature))>
    Public Function parse_xcms_samples(<RRawVectorArgument> file As Object,
                                       Optional group_features As Boolean = False,
                                       Optional env As Environment = Nothing) As Object

        Dim auto_close As Boolean = False
        Dim s = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env,
                                            is_filepath:=auto_close)

        If s Like GetType(Message) Then
            Return s.TryCast(Of Message)
        End If

        Dim peaks As IEnumerable(Of XcmsSamplePeak) = XcmsSamplePeak.ParseCsv(s.TryCast(Of Stream))

        If group_features Then
            Dim samples = peaks.GroupBy(Function(si) si.sample).ToArray
            Dim samplelist As list = list.empty

            For Each sample In samples
                Call samplelist.add(sample.Key, From si In sample Select New PeakFeature(si))
            Next

            Return samplelist
        Else
            Return peaks.ToArray
        End If
    End Function

End Module
