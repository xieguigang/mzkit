Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection

Public Class FormulaBuilder

    ReadOnly graph As ChemicalFormula

    Dim empirical As New StringBuilder
    Dim composition As New Dictionary(Of String, Integer)
    Dim visited As New Index(Of String)
    Dim atomProfile As Dictionary(Of String, Atom)

    Sub New(graph As ChemicalFormula)
        Me.graph = graph
        Me.atomProfile = Atom _
            .DefaultElements _
            .ToDictionary(Function(a) a.label)
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
            Case "H" : Call Push("H")
            Case Else

                If atomProfile.ContainsKey(element.elementName) Then
                    Call Push(atomProfile(element.elementName), element, bond)
                Else
                    Throw New NotImplementedException(element.elementName)
                End If
        End Select
    End Sub

    Private Sub Push(atom As Atom, element As ChemicalElement, bond As Integer)
        Call Push(atom.label)
        Call Push("H", atom.maxKeys - element.Keys - bond)
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
