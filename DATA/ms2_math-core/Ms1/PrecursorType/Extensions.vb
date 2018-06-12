Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Emit.Marshal

Namespace Ms1.PrecursorType

    Public Module Extensions

        ' [M+H]+
        ' [M+H2O]+
        ' [M+Cl]-
        ' [2M+NH4-H2O]4+

        <Extension>
        Public Function PrecursorTypeParser(precursorType As String, formulaMass As Func(Of String, Double)) As MzCalculator
            Dim type$ = precursorType.GetStackValue("[", "]")
            Dim mode$ = precursorType.Split("]"c).Last.Match("[+-]")
            Dim charge$ = precursorType.Split("]"c).Last.Match("\d+")

            If charge.StringEmpty Then
                charge = 1
            End If

            Dim M% = Val(type.Matches("\d*M[+-]+"))

            If M = 0 Then
                M = 1
            End If

            Dim formulas As New List(Of String)
            Dim parser As New Pointer(Of Char)(type)
            Dim buffer As New List(Of Char)
            Dim c As Char

            Do While Not parser.EndRead

            Loop

            Dim adducts# = Aggregate formula As String
                           In formulas
                           Let mass As Double = formulaMass(formula)
                           Into Sum(mass)

            Return New MzCalculator With {
                .M = M,
                .charge = charge,
                .Name = precursorType,
                .adducts = adducts
            }
        End Function
    End Module
End Namespace