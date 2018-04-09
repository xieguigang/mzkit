Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Scripting
Imports SMRUCC.MassSpectrum.Math.Chromatogram

''' <summary>
''' 根据累加线来查找色谱峰的ROI
''' </summary>
Public Module AccumulateROI

    ' 算法原理，每当出现一个峰的时候，累加线就会明显升高一个高度
    ' 当升高的时候，曲线的斜率大于零
    ' 当处于基线水平的时候，曲线的斜率接近于零
    ' 则可以利用这个特性将色谱峰给识别出来
    ' 这个方法仅局限于色谱峰都是各自相互独立的情况之下

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="angleThreshold#">区分色谱峰的累加线切线角度的阈值，单位为度</param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Iterator Function PopulateROI(chromatogram As IVector(Of ChromatogramTick), Optional angleThreshold# = 5, Optional baselineQuantile# = 0.65) As IEnumerable(Of ROI)
        ' 先计算出基线和累加线
        Dim baseline# = chromatogram.Baseline(baselineQuantile)
        Dim intensity As Vector = chromatogram!intensity
        ' Dim maxInto# = intensity.Max - baseline
        Dim accumulate#
        Dim sumALL# = (chromatogram!intensity - baseline).Where(Function(x) x > 0).Sum
        Dim ay = Function(into As Double) As Double
                     into -= baseline
                     accumulate += If(into < 0, 0, into)
                     Return (accumulate / sumALL) ' * maxInto
                 End Function
        Dim accumulateLine = chromatogram _
            .Select(Function(tick)
                        Return New PointF(tick.Time, ay(tick.Intensity))
                    End Function) _
            .ToArray

        ' 使用滑窗计算出切线的斜率
        Dim windows = accumulateLine.SlideWindows(winSize:=2).ToArray
        Dim peaks = windows.Split(Function(tangent) (tangent.First, tangent.Last).Angle <= angleThreshold).ToArray

    End Function
End Module
