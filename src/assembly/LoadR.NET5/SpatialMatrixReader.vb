Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components.Interface
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Vectorization

''' <summary>
''' The wrapper of the spatial matrix to R# language
''' </summary>
Public Class SpatialMatrixReader : Implements IdataframeReader, IReflector

    ReadOnly m As MzMatrix
    ReadOnly index As MzPool
    ReadOnly mzdiff As Tolerance
    ReadOnly spatialIndex As Dictionary(Of String, Integer)

    Sub New(m As MzMatrix)
        Me.m = m
        Me.mzdiff = Tolerance.ParseScript(m.tolerance)
        Me.index = New MzPool(m.mz)

        Call CreateIndex(m, spatialIndex)
    End Sub

    Private Shared Sub CreateIndex(m As MzMatrix, ByRef spatial As Dictionary(Of String, Integer))
        Dim offsets = m.matrix.Select(Function(s, i) (s, i)).ToArray

        spatial = offsets _
            .ToDictionary(Function(s) $"{s.s.X},{s.s.Y}",
                          Function(a)
                              Return a.i
                          End Function)
    End Sub

    Public Function getColumn(index As Object, env As Environment) As Object Implements IdataframeReader.getColumn
        Dim mz As Double() = CLRVector.asNumeric(index)

        If mz.IsNullOrEmpty Then
            Return Nothing
        ElseIf mz.Length = 1 Then
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
            Return m.matrix(spatialIndex(xy(Scan0))).intensity
        End If

        Dim vec As New list With {.slots = New Dictionary(Of String, Object)}

        For Each spatial As String In xy
            If spatialIndex.ContainsKey(spatial) Then
                Call vec.add(spatial, m.matrix(spatialIndex(spatial)).intensity)
            End If
        Next

        Return vec
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function getNames() As String() Implements IReflector.getNames
        Return m.mz.Select(Function(mzi) mzi.ToString).ToArray
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function getRowNames() As String() Implements IdataframeReader.getRowNames
        Return spatialIndex.Keys.ToArray
    End Function

    Public Function getMatrix() As MzMatrix
        Return m
    End Function
End Class
