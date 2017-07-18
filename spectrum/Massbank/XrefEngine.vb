Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Assembly.EBI.ChEBI.XML
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.proteomics.MS_Spectrum.DATA.HMDB

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

    Dim metlin2Hmdb As New Dictionary(Of String, String)
    Dim CAS2hmdb As New Dictionary(Of String, String)

#Region "ChEBI注释和hmdb注释都是使用数据库主编号来作为唯一标识符的"

    Dim hmdb As New Dictionary(Of metabolite)
    Dim chebi As New Dictionary(Of ChEBIEntity)

#End Region

    ''' <summary>
    ''' Get ``chebi`` metabolite data
    ''' </summary>
    ''' <param name="id">纯数字的ChEBI编号类型</param>
    ''' <returns></returns>
    Default Public Overloads ReadOnly Property GetDATA(id&) As ChEBIEntity
        Get
            Dim chebiID$ = UCase(chebi2ndMapSolver("CHEBI:" & id))

            If chebi.ContainsKey(chebiID) Then
                Return chebi(chebiID)
            Else
                Return Nothing
            End If
        End Get
    End Property

    ''' <summary>
    ''' Get ``hmdb`` metabolite data
    ''' </summary>
    ''' <param name="id$"></param>
    ''' <returns></returns>
    Default Public Overloads ReadOnly Property GetDATA(id$) As metabolite
        Get
            id = UCase(hmdb2ndMapSolver(id))

            If hmdb.ContainsKey(id) Then
                Return hmdb(id)
            Else
                Return Nothing
            End If
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="hmdb$">
    ''' 从 http://www.hmdb.ca/downloads 页面所下载的**All Metabolites**数据集
    ''' </param>
    ''' <param name="chebiRepo">
    ''' 包含有两种数据：从chebi下载的在线数据以及从chebi ftp服务器上面所下载的tsv文件的文件夹
    ''' </param>
    Sub New(hmdb$, chebiRepo As (cache$, tsv$))
        Dim getXref = Xref.CreateDictionary(Of metabolite)

        Call "Scaning ChEBI local cache repository...".__DEBUG_ECHO

        For Each xml As String In (ls - l - r - "*.XML" <= chebiRepo.cache)
            Dim entity = xml.LoadXml(Of ChEBIEntity())

            For Each chebiData As ChEBIEntity In entity
                With chebiData
                    Call chebi2ndMapSolver.Add(
                        .chebiId,
                        .SecondaryChEBIIds)

                    chebi(.chebiId) = chebiData
                End With
            Next
        Next

        Call "Indexing of the HMDB data...".__DEBUG_ECHO

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
                Call Me.hmdb.Add(m)

                If Not .metlin_id.StringEmpty AndAlso
                    Not metlin2Hmdb.ContainsKey(.metlin_id) Then
                    Call metlin2Hmdb.Add(.metlin_id, .accession)
                End If
                If Not .cas_registry_number.StringEmpty AndAlso
                    Not CAS2hmdb.ContainsKey(.cas_registry_number) Then
                    Call CAS2hmdb.Add(.cas_registry_number, .accession)
                End If
            End With
        Next
    End Sub

    Public Function GetHMDBFromChEBI(chebi$) As metabolite
        Dim chebiData As ChEBIEntity = GetDATA(CLng(Val(chebi.Split(":"c).Last)))

        If chebiData Is Nothing Then
            Return Nothing
        End If

        Dim hmdbID = chebiData.DatabaseLinks _
            .Where(Function(link) link.type = DatabaseLinks.HMDB_accession) _
            .FirstOrDefault _
           ?.data

        If hmdbID.StringEmpty Then
            Return Nothing
        Else
            Return GetDATA(hmdbID)
        End If
    End Function

    ''' <summary>
    ''' 从hmdb编号mapping到chebi的主编号
    ''' </summary>
    ''' <param name="hmdb"></param>
    ''' <returns></returns>
    Public Function HMDB2ChEBI(hmdb$) As String
        Dim id = UCase(hmdb2ndMapSolver(hmdb))

        If Not hmdbXrefs.ContainsKey(id) Then
            Return Nothing
        Else
            hmdb = id
        End If

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

    Public Function CAS_toHMDB(cas$) As String
        If CAS2hmdb.ContainsKey(cas) Then
            Return CAS2hmdb(cas)
        Else
            Return Nothing
        End If
    End Function

    Public Function CAS2ChEBI(cas$) As String
        If CAS2hmdb.ContainsKey(cas) Then
            Return HMDB2ChEBI(CAS2hmdb(cas))
        Else
            Return Nothing
        End If
    End Function
End Class
