Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports r = System.Text.RegularExpressions.Regex

Public Enum ContentUnits As Integer
    ppm = ppb * 1000
    ppb = ppt * 1000
    ppt = 1
End Enum

Public Module UnitExtensions

    Public Function ppm2ppb(ppm As Double) As Double
        Return ppm.Unit(ContentUnits.ppm).ScaleTo(ContentUnits.ppb).Value
    End Function

    Const contentPattern$ = NumericPattern & "\s*pp(m|b|t)"

    <Extension>
    Public Function ParseContent(text As String) As UnitValue(Of ContentUnits)
        text = LCase(r.Match(text, contentPattern, RegexICSng).Value)

        Return New UnitValue(Of ContentUnits) With {
            .Unit = [Enum].Parse(GetType(ContentUnits), r.Match(text, "pp(m|b|t)", RegexICSng).Value),
            .Value = Val(text)
        }
    End Function
End Module