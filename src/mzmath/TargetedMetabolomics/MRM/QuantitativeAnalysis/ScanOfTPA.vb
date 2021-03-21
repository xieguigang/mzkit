#Region "Microsoft.VisualBasic::f9a28d6e276f60366fbae2a801c85f1e, src\mzmath\TargetedMetabolomics\MRM\QuantitativeAnalysis\ScanOfTPA.vb"

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

    '     Module ScanOfTPA
    ' 
    '         Function: GetFactor, ScanTPA
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports stdNum = System.Math

Namespace MRM

    Public Module ScanOfTPA

        ''' <summary>
        ''' 从一个原始文件之中扫描出给定的离子对的峰面积数据
        ''' </summary>
        ''' <param name="raw">``*.mzML``原始样本数据文件</param>
        ''' <param name="ionpairs"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ScanTPA(raw$, ionpairs As IonPair(), rtshifts As Dictionary(Of String, Double), args As MRMArguments) As IonTPA()
            ' 从原始文件之中读取出所有指定的离子对数据
            Dim ionData As IonChromatogram() = IonPair.GetIsomerism(ionpairs, args.tolerance) _
                .ExtractIonData(
                    mzML:=raw,
                    assignName:=Function(ion) ion.accession,
                    tolerance:=args.tolerance
                )

            If rtshifts Is Nothing Then
                rtshifts = New Dictionary(Of String, Double)
            End If

            For Each ion As IonChromatogram In ionData
                Dim shiftVal As Double = rtshifts.TryGetValue(ion.ion.target.accession)

                If stdNum.Abs(shiftVal) > args.timeWindowSize Then
                    ' required rt calibration
                    ion.chromatogram = ion.chromatogram _
                        .Select(Function(tick)
                                    Return New ChromatogramTick(tick.Time + shiftVal, tick.Intensity)
                                End Function) _
                        .ToArray
                End If
            Next

            ' 进行最大峰的查找，然后计算出净峰面积，用于回归建模
            Dim TPA As IonTPA() = ionData _
                .Select(Function(ion)
                            Dim factorVal As Double = args.TPAFactors.GetFactor(ion.name)
                            Dim result As IonTPA = ion.ionTPA(factorVal, args)

                            Return result
                        End Function) _
                .ToArray

            Return TPA
        End Function

        <Extension>
        Public Function GetFactor(TPAFactors As Dictionary(Of String, Double), ionName$, Optional default# = 1) As Double
            Dim factor#

            If Not TPAFactors Is Nothing AndAlso TPAFactors.ContainsKey(ionName) Then
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

End Namespace
