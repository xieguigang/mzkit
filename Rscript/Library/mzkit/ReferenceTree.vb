Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("spectrumTree")>
Module ReferenceTreePkg

    <ExportAPI("new")>
    <RApiReturn(GetType(ReferenceTree))>
    Public Function CreateNew(file As Object, Optional env As Environment = Nothing) As Object
        Dim buffer = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)

        If buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        End If

        Dim stream As Stream = buffer.TryCast(Of Stream)
        stream.Seek(Scan0, SeekOrigin.Begin)
        Return New ReferenceTree(stream)
    End Function

    <ExportAPI("open")>
    Public Function open(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim buffer = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        End If

        Return New TreeSearch(buffer.TryCast(Of Stream))
    End Function

    <ExportAPI("query")>
    Public Function QueryTree(tree As TreeSearch, x As LibraryMatrix) As Object
        Dim centroid = tree.Centroid(x.ms2)
        Dim result = tree.Search(centroid)

        Return result
    End Function

    <ExportAPI("addBucket")>
    Public Function addBucket(tree As ReferenceTree, <RRawVectorArgument> x As Object, Optional env As Environment = Nothing) As Object
        Dim list As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(x, env)

        If list.isError Then
            Return list.getError
        End If

        For Each spectrum As PeakMs2 In list.populates(Of PeakMs2)(env)
            Call tree.Push(spectrum)
        Next

        Return tree
    End Function
End Module
