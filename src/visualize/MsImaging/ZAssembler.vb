Imports System.Drawing
Imports System.IO
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells
Imports Microsoft.VisualBasic.ComponentModel.Algorithm

''' <summary>
''' assemble the 2D layers as 3D volume data
''' </summary>
Public Class ZAssembler : Implements IDisposable

    ReadOnly s As Stream
    ReadOnly bin As BinaryWriter
    ReadOnly writeSpots As SpotWriter

    ''' <summary>
    ''' offset position of the spot data blocks start location
    ''' </summary>
    Dim offset As Long
    Dim mzIndex As BlockSearchFunction(Of (mz As Double, Integer))
    Dim len As Integer

    Private disposedValue As Boolean

    Sub New(buf As Stream)
        s = buf
        bin = New BinaryWriter(buf, encoding:=Encoding.ASCII)
        writeSpots = New SpotWriter(bin)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="header">the <see cref="MatrixHeader.numSpots"/> may be zero in lazy mode!</param>
    Public Sub WriteHeader(header As MatrixHeader)
        offset = MatrixWriter.WriteHeader(bin, header)
        len = header.mz.Length
        mzIndex = header.mz.CreateMzIndex(win_size:=1.25)

        ' write index placeholder
        Call bin.Write(0&)
        Call bin.Write(0&)
    End Sub

    Public Sub Write2DLayer(layer As IMZPack, z As Integer)
        Call VBDebugger.cat({$"   * process {layer.source} ... "})

        If layer Is Nothing OrElse layer.MS.TryCount = 0 Then
            Call VBDebugger.EchoLine("skiped.")
            Return
        End If

        For Each cell As ScanMS1 In layer.MS
            Dim xy As Point = cell.GetMSIPixel
            Dim v As Double() = Deconvolute.DeconvoluteScan(cell.mz, cell.into, len, mzIndex)
            Dim spot As New Deconvolute.PixelData With {
                .X = xy.X,
                .Y = xy.Y,
                .Z = z,
                .label = $"{z} - {cell.scan_id}",
                .intensity = v
            }

            Call writeSpots.AddSpot(spot)
        Next

        Call VBDebugger.EchoLine("ok!")
    End Sub

    Private Sub flushIndex()
        Dim offset1, offset2 As Long

        Call MatrixWriter.WriteIndex(bin, writeSpots, offset1, offset2)

        Call s.Seek(offset, SeekOrigin.Begin)
        Call bin.Write(offset1)
        Call bin.Write(offset2)
        Call bin.Flush()
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                Call flushIndex()
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
