#Region "Microsoft.VisualBasic::270d243955ecb5a8f3019dc6f0f9aa61, FormulaSearch.Extensions\Scores\NitrogenRule.vb"

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

    ' Class NitrogenRule
    ' 
    '     Function: TestRule
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Public Class NitrogenRule

    ''' <summary>
    ''' 若有机化合物有偶数个N原子或不含N原子，则其分子离子的质量是偶数；
    ''' 含奇数个N原子，其质量数是奇数。质谱中最高质量峰不符合氮律就不是
    ''' 分子离子峰。
    ''' </summary>
    ''' <param name="exact_mass#"></param>
    ''' <param name="formula"></param>
    ''' <returns></returns>
    Public Shared Function TestRule(exact_mass#, formula As Formula) As Boolean
        Dim isEven As Boolean = CInt(exact_mass) Mod 2 = 0

        If isEven Then
            Return formula("N") = 0 OrElse formula("N") Mod 2 = 0
        Else
            Return formula("N") Mod 2 <> 0
        End If
    End Function
End Class

