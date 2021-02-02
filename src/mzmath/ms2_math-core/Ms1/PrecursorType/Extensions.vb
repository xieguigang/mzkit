#Region "Microsoft.VisualBasic::b646dd6f20e5bb7762618ffbe1e5af99, ms2_math-core\Ms1\PrecursorType\Extensions.vb"

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

    '     Module Parser
    ' 
    '         Function: Formula, ParseCharge, ParseMzCalculator, ParseMzCalculatorInternal, ToString
    ' 
    '     Module Extensions
    ' 
    '         Sub: PrintTable
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Text.Parser
Imports Microsoft.VisualBasic.Text.Xml.HtmlBuilder

Namespace Ms1.PrecursorType

    Public Module Parser

        ' [M+H]+
        ' [M+H2O]+
        ' [M+Cl]-
        ' [2M+NH4-H2O]4+

        ReadOnly defaultCharge As [Default](Of String) = "1"
        ReadOnly defaultMassCount As [Default](Of Integer) = 1.AsDefault(Function(m) CInt(m) <= 0)

        <Extension>
        Public Function ParseCharge(precursor_type$, ionmode$) As Integer
            Dim charge$ = precursor_type.Split("]"c).Last
            Dim chargeValue As Integer
            Dim iint As Integer = ParseIonMode(ionmode)

            If charge = "+" Then
                chargeValue = 1
            ElseIf charge = "-" Then
                chargeValue = -1
            ElseIf charge.StringEmpty Then
                chargeValue = iint
            Else
                chargeValue = Val(charge) * iint
            End If

            Return chargeValue
        End Function

        ''' <summary>
        ''' 这个函数是具有缓存功能的
        ''' </summary>
        ''' <param name="precursor_type">如果这个字符串没有电荷数量结尾,则这个函数默认是带有1个单位的电荷的</param>
        ''' <param name="ionMode">只允许出现``+/-``这两种字符串</param>
        ''' <param name="skipEvalAdducts">
        ''' 如果有时候只需要对<paramref name="precursor_type"/>字符串做格式化,而不需要做计算的话,可以将这个参数设置为True,加快计算效率
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function ParseMzCalculator(precursor_type$, Optional ionMode$ = "+", Optional skipEvalAdducts As Boolean = False) As MzCalculator
            Static cache As New Dictionary(Of String, Dictionary(Of String, MzCalculator)) From {
                {"+", New Dictionary(Of String, MzCalculator)},
                {"-", New Dictionary(Of String, MzCalculator)}
            }

            If cache(ionMode).ContainsKey(precursor_type) Then
                Return cache(ionMode)(precursor_type)
            Else
                Dim mz = ParseMzCalculatorInternal(precursor_type, ionMode, skipEvalAdducts)
                cache(ionMode).Add(precursor_type, mz)
                Return mz
            End If
        End Function

        <Extension>
        Private Function ParseMzCalculatorInternal(precursor_type$, Optional ionMode$ = "+", Optional skipEvalAdducts As Boolean = False) As MzCalculator
            Dim type$ = precursor_type.GetStackValue("[", "]")
            Dim mode$ = precursor_type.Split("]"c).Last.Match("[+-]") Or ionMode.AsDefault
            Dim charge$ = precursor_type.Split("]"c).Last.Match("\d+") Or defaultCharge
            Dim M% = CInt(Val(type.Matches("\d*M[+-]+").FirstOrDefault)) Or defaultMassCount
            Dim formulas = Parser.Formula(Strings.Trim(precursor_type), raw:=True) _
                .TryCast(Of IEnumerable(Of (sign%, expression As String))) _
                .ToArray
            Dim adducts# = 0

            If Not skipEvalAdducts Then
                adducts = Aggregate formula As (sign%, expression$)
                          In formulas
                          Let mass As Double = ExactMass.Eval(formula.expression)
                          Into Sum(formula.sign * mass)
            End If

            If ParseIonMode(mode) = 0 Then
                mode = ionMode
            End If

            Return New MzCalculator With {
                .M = M,
                .charge = charge,
                .name = formulas.ToString(M, charge, mode),
                .adducts = adducts,
                .mode = mode
            }
        End Function

        <Extension>
        Private Function ToString(formula As (sign%, expression$)(), M%, charge$, mode$) As String
            Dim adducts$ = formula _
                .Select(Function(t) $"{If(t.sign > 0, "+", "-")}{t.expression}") _
                .JoinBy("")
            Dim main$ = If(M = 1, "M", M & "M")
            Dim chargeMode$ = If(charge = 1, mode, charge & mode)

            Return $"[{main}{adducts}]{chargeMode}"
        End Function

        Public Function Formula(precursor_type$, Optional raw As Boolean = True) As [Variant](Of String, IEnumerable(Of (sign%, expression As String)))
            Dim formulas As New List(Of (sign%, expression As String))
            Dim parser As CharPtr = precursor_type.GetStackValue("[", "]").StringReplace("\d*M", "")
            Dim buffer As New List(Of Char)
            Dim c As Char
            Dim sign% = 1

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
            If buffer > 0 Then
                formulas += (sign, buffer.CharString)
            End If

            If raw Then
                ' 20190510
                ' 运行时不允许隐式转换  
                Return New [Variant](Of String, IEnumerable(Of (sign As Integer, expression As String)))(formulas.AsEnumerable)
            Else
                Throw New NotImplementedException
            End If
        End Function
    End Module

    Public Module Extensions

        ''' <summary>
        ''' Debug used
        ''' </summary>
        ''' <param name="report"></param>
        ''' <param name="output"></param>
        <Extension>
        Public Sub PrintTable(report As IEnumerable(Of PrecursorInfo), output As TextWriter)
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

            For Each type As PrecursorInfo In report
                Call output.WriteLine(
                    <tr>
                        <td><%= type.precursor_type %></td>
                        <td><%= type.charge %></td>
                        <td><%= type.M %></td>
                        <td><%= type.adduct %></td>
                        <td><%= type.mz %></td>
                    </tr>)
            Next

            Call output.WriteLine("</tbody>")

            Call output.WriteLine("</table>")
        End Sub
    End Module
End Namespace
