Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports EmpiricalFormula = BioNovoGene.BioDeep.Chemoinformatics.Formula.Formula

Public Class ChemicalFormula : Inherits NetworkGraph(Of ChemicalElement, ChemicalKey)

    Public ReadOnly Property AllBonds As IEnumerable(Of ChemicalKey)
        Get
            Return edges.Values
        End Get
    End Property

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
        Return New EmpiricalFormula(New FormulaBuilder(Me).GetComposition)
    End Function

    Public Overrides Function ToString() As String
        Return GetFormula.ToString
    End Function
End Class
