Imports System.Runtime.CompilerServices

Namespace Embedding

    <HideModuleName>
    Public Module Embedding

        <Extension>
        Public Iterator Function GetAtomTable(g As ChemicalFormula) As IEnumerable(Of SmilesAtom)
            Dim elements As ChemicalElement() = g.AllElements _
                .OrderBy(Function(a) a.label.Match("\d+").ParseInteger) _
                .ToArray

            For i As Integer = 0 To elements.Length - 1
                Dim e As ChemicalElement = elements(i)
                Dim atom As New SmilesAtom With {
                    .atom = e.elementName,
                    .group = e.group,
                    .links = e.Keys,
                    .id = e.label,
                    .ion_charge = e.charge,
                    .connected = ChemicalElement _
                        .GetConnection(g, e) _
                        .Select(Function(a)
                                    Return $"{CInt(a.keys)}({a.Item2.group})"
                                End Function) _
                        .ToArray
                }

                Yield atom
            Next
        End Function
    End Module
End Namespace