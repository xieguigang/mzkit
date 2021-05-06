Imports Microsoft.VisualBasic.ComponentModel.Collection

Public Class FormulaBuilder

    ReadOnly graph As ChemicalFormula

    Dim composition As New Dictionary(Of String, Integer)
    Dim visited As New Index(Of String)

    Sub New(graph As ChemicalFormula)
        Me.graph = graph
    End Sub

    Public Function GetComposition() As Dictionary(Of String, Integer)
        For Each bond As ChemicalKey In graph.AllBonds
            Call WalkElement(bond.U)
            Call WalkElement(bond.V)
        Next

        Return composition
    End Function

    Private Sub WalkElement(element As ChemicalElement)
        If Not element.label Like visited Then
            visited += element.label
        Else
            Return
        End If

        Select Case element.elementName
            Case "C" : Call Push("C")
            Case "H" : Call Push("H")
            Case Else
                Throw New NotImplementedException(element.elementName)
        End Select
    End Sub

    Private Sub Push(element As String)
        If Not composition.ContainsKey(element) Then
            composition.Add(element, 0)
        End If

        composition(element) += 1
    End Sub
End Class
