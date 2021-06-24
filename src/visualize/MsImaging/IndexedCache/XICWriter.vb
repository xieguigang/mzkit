Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Data.IO
Imports stdNum = System.Math

Namespace IndexedCache

    Public Class XICWriter : Implements IDisposable

        ReadOnly tolerance As Tolerance = Tolerance.PPM(10)
        ReadOnly offsets As New Dictionary(Of Double, BufferRegion)
        ReadOnly length As New Dictionary(Of Double, Integer)
        ReadOnly bufferSize As Long
        ReadOnly cache As String
        ReadOnly cachefile As BinaryDataWriter

        Private disposedValue As Boolean

        ''' <summary>
        ''' [x,y]intensity
        ''' </summary>
        Const delta As Integer = 4 + 4 + 8

        Sub New(Optional bufferSize As Long = 1024 * 1024 * 2)
            Me.bufferSize = bufferSize
            Me.cache = TempFileSystem.GetAppSysTempFile(, App.PID, "MSI_XIC_")
            Me.cachefile = New BinaryDataWriter(cache.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False))
        End Sub

        Public Sub WritePixels(pixel As PixelScan)
            Dim xy As Integer() = {pixel.X, pixel.Y}

            For Each mz As ms2 In pixel.GetMs
                Dim mzi As Double = stdNum.Round(mz.mz, 4)
                Dim offset As Long

                If offsets.ContainsKey(mzi) Then
                    offset = offsets(mzi).position
                Else
                    offset = Allocates(bufferSize)

                    length(mzi) = 0
                    offsets(mzi) = New BufferRegion With {
                        .position = offset,
                        .size = bufferSize
                    }
                End If

                offset += length(mzi) * delta

                cachefile.Seek(offset, SeekOrigin.Begin)
                cachefile.Write(xy)
                cachefile.Write(mz.intensity)
            Next
        End Sub

        Private Function Allocates(bufferSize As Long) As Long

        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)
                    Call cachefile.Flush()
                    Call cachefile.Close()
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
End Namespace