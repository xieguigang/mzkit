Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Linq

Namespace IndexedCache

    Public Class XICReader : Implements IDisposable

        Dim disposedValue As Boolean
        Dim file As BinaryDataReader
        Dim pixeloffset As Long()()
        Dim pixelCache As Long

        Public ReadOnly Property meta As XICIndex

        Sub New(file As String)
            Me.file = New BinaryDataReader(file.Open(FileMode.Open, doClear:=False, [readOnly]:=True)) With {
                .ByteOrder = ByteOrder.BigEndian
            }

            Call loadIndex()
            Call loadPixels()
        End Sub

        Sub New(file As Stream)
            Me.file = New BinaryDataReader(file) With {
                .ByteOrder = ByteOrder.BigEndian
            }

            Call loadIndex()
            Call loadPixels()
        End Sub

        Private Sub loadPixels()
            Dim loading As New List(Of Long())

            Call file.Seek(pixelCache, SeekOrigin.Begin)

            ' width in each row
            For i As Integer = 1 To meta.height
                loading.Add(file.ReadInt64s(meta.width))
            Next

            pixeloffset = loading.ToArray
        End Sub

        Private Sub loadIndex()
            If file.ReadString(XICIndex.MagicHeader.Length) <> XICIndex.MagicHeader Then
                Throw New InvalidProgramException("invalid magic header data!")
            End If

            Dim nsize As Integer = file.ReadInt32
            Dim width As Integer = file.ReadInt32
            Dim height As Integer = file.ReadInt32
            Dim source As String = file.ReadString(BinaryStringFormat.ZeroTerminated)
            Dim tolerance As String = file.ReadString(BinaryStringFormat.ZeroTerminated)
            Dim time As String = file.ReadString(BinaryStringFormat.ZeroTerminated)

            Call file.ReadByte()

            Dim mz As Double() = file.ReadDoubles(nsize)
            Dim offset As Long() = file.ReadInt64s(nsize)

            pixelCache = file.ReadInt64
            file.ReadByte()

            _meta = New XICIndex(mz, offset, width, height, source, tolerance, Date.Parse(time))
        End Sub

        Public Function GetPixel(x As Integer, y As Integer) As ibdPixel
            Dim offset As Long = pixeloffset(y)(x)

            file.Seek(offset, SeekOrigin.Begin)
            file.ReadInt32s(2)

            Dim nsize As Integer = file.ReadInt32
            Dim mz As Double() = file.ReadDoubles(nsize)
            Dim into As Double() = file.ReadDoubles(nsize)

            Return New ibdPixel(x, y, mz.Select(Function(mzi, i) New ms2 With {.mz = mzi, .intensity = into(i)}))
        End Function

        Public Function GetIonLayer(mz As Double, tolerance As Tolerance) As PixelData()
            Return meta.GetOffsets(mz, tolerance) _
                .Select(Function(offset)
                            file.Seek(offset, SeekOrigin.Begin)

                            Dim mzi As Double = file.ReadDouble
                            Dim nlen As Integer = file.ReadInt32
                            Dim intensity As Double() = file.ReadDoubles(nlen)
                            Dim x As Integer() = file.ReadInt32s(nlen)
                            Dim y As Integer() = file.ReadInt32s(nlen)

                            Return intensity _
                                .Select(Function(into, i)
                                            Return New PixelData With {
                                                .intensity = into,
                                                .mz = mzi,
                                                .x = x(i),
                                                .y = y(i)
                                            }
                                        End Function)
                        End Function) _
                .IteratesALL _
                .ToArray
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)
                    Call file.Dispose()
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