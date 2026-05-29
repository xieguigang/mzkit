#Region "Microsoft.VisualBasic::14cef8622bba8402a96182d955535acd, mzmath\ms2_simulator\MolecularMechanics.vb"

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

    '   Total Lines: 40
    '    Code Lines: 10 (25.00%)
    ' Comment Lines: 28 (70.00%)
    '    - Xml Docs: 64.29%
    ' 
    '   Blank Lines: 2 (5.00%)
    '     File Size: 1.76 KB


    ' Module MolecularMechanics
    ' 
    '     Function: CalculateHarmesFinkelPotential, CalculateLennardJonesPotential
    ' 
    ' /********************************************************************************/

#End Region

Public Module MolecularMechanics

    ''' <summary>
    ''' 哈默斯-芬克（Harmes-Finkel）势能函数
    ''' 
    ''' 哈默斯定律势能函数 for 键长，哈默斯-芬克势能函数通常用于描述键长和键角的势能
    ''' </summary>
    ''' <param name="k">力常数</param>
    ''' <param name="r">当前键长</param>
    ''' <param name="r0">平衡键长</param>
    ''' <returns>势能 V = 0.5 * k * (r - r0)^2</returns>
    Public Function CalculateHarmesFinkelPotential(k As Double, r As Double, r0 As Double) As Double
        ' k 是力常数
        ' r 是当前键长
        ' r0 是平衡键长
        ' 势能 V = 0.5 * k * (r - r0)^2
        Return 0.5 * k * (r - r0) ^ 2
    End Function

    ''' <summary>
    ''' 伦纳德-琼斯势能函数
    ''' 
    ''' 伦纳德-琼斯（Lennard-Jones）势能函数，伦纳德-琼斯势能函数用于描述非键相互作用，特别是范德华力。
    ''' </summary>
    ''' <param name="epsilon">
    ''' 势能阱的深度
    ''' </param>
    ''' <param name="sigma">分子间的范德华半径</param>
    ''' <param name="r">分子间的距离</param>
    ''' <returns>势能 V = 4 * epsilon * [(sigma / r)^12 - (sigma / r)^6]</returns>
    Public Function CalculateLennardJonesPotential(epsilon As Double, sigma As Double, r As Double) As Double
        ' epsilon 是势能阱的深度
        ' sigma 是分子间的范德华半径
        ' r 是分子间的距离
        ' 势能 V = 4 * epsilon * [(sigma / r)^12 - (sigma / r)^6]
        Dim sigmaOverR6 As Double = (sigma / r) ^ 6
        Dim sigmaOverR12 As Double = sigmaOverR6 ^ 2
        Return 4 * epsilon * (sigmaOverR12 - sigmaOverR6)
    End Function
End Module
