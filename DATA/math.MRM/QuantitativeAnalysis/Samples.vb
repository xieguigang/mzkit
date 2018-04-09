Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzML
Imports SMRUCC.MassSpectrum.Math.Chromatogram
Imports SMRUCC.MassSpectrum.Math.MRM.Dumping
Imports SMRUCC.MassSpectrum.Math.MRM.Models

Public Module MRMSamples

    ''' <summary>
    ''' 从mzML原始数据文件之中取出每一个离子对所对应的色谱数据
    ''' </summary>
    ''' <param name="ion_pairs"></param>
    ''' <param name="mzML$"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function ExtractIonData(ion_pairs As IonPair(), mzML$, assignName As Func(Of IonPair, String)) As NamedCollection(Of ChromatogramTick)()
        Return LoadChromatogramList(mzML) _
            .MRMSelector(ion_pairs) _
            .Where(Function(ion) Not ion.chromatogram Is Nothing) _
            .Select(Function(ionData)
                        Dim ion = ionData.ion
                        Return New NamedCollection(Of ChromatogramTick) With {
                            .Name = assignName(ion),
                            .Description = ion.name & $" [{ion.precursor}/{ion.product}]",
                            .Value = ionData.chromatogram.Ticks
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' 通过标准曲线对样品进行定量结果数据的获取
    ''' </summary>
    ''' <param name="wiff$"></param>
    ''' <param name="model">标准曲线线性回归模型</param>
    ''' <param name="X">标准曲线之中的``AIS/A``峰面积比数据，即线性回归模型之中的X样本点</param>
    ''' <returns>经过定量计算得到的浓度数据</returns>
    Public Function QuantitativeAnalysis(wiff$, ions As IonPair(), coordinates As Standards(), [IS] As [IS](),
                                         <Out> Optional ByRef model As NamedValue(Of FitResult)() = Nothing,
                                         <Out> Optional ByRef standardPoints As NamedValue(Of MRMStandards())() = Nothing,
                                         <Out> Optional ByRef X As List(Of DataSet) = Nothing,
                                         <Out> Optional ByRef peaktable As MRMPeakTable() = Nothing,
                                         Optional calibrationNamedPattern$ = ".+[-]L\d+",
                                         Optional levelPattern$ = "[-]L\d+",
                                         Optional peakAreaMethod As PeakArea.Methods = Methods.NetPeakSum) As IEnumerable(Of DataSet)
        Dim standardNames$() = Nothing
        Dim detections As NamedValue(Of (FitResult, MRMStandards()))() =
            StandardCurve _
            .Scan(wiff, ions, coordinates,
                  refName:=standardNames,
                  calibrationNamedPattern:=calibrationNamedPattern,
                  levelPattern:=levelPattern,
                  peakAreaMethod:=peakAreaMethod
            ) _
            .ToDictionary _
            .Regression(coordinates, ISvector:=[IS]) _
            .ToArray

        X = New List(Of DataSet)
        model = detections _
            .Select(Function(i) New NamedValue(Of FitResult)(i.Name, i.Value.Item1, i.Description)) _
            .ToArray
        standardPoints = detections _
            .Select(Function(i)
                        Return New NamedValue(Of MRMStandards())(i.Name, i.Value.Item2)
                    End Function) _
            .ToArray

        Dim nameIndex As Index(Of String) = standardNames.Indexing
        Dim out As New List(Of DataSet)
        Dim mrmpeaktable As New List(Of MRMPeakTable)

        ' 在上面获取得到了目标物质的回归模型以及离子对信息
        ' 在这个循环之中扫描每一个原始文件，进行物质的浓度定量计算
        For Each file As String In (ls - l - r - "*.mzML" <= wiff.ParentPath) _
            .Where(Function(path)
                       Dim basename$ = path.BaseName
                       Return Not basename.IsOneOfA(nameIndex) AndAlso
                                  InStr(basename, "-KB") = 0
                   End Function)

            Call file.ToFileURL.__INFO_ECHO

            Dim result = model _
                .ScanContent(
                    raw:=file,
                    ions:=ions,
                    peakAreaMethod:=peakAreaMethod
                ) _
                .ToArray

            For Each metabolite In result
                mrmpeaktable += metabolite.Item1
            Next

            If result.Length = 0 Then
                Call $"[NO_DATA] {file.ToFileURL} found nothing!".Warning
            Else
                ' 这个是浓度结果数据
                out += New DataSet With {
                    .ID = file.BaseName,
                    .Properties = result _
                        .ToDictionary(Function(i) i.Item2.Name,
                                      Function(i) i.Item2.Value)
                }
                ' 这个是峰面积比 AIS/At 数据
                X += New DataSet With {
                    .ID = file.BaseName,
                    .Properties = result _
                        .ToDictionary(Function(i) i.Item2.Name,
                                      Function(i)
                                          Return Val(i.Item2.Description)
                                      End Function)
                }
            End If
        Next

        peaktable = mrmpeaktable

        Return out
    End Function
End Module
