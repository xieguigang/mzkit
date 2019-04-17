Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.MassSpectrum.DATA.MetaLib

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
    <XmlType("Synonym")> Public Class CIDSynonym

        <XmlAttribute>
        Public Property CID As Integer

        <XmlText>
        Public Property Synonym As String

        Public ReadOnly Property IsChEBI As Boolean
            Get
                Return xref.IsChEBI(Synonym)
            End Get
        End Property

        Public ReadOnly Property IsCAS As Boolean
            Get
                Return xref.IsCAS(Synonym)
            End Get
        End Property

        Public ReadOnly Property IsHMDB As Boolean
            Get
                Return xref.IsHMDB(Synonym)
            End Get
        End Property

        Public ReadOnly Property IsKEGG As Boolean
            Get
                Return xref.IsKEGG(Synonym)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Synonym
        End Function

        Shared ReadOnly patterns As Func(Of String, Boolean)() = {
            AddressOf xref.IsCAS,
            AddressOf xref.IsChEBI,
            AddressOf xref.IsHMDB,
            AddressOf xref.IsKEGG
        }

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="file"></param>
        ''' <param name="filter"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' ' 第一个名称是权重最好的名称
        ''' </remarks>
        Public Shared Iterator Function LoadNames(file As String, Optional filter As Boolean = True) As IEnumerable(Of CIDSynonym)
            Dim cid As Integer = 0
            Dim name As String
            Dim t As String()

            For Each line As String In file.IterateAllLines
                t = line.Split(ASCII.TAB)
                name = t(1)

                If Integer.Parse(t(0)) <> cid Then
                    cid = Integer.Parse(t(0))

                    ' 这个是新的物质行
                    ' 第一个名称是权重最高的名称, 直接返回
                    Yield New CIDSynonym With {
                        .CID = cid,
                        .Synonym = name
                    }
                Else
                    If filter Then
                        If patterns.Any(Function(test) test(name)) Then
                            ' 只返回kegg/hmdb/chebi/cas编号名称
                            Yield New CIDSynonym With {
                                .CID = cid,
                                .Synonym = name
                            }
                        End If
                    Else
                        Yield New CIDSynonym With {
                            .CID = cid,
                            .Synonym = name
                        }
                    End If
                End If
            Next
        End Function

        Public Shared Iterator Function LoadMetInfo(file As String) As IEnumerable(Of MetaLib.MetaLib)
            Dim xref As xref = Nothing
            Dim meta As MetaLib.MetaLib = Nothing
            Dim cid As Integer = 0
            Dim cas As New List(Of String)

            For Each synonym As CIDSynonym In LoadNames(file)
                If synonym.CID <> cid Then
                    ' 这是一个新的物质,并且是第一个最好的名称
                    ' 则会需要将前面的meta信息抛出,并构建一个新的注释信息对象
                    If Not meta Is Nothing Then
                        meta.xref.CAS = cas _
                            .Distinct _
                            .ToArray

                        ' yield new unify meta info
                        Yield meta
                    End If

                    xref = New xref With {.pubchem = synonym.CID}
                    cas *= 0
                    meta = New MetaLib.MetaLib With {
                        .ID = cid,
                        .name = synonym.Synonym,
                        .xref = xref
                    }
                Else
                    If synonym.IsCAS Then
                        cas += synonym.Synonym
                    ElseIf synonym.IsChEBI Then
                        xref.chebi = synonym.Synonym
                    ElseIf synonym.IsHMDB Then
                        xref.HMDB = synonym.Synonym
                    ElseIf synonym.IsKEGG Then
                        xref.KEGG = synonym.Synonym
                    Else
                        ' do nothing
                    End If
                End If
            Next

            Yield meta
        End Function
    End Class
End Namespace