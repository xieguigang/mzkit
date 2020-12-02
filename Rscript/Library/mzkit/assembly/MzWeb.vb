Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object

<Package("mzweb")>
Module MzWeb

    <ExportAPI("load.stream")>
    Public Function loadStream(scans As pipeline, Optional mzErr$ = "da:0.1", Optional env As Environment = Nothing) As pipeline
        If scans.elementType Like GetType(mzXML.scan) Then
            Return mzWebCache.Load(scans.populates(Of scan)(env), mzErr).DoCall(AddressOf pipeline.CreateFromPopulator)
        ElseIf scans.elementType Like GetType(mzML.spectrum) Then
            Return mzWebCache.Load(scans.populates(Of mzML.spectrum)(env), mzErr).DoCall(AddressOf pipeline.CreateFromPopulator)
        Else
            Return Message.InCompatibleType(GetType(mzXML.scan), scans.elementType, env)
        End If
    End Function

    <ExportAPI("write.cache")>
    Public Function writeStream(scans As pipeline, Optional file As Object = Nothing, Optional env As Environment = Nothing) As Object
        Dim stream As Stream

        If file Is Nothing Then
            stream = Console.OpenStandardOutput
        ElseIf TypeOf file Is String Then
            stream = DirectCast(file, String).Open(doClear:=True)
        ElseIf TypeOf file Is Stream Then
            stream = DirectCast(file, Stream)
        Else
            Return Message.InCompatibleType(GetType(Stream), file.GetType, env)
        End If

        Call scans.populates(Of ScanMS1)(env).Write(stream)

        Return True
    End Function
End Module
