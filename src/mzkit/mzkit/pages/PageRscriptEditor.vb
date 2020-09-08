#Region "Microsoft.VisualBasic::5ca6d4d5c346b561f965efaa401fea88, src\mzkit\mzkit\pages\PageRscriptEditor.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    ' Class PageRscriptEditor
    ' 
    '     Sub: FastColoredTextBox1_GotFocus, FastColoredTextBox1_TextChanged, FastColoredTextBox1_ToolTipNeeded, PageRscriptEditor_Load
    ' 
    ' /********************************************************************************/

#End Region

Imports FastColoredTextBoxNS
Imports mzkit.My
Imports RibbonLib.Interop

Public Class PageRscriptEditor

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

    Private Sub FastColoredTextBox1_GotFocus(sender As Object, e As EventArgs) Handles FastColoredTextBox1.GotFocus
        MyApplication.host.ribbonItems.TabGroupRscriptTools.ContextAvailable = ContextAvailability.Active
    End Sub
End Class

