#Region "Microsoft.VisualBasic::ecfa7152d41786887de40b4f3fa70e4f, assembly\LoadR.NET5\SpatialMatrixReader.vb"

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

'   Total Lines: 143
'    Code Lines: 108 (75.52%)
' Comment Lines: 9 (6.29%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 26 (18.18%)
'     File Size: 4.75 KB


' Class SpatialMatrixReader
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: getColumn, getMatrix, getNames, getRow, getRowNames
'               loadLayer
' 
'     Sub: CreateIndex
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components.Interface
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports std = System.Math

''' <summary>
''' The wrapper of the spatial matrix to R# language
''' </summary>
Public Class SpatialMatrixReader : Implements IdataframeReader, IReflector

    ReadOnly m As MzMatrix
    ReadOnly index As MzPool
    ReadOnly mzdiff As Tolerance
    ReadOnly spotIndex As Dictionary(Of String, Integer)

    Sub New(m As MzMatrix)
        Me.m = m
        Me.mzdiff = Tolerance.ParseScript(m.tolerance)
        Me.index = New MzPool(m.mz)

        If m.matrixType <> FileApplicationClass.SingleCellsMetabolomics Then
            Call CreateIndex(m, spotIndex)
        Else
            ' index via the cell labels
            spotIndex = m.matrix _
                .Select(Function(s, i) (s, i)) _
                .GroupBy(Function(s) s.s.label) _
                .ToDictionary(Function(s) s.Key,
                              Function(s)
                                  Return s.First.i
                              End Function)
        End If
    End Sub

    Private Shared Sub CreateIndex(m As MzMatrix, ByRef spatial As Dictionary(Of String, Integer))
        Dim offsets = m.matrix.Select(Function(s, i) (s, i)).ToArray

        ' 20240617
        '
        ' single cells data x,y always 0,0
        ' 
        '  Error in <globalEnvironment> -> InitializeEnvironment -> "RunAnalysis"("rawdata" <- "./Saccharomyces_ce...) -> R_invoke$RunAnalysis -> "run"(...)(...) -> R_invoke$run -> "__runImpl"("context" <- Call ".get_context"...)("context" <- Call ".get_context"...) -> R_invoke$__runImpl -> for_loop_[[1]: "exportMzMatrix"] -> if_closure -> R_invoke$if_closure_internal -> ".internal_call"(&app, &context...)(&app, &context...) -> R_invoke$.internal_call -> if_closure -> R_invoke$if_closure_internal -> "do.call"(&app["call"], "args" <- &argv...)(&app["call"], "args" <- &argv...) -> do.call -> R_invoke$exportMzMatrix -> "write.matrix"(Call "filter_spot_features"(&raw...)(Call "filter_spot_features"(&raw...) -> write.matrix -> "filter_spot_features"(&rawdata, Call "as.data.frame"(&...)(&rawdata, Call "as.data.frame"(&...) -> R_invoke$filter_spot_features -> if_closure -> R_invoke$if_closure_internal -> "df.mz_matrix"(&m...)(&m...) -> df.mz_matrix
        '   1. ArgumentException: An item with the same key has already been added. Key: 0,0
        '   2. stackFrames: 
        '    at System.Collections.Generic.Dictionary`2.TryInsert(TKey key, TValue value, InsertionBehavior behavior)
        '    at System.Linq.Enumerable.ToDictionary[TSource,TKey,TElement](TSource[] source, Func`2 keySelector, Func`2 elementSelector, IEqualityComparer`1 comparer)
        '    at System.Linq.Enumerable.ToDictionary[TSource,TKey,TElement](IEnumerable`1 source, Func`2 keySelector, Func`2 elementSelector)
        '    at BioNovoGene.Analytical.MassSpectrometry.SpatialMatrixReader.CreateIndex(MzMatrix m, Dictionary`2& spatial)
        '    at BioNovoGene.Analytical.MassSpectrometry.SpatialMatrixReader..ctor(MzMatrix m)
        '    at mzkit.SingleCells.dfMzMatrix(MzMatrix x)
        '
        '    "m" <- Call SingleCells::"df.mz_matrix"(&m)
        '    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        '
        ' SingleCells.R#_clr_interop::.df.mz_matrix at [mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]:line &Hx045cb9d00
        ' MSI.call_function."df.mz_matrix"(&m...)(&m...) at filter_spot_features.R:line 13
        ' unknown.unknown.R_invoke$if_closure_internal at n/a:line n/a
        ' MSI.n/a.if_closure at filter_spot_features.R:line 6
        ' MSI.declare_function.R_invoke$filter_spot_features at filter_spot_features.R:line 5
        ' MSI.call_function."filter_spot_features"(&rawdata, Call "as.data.frame"(&...)(&rawdata, Call "as.data.frame"(&...) at exportMzMatrix.R:line 61
        ' SingleCells.R#_clr_interop::.write.matrix at [mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]:line &Hx0e3908600
        ' MSI.call_function."write.matrix"(Call "filter_spot_features"(&raw...)(Call "filter_spot_features"(&raw...) at exportMzMatrix.R:line 61
        ' MSI.declare_function.R_invoke$exportMzMatrix at exportMzMatrix.R:line 7
        ' env.R#_clr_interop::.do.call at [REnv, Version=2.33.856.6961, Culture=neutral, PublicKeyToken=null]:line &Hx02b13a100
        ' WorkflowRender.call_function."do.call"(&app["call"], "args" <- &argv...)(&app["call"], "args" <- &argv...) at runner.R:line 20
        ' unknown.unknown.R_invoke$if_closure_internal at n/a:line n/a
        ' WorkflowRender.n/a.if_closure at runner.R:line 13
        ' WorkflowRender.declare_function.R_invoke$.internal_call at runner.R:line 3
        ' WorkflowRender.call_function.".internal_call"(&app, &context...)(&app, &context...) at workflowRender.R:line 69
        ' unknown.unknown.R_invoke$if_closure_internal at n/a:line n/a
        ' WorkflowRender.n/a.if_closure at workflowRender.R:line 68
        ' WorkflowRender.forloop.for_loop_[[1]: "exportMzMatrix"] at workflowRender.R:line 52
        ' WorkflowRender.declare_function.R_invoke$__runImpl at workflowRender.R:line 37
        ' WorkflowRender.call_function."__runImpl"("context" <- Call ".get_context"...)("context" <- Call ".get_context"...) at workflowRender.R:line 32
        ' WorkflowRender.declare_function.R_invoke$run at workflowRender.R:line 18
        ' MSI.call_function."run"(...)(...) at RunAnalysis.R:line 73
        ' MSI.declare_function.R_invoke$RunAnalysis at RunAnalysis.R:line 19
        ' SMRUCC/R#.call_function."RunAnalysis"("rawdata" <- "./Saccharomyces_ce...) at test.R:line 5
        ' SMRUCC/R#.n/a.InitializeEnvironment at test.R:line 0
        ' SMRUCC/R#.global.<globalEnvironment> at <globalEnvironment>:line n/a
        If m.matrixType = FileApplicationClass.MSImaging3D Then
            spatial = offsets _
                .ToDictionary(Function(s) $"{s.s.X},{s.s.Y},{s.s.Z}",
                              Function(a)
                                  Return a.i
                              End Function)
        Else
            spatial = offsets _
                .ToDictionary(Function(s) $"{s.s.X},{s.s.Y}",
                              Function(a)
                                  Return a.i
                              End Function)
        End If
    End Sub

    ''' <summary>
    ''' get matrix column projection via m/z or index vector
    ''' </summary>
    ''' <param name="index"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    Public Function getColumn(index As Object, env As Environment) As Object Implements IdataframeReader.getColumn
        Dim mz As Double() = CLRVector.asNumeric(index)

        If mz.IsNullOrEmpty Then
            Return Nothing
        End If

        ' the input index is integer offsets?
        If mz.All(Function(value) value = std.Round(value)) Then
            Return matrixProjection(mz.AsInteger)
        End If

        If mz.Length = 1 Then
            Return loadLayer(mz(0))
        End If

        Dim out As New list With {
            .slots = New Dictionary(Of String, Object)
        }

        For Each mzi As Double In mz
            Call out.add(mzi.ToString, loadLayer(mzi))
        Next

        Return out
    End Function

    ''' <summary>
    ''' create matrix projection via the integer index offsets
    ''' </summary>
    ''' <param name="offsets"></param>
    ''' <returns></returns>
    Private Function matrixProjection(offsets As Integer()) As MzMatrix
        Dim m As New MzMatrix With {
            .matrixType = Me.m.matrixType,
            .tolerance = Me.m.tolerance
        }
        Dim mz As Double() = Me.m.mz.CopyOf(offsets)
        Dim mzmin As Double() = Me.m.mzmin.CopyOf(offsets)
        Dim mzmax As Double() = Me.m.mzmax.CopyOf(offsets)
        Dim rawdata = Me.m.matrix
        Dim project As PixelData() = New PixelData(rawdata.Length - 1) {}
        Dim source As PixelData

        For Each i As Integer In Tqdm.Range(0, Me.m.matrix.Length)
            source = rawdata(i)
            project(i) = New PixelData With {
                .label = source.label,
                .X = source.X,
                .Y = source.Y,
                .Z = source.Z,
                .intensity = source.intensity.CopyOf(offsets)
            }
        Next

        m.mz = mz
        m.mzmin = mzmin
        m.mzmax = mzmax
        m.matrix = project

        Return m
    End Function

    ''' <summary>
    ''' load layer data via a given ion m/z value
    ''' </summary>
    ''' <param name="mzi"></param>
    ''' <returns></returns>
    Private Function loadLayer(mzi As Double) As dataframe
        Dim offsets As Integer() = Me.index _
            .Search(mzi) _
            .Where(Function(mzhit)
                       Return mzdiff(mzi, mzhit.mz)
                   End Function) _
            .Select(Function(h) h.index) _
            .ToArray

        If offsets.Length = 0 Then
            Return Nothing
        End If

        Dim x As New List(Of Integer)
        Dim y As New List(Of Integer)
        Dim label As New List(Of String)
        Dim into As New List(Of Double)

        For Each spot As PixelData In m.matrix
            Dim intensity As Double = Aggregate i In offsets Into Sum(spot.intensity(i))

            If intensity <= 0.0 Then
                Continue For
            End If

            Call x.Add(spot.X)
            Call y.Add(spot.Y)
            Call label.Add(spot.label)
            Call into.Add(intensity)
        Next

        Return New dataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {"x", x.ToArray},
                {"y", y.ToArray},
                {"label", label.ToArray},
                {"mz", {mzi}},
                {"intensity", into.ToArray}
            },
            .rownames = x _
                .Select(Function(xi, i) $"{xi},{y(i)}") _
                .ToArray
        }
    End Function

    ''' <summary>
    ''' get a list of the spatial expression vector
    ''' </summary>
    ''' <param name="index"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    Public Function getRow(index As Object, env As Environment) As Object Implements IdataframeReader.getRow
        Dim xy As String() = CLRVector.asCharacter(index)

        If xy.IsNullOrEmpty Then
            Return Nothing
        ElseIf xy.Length = 1 Then
            Return m.matrix(spotIndex(xy(Scan0))).intensity
        End If

        Dim vec As New list With {.slots = New Dictionary(Of String, Object)}

        For Each spatial As String In xy
            If spotIndex.ContainsKey(spatial) Then
                Call vec.add(spatial, m.matrix(spotIndex(spatial)).intensity)
            End If
        Next

        Return vec
    End Function

    ''' <summary>
    ''' get column m/z ion feature labels
    ''' </summary>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function getNames() As String() Implements IReflector.getNames
        Return m.mz.Select(Function(mzi) mzi.ToString).ToArray
    End Function

    ''' <summary>
    ''' get row labels of this matrix value:
    ''' 
    ''' 1. ms-imaging data get x,y,z
    ''' 2. single cells get cell labels
    ''' </summary>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function getRowNames() As String() Implements IdataframeReader.getRowNames
        If m.matrixType = FileApplicationClass.SingleCellsMetabolomics Then
            Return m.matrix.Select(Function(i) i.label).ToArray
        Else
            Return spotIndex.Keys.ToArray
        End If
    End Function

    Public Function getMatrix() As MzMatrix
        Return m
    End Function
End Class
