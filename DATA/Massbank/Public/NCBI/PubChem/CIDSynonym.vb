Imports Microsoft.VisualBasic.Text

Namespace NCBI.PubChem

    ''' <summary>
    ''' These are listings of all names associated with a CID. The
    ''' unfiltered list are names aggregated from all SIDs whose 
    ''' standardized form Is that CID, sorted by weight With the "best"
    ''' names first. The filtered list has some names removed that are
    ''' considered inconsistend With the Structure. Both are gzipped text
    ''' files with CID, tab, And name on each line. Note that the
    ''' names may be composed Of more than one word, separated by spaces.
    ''' </summary>
    ''' <remarks>Data for ``CID-Synonym-filtered.gz`` file</remarks>
    Public Class CIDSynonym

        Public Property CID As String
        Public Property Synonym As String

        Public ReadOnly Property IsChEBI As Boolean
            Get
                Return Synonym.IsPattern("CHEBI[:]\d+", RegexICSng)
            End Get
        End Property

        Public ReadOnly Property IsCAS As Boolean
            Get
                Return Synonym.IsPattern("\d+([-]\d+)+", RegexICSng)
            End Get
        End Property

        Public ReadOnly Property IsHMDB As Boolean
            Get
                Return Synonym.IsPattern("HMDB\d+", RegexICSng)
            End Get
        End Property

        Public ReadOnly Property IsKEGG As Boolean
            Get
                Return Synonym.IsPattern("C((\d){5})", RegexICSng)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Synonym
        End Function

        Public Shared Iterator Function LoadNames(file As String) As IEnumerable(Of CIDSynonym)
            Dim cid$ = 0
            Dim t As String()

            ' 第一个名称是权重最好的名称
            For Each line As String In file.IterateAllLines
                t = line.Split(ASCII.TAB)

                If t(0) <> cid Then
                    ' 这个是新的物质行
                    ' 第一个名称是权重最高的名称, 直接返回
                End If
            Next
        End Function
    End Class
End Namespace