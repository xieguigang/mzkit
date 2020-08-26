Imports System.ComponentModel
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Public Class PrecursorSearchSettings

    Public Property ppm As Double
    Public Property precursor_types As String()

End Class

Public Enum FormulaSearchProfiles
    <Description("Custom_Profile")> Custom
    <Description("Default_Profile")> [Default]
    <Description("Small_Molecule")> SmallMolecule
    <Description("Natural_Product")> NaturalProduct
End Enum

Public Class FormulaSearchProfile

    Public Function CreateOptions() As SearchOption

    End Function
End Class