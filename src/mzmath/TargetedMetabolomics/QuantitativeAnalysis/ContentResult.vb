#Region "Microsoft.VisualBasic::50e61c39021f5bff057de8faf3402c2c, src\mzmath\TargetedMetabolomics\QuantitativeAnalysis\ContentResult.vb"

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

    ' Class ContentResult
    ' 
    '     Properties: Content, Name, Peaktable, X
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

''' <summary>
''' 客户的实验样本数据之中的某一种目标代谢物质的浓度计算结果
''' </summary>
''' <typeparam name="T"></typeparam>
Public Class ContentResult(Of T As IROI) : Implements INamedValue

    Public Property Name As String Implements IKeyedEntity(Of String).Key
    Public Property Content As Double

    ''' <summary>
    ''' 目标物质的峰面积
    ''' </summary>
    ''' <returns></returns>
    Public Property Peaktable As T

    ''' <summary>
    ''' 是``AIS/A``的结果，即X轴的数据
    ''' </summary>
    ''' <returns></returns>
    Public Property X As Double

    Public Overrides Function ToString() As String
        Return $"Dim {Name} As [{Peaktable.rtmin},{Peaktable.rtmax}] = {Content}"
    End Function
End Class
