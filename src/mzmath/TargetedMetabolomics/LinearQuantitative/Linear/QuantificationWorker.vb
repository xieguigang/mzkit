#Region "Microsoft.VisualBasic::7f21c3a1f9fdb5e0d813e17e1c937a0f, TargetedMetabolomics\LinearQuantitative\Linear\QuantificationWorker.vb"

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

'     Module QuantificationWorker
' 
'         Function: DoLinearQuantify, ReverseModelFunction
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports stdNum = System.Math

Namespace LinearQuantitative.Linear

    Module QuantificationWorker

        Private Function LevelFactorName(levelFactors As String()) As Func(Of Integer, String)
            If Not levelFactors.IsNullOrEmpty Then
                Return Function(i) levelFactors(i)
            Else
                Return Function(i) "L" & (i + 1)
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="points">如果这个参数为空值, 说明不需要返回测试数据</param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function CreateModelPoints(C#(), A#(),
                                                   Optional ISA#() = Nothing,
                                                   Optional CIS# = 0,
                                                   Optional id$ = Nothing,
                                                   Optional name$ = Nothing,
                                                   Optional levelFactors As String() = Nothing,
                                                   Optional points As List(Of ReferencePoint) = Nothing) As IEnumerable(Of PointF)
            Dim AIS#
            Dim factorName = LevelFactorName(levelFactors)

            If points Is Nothing Then
                points = New List(Of ReferencePoint)
            Else
                points *= 0
            End If

            If ISA.IsNullOrEmpty Then
                ISA = Nothing
            End If

            For i As Integer = 0 To C.Length - 1
                Dim Ct_i = C(i)
                Dim At_i = stdNum.Round(A(i))

                ' X 为峰面积，这样子在后面计算的时候就可以直接将离子对的峰面积带入方程计算出浓度结果了
                Dim pX#
                ' 20181106
                ' 因为CIS是假设恒定不变的，所以在这里就直接使用标准曲线的点的浓度来作为Y轴的值了
                Dim pY# = Ct_i ' / CIS   

                ' 获取内标的峰面积以及进行内标矫正
                If ISA Is Nothing Then
                    ' 不需要进行内标校正的情况
                    ' 直接使用样本的峰面积作为X轴数据
                    AIS = 0
                    pX = At_i
                Else
                    ' 需要做内标校正的情况
                    AIS = ISA(i)
                    pX = At_i / AIS
                End If

                ' C = f(A/AIS) = a * X + b
                ' 在进行计算的时候，直接将 样本的峰面积除以内标的峰面积 作为X
                ' 然后代入标准曲线公式即可得到Y，即样本的浓度
                points += New ReferencePoint With {
                    .AIS = AIS,
                    .Ati = At_i,
                    .cIS = CIS,
                    .Cti = Ct_i,
                    .ID = id,
                    .Name = name,
                    .level = factorName(i),
                    .valid = True
                }

                ' 得到标准曲线之中的一个点

                ' 20200103
                '
                ' it's wired that axis X should be the content and 
                ' Y Is the peak area ratio in targeted quantify 
                ' analysis
                Yield New PointF(CSng(pY), CSng(pX))
            Next
        End Function

        <Extension>
        Friend Function ReverseModelFunction(model As StandardCurve) As Func(Of Double, Double)
            Dim fx As Polynomial = model.linear.Polynomial
            Dim a As Double = fx.Factors(1)
            Dim b As Double = fx.Factors(Scan0)

            Return Function(y)
                       y = (y - b) / a

                       ' ND value?
                       If y < 0 Then
                           y = 0
                       End If

                       Return y
                   End Function
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="TPA">file data peak area contents</param>
        ''' <param name="model"></param>
        ''' <param name="names">
        ''' [id -> common name]
        ''' </param>
        ''' <param name="raw">
        ''' the raw file name, which file that current <paramref name="TPA"/> data comes from?
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function DoLinearQuantify(TPA As Dictionary(Of String, IonTPA),
                                         model As StandardCurve,
                                         names As Dictionary(Of String, String),
                                         raw$) As ContentResult(Of IonPeakTableRow)
            Dim AIS As New IonTPA
            Dim X#
            ' 得到样品之中的峰面积
            Dim A = TPA(model.name)
            Dim C#

            If Not model.requireISCalibration Then
                ' 不需要内标进行校正
                ' 则X轴的数据直接是代谢物的峰面积数据
                X = A.area
            Else
                ' 数据存在丢失
                If Not TPA.ContainsKey(model.IS.ID) Then
                    Call $"Missing internal standard for {model.name}!".Warning
                    Return Nothing
                Else
                    ' 得到与样品混在一起的内标的峰面积
                    ' X轴数据需要做内标校正
                    AIS = TPA(model.IS.ID)
                    X# = A.area / AIS.area
                End If
            End If

            If model.linear Is Nothing Then
                Call $"Missing metabolite {model.name} in raw file!".Warning
                Return Nothing
            Else
                ' 利用峰面积比计算出浓度结果数据
                ' 然后通过X轴的数据就可以通过标准曲线的线性回归模型计算出浓度了
                If X.IsNaNImaginary OrElse X <= 0 Then
                    ' ND
                    C = 0
                Else
                    C# = model.ReverseModelFunction(X)
                End If
            End If

            ' 这里的C是相当于 cIS/ct = C，则样品的浓度结果应该为 ct = cIS/C
            ' C = Val(info!cIS) / C

            Dim [IS] As String = names.TryGetValue(model.IS.ID)
            Dim peaktable As New IonPeakTableRow With {
                .content = C,
                .ID = model.name,
                .raw = raw,
                .rtmax = A.peakROI.Max,
                .rtmin = A.peakROI.Min,
                .Name = names(model.name),
                .TPA = A.area,
                .TPA_IS = AIS.area,
                .base = A.baseline,
                .IS = If([IS] Is Nothing, "", $"{model.IS.ID} ({[IS]})"),
                .maxinto = A.maxPeakHeight,
                .maxinto_IS = AIS.maxPeakHeight
            }

            Return New ContentResult(Of IonPeakTableRow) With {
                .Name = model.name,
                .Content = C,
                .X = X,
                .Peaktable = peaktable
            }
        End Function
    End Module
End Namespace
