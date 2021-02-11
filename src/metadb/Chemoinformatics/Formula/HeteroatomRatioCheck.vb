#Region "Microsoft.VisualBasic::eca7986c133dbfcdcc37611c11bc46d1, Chemoinformatics\Formula\HeteroatomRatioCheck.vb"

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

    '     Module HeteroatomRatioCheck
    ' 
    '         Function: ElementProbabilityCheck, HeteroatomRatioCheck, HydrogenCarbonElementRatioCheck
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Formula

    Module HeteroatomRatioCheck

        <Extension>
        Public Function HeteroatomRatioCheck(formula As Formula) As Boolean
            Dim C As Integer = formula!C

            If C = 0 Then
                ' 不支持这种检查规则，则跳过
                Return True
            End If

            If formula!H / C > 6 Then Return False
            If formula!F / C > 6 Then Return False
            If formula!Cl / C > 2 Then Return False
            If formula!Br / C > 2 Then Return False
            If formula!N / C > 4 Then Return False
            If formula!O / C > 3 Then Return False
            If formula!P / C > 2 Then Return False
            If formula!S / C > 3 Then Return False
            If formula!Si / C > 1 Then Return False

            Return True
        End Function

        ''' <summary>
        ''' Rule #4 – Hydrogen/Carbon element ratio check
        ''' 
        ''' + In most cases the hydrogen/carbon ratio does not exceed ``H/C`` > 3 with rare exception such as in methylhydrazine (CH6N2).
        ''' + Conversely, the ``H/C`` ratio Is usually smaller than 2, And should Not be less than 0.125 Like in the case of tetracyanopyrrole (C8HN5).
        ''' + Most typical ratios are found between ``2.0 > H/C > 0.5``
        ''' + More than 99.7% Of all formulas were included With H/C ratios between ``0.2–3.1``. Consequently, we Call this range the 'common range'.
        ''' + However, a number of chemical classes fall out of this range, And we have hence enabled the user to select 'extended ranges' covering 99.99% of all formulas in this development database (H/C 0.1–6).
        ''' </summary>
        ''' <param name="formula"></param>
        ''' <returns></returns>
        <Extension>
        Public Function HydrogenCarbonElementRatioCheck(formula As FormulaComposition) As Boolean
            Dim HCRatio As Double = formula.HCRatio

            If HCRatio >= 3 Then
                With formula
                    Return !C = 1 AndAlso !H = 6 AndAlso !N = 2
                End With
            ElseIf HCRatio <= 0.125 Then
                With formula
                    Return !C = 8 AndAlso !H = 1 AndAlso !N = 5
                End With
            Else
                Return HCRatio >= 0.2 AndAlso HCRatio <= 3.1
            End If
        End Function

        ''' <summary>
        ''' Rule #6 – element probability check
        ''' 
        ''' Multiple element count restriction for compounds &lt; 2000 Da, 
        ''' based on the examination of the Beilstein database and the 
        ''' Dictionary of Natural Products
        ''' </summary>
        ''' <param name="formula"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ElementProbabilityCheck(formula As Formula) As Boolean
            With formula
                If !N > 1 AndAlso !O > 1 AndAlso !P > 1 AndAlso !S > 1 Then
                    Return !N < 10 AndAlso !O < 20 AndAlso !P < 4 AndAlso !S < 3
                End If
                If !N > 3 AndAlso !O > 3 AndAlso !P > 3 Then
                    Return !N < 11 AndAlso !O < 22 AndAlso !P < 6
                End If
                If !O > 1 AndAlso !P > 1 AndAlso !S > 1 Then
                    Return !O < 14 AndAlso !P < 3 AndAlso !S < 3
                End If
                If !N > 1 AndAlso !P > 1 AndAlso !S > 1 Then
                    Return !N < 4 AndAlso !P < 3 AndAlso !S < 3
                End If
                If !N > 6 AndAlso !O > 6 AndAlso !S > 6 Then
                    Return !N < 19 AndAlso !O < 14 AndAlso !S < 8
                End If
            End With

            Return True
        End Function
    End Module
End Namespace
