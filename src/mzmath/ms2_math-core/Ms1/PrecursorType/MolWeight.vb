#Region "Microsoft.VisualBasic::a7a00c3617d341de990d9b22e50dd766, src\mzmath\ms2_math-core\Ms1\PrecursorType\MolWeight.vb"

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

    '     Module MolWeight
    ' 
    '         Function: Eval, Mul, Weight
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports r = System.Text.RegularExpressions.Regex

Namespace Ms1.PrecursorType

    Public Module MolWeight

        ReadOnly weights As New Dictionary(Of String, Double) From {
            {"H", 1.007276},
            {"CH3COO", 59.013},
            {"C3H7O2", 75.045},
            {"C2H3O", 43.018},
            {"Methylcarbonyl", 43.018},
            {"Propyldioxy", 75.045},
            {"C12H20O9", 308.111},
            {"Lactal", 308.111},
            {"CH3", 15.032528},
            {"C", 12.0107},
            {"Na", 22.98976928},
            {"NH4", 18.035534},
            {"K", 39.0983},
            {"F", 18.998},
            {"Li", 6.941},
            {"H2O", 18.01471},
            {"ACN", 41.04746},      ' Acetonitrile (CH3CN)
            {"CH3OH", 32.03773},
            {"C2H3O2", 59.013},     ' Acetate
            {"DMSO", 78.12089},     ' dimethyl sulfoxide (CH3)2SO 
            {"IsoProp", 60.058064}, ' Unknown
            {"Cl", 35.446},
            {"FA", 46.00548},       ' Unknown
            {"Hac", 60.04636},      ' AceticAcid (CH3COOH)
            {"Br", 79.901},
            {"TFA", 113.9929}       ' Unknown
        }

        Public Function Weight(symbol As String) As Double
            If weights.ContainsKey(symbol) Then
                Return weights(symbol)
            Else
                Return -1
            End If
        End Function

        ''' <summary>
        ''' 主要是用于计算<see cref="MzCalculator.adducts"/>部分的质量
        ''' </summary>
        ''' <param name="formula"></param>
        ''' <returns></returns>
        Public Function Eval(formula As String) As Double
            Static ionModeSymbols As Index(Of Char) = {"+"c, "-"c}

            If formula.StringEmpty Then
                ' [M]+, [M]-是没有adducts的
                Return 0
            End If
            If formula.First Like ionModeSymbols Then
                formula = "0H" & formula
            End If

            Dim mt = r.Split(formula, "[+-]")
            Dim op = r.Matches(formula, "[+-]").ToArray
            Dim x# = 0
            Dim [next] As Char = "+"c

            For i As Integer = 0 To mt.Length - 1
                Dim token = MolWeight.Mul(mt(i))
                Dim m = token.Value
                Dim name = token.Name

                If Weight(name) = -1.0# Then
                    Dim msg$ = $"Unknown symbol in: '{formula}', where symbol={token}"
                    Throw New Exception(msg)
                End If

                If [next] = "+"c Then
                    x += (m * weights(name))
                Else
                    x -= (m * weights(name))
                End If

                If ((Not op.IsNullOrEmpty) AndAlso (i <= op.Length - 1)) Then
                    [next] = op(i)
                End If
            Next

            Return x
        End Function

        Private Function Mul(token As String) As NamedValue(Of Integer)
            Dim n$ = ""
            Dim len% = Strings.Len(token)

            Static x0 As Integer = Asc("0")
            Static x9 As Integer = Asc("9")

            For i As Integer = 0 To len - 1
                Dim x% = Asc(token(i))

                If (x >= x0 AndAlso x <= x9) Then
                    n = n & token(i)
                Else
                    Exit For
                End If
            Next

            If Strings.Len(n) = 0 Then
                Return New NamedValue(Of Integer) With {
                    .Name = token,
                    .Value = 1
                }
            Else
                token = token.Substring(n.Length)
            End If

            Return New NamedValue(Of Integer) With {
                .Name = token,
                .Value = CInt(Val(n))
            }
        End Function
    End Module
End Namespace
