#Region "Microsoft.VisualBasic::fdc4bdb527db89a260793b3192a65836, ms2_math-core\Chromatogram\IROI.vb"

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

    '     Interface IROI
    ' 
    '         Properties: rtmax, rtmin
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Chromatogram

    ''' <summary>
    ''' 一个ROI区域就是色谱图上面的一个时间范围内的色谱峰数据
    ''' </summary>
    Public Interface IROI

        ''' <summary>
        ''' 色谱图区域范围的时间下限
        ''' </summary>
        ''' <returns></returns>
        Property rtmin As Double

        ''' <summary>
        ''' 色谱图区域范围的时间上限
        ''' </summary>
        ''' <returns></returns>
        Property rtmax As Double
    End Interface
End Namespace
