
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Public Class PrecursorIonComposition : Inherits FormulaComposition

    Public Property precursor_type As String
    Public Property adducts As Double
    Public Property M As Integer

    Public Sub New(counts As IDictionary(Of String, Integer), Optional formula As String = Nothing)
        MyBase.New(counts, formula)
    End Sub
End Class