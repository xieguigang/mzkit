Imports System.IO
Imports System.IO.Compression

Namespace MZWork

    Public Class WorkspaceAccess : Implements IDisposable

        Private disposedValue As Boolean

        ReadOnly zip As ZipArchive

        Sub New(zip As String)
            Me.zip = New ZipArchive(zip.Open(FileMode.Open, doClear:=False, [readOnly]:=True), ZipArchiveMode.Read)
        End Sub

        Sub New(zip As ZipArchive)
            Me.zip = zip
        End Sub

        Public Function ListAllFileNames() As String()

        End Function

        Public Function GetByFileName(fileName As String) As mzPack

        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)
                    Call zip.Dispose()
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