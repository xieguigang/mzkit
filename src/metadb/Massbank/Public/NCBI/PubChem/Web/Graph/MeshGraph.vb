#Region "Microsoft.VisualBasic::a0e38ad12f630a8597a3631cf52d9ae4, mzkit\src\metadb\Massbank\Public\NCBI\PubChem\Web\Graph\MeshGraph.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
' 
' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' MIT License
' 
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 93
'    Code Lines: 74
' Comment Lines: 3
'   Blank Lines: 16
'     File Size: 3.00 KB


'     Class GraphId
' 
'         Properties: CID, GeneSymbol, GraphId, MeSH
' 
'         Function: ToString
' 
'     Class Evidence
' 
'         Properties: ChemicalDiseaseNeighbor, ChemicalGeneSymbolNeighbor, ChemicalNeighbor, Graph
' 
'     Class MeshGraph
' 
'         Properties: Evidence, ID_1, ID_2
' 
'     Class PubChemGraph
' 
'         Properties: Article, ArticleCount, CooccurrenceScore, EffectiveTotalArticleCount, NeighborArticleCount
'                     NeighborName, OrderingByCooccurrenceScore, QueryArticleCount, TotalArticleCount
' 
'     Class Article
' 
'         Properties: Author, ChemicalName, ChemicalName_1, ChemicalName_2, Citation
'                     DiseaseName, DOI, GenericArticleId, GeneSymbolName, IsReview
'                     Journal, PMID, PublicationDate, RelevanceScore, Title
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace NCBI.PubChem.Graph

    Public Class GraphId

        Public Property CID As String
        Public Property MeSH As String
        Public Property GeneSymbol As String

        ''' <summary>
        ''' a union method for get the current id value:
        ''' 
        ''' + <see cref="CID"/>
        ''' + <see cref="MeSH"/>
        ''' + <see cref="GeneSymbol"/>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <DataIgnored>
        Public ReadOnly Property GraphId As String
            Get
                If Not CID.StringEmpty Then
                    Return CID
                ElseIf Not MeSH.StringEmpty Then
                    Return MeSH
                ElseIf Not GeneSymbol.StringEmpty Then
                    Return GeneSymbol
                Else
                    Return "n/a"
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return GraphId
        End Function
    End Class

    ''' <summary>
    ''' evidence data for the knowledge graph link
    ''' </summary>
    Public Class Evidence

        Public Property ChemicalDiseaseNeighbor As PubChemGraph
        Public Property ChemicalGeneSymbolNeighbor As PubChemGraph
        Public Property ChemicalNeighbor As PubChemGraph

        ''' <summary>
        ''' a union method for get one of the evidence graph data for current knowledg graph:
        ''' 
        ''' + <see cref="ChemicalDiseaseNeighbor"/>
        ''' + <see cref="ChemicalGeneSymbolNeighbor"/>
        ''' + <see cref="ChemicalNeighbor"/>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        <DataIgnored>
        Public ReadOnly Property Graph As PubChemGraph
            Get
                If Not ChemicalDiseaseNeighbor Is Nothing Then
                    Return ChemicalDiseaseNeighbor
                ElseIf Not ChemicalGeneSymbolNeighbor Is Nothing Then
                    Return ChemicalGeneSymbolNeighbor
                ElseIf Not ChemicalNeighbor Is Nothing Then
                    Return ChemicalNeighbor
                Else
                    Return Nothing
                End If
            End Get
        End Property

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
        Public Property PMID As Integer
        Public Property DOI As String
        Public Property PublicationDate As String
        Public Property IsReview As Boolean
        Public Property Title As String
        Public Property Author As String
        Public Property Journal As String
        Public Property Citation As String
        Public Property ChemicalName As String
        Public Property DiseaseName As String
        Public Property GeneSymbolName As String
        Public Property ChemicalName_1 As String
        Public Property ChemicalName_2 As String

    End Class
End Namespace
