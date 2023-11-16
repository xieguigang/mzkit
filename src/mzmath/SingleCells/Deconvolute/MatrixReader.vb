Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute

Public Class MatrixReader : Implements IDisposable

    ''' <summary>
    ''' the target binary file
    ''' </summary>
    ReadOnly bin As BinaryReader
    ''' <summary>
    ''' the start location of the spot data
    ''' </summary>
    ReadOnly scan0 As Long

    Public ReadOnly Property tolerance As String
    Public ReadOnly Property featureSize As Integer
    Public ReadOnly Property ionSet As Double()
    Public ReadOnly Property spots As Integer

    Dim disposedValue As Boolean
    Dim spot_index As Dictionary(Of Long, Dictionary(Of Long, Long))
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

        Dim mz As Double() = New Double(featureSize - 1) {}

        For i As Integer = 0 To mz.Length - 1
            mz(i) = bin.ReadDouble
        Next

        _spots = bin.ReadInt32
        _ionSet = mz

        Dim offset1 As Long = bin.ReadInt64
        Dim offset2 As Long = bin.ReadInt64
        Dim offset_spots As Long = bin.BaseStream.Position

        Dim spot_index As New List(Of (Integer, Integer, Long))
        Dim label_index As New List(Of (String, Long))

        Call bin.BaseStream.Seek(offset1, SeekOrigin.Begin)

        For i As Integer = 0 To _spots - 1
            Dim x As Integer = bin.ReadInt32
            Dim y As Integer = bin.ReadInt32
            Dim p As Long = bin.ReadInt64

            Call spot_index.Add((x, y, p))
        Next

        Call bin.BaseStream.Seek(offset2, SeekOrigin.Begin)

        For i As Integer = 0 To _spots - 1
            Dim label As String = bin.ReadString
            Dim p As Long = bin.ReadInt64

            Call label_index.Add((label, p))
        Next

        Me.label_index = label_index _
            .GroupBy(Function(d) d.Item1) _
            .ToDictionary(Function(d) d.Key,
                          Function(d)
                              Return d _
                                  .Select(Function(o) o.Item2) _
                                  .ToArray
                          End Function)
        Me.spot_index = spot_index _
            .GroupBy(Function(a) CLng(a.Item1)) _
            .ToDictionary(Function(a)
                              Return a.Key
                          End Function, AddressOf offsetIndex)

        Return offset_spots
    End Function

    Public Function GetSpot(x As Integer, y As Integer) As PixelData
        Dim xl As Long = CLng(x)
        Dim yl As Long = CLng(y)

        If Not spot_index.ContainsKey(xl) Then
            Return Nothing
        End If

        Dim index = spot_index(xl)

        If Not index.ContainsKey(yl) Then
            Return Nothing
        Else
            Call bin.BaseStream.Seek(index(yl), SeekOrigin.Begin)
            Return LoadCurrentSpot()
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Shared Function offsetIndex(a As IGrouping(Of Long, (Integer, Integer, Long))) As Dictionary(Of Long, Long)
        Return a.ToDictionary(Function(ai) CLng(ai.Item2), Function(ai) ai.Item3)
    End Function

    Public Iterator Function LoadSpots() As IEnumerable(Of PixelData)
        Call bin.BaseStream.Seek(scan0, SeekOrigin.Begin)

        For i As Integer = 0 To spots - 1
            Yield LoadCurrentSpot()
        Next
    End Function

    Private Function LoadCurrentSpot() As PixelData
        Dim x As Integer = bin.ReadInt32
        Dim y As Integer = bin.ReadInt32
        Dim label As String = bin.ReadString
        Dim into As Double() = New Double(featureSize - 1) {}

        For offset As Integer = 0 To into.Length - 1
            into(offset) = bin.ReadDouble
        Next

        Return New PixelData With {
            .X = x,
            .Y = y,
            .label = label,
            .intensity = into
        }
    End Function

    ''' <summary>
    ''' load all matrix into memory
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function LoadMemory() As MzMatrix
        Return New MzMatrix With {
            .mz = ionSet,
            .tolerance = tolerance,
            .matrix = LoadSpots.ToArray
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
