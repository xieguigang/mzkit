Imports System.IO
Imports Microsoft.VisualBasic.Data.IO.ManagedSqlite.Core
Imports Microsoft.VisualBasic.Data.IO.ManagedSqlite.Core.SQLSchema
Imports Microsoft.VisualBasic.Data.IO.ManagedSqlite.Core.Tables
Imports Microsoft.VisualBasic.My.JavaScript

Public Class IndexParser : Implements IDisposable

    ReadOnly sqlite As Sqlite3Database
    Private disposedValue As Boolean

    Sub New(indexfile As String)
        sqlite = New Sqlite3Database(indexfile.Open(FileMode.Open, doClear:=False, [readOnly]:=True))
    End Sub

    Public Iterator Function LoadTable(tableName As String) As IEnumerable(Of JavaScriptObject)
        Dim table As Sqlite3Table = sqlite.GetTable(tableName)
        Dim schema As Schema = table.SchemaDefinition.ParseSchema

        For Each row As Sqlite3Row In table.EnumerateRows
            Dim json As New JavaScriptObject

            For i As Integer = 0 To schema.columns.Length - 1
                json(schema.columns(i).Name) = row(i)
            Next

            Yield json
        Next
    End Function

    Public Iterator Function GetContainerIndex() As IEnumerable(Of ContainerIndex)
        Dim table As Sqlite3Table = sqlite.GetTable("ContainerIndex")
        Dim schema As Schema = table.SchemaDefinition.ParseSchema
        Dim guidA As Integer = schema.GetOrdinal(NameOf(ContainerIndex.GuidA))
        Dim guidB As Integer = schema.GetOrdinal(NameOf(ContainerIndex.GuidB))
        Dim blobType As Integer = schema.GetOrdinal(NameOf(ContainerIndex.BlobResType))
        Dim offset As Integer = schema.GetOrdinal(NameOf(ContainerIndex.Offset))
        Dim blobSize As Integer = schema.GetOrdinal(NameOf(ContainerIndex.BlobSize))

        For Each row As Sqlite3Row In table.EnumerateRows
            Yield New ContainerIndex With {
                .BlobResType = row(blobType),
                .BlobSize = row(blobSize),
                .GuidA = row(guidA),
                .GuidB = row(guidB),
                .Offset = row(offset)
            }
        Next
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call sqlite.Dispose()
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
