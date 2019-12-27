#Region "Microsoft.VisualBasic::c50e993dafadb844e153dc23d46f1932, DATA\TargetedMetabolomics\GCMS\QuantifyAnalysis\StandardCurve.vb"

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

    '     Module StandardCurve
    ' 
    '         Function: LinearRegression, LoadStandardCurve
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.MassSpectrum.Math.MRM.Models

Namespace GCMS.QuantifyAnalysis

    Public Module StandardCurve

        ''' <summary>
        ''' 选取所传递进来的峰面积作为X轴，tuple第一项目的浓度值作为Y轴结果构建线性回归模型
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <returns></returns>
        <Extension>
        Public Function LinearRegression(raw As (content#, data As ChromatographyPeaktable)()) As IFitted
            Dim line As PointF() = raw _
                .OrderBy(Function(p)
                             ' 从小到大进行排序
                             Return p.content
                         End Function) _
                .Select(Function(p)
                            Return New PointF(p.data.TPACalibration, p.content)
                        End Function) _
                .ToArray
            Dim model As IFitted = FitModel.CreateLinearRegression(line, True)
            Return model
        End Function

        ''' <summary>
        ''' 因为浓度值之类的赋值不太通用，所以在这里写下标准曲线的构建方法
        ''' 在这里浓度值是被直接标记在文件名之中的
        ''' </summary>
        ''' <param name="dir"></param>
        ''' <param name="standards">
        ''' 用于扫描物质的
        ''' </param>
        ''' <param name="scale">
        ''' 所有的浓度值都默认被缩放到了``ppb``单位
        ''' </param>
        ''' <param name="IS">
        ''' 内标列表
        ''' </param>
        ''' <returns></returns>
        Public Function LoadStandardCurve(dir$, standards As ROITable(), [IS] As [IS](),
                                          Optional scale As ContentUnits = ContentUnits.ppb,
                                          Optional angleThreshold# = 5,
                                          Optional snThreshold As Double = 1) As (peaktable As ChromatographyPeaktable(), FitModel())

            ' 默认这个dir文件夹之中所有的cdf文件都是标准品参考曲线的结果文件
            Dim contentFiles As Dictionary(Of String, UnitValue(Of ContentUnits)) =
                dir.EnumerateFiles("*.cdf") _
                   .ToDictionary(Function(path) path,
                                 Function(path)
                                     Return path.BaseName _
                                         .ParseContent _
                                         .ScaleTo(scale)
                                 End Function)
            Dim TPA As New Dictionary(Of String, List(Of (content#, ROI As ChromatographyPeaktable)))
            Dim peaktable As New List(Of ChromatographyPeaktable)
            Dim ISTable As Dictionary(Of String, [IS]) = [IS].ToDictionary(Function(i) i.ID)

            For Each target As ROITable In standards
                TPA.Add(target.ID, New List(Of (content As Double, ROI As ChromatographyPeaktable)))
            Next

            ' 循环遍历每一个标准品文件
            For Each contentFile In contentFiles
                Dim content As UnitValue(Of ContentUnits) = contentFile.Value
                Dim cdf As String = contentFile.Key
                ' 这个物质的浓度都是一样的
                ' 主要是需要从原始数据之中提取出峰面积等计算信息
                Dim data As ROITable() = ScanModeWorker.ScanContents(
                    ref:=standards,
                    experiments:=cdf,
                    angle:=angleThreshold,
                    sn_threshold:=snThreshold
                )
                Dim rawFile$ = cdf.FileName
                Dim rawTPATable = data.ToDictionary(Function(d) d.ID)

                ' 循环标准品文件之中的每一种所检测到的物质
                For Each target As ROITable In data
                    Dim targetIS = ISTable.TryGetValue(target.IS)
                    Dim calibration#

                    If Not targetIS Is Nothing Then
                        ' TPA / IS_TPA
                        calibration = target.integration / rawTPATable(target.IS).integration
                    Else
                        ' 没有内标做校正
                        calibration = target.integration
                    End If

                    peaktable += New ChromatographyPeaktable With {
                        .baseline = target.baseline,
                        .content = content.Value,
                        .ID = target.ID,
                        .integration = target.integration,
                        .mass_spectra = target.mass_spectra,
                        .maxInto = target.maxInto,
                        .ri = target.ri,
                        .rt = target.rt,
                        .rtmax = target.rtmax,
                        .rtmin = target.rtmin,
                        .sn = target.sn,
                        .rawFile = rawFile,
                        .[IS] = target.IS,
                        .TPACalibration = calibration
                    }

                    TPA(target.ID).Add((content.Value, peaktable.Last))
                Next
            Next

            ' 将峰面积和浓度数据之间建立线型关系
            Dim standardsTable = standards.ToDictionary(Function(s) s.ID)
            Dim lines As FitModel() = TPA _
                .Where(Function(ion) Not ISTable.ContainsKey(ion.Key)) _
                .Select(Function(target)
                            Dim name$ = target.Key
                            Dim curve = target.Value.ToArray.LinearRegression
                            Dim ISName$ = standardsTable(name).IS

                            Return New FitModel With {
                                .Name = name,
                                .LinearRegression = curve,
                                .[IS] = ISTable.TryGetValue(ISName)
                            }
                        End Function) _
                .ToArray

            Return (peaktable.ToArray, lines)
        End Function
    End Module
End Namespace
