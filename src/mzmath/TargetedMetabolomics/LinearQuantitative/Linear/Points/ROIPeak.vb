#Region "Microsoft.VisualBasic::0908a1abf1250420c3bcc4812eb578c1, src\mzmath\TargetedMetabolomics\LinearQuantitative\Linear\Points\ROIPeak.vb"

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

    '     Class ROIPeak
    ' 
    '         Properties: base, peakHeight, ticks, window
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Namespace LinearQuantitative.Linear

    ''' <summary>
    ''' 可以通过这个对象计算出峰面积积分
    ''' </summary>
    Public Class ROIPeak

        ''' <summary>
        ''' Time range or peak width
        ''' </summary>
        ''' <returns></returns>
        Public Property window As DoubleRange

        <XmlAttribute("height")>
        Public Property peakHeight As Double

        <XmlAttribute("baseline")>
        Public Property base As Double

        ''' <summary>
        ''' 在<see cref="window"/>范围内的色谱信号数据
        ''' </summary>
        ''' <returns></returns>
        <XmlArray("ticks")>
        Public Property ticks As ChromatogramTick()

        Public Overrides Function ToString() As String
            Return $"{ticks.Sum(Function(t) t.Intensity)} @ [{window.Min}, {window.Max}]"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Narrowing Operator CType(ROI As ROIPeak) As DoubleRange
            Return ROI.window
        End Operator

    End Class
End Namespace
