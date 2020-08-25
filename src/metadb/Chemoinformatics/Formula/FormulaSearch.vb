Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Public Class FormulaSearch

    ReadOnly opts As SearchOption

    Sub New(opts As SearchOption)
        Me.opts = opts
    End Sub

    Public Function SearchByExactMass(exact_mass As Double) As FormulaComposition

    End Function
End Class
