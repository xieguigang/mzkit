Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports EmpiricalFormula = BioNovoGene.BioDeep.Chemoinformatics.Formula.Formula

Public Class ChemicalFormula : Inherits NetworkGraph(Of ChemicalElement, ChemicalKey)

    ''' <summary>
    ''' <see cref="AddEdge"/> without add elements
    ''' </summary>
    ''' <param name="bond">
    ''' the elements has already been added.
    ''' </param>
    Friend Sub AddBond(bond As ChemicalKey)
        Call edges.Add(bond)
    End Sub

    Public Function GetFormula() As EmpiricalFormula
        Dim composition As New Dictionary(Of String, Integer)

        For Each bond As ChemicalKey In edges.Values

        Next

        Return New EmpiricalFormula(composition)
    End Function

    Public Overrides Function ToString() As String
        Return GetFormula.ToString
    End Function
End Class
