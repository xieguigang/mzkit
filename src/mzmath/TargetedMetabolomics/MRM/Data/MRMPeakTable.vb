#Region "Microsoft.VisualBasic::44e8a72cea578bc2b9bada2231de3cfb, src\mzmath\TargetedMetabolomics\MRM\Data\MRMPeakTable.vb"

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

'     Class MRMPeakTable
' 
'         Properties: [IS], base, content, ID, maxinto
'                     maxinto_IS, Name, raw, rtmax, rtmin
'                     TPA, TPA_IS
' 
'         Function: ToString
' 
'     Class MRMStandards
' 
'         Properties: AIS, Ati, cIS, Cti, ID
'                     level, Name
' 
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace MRM.Data

    Public Class MRMPeakTable : Implements IROI

        ''' <summary>
        ''' 标志物物质编号
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As String
        ''' <summary>
        ''' 通用名称
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String
        ''' <summary>
        ''' 保留时间的下限
        ''' </summary>
        ''' <returns></returns>
        Public Property rtmin As Double Implements IROI.rtmin
        ''' <summary>
        ''' 保留时间的上限
        ''' </summary>
        ''' <returns></returns>
        Public Property rtmax As Double Implements IROI.rtmax
        ''' <summary>
        ''' 浓度
        ''' </summary>
        ''' <returns></returns>
        Public Property content As Double
        ''' <summary>
        ''' 最大的信号响应值
        ''' </summary>
        ''' <returns></returns>
        Public Property maxinto As Double
        ''' <summary>
        ''' 内标的最大信号响应值
        ''' </summary>
        ''' <returns></returns>
        Public Property maxinto_IS As Double

        ''' <summary>
        ''' 峰面积
        ''' </summary>
        ''' <returns></returns>
        Public Property TPA As Double
        ''' <summary>
        ''' 内标的峰面积
        ''' </summary>
        ''' <returns></returns>
        Public Property TPA_IS As Double
        ''' <summary>
        ''' 内标编号
        ''' </summary>
        ''' <returns></returns>
        <Column("IS")>
        Public Property [IS] As String
        ''' <summary>
        ''' 信号基线水平
        ''' </summary>
        ''' <returns></returns>
        Public Property base As Double
        ''' <summary>
        ''' 实验数据的原始文件名
        ''' </summary>
        ''' <returns></returns>
        Public Property raw As String

        Public Overrides Function ToString() As String
            Return Name
        End Function

    End Class

    ''' <summary>
    ''' 表示标准曲线上面的一个实验数据点
    ''' </summary>
    Public Class MRMStandards

        Public Property ID As String
        Public Property Name As String

        ''' <summary>
        ''' 内标峰面积
        ''' </summary>
        ''' <returns></returns>
        Public Property AIS As Double
        ''' <summary>
        ''' 当前试验点的标准品峰面积
        ''' </summary>
        ''' <returns></returns>
        Public Property Ati As Double
        ''' <summary>
        ''' 内标浓度
        ''' </summary>
        ''' <returns></returns>
        Public Property cIS As Double
        ''' <summary>
        ''' 当前试验点的标准品浓度
        ''' </summary>
        ''' <returns></returns>
        Public Property Cti As Double

        ''' <summary>
        ''' 浓度梯度水平的名称，例如：``L1, L2, L3, ...``
        ''' </summary>
        ''' <returns></returns>
        Public Property level As String

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"Dim {Name} As {ID} = {Cti}"
        End Function
    End Class
End Namespace
