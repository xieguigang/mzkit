#Region "Microsoft.VisualBasic::b533bb06872e1795daf6570ff6f1e9bb, ms2_math-core\Ms1\PrecursorType\Extensions.vb"

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

'     Module Extensions
' 
'         Function: PrecursorTypeParser
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Emit.Marshal
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.HtmlBuilder
Imports Microsoft.VisualBasic.Text.Xml.Models

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
                    End If

                    sign = If(c = "+"c, 1, -1)
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
                .name = precursorType,
                .adducts = adducts,
                .mode = mode
            }
        End Function

        ''' <summary>
        ''' Debug used
        ''' </summary>
        ''' <param name="report"></param>
        ''' <param name="output"></param>
        <Extension>
        Public Sub PrintTable(report As IEnumerable(Of MzReport), output As TextWriter)
            Call output.WriteLine("<table style='width:100% font-size:0.9em;'>")
            Call output.WriteLine(html:=
                 <thead>
                     <tr>
                         <th>Precursor Type</th>
                         <th>Charge</th>
                         <th>M</th>
                         <th>Adduct</th>
                         <th>m/z</th>
                     </tr>
                 </thead>)

            Call output.WriteLine("<tbody>")

            For Each type As MzReport In report
                Call output.WriteLine(
                    <tr>
                        <td><%= type.PrecursorType %></td>
                        <td><%= type.Charge %></td>
                        <td><%= type.M %></td>
                        <td><%= type.Adduct %></td>
                        <td><%= type.mz %></td>
                    </tr>)
            Next

            Call output.WriteLine("</tbody>")

            Call output.WriteLine("</table>")
        End Sub
    End Module
End Namespace
