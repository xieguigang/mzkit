Imports Microsoft.VisualBasic.Text

Namespace NCBI.PubChem

    ''' <summary>
    ''' This is a listing of all (live) SIDs with their source names and
    ''' registry identifiers, and the standardized CID If present. It Is 
    ''' a gzipped text file where Each line contains at least three
    ''' columns: SID, tab, source name, tab, registry identifier; then
    ''' a fourth column Of tab, CID If there Is a standardized CID For 
    ''' the given SID.
    ''' </summary>
    Public Class SIDMap

        Public Property SID As Integer
        Public Property sourceName As String
        Public Property registryIdentifier As String
        Public Property CID As Integer

        Public Shared Iterator Function GetMaps(path As String) As IEnumerable(Of SIDMap)
            Dim t As String()
            Dim CID As Integer

            For Each line As String In path.IterateAllLines
                t = line.Split(ASCII.TAB)

                If t.Length > 3 Then
                    CID = Integer.Parse(t(3))
                Else
                    CID = -1
                End If

                Yield New SIDMap With {
                    .CID = CID,
                    .SID = Integer.Parse(t(0)),
                    .sourceName = t(1),
                    .registryIdentifier = t(2)
                }
            Next
        End Function
    End Class
End Namespace