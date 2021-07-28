#Region "Microsoft.VisualBasic::99fb6cd7ddbf0f175157f804472d5011, src\mzmath\ms2_math-core\Chromatogram\ROI.vb"

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

    '     Class ROI
    ' 
    '         Properties: baseline, integration, maxInto, noise, peakWidth
    '                     rt, snRatio, ticks, time
    ' 
    '         Function: GetChromatogramData, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Math

Namespace Chromatogram

    ''' <summary>
    ''' Region of interest
    ''' </summary>
    Public Class ROI : Implements IRetentionTime

        ''' <summary>
        ''' 这个区域的起始和结束的时间点
        ''' </summary>
        ''' <returns></returns>
        Public Property time As DoubleRange
        ''' <summary>
        ''' 出峰达到峰高最大值<see cref="maxInto"/>的时间点
        ''' </summary>
        ''' <returns></returns>
        Public Property rt As Double Implements IRetentionTime.rt
        ''' <summary>
        ''' 这个区域的最大峰高度
        ''' </summary>
        ''' <returns></returns>
        Public Property maxInto As Double

        ''' <summary>
        ''' 在这个ROI时间窗区域内的色谱图数据
        ''' </summary>
        ''' <returns></returns>
        Public Property ticks As ChromatogramTick()
        ''' <summary>
        ''' 所计算出来的基线的响应强度
        ''' </summary>
        ''' <returns></returns>
        Public Property baseline As Double
        ''' <summary>
        ''' 当前的这个ROI的峰面积积分值
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 为当前的ROI峰面积占整个TIC峰面积的百分比，一个实验所导出来的所有的ROI的
        ''' 积分值加起来应该是约等于100的
        ''' </remarks>
        Public Property integration As Double

        ''' <summary>
        ''' 噪声的面积积分百分比
        ''' </summary>
        ''' <returns></returns>
        Public Property noise As Double

        ''' <summary>
        ''' 信噪比
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property snRatio As Double
            Get
                Dim signal As Double = Aggregate tick As ChromatogramTick
                                       In ticks
                                       Into Sum(tick.Intensity - baseline)
                Dim sn As Double = SignalProcessing.SNRatio(signal, noise)

                Return sn
            End Get
        End Property

        Public ReadOnly Property peakWidth As Single
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return time.Length
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetChromatogramData(Optional getTitle As Func(Of ROI, String) = Nothing) As NamedCollection(Of ChromatogramTick)
            Static defaultRtTitle As New [Default](Of Func(Of ROI, String))(
                Function(roi)
                    Return $"[{roi.time.Min.ToString("F0")},{roi.time.Max.ToString("F0")}]"
                End Function)
            Return New NamedCollection(Of ChromatogramTick)((getTitle Or defaultRtTitle)(Me), ticks)
        End Function

        Public Overrides Function ToString() As String
            Return time.ToString
        End Function

        Public Shared Narrowing Operator CType(ROI As ROI) As DoubleRange
            If ROI Is Nothing Then
                Return Nothing
            Else
                Return ROI.time
            End If
        End Operator
    End Class
End Namespace
