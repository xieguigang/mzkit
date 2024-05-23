#Region "Microsoft.VisualBasic::4bb2bf4f62f9bdb410b0535a49a7741f, metadb\SMILES\Embedding\Embedding.vb"

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

    '   Total Lines: 94
    '    Code Lines: 80 (85.11%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 14 (14.89%)
    '     File Size: 3.62 KB


    '     Module Embedding
    ' 
    '         Function: GetAtomTable, GraphEmbedding
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
        Public Iterator Function GraphEmbedding(g As ChemicalFormula,
                                                Optional kappa As Double = 2,
                                                Optional normalizeSize As Boolean = False) As IEnumerable(Of AtomLink)
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
                Dim keys As New Vector(group.Select(Function(a) CDbl(a.bond)))

                v0.Array(i) = group.Count
                vk.Array(i) = (-kappa * keys).Exp().Sum
            Next

            If normalizeSize Then
                v0 /= g.AllElements.Count
            End If

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
                    .vertex = gv,
                    .vk = vk(i),
                    .v0 = v0(i)
                }
            Next
        End Function
    End Module
End Namespace
