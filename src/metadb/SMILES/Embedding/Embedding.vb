Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra

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

        <Extension>
        Public Iterator Function GraphEmbedding(g As ChemicalFormula, Optional kappa As Double = 1) As IEnumerable(Of AtomLink)
            Dim links = g.AllBonds _
                .GroupBy(Function(l)
                             Return New String() {l.U.group, l.V.group} _
                                .OrderBy(Function(v) v) _
                                .JoinBy("|")
                         End Function) _
                .ToArray
            Dim v0 As New Vector(links.Length)
            Dim vk As New Vector(links.Length)

            For i As Integer = 0 To links.Length - 1
                Dim group As IGrouping(Of String, ChemicalKey) = links(i)

                v0.Array(i) = group.Count
                vk.Array(i) = (-kappa * New Vector(group.Select(Function(a) CDbl(a.bond)))).Exp().Sum
            Next

            Dim sgv As Double() = (vk / v0) ^ (1 / kappa)

            For i As Integer = 0 To links.Length - 1
                Dim t As String() = links(i).Key.Split("|"c)
                Dim gv As New Dictionary(Of String, String())

                For Each v As ChemicalElement In links(i) _
                    .Select(Function(l) {l.U, l.V}) _
                    .IteratesALL _
                    .GroupBy(Function(vi) vi.label) _
                    .Select(Function(vg) vg.First)

                    gv(v.label) = ChemicalElement _
                        .GetConnection(g, v) _
                        .Select(Function(a)
                                    Return $"{CInt(a.keys)}({a.Item2.group})"
                                End Function) _
                        .ToArray
                Next

                Yield New AtomLink With {
                    .atom1 = t(0),
                    .atom2 = t(1),
                    .score = sgv(i),
                    .vertex = gv
                }
            Next
        End Function
    End Module
End Namespace