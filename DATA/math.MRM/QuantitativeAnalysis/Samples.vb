Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzML
Imports SMRUCC.MassSpectrum.Math.MRM.Models

Public Module MRMSamples

    ''' <summary>
    ''' 通过标准曲线对样品进行定量结果数据的获取
    ''' </summary>
    ''' <param name="wiff$"></param>
    ''' <param name="model">标准曲线线性回归模型</param>
    ''' <param name="X">标准曲线之中的``AIS/A``峰面积比数据，即线性回归模型之中的X样本点</param>
    ''' <returns>经过定量计算得到的浓度数据</returns>
    Public Function QuantitativeAnalysis(wiff$, ions As IonPair(), coordinates As Coordinate(), [IS] As [IS](),
                                         <Out> Optional ByRef model As NamedValue(Of FitResult)() = Nothing,
                                         <Out> Optional ByRef X As List(Of DataSet) = Nothing,
                                         Optional calibrationNamedPattern$ = ".+[-]L\d+",
                                         Optional levelPattern$ = "[-]L\d+") As IEnumerable(Of DataSet)
        Dim standardNames$() = Nothing
        Dim detections As NamedValue(Of FitResult)() =
            StandardCurve _
            .Scan(wiff, ions, coordinates,
                  refName:=standardNames,
                  calibrationNamedPattern:=calibrationNamedPattern,
                  levelPattern:=levelPattern
            ) _
            .ToDictionary _
            .Regression(coordinates, ISvector:=[IS]) _
            .ToArray

        X = New List(Of DataSet)
        model = detections

        Dim nameIndex As Index(Of String) = standardNames.Indexing
        Dim out As New List(Of DataSet)

        ' 在上面获取得到了目标物质的回归模型以及离子对信息
        ' 在这个循环之中扫描每一个原始文件，进行物质的浓度定量计算
        For Each file As String In (ls - l - r - "*.mzML" <= wiff.ParentPath) _
            .Where(Function(path)
                       Dim basename$ = path.BaseName
                       Return Not basename.IsOneOfA(nameIndex) AndAlso
                                  InStr(basename, "-KB") = 0
                   End Function)

            Call file.ToFileURL.__INFO_ECHO

            Dim result = detections _
                .ScanContent(raw:=file, ions:=ions) _
                .ToArray

            If result.Length = 0 Then
                Call $"[NO_DATA] {file.ToFileURL} found nothing!".Warning
            Else
                ' 这个是浓度结果数据
                out += New DataSet With {
                    .ID = file.BaseName,
                    .Properties = result _
                        .ToDictionary(Function(i) i.Name,
                                      Function(i) i.Value)
                }
                ' 这个是峰面积比 AIS/At 数据
                X += New DataSet With {
                    .ID = file.BaseName,
                    .Properties = result _
                        .ToDictionary(Function(i) i.Name,
                                      Function(i) Val(i.Description))
                }
            End If
        Next

        Return out
    End Function
End Module
