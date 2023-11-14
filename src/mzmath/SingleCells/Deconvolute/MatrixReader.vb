Imports System.IO
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

    Private disposedValue As Boolean

    Sub New(s As Stream)
        Me.bin = New BinaryReader(s, Encoding.ASCII)
        Me.bin.BaseStream.Seek(0, SeekOrigin.Begin)
        Me.scan0 = loadHeaders()
    End Sub

    Private Function loadHeaders() As Long
        Dim bytes As Byte() = bin.ReadBytes(MatrixWriter.magic)
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

        Return bin.BaseStream.Position
    End Function

    Public Iterator Function LoadSpots() As IEnumerable(Of PixelData)
        Call bin.BaseStream.Seek(scan0, SeekOrigin.Begin)

        For i As Integer = 0 To spots - 1
            Dim x As Integer = bin.ReadInt32
            Dim y As Integer = bin.ReadInt32
            Dim label As String = bin.ReadString
            Dim into As Double() = New Double(featureSize - 1) {}

            For offset As Integer = 0 To into.Length - 1
                into(offset) = bin.ReadDouble
            Next

            Yield New PixelData With {
                .X = x,
                .Y = y,
                .label = label,
                .intensity = into
            }
        Next
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
