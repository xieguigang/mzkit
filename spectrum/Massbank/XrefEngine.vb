Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Assembly.EBI.ChEBI.XML
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

''' <summary>
''' 基于hmdb和chebi数据库所构建的代谢物的注释数据的查询引擎
''' 主要是通过编号进行查询注释
''' </summary>
Public Class XrefEngine

    ''' <summary>
    ''' <see cref="EntityObject.ID"/>的属性值为hmdb编号值
    ''' </summary>
    Dim hmdbXrefs As New Dictionary(Of EntityObject)
    ''' <summary>
    ''' hmdb数据库之中，次级编号转换为主编号
    ''' </summary>
    Dim hmdb2ndMapSolver As New SecondaryIDSolver
    Dim chebi2ndMapSolver As New SecondaryIDSolver
    Dim chebi As New Dictionary(Of ChEBIEntity)
    Dim metlin2Hmdb As New Dictionary(Of String, String)

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="hmdb$">
    ''' 从 http://www.hmdb.ca/downloads 页面所下载的**All Metabolites**数据集
    ''' </param>
    ''' <param name="chebi">
    ''' 包含有两种数据：从chebi下载的在线数据以及从chebi ftp服务器上面所下载的tsv文件的文件夹
    ''' </param>
    Sub New(hmdb$, chebi As (cache$, tsv$))
        Dim getXref = Xref.CreateDictionary(Of metabolite)

        For Each m As metabolite In metabolite.Load(hmdb)
            With m
                Call hmdb2ndMapSolver.Add(
                    .accession,
                    .secondary_accessions.accession)
                Call hmdbXrefs.Add(
                    New EntityObject With {
                        .ID = m.accession,
                        .Properties = getXref(m)
                    })

                If Not .metlin_id.StringEmpty AndAlso
                    Not metlin2Hmdb.ContainsKey(.metlin_id) Then

                    Call metlin2Hmdb.Add(.metlin_id, .accession)
                End If
            End With
        Next

        For Each xml As String In (ls - l - r - "*.XML" <= chebi.cache)
            Dim entity = xml.LoadXml(Of ChEBIEntity())

            For Each chebiData As ChEBIEntity In entity
                With chebiData
                    Call chebi2ndMapSolver.Add(
                        .chebiId,
                        .SecondaryChEBIIds)

                    Me.chebi(.chebiId) = chebiData
                End With
            Next
        Next
    End Sub

    ''' <summary>
    ''' 从hmdb编号mapping到chebi的主编号
    ''' </summary>
    ''' <param name="hmdb"></param>
    ''' <returns></returns>
    Public Function HMDB2ChEBI(hmdb$) As String
        Dim chebi$ = hmdbXrefs(hmdb)(NameOf(metabolite.chebi_id))

        If chebi.StringEmpty Then
            Return Nothing
        Else
            chebi = chebi2ndMapSolver.SolveIDMapping(chebi)
            Return chebi
        End If
    End Function

    Public Function Metlin2ChEBI(mid$) As String
        If metlin2Hmdb.ContainsKey(mid) Then
            Return HMDB2ChEBI(metlin2Hmdb(mid))
        Else
            Return Nothing
        End If
    End Function
End Class
