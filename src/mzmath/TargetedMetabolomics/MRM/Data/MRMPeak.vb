
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Namespace MRM.Data

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