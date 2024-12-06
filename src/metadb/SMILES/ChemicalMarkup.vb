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

        For Each element As ChemicalElement In graph.AllElements
            Call atoms.Add(New ChemicalMarkupLanguage.atom With {
                .elementType = element.elementName,
                .hydrogenCount = element.hydrogen,
                .id = element.label
            })
        Next

        For Each key As ChemicalKey In graph.AllBonds
            Call bonds.Add(New bond With {
                .id = key.ID,
                .order = 1,
                .atomRefs2 = {key.U.label, key.V.label}
            })
        Next

        data.atomArray = New atomArray("atoms", atoms)
        data.bondArray = New bondArray("bonds", bonds)

        Return New MarkupFile With {
            .molecule = data,
            .title = graph.name
        }
    End Function

End Module
