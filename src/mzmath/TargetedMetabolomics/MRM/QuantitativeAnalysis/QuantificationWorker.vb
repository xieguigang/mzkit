#Region "Microsoft.VisualBasic::00842b44f0d81fcf952489d5a62915c9, TargetedMetabolomics\MRM\QuantitativeAnalysis\QuantificationWorker.vb"

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

'     Module QuantificationWorker
' 
'         Function: doLinearQuantify, reverseModel, ScanContent
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language

Namespace MRM

    Public Module QuantificationWorker

        ''' <summary>
        ''' 根据建立起来的线性回归模型进行样品数据的扫描，根据曲线的结果得到浓度数据
        ''' </summary>
        ''' <param name="linearModels">标准曲线线性回归模型，X为峰面积</param>
        ''' <param name="raw$"></param>
        ''' <param name="ions"></param>
        ''' <returns>
        ''' <see cref="NamedValue(Of Double).Value"/>是指定的代谢物的浓度结果数据，
        ''' <see cref="NamedValue(Of Double).Description"/>则是AIS/A的结果，即X轴的数据
        ''' </returns>
        <Extension>
        Public Iterator Function ScanContent(linearModels As StandardCurve(),
                                             raw$,
                                             ions As IonPair(),
                                             rtshifts As Dictionary(Of String, Double),
                                             args As MRMArguments) As IEnumerable(Of ContentResult(Of IonPeakTableRow))

            Dim TPA As Dictionary(Of String, IonTPA) = raw _
                .ScanTPA(ionpairs:=ions,
                         rtshifts:=rtshifts,
                         args:=args
                ) _
                .ToDictionary(Function(ion)
                                  Return ion.name
                              End Function)

            Dim names As Dictionary(Of String, String) = ions _
                .ToDictionary(Function(i) i.accession,
                              Function(i)
                                  Return i.name
                              End Function)

            ' model -> y = ax + b
            ' in_calculation -> x = (y-b)/a
            Dim quantifyLinears As StandardCurve() = linearModels _
                .Where(Function(m) TPA.ContainsKey(m.name)) _
                .ToArray
            Dim quantify As New Value(Of ContentResult(Of IonPeakTableRow))

            raw = raw.FileName

            ' 遍历得到的所有的标准曲线，进行样本之中的浓度的计算
            For Each metabolite In quantifyLinears
                If Not (quantify = TPA.doLinearQuantify(metabolite, names, raw)) Is Nothing Then
                    Yield quantify.Value
                End If
            Next
        End Function
    End Module
End Namespace
