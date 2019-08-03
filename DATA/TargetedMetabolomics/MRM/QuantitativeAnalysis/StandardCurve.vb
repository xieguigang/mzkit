#Region "Microsoft.VisualBasic::360a48b196e43a5d97e9d722556444b3, TargetedMetabolomics\MRM\QuantitativeAnalysis\StandardCurve.vb"

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

' Module StandardCurve
' 
'     Function: GetFactor, Regression, Scan, ScanContent, ScanTPA
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzML
Imports SMRUCC.MassSpectrum.Math.MRM.Data
Imports SMRUCC.MassSpectrum.Math.MRM.Models

''' <summary>
''' 对当前批次的标准曲线进行回归建模
''' </summary>
Public Module StandardCurve

    ''' <summary>
    ''' 根据建立起来的线性回归模型进行样品数据的扫描，根据曲线的结果得到浓度数据
    ''' </summary>
    ''' <param name="model">标准曲线线性回归模型，X为峰面积</param>
    ''' <param name="raw$"></param>
    ''' <param name="ions"></param>
    ''' <returns>
    ''' <see cref="NamedValue(Of Double).Value"/>是指定的代谢物的浓度结果数据，
    ''' <see cref="NamedValue(Of Double).Description"/>则是AIS/A的结果，即X轴的数据
    ''' </returns>
    <Extension>
    Public Iterator Function ScanContent(model As FitModel(),
                                         raw$,
                                         ions As IonPair(),
                                         peakAreaMethod As PeakArea.Methods,
                                         TPAFactors As Dictionary(Of String, Double)) As IEnumerable(Of ContentResult(Of MRMPeakTable))
        Dim baseline# = 0
        Dim TPA = raw _
            .ScanTPA(ionpairs:=ions,
                     peakAreaMethod:=peakAreaMethod,
                     TPAFactors:=TPAFactors
            ) _
            .ToDictionary(Function(ion) ion.Name,
                          Function(A) A.Value)
        Dim names As Dictionary(Of String, IonPair) = ions.ToDictionary(Function(i) i.AccID)
        Dim C#

        raw = raw.FileName

        ' 遍历得到的所有的标准曲线，进行样本之中的浓度的计算
        For Each metabolite As FitModel In model.Where(Function(m) TPA.ContainsKey(m.Name))
            Dim AIS As (ROI As DoubleRange, TPA#, baseline#, maxinto#)
            Dim X#
            ' 得到样品之中的峰面积
            Dim A = TPA(metabolite.Name)

            If Not metabolite.RequireISCalibration Then
                ' 不需要内标进行校正
                ' 则X轴的数据直接是代谢物的峰面积数据
                X = A.TPA
            Else
                ' 数据存在丢失
                If Not TPA.ContainsKey(metabolite.IS.ID) Then
                    Continue For
                Else
                    ' 得到与样品混在一起的内标的峰面积
                    ' X轴数据需要做内标校正
                    AIS = TPA(metabolite.IS.ID)
                    X# = A.TPA / AIS.TPA
                End If
            End If

            If metabolite.LinearRegression Is Nothing Then
                Call $"Missing metabolite {metabolite.Name} in raw file!".Warning

                Continue For
            Else
                ' 利用峰面积比计算出浓度结果数据
                ' 然后通过X轴的数据就可以通过标准曲线的线性回归模型计算出浓度了
                C# = metabolite.LinearRegression(X)
            End If

            ' 这里的C是相当于 cIS/ct = C，则样品的浓度结果应该为 ct = cIS/C
            ' C = Val(info!cIS) / C

            Dim [IS] As IonPair = names.TryGetValue(metabolite.IS.ID)
            Dim peaktable As New MRMPeakTable With {
                .content = C,
                .ID = metabolite.Name,
                .raw = raw,
                .rtmax = A.ROI.Max,
                .rtmin = A.ROI.Min,
                .Name = names(metabolite.Name).name,
                .TPA = A.TPA,
                .TPA_IS = AIS.TPA,
                .base = A.baseline,
                .IS = If([IS] Is Nothing, "", $"{[IS].AccID} ({[IS].name})"),
                .maxinto = A.maxinto,
                .maxinto_IS = AIS.maxinto
            }

            Yield New ContentResult(Of MRMPeakTable) With {
                .Name = metabolite.Name,
                .Content = C,
                .X = X,
                .Peaktable = peaktable
            }
        Next
    End Function

    ' 实验采用内标法定量。配制5个不同浓度的标准系列，系列中目标分析物浓度(cti)呈梯度变化
    ' (ct1, ct2, ct3, ct4, ct5)，
    ' 内标浓度保持不变(cIS)。
    ' 将标准系列进样分析，获得各组分的峰面积

    ' 将峰面积比(AIS1/At1, AIS2/At2, AIS3/At3, AIS4/At4, AIS5/At5)
    ' 对浓度比(cIS/ct1, cIS/ct2, cIS/ct3, cIS/ct4, cIS/ct5）作图得到标准曲线。

    ' 由于目标物与内标在同一溶液体系中，因此其浓度比常表示为质量比(WIS/Wti）。
    ' 样品中加人与标准中相等质量的内标物，进样分析后得到待测组分与内标峰面积比，根据标准曲线即可求得
    ' 质量比，而内标质量已知，可得待测组分质量。

    ' 在计算峰面积的时候，对于亮氨酸和异亮氨酸，会需要乘以一个系数
    ' 这个Factor参数默认为1

    ReadOnly NoChange As [Default](Of Double) = 1.0R

    ''' <summary>
    ''' 根据扫描出来的TPA峰面积进行对标准曲线的回归建模
    ''' </summary>
    ''' <param name="ionTPA"></param>
    ''' <param name="calibrates"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function Regression(ionTPA As Dictionary(Of DataSet),
                                        calibrates As Standards(),
                                        [ISvector] As [IS]()) As IEnumerable(Of NamedValue(Of (IFitted, MRMStandards(), [IS])))

        Dim [IS] As Dictionary(Of String, [IS]) = ISvector.ToDictionary(Function(i) i.ID)

        For Each ion As Standards In calibrates
            ' 20181106 如果没有内标，则不进行内标校正
            ' 所以在这里移除下面的where筛选
            ' .Where(Function(i)
            '            Return Not i.IS.StringEmpty AndAlso
            '                   Not ionTPA(i.IS) Is Nothing AndAlso
            '                   Not ionTPA(i.IS).Properties.Count < i.C.Count ' 标曲文件之中只有7个点，但是实际上打了10个点，剩下的三个点可以不要了
            '        End Function)

            Dim TPA As DataSet = ionTPA(ion.HMDB)                             ' 得到标准曲线实验数据
            Dim ISA As DataSet = ionTPA(ion.IS)                               ' 得到内标的实验数据，如果是空值的话，说明不需要内标进行校正
            Dim IsIon As [IS] = [IS].TryGetValue(ion.IS, [default]:=New [IS]) ' 尝试得到内标的数据
            Dim CIS# = IsIon?.CIS                                             ' 内标的浓度，是不变的，所以就只有一个值
            Dim points As New List(Of MRMStandards)

            ' 标准曲线数据
            ' 从实验数据之中产生线性回归计算所需要的点的集合
            ' 注意，这些点之间是具有先后顺序的
            Dim line As PointF() = ion _
                .C _
                .OrderBy(Function(l)
                             ' 按照标曲编号从小到大进行排序
                             Return Val(l.Key.Match("\d+"))
                         End Function) _
                .Select(Function(level)

                            Dim key As String = level.Key.ToUpper
                            Dim At_i = TPA(key) ' 得到峰面积Ati
                            Dim Ct_i = level.Value    ' 得到已知的浓度数据
                            Dim AIS#                  ' 内标的峰面积

                            ' X 为峰面积，这样子在后面计算的时候就可以直接将离子对的峰面积带入方程计算出浓度结果了
                            Dim pX#
                            ' 20181106
                            ' 因为CIS是假设恒定不变的，所以在这里就直接使用标准曲线的点的浓度来作为Y轴的值了
                            Dim pY# = Ct_i ' / CIS   

                            If ISA Is Nothing Then
                                ' 不需要进行内标校正的情况
                                ' 直接使用样本的峰面积作为X轴数据
                                AIS = 0
                                pX = At_i
                            Else
                                ' 需要做内标校正的情况
                                AIS = ISA(key)
                                pX = At_i / AIS
                            End If

                            ' C = f(A/AIS) = a * X + b
                            ' 在进行计算的时候，直接将 样本的峰面积除以内标的峰面积 作为X
                            ' 然后代入标准曲线公式即可得到Y，即样本的浓度

                            points += New MRMStandards With {
                                .AIS = AIS,
                                .Ati = At_i,
                                .cIS = CIS,
                                .Cti = Ct_i,
                                .ID = ion.HMDB,
                                .Name = ion.Name,
                                .level = key
                            }

                            ' 得到标准曲线之中的一个点
                            Return New PointF(pX, pY)
                        End Function) _
                .ToArray

            Dim fit As WeightedFit = FitModel.CreateLinearRegression(line, True)
            Dim out As New NamedValue(Of (IFitted, MRMStandards(), [IS])) With {
                .Name = ion.HMDB,
                .Value = (fit, points.ToArray, IsIon)
            }

            Yield out
        Next
    End Function

    ''' <summary>
    ''' 从原始数据之中扫描峰面积数据，返回来的数据集之中的<see cref="DataSet.ID"/>是HMDB代谢物编号
    ''' </summary>
    ''' <param name="raw">``*.wiff``，转换之后的结果文件夹，其中标准曲线的数据都是默认使用``L数字``标记的。</param>
    ''' <param name="ions">包括离子对的定义数据以及浓度区间</param>
    ''' <param name="TPAFactors">
    ''' ``{<see cref="Standards.HMDB"/>, <see cref="Standards.Factor"/>}``，这个是为了计算亮氨酸和异亮氨酸这类无法被区分的物质的峰面积所需要的
    ''' </param>
    ''' <returns></returns>
    Public Function Scan(raw$,
                         ions As IonPair(),
                         calibrates As Standards(),
                         peakAreaMethod As PeakArea.Methods,
                         TPAFactors As Dictionary(Of String, Double),
                         Optional ByRef refName$() = Nothing,
                         Optional calibrationNamedPattern$ = ".+[-]L\d+",
                         Optional levelPattern$ = "[-]L\d+") As DataSet()

        Dim rawName$ = raw.BaseName
        Dim ionTPAs As New Dictionary(Of String, Dictionary(Of String, Double))
        Dim refNames As New List(Of String)

        For Each ion As IonPair In ions
            ionTPAs(ion.AccID) = New Dictionary(Of String, Double)
        Next

        ' 扫描所有的符合命名规则要求的原始文件
        ' 假设这些符合命名规则的文件都是标准曲线文件
        For Each file As String In (ls - l - r - "*.mzML" <= raw.ParentPath) _
            .Where(Function(path)
                       Return path _
                           .BaseName _
                           .IsPattern(calibrationNamedPattern, RegexICSng)
                   End Function)

            ' 得到当前的这个原始文件之中的峰面积数据
            Dim TPA() = file.ScanTPA(
                ionpairs:=ions,
                peakAreaMethod:=peakAreaMethod,
                TPAFactors:=TPAFactors
            )

            ' 从文件名之中得到浓度的等级，以方便查找出相应的浓度数据
            Dim level$ = file.BaseName _
                             .Match(levelPattern, RegexICSng) _
                             .Trim("-"c)

            refNames += file.BaseName

            ' level = level.Match("[-]L\d+", RegexICSng).Trim("-"c)

            For Each ion In TPA
                ionTPAs(ion.Name).Add(level.ToUpper, ion.Value.TPA)
            Next
        Next

        refName = refNames

        Return ionTPAs _
            .Select(Function(ion)
                        Return New DataSet With {
                            .ID = ion.Key,
                            .Properties = ion.Value
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' 从一个原始文件之中扫描出给定的离子对的峰面积数据
    ''' </summary>
    ''' <param name="raw$">``*.mzML``原始样本数据文件</param>
    ''' <param name="ionpairs"></param>
    ''' <param name="TPAFactors">
    ''' ``{<see cref="Standards.HMDB"/>, <see cref="Standards.Factor"/>}``，这个是为了计算亮氨酸和异亮氨酸这类无法被区分的物质的峰面积所需要的
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function ScanTPA(raw$, ionpairs As IonPair(), TPAFactors As Dictionary(Of String, Double),
                            Optional baselineQuantile# = 0.65,
                            Optional integratorTicks% = 5000,
                            Optional peakAreaMethod As PeakArea.Methods = Methods.Integrator) As NamedValue(Of (ROI As DoubleRange, TPA#, baseline#, maxinto#))()

        ' 从原始文件之中读取出所有指定的离子对数据
        Dim ionData = ionpairs.ExtractIonData(
            mzML:=raw,
            assignName:=Function(ion) ion.AccID
        )
        ' 进行最大峰的查找，然后计算出净峰面积，用于回归建模
        Dim TPA = ionData _
            .Select(Function(ion)
                        Return ion.ionTPA(
                            baselineQuantile,
                            peakAreaMethod,
                            integratorTicks,
                            TPAFactors.GetFactor(ion.Name)
                        )
                    End Function) _
            .ToArray

        Return TPA
    End Function

    <Extension>
    Public Function GetFactor(TPAFactors As Dictionary(Of String, Double), ionName$, Optional default# = 1) As Double
        Dim factor#

        If TPAFactors.ContainsKey(ionName) Then
            factor = TPAFactors(ionName)

            ' factor列可能没有设置值，则加载之后会被默认转换为零
            ' 在这里将其设置为默认值1
            If factor = 0 Then
                factor = [default]
            End If
        Else
            ' 没有值的时候，默认是1，即不做处理
            factor = [default]
        End If

        Return factor
    End Function
End Module
