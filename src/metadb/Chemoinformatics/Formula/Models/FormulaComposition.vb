
Namespace Formula

    Public Class FormulaComposition : Inherits Formula

        Public Property charge As Double
        Public Property ppm As Double

        Public ReadOnly Property HCRatio As Double
            Get
                If CountsByElement.ContainsKey("C") AndAlso CountsByElement.ContainsKey("H") Then
                    Return CountsByElement!C / CountsByElement!H
                Else
                    Return -1
                End If
            End Get
        End Property

        Sub New(counts As IDictionary(Of String, Integer), Optional formula$ = Nothing)
            Call MyBase.New(counts, formula)
        End Sub

        ''' <summary>
        ''' make a copy and then append a given atom element into formula model
        ''' </summary>
        ''' <param name="element"></param>
        ''' <param name="count"></param>
        ''' <returns></returns>
        Public Function AppendElement(element As String, count As Integer) As FormulaComposition
            Dim copy As FormulaComposition = GetCopy()

            If copy.CountsByElement.ContainsKey(element) Then
                copy.CountsByElement(element) += count
            Else
                copy.CountsByElement(element) = count
            End If

            copy.charge = copy.charge + Formula.Elements(element).charge * count
            copy.m_formula = Formula.BuildFormula(copy.CountsByElement)

            Return copy
        End Function

        Friend Function GetCopy() As FormulaComposition
            Return New FormulaComposition(CountsByElement, EmpiricalFormula) With {
                .charge = charge,
                .ppm = ppm
            }
        End Function
    End Class
End Namespace