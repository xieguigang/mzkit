#Region "Microsoft.VisualBasic::cbd112c9dfa5e7fd396fbc40d1977cef, mzmath\ms2_math-core\Ms1\PrecursorType\PrecursorType.vb"

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

    '   Total Lines: 169
    '    Code Lines: 118 (69.82%)
    ' Comment Lines: 33 (19.53%)
    '    - Xml Docs: 75.76%
    ' 
    '   Blank Lines: 18 (10.65%)
    '     File Size: 7.09 KB


    '     Module PrecursorType
    ' 
    '         Function: CalcMass, (+3 Overloads) FindPrecursorType
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language.C
Imports std = System.Math

Namespace Ms1.PrecursorType

    ''' <summary>
    ''' 质谱前体离子计算器
    ''' </summary>
    Public Module PrecursorType

        Public Const no_result$ = "Unknown"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="chargeMode">+/-</param>
        ''' <param name="charge"></param>
        ''' <param name="precursor_type"></param>
        ''' <returns></returns>
        Public Function CalcMass(chargeMode$, charge%, precursor_type$) As Func(Of Double, Double)
            If (precursor_type = "[M]+" OrElse precursor_type = "[M]-") Then
                Return (Function(x) x)
            End If

            Dim mode As Dictionary(Of String, MzCalculator) = Provider.GetCalculator(chargeMode)
            Dim found As MzCalculator = Nothing

            For Each cacl In mode.Values
                If (cacl.name = precursor_type) Then
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
        ''' <param name="precursor_mz">前体的m/z</param>
        ''' <param name="charge">the signed charge value.</param>
        ''' <param name="chargeMode">极性</param>
        ''' <param name="tolerance">所能够容忍的质量误差</param>
        ''' <returns></returns>
        Public Function FindPrecursorType(mass#, precursor_mz#, charge%,
                                          Optional chargeMode$ = "+",
                                          Optional tolerance As Tolerance = Nothing,
                                          Optional firstMatch As Boolean = False) As TypeMatch
            If charge = 0 Then
                Return New TypeMatch With {
                    .errors = Double.NaN,
                    .precursorType = no_result,
                    .message = "I can't calculate the ionization mode for no charge(charge = 0)!"
                }
            ElseIf (mass.IsNaNImaginary OrElse precursor_mz.IsNaNImaginary) Then
                Return New TypeMatch With {
                    .errors = Double.NaN,
                    .precursorType = no_result,
                    .message = sprintf("  ****** mass='%s' or precursor_M/Z='%s' is an invalid value!", mass, precursor_mz)
                }
            Else
                tolerance = tolerance Or Tolerance.DefaultTolerance
            End If

            Dim mz# = mass / std.Abs(charge)
            Dim ppm As Double = tolerance.MassError(precursor_mz, mz)

            If tolerance.MatchTolerance([error]:=ppm) Then
                ' 本身的分子质量和前体的mz一样，说明为[M]类型
                If (std.Abs(charge) = 1) Then
                    Return New TypeMatch With {
                        .errors = ppm,
                        .precursorType = "[M]" & chargeMode,
                        .message = "mass equals to the precursor_mz within the given tolerance error.",
                        .adducts = Provider.ParseAdductModel(.precursorType)
                    }
                Else
                    Return New TypeMatch With {
                        .errors = ppm,
                        .precursorType = sprintf("[M]%s%s", charge, chargeMode),
                        .message = "invalid adducts type?"
                    }
                End If
            Else
                Return Provider _
                    .GetCalculator(chargeMode).Values _
                    .Where(Function(cal)
                               Return cal.charge = charge
                           End Function) _
                    .FindPrecursorType(
                        mass,
                        precursor_mz,
                        tolerance:=tolerance,
                        firstMatch:=firstMatch
                    )
            End If
        End Function

        Public Function FindPrecursorType(mass#, precursor_mz#, adducts As String(),
                                          Optional tolerance As Tolerance = Nothing,
                                          Optional firstMatch As Boolean = False) As TypeMatch

            Dim listSet As IEnumerable(Of MzCalculator) = adducts _
                .Select(Function(type)
                            Return Provider.ParseAdductModel(type)
                        End Function)

            Return listSet.FindPrecursorType(
                mass, precursor_mz,
                tolerance:=tolerance Or Tolerance.DefaultTolerance,
                firstMatch:=firstMatch
            )
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="mass"></param>
        ''' <param name="precursor_mz"></param>
        ''' <param name="adducts">得到某一个离子模式下的计算程序</param>
        ''' <param name="tolerance"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function FindPrecursorType(adducts As IEnumerable(Of MzCalculator), mass#, precursor_mz#, tolerance As Tolerance, Optional firstMatch As Boolean = False) As TypeMatch
            ' 每一个模式都计算一遍，然后返回最小的ppm差值结果
            Dim min As Double = Double.MaxValue
            Dim minType As TypeMatch = Nothing

            ' 然后遍历这个模式下的所有离子前体计算
            ' 跳过电荷数不匹配的离子模式计算表达式
            For Each calc As MzCalculator In adducts
                Dim mz As Double = calc.CalcMZ(mass)
                Dim mzdiff As Double = std.Abs(precursor_mz - mz)

                If mzdiff < min Then
                    min = mzdiff
                    minType = New TypeMatch With {
                        .errors = tolerance.MassError(mz, precursor_mz),
                        .precursorType = calc.name,
                        .adducts = calc
                    }

                    ' just returns the first matched result
                    If firstMatch AndAlso tolerance(mz, precursor_mz) Then
                        Return minType
                    End If
                End If
            Next

            If minType.adducts IsNot Nothing AndAlso tolerance(minType.adducts.CalcMZ(mass), precursor_mz) Then
                Return minType
            Else
                Return New TypeMatch With {
                    .precursorType = no_result,
                    .errors = Double.NaN,
                    .message = "No match"
                }
            End If
        End Function
    End Module
End Namespace
