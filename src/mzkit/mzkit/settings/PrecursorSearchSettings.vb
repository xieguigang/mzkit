Imports System.ComponentModel
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Public Class PrecursorSearchSettings

    Public Property ppm As Double
    Public Property precursor_types As String()

End Class

Public Enum FormulaSearchProfiles
    Custom
    [Default]
    <Description("Small Molecule")> SmallMolecule
    <Description("Natural Product")> NaturalProduct
End Enum

Public Class FormulaSearchProfile

    Public Function CreateOptions() As SearchOption

    End Function
End Class