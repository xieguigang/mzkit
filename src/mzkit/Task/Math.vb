Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Public Module Math

    Public Function EvaluateFormula(formula As String) As Double
        Dim composition As FormulaComposition = FormulaScanner.ScanFormula(formula)
        Dim exact_mass As Double = Aggregate atom
                                   In composition.CountsByElement
                                   Let eval As Double = ExactMass.Eval(atom.Key) * atom.Value
                                   Into Sum(eval)
        Return exact_mass
    End Function
End Module
