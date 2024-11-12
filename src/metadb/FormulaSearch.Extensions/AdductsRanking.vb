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

    Private Function GetAdductsFormula(adduct As MzCalculator) As (sign%, formula As Formula)()
        Dim adduct_str As String = adduct.ToString

        Static formulaCache As New Dictionary(Of String, (sign%, formula As Formula)())

        SyncLock formulaCache
            If formulaCache.ContainsKey(adduct_str) Then
                Return formulaCache(adduct_str)
            End If
        End SyncLock

        Dim tokens = Parser.Formula(adduct_str, raw:=True)

        If tokens Like GetType(String) Then
            Throw New InvalidProgramException(tokens.TryCast(Of String))
        End If

        Dim formulaParts = tokens _
            .TryCast(Of (sign%, expression As String)()) _
            .Select(Function(part)
                        Dim multiply = ExactMass.Mul(part.expression)
                        Dim f = FormulaScanner.ScanFormula(multiply.Name)

                        Return (part.sign, f * multiply.Value)
                    End Function) _
            .ToArray

        SyncLock formulaCache
            formulaCache(adduct_str) = formulaParts
        End SyncLock

        Return formulaParts
    End Function

    Private Function RankPositive(formula As Formula, adduct As MzCalculator) As Double
        Dim mz As Double = adduct.CalcMZ(formula.ExactMass)

        If mz <= 0 Then
            Return 0
        End If

        Dim adduct_parts = GetAdductsFormula(adduct)

        Throw New NotImplementedException
    End Function

    Private Function RankNegative(formula As Formula, adduct As MzCalculator) As Double
        Dim mz As Double = adduct.CalcMZ(formula.ExactMass)

        If mz <= 0 Then
            Return 0
        End If

        Dim adduct_parts = GetAdductsFormula(adduct)


    End Function
End Class
