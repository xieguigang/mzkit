#Region "Microsoft.VisualBasic::310ade9e37960b11ea0f812bf8bec7ad, src\mzkit\mzkit\pages\dockWindow\frmRScriptEdit.vb"

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

    ' Class frmRScriptEdit
    ' 
    '     Properties: MimeType, scriptFile
    ' 
    '     Function: (+2 Overloads) Save
    ' 
    '     Sub: frmRScriptEdit_Closing, frmRScriptEdit_Load, LoadScript
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Text
Imports FastColoredTextBoxNS
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Text
Imports mzkit.My
Imports RibbonLib.Interop

Public Class frmRScriptEdit
    Implements ISaveHandle
    Implements IFileReference

    Public Property scriptFile As String Implements IFileReference.FilePath

    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
        Get
            Return {
                New ContentType With {.Details = "http://r_lang.dev.smrucc.org/", .FileExt = ".R", .MIMEType = "text/r_sharp", .Name = "R# script"}
            }
        End Get
    End Property

    Public Sub LoadScript(script As String)
        FastColoredTextBox1.Text = script
    End Sub

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

    Private Sub frmRScriptEdit_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        ' 只保存新文件？
        If scriptFile.StringEmpty Then
            Dim result = MessageBox.Show("Save current script file?", "File Not Saved", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                MyApplication.host.SaveScript(Me)
            ElseIf result = DialogResult.Cancel Then
                e.Cancel = True
            End If
        End If

        If Not e.Cancel Then
            MyApplication.host.scriptFiles.Remove(Me)
        End If
    End Sub

    Private Sub frmRScriptEdit_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Icon = My.Resources.vs

        Me.ShowIcon = True

        Dim syntaxHighlighter As New SyntaxHighlighter(FastColoredTextBox1)

        FastColoredTextBox1.Text = "#!/usr/local/bin/R#"
        FastColoredTextBox1.SyntaxHighlighter = syntaxHighlighter
    End Sub

    Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        Return FastColoredTextBox1.Text.SaveTo(path, encoding)
    End Function

    Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
        Return Save(path, encoding.CodePage)
    End Function
End Class
