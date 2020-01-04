#Region "Microsoft.VisualBasic::48a48761611bd554a34966387a05141a, src\mzmath\TargetedMetabolomics\MRM\Data\TargetQuantification.vb"

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

    '     Class TargetQuantification
    ' 
    '         Properties: [IS], ChromatogramSummary, MRMPeak, Name, Standards
    ' 
    '         Function: ToString
    ' 
    '     Class MRMPeak
    ' 
    '         Properties: Base, PeakHeight, Ticks, Window
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports SMRUCC.MassSpectrum.Math.Chromatogram
Imports SMRUCC.MassSpectrum.Math.MRM.Models

Namespace MRM.Data

    ''' <summary>
    ''' Dump MRM target quantification result in XML format.
    ''' </summary>
    Public Class TargetQuantification

        Public Property Name As String
        Public Property [IS] As [IS]
        Public Property Standards As Standards
        Public Property MRMPeak As MRMPeak
        Public Property ChromatogramSummary As Quantile()

        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class

    ''' <summary>
    ''' 可以通过这个对象计算出峰面积积分
    ''' </summary>
    Public Class MRMPeak

        ''' <summary>
        ''' Time range or peak width
        ''' </summary>
        ''' <returns></returns>
        Public Property Window As DoubleRange
        <XmlAttribute("height")>
        Public Property PeakHeight As Double
        <XmlAttribute("baseline")>
        Public Property Base As Double

        ''' <summary>
        ''' 在<see cref="Window"/>范围内的色谱信号数据
        ''' </summary>
        ''' <returns></returns>
        <XmlArray("ticks")>
        Public Property Ticks As ChromatogramTick()

    End Class
End Namespace
