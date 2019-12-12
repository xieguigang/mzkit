Imports System.Runtime.CompilerServices

Namespace MetaLib

    Public Class FormulaComposition

        Default Public ReadOnly Property GetAtomCount(atom As String) As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return CountsByElement.TryGetValue(atom)
            End Get
        End Property

        Public ReadOnly Property CountsByElement As Dictionary(Of String, Integer)
        Public ReadOnly Property EmpiricalFormula As String

        Sub New(counts As IDictionary(Of String, Integer), Optional formula$ = Nothing)
            CountsByElement = New Dictionary(Of String, Integer)(counts)

            If formula.StringEmpty Then
                EmpiricalFormula = CountsByElement _
                    .Select(Function(e)
                                Return If(e.Value = 1, e.Key, e.Key & e.Value)
                            End Function) _
                    .JoinBy("")
            Else
                EmpiricalFormula = formula
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return EmpiricalFormula
        End Function

        Public Shared Operator *(composition As FormulaComposition, n%) As FormulaComposition
            Dim newFormula$ = $"({composition}){n}"
            Dim newComposition = composition _
            .CountsByElement _
            .ToDictionary(Function(e) e.Key,
                          Function(e)
                              Return e.Value * n
                          End Function)

            Return New FormulaComposition(newComposition, newFormula)
        End Operator
    End Class
End Namespace