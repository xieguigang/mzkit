Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace MarkupData.mzXML

    Public Class dataProcessing

        <XmlElement("software")>
        Public Property softwares As software()
        Public Property processingOperation As software
        Public Property comment As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class

    Public Class msInstrument

        <XmlAttribute>
        Public Property msInstrumentID As String
        Public Property msManufacturer As CategoryValue
        Public Property msModel As CategoryValue
        Public Property software As software

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class

    Public Structure software

        <XmlAttribute> Public Property type As String
        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property version As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure

    Public Structure CategoryValue
        <XmlAttribute> Public Property category As String
        <XmlAttribute> Public Property value As String
    End Structure

    Public Structure parentFile

        <XmlAttribute> Public Property fileName As String
        <XmlAttribute> Public Property fileType As String
        <XmlAttribute> Public Property fileShal As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Structure

    Public Class index

        <XmlAttribute>
        Public Property name As String
        <XmlElement("offset")>
        Public Property offsets As offset()

    End Class

    Public Structure offset

        <XmlAttribute>
        Public Property id As Integer
        <XmlText>
        Public Property value As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure

End Namespace