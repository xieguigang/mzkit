#Region "Microsoft.VisualBasic::29b9d2bd933fde07d7d379dbcfa21181, metadb\FormulaSearch.Extensions\AnthocyaninValidator.vb"

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

    '   Total Lines: 140
    '    Code Lines: 75 (53.57%)
    ' Comment Lines: 41 (29.29%)
    '    - Xml Docs: 58.54%
    ' 
    '   Blank Lines: 24 (17.14%)
    '     File Size: 4.49 KB


    ' Module AnthocyaninValidator
    ' 
    '     Function: CalculateProbability, (+2 Overloads) CheckRules, IsLikelyAnthocyanin
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports std = System.Math

Public Module AnthocyaninValidator

    ' 原子量常量
    ReadOnly C_Weight As Double = Formula.AllAtomElements!C.isotopic
    ReadOnly H_Weight As Double = Formula.AllAtomElements!H.isotopic
    ReadOnly O_Weight As Double = Formula.AllAtomElements!O.isotopic

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="elements"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' cutoff is 30%
    ''' </remarks>
    Public Function CalculateProbability(elements As Dictionary(Of String, Integer)) As Double
        Dim score As Double = 0

        If elements Is Nothing Then Return 0

        ' 规则1：必需元素检查
        If Not elements.ContainsKey("C") OrElse Not elements.ContainsKey("H") OrElse Not elements.ContainsKey("O") Then
            Return 0
        End If

        ' 规则2：碳数范围（15-30）
        Dim cCount = elements("C")
        score += std.Max(0, 1 - std.Abs(cCount - 20) / 10) ' 15-30得0.5-1分

        ' 规则3：氧数范围（5-20）
        Dim oCount = elements("O")
        score += std.Max(0, 1 - std.Abs(oCount - 12) / 8) ' 5-20得0.5-1分

        ' 规则4：分子量检查（280-1500）
        Dim molecularWeight = cCount * C_Weight + elements("H") * H_Weight + oCount * O_Weight
        score += If(molecularWeight >= 280 AndAlso molecularWeight <= 1500, 1, 0)

        ' 规则5：糖基检测（如C6H10O5）
        If Regex.IsMatch(Canonical.BuildCanonicalFormula(elements), "C\d+H\d+O5") Then score += 1

        ' 规则6：酚羟基检测（H/O比例）
        Dim h_oRatio = elements("H") / oCount
        score += If(h_oRatio > 1.2 AndAlso h_oRatio < 2.5, 0.5, 0)

        Return score / 4.5 * 100 ' 总分4.5分转换为百分比
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="formula"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' cutoff is 40%
    ''' </remarks>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function CheckRules(formula As String) As Double
        Return CheckRules(elements:=FormulaScanner.ScanFormula(formula).CountsByElement)
    End Function

    ''' <returns></returns>
    ''' <remarks>
    ''' cutoff is 40%
    ''' </remarks>
    Public Function CheckRules(elements As Dictionary(Of String, Integer)) As Double
        Dim score1 = CalculateProbability(elements) / 100
        Dim score2 = IsLikelyAnthocyanin(elements) / 100
        Dim score As Double = 2 * (score1 * score2) * 100

        Return score
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' cutoff is 50%
    ''' </remarks>
    Public Function IsLikelyAnthocyanin(elements As Dictionary(Of String, Integer)) As Double
        Dim score As Integer = 0

        ' 必须元素检查
        If Not elements.ContainsKey("C") OrElse
           Not elements.ContainsKey("H") OrElse
           Not elements.ContainsKey("O") Then
            Return 0
        End If
        score += 3

        Dim c As Integer = elements("C")
        Dim h As Integer = elements("H")
        Dim o As Integer = elements("O")
        Dim n As Integer = If(elements.ContainsKey("N"), elements("N"), 0)

        ' 氧含量检查（关键特征）
        If o < 5 Then Return 0
        score += 2

        ' 碳骨架范围（核心结构C15-C22）
        If c >= 15 AndAlso c <= 22 Then
            score += 2
        Else
            score -= 2
        End If

        ' 不饱和度验证
        Dim maxH As Integer = 2 * c + n - 18
        If h <= maxH Then
            score += 2
        Else
            score -= 1
        End If

        ' 氮含量检查
        If n = 0 OrElse n = 1 Then
            score += 1
        Else
            score -= 1
        End If

        Static elementSet As Index(Of String) = {"C", "H", "O", "N", "CL"}

        ' 排除其他元素
        For Each element In elements.Keys
            If Not element.ToUpperInvariant() Like elementSet Then
                score -= 1
            End If
        Next

        Return std.Max(0, score) * 10.0
    End Function
End Module
