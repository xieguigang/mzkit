Imports System.IO

Public Class mcfParser : Implements IDisposable

    ReadOnly buffer As Stream
    Private disposedValue As Boolean

    Sub New(mcf_file As String)
        buffer = mcf_file.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
    End Sub

    Public Function GetBlob(index As ContainerIndex) As Byte()
        Dim buf As Byte() = New Byte(index.BlobSize - 1) {}

        Call buffer.Seek(index.Offset, SeekOrigin.Begin)
        Call buffer.Read(buf, Scan0, index.BlobSize)

        Return buf
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call buffer.Dispose()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
