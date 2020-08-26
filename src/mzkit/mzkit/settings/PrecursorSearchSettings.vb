Imports System.ComponentModel
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

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

    Public Property elements As Dictionary(Of String, IntRange)

    Public Function CreateOptions() As SearchOption
        Dim opts = New SearchOption(-99999, 99999, 5)

        For Each element In elements
            opts.AddElement(element.Key, element.Value.Min, element.Value.Max)
        Next

        Return opts
    End Function
End Class