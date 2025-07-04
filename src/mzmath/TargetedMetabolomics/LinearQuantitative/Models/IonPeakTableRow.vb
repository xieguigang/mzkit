#Region "Microsoft.VisualBasic::5aca48bfef023b70511da622ef980d55, mzmath\TargetedMetabolomics\LinearQuantitative\Models\IonPeakTableRow.vb"

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

    '   Total Lines: 99
    '    Code Lines: 40 (40.40%)
    ' Comment Lines: 51 (51.52%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 8 (8.08%)
    '     File Size: 3.04 KB


    '     Class IonPeakTableRow
    ' 
    '         Properties: [IS], base, content, ID, maxinto
    '                     maxinto_IS, Name, raw, rtmax, rtmin
    '                     TPA, TPA_IS
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection

Namespace LinearQuantitative

    ''' <summary>
    ''' the peaktable data for a single ion
    ''' </summary>
    Public Class IonPeakTableRow : Implements IROI, INamedValue

        ''' <summary>
        ''' 标志物物质编号
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property ID As String Implements INamedValue.Key
        ''' <summary>
        ''' 通用名称
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String
        ''' <summary>
        ''' 保留时间的下限
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property rtmin As Double Implements IROI.rtmin
        ''' <summary>
        ''' 保留时间的上限
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property rtmax As Double Implements IROI.rtmax
        ''' <summary>
        ''' 浓度
        ''' </summary>
        ''' <returns></returns>
        Public Property content As Double
        ''' <summary>
        ''' 最大的信号响应值
        ''' </summary>
        ''' <returns></returns>
        Public Property maxinto As Double
        ''' <summary>
        ''' 内标的最大信号响应值
        ''' </summary>
        ''' <returns></returns>
        Public Property maxinto_IS As Double

        ''' <summary>
        ''' 峰面积
        ''' </summary>
        ''' <returns></returns>
        Public Property TPA As Double
        ''' <summary>
        ''' 内标的峰面积
        ''' </summary>
        ''' <returns></returns>
        Public Property TPA_IS As Double
        ''' <summary>
        ''' 内标编号
        ''' </summary>
        ''' <returns></returns>
        <Column("IS")>
        Public Property [IS] As String
        ''' <summary>
        ''' 信号基线水平
        ''' </summary>
        ''' <returns></returns>
        Public Property base As Double
        ''' <summary>
        ''' 实验数据的原始文件名
        ''' </summary>
        ''' <returns></returns>
        Public Property raw As String

        Sub New()
        End Sub

        Sub New(clone As IonPeakTableRow)
            ID = clone.ID
            Name = clone.Name
            rtmin = clone.rtmin
            rtmax = clone.rtmax
            content = clone.content
            maxinto = clone.maxinto
            maxinto_IS = clone.maxinto_IS
            TPA = clone.TPA
            TPA_IS = clone.TPA_IS
            [IS] = clone.IS
            base = clone.base
            raw = clone.raw
        End Sub

        Public Overrides Function ToString() As String
            Return Name
        End Function

    End Class
End Namespace
