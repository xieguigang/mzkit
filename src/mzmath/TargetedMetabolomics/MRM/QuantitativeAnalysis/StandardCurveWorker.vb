#Region "Microsoft.VisualBasic::5ce2771b53add01f7ad2e5f5d0c46e5a, DATA\TargetedMetabolomics\MRM\QuantitativeAnalysis\StandardCurve.vb"

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
'     Function: CreateModelPoints, getByLevel, Regression, (+2 Overloads) Scan, ScanContent
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
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
Public Module StandardCurveWorker

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
    Public Iterator Function ScanContent(model As StandardCurve(),
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
            .ToDictionary(Function(ion) ion.name)

        Dim names As Dictionary(Of String, IonPair) = ions.ToDictionary(Function(i) i.accession)
        Dim C#

        raw = raw.FileName

        ' 遍历得到的所有的标准曲线，进行样本之中的浓度的计算
        For Each metabolite As StandardCurve In model.Where(Function(m) TPA.ContainsKey(m.name))
            Dim AIS As New IonTPA  ' (ROI As DoubleRange, TPA#, baseline#, maxinto#)
            Dim X#
            ' 得到样品之中的峰面积
            Dim A = TPA(metabolite.name)

            If Not metabolite.requireISCalibration Then
                ' 不需要内标进行校正
                ' 则X轴的数据直接是代谢物的峰面积数据
                X = A.area
            Else
                ' 数据存在丢失
                If Not TPA.ContainsKey(metabolite.IS.ID) Then
                    Continue For
                Else
                    ' 得到与样品混在一起的内标的峰面积
                    ' X轴数据需要做内标校正
                    AIS = TPA(metabolite.IS.ID)
                    X# = A.area / AIS.area
                End If
            End If

            If metabolite.linear Is Nothing Then
                Call $"Missing metabolite {metabolite.name} in raw file!".Warning

                Continue For
            Else
                ' 利用峰面积比计算出浓度结果数据
                ' 然后通过X轴的数据就可以通过标准曲线的线性回归模型计算出浓度了
                C# = metabolite.linear(X)
            End If

            ' 这里的C是相当于 cIS/ct = C，则样品的浓度结果应该为 ct = cIS/C
            ' C = Val(info!cIS) / C

            Dim [IS] As IonPair = names.TryGetValue(metabolite.IS.ID)
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

            Yield New ContentResult(Of MRMPeakTable) With {
                .Name = metabolite.name,
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
                                        [ISvector] As [IS](),
                                        Optional weighted As Boolean = False) As IEnumerable(Of StandardCurve)

        Dim [IS] As Dictionary(Of String, [IS]) = ISvector.ToDictionary(Function(i) i.ID)

        For Each ion As Standards In calibrates
            ' 20181106 如果没有内标，则不进行内标校正
            ' 所以在这里移除下面的where筛选
            ' .Where(Function(i)
            '            Return Not i.IS.StringEmpty AndAlso
            '                   Not ionTPA(i.IS) Is Nothing AndAlso
            '                   Not ionTPA(i.IS).Properties.Count < i.C.Count ' 标曲文件之中只有7个点，但是实际上打了10个点，剩下的三个点可以不要了
            '        End Function)

            Dim TPA = ionTPA(ion.HMDB).Properties.ToLower                     ' 得到标准曲线实验数据
            Dim ISA = ionTPA.getIS(ion)                                       ' 得到内标的实验数据，如果是空值的话，说明不需要内标进行校正
            Dim IsIon As [IS] = [IS].TryGetValue(ion.IS, [default]:=New [IS]) ' 尝试得到内标的数据
            Dim CIS# = IsIon?.CIS                                             ' 内标的浓度，是不变的，所以就只有一个值
            Dim points As New List(Of MRMStandards)

            ' 标准曲线数据
            ' 从实验数据之中产生线性回归计算所需要的点的集合
            ' 注意，这些点之间是具有先后顺序的
            Dim rawLevels = ion _
                .C _
                .OrderBy(Function(l)
                             ' 按照标曲编号从小到大进行排序
                             Return Val(l.Key.Match("\d+"))
                         End Function) _
                .ToArray

            Dim C = rawLevels.Select(Function(L) L.Value).ToArray
            Dim A = rawLevels.Select(TPA.getByLevel).ToArray
            Dim ISTPA As Double()

            If ISA Is Nothing Then
                ISTPA = Nothing
            Else
                ISTPA = rawLevels _
                    .Select(ISA.getByLevel) _
                    .ToArray
            End If

            Dim line As PointF() = StandardCurveWorker _
                .CreateModelPoints(C, A, ISTPA, CIS, ion.HMDB, ion.Name, points) _
                .ToArray
            Dim fit As IFitted = StandardCurve.CreateLinearRegression(line, weighted)
            Dim out As New StandardCurve With {
                .name = ion.HMDB,
                .linear = fit,
                .points = points.ToArray,
                .[IS] = IsIon
            }

            Yield out
        Next
    End Function

    <Extension>
    Private Function getIS(ionTPA As Dictionary(Of DataSet), ion As Standards) As Dictionary(Of String, Double)
        If ion.IS.StringEmpty Then
            Return Nothing
        Else
            Return ionTPA(ion.IS).Properties.ToLower
        End If
    End Function

    ''' <summary>
    ''' 得到峰面积Ati
    ''' </summary>
    ''' <param name="ref"></param>
    ''' <returns></returns>
    <Extension>
    Private Function getByLevel(ref As Dictionary(Of String, Double)) As Func(Of KeyValuePair(Of String, Double), Double)
        Return Function(L)
                   Dim key As String = L.Key.ToLower
                   Dim At_i = ref(key)

                   Return At_i
               End Function
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
                                               Optional points As List(Of MRMStandards) = Nothing) As IEnumerable(Of PointF)
        Dim AIS#

        If points Is Nothing Then
            points = New List(Of MRMStandards)
        Else
            points *= 0
        End If

        If ISA.IsNullOrEmpty Then
            ISA = Nothing
        End If

        For i As Integer = 0 To C.Length - 1
            Dim Ct_i = C(i)
            Dim At_i = A(i)

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
            points += New MRMStandards With {
                .AIS = AIS,
                .Ati = At_i,
                .cIS = CIS,
                .Cti = Ct_i,
                .ID = id,
                .Name = name,
                .level = "L" & i
            }

            ' 得到标准曲线之中的一个点
            Yield New PointF(pX, pY)
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
                         Optional peakAreaMethod As PeakArea.Methods = PeakArea.Methods.NetPeakSum,
                         Optional TPAFactors As Dictionary(Of String, Double) = Nothing,
                         Optional ByRef refName$() = Nothing,
                         Optional calibrationNamedPattern$ = ".+[-]CAL\d+",
                         Optional levelPattern$ = "[-]CAL\d+") As DataSet()

        ' 扫描所有的符合命名规则要求的原始文件
        ' 假设这些符合命名规则的文件都是标准曲线文件
        Dim mzMLRawFiles As String() = (ls - l - r - "*.mzML" <= raw.ParentPath) _
            .Where(Function(path)
                       Return path _
                           .BaseName _
                           .IsPattern(calibrationNamedPattern, RegexICSng)
                   End Function) _
            .ToArray

        If mzMLRawFiles.IsNullOrEmpty Then
            Throw New InvalidExpressionException($"No mzML file could be match by given regexp patterns: '{calibrationNamedPattern}'")
        Else
            Return mzMLRawFiles.Scan(ions, peakAreaMethod, TPAFactors, refName, levelPattern)
        End If
    End Function

    ''' <summary>
    ''' 从原始数据之中扫描峰面积数据，返回来的数据集之中的<see cref="DataSet.ID"/>是HMDB代谢物编号
    ''' </summary>
    ''' <param name="mzMLRawFiles">``*.wiff``，转换之后的结果文件夹，其中标准曲线的数据都是默认使用``L数字``标记的。</param>
    ''' <param name="ions">包括离子对的定义数据以及浓度区间</param>
    ''' <param name="TPAFactors">
    ''' ``{<see cref="Standards.HMDB"/>, <see cref="Standards.Factor"/>}``，这个是为了计算亮氨酸和异亮氨酸这类无法被区分的物质的峰面积所需要的
    ''' </param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function Scan(mzMLRawFiles$(),
                         ions As IonPair(),
                         peakAreaMethod As PeakArea.Methods,
                         TPAFactors As Dictionary(Of String, Double),
                         Optional ByRef refName$() = Nothing,
                         Optional levelPattern$ = "[-]CAL\d+") As DataSet()

        Dim levelName As Func(Of KeyValuePair(Of String, Double), String) =
            Function(file) As String
                Return file.Key _
                    .Match(levelPattern, RegexICSng) _
                    .Trim("-"c) _
                    .ToUpper
            End Function

        ' get reference data
        Dim result As DataSet() = WiffRaw _
            .Scan(mzMLRawFiles, ions, peakAreaMethod, TPAFactors, refName, False) _
            .Select(Function(ion)
                        Return New DataSet With {
                            .ID = ion.ID,
                            .Properties = ion.Properties _
                                .ToDictionary(levelName,
                                              Function(file)
                                                  Return file.Value
                                              End Function)
                        }
                    End Function) _
            .ToArray

        Return result
    End Function
End Module
