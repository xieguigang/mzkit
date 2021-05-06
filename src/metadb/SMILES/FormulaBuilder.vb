Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection

Public Class FormulaBuilder

    ReadOnly graph As ChemicalFormula

    Dim empirical As New StringBuilder
    Dim composition As New Dictionary(Of String, Integer)
    Dim visited As New Index(Of String)

    Sub New(graph As ChemicalFormula)
        Me.graph = graph
    End Sub

    Public Function GetComposition(ByRef empirical As String) As Dictionary(Of String, Integer)
        For Each bond As ChemicalKey In graph.AllBonds
            Call WalkElement(bond.U, bond.bond)
            Call WalkElement(bond.V, bond.bond)
        Next

        empirical = Me.empirical.ToString

        Return composition
    End Function

    Private Sub WalkElement(element As ChemicalElement, bond As Bonds)
        If Not element.label Like visited Then
            visited += element.label
        Else
            Return
        End If

        Select Case element.elementName
            Case "C"
                Call Push("C")

                If element.Keys = 1 Then
                    ' X-CH3
                    Call Push("H", 4 - bond)
                ElseIf element.Keys = 2 Then

                    Call Push("H", 2 - bond)

                Else
                    Throw New NotImplementedException
                End If

            Case "H" : Call Push("H")

            Case "O"
                Call Push("O")

                If element.Keys = 1 Then

                    Call Push("H", 2 - bond)

                ElseIf element.Keys = 2 Then
                    ' do nothing
                    ' no additional H element
                Else
                    Throw New InvalidConstraintException
                End If

            Case Else
                Throw New NotImplementedException(element.elementName)
        End Select
    End Sub

    Private Sub Push(element As String, Optional n As Integer = 1)
        If Not composition.ContainsKey(element) Then
            composition.Add(element, 0)
        End If

        If n > 0 Then
            composition(element) += n

            If n = 1 Then
                empirical.Append(element)
            Else
                empirical.Append($"{element}{n}")
            End If
        End If
    End Sub
End Class
