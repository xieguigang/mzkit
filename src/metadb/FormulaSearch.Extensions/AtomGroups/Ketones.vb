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

End Class
