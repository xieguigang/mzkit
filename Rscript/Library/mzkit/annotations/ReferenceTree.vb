#Region "Microsoft.VisualBasic::08d28b45e9fc277ea57b94d74ef4807a, mzkit\Rscript\Library\mzkit\annotations\ReferenceTree.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
' 
' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' MIT License
' 
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 251
'    Code Lines: 167
' Comment Lines: 49
'   Blank Lines: 35
'     File Size: 9.18 KB


' Module ReferenceTreePkg
' 
'     Function: addBucket, createJaccardSet, CreateNew, open, (+2 Overloads) QuerySingle
'               (+2 Overloads) QueryTree, set_dotcutoff
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.Query
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Interpreter
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' the spectrum tree reference library tools
''' </summary>
''' <remarks>
''' the spectrum data is clustering and save in family 
''' tree data structure.
''' </remarks>
<Package("spectrumTree")>
Module ReferenceTreePkg

    ''' <summary>
    ''' create new reference spectrum database
    ''' </summary>
    ''' <param name="file">
    ''' A file path to save the spectrum reference database
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("new")>
    <RApiReturn(GetType(ReferenceTree), GetType(ReferenceBinaryTree), GetType(SpectrumPack))>
    Public Function CreateNew(file As Object,
                              Optional type As ClusterTypes = ClusterTypes.Default,
                              Optional env As Environment = Nothing) As Object

        Dim buffer = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)

        If buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        End If

        Dim stream As Stream = buffer.TryCast(Of Stream)
        Call stream.Seek(Scan0, SeekOrigin.Begin)

        Select Case type
            Case ClusterTypes.Binary : Return New ReferenceBinaryTree(stream)
            Case ClusterTypes.Tree, ClusterTypes.Default : Return New ReferenceTree(stream)
            Case ClusterTypes.Pack : Return New SpectrumPack(stream)
            Case Else
                Return Internal.debug.stop(New NotImplementedException(type.Description), env)
        End Select
    End Function

    ''' <summary>
    ''' ### open the spectrum reference database
    ''' 
    ''' open the reference spectrum database file and 
    ''' then create a host to run spectrum cluster 
    ''' search
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' the data format is test via the magic header
    ''' </remarks>
    <ExportAPI("open")>
    <RApiReturn(GetType(PackAlignment), GetType(TreeSearch))>
    Public Function open(<RRawVectorArgument>
                         file As Object,
                         Optional dotcutoff As Double = 0.6,
                         Optional env As Environment = Nothing) As Object

        Dim buffer = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        End If

        Dim buf As Stream = buffer.TryCast(Of Stream)
        Dim isHDS = StreamPack.TestMagic(buf)

        Call buf.Seek(Scan0, SeekOrigin.Begin)

        If isHDS Then
            Return New PackAlignment(New SpectrumReader(buf), dotcutoff)
        Else
            Return New TreeSearch(buffer.TryCast(Of Stream)).SetCutoff(dotcutoff)
        End If
    End Function

    ''' <summary>
    ''' set dot cutoff parameter for the cos score similarity algorithm
    ''' </summary>
    ''' <param name="search">
    ''' The spectrum library stream engine
    ''' </param>
    ''' <param name="cutoff">
    ''' cutoff threshold value of the cos score
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("dotcutoff")>
    Public Function set_dotcutoff(search As TreeSearch, cutoff As Double) As TreeSearch
        Call search.SetCutoff(cutoff)
        Return search
    End Function

    ''' <summary>
    ''' construct a fragment set library for run spectrum search in jaccard matches
    ''' </summary>
    ''' <param name="libname"></param>
    ''' <param name="mz"></param>
    ''' <param name="mzset"></param>
    ''' <param name="rt"></param>
    ''' <param name="cutoff"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("jaccardSet")>
    Public Function createJaccardSet(libname As String(),
                                     mz As Double(),
                                     mzset As String(),
                                     Optional rt As Double() = Nothing,
                                     Optional cutoff As Double = 0.1,
                                     Optional env As Environment = Nothing) As JaccardSearch

        Dim dataset As JaccardSet() = libname _
            .Select(Function(a, i)
                        Return New JaccardSet With {
                            .libname = a,
                            .mz1 = mz(i),
                            .rt = rt.ElementAtOrDefault(i),
                            .ms2 = mzset(i) _
                                .Replace("["c, "") _
                                .Replace("]"c, "") _
                                .Replace(" "c, "") _
                                .Split(","c) _
                                .Select(AddressOf Conversion.Val) _
                                .ToArray
                        }
                    End Function) _
            .ToArray
        Dim println = env.WriteLineHandler

        Call println($"Do jaccard match with cutoff value: {cutoff}!")

        Return New JaccardSearch(dataset, cutoff)
    End Function

    ''' <summary>
    ''' do spectrum family alignment via cos similarity
    ''' </summary>
    ''' <param name="tree">
    ''' The reference spectrum tree object to search 
    ''' </param>
    ''' <param name="x">
    ''' The query spectrum data from the sample raw data files
    ''' </param>
    ''' <param name="maxdepth">
    ''' The max depth of the tree search
    ''' </param>
    ''' <param name="treeSearch">
    ''' Do alignment in family tree search mode?
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' function returns nothing means no query hits or the 
    ''' given input query sample data <paramref name="x"/>
    ''' is nothing
    ''' </returns>
    <ExportAPI("query")>
    <RApiReturn(GetType(ClusterHit))>
    Public Function QueryTree(tree As Ms2Search, x As Object,
                              Optional maxdepth As Integer = 1024,
                              Optional treeSearch As Boolean = False,
                              Optional env As Environment = Nothing) As Object

        If x Is Nothing Then
            Call env.AddMessage("The given spectrum input data is nothing!", MSG_TYPES.WRN)
            Return Nothing
        End If

        If TypeOf x Is LibraryMatrix Then
            Return DirectCast(x, LibraryMatrix).QuerySingle(tree, maxdepth, treeSearch, env)
        ElseIf TypeOf x Is PeakMs2 Then
            Return DirectCast(x, PeakMs2).QuerySingle(tree, maxdepth, treeSearch, env)
        ElseIf TypeOf x Is list Then
            Return DirectCast(x, list).QueryTree(tree, maxdepth, treeSearch, env)
        Else
            Throw New NotImplementedException("Not supported input spectrum data type: " & x.GetType.FullName)
        End If
    End Function

    <Extension>
    Private Function QuerySingle(x As LibraryMatrix, tree As Ms2Search,
                                 Optional maxdepth As Integer = 1024,
                                 Optional treeSearch As Boolean = False,
                                 Optional env As Environment = Nothing) As Object

        Dim centroid = tree.Centroid(DirectCast(x, LibraryMatrix).ms2)
        Dim result As ClusterHit()

        If (Not treeSearch) AndAlso x.parentMz <= 0.0 Then
            Return Internal.debug.stop($"mz query required a positive m/z value!", env)
        End If
        If treeSearch Then
            result = {DirectCast(tree, TreeSearch).Search(centroid, maxdepth:=maxdepth)}
        Else
            result = tree.Search(centroid, mz1:=x.parentMz).ToArray
        End If

        Dim basePeakMz As Double = x.ms2 _
            .OrderByDescending(Function(a) a.intensity) _
            .FirstOrDefault _
            ?.mz
        Dim output As New List(Of ClusterHit)

        For Each hit As ClusterHit In result
            If Not hit Is Nothing Then
                hit.queryId = x.name
                hit.queryMz = x.parentMz
                hit.basePeak = basePeakMz

                Call output.Add(hit)
            End If
        Next

        If output.Count = 0 Then
            Return Nothing
        Else
            Return output.ToArray
        End If
    End Function

    <Extension>
    Private Function QuerySingle(x As PeakMs2, tree As Ms2Search,
                                 Optional maxdepth As Integer = 1024,
                                 Optional treeSearch As Boolean = False,
                                 Optional env As Environment = Nothing) As Object

        Dim centroid = tree.Centroid(DirectCast(x, PeakMs2).mzInto)
        Dim result As ClusterHit()

        If (Not treeSearch) AndAlso x.mz <= 0.0 Then
            Return Internal.debug.stop($"mz query required a positive m/z value!", env)
        End If
        If treeSearch Then
            result = {DirectCast(tree, TreeSearch).Search(centroid, maxdepth:=maxdepth)}
        Else
            result = tree.Search(centroid, mz1:=x.mz)
        End If

        Dim basePeakMz As Double = x.mzInto _
            .OrderByDescending(Function(a) a.intensity) _
            .FirstOrDefault _
            ?.mz
        Dim output As New List(Of ClusterHit)

        For Each hit As ClusterHit In result
            If Not hit Is Nothing Then
                hit.queryId = x.lib_guid
                hit.queryMz = x.mz
                hit.queryRt = x.rt
                hit.basePeak = basePeakMz

                Call output.Add(hit)
            End If
        Next

        If output.Count = 0 Then
            Return Nothing
        Else
            Return output.ToArray
        End If
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

    ''' <summary>
    ''' push the reference spectrum data into the spectrum reference tree library
    ''' </summary>
    ''' <param name="tree">
    ''' The reference spectrum database, which the spectrum data 
    ''' is store in family tree style
    ''' </param>
    ''' <param name="x">
    ''' A new spectrum data to push into the reference database
    ''' </param>
    ''' <param name="args">
    ''' additional parameters for create the spectrum library in spectrum pack format:
    ''' 
    ''' 1. uuid, BioDeepID, biodeep_id is used for the metabolite unique reference id
    ''' 2. exactMass, exact_mass, mass is used for the metabolite exact mass value
    ''' 
    ''' and the spectrum input of x should be the same metabolite if save data as 
    ''' the spectrum pack data.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("addBucket")>
    Public Function addBucket(tree As Object,
                              <RRawVectorArgument> x As Object,
                              <RListObjectArgument>
                              Optional args As list = Nothing,
                              Optional env As Environment = Nothing) As Object

        Dim list As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(x, env)

        If list.isError Then
            Return list.getError
        ElseIf tree Is Nothing Then
            Return Internal.debug.stop("The required reference library object can not be nothing!", env)
        End If

        If TypeOf tree Is ReferenceTree Then
            For Each spectrum As PeakMs2 In list.populates(Of PeakMs2)(env)
                Call DirectCast(tree, ReferenceTree).Push(spectrum)
            Next
        ElseIf TypeOf tree Is SpectrumPack Then
            Dim uuid As String = args.getValue(Of String)({"uuid", "BioDeepID", "biodeep_id"}, env)
            Dim mass As Double = args.getValue(Of Double)({"exactMass", "exact_mass", "mass"}, env, [default]:=-1.0)

            If uuid.StringEmpty Then
                Return Internal.debug.stop("No metabolite uuid or biodeep id was provided!", env)
            End If
            If mass <= 0 Then
                Return Internal.debug.stop("A positive exact mass value of the target metabolite must be provided!", env)
            End If

            For Each spectrum As PeakMs2 In list.populates(Of PeakMs2)(env)
                Call DirectCast(tree, SpectrumPack).Push(uuid, mass, spectrum)
            Next
        Else
            Return Message.InCompatibleType(GetType(ReferenceTree), tree.GetType, env)
        End If

        Return tree
    End Function
End Module
