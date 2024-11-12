Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

''' <summary>
''' A helper tools for make adducts ions ranking
''' </summary>
Public Class AdductsRanking

    ReadOnly ion As IonModes

    Const maxValue As Double = 10

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

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="formula_str"></param>
    ''' <param name="adducts"></param>
    ''' <returns>
    ''' the function only populates the valid adducts object and
    ''' sort these adducts object in desc ranking order.
    ''' </returns>
    Public Function RankAdducts(formula_str As String, adducts As IEnumerable(Of MzCalculator)) As IEnumerable(Of MzCalculator)
        Dim formula As Formula = FormulaScanner.ScanFormula(formula_str)
        Dim ranks As IEnumerable(Of (rank As Double, adduct As MzCalculator))

        If ion = IonModes.Positive Then
            ranks = adducts.Select(Function(adduct) (RankPositive(formula, adduct), adduct))
        Else
            ranks = adducts.Select(Function(adduct) (RankNegative(formula, adduct), adduct))
        End If

        Return ranks _
            .Where(Function(a) a.Item1 > 0) _
            .OrderByDescending(Function(a) a.Item1) _
            .Select(Function(a)
                        Return a.adduct
                    End Function)
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
            .TryCast(Of IEnumerable(Of (sign%, expression As String))) _
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
        If InvalidAdduct(formula, adduct) Then
            Return 0
        End If

        Return 1
    End Function

    Private Function InvalidAdduct(formula As Formula, adduct As MzCalculator) As Boolean
        Dim mz As Double = adduct.CalcMZ(formula.ExactMass)

        If mz <= 0 Then
            Return True
        End If

        Dim adduct_parts = GetAdductsFormula(adduct)
        Dim copy As New Formula(formula)

        For Each part In adduct_parts
            copy += part.sign * part.formula
        Next

        If copy.CountsByElement.Any(Function(a) a.Value < 0) Then
            Return True
        End If

        Return False
    End Function

    Private Function RankNegative(formula As Formula, adduct As MzCalculator) As Double
        If InvalidAdduct(formula, adduct) Then
            Return 0
        End If

        Dim adduct_str As String = adduct.ToString

        ' deal with some special adducts type situation
        If formula.CheckElement("Na") Then
            If adduct_str = "[M-Na]-" Then
                If formula!Na = 1 Then
                    Return maxValue
                End If
            End If
        End If
        If formula.CheckElement("K") Then
            If adduct_str = "[M-K]-" Then
                If formula!K = 1 Then
                    Return maxValue
                End If
            End If
        End If
        If formula.CheckElement("Li") Then
            If adduct_str = "[M-Li]-" Then
                If formula!Li = 1 Then
                    Return maxValue
                End If
            End If
        End If

        Return 1
    End Function
End Class
