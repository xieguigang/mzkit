Imports System.IO
Imports System.IO.Compression
Imports Microsoft.VisualBasic.Data.IO.MessagePack

Public Class Writer : Implements IDisposable

    Dim disposedValue As Boolean
    Dim file As ZipArchive
    Dim index As New List(Of MassIndex)

    Const IndexPath As String = ".metadata/index"

    Sub New(file As String)
        Call Me.New(file.Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=True))
    End Sub

    Sub New(file As Stream)
        Call Me.New(New ZipArchive(file, ZipArchiveMode.Update))
    End Sub

    Sub New(file As ZipArchive)
        Me.file = file
        Me.LoadIndex()
    End Sub

    Private Sub LoadIndex()
        Dim list = From file As ZipArchiveEntry
                   In Me.file.Entries
                   Where file.FullName.StartsWith(IndexPath)

        For Each i As ZipArchiveEntry In list
            Call index.AddRange(MsgPackSerializer.Deserialize(Of MassIndex())(i.Open))
        Next
    End Sub

    Public Sub AddReference(ref As Metabolite)
        Call AddIndex(ref)
    End Sub

    Private Sub AddIndex(ref As Metabolite)

    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call file.Dispose()
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
