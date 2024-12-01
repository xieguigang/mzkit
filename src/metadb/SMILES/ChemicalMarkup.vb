Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.ChemicalMarkupLanguage

Public Module ChemicalMarkup

    ''' <summary>
    ''' cast the smiles graph as chemical markup language model data
    ''' </summary>
    ''' <param name="graph"></param>
    ''' <returns></returns>
    <Extension>
    Public Function AsCML(graph As ChemicalFormula) As MarkupFile
        Dim data As New molecule With {
            .id = graph.id
        }
        Dim atoms As New List(Of ChemicalMarkupLanguage.atom)
        Dim bonds As New List(Of ChemicalMarkupLanguage.bond)

        data.atomArray = New atomArray("atoms", atoms)
        data.bondArray = New bondArray("bonds", bonds)

        Return New MarkupFile With {
            .molecule = data,
            .title = graph.name
        }
    End Function

End Module
