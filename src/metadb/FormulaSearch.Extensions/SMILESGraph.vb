Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Public Module SMILESGraph

    ''' <summary>
    ''' convert the smiles chemical formula graph data to a network graph object
    ''' </summary>
    ''' <param name="f"></param>
    ''' <returns></returns>
    <Extension>
    Public Function AsGraph(f As ChemicalFormula) As NetworkGraph
        Dim g As New NetworkGraph

        For Each atom As ChemicalElement In f.AllElements
            Call g.CreateNode(atom.label, New NodeData With {
                .label = atom.label,
                .origID = atom.label,
                .Properties = New Dictionary(Of String, String) From {
                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, atom.group}
                }
            })
        Next

        For Each key As ChemicalKey In f.AllBonds
            Call g.CreateEdge(key.U.label, key.V.label, weight:=1, data:=New EdgeData With {
                .label = key.ID,
                .Properties = New Dictionary(Of String, String)
            })
        Next

        Return g
    End Function

End Module
