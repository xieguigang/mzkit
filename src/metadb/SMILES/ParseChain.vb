Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Linq

Public Class ParseChain

    ReadOnly graph As New ChemicalFormula
    ReadOnly chainStack As New Stack(Of ChemicalElement)
    ReadOnly SMILES As String
    ReadOnly tokens As Token()

    Dim lastKey As Bonds?

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
        Select Case t.name
            Case ElementTypes.Element : Call WalkElement(t)
            Case ElementTypes.Key : Call WalkKey(t)
            Case Else
                Throw New NotImplementedException(t.ToString)
        End Select
    End Sub

    Private Sub WalkKey(t As Token)
        lastKey = CType(CByte(ChemicalBonds.IndexOf(t.text)), Bonds)
    End Sub

    Private Sub WalkElement(t As Token)
        Dim element As New ChemicalElement(t.text)

        Call graph.AddVertex(element)

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

            Call graph.AddBond(bond)
        End If

        lastKey = Nothing
        chainStack.Push(element)
    End Sub

    Public Overrides Function ToString() As String
        Return SMILES
    End Function

End Class
