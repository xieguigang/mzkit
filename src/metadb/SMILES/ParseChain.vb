Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Linq

Public Class ParseChain

    ReadOnly graph As New ChemicalFormula
    ReadOnly chainStack As New Stack(Of ChemicalElement)
    ReadOnly SMILES As String
    ReadOnly tokens As Token()

    Sub New(tokens As IEnumerable(Of Token))
        Me.tokens = tokens.ToArray
        Me.SMILES = Me.tokens _
            .Select(Function(t) t.text) _
            .JoinBy("")
    End Sub

    Public Shared Function ParseGraph(SMILES As String) As ChemicalFormula
        Dim tokens As Token() = New Scanner(SMILES).GetTokens().ToArray
        Dim graph As ChemicalFormula = New ParseChain(tokens).CreateGraph
        Dim degree = graph.AllBonds.DoCall(AddressOf Network.ComputeDegreeData(Of ChemicalElement, ChemicalKey))

        For Each element As ChemicalElement In graph.AllElements
            element.degree = (degree.in.TryGetValue(element.label), degree.out.TryGetValue(element.label))
        Next

        Return graph
    End Function

    Public Function CreateGraph() As ChemicalFormula
        For Each t As Token In tokens
            Call WalkToken(t)
        Next

        Return graph
    End Function

    Private Sub WalkToken(t As Token)
        If t.name = ElementTypes.Element Then
            Dim element As New ChemicalElement(t.text)

            Call graph.AddVertex(element)

            If chainStack.Count > 0 Then
                Dim lastElement As ChemicalElement = chainStack.Peek
                ' 默认为单键
                Dim bond As New ChemicalKey With {
                    .U = lastElement,
                    .V = element,
                    .weight = 1,
                    .Bond = Bonds.single
                }

                Call graph.AddBond(bond)
            End If

            Call chainStack.Push(element)
        End If
    End Sub

    Public Overrides Function ToString() As String
        Return SMILES
    End Function

End Class
