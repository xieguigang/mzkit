#Region "Microsoft.VisualBasic::6c08be7bf370b62684dac068073d1134, G:/mzkit/src/mzmath/Mummichog//Models/KEGG.vb"

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

    '   Total Lines: 120
    '    Code Lines: 84
    ' Comment Lines: 18
    '   Blank Lines: 18
    '     File Size: 4.67 KB


    ' Class MapGraphPopulator
    ' 
    ' 
    ' 
    ' Class DefaultMapGraphPopulator
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: CreateGraphModel
    ' 
    ' Module KEGG
    ' 
    '     Function: (+2 Overloads) CreateBackground, graphModel
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Public MustInherit Class MapGraphPopulator

    Public MustOverride Function CreateGraphModel(map As Map) As NetworkGraph

End Class

Friend Class DefaultMapGraphPopulator : Inherits MapGraphPopulator

    ReadOnly reactions As Dictionary(Of String, Reaction)

    Sub New(reactions As Dictionary(Of String, Reaction))
        Me.reactions = reactions
    End Sub

    Public Overrides Function CreateGraphModel(map As Map) As NetworkGraph
        Return map.graphModel(reactions)
    End Function
End Class

''' <summary>
''' create background network graph model for kegg data
''' </summary>
Public Module KEGG

    <Extension>
    Public Iterator Function CreateBackground(pathways As IEnumerable(Of Map), populator As MapGraphPopulator) As IEnumerable(Of NamedValue(Of NetworkGraph))
        For Each map As Map In From pwy In pathways Where Not pwy Is Nothing
            Dim model As NetworkGraph = populator.CreateGraphModel(map)
            Dim referId As String = If(map.EntryId.IsPattern("\d+"), $"map{map.EntryId}", map.EntryId)
            Dim name As String = map.name _
                .Replace("Reference pathway", "") _
                .Trim(" "c, "-"c)

            Yield New NamedValue(Of NetworkGraph) With {
                .Name = referId,
                .Description = name,
                .Value = model
            }
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function CreateBackground(pathways As IEnumerable(Of Map), reactions As Dictionary(Of String, Reaction)) As IEnumerable(Of NamedValue(Of NetworkGraph))
        Return pathways.CreateBackground(New DefaultMapGraphPopulator(reactions))
    End Function

    ''' <summary>
    ''' convert the kegg map object to the graph model
    ''' </summary>
    ''' <param name="map"></param>
    ''' <param name="reactions"></param>
    ''' <returns>
    ''' the data graph model contains the metabolite item
    ''' which is connected via the reaction liks and a set
    ''' of the single metabolites which is currently no 
    ''' partner connections in this graph model.
    ''' </returns>
    <Extension>
    Friend Function graphModel(map As Map, reactions As Dictionary(Of String, Reaction)) As NetworkGraph
        Dim allShapes As String() = map.shapes.mapdata _
            .Select(Function(a) a.IDVector) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim reactionIds As String() = allShapes _
            .Where(Function(id)
                       Return id.IsPattern("R\d{5}")
                   End Function) _
            .ToArray
        Dim model As New NetworkGraph

        ' add connected graph
        ' via the reaction links
        For Each id As String In reactionIds.Where(AddressOf reactions.ContainsKey)
            Dim reaction As Reaction = reactions(id)
            Dim formula As Equation = reaction.ReactionModel

            Call model.CreateNode(id, New NodeData With {
                .Properties = New Dictionary(Of String, String) From {
                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "reaction"}
                }
            })

            For Each cid As String In formula.GetMetabolites.Select(Function(c) c.ID)
                If model.GetElementByID(cid) Is Nothing Then
                    Call model.CreateNode(cid, New NodeData)
                End If
            Next

            For Each substrate As CompoundSpecieReference In formula.Reactants
                If model.QueryEdge(substrate.ID, id) Is Nothing Then
                    model.CreateEdge(
                        u:=model.GetElementByID(substrate.ID),
                        v:=model.GetElementByID(id)
                    )
                End If
            Next
        Next

        ' add single node into this graph object
        ' via loop through all shape id
        For Each id As String In allShapes
            If model.GetElementByID(id) Is Nothing Then
                model.CreateNode(id, New NodeData)
            End If
        Next

        Return model
    End Function
End Module
