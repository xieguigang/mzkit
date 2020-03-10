Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Namespace MRM

    Public Module QuantificationWorker

        <Extension>
        Friend Function reverseModel(model As StandardCurve) As Func(Of Double, Double)
            Dim fx As Polynomial = model.linear.Polynomial
            Dim a As Double = fx.Factors(1)
            Dim b As Double = fx.Factors(Scan0)

            Return Function(y)
                       y = (y - b) / a

                       ' ND value?
                       'If y < 0 Then
                       '    y = 0
                       'End If

                       Return y
                   End Function
        End Function

        ''' <summary>
        ''' 根据建立起来的线性回归模型进行样品数据的扫描，根据曲线的结果得到浓度数据
        ''' </summary>
        ''' <param name="linearModels">标准曲线线性回归模型，X为峰面积</param>
        ''' <param name="raw$"></param>
        ''' <param name="ions"></param>
        ''' <returns>
        ''' <see cref="NamedValue(Of Double).Value"/>是指定的代谢物的浓度结果数据，
        ''' <see cref="NamedValue(Of Double).Description"/>则是AIS/A的结果，即X轴的数据
        ''' </returns>
        <Extension>
        Public Iterator Function ScanContent(linearModels As StandardCurve(),
                                             raw$,
                                             ions As IonPair(),
                                             peakAreaMethod As PeakAreaMethods,
                                             angleThreshold#,
                                             baselineQuantile#,
                                             TPAFactors As Dictionary(Of String, Double),
                                             tolerance As Tolerance,
                                             timeWindowSize#,
                                             rtshifts As Dictionary(Of String, Double)) As IEnumerable(Of ContentResult(Of MRMPeakTable))

            Dim TPA As Dictionary(Of String, IonTPA) = raw _
                .ScanTPA(ionpairs:=ions,
                         peakAreaMethod:=peakAreaMethod,
                         TPAFactors:=TPAFactors,
                         tolerance:=tolerance,
                         timeWindowSize:=timeWindowSize,
                         angleThreshold:=angleThreshold,
                         baselineQuantile:=baselineQuantile,
                         rtshifts:=rtshifts
                ) _
                .ToDictionary(Function(ion) ion.name)

            Dim names As Dictionary(Of String, IonPair) = ions.ToDictionary(Function(i) i.accession)
            ' model -> y = ax + b
            ' in_calculation -> x = (y-b)/a
            Dim quantifyLinears = linearModels _
                .Select(Function(line)
                            Return (fx:=line.reverseModel, linearModels:=line, name:=line.name)
                        End Function) _
                .Where(Function(m) TPA.ContainsKey(m.name)) _
                .ToArray
            Dim quantify As New Value(Of ContentResult(Of MRMPeakTable))

            raw = raw.FileName

            ' 遍历得到的所有的标准曲线，进行样本之中的浓度的计算
            For Each metabolite In quantifyLinears
                If Not (quantify = TPA.doLinearQuantify(metabolite, names, raw)) Is Nothing Then
                    Yield quantify.Value
                End If
            Next
        End Function

        <Extension>
        Private Function doLinearQuantify(TPA As Dictionary(Of String, IonTPA),
                                          metabolite As (fx As Func(Of Double, Double), model As StandardCurve, name$),
                                          names As Dictionary(Of String, IonPair),
                                          raw$) As ContentResult(Of MRMPeakTable)
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
                C# = metabolite.fx(X)
            End If

            ' 这里的C是相当于 cIS/ct = C，则样品的浓度结果应该为 ct = cIS/C
            ' C = Val(info!cIS) / C

            Dim [IS] As IonPair = names.TryGetValue(model.IS.ID)
            Dim peaktable As New MRMPeakTable With {
                .content = C,
                .ID = metabolite.name,
                .raw = raw,
                .rtmax = A.peakROI.Max,
                .rtmin = A.peakROI.Min,
                .Name = names(metabolite.name).name,
                .TPA = A.area,
                .TPA_IS = AIS.area,
                .base = A.baseline,
                .IS = If([IS] Is Nothing, "", $"{[IS].accession} ({[IS].name})"),
                .maxinto = A.maxPeakHeight,
                .maxinto_IS = AIS.maxPeakHeight
            }

            Return New ContentResult(Of MRMPeakTable) With {
                .Name = metabolite.name,
                .Content = C,
                .X = X,
                .Peaktable = peaktable
            }
        End Function

    End Module
End Namespace