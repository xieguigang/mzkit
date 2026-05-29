#Region "Microsoft.VisualBasic::8e503101593a4e311ba0e83f037320da, metadb\SMILES\ParseChain.vb"

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

    '   Total Lines: 203
    '    Code Lines: 151 (74.38%)
    ' Comment Lines: 18 (8.87%)
    '    - Xml Docs: 55.56%
    ' 
    '   Blank Lines: 34 (16.75%)
    '     File Size: 7.05 KB


    ' Class ParseChain
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: CreateGraph, ParseGraph, ToString
    ' 
    '     Sub: WalkElement, WalkKey, WalkToken
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES.Language
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Class ParseChain

    ReadOnly graph As New ChemicalFormula
    ReadOnly chainStack As New Stack(Of ChemicalElement)
    ReadOnly stackSize As New Stack(Of Counter)
    ReadOnly SMILES As String
    ReadOnly tokens As Token()
    ReadOnly rings As New Dictionary(Of String, ChemicalElement)

    Dim lastKey As Bonds?

    ''' <summary>
    ''' the chemical graph id, for deal with the SMILES contains multiple independent parts
    ''' </summary>
    ReadOnly gid As Integer

    Sub New(tokens As IEnumerable(Of Token), gid As Integer)
        Me.tokens = tokens.ToArray
        Me.gid = gid
        Me.SMILES = Me.tokens _
            .Select(Function(t)
                        If t.aromatic Then
                            Return t.text.ToLower
                        End If
                        If t.ring Is Nothing Then
                            Return t.text
                        Else
                            Return t.text & t.ring.ToString
                        End If
                    End Function) _
            .JoinBy("")
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="SMILES"></param>
    ''' <param name="strict"></param>
    ''' <returns>
    ''' this function may returns nothing if the given smiles string is invalid and case the parser error when strict is false.
    ''' </returns>
    Public Shared Function ParseGraph(SMILES As String, Optional strict As Boolean = True) As ChemicalFormula
        Dim tokens As New List(Of Token())

        ' 20240820
        ' A.B
        ' A and B are the independent parts
        ' the input smiles string needs split these independent parts at first

        Try
            For Each part As String In SMILES.Split("."c)
                Call tokens.Add(New Scanner(part).GetTokens().ToArray)
            Next
        Catch ex As Exception
            If strict Then
                Throw New Exception("SMILES string for parse:" & SMILES, ex)
            Else
                Call $"SMILES string for parse: {SMILES}".Warning
                Call App.LogException(New Exception("SMILES string for parse:" & SMILES, ex))

                Return Nothing
            End If
        End Try

        Dim graph As ChemicalFormula = Nothing
        Dim append As ChemicalFormula
        Dim gid As i32 = 1

        For Each part As Token() In tokens
            If graph Is Nothing Then
                graph = New ParseChain(part, ++gid).CreateGraph(strict)
            Else
                append = New ParseChain(part, ++gid).CreateGraph(strict)

                If append Is Nothing OrElse Not append.vertex.Any Then
                    Continue For
                Else
                    graph = graph.Join(append)
                End If
            End If
        Next

        Dim degree As DegreeData = graph _
            .AllBonds _
            .DoCall(AddressOf Network.ComputeDegreeData(Of ChemicalElement, ChemicalKey))

        For Each element As ChemicalElement In graph.AllElements
            element.degree = New NodeDegree(
                degree.In.TryGetValue(element.label),
                degree.Out.TryGetValue(element.label)
            )
        Next

        Return graph
    End Function

    Public Function CreateGraph(Optional strict As Boolean = True) As ChemicalFormula
        Dim i As i32 = 1

        graph.id = gid

        For Each t As Token In tokens
            Call WalkToken(t, ++i)
        Next

        Call ChemicalElement.SetAtomGroups(formula:=graph)

        Return graph.AutoLayout(strict:=strict)
    End Function

    Private Sub WalkToken(t As Token, i As Integer)
        Select Case t.name
            Case ElementTypes.Element, ElementTypes.AtomGroup : Call WalkElement(t, i)
            Case ElementTypes.Key : Call WalkKey(t)
            Case ElementTypes.Open
                ' do nothing
                stackSize.Push(0)
            Case ElementTypes.Close
                Call chainStack.Pop(stackSize.Pop())
            Case ElementTypes.Disconnected, ElementTypes.None, ElementTypes.Isomers
                ' unsure how to break the graph, do nothing?
            Case Else
                Throw New NotImplementedException($"Unknown element type for build structure graph: ({t.name.ToString})" & t.ToString)
        End Select
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Sub WalkKey(t As Token)
        lastKey = CType(CByte(ChemicalBonds.IndexOf(t.text)), Bonds)
    End Sub

    Private Sub WalkElement(t As Token, i As Integer)
        Dim element As ChemicalElement
        Dim ringId As String = If(t.ring Is Nothing, Nothing, t.ring.ToString)

        If t.name = ElementTypes.AtomGroup Then
            element = New ChemicalElement("[" & t.text & "]", index:=i)

            If AtomGroup.CheckDefaultLabel(t.text) AndAlso AtomGroup.AtomGroups(t.text).CheckSingleValence Then
                element.charge = AtomGroup.GetDefaultValence(t.text, -1)
            End If
        Else
            element = New ChemicalElement(t.text, index:=i) With {
                .charge = If(t.charge Is Nothing, If(t.aromatic, 2, 1), Val(t.charge))
            }
        End If

        element.graph_id = gid
        element.aromatic = t.aromatic
        element.ID = graph.vertex.Count + 1

        Call graph.AddVertex(element)

        If Not ringId Is Nothing Then
            If rings.ContainsKey(ringId) Then
                Dim [next] = rings(ringId)
                Dim bond As New ChemicalKey With {
                    .U = [next],
                    .V = element,
                    .weight = 1,
                    .bond = Bonds.single
                }

                ' 20260223 avoid of the possible duplicated bond?
                If Not graph.ExistEdge(bond) Then
                    Call graph.Insert(bond)
                End If
            Else
                rings(ringId) = element
            End If
        End If

        If chainStack.Count > 0 Then
            Dim lastElement As ChemicalElement = chainStack.Peek
            Dim bondType As Bonds = If(lastKey Is Nothing, Bonds.single, lastKey.Value)
            ' 默认为单键
            Dim bond As New ChemicalKey With {
                .U = lastElement,
                .V = element,
                .weight = 1,
                .bond = bondType
            }

            ' 20260223 avoid of the possible duplicated bond?
            If Not graph.ExistEdge(bond) Then
                Call graph.Insert(bond)
            End If
        End If
        If stackSize.Count > 0 Then
            Call stackSize.Peek.Hit()
        End If

        lastKey = Nothing
        chainStack.Push(element)
    End Sub

    Public Overrides Function ToString() As String
        Return SMILES
    End Function

End Class
