#Region "Microsoft.VisualBasic::2dfb490dc51440cb9cbe0aaebd0616ad, src\mzmath\TargetedMetabolomics\MRM\QuantitativeAnalysis\StandardCurveWorker.vb"

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

'     Module StandardCurveWorker
' 
'         Function: CreateModelPoints, getBlankControls, getByLevel, getIS, Regression
'                   (+2 Overloads) Scan
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Quantile
Imports regexp = System.Text.RegularExpressions.Regex
Imports stdNum = System.Math

Namespace MRM

    ''' <summary>
    ''' 对当前批次的标准曲线进行回归建模
    ''' </summary>
    Public Module StandardCurveWorker

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

        <Extension>
        Private Function getBlankControls(blanks As DataSet) As Double()
            If blanks Is Nothing OrElse blanks.Properties.Count = 0 Then
                Return {}
            Else
                Dim baseline As Double() = blanks.Properties.Values.ToArray
                ' removes outlier by quantile
                Dim q As DataQuartile = baseline.Quartile

                Return q.Outlier(samples:=baseline).normal
            End If
        End Function

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
                                            Optional blankControls As DataSet() = Nothing,
                                            Optional maxDeletions As Integer = 1,
                                            Optional isWorkCurveMode As Boolean = True) As IEnumerable(Of StandardCurve)

            Dim [IS] As Dictionary(Of String, [IS]) = ISvector.SafeQuery.ToDictionary(Function(i) i.ID)
            Dim blanks As New Dictionary(Of String, DataSet)

            If Not blankControls.IsNullOrEmpty Then
                blanks = blankControls.ToDictionary(Function(metabolite) metabolite.ID)
            End If

            For Each ion As Standards In calibrates
                ' 20181106 如果没有内标，则不进行内标校正
                ' 所以在这里移除下面的where筛选
                ' .Where(Function(i)
                '            Return Not i.IS.StringEmpty AndAlso
                '                   Not ionTPA(i.IS) Is Nothing AndAlso
                '                   Not ionTPA(i.IS).Properties.Count < i.C.Count ' 标曲文件之中只有7个点，但是实际上打了10个点，剩下的三个点可以不要了
                '        End Function)

                Dim TPA = ionTPA(ion.ID).Properties.ToLower                       ' 得到标准曲线实验数据
                Dim ISA = ionTPA.getIS(ion)                                       ' 得到内标的实验数据，如果是空值的话，说明不需要内标进行校正
                Dim IsIon As [IS] = [IS].TryGetValue(ion.IS, [default]:=New [IS]) ' 尝试得到内标的数据
                Dim CIS# = CDbl(IsIon?.CIS)                                       ' 内标的浓度，是不变的，所以就只有一个值
                Dim points As New List(Of MRMStandards)
                Dim blankPoints = blanks.TryGetValue(ion.ID).getBlankControls
                Dim blankISPoints = blanks.TryGetValue(ion.IS).getBlankControls

                ' 标准曲线数据
                ' 从实验数据之中产生线性回归计算所需要的点的集合
                ' 注意，这些点之间是具有先后顺序的
                Dim rawLevels = ion.C _
                    .OrderBy(Function(l)
                                 ' 按照标曲编号从小到大进行排序
                                 Return Val(l.Key.Match("\d+"))
                             End Function) _
                    .ToArray

                Dim C = rawLevels.Select(Function(L) L.Value).ToArray
                Dim A = rawLevels.Select(TPA.getByLevel).ToArray
                Dim ISTPA As Double()

                If ISA Is Nothing Then
                    ISTPA = {}
                Else
                    ISTPA = rawLevels _
                        .Select(ISA.getByLevel) _
                        .ToArray
                End If

                Dim line As PointF()
                Dim fit As IFitted
                Dim invalids As New List(Of PointF)

                If blankPoints.Length > 0 Then
                    Dim baseline = blankPoints.Average
                    Dim nA As Double()

                    If blankISPoints.IsNullOrEmpty OrElse Not isWorkCurveMode Then
                        nA = A.Select(Function(xa) xa - baseline).ToArray
                    Else
                        Dim blankISBase# = blankISPoints.Average

                        ISTPA = {}
                        nA = A _
                            .Select(Function(xa, i) xa / ISTPA(i) - baseline / blankISBase) _
                            .ToArray
                    End If

                    line = StandardCurveWorker _
                       .CreateModelPoints(C, nA, ISTPA, CIS, ion.ID, ion.Name, points) _
                       .ToArray
                    fit = StandardCurve.CreateLinearRegression(line, maxDeletions, removed:=invalids)
                Else
                    line = StandardCurveWorker _
                       .CreateModelPoints(C, A, ISTPA, CIS, ion.ID, ion.Name, points) _
                       .ToArray
                    fit = StandardCurve.CreateLinearRegression(line, maxDeletions, removed:=invalids)
                End If

                If fit Is Nothing Then
                    Call $"Missing {ion.ToString}!".Warning
                    Continue For
                Else
                    ' get points that removed from linear modelling
                    For Each ptRef As MRMStandards In points
                        For Each invalid In invalids
                            If stdNum.Abs(invalid.X - ptRef.Cti) <= 0.0001 AndAlso stdNum.Abs(invalid.Y - ptRef.Px) <= 0.0001 Then
                                ptRef.valid = False
                                Exit For
                            End If
                        Next
                    Next
                End If

                Dim out As New StandardCurve With {
                    .name = ion.ID,
                    .linear = fit,
                    .points = points.PopAll,
                    .[IS] = IsIon
                }
                Dim fy As Func(Of Double, Double) = out.reverseModel
                Dim ptY#

                For Each pt As MRMStandards In out.points
                    If pt.AIS > 0 Then
                        ptY = pt.Ati / pt.AIS
                    Else
                        ptY = pt.Ati
                    End If

                    pt.yfit = stdNum.Round(fy(ptY), 5)
                Next

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
            Dim getLevelNumber = Function(level As String) As String
                                     Return regexp.Replace(level, "^\d+[-]", "", RegexICMul) _
                                        .Trim("-"c, " "c) _
                                        .Matches("\d+") _
                                        .Last
                                 End Function
            Dim refPoints = ref _
                .ToDictionary(Function(level)
                                  Return "L" & getLevelNumber(level.Key)
                              End Function,
                              Function(level)
                                  Return level.Value
                              End Function)

            Return Function(L)
                       Dim key As String = "L" & getLevelNumber(L.Key)

                       If ref.Count = 0 Then
                           Return 0
                       ElseIf Not refPoints.ContainsKey(key) Then
                           Throw New MissingPrimaryKeyException($"Missing reference point '{key}', or you can just delete the reference point from table file before you run the quantify script!")
                       Else
                           Dim At_i = refPoints(key)

                           Return At_i
                       End If
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
                points += New MRMStandards With {
                    .AIS = AIS,
                    .Ati = At_i,
                    .cIS = CIS,
                    .Cti = Ct_i,
                    .ID = id,
                    .Name = name,
                    .level = "L" & (i + 1),
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

        ''' <summary>
        ''' 从原始数据之中扫描峰面积数据，返回来的数据集之中的<see cref="DataSet.ID"/>是HMDB代谢物编号
        ''' </summary>
        ''' <param name="raw">``*.wiff``，转换之后的结果文件夹，其中标准曲线的数据都是默认使用``L数字``标记的。</param>
        ''' <param name="ions">包括离子对的定义数据以及浓度区间</param>
        ''' <returns></returns>
        Public Function Scan(raw$, ions As IonPair(), args As MRMArguments,
                             Optional ByRef refName$() = Nothing,
                             Optional calibrationNamedPattern$ = ".+[-]CAL\d+",
                             Optional levelPattern$ = "[-]CAL\d+",
                             Optional rtshifts As RTAlignment() = Nothing) As DataSet()

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
                Return mzMLRawFiles.Scan(
                    ions:=ions,
                    refName:=refName,
                    levelPattern:=levelPattern,
                    rtshifts:=rtshifts,
                    args:=args
                )
            End If
        End Function

        ''' <summary>
        ''' 从原始数据之中扫描峰面积数据，返回来的数据集之中的<see cref="DataSet.ID"/>是HMDB代谢物编号
        ''' </summary>
        ''' <param name="mzMLRawFiles">``*.wiff``，转换之后的结果文件夹，其中标准曲线的数据都是默认使用``L数字``标记的。</param>
        ''' <param name="ions">包括离子对的定义数据以及浓度区间</param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function Scan(mzMLRawFiles$(),
                             ions As IonPair(),
                             rtshifts As RTAlignment(),
                             args As MRMArguments,
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
                .Scan(mzMLRawFiles:=mzMLRawFiles,
                      ions:=ions,
                      refName:=refName,
                      removesWiffName:=False,
                      rtshifts:=rtshifts,
                      args:=args
                 ) _
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
End Namespace
