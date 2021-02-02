
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