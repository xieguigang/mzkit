
Imports Microsoft.VisualBasic.Data.IO.ManagedSqlite.Core
Imports Microsoft.VisualBasic.Data.IO.ManagedSqlite.Core.SQLSchema
Imports Microsoft.VisualBasic.Data.IO.ManagedSqlite.Core.Tables

''' <summary>
''' peaks.sqlite
''' </summary>
Public Class PeakReader : Implements IDisposable

    ReadOnly peaks As Sqlite3Database
    Private disposedValue As Boolean

    Sub New(file As String)
        peaks = Sqlite3Database.OpenFile(file)
    End Sub

    Public Iterator Function GetProperties() As IEnumerable(Of Properties)
        Dim table As Sqlite3Table = peaks.GetTable("Properties")
        Dim schema As Schema = table.SchemaDefinition.ParseSchema
        Dim key As Integer = schema.GetOrdinal("Key")
        Dim value As Integer = schema.GetOrdinal("Value")

        For Each row As Sqlite3Row In table.EnumerateRows
            Yield New Properties With {
                .Key = row(key),
                .Value = row(value)
            }
        Next
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call peaks.Dispose()
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
