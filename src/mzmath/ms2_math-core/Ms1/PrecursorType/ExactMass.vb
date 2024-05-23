#Region "Microsoft.VisualBasic::a0ffff38729d0576db2be22d64a1d5fd, mzmath\ms2_math-core\Ms1\PrecursorType\ExactMass.vb"

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

    '   Total Lines: 185
    '    Code Lines: 131 (70.81%)
    ' Comment Lines: 29 (15.68%)
    '    - Xml Docs: 89.66%
    ' 
    '   Blank Lines: 25 (13.51%)
    '     File Size: 6.17 KB


    '     Module ExactMass
    ' 
    '         Function: Eval, Mul, Weight
    ' 
    '         Sub: SetExactMassParser, ThrowErr
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports r = System.Text.RegularExpressions.Regex

Namespace Ms1.PrecursorType

    ''' <summary>
    ''' exact mass calculator apply for LCMS analysis.
    ''' </summary>
    Public Module ExactMass

        Public Const H = 1.007825
        Public Const C = 12
        Public Const O = 15.994915
        Public Const CH3COO = C + H * 3 + C + O + O
        Public Const H2O = H * 2 + O
        Public Const CH3 = C + H * 3
        Public Const N = 14.003074
        Public Const P = 30.973763

        Dim _eval As Func(Of String, Double)

        ReadOnly weights As New Dictionary(Of String, Double) From {
            {"H", H},
            {"HCOO", H + C + O * 2},
            {"COOH", C + O * 2 + H},
            {"CH3COO", CH3COO},
            {"CH3COOH", CH3COO + H},
            {"C3H7O2", C * 3 + H * 7 + O * 2},
            {"C2H3O", C * 2 + H * 3 + O},
            {"O", O},
            {"P", P},
            {"OH", O + H},
            {"CO2", C + O * 2},
            {"Methylcarbonyl", 43.018},
            {"Propyldioxy", 75.045},
            {"C12H20O9", 308.111},
            {"Lactal", 308.111},
            {"CH3", CH3},
            {"C", C},
            {"N", N},
            {"Na", 22.98976928},
            {"NH4", N + H * 4},
            {"NH3", N + H * 3},
            {"K", 39.0983},
            {"F", 18.998},
            {"Li", 6.941},
            {"H2O", H2O},
            {"HCO3", H + C + O * 3},
            {"ACN", 41.04746},      ' Acetonitrile (CH3CN)
            {"CH3OH", C + H * 3 + O + H},
            {"C2H3O2", C * 2 + H * 3 + O * 2},     ' Acetate
            {"DMSO", 78.12089},     ' dimethyl sulfoxide (CH3)2SO 
            {"IsoProp", 60.058064}, ' Unknown
            {"Cl", 35.446},
            {"FA", 46.00548},       ' Unknown
            {"Hac", 60.04636},      ' AceticAcid (CH3COOH)
            {"Br", 79.901},
            {"TFA", 113.9929}       ' Unknown
        }

        ''' <summary>
        ''' set the parser lambda function from here for handling all
        ''' unknown symbols
        ''' </summary>
        ''' <param name="eval"></param>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub SetExactMassParser(eval As Func(Of String, Double))
            _eval = eval
        End Sub

        ''' <summary>
        ''' get the exact mass of the given formula symbol part
        ''' </summary>
        ''' <param name="symbol"></param>
        ''' <returns>
        ''' get symbol from <see cref="weights"/> or evaluate the symbol
        ''' from the external formula parser function
        ''' </returns>
        Public Function Weight(symbol As String) As Double
            If weights.ContainsKey(symbol) Then
                Return weights(symbol)

                ' enable evaluate the unknown symbol
                ' via the external parser function
            ElseIf Not _eval Is Nothing Then
                Return _eval(symbol)
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
            Dim raw_formula As String = formula

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
                Dim token = ExactMass.Mul(mt(i))
                Dim m As Integer = token.Value
                Dim name = token.Name
                Dim w As Double = Weight(name)

                If w = -1.0# Then
                    ThrowErr(raw_formula, token)
                End If

                If [next] = "+"c Then
                    x += m * w
                Else
                    x -= m * w
                End If

                If ((Not op.IsNullOrEmpty) AndAlso (i <= op.Length - 1)) Then
                    [next] = op(i)
                End If
            Next

            Return x
        End Function

        Private Sub ThrowErr(formula As String, token As String)
            Dim msg$ = $"Unknown symbol in formula string: '{formula}', where symbol is '{token}'"

            VBDebugger.EchoLine(msg)
            Throw New Exception(msg)
        End Sub

        Const x0 As Integer = Asc("0"c)
        Const x9 As Integer = Asc("9"c)

        ''' <summary>
        ''' parse the element group multiply
        ''' </summary>
        ''' <param name="token"></param>
        ''' <returns></returns>
        Public Function Mul(token As String) As NamedValue(Of Integer)
            Dim n$ = ""
            Dim len% = Strings.Len(token)

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
