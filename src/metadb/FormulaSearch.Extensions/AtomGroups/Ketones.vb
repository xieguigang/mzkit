Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

''' <summary>
''' 酮基是一个碳原子和氧原子形成双键，同时这个碳原子还和另外两个碳原子形成共价键结构式，可以用R1-(C=O)-R2。
''' 酮基是羰基的一种。酮基能够强烈吸收300nm左右光波的基团，含酮基的高分子容易吸收紫外线而导致光降解。
''' </summary>
Public Class Ketones

    Public Shared ReadOnly Property aldehyde As Formula = FormulaScanner.ScanFormula("COH")
    Public Shared ReadOnly Property ketone As Formula = FormulaScanner.ScanFormula("CO")
    Public Shared ReadOnly Property carboxylic_acid As Formula = FormulaScanner.ScanFormula("COOH")
    Public Shared ReadOnly Property carboxylic_ester As Formula = FormulaScanner.ScanFormula("COO")
    Public Shared ReadOnly Property acid_anhydride As Formula = FormulaScanner.ScanFormula("COOCO")
    Public Shared ReadOnly Property acyl_peroxide As Formula = FormulaScanner.ScanFormula("COOOCO")
    Public Shared ReadOnly Property acid_amides As Formula = FormulaScanner.ScanFormula("CONH2")
    Public Shared ReadOnly Property ketenes As Formula = FormulaScanner.ScanFormula("CHCO")
    Public Shared ReadOnly Property isocyanate As Formula = FormulaScanner.ScanFormula("NCO")

    Public Shared ReadOnly Property acyl_halideF As Formula = FormulaScanner.ScanFormula("COF")
    Public Shared ReadOnly Property acyl_halideCl As Formula = FormulaScanner.ScanFormula("COCl")
    Public Shared ReadOnly Property acyl_halideBr As Formula = FormulaScanner.ScanFormula("COBr")
    Public Shared ReadOnly Property acyl_halideI As Formula = FormulaScanner.ScanFormula("COI")

End Class
