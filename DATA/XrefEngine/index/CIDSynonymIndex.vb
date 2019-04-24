Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Data.IO.SearchEngine.Index
Imports SMRUCC.MassSpectrum.DATA.NCBI.PubChem

''' <summary>
''' 这个查询索引是大小写敏感的
''' </summary>
Public Class CIDSynonymIndex : Implements IDisposable

    Dim reader As TrieIndexReader

    Sub New(dbFile As String)
        reader = New TrieIndexReader(dbFile)
    End Sub

    Public Function GetCID(name As String) As Long
        Return reader.GetData(name)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mapFile">CID-Synonym-filtered.txt</param>
    Public Shared Sub BuildIndex(mapFile As String, indexFile$, Optional doClear As Boolean = True)
        Using index As New TrieIndexWriter(indexFile.Open(doClear:=doClear))
            For Each name As CIDSynonym In CIDSynonym.LoadNames(mapFile, filter:=False)
                Call index.AddTerm(name.Synonym, name.CID)
            Next
        End Using
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                Call reader.Dispose()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
