Imports System.IO
Imports Microsoft.VisualBasic.Data.IO

Public Class Reader : Implements IDisposable

    Dim infile As BinaryDataReader
    Dim disposedValue As Boolean

    Public ReadOnly Property root As BlockNode

    Sub New(buf As Stream)
        infile = New BinaryDataReader(buf)

        If infile.ReadString(Writer.magic.Length) <> Writer.magic Then
            Throw New InvalidDataException("invalid magic header!")
        End If

        root = ReadNextNode(infile.BaseStream.Position)
    End Sub

    Public Function ReadNextNode(pos As Long) As BlockNode
        infile.Seek(pos, SeekOrigin.Begin)

        If infile.ReadByte = 0 Then
            Return Nothing
        End If

        Dim scan0 = infile.BaseStream.Position  ' data size entry
        Dim left = scan0 + infile.ReadInt64     ' offset + data size = left
        Dim right As Long

        infile.Seek(left, SeekOrigin.Begin)
        right = left + infile.ReadInt64         ' left_offset + data size = right
        infile.Seek(right, SeekOrigin.Begin)

        Return New BlockNode With {
            .scan0 = scan0,
            .left = left,
            .right = right
        }
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                Call infile.Dispose()
            End If

            ' TODO: 释放未托管的资源(未托管的对象)并替代终结器
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
