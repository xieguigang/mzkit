
Namespace NCBI.PubChem.Graph

    Public Class GraphId
        Public Property CID As String
        Public Property MeSH As String
        Public Property GeneSymbol As String
    End Class

    Public Class Evidence

        Public Property ChemicalDiseaseNeighbor As PubChemGraph
        Public Property ChemicalGeneSymbolNeighbor As PubChemGraph
        Public Property ChemicalNeighbor As PubChemGraph

    End Class

    ''' <summary>
    ''' graph [id1 -> id2] with evidences
    ''' </summary>
    Public Class MeshGraph

        Public Property ID_1 As GraphId
        Public Property ID_2 As GraphId
        Public Property Evidence As Evidence

    End Class

    Public Class PubChemGraph

        Public Property NeighborName As String
        Public Property OrderingByCooccurrenceScore As Boolean
        Public Property QueryArticleCount As Integer
        Public Property NeighborArticleCount As Integer
        Public Property TotalArticleCount As Integer
        Public Property EffectiveTotalArticleCount As Integer
        Public Property ArticleCount As Integer
        Public Property CooccurrenceScore As Integer
        Public Property Article As Article()

    End Class

    Public Class Article
        Public Property GenericArticleId As Integer
        Public Property RelevanceScore As Integer
        Public Property PMID As  Integer
        Public Property DOI As String
        Public Property PublicationDate As  String
        Public Property IsReview As Boolean
        Public Property Title as String
        Public Property Author As String
        Public Property Journal As  String
        Public Property Citation As String
        Public Property ChemicalName As  String
        Public Property DiseaseName As String
        Public Property GeneSymbolName As String
    End Class
End Namespace