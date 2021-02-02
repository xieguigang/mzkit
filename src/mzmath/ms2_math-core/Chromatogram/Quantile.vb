#Region "Microsoft.VisualBasic::8af31f12471b7e8ba214e025be981e12, ms2_math-core\Chromatogram\Quantile.vb"

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

    '     Class Quantile
    ' 
    '         Properties: Percentage, Quantile
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Chromatogram

    ''' <summary>
    ''' 可以通过这个quantile分布对象来了解基线数据是否计算正确
    ''' </summary>
    Public Class Quantile

        ''' <summary>
        ''' Quantile value in this <see cref="Percentage"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property Quantile As Double
        ''' <summary>
        ''' [0, 1] quantile percentage
        ''' </summary>
        ''' <returns></returns>
        Public Property Percentage As Double

        Public Overrides Function ToString() As String
            Return $"{Quantile} @ {Percentage * 100}%"
        End Function
    End Class
End Namespace
