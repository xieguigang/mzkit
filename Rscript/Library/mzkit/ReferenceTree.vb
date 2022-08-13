Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Interpreter
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
    <RApiReturn(GetType(ClusterHit))>
    Public Function QueryTree(tree As TreeSearch, x As Object,
                              Optional maxdepth As Integer = 1024,
                              Optional env As Environment = Nothing) As Object

        If TypeOf x Is LibraryMatrix Then
            Dim centroid = tree.Centroid(DirectCast(x, LibraryMatrix).ms2)
            Dim result = tree.Search(centroid, maxdepth:=maxdepth)

            If Not result Is Nothing Then
                result.queryId = DirectCast(x, LibraryMatrix).name
                result.queryMz = DirectCast(x, LibraryMatrix).parentMz
            End If

            Return result
        ElseIf TypeOf x Is PeakMs2 Then
            Dim centroid = tree.Centroid(DirectCast(x, PeakMs2).mzInto)
            Dim result = tree.Search(centroid, maxdepth:=maxdepth)

            If Not result Is Nothing Then
                result.queryId = DirectCast(x, PeakMs2).lib_guid
                result.queryMz = DirectCast(x, PeakMs2).mz
                result.queryRt = DirectCast(x, PeakMs2).rt
            End If

            Return result
        ElseIf TypeOf x Is list Then
            Dim output As New list With {.slots = New Dictionary(Of String, Object)}
            Dim input As list = DirectCast(x, list)
            Dim result As Object

            For Each name As String In input.getNames
                result = QueryTree(tree, input(name), maxdepth, env)

                If Program.isException(result) Then
                    Return result
                Else
                    Call output.add(name, result)
                End If
            Next

            Return output
        Else
            Throw New NotImplementedException
        End If
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
