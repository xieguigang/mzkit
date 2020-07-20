<HideModuleName>
Public Module Extensions

    Public Function HtmlView(formula As String) As String
        Dim numbers As String() = formula _
            .Matches("\d+") _
            .Distinct _
            .OrderByDescending(Function(d) d.Length) _
            .ToArray

        For Each n As String In numbers
            formula = formula.Replace(n, $"<sub>{n}</sub>")
        Next

        Return formula
    End Function
End Module
