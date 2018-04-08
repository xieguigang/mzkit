Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports SMRUCC.MassSpectrum.Math.Chromatogram
Imports SMRUCC.MassSpectrum.Math.MRM.Models

Namespace Dumping

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

    ''' <summary>
    ''' 可以通过这个quantile分布对象来了解基线数据是否计算正确
    ''' </summary>
    Public Class Quantile

        ''' <summary>
        ''' Quantile value in this <see cref="Percentage"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property Quantile As Double
        ''' <summary>
        ''' [0, 1] quantile percentage
        ''' </summary>
        ''' <returns></returns>
        Public Property Percentage As Double

        Public Overrides Function ToString() As String
            Return $"{Quantile} @ {Percentage * 100}%"
        End Function
    End Class
End Namespace