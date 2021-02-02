#Region "Microsoft.VisualBasic::c14bbcf08416d1674ff70eef37c1e3b3, TargetedMetabolomics\MRM\QuantitativeAnalysis\Samples.vb"

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

    '     Structure IonChromatogram
    ' 
    '         Properties: chromatogram, description, ion, name
    ' 
    '         Function: ToString
    ' 
    '     Module MRMSamples
    ' 
    '         Function: ExtractIonData, QuantitativeAnalysis, SampleQuantify
    ' 
    '     Class QuantifyScan
    ' 
    '         Properties: MRMPeaks, quantify, rawX
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq

Namespace MRM

    Public Structure IonChromatogram

        Public Property name As String
        Public Property description As String
        Public Property chromatogram As ChromatogramTick()
        Public Property ion As IsomerismIonPairs

        Public Overrides Function ToString() As String
            Return name
        End Function

    End Structure

    Public Module MRMSamples

        ''' <summary>
        ''' 从mzML原始数据文件之中取出每一个离子对所对应的色谱数据
        ''' </summary>
        ''' <param name="ion_pairs"></param>
        ''' <param name="mzML"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' This function just read raw data
        ''' </remarks>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ExtractIonData(ion_pairs As IEnumerable(Of IsomerismIonPairs),
                                       mzML$,
                                       assignName As Func(Of IonPair, String),
                                       tolerance As Tolerance) As IonChromatogram()

            Return LoadChromatogramList(mzML) _
                .MRMSelector(ion_pairs, tolerance) _
                .Where(Function(ion) Not ion.chromatogram Is Nothing) _
                .Select(Function(ionData)
                            Dim ion As IonPair = ionData.ion.target
                            Dim note$ = ion.name & $" [{ion.precursor}/{ion.product}]"

                            Return New IonChromatogram With {
                                .name = assignName(ion),
                                .description = note,
                                .chromatogram = ionData.chromatogram.Ticks,
                                .ion = ionData.ion
                            }
                        End Function) _
                .ToArray
        End Function

        ''' <summary>
        ''' 默认将``-KB``和``-BLK``结尾的文件都判断为实验空白
        ''' </summary>
        ReadOnly defaultBlankNames As New [Default](Of Func(Of String, Boolean))(
            Function(basename)
                Return InStr(basename, "-KB") > 0 OrElse InStr(basename, "-BLK") > 0
            End Function)

        ''' <summary>
        ''' 通过标准曲线对样品进行定量结果数据的获取
        ''' 
        ''' 这个函数对参考标曲的大小写不敏感,只需要名称的pattern正确就可以正常工作
        ''' </summary>
        ''' <param name="wiff$"></param>
        ''' <param name="model">标准曲线线性回归模型</param>
        ''' <param name="X">标准曲线之中的``AIS/A``峰面积比数据，即线性回归模型之中的X样本点</param>
        ''' <param name="externalStandardsWiff">
        ''' 如果定量参考用的标准曲线不是和样本数据在一个批次内的，而是分别在一个外部wiff文件之中的话，
        ''' 可以对这个函数参数进行赋值
        ''' </param>
        ''' <param name="isBlank">
        ''' 默认将``-KB``和``-BLK``结尾的文件都判断为实验空白
        ''' </param>
        ''' <returns>经过定量计算得到的浓度数据</returns>
        Public Function QuantitativeAnalysis(wiff$, ions As IonPair(), calibrates As Standards(), [IS] As [IS](), args As MRMArguments,
                                             <Out> Optional ByRef model As StandardCurve() = Nothing,
                                             <Out> Optional ByRef standardPoints As NamedValue(Of ReferencePoint())() = Nothing,
                                             <Out> Optional ByRef X As List(Of DataSet) = Nothing,
                                             <Out> Optional ByRef peaktable As IonPeakTableRow() = Nothing,
                                             Optional calibrationNamedPattern$ = ".+[-]L\d+",
                                             Optional levelPattern$ = "[-]L\d+",
                                             Optional externalStandardsWiff$ = Nothing,
                                             Optional isBlank As Func(Of String, Boolean) = Nothing,
                                             Optional rtshifts As RTAlignment() = Nothing) As IEnumerable(Of DataSet)
            Dim standardNames$() = Nothing
            Dim TPAFactors = calibrates.ToDictionary(Function(ion) ion.ID, Function(ion) ion.Factor)
            ' 扫描标准曲线的样本，然后进行回归建模 
            Dim calWiffRaw$ = externalStandardsWiff Or wiff.AsDefault

            model = StandardCurveWorker _
                .Scan(calWiffRaw, ions,
                      refName:=standardNames,
                      calibrationNamedPattern:=calibrationNamedPattern,
                      levelPattern:=levelPattern,
                      rtshifts:=rtshifts,
                      args:=args
                ) _
                .ToDictionary _
                .Regression(calibrates, ISvector:=[IS]) _
                .ToArray

            X = New List(Of DataSet)
            isBlank = isBlank Or defaultBlankNames
            standardPoints = model _
                .Select(Function(i)
                            Return New NamedValue(Of ReferencePoint())(i.name, i.points)
                        End Function) _
                .ToArray

            Dim nameIndex As Index(Of String) = standardNames.Indexing
            Dim out As New List(Of DataSet)
            Dim mrmPeaktable As New List(Of IonPeakTableRow)
            Dim allSamples As List(Of String) = (ls - l - r - "*.mzML" <= wiff.ParentPath).AsList
            Dim scan As QuantifyScan

            If Not externalStandardsWiff.StringEmpty AndAlso externalStandardsWiff.ParentPath.DirectoryExists Then
                allSamples += (ls - l - r - "*.mzML" <= externalStandardsWiff.ParentPath)
            End If

            ' 在上面获取得到了目标物质的回归模型以及离子对信息
            ' 在这个循环之中扫描每一个原始文件，进行物质的浓度定量计算
            For Each file As String In allSamples _
                .Where(Function(path)
                           Dim basename$ = path.BaseName
                           Return Not basename Like nameIndex AndAlso Not isBlank(basename)
                       End Function)

                Call file.ToFileURL.__INFO_ECHO

                scan = model.SampleQuantify(
                    file:=file,
                    ions:=ions,
                    rtshifts:=New Dictionary(Of String, Double),
                    args:=args
                )
                mrmPeaktable += scan.MRMPeaks
                X += scan.rawX
                out += scan.quantify
            Next

            peaktable = mrmPeaktable

            Return out
        End Function

        <Extension>
        Public Function SampleQuantify(model As StandardCurve(), file$, ions As IonPair(), rtshifts As Dictionary(Of String, Double), args As MRMArguments) As QuantifyScan

            ' 使用离子对信息扫面当前的这个原始数据文件
            ' 得到峰面积等定量计算所需要的结果信息
            Dim result As ContentResult(Of IonPeakTableRow)() = model _
                .ScanContent(
                    raw:=file,
                    ions:=ions,
                    args:=args,
                    rtshifts:=rtshifts
                ) _
                .ToArray
            Dim MRMPeakTable As New List(Of IonPeakTableRow)

            For Each metabolite As ContentResult(Of IonPeakTableRow) In result
                MRMPeakTable += metabolite.Peaktable
            Next

            If result.Length = 0 Then
                Call $"[NO_DATA] {file.ToFileURL} found nothing!".Warning
                Return Nothing
            End If

            ' 这个是浓度结果数据
            Dim quantify As New DataSet With {
                .ID = file.BaseName,
                .Properties = result _
                    .ToDictionary(Function(i) i.Name,
                                    Function(i)
                                        Return i.Content
                                    End Function)
            }

            ' 这个是峰面积比 AIS/At 数据
            Dim X As New DataSet With {
                .ID = file.BaseName,
                .Properties = result _
                    .ToDictionary(Function(i) i.Name,
                                    Function(i)
                                        Return i.X
                                    End Function)
            }

            Return New QuantifyScan With {
                .MRMPeaks = MRMPeakTable,
                .quantify = quantify,
                .rawX = X
            }
        End Function

    End Module

    Public Class QuantifyScan

        Public Property MRMPeaks As IonPeakTableRow()
        ''' <summary>
        ''' 定量结果
        ''' </summary>
        ''' <returns></returns>
        Public Property quantify As DataSet
        ''' <summary>
        ''' 原始的峰面积数据
        ''' </summary>
        ''' <returns></returns>
        Public Property rawX As DataSet

    End Class
End Namespace
