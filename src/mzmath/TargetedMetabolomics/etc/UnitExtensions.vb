Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports r = System.Text.RegularExpressions.Regex

Namespace LinearQuantitative

    <HideModuleName>
    Public Module UnitExtensions

        Public Function ppm2ppb(ppm As Double) As Double
            Return ppm.Unit(ContentUnits.ppm).ScaleTo(ContentUnits.ppb).Value
        End Function

        Const contentPattern$ = NumericPattern & "\s*pp(m|b|t)"

        <Extension>
        Public Function ParseContent(text As String) As UnitValue(Of ContentUnits)
            text = LCase(r.Match(text, contentPattern, RegexICSng).Value)

            Return New UnitValue(Of ContentUnits) With {
            .Unit = r _
                .Match(text, "pp(m|b|t)", RegexICSng) _
                .TryParse(Of ContentUnits),
            .Value = Val(text)
        }
        End Function
    End Module
End Namespace