﻿#Region "Microsoft.VisualBasic::c83878935b5f440a226e01be40cd037e, mzmath\TargetedMetabolomics\MRM\QuantitativeAnalysis\Samples.vb"

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


    ' Code Statistics:

    '   Total Lines: 192
    '    Code Lines: 131 (68.23%)
    ' Comment Lines: 39 (20.31%)
    '    - Xml Docs: 84.62%
    ' 
    '   Blank Lines: 22 (11.46%)
    '     File Size: 9.13 KB


    '     Module MRMSamples
    ' 
    '         Function: (+2 Overloads) ExtractIonData, QuantitativeAnalysis, SampleQuantify
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.SignalReader
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq

Namespace MRM

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
                                .ion = ionData.ion,
                                .source = mzML.BaseName
                            }
                        End Function) _
                .ToArray
        End Function

        Public Function ExtractIonData(mzML As String, ion As IonPair, tolerance As Tolerance) As IonChromatogram
            Dim xicdata = LoadChromatogramList(mzML).MRMSelector(ion, tolerance)

            If xicdata Is Nothing Then
                Return Nothing
            End If

            Return New IonChromatogram With {
                .name = ion.name,
                .chromatogram = xicdata.Ticks,
                .description = ion.name & $" [{ion.precursor}/{ion.product}]",
                .ion = New IsomerismIonPairs(ion)
            }
        End Function

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
                mrmPeaktable += scan.ionPeaks
                X += scan.rawX
                out += scan.quantify
            Next

            peaktable = mrmPeaktable

            Return out
        End Function

        ''' <summary>
        ''' 对单个原始数据文件做定量计算
        ''' </summary>
        ''' <param name="model"></param>
        ''' <param name="file$"></param>
        ''' <param name="ions"></param>
        ''' <param name="rtshifts"></param>
        ''' <param name="args"></param>
        ''' <returns></returns>
        <Extension>
        Public Function SampleQuantify(model As StandardCurve(),
                                       file$,
                                       ions As IonPair(),
                                       rtshifts As Dictionary(Of String, Double),
                                       args As MRMArguments) As QuantifyScan

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
            Dim scanOut As QuantifyScan = result _
                .Where(Function(a) Not a Is Nothing) _
                .ToArray _
                .SampleQuantifyScan(file)

            Return scanOut
        End Function

    End Module
End Namespace
