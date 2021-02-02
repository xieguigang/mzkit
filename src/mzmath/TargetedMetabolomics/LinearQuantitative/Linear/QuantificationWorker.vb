Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Namespace LinearQuantitative.Linear

    Module QuantificationWorker

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

        <Extension>
        Public Function DoLinearQuantify(TPA As Dictionary(Of String, IonTPA),
                                         metabolite As (fx As Func(Of Double, Double), model As StandardCurve, name$),
                                         names As Dictionary(Of String, String),
                                         raw$) As ContentResult(Of IonPeakTableRow)
            Dim AIS As New IonTPA
            Dim X#
            ' 得到样品之中的峰面积
            Dim A = TPA(metabolite.name)
            Dim model As StandardCurve = metabolite.model
            Dim C#

            If Not model.requireISCalibration Then
                ' 不需要内标进行校正
                ' 则X轴的数据直接是代谢物的峰面积数据
                X = A.area
            Else
                ' 数据存在丢失
                If Not TPA.ContainsKey(model.IS.ID) Then
                    Call $"Missing internal standard for {metabolite.name}!".Warning
                    Return Nothing
                Else
                    ' 得到与样品混在一起的内标的峰面积
                    ' X轴数据需要做内标校正
                    AIS = TPA(model.IS.ID)
                    X# = A.area / AIS.area
                End If
            End If

            If model.linear Is Nothing Then
                Call $"Missing metabolite {metabolite.name} in raw file!".Warning
                Return Nothing
            Else
                ' 利用峰面积比计算出浓度结果数据
                ' 然后通过X轴的数据就可以通过标准曲线的线性回归模型计算出浓度了
                If X.IsNaNImaginary OrElse X <= 0 Then
                    ' ND
                    C = 0
                Else
                    C# = metabolite.fx(X)
                End If
            End If

            ' 这里的C是相当于 cIS/ct = C，则样品的浓度结果应该为 ct = cIS/C
            ' C = Val(info!cIS) / C

            Dim [IS] As String = names.TryGetValue(model.IS.ID)
            Dim peaktable As New IonPeakTableRow With {
                .content = C,
                .ID = metabolite.name,
                .raw = raw,
                .rtmax = A.peakROI.Max,
                .rtmin = A.peakROI.Min,
                .Name = names(metabolite.name),
                .TPA = A.area,
                .TPA_IS = AIS.area,
                .base = A.baseline,
                .IS = If([IS] Is Nothing, "", $"{model.IS.ID} ({[IS]})"),
                .maxinto = A.maxPeakHeight,
                .maxinto_IS = AIS.maxPeakHeight
            }

            Return New ContentResult(Of IonPeakTableRow) With {
                .Name = metabolite.name,
                .Content = C,
                .X = X,
                .Peaktable = peaktable
            }
        End Function
    End Module
End Namespace