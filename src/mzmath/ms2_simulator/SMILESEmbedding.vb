Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES.Embedding
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

Public Module SMILESEmbedding

    ''' <summary>
    ''' Generates the molecule structure documents
    ''' </summary>
    ''' <param name="mols"></param>
    ''' <returns></returns>
    Public Function StructureDocuments(mols As IEnumerable(Of NamedValue(Of ChemicalFormula))) As IEnumerable(Of NamedCollection(Of String))
        Dim documents As New Dictionary(Of String, List(Of String))

        For Each smiles As NamedValue(Of ChemicalFormula) In mols
            For Each atom_group In smiles.Value.GetAtomTable
                If Not documents.ContainsKey(atom_group.group) Then
                    documents(atom_group.group) = New List(Of String)
                End If

                Call documents(atom_group.group).Add(smiles.Name)
            Next
        Next

        Return documents.BuildDocuments
    End Function

    <Extension>
    Private Iterator Function BuildDocuments(tokens As Dictionary(Of String, List(Of String))) As IEnumerable(Of NamedCollection(Of String))
        For Each atom_group In tokens
            Dim mols = atom_group.Value _
                .GroupBy(Function(id) id) _
                .OrderByDescending(Function(id) id.Count) _
                .IteratesALL _
                .ToArray

            Yield New NamedCollection(Of String)(atom_group.Key, mols)
        Next
    End Function

End Module
