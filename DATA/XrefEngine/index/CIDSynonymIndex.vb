Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Data.IO.SearchEngine.Index
Imports SMRUCC.MassSpectrum.DATA.NCBI.PubChem

Public Class CIDSynonymIndex

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mapFile">CID-Synonym-filtered.txt</param>
    Public Shared Sub BuildIndex(mapFile As String, indexFile As String)
        Using index As New TrieIndexWriter(New BinaryDataWriter(indexFile.Open(doClear:=False)))
            For Each name As CIDSynonym In CIDSynonym.LoadNames(mapFile, filter:=False)
                Call index.AddTerm(name.Synonym)
            Next

            Stop
        End Using
    End Sub
End Class
