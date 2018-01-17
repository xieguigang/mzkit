Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace mzXML

    <XmlType("msRun")> Public Class MSData

        <XmlAttribute> Public Property scanCount As Integer
        <XmlAttribute> Public Property startTime As String
        <XmlAttribute> Public Property endTime As String

        Public Property parentFile As parentFile
        Public Property msInstrument As msInstrument
        Public Property dataProcessing As dataProcessing

        <XmlElement("scan")>
        Public Property scans As scan()

    End Class

    Public Class scan

        ''' <summary>
        ''' The scan number
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property num As Integer
        <XmlAttribute> Public Property scanType As String
        <XmlAttribute> Public Property centroided As String
        <XmlAttribute> Public Property msLevel As String
        <XmlAttribute> Public Property peaksCount As Integer
        <XmlAttribute> Public Property polarity As String
        <XmlAttribute> Public Property retentionTime As String
        <XmlAttribute> Public Property basePeakMz As Double
        <XmlAttribute> Public Property basePeakIntensity As Double
        <XmlAttribute> Public Property totIonCurrent As Double
        <XmlAttribute> Public Property collisionEnergy As String
        <XmlAttribute> Public Property lowMz As Double
        <XmlAttribute> Public Property highMz As Double
        <XmlAttribute> Public Property msInstrumentID As String

        Public Property precursorMz As precursorMz
        Public Property peaks As peaks

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class

    Public Class peaks : Implements IBase64Container

        <XmlAttribute> Public Property compressionType As String
        <XmlAttribute> Public Property compressedLen As Integer
        <XmlAttribute> Public Property precision As Double
        <XmlAttribute> Public Property byteOrder As String
        <XmlAttribute> Public Property contentType As String

        <XmlText>
        Public Property value As String Implements IBase64Container.BinaryArray

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Interface IBase64Container
        Property BinaryArray As String
    End Interface

    Public Structure precursorMz

        <XmlAttribute> Public Property windowWideness As String
        <XmlAttribute> Public Property precursorCharge As Double
        ''' <summary>
        ''' 母离子可以从这个属性指向的ms1 scan获取，这个属性对应着<see cref="scan.num"/>属性
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property precursorScanNum As String
        <XmlAttribute> Public Property precursorIntensity As Double
        <XmlAttribute> Public Property activationMethod As String
        <XmlText>
        Public Property value As Double

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace
