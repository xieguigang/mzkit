Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph

''' <summary>
''' A lazy binary data matrix reader for the singlecells/spatial data
''' </summary>
Public Class MatrixReader : Implements IDisposable

    ''' <summary>
    ''' the target binary file
    ''' </summary>
    ReadOnly bin As BinaryReader
    ''' <summary>
    ''' the start location of the spot data
    ''' </summary>
    ReadOnly scan0 As Long

    ''' <summary>
    ''' the description text of the mass tolerance error
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property tolerance As String
    ''' <summary>
    ''' number of the ion features
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property featureSize As Integer
    ''' <summary>
    ''' A numeric vector of the ion features m/z
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ionSet As Double()
    ''' <summary>
    ''' the number of the single cells or spatial spot data
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property spots As Integer

    ''' <summary>
    ''' <see cref="MzMatrix.matrixType"/>
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property matrixType As FileApplicationClass

    Dim disposedValue As Boolean
    Dim spot_index As Spatial3D(Of SpatialIndex)
    Dim label_index As Dictionary(Of String, Long())

    Sub New(s As Stream)
        Me.bin = New BinaryReader(s, Encoding.ASCII)
        Me.bin.BaseStream.Seek(0, SeekOrigin.Begin)
        Me.scan0 = loadHeaders()
    End Sub

    Private Function loadHeaders() As Long
        Dim bytes As Byte() = bin.ReadBytes(MatrixWriter.magic.Length)
        Dim si As String = Encoding.ASCII.GetString(bytes)

        If si <> MatrixWriter.magic Then
            Throw New InvalidProgramException("invalid magic header!")
        End If

        _tolerance = bin.ReadString
        _featureSize = bin.ReadInt32
        _matrixType = CType(bin.ReadInt32, FileApplicationClass)

        Dim mz As Double() = New Double(featureSize - 1) {}

        For i As Integer = 0 To mz.Length - 1
            mz(i) = bin.ReadDouble
        Next

        _spots = bin.ReadInt32
        _ionSet = mz

        Dim offset1 As Long = bin.ReadInt64
        Dim offset2 As Long = bin.ReadInt64
        Dim offset_spots As Long = bin.BaseStream.Position

        Dim spot_index As New List(Of SpatialIndex)
        Dim label_index As New List(Of (String, Long))

        Call bin.BaseStream.Seek(offset1, SeekOrigin.Begin)

        For i As Integer = 0 To _spots - 1
            Dim x As Integer = bin.ReadInt32
            Dim y As Integer = bin.ReadInt32
            Dim z As Integer = bin.ReadInt32
            Dim p As Long = bin.ReadInt64

            Call spot_index.Add(New SpatialIndex(x, y, z, p))
        Next

        Call bin.BaseStream.Seek(offset2, SeekOrigin.Begin)

        For i As Integer = 0 To _spots - 1
            Dim label As String = bin.ReadString
            Dim p As Long = bin.ReadInt64

            Call label_index.Add((label, p))
        Next

        Me.spot_index = Spatial3D(Of SpatialIndex).CreateSpatial3D(Of SpatialIndex)(spot_index)
        Me.label_index = label_index _
            .GroupBy(Function(d) d.Item1) _
            .ToDictionary(Function(d) d.Key,
                          Function(d)
                              Return d _
                                  .Select(Function(o) o.Item2) _
                                  .ToArray
                          End Function)

        Return offset_spots
    End Function

    ''' <summary>
    ''' get a 2d spatial spot data
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <returns></returns>
    Public Function GetSpot(x As Integer, y As Integer, Optional z As Integer = 0) As PixelData
        Dim hit As Boolean = False
        Dim index As SpatialIndex = Me.spot_index.GetData(x, y, z, hit)

        If Not hit Then
            Return Nothing
        Else
            Call bin.BaseStream.Seek(index.offset, SeekOrigin.Begin)
            Return LoadCurrentSpot()
        End If
    End Function

    Public Function GetSpot(cell_id As String) As PixelData
        If Not label_index.ContainsKey(cell_id) Then
            Return Nothing
        End If

        Call bin.BaseStream.Seek(label_index(cell_id)(0), SeekOrigin.Begin)
        Return LoadCurrentSpot()
    End Function

    Public Iterator Function LoadSpots() As IEnumerable(Of PixelData)
        Call bin.BaseStream.Seek(scan0, SeekOrigin.Begin)

        For i As Integer = 0 To spots - 1
            Yield LoadCurrentSpot()
        Next
    End Function

    ''' <summary>
    ''' for a better perfermance of binary data file seek operation
    ''' the scan data is in structrue of:
    ''' 
    ''' ```
    '''   x,  y,  z,intensity,label_string
    ''' i32,i32,i32,  f64 * n,string
    ''' ```
    ''' 
    ''' so, for seek a ion intensity value will be in fast speed
    ''' </summary>
    ''' <returns></returns>
    Friend Function LoadCurrentSpot() As PixelData
        Dim x As Integer = bin.ReadInt32
        Dim y As Integer = bin.ReadInt32
        Dim z As Integer = bin.ReadInt32
        Dim into As Double() = New Double(featureSize - 1) {}

        For offset As Integer = 0 To into.Length - 1
            into(offset) = bin.ReadDouble
        Next

        Return New PixelData With {
            .X = x,
            .Y = y,
            .label = bin.ReadString,
            .intensity = into,
            .Z = z
        }
    End Function

    ''' <summary>
    ''' load all matrix into memory at once
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function LoadMemory() As MzMatrix
        Return New MzMatrix With {
            .mz = ionSet,
            .tolerance = tolerance,
            .matrix = LoadSpots.ToArray,
            .matrixType = matrixType
        }
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                Call bin.Dispose()
            End If

            ' TODO: 释放未托管的资源(未托管的对象)并重写终结器
            ' TODO: 将大型字段设置为 null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
    ' Protected Overrides Sub Finalize()
    '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
