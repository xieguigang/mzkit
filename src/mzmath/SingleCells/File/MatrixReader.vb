#Region "Microsoft.VisualBasic::42e2031b46cad555ab884e166ff73d9f, mzmath\SingleCells\File\MatrixReader.vb"

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

'   Total Lines: 283
'    Code Lines: 173 (61.13%)
' Comment Lines: 63 (22.26%)
'    - Xml Docs: 77.78%
' 
'   Blank Lines: 47 (16.61%)
'     File Size: 9.54 KB


' Class MatrixReader
' 
'     Properties: featureSize, ionSet, matrixType, spots, tolerance
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: GetIntensity, GetRaster, (+2 Overloads) GetSpot, LoadCurrentSpot, loadHeaders
'               LoadMemory, LoadSpots
' 
'     Sub: (+2 Overloads) Dispose
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Serialization

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
        Get
            Return mzwindows.Mass
        End Get
    End Property

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
    Dim mzIndex As MzPool
    Dim mzdiff As Double
    Dim mzwindows As MassWindow()
    Dim dimX, dimY, dimZ As Integer()

    Public ReadOnly Property dim_size As Size
        Get
            Return New Size(dimX(1), dimY(1))
        End Get
    End Property

    Sub New(s As Stream)
        Me.bin = New BinaryReader(s, Encoding.ASCII)
        Me.bin.BaseStream.Seek(0, SeekOrigin.Begin)
        Me.scan0 = loadHeaders()
        Me.mzIndex = New MzPool(ionSet)
        Me.mzdiff = Val(tolerance)
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

        Dim mzmin As Double() = New Double(featureSize - 1) {}

        For i As Integer = 0 To mz.Length - 1
            mzmin(i) = bin.ReadDouble
        Next

        Dim mzmax As Double() = New Double(featureSize - 1) {}

        For i As Integer = 0 To mz.Length - 1
            mzmax(i) = bin.ReadDouble
        Next

        _spots = bin.ReadInt32

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

        Me.dimX = IntRange.MinMax(spot_index.Select(Function(a) a.X))
        Me.dimY = IntRange.MinMax(spot_index.Select(Function(a) a.Y))
        Me.dimZ = IntRange.MinMax(spot_index.Select(Function(a) a.Z))

        Call bin.BaseStream.Seek(offset2, SeekOrigin.Begin)

        For i As Integer = 0 To _spots - 1
            Dim label As String = bin.ReadString
            Dim p As Long = bin.ReadInt64

            Call label_index.Add((label, p))
        Next

        Me.mzwindows = mz _
            .Select(Function(mzi, i)
                        Return New MassWindow(mzi) With {
                            .mzmin = mzmin(i),
                            .mzmax = mzmax(i)
                        }
                    End Function) _
            .ToArray
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

    Public Function GetIntensity(cell_id As String, mz As Double) As Double
        Dim offset As MzIndex = mzIndex.SearchBest(mz, mzdiff)

        If Not label_index.ContainsKey(cell_id) Then
            Return 0
        End If
        If offset Is Nothing Then
            Return 0
        End If

        Call bin.BaseStream.Seek(label_index(cell_id)(0), SeekOrigin.Begin)
        Call bin.BaseStream.Seek(RawStream.INT32 * 3 + RawStream.DblFloat * offset, SeekOrigin.Current)

        Return bin.ReadDouble
    End Function

    Public Iterator Function GetRaster(mz As Double, Optional dims As (x As Integer, y As Integer, z As Integer) = Nothing) As IEnumerable(Of Double()())
        Dim s As Stream = bin.BaseStream
        Dim offset As Long
        Dim i As MzIndex = mzIndex.SearchBest(mz, mzdiff)

        If i Is Nothing Then
            ' current data contains no such ion m/z value
            Return
        End If

        If dims.x = 0 OrElse dims.y = 0 Then
            dims = spot_index.GetDimensions
        End If

        For Each layer As Grid(Of SpatialIndex) In spot_index.ZLayers
            Dim buf As Double()() = RectangularArray.Matrix(Of Double)(dims.y, dims.x)
            Dim v As Double

            For Each spot As SpatialIndex In layer.EnumerateData
                offset = spot + RawStream.INT32 * 3 + RawStream.DblFloat * i
                s.Seek(offset, SeekOrigin.Begin)
                v = bin.ReadDouble

                buf(spot.Y - 1)(spot.X - 1) = v
            Next

            Yield buf
        Next
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
            .matrixType = matrixType,
            .mzmin = mzwindows.Select(Function(a) a.mzmin).ToArray,
            .mzmax = mzwindows.Select(Function(a) a.mzmax).ToArray
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
