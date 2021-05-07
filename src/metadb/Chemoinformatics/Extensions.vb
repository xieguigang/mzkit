#Region "Microsoft.VisualBasic::aca06a371533b558cf87ca568a50f68c, src\metadb\Chemoinformatics\Extensions.vb"

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

' Module Extensions
' 
'     Function: HtmlView
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

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

    Public Function GetQuantityPrefix(n As Integer) As String
        Select Case n
            Case 1 : Return "mono"
            Case 2 : Return "di"
            Case 3 : Return "tri"
            Case 4 : Return "tetra"
            Case 5 : Return "penta"
            Case 6 : Return "hexa"
            Case 7 : Return "hepta"
            Case 8 : Return "octa"
            Case 9 : Return "ennea"
            Case 10 : Return "deca"
            Case 11 : Return "undeca"
            Case 12 : Return "dodeca"
            Case Else
                Return n.ToString
        End Select
    End Function

    ReadOnly steric As Index(Of String) = {"alpha", "beta", "trans"}

    <Extension>
    Public Iterator Function GlycosylNameParser(glycosyl As String) As IEnumerable(Of String)
        Dim qprefix As NamedValue(Of Integer)() = Enums(Of QuantityPrefix) _
            .Select(Function(n)
                        Return New NamedValue(Of Integer)(n.Description, CInt(n))
                    End Function) _
            .ToArray

        glycosyl = glycosyl.StringReplace("\d+", " ").ToLower
        glycosyl = glycosyl.StringReplace("[()]", " ")
        glycosyl = glycosyl.Replace("'", "").Replace("[", " ").Replace("]", " ")
        glycosyl = glycosyl.StringReplace("[-]{2,}", "-")
        glycosyl = glycosyl.Trim(" "c, "-"c)

        For Each token As String In glycosyl.StringSplit("([-])|\s+")
            Dim hitPrefix As Boolean = False

            If token.Length < 3 Then
                Continue For
            ElseIf token Like steric Then
                Continue For
            End If

            For Each q In qprefix
                If token.StartsWith(q.Name) Then
                    token = token.Substring(q.Name.Length)
                    hitPrefix = True

                    For i As Integer = 1 To q.Value
                        Yield token
                    Next

                    Exit For
                End If
            Next

            If Not hitPrefix Then
                Yield token
            End If
        Next
    End Function
End Module

Public Enum QuantityPrefix
    mono = 1
    di
    tri
    tetra
    penta
    hexa
    hepta
    octa
    ennea
    deca
    undeca
    dodeca
End Enum