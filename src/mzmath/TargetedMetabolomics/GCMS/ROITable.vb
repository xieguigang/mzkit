#Region "Microsoft.VisualBasic::549546d6449d252e08ffb0dc1f3ad49c, src\mzmath\TargetedMetabolomics\GCMS\ROITable.vb"

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

    '     Class ROITable
    ' 
    '         Properties: [IS], baseline, ID, integration, mass_spectra
    '                     maxInto, ri, rt, rtmax, rtmin
    '                     rtMinute, sn
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Data.Linq.Mapping
Imports SMRUCC.MassSpectrum.Math.Chromatogram

Namespace GCMS

    ''' <summary>
    ''' 用于GCMS定量结果输出的色谱图ROI区域表格
    ''' </summary>
    Public Class ROITable : Implements IRetentionTime, IROI

        Public Property ID As String

        Public Property rtmin As Double Implements IROI.rtmin
        Public Property rtmax As Double Implements IROI.rtmax

        Public Property rt As Double Implements IRetentionTime.rt

        ''' <summary>
        ''' 以分钟为单位的保留时间
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="rt(minute)")>
        Public ReadOnly Property rtMinute As Double
            Get
                Return rt / 60
            End Get
        End Property

        ''' <summary>
        ''' 保留指数
        ''' </summary>
        ''' <returns></returns>
        Public Property ri As Double

        ''' <summary>
        ''' 这个区域的最大峰高度
        ''' </summary>
        ''' <returns></returns>
        Public Property maxInto As Double
        ''' <summary>
        ''' 所计算出来的基线的响应强度
        ''' </summary>
        ''' <returns></returns>
        Public Property baseline As Double
        ''' <summary>
        ''' 当前的这个ROI的峰面积积分值
        ''' </summary>
        ''' <returns></returns>
        Public Property integration As Double
        ''' <summary>
        ''' 目标进行浓度计算构建线性回归的时候所需要的内标
        ''' </summary>
        ''' <returns></returns>
        Public Property [IS] As String

        ''' <summary>
        ''' 信噪比
        ''' </summary>
        ''' <returns></returns>
        Public Property sn As Double
        Public Property mass_spectra As String

    End Class
End Namespace
