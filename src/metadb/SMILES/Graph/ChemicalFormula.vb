Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports EmpiricalFormula = BioNovoGene.BioDeep.Chemoinformatics.Formula.Formula

Public Class ChemicalFormula : Inherits NetworkGraph(Of ChemicalElement, ChemicalKey)

    Public Function GetFormula() As EmpiricalFormula

    End Function
End Class
