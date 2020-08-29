Imports System.Text
Imports FastColoredTextBoxNS
Imports Microsoft.VisualBasic.ComponentModel

Public Class PageRscriptEditor
    Implements ISaveHandle

    Private Sub FastColoredTextBox1_ToolTipNeeded(sender As Object, e As ToolTipNeededEventArgs) Handles FastColoredTextBox1.ToolTipNeeded
        If Not String.IsNullOrEmpty(e.HoveredWord) Then
            e.ToolTipTitle = e.HoveredWord
            e.ToolTipText = "This is tooltip for '" & e.HoveredWord & "'"
        End If

        Dim range As New Range(sender, e.Place, e.Place)
        Dim hoveredWord = range.GetFragment("[^\n]").Text
        e.ToolTipTitle = hoveredWord
        e.ToolTipText = "This is tooltip for '" & hoveredWord & "'"
    End Sub

    Private Sub PageRscriptEditor_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim syntaxHighlighter As New SyntaxHighlighter(FastColoredTextBox1)

        FastColoredTextBox1.Text = "#!/usr/local/bin/R#"
        FastColoredTextBox1.SyntaxHighlighter = syntaxHighlighter
    End Sub

    Dim blue As New TextStyle(Brushes.Blue, Nothing, FontStyle.Regular)
    Dim green As New TextStyle(Brushes.Green, Nothing, FontStyle.Italic)
    Dim red As New TextStyle(Brushes.Red, Nothing, FontStyle.Bold)
    Dim endSymbol As New TextStyle(Brushes.Black, Nothing, FontStyle.Bold)

    Dim keywords As String = "(\s)?(" & {
        "let", "as", "integer",
        "imports", "from",
        "in"
    }.Select(Function(a) $"({a})").JoinBy("|") & ")\s"

    Dim keyword2 As String = "\s(" & {"function", "double", "boolean", "string", "for", "if", "else"}.Select(Function(a) $"({a})").JoinBy("|") & ")(\s|\)|,)"

    Private Sub FastColoredTextBox1_TextChanged(sender As Object, e As TextChangedEventArgs) Handles FastColoredTextBox1.TextChanged
        ' clear folding markers of changed range
        e.ChangedRange.ClearFoldingMarkers()
        ' set folding markers
        e.ChangedRange.SetFoldingMarkers("{", "}")
        e.ChangedRange.SetFoldingMarkers("#region", "#endregion")

        e.ChangedRange.ClearStyle(blue, green, red, endSymbol)
        e.ChangedRange.SetStyle(red, "([""].*[""])|(['].*['])|([`].*[`])")
        e.ChangedRange.SetStyle(blue, keywords)
        e.ChangedRange.SetStyle(blue, keyword2)
        e.ChangedRange.SetStyle(green, "#.*$")
        e.ChangedRange.SetStyle(endSymbol, ";")
    End Sub

    Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        Throw New NotImplementedException()
    End Function

    Public Function Save(path As String, Optional encoding As Microsoft.VisualBasic.Text.Encodings = Microsoft.VisualBasic.Text.Encodings.UTF8) As Boolean Implements ISaveHandle.Save
        Throw New NotImplementedException()
    End Function
End Class
