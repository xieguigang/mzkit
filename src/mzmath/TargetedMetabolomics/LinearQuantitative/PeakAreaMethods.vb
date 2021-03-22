#Region "Microsoft.VisualBasic::af84cd48b8aea6bf935184af1bc0637c, src\mzmath\TargetedMetabolomics\LinearQuantitative\PeakAreaMethods.vb"

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

    '     Enum PeakAreaMethods
    ' 
    '         MaxPeakHeight, SumAll
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace LinearQuantitative

    ''' <summary>
    ''' + 积分方法太敏感了，可能会对ROI以及峰型要求非常高
    ''' + 净峰法简单相加会比较鲁棒一些
    ''' </summary>
    Public Enum PeakAreaMethods
#Region "A = S - B"
        ''' <summary>
        ''' 使用简单的信号相加净峰法来计算峰面积
        ''' </summary>
        NetPeakSum = 0

        ''' <summary>
        ''' 使用积分器来进行峰面积的计算
        ''' </summary>
        Integrator = 1
#End Region
        ''' <summary>
        ''' No peak finding, sum all chromatogram ticks signal intensity directly.
        ''' 基线非常低（接近于零）的时候可以使用
        ''' </summary>
        SumAll
        ''' <summary>
        ''' 如果色谱柱的压力非常大，出峰非常的集中，可以直接使用最大的峰高度来近似为峰面积
        ''' </summary>
        MaxPeakHeight
    End Enum
End Namespace
