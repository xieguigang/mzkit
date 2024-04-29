#Region "Microsoft.VisualBasic::34c2c7724a58ed2c99c3ce9d2b31349d, E:/mzkit/src/metadb/Chemoinformatics//NaturalProduct/TokenScanner.vb"

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


    ' Code Statistics:

    '   Total Lines: 63
    '    Code Lines: 50
    ' Comment Lines: 0
    '   Blank Lines: 13
    '     File Size: 1.85 KB


    '     Class TokenScanner
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetTokens, MeasureToken, ScanTokens, WalkChar
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Text.Parser

Namespace NaturalProduct

    Public Class TokenScanner

        Dim name As CharPtr
        Dim buf As New CharBuffer

        Sub New(name As String)
            Me.name = name
        End Sub

        Public Shared Function ScanTokens(name As String) As IEnumerable(Of Token)
            Return New TokenScanner(name).GetTokens
        End Function

        Public Iterator Function GetTokens() As IEnumerable(Of Token)
            Do While name
                For Each token As Token In WalkChar(++name)
                    Yield token
                Next
            Loop

            If buf > 0 Then
                Yield MeasureToken()
            End If
        End Function

        Private Iterator Function WalkChar(c As Char) As IEnumerable(Of Token)
            If c = "("c Then
                If buf > 0 Then
                    Yield MeasureToken()
                End If

                Yield New Token(NameTokens.open, "(")
            ElseIf c = ")"c Then
                If buf > 0 Then
                    Yield MeasureToken()
                End If

                Yield New Token(NameTokens.close, ")")
            ElseIf c = "-"c OrElse c = ","c OrElse c = " "c Then
                If buf > 0 Then
                    Yield MeasureToken()
                End If
            Else
                buf += c
            End If
        End Function

        Private Function MeasureToken() As Token
            Dim text As String = buf.PopAllChars.CharString

            If GlycosylNameSolver.qprefix.ContainsKey(text) Then
                Return New Token(NameTokens.number, text)
            Else
                Return New Token(NameTokens.name, text)
            End If
        End Function

    End Class
End Namespace
