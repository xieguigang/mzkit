#Region "Microsoft.VisualBasic::465318eb9a806ce95774ceecec6c491b, ms2_math-core\Ms1\PrecursorType\PrecursorType.vb"

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

    '     Module PrecursorType
    ' 
    '         Function: CalcMass, (+2 Overloads) FindPrecursorType
    ' 
    '     Structure TypeMatch
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language.C
Imports sys = System.Math

Namespace Ms1.PrecursorType

    ''' <summary>
    ''' 质谱前体离子计算器
    ''' </summary>
    Public Module PrecursorType

        Public Const no_result$ = "Unknown"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="chargeMode$">+/-</param>
        ''' <param name="charge%"></param>
        ''' <param name="PrecursorType$"></param>
        ''' <returns></returns>
        Public Function CalcMass(chargeMode$, charge%, PrecursorType$) As Func(Of Double, Double)
            If (PrecursorType = "[M]+" OrElse PrecursorType = "[M]-") Then
                Return (Function(x) x)
            End If

            Dim mode As Dictionary(Of String, MzCalculator) = Calculator(chargeMode)
            Dim found As MzCalculator = Nothing

            For Each cacl In mode.Values
                If (cacl.name = PrecursorType) Then
                    found = cacl
                    Exit For
                End If
            Next

            If found.name.StringEmpty Then
                Return Nothing
            Else
                Return AddressOf found.CalcMass
            End If
        End Function

        ''' <summary>
        ''' 计算出前体离子的加和模式
        ''' </summary>
        ''' <param name="mass">分子质量</param>
        ''' <param name="precursorMZ">前体的m/z</param>
        ''' <param name="charge">电荷量</param>
        ''' <param name="chargeMode">极性</param>
        ''' <param name="tolerance">所能够容忍的质量误差</param>
        ''' <returns></returns>
        Public Function FindPrecursorType(mass#, precursorMZ#, charge%, Optional chargeMode$ = "+", Optional tolerance As Tolerance = Nothing) As TypeMatch
            If charge = 0 Then
                Return New TypeMatch With {
                    .errors = Double.NaN,
                    .precursorType = no_result,
                    .message = "I can't calculate the ionization mode for no charge(charge = 0)!"
                }
            ElseIf (mass.IsNaNImaginary OrElse precursorMZ.IsNaNImaginary) Then
                Return New TypeMatch With {
                    .errors = Double.NaN,
                    .precursorType = no_result,
                    .message = sprintf("  ****** mass='%s' or precursor_M/Z='%s' is an invalid value!", mass, precursorMZ)
                }
            Else
                tolerance = tolerance Or Tolerance.DefaultTolerance
            End If

            Dim mz# = mass / sys.Abs(charge)
            Dim ppm As Double = tolerance.MassError(precursorMZ, mz)

            If tolerance.MatchTolerance([error]:=ppm) Then
                ' 本身的分子质量和前体的mz一样，说明为[M]类型
                If (sys.Abs(charge) = 1) Then
                    Return New TypeMatch With {
                        .errors = ppm,
                        .precursorType = "[M]" & chargeMode
                    }
                Else
                    Return New TypeMatch With {
                        .errors = ppm,
                        .precursorType = sprintf("[M]%s%s", charge, chargeMode)
                    }
                End If
            Else
                Return FindPrecursorType(mass, precursorMZ, charge, Provider.Calculator(chargeMode), tolerance)
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="mass"></param>
        ''' <param name="precursorMZ#"></param>
        ''' <param name="charge%"></param>
        ''' <param name="calculator">得到某一个离子模式下的计算程序</param>
        ''' <param name="tolerance"></param>
        ''' <returns></returns>
        Private Function FindPrecursorType(mass#, precursorMZ#, charge%, calculator As Dictionary(Of String, MzCalculator), tolerance As Tolerance) As TypeMatch
            ' 每一个模式都计算一遍，然后返回最小的ppm差值结果
            Dim min = 999999
            Dim minType$ = Nothing

            ' 然后遍历这个模式下的所有离子前体计算
            ' 跳过电荷数不匹配的离子模式计算表达式
            For Each calc As MzCalculator In calculator.Values.Where(Function(cal) cal.charge = charge)
                Dim mz As Double = calc.CalcMZ(mass)

                If tolerance(mz, precursorMZ) Then
                    Return New TypeMatch With {
                        .errors = tolerance.MassError(mz, precursorMZ),
                        .precursorType = calc.name
                    }
                End If
            Next

            Return New TypeMatch With {
                .precursorType = no_result,
                .errors = Double.NaN,
                .message = "No match"
            }
        End Function
    End Module

    Public Structure TypeMatch

        ''' <summary>
        ''' 值误差
        ''' </summary>
        Dim errors As Double
        Dim precursorType As String
        Dim message As String

    End Structure
End Namespace
