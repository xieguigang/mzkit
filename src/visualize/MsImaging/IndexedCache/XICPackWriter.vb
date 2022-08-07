Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem

Namespace IndexedCache

    Public Class XICPackWriter : Implements IDisposable

        ReadOnly stream As StreamPack

        Dim disposedValue As Boolean

        Sub New(file As String)
            Call Me.New(file.Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=False))
        End Sub

        Sub New(file As Stream)
            stream = New StreamPack(file,, meta_size:=32 * 1024 * 1024)
        End Sub

        Public Sub SetAttribute(dims As Size)
            Call stream.globalAttributes.Add("dims", {dims.Width, dims.Height})
        End Sub

        Public Sub AddLayer(layer As MatrixXIC)
            Using buffer As Stream = stream.OpenBlock($"/{layer.GetType.Name}/{layer.mz}.ms")
                Call layer.Serialize(buffer)
            End Using
        End Sub



        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
                    Call stream.Dispose()
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
End Namespace