Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Emit.Marshal
Imports Microsoft.VisualBasic.Language

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

            Dim M% = Val(type.Matches("\d*M[+-]+").FirstOrDefault)

            If M = 0 Then
                M = 1
            End If

            Dim formulas As New List(Of (sign%, expression As String))
            Dim parser As New Pointer(Of Char)(type.StringReplace("\d*M", ""))
            Dim buffer As New List(Of Char)
            Dim c As Char
            Dim sign%

            Do While Not parser.EndRead
                c = ++parser

                If c = "+"c OrElse c = "-"c Then
                    If buffer > 0 Then
                        formulas += (sign, buffer.CharString)
                        buffer *= 0
                    Else
                        sign = If(c = "+"c, 1, -1)
                    End If
                Else
                    buffer += c
                End If
            Loop

            ' 补上最后一个元素
            formulas += (sign, buffer.CharString)

            Dim adducts# = Aggregate formula
                           In formulas
                           Let mass As Double = formulaMass(formula.expression)
                           Into Sum(formula.sign * mass)

            Return New MzCalculator With {
                .M = M,
                .charge = charge,
                .Name = precursorType,
                .adducts = adducts
            }
        End Function
    End Module
End Namespace