#Region "Microsoft.VisualBasic::0ba5d7e8848ddbc0fb2909eb3a4b1f8b, metadb\Massbank\Public\NCBI\PubChem\Web\Graph\WebGraph.vb"

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

    '   Total Lines: 130
    '    Code Lines: 94 (72.31%)
    ' Comment Lines: 12 (9.23%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 24 (18.46%)
    '     File Size: 4.94 KB


    '     Enum Types
    ' 
    '         ChemicalDiseaseNeighbor, ChemicalGeneSymbolNeighbor, ChemicalNeighbor, GeneSymbolDiseaseNeighbor
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class LinkDataSet
    ' 
    '         Properties: LinkData
    ' 
    '     Class WebGraph
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: getJSONUrl, parseJSON, (+2 Overloads) Query
    ' 
    '     Class GraphJSON
    ' 
    '         Properties: LinkDataSet
    ' 
    '         Function: GenericEnumerator
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace NCBI.PubChem.Graph

    Public Enum Types

        ''' <summary>
        ''' Metabolite Gene Association
        ''' </summary>
        <Description("Metabolite Gene Association")>
        ChemicalGeneSymbolNeighbor

        ''' <summary>
        ''' Metabolite Disease Association
        ''' </summary>
        <Description("Metabolite Disease Association")>
        ChemicalDiseaseNeighbor

        ''' <summary>
        ''' Metabolite Literature Co-Occurrence
        ''' </summary>
        <Description("Metabolite Literature Co-Occurrence")>
        ChemicalNeighbor

        ''' <summary>
        ''' Gene Disease Association
        ''' </summary>
        <Description("Gene Disease Association")>
        GeneSymbolDiseaseNeighbor
    End Enum

    Public Class LinkDataSet

        Public Property LinkData As MeshGraph()

    End Class

    Public Class WebGraph : Inherits WebQuery(Of (cid As String, type As Types))

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub New(<CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False
        )
            MyBase.New(
                url:=AddressOf getJSONUrl,
                contextGuid:=Function(q) $"{q.type}_{q.cid}",
                parser:=AddressOf parseJSON,
                prefix:=Function(q) q.Split("_"c).First & "/" & MD5(q).Substring(1, 2),
                cache:=cache,
                interval:=interval,
                offline:=offline
            )
        End Sub

        Sub New(cache As IFileSystemEnvironment, Optional interval As Integer = -1, Optional offline As Boolean = False)
            MyBase.New(
                url:=AddressOf getJSONUrl,
                contextGuid:=Function(q) $"{q.type}_{q.cid}",
                parser:=AddressOf parseJSON,
                prefix:=Function(q) q.Split("_"c).First & "/" & MD5(q).Substring(1, 2),
                cache:=cache,
                interval:=interval,
                offline:=offline)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function parseJSON(data As String, schema As Type) As Object
            Return data.LoadJSON(Of GraphJSON)(throwEx:=False)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function getJSONUrl(q As (cid As String, type As Types)) As String
            Return $"https://pubchem.ncbi.nlm.nih.gov/link_db/link_db_server.cgi?format=JSON&type={q.type.Description}&operation=GetAllLinks&id_1={q.cid}&response_type=display"
        End Function

        Public Overloads Shared Function Query(cid As String,
                                               Optional type As Types = Types.ChemicalDiseaseNeighbor,
                                               Optional cache As String = "./graph",
                                               Optional interval As Integer = -1,
                                               Optional offline As Boolean = False) As MeshGraph()

            Static web As New Dictionary(Of String, WebGraph)

            Return web _
                .ComputeIfAbsent(
                    key:=cache,
                    lazyValue:=Function()
                                   Return New WebGraph(cache, interval, offline)
                               End Function
                ) _
                .Query(cid, type)
        End Function

        Public Overloads Function Query(cid As String, type As Types) As MeshGraph()
            Dim json As GraphJSON = Query(Of GraphJSON)(context:=(cid, type), cacheType:=".json")

            If json Is Nothing OrElse
                json.LinkDataSet Is Nothing OrElse
                json.LinkDataSet.LinkData Is Nothing Then

                Return Nothing
            Else
                Return json.LinkDataSet.LinkData
            End If
        End Function
    End Class

    Public Class GraphJSON : Implements Enumeration(Of MeshGraph)

        Public Property LinkDataSet As LinkDataSet

        Public Iterator Function GenericEnumerator() As IEnumerator(Of MeshGraph) Implements Enumeration(Of MeshGraph).GenericEnumerator
            If LinkDataSet Is Nothing Then
                Return
            End If

            For Each item As MeshGraph In LinkDataSet.LinkData.SafeQuery
                Yield item
            Next
        End Function
    End Class
End Namespace
