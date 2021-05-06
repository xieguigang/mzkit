Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports EmpiricalFormula = BioNovoGene.BioDeep.Chemoinformatics.Formula.Formula

Public Class ChemicalFormula : Inherits NetworkGraph(Of ChemicalElement, ChemicalKey)

    Public ReadOnly Property AllBonds As IEnumerable(Of ChemicalKey)
        Get
            Return edges.Values
        End Get
    End Property

    Public ReadOnly Property AllElements As IEnumerable(Of ChemicalElement)
        Get
            Return vertex
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

    Public Iterator Function FindKeys(elementkey As String) As IEnumerable(Of ChemicalKey)
        For Each key As ChemicalKey In AllBonds
            If key.U.label = elementkey OrElse key.V.label = elementkey Then
                Yield key
            End If
        Next
    End Function

    Public Function GetFormula() As EmpiricalFormula
        Dim empiricalFormula As String = Nothing
        Dim composition As Dictionary(Of String, Integer) = New FormulaBuilder(Me).GetComposition(empiricalFormula)

        Return New EmpiricalFormula(composition, empiricalFormula)
    End Function

    Public Overrides Function ToString() As String
        Return GetFormula.ToString
    End Function
End Class
