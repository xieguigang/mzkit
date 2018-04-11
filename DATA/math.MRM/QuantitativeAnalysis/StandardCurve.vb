Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzML
Imports SMRUCC.MassSpectrum.Math.Chromatogram
Imports SMRUCC.MassSpectrum.Math.MRM
Imports SMRUCC.MassSpectrum.Math.MRM.Dumping
Imports SMRUCC.MassSpectrum.Math.MRM.Models

''' <summary>
''' 对当前批次的标准曲线进行回归建模
''' </summary>
Public Module StandardCurve

    ''' <summary>
    ''' 根据建立起来的线性回归模型进行样品数据的扫描，根据曲线的结果得到浓度数据
    ''' </summary>
    ''' <param name="model">X为峰面积</param>
    ''' <param name="raw$"></param>
    ''' <param name="ions"></param>
    ''' <returns><see cref="NamedValue(Of Double).Value"/>是指定的代谢物的浓度结果数据，<see cref="NamedValue(Of Double).Description"/>则是AIS/A的结果，即X轴的数据</returns>
    <Extension>
    Public Iterator Function ScanContent(model As NamedValue(Of IFitted)(), raw$, ions As IonPair(), peakAreaMethod As PeakArea.Methods) As IEnumerable(Of (MRMPeakTable, NamedValue(Of Double)))
        Dim baseline# = 0
        Dim TPA = raw _
            .ScanTPA(ionpairs:=ions, peakAreaMethod:=peakAreaMethod) _
            .ToDictionary(Function(ion) ion.Name,
                          Function(A) A.Value)
        Dim names = ions.ToDictionary(Function(i) i.AccID)

        raw = raw.FileName

        For Each metabolite In model.Where(Function(m) TPA.ContainsKey(m.Name))
            Dim info = metabolite.Description.LoadObject(Of Dictionary(Of String, String))
            Dim line = metabolite.Value    ' 该代谢物的线性回归模型

            If Not TPA.ContainsKey(info!IS) Then
                Continue For
            End If

            Dim A = TPA(metabolite.Name)     ' 得到样品之中的峰面积
            Dim AIS = TPA(info!IS)           ' 得到与样品混在一起的内标的峰面积
            Dim C = line(x:=AIS.TPA / A.TPA) ' 利用峰面积比计算出浓度结果数据

            ' 这里的C是相当于 cIS/ct = C，则样品的浓度结果应该为 ct = cIS/C
            C = Val(info!cIS) / C

            Dim [IS] = names(info!IS)
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
                .IS = $"{[IS].AccID} ({[IS].name})",
                .maxinto = A.maxinto,
                .maxinto_IS = AIS.maxinto
            }
            Dim result As New NamedValue(Of Double) With {
                .Name = metabolite.Name,
                .Value = C,
                .Description = AIS.TPA / A.TPA
            }

            Yield (peaktable, result)
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

    ''' <summary>
    ''' 根据扫描出来的TPA峰面积进行对标准曲线的回归建模
    ''' </summary>
    ''' <param name="ionTPA"></param>
    ''' <param name="coordinates"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function Regression(ionTPA As Dictionary(Of DataSet), coordinates As Standards(), [ISvector] As [IS]()) As IEnumerable(Of NamedValue(Of (IFitted, MRMStandards())))
        Dim [IS] As Dictionary(Of String, [IS]) = ISvector.ToDictionary(Function(i) i.ID)

        For Each ion As Standards In coordinates _
            .Where(Function(i)
                       Return Not i.IS.StringEmpty AndAlso
                              Not ionTPA(i.IS) Is Nothing AndAlso
                              Not ionTPA(i.IS).Properties.Count < i.C.Count ' 标曲文件之中只有7个点，但是实际上打了10个点，剩下的三个点可以不要了
                   End Function)

            Dim TPA As DataSet = ionTPA(ion.HMDB)  ' 得到标准曲线实验数据
            Dim ISA As DataSet = ionTPA(ion.IS)    ' 得到内标的实验数据
            Dim CIS# = [IS](ion.IS).CIS            ' 内标的浓度，是不变的，所以就只有一个值
            Dim points As New List(Of MRMStandards)

            ' 标准曲线数据
            Dim line As PointF() = ion _
                .C _
                .Select(Function(level)
                            Dim At_i = TPA(level.Key)   ' 得到峰面积Ati
                            Dim Ct_i = level.Value      ' 得到已知的浓度数据
                            Dim AIS = ISA(level.Key)    ' 内标的峰面积

                            ' X 为峰面积，这样子在后面计算的时候就可以直接将离子对的峰面积带入方程计算出浓度结果了
                            Dim pX = At_i / AIS
                            Dim pY = Ct_i / CIS

                            points += New MRMStandards With {
                                .AIS = AIS,
                                .Ati = At_i,
                                .cIS = CIS,
                                .Cti = Ct_i,
                                .ID = ion.HMDB,
                                .Name = ion.Name,
                                .level = level.Key
                            }

                            ' 得到标准曲线之中的一个点
                            Return New PointF(pX, pY)
                        End Function) _
                .ToArray

            ' 对标准曲线进行线性回归建模
            Dim X = line.X.AsVector
            Dim Y = line.Y.AsVector
            Dim W = 1 / X ^ 2
            Dim fit As WeightedFit = WeightedLinearRegression.Regress(X, Y, W, 1)
            Dim info As New Dictionary(Of String, String) From {
                {"IS", ion.IS},
                {"cIS", CIS}
            }
            Dim out As New NamedValue(Of (IFitted, MRMStandards())) With {
                .Name = ion.HMDB,
                .Value = (fit, points.ToArray),
                .Description = info.GetJson
            }

            Yield out
        Next
    End Function

    ''' <summary>
    ''' 从原始数据之中扫描峰面积数据，返回来的数据集之中的<see cref="DataSet.ID"/>是HMDB代谢物编号
    ''' </summary>
    ''' <param name="raw$">``*.wiff``，转换之后的结果文件夹，其中标准曲线的数据都是默认使用``L数字``标记的。</param>
    ''' <param name="ions$">包括离子对的定义数据以及浓度区间</param>
    ''' <returns></returns>
    Public Function Scan(raw$,
                         ions As IonPair(),
                         coordinates As Standards(),
                         peakAreaMethod As PeakArea.Methods,
                         Optional ByRef refName$() = Nothing,
                         Optional calibrationNamedPattern$ = ".+[-]L\d+",
                         Optional levelPattern$ = "[-]L\d+") As DataSet()

        Dim rawName$ = raw.BaseName
        Dim ionTPAs As New Dictionary(Of String, Dictionary(Of String, Double))
        Dim refNames As New List(Of String)

        For Each ion As IonPair In ions
            ionTPAs(ion.AccID) = New Dictionary(Of String, Double)
        Next

        For Each file As String In (ls - l - r - "*.mzML" <= raw.ParentPath) _
            .Where(Function(path)
                       Return path _
                           .BaseName _
                           .IsPattern(calibrationNamedPattern, RegexICSng)
                   End Function)

            Dim TPA() = file.ScanTPA(
                ionpairs:=ions,
                peakAreaMethod:=peakAreaMethod
            )
            Dim level$ = file.BaseName _
                             .Match(levelPattern, RegexICSng) _
                             .Trim("-"c)

            refNames += file.BaseName

            ' level = level.Match("[-]L\d+", RegexICSng).Trim("-"c)

            For Each ion In TPA
                ionTPAs(ion.Name).Add(level, ion.Value.TPA)
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
    ''' <param name="raw$"></param>
    ''' <param name="ionpairs"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ScanTPA(raw$, ionpairs As IonPair(),
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
                        Dim vector As IVector(Of ChromatogramTick) = ion.Value.Shadows
                        Dim peak As DoubleRange = vector _
                            .PopulateROI _
                            .OrderByDescending(Function(ROI) ROI.Integration) _
                            .First _
                            .Time ' .MRMPeak(baselineQuantile:=baselineQuantile)

                        Dim area#
                        Dim baseline# = vector.Baseline(quantile:=baselineQuantile)

                        Select Case peakAreaMethod
                            Case Methods.NetPeakSum
                                area = vector.PeakArea(peak, baseline:=baselineQuantile)
                            Case Methods.SumAll
                                area = vector.SumAll
                            Case Methods.MaxPeakHeight
                                area = vector.MaxPeakHeight
                            Case Else
                                ' 默认是使用积分器方法
                                area = vector.PeakAreaIntegrator(
                                    peak:=peak,
                                    baselineQuantile:=baselineQuantile,
                                    n:=integratorTicks
                                )
                        End Select

                        Return New NamedValue(Of (DoubleRange, Double, Double, Double)) With {
                            .Name = ion.Name,
                            .Value = (peak, area, baseline, vector.MaxPeakHeight)
                        }
                    End Function) _
            .ToArray

        Return TPA
    End Function
End Module
