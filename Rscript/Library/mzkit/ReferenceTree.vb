Imports System.IO
Imports System.Runtime.CompilerServices
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

    ''' <summary>
    ''' create new reference spectrum database
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("new")>
    <RApiReturn(GetType(ReferenceTree))>
    Public Function CreateNew(file As Object,
                              Optional binary As Boolean = True,
                              Optional env As Environment = Nothing) As Object

        Dim buffer = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)

        If buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        End If

        Dim stream As Stream = buffer.TryCast(Of Stream)
        Call stream.Seek(Scan0, SeekOrigin.Begin)

        If binary Then
            Return New ReferenceBinaryTree(stream)
        Else
            Return New ReferenceTree(stream)
        End If
    End Function

    ''' <summary>
    ''' open the reference spectrum database file and 
    ''' then create a host to run spectrum cluster 
    ''' search
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("open")>
    Public Function open(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim buffer = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        End If

        Return New TreeSearch(buffer.TryCast(Of Stream))
    End Function

    <ExportAPI("jaccardSet")>
    Public Function createJaccardSet(libname As String(),
                                     mz As Double(),
                                     mzset As String(),
                                     Optional rt As Double() = Nothing) As JaccardSearch

        Dim dataset As JaccardSet() = libname _
            .Select(Function(a, i)
                        Return New JaccardSet With {
                            .libname = a,
                            .mz1 = mz(i),
                            .rt = rt.ElementAtOrDefault(i),
                            .ms2 = mzset(i) _
                                .StringSplit(",\s*") _
                                .Select(AddressOf Conversion.Val) _
                                .ToArray
                        }
                    End Function) _
            .ToArray

        Return New JaccardSearch(dataset)
    End Function

    <ExportAPI("query")>
    <RApiReturn(GetType(ClusterHit))>
    Public Function QueryTree(tree As Ms2Search, x As Object,
                              Optional maxdepth As Integer = 1024,
                              Optional treeSearch As Boolean = False,
                              Optional env As Environment = Nothing) As Object

        If TypeOf x Is LibraryMatrix Then
            Return DirectCast(x, LibraryMatrix).QuerySingle(tree, maxdepth, treeSearch, env)
        ElseIf TypeOf x Is PeakMs2 Then
            Return DirectCast(x, PeakMs2).QuerySingle(tree, maxdepth, treeSearch, env)
        ElseIf TypeOf x Is list Then
            Return DirectCast(x, list).QueryTree(tree, maxdepth, treeSearch, env)
        Else
            Throw New NotImplementedException
        End If
    End Function

    <Extension>
    Private Function QuerySingle(x As LibraryMatrix, tree As Ms2Search,
                                 Optional maxdepth As Integer = 1024,
                                 Optional treeSearch As Boolean = False,
                                 Optional env As Environment = Nothing) As Object

        Dim centroid = tree.Centroid(DirectCast(x, LibraryMatrix).ms2)
        Dim result As ClusterHit

        If (Not treeSearch) AndAlso x.parentMz <= 0.0 Then
            Return Internal.debug.stop($"mz query required a positive m/z value!", env)
        End If
        If treeSearch Then
            result = DirectCast(tree, TreeSearch).Search(centroid, maxdepth:=maxdepth)
        Else
            result = tree.Search(centroid, mz1:=x.parentMz)
        End If

        If Not result Is Nothing Then
            result.queryId = DirectCast(x, LibraryMatrix).name
            result.queryMz = DirectCast(x, LibraryMatrix).parentMz
        End If

        Return result
    End Function

    <Extension>
    Private Function QuerySingle(x As PeakMs2, tree As Ms2Search,
                                 Optional maxdepth As Integer = 1024,
                                 Optional treeSearch As Boolean = False,
                                 Optional env As Environment = Nothing) As Object

        Dim centroid = tree.Centroid(DirectCast(x, PeakMs2).mzInto)
        Dim result As ClusterHit

        If (Not treeSearch) AndAlso x.mz <= 0.0 Then
            Return Internal.debug.stop($"mz query required a positive m/z value!", env)
        End If
        If treeSearch Then
            result = DirectCast(tree, TreeSearch).Search(centroid, maxdepth:=maxdepth)
        Else
            result = tree.Search(centroid, mz1:=x.mz)
        End If

        If Not result Is Nothing Then
            result.queryId = DirectCast(x, PeakMs2).lib_guid
            result.queryMz = DirectCast(x, PeakMs2).mz
            result.queryRt = DirectCast(x, PeakMs2).rt
        End If

        Return result
    End Function

    <Extension>
    Private Function QueryTree(input As list, tree As Ms2Search,
                               Optional maxdepth As Integer = 1024,
                               Optional treeSearch As Boolean = False,
                               Optional env As Environment = Nothing) As Object

        Dim output As New list With {.slots = New Dictionary(Of String, Object)}
        Dim result As Object

        For Each name As String In input.getNames
            result = QueryTree(tree, input(name), maxdepth, treeSearch, env)

            If Program.isException(result) Then
                Return result
            Else
                Call output.add(name, result)
            End If
        Next

        Return output
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
