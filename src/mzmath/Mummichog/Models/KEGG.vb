Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

''' <summary>
''' create background network graph model for kegg data
''' </summary>
Public Module KEGG

    <Extension>
    Public Iterator Function CreateBackground(pathways As IEnumerable(Of Map), reactions As Dictionary(Of String, Reaction)) As IEnumerable(Of NamedValue(Of NetworkGraph))
        For Each map As Map In pathways
            Dim allShapes = map.shapes _
                .Select(Function(a) a.IDVector) _
                .IteratesALL _
                .Distinct _
                .ToArray
            Dim reactionIds As String() = allShapes.Where(Function(id) id.IsPattern("R\d{5}")).ToArray
            Dim model As New NetworkGraph

            For Each id As String In reactionIds
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

            Yield New NamedValue(Of NetworkGraph) With {
                .Name = map.Name,
                .Description = map.description,
                .Value = model
            }
        Next
    End Function
End Module
