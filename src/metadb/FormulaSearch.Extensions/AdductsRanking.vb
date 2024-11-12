Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

''' <summary>
''' A helper tools for make adducts ions ranking
''' </summary>
Public Class AdductsRanking

    ReadOnly ion As IonModes

    Sub New(ion As IonModes)
        Me.ion = ion
    End Sub

    Public Function Rank(formula_str As String, adduct_str As String) As Double
        Dim formula As Formula = FormulaScanner.ScanFormula(formula_str)
        Dim adduct As MzCalculator = Provider.ParseAdductModel(adduct_str)

        If ion = IonModes.Positive Then
            Return RankPositive(formula, adduct)
        Else
            Return RankNegative(formula, adduct)
        End If
    End Function

    Private Function RankPositive(formula As Formula, adduct As MzCalculator) As Double
        Throw New NotImplementedException
    End Function

    Private Function RankNegative(formula As Formula, adduct As MzCalculator) As Double

    End Function
End Class
