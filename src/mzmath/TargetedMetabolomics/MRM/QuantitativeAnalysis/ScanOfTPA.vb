#Region "Microsoft.VisualBasic::f81b3bc3a53ee82f93ac2e491b7f45be, DATA\TargetedMetabolomics\MRM\QuantitativeAnalysis\ScanOfTPA.vb"

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

    ' Module ScanOfTPA
    ' 
    '     Function: GetFactor, ScanTPA
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzML
Imports SMRUCC.MassSpectrum.Math.MRM.Models

Public Module ScanOfTPA

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
                            Optional peakAreaMethod As PeakArea.Methods = Methods.Integrator) As IonTPA()

        ' 从原始文件之中读取出所有指定的离子对数据
        Dim ionData = ionpairs.ExtractIonData(
            mzML:=raw,
            assignName:=Function(ion) ion.accession
        )
        ' 进行最大峰的查找，然后计算出净峰面积，用于回归建模
        Dim TPA = ionData _
            .Select(Function(ion)
                        Return ion.ionTPA(
                            baselineQuantile,
                            peakAreaMethod,
                            integratorTicks,
                            TPAFactors.GetFactor(ion.name)
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

