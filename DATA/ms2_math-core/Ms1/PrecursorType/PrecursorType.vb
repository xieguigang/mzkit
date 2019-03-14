#Region "Microsoft.VisualBasic::e1e3d93c2b2f5fb037ec0ac9ae0813cc, ms2_math-core\Ms1\PrecursorType\PrecursorType.vb"

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
    '         Function: CalcMass, FindPrecursorType, Negative, Positive, ppm
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

            Dim mode As Dictionary(Of String, MzCalculator) = calculator(chargeMode)
            Dim found As MzCalculator = Nothing

            For Each cacl In mode.Values
                If (cacl.Name = PrecursorType) Then
                    found = cacl
                    Exit For
                End If
            Next

            If found.Name.StringEmpty Then
                Return Nothing
            Else
                Return AddressOf found.CalcMass
            End If
        End Function

        ReadOnly calculator As New Dictionary(Of String, Dictionary(Of String, MzCalculator)) From {
            {"+", Positive()},
            {"-", Negative()}
        }

        ''' <summary>
        ''' 计算出前体离子的加和模式
        ''' </summary>
        ''' <param name="mass#">分子质量</param>
        ''' <param name="precursorMZ#">前体的m/z</param>
        ''' <param name="charge%">电荷量</param>
        ''' <param name="chargeMode$">极性</param>
        ''' <param name="minError_ppm#">所能够容忍的质量误差</param>
        ''' <param name="debugEcho"></param>
        ''' <returns></returns>
        Public Function FindPrecursorType(mass#, precursorMZ#, charge%,
                                          Optional chargeMode$ = "+",
                                          Optional minError_ppm# = 100,
                                          Optional debugEcho As Boolean = True) As (ppm#, type$)
            If (charge = 0) Then
                println("I can't calculate the ionization mode for no charge(charge = 0)!")
                Return (Double.NaN, no_result)
            End If

            If (mass.IsNaNImaginary OrElse precursorMZ.IsNaNImaginary) Then
                println("  ****** mass='%s' or precursor_M/Z='%s' is an invalid value!", mass, precursorMZ)
                Return (Double.NaN, no_result)
            End If

            Dim ppm = PrecursorType.ppm(precursorMZ, mass / sys.Abs(charge))

            If (ppm <= 500) Then
                ' 本身的分子质量和前体的mz一样，说明为[M]类型
                If (sys.Abs(charge) = 1) Then
                    Return (ppm, "[M]" & chargeMode)
                Else
                    Return (ppm, sprintf("[M]%s%s", charge, chargeMode))
                End If
            End If

            ' 每一个模式都计算一遍，然后返回最小的ppm差值结果
            Dim min = 999999
            Dim minType$ = Nothing

            ' 得到某一个离子模式下的计算程序
            Dim mode As Dictionary(Of String, MzCalculator) = calculator(chargeMode)

            If (chargeMode = "-") Then
                ' 对于负离子模式而言，虽然电荷量是负数的，但是使用xcms解析出来的却是一个电荷数的绝对值
                ' 所以需要判断一次，乘以-1 
                If (charge > 0) Then
                    charge = -1 * charge
                End If
            End If

            ' 然后遍历这个模式下的所有离子前体计算
            For Each calc In mode.Values
                Dim ptype = calc.Name

                ' 跳过电荷数不匹配的离子模式计算表达式
                If (charge <> calc.charge) Then
                    Continue For
                End If

                ' 这里实际上是根据数据库之中的分子质量，通过前体离子的质量计算出mz结果
                ' 然后计算mz计算结果和precursorMZ的ppm信息
                Dim massReverse = calc.CalcMass(precursorMZ)
                Dim deltappm = PrecursorType.ppm(massReverse, actualValue:=mass)

                If (debugEcho) Then
                    println("%s - %s = %s(ppm), type=%s", mass, massReverse, deltappm, ptype)
                End If

                ' 根据质量计算出前体质量，然后计算出差值
                If (deltappm < min) Then
                    min = deltappm
                    minType = ptype
                End If
            Next

            ' 假若这个最小的ppm差值符合误差范围，则认为找到了一个前体模式
            If (debugEcho) Then
                println("  ==> %s", minType)
            End If

            If (min <= minError_ppm) Then
                Return (min, minType)
            Else
                If (debugEcho) Then
                    println("But the '%s' ionization mode its ppm error (%s ppm) is ", minType, min)
                    println("not satisfy the minError requirement(%s), returns Unknown!", minError_ppm)
                End If

                Return (-1, no_result)
            End If
        End Function
    End Module
End Namespace
